using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using Envelope.Validation.Client;
using Envelope.Validation.Helpers;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class EmailValidator<T> : PropertyValidator<T, string?>
{
	private const string DEFAULT_ValidationMessage = "Is not a valid email address.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' is not a valid email address.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public EmailValidator(
		Func<T, string> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		Func<T?, string?, string, string?>? messageGetter,
		Func<T?, string?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.Email, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T, string?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate == null || EmailValidator.IsValidEmail(ctx.ValueToValidate))
			return null;
		else
			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Email, options?.EmailMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Email_WithProperty, options?.EmailMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.Validation.__Keys.Email, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.Email_WithProperty, null))
		{
		};
}
