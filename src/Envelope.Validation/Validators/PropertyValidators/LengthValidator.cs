using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class LengthValidator<T> : PropertyValidator<T, string?>
{
	private const string DEFAULT_ValidationMessage = "Must be between {MinLength} and {MaxLength} characters.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must be between {MinLength} and {MaxLength} characters.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public int MinLength { get; }
	public int MaxLength { get; }

	public LengthValidator(
		Func<T, string> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		int minLength,
		int maxLength,
		Func<T?, string?, string, string?>? messageGetter,
		Func<T?, string?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.Length, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		if (maxLength < minLength)
			throw new ArgumentOutOfRangeException(nameof(maxLength), $"{nameof(maxLength)} should be larger than {nameof(minLength)}.");

		MinLength = minLength;
		MaxLength = maxLength;
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ nameof(MinLength), MinLength },
				{ nameof(MaxLength), MaxLength },
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

		if (MinLength <= ctx.ValueToValidate.Length && ctx.ValueToValidate.Length <= MaxLength)
			return null;
		else
			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Length, options?.LengthMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Length_WithProperty, options?.LengthMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.Validation.__Keys.Length, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.Length_WithProperty, null))
		{
			MaxLength = MaxLength,
			MinLength = MinLength
		};
}
