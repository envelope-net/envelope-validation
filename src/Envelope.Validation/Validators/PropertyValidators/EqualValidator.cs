using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Collections;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class EqualValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must be equal to '{ValueToCompare}'.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must be equal to '{ValueToCompare}'.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public IComparable? ValueToCompare { get; }
	public IEqualityComparer? Comparer { get; }

	public EqualValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		IComparable? valueToCompare,
		IEqualityComparer? comparer,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.Equal, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
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
				? null
				: new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal, options?.EqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal_WithProperty, options?.EqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));

		if (ValueToCompare == null)
			new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal, options?.EqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal_WithProperty, options?.EqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));

		if (Comparer == null)
			return Equals(ValueToCompare, ctx.ValueToValidate)
				? null
				: new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal, options?.EqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal_WithProperty, options?.EqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		else
			return Comparer.Equals(ValueToCompare, ctx.ValueToValidate)
				? null
				: new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal, options?.EqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.Equal_WithProperty, options?.EqualMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.ValidationKeys.Equal, null),
			GetValidationMessageWithProperty(default, default, Resources.ValidationKeys.Equal_WithProperty, null))
		{
			ValueToCompare = ValueToCompare,
			Comparer = Comparer
		};
}
