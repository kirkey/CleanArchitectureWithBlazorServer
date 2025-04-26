// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using Microsoft.AspNetCore.Components.Forms;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public List<ProductImage>? Pictures { get; set; }

    public IReadOnlyList<IBrowserFile>? UploadPictures { get; set; }
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ProductDto, AddEditProductCommand>(MemberList.None);
            CreateMap<AddEditProductCommand, Product>(MemberList.None);
        }
    }
}

public class AddEditProductCommandHandler(
    IMapper mapper,
    IApplicationDbContext context) : IRequestHandler<AddEditProductCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddEditProductCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var item = await context.Products.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item == null) return await Result<int>.FailureAsync($"Product with id: [{request.Id}] not found.");
            item = mapper.Map(request, item);
            item.AddDomainEvent(new UpdatedEvent<Product>(item));
            await context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = mapper.Map<Product>(request);
            item.AddDomainEvent(new CreatedEvent<Product>(item));
            context.Products.Add(item);
            await context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}