using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class NullValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must be empty.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must be empty.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public NullValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.Null, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
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

		if (context is not ValidationContext<T, TProperty?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate == null)
			return null;
		else
			return
				new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Null, options?.NullMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.Null_WithProperty, options?.NullMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.Validation.__Keys.Null, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.Null_WithProperty, null))
		{
		};
}
