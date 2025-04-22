// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.PicklistSets.Caching;
using CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.AddEdit;

namespace CleanArchitecture.Blazor.Application.Features.PicklistSets.Commands.Import;

public class ImportPicklistSetsCommand(string fileName, byte[] data) : ICacheInvalidatorRequest<Result>
{
    public string FileName { get; set; } = fileName;
    public byte[] Data { get; set; } = data;
    public IEnumerable<string>? Tags => PicklistSetCacheKey.Tags;
}

 

public class ImportPicklistSetsCommandHandler(
    IApplicationDbContext context,
    IExcelService excelService,
    IStringLocalizer<ImportPicklistSetsCommandHandler> localizer,
    IValidator<AddEditPicklistSetCommand> addValidator)
    :
        IRequestHandler<ImportPicklistSetsCommand, Result>
{
#nullable disable warnings
    public async Task<Result> Handle(ImportPicklistSetsCommand request, CancellationToken cancellationToken)
    {
        var result = await excelService.ImportAsync(request.Data,
            new Dictionary<string, Func<DataRow, PicklistSet, object?>>
            {
                {
                    localizer["Name"],
                    (row, item) =>
                        item.Name = (Picklist)Enum.Parse(typeof(Picklist), row[localizer["Name"]].ToString())
                },
                { localizer["Value"], (row, item) => item.Value = row[localizer["Value"]]?.ToString() },
                { localizer["Text"], (row, item) => item.Text = row[localizer["Text"]]?.ToString() },
                {
                    localizer["Description"],
                    (row, item) => item.Description = row[localizer["Description"]]?.ToString()
                }
            }, localizer["Data"]);

        if (result is not { Succeeded: true, Data: not null }) return await Result.FailureAsync(result.Errors);
        {
            var importItems = result.Data;
            var errors = new List<string>();
            var errorsOccurred = false;
            foreach (var item in importItems)
            {
                var validationResult = await addValidator.ValidateAsync(
                    new AddEditPicklistSetCommand
                        { Name = item.Name, Value = item.Value, Description = item.Description, Text = item.Text },
                    cancellationToken);
                if (validationResult.IsValid)
                {
                    var exist = await context.PicklistSets.AnyAsync(x => x.Name == item.Name && x.Value == item.Value,
                        cancellationToken);
                    if (exist) continue;

                    item.AddDomainEvent(new CreatedEvent<PicklistSet>(item));
                    await context.PicklistSets.AddAsync(item, cancellationToken);
                }
                else
                {
                    errorsOccurred = true;
                    errors.AddRange(validationResult.Errors.Select(e =>
                        $"{(!string.IsNullOrWhiteSpace(item.Name.ToString()) ? $"{item.Name} - " : string.Empty)}{e.ErrorMessage}"));
                }
            }

            if (errorsOccurred) return await Result.FailureAsync(errors.ToArray());

            await context.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync();
        }
    }
}