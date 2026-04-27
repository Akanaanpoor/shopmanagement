using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Mediators.Brands.Query.GetAllBrands;

public record GetAllBrandsQuery : IRequest<IList<BrandResponse>>
{
}