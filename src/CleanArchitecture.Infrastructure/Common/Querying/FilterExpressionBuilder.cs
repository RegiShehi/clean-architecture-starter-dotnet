namespace CleanArchitecture.Infrastructure.Common.Querying;

using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using SharedKernel.Query;

internal static class FilterExpressionBuilder
{
    public static Expression<Func<T, bool>> BuildPredicate<T>(Filter filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var member = PropertyAccessorCache.GetMemberAccess(parameter, filter.Field);
        var type = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

        if (filter.Operator is FilterOperator.IsNull or FilterOperator.IsNotNull)
        {
            var nullConst = Expression.Constant(null, member.Type);

            var expr = filter.Operator == FilterOperator.IsNull
                ? Expression.Equal(member, nullConst)
                : Expression.NotEqual(member, nullConst);

            return Expression.Lambda<Func<T, bool>>(expr, parameter);
        }

        if (type == typeof(string) &&
            filter.Operator is FilterOperator.Contains or FilterOperator.StartsWith or FilterOperator.EndsWith)
        {
            var value = Expression.Constant((filter.Value ?? "").ToString()!);
            var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
            var left = Expression.Call(Expression.Convert(member, typeof(string)), toLower);
            var right = Expression.Call(value, toLower);

            var method = filter.Operator switch
            {
                FilterOperator.Contains => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                FilterOperator.StartsWith => typeof(string).GetMethod(nameof(string.StartsWith),
                    [typeof(string)])!,
                FilterOperator.EndsWith => typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!,
                _ => throw new NotSupportedException()
            };

            return Expression.Lambda<Func<T, bool>>(Expression.Call(left, method, right), parameter);
        }

        if (filter.Operator == FilterOperator.Between)
        {
            var lo = Expression.Constant(ConvertTo(filter.Value, type), type);
            var hi = Expression.Constant(ConvertTo(filter.Extra, type), type);
            var nn = ToNonNullable(member);
            var ge = Expression.GreaterThanOrEqual(nn, lo);
            var le = Expression.LessThanOrEqual(nn, hi);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(ge, le), parameter);
        }

        if (filter.Operator is FilterOperator.In or FilterOperator.NotIn)
        {
            var arr = (filter.Value as IEnumerable<object?> ?? [filter.Value]).Select(v => ConvertTo(v, type))
                .ToArray();
            var list = Expression.Constant(arr);
            var contains = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
                .MakeGenericMethod(type);
            var call = Expression.Call(contains, list, ToNonNullable(member));
            Expression expr = filter.Operator == FilterOperator.In ? call : Expression.Not(call);
            return Expression.Lambda<Func<T, bool>>(expr, parameter);
        }

        var val = Expression.Constant(ConvertTo(filter.Value, type), type);
        var leftNn = ToNonNullable(member);
        var cmp = filter.Operator switch
        {
            FilterOperator.Eq => Expression.Equal(leftNn, val),
            FilterOperator.Ne => Expression.NotEqual(leftNn, val),
            FilterOperator.Gt => Expression.GreaterThan(leftNn, val),
            FilterOperator.Ge => Expression.GreaterThanOrEqual(leftNn, val),
            FilterOperator.Lt => Expression.LessThan(leftNn, val),
            FilterOperator.Le => Expression.LessThanOrEqual(leftNn, val),
            _ => throw new NotSupportedException($"Operator {filter.Operator} not supported.")
        };

        return Expression.Lambda<Func<T, bool>>(cmp, parameter);
    }

    private static object? ConvertTo(object? value, Type type)
    {
        if (value is null)
        {
            return null;
        }

        if (value is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (je.ValueKind == JsonValueKind.Array)
            {
                throw new InvalidOperationException(
                    "Array provided to a scalar converter. Use In/NotIn with array handling.");
            }

            if (type == typeof(string))
            {
                return je.ValueKind == JsonValueKind.String ? je.GetString() : je.ToString();
            }


            if (type == typeof(Guid))
            {
                var s = je.ValueKind == JsonValueKind.String ? je.GetString() : je.ToString();
                return Guid.Parse(s!);
            }

            if (type == typeof(DateTime))
            {
                return DateTime.Parse(je.GetString()!, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            if (type == typeof(DateOnly))
            {
                return DateOnly.Parse(je.GetString()!, CultureInfo.InvariantCulture);
            }

            if (type == typeof(TimeOnly))
            {
                return TimeOnly.Parse(je.GetString()!, CultureInfo.InvariantCulture);
            }

            if (type == typeof(bool))
            {
                return je.GetBoolean();
            }

            // Numerics
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Decimal => je.TryGetDecimal(out var d)
                    ? d
                    : decimal.Parse(je.ToString(), CultureInfo.InvariantCulture),
                TypeCode.Double => je.TryGetDouble(out var db)
                    ? db
                    : double.Parse(je.ToString(), CultureInfo.InvariantCulture),
                TypeCode.Single => je.TryGetSingle(out var sg)
                    ? sg
                    : float.Parse(je.ToString(), CultureInfo.InvariantCulture),
                TypeCode.Int64 => je.TryGetInt64(out var i64)
                    ? i64
                    : long.Parse(je.ToString(), CultureInfo.InvariantCulture),
                TypeCode.Int32 => je.TryGetInt32(out var i32)
                    ? i32
                    : int.Parse(je.ToString(), CultureInfo.InvariantCulture),
                TypeCode.Int16 => short.Parse(je.ToString(), CultureInfo.InvariantCulture),
                TypeCode.Byte => byte.Parse(je.ToString(), CultureInfo.InvariantCulture),

                // // Enum: expect a string value (name) or numeric underlying value
                // if (type.IsEnum)
                // {
                //     if (je.ValueKind == JsonValueKind.String)
                //     {
                //         return Enum.Parse(type, je.GetString()!, true);
                //     }
                //
                //     // numeric enum
                //     var underlying = Enum.GetUnderlyingType(type);
                //     var num = ExtractNumberAsType(je, underlying);
                //     return Enum.ToObject(type, num!);
                // }
                _ => Convert.ChangeType(je.ToString(), type, CultureInfo.InvariantCulture)
            };
        }

        if (type.IsEnum)
        {
            return Enum.Parse(type, value.ToString()!, true);
        }

        if (type == typeof(Guid))
        {
            return Guid.Parse(value.ToString()!);
        }

        if (type == typeof(DateTime))
        {
            return DateTime.Parse(value.ToString()!, CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        if (type == typeof(DateOnly))
        {
            return DateOnly.Parse(value.ToString()!, CultureInfo.InvariantCulture);
        }

        if (type == typeof(TimeOnly))
        {
            return TimeOnly.Parse(value.ToString()!, CultureInfo.InvariantCulture);
        }

        return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }

    private static Expression ToNonNullable(Expression e) =>
        Nullable.GetUnderlyingType(e.Type) is { } u ? Expression.Convert(e, u) : e;
}
