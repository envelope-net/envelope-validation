namespace Envelope.Validation.Results;

internal class ValidationResult : IValidationResult
{
	private readonly List<IValidationFailure> _errors = new();

	public IReadOnlyList<IValidationFailure> Errors => _errors;

	public bool Interrupted { get; set; }
	public bool SkipNestedValidation { get; set; }

	IReadOnlyList<IBaseValidationFailure> IValidationResult.Errors => _errors;

	public ValidationResult()
	{
	}

	public ValidationResult(IValidationFailure failure)
	{
		if (failure == null)
			throw new ArgumentNullException(nameof(failure));

		_errors.Add(failure);
	}

	internal ValidationResult AddFailure(IValidationFailure? failure)
	{
		if (failure != null)
			_errors.Add(failure);

		return this;
	}

	internal void Merge(ValidationResult? result)
	{
		if (result == null)
			return;

		foreach (var error in result.Errors)
			AddFailure(error);
	}
}
