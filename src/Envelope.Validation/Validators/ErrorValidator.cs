using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators;

public class ErrorValidator<T> : Validator<T>
{
	public Func<T?, bool> Condition { get; }
	internal Func<T?, string?>? FailureInfoFunc { get; }

	public Func<T?, string> ErrorMessageFunc { get; }

	public ErrorValidator(
		IObjectPath objectPath,
		Func<T?, bool> condition,
		Func<T?, string> errorMessage,
		Func<T?, string?>? failureInfoFunc)
		: base(ValidatorType.ErrorObject, objectPath, true, null)
	{
		Condition = condition ?? throw new ArgumentNullException(nameof(condition));
		FailureInfoFunc = failureInfoFunc;

		ErrorMessageFunc = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));

		if (condition == null)
			throw new ArgumentNullException(nameof(condition));
	}

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		var result = new ValidationResult();

		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (Condition.Invoke(ctx.InstanceToValidate))
		{
			var errorMessage = ErrorMessageFunc.Invoke(ctx.InstanceToValidate);

			result.AddFailure(
				new ValidationFailure(
					ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
					context,
					ValidatorType,
					HasServerCondition,
					null,
					errorMessage,
					errorMessage,
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		}

		return result;
	}
}
