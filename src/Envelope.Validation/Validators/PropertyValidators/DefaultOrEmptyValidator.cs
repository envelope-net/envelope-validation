using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class DefaultOrEmptyValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must be empty.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must be empty.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	private readonly object? _defaultValue;

	public DefaultOrEmptyValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		object? defaultValue,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.DefaultOrEmpty, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		_defaultValue = defaultValue;
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

		if (ValidationHelper.IsDefaultOrEmpty(ctx.ValueToValidate, _defaultValue))
			return null;
		else
			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.DefaultOrEmpty, options?.DefaultOrEmptyMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.DefaultOrEmpty_WithProperty, options?.DefaultOrEmptyMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.Validation.__Keys.DefaultOrEmpty, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.DefaultOrEmpty_WithProperty, null))
		{
			DefaultValue = _defaultValue
		};
}
