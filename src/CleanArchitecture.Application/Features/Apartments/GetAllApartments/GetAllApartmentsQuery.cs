namespace CleanArchitecture.Application.Features.Apartments.GetAllApartments;

using Abstractions.Messaging;

public sealed record GetAllApartmentsQuery : IQuery<List<ApartmentResponse>>;
