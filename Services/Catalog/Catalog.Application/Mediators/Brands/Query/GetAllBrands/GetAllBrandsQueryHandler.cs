using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Mediators.Brands.Query.GetAllBrands;

public class GetAllBrandsQueryHandler(IBrandRepository brandRepository) : IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
{
    public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var brandList = await brandRepository.GetAllBrandsAsync();
        
        return brandList.ToResponseList();
    }
}