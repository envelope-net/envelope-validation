using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Collections;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class NotEqualValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must not be equal to '{ValueToCompare}'.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must not be equal to '{ValueToCompare}'.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public IComparable? ValueToCompare { get; }
	public IEqualityComparer? Comparer { get; }

	public NotEqualValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		IComparable? valueToCompare,
		IEqualityComparer? comparer,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.NotEqual, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		ValueToCompare = valueToCompare;
		Comparer = comparer;
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ nameof(ValueToCompare), ValueToCompare },
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T, TProperty?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate == null)
			return ValueToCompare == null
				? new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.NotEqual, options?.NotEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.NotEqual_WithProperty, options?.NotEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)))
				: null;

		if (ValueToCompare == null)
			return null;

		if (Comparer == null)
			return Equals(ValueToCompare, ctx.ValueToValidate)
				? new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.NotEqual, options?.NotEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.NotEqual_WithProperty, options?.NotEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)))
				: null;
		else
			return Comparer.Equals(ValueToCompare, ctx.ValueToValidate)
				? new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.NotEqual, options?.NotEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.NotEqual_WithProperty, options?.NotEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)))
				: null;
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, Resources.Validation.__Keys.NotEqual, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.NotEqual_WithProperty, null))
		{
			ValueToCompare = ValueToCompare,
			Comparer = Comparer
		};
}
