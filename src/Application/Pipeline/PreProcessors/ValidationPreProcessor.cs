namespace CleanArchitecture.Blazor.Application.Pipeline.PreProcessors;

public sealed class ValidationPreProcessor<TRequest>(IEnumerable<IValidator<TRequest>> validators)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly IReadOnlyCollection<IValidator<TRequest>> _validators = validators.ToList() ?? throw new ArgumentNullException(nameof(validators));

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Count == 0) return;

        var validationContext = new ValidationContext<TRequest>(request);

        var failures = await _validators.ValidateAsync(validationContext, cancellationToken);

        if (failures.Count != 0) throw new ValidationException(failures);
    }
}