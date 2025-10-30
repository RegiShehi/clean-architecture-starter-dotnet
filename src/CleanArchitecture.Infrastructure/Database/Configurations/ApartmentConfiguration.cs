namespace CleanArchitecture.Infrastructure.Database.Configurations;

using Domain.Common;
using Domain.Features.Apartments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("Apartments");

        builder.HasKey(apartment => apartment.Id);

        builder.OwnsOne(apartment => apartment.Address);

        builder.OwnsOne(x => x.Name, o =>
        {
            o.Property(p => p.Value)
                .HasMaxLength(200)
                .HasColumnName("name")
                .IsRequired();

            o.WithOwner();
        });

        builder.OwnsOne(x => x.Description, o =>
        {
            o.Property(p => p.Value)
                .HasColumnName("description")
                .HasMaxLength(2000);

            o.WithOwner();
        });

        builder.OwnsOne(apartment => apartment.Price, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        builder.OwnsOne(apartment => apartment.CleaningFee, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });
    }
}
