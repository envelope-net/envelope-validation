using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class ExactLengthValidator<T> : PropertyValidator<T, string?>
{
	private const string DEFAULT_ValidationMessage = "Must be {Length} characters long.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must be {Length} characters long.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public int Length { get; }

	public ExactLengthValidator(
		Func<T, string> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		int length,
		Func<T?, string?, string, string?>? messageGetter,
		Func<T?, string?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.ExactLength, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		if (length <= 0)
			throw new ArgumentOutOfRangeException(nameof(length), $"{nameof(length)} should be larger than 0.");

		Length = length;
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ nameof(Length), Length },
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T, string?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate == null)
			return null;

		if (Length == ctx.ValueToValidate.Length)
			return null;
		else
			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.ExactLength, options?.LengthMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.ExactLength_WithProperty, options?.LengthMessageWithPropertyGetter),
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, Resources.Validation.__Keys.ExactLength, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.ExactLength_WithProperty, null))
		{
			MaxLength = Length,
			MinLength = Length
		};
}
