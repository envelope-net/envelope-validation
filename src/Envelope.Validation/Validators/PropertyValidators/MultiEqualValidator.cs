using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Collections;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class MultiEqualValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must be equal to one of the values '[{ValuesToCompare}]'.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must be equal to one of the values '[{ValuesToCompare}]'.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public IEnumerable<IComparable?>? ValuesToCompare { get; }
	public IEqualityComparer? Comparer { get; }

	public MultiEqualValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		IEnumerable<IComparable?>? valuesToCompare,
		IEqualityComparer? comparer,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.MultiEqual, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		ValuesToCompare = valuesToCompare?.Distinct().ToList();
		Comparer = comparer;
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ nameof(ValuesToCompare), ValuesToCompare },
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T, TProperty?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate == null)
			return (ValuesToCompare == null || ValuesToCompare.Any(x => x == null))
				? null
				: new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual, options?.MultiEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual_WithProperty, options?.MultiEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));

		if (ValuesToCompare == null)
			return new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual, options?.MultiEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual_WithProperty, options?.MultiEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));

		if (Comparer == null)
			return ValuesToCompare.Any(x => Equals(x, ctx.ValueToValidate))
				? null
				: new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual, options?.MultiEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual_WithProperty, options?.MultiEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		else
			return ValuesToCompare.Any(x => Comparer.Equals(x, ctx.ValueToValidate))
				? null
				: new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual, options?.MultiEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.ValidationKeys.MultiEqual_WithProperty, options?.MultiEqualMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.ValidationKeys.MultiEqual, null),
			GetValidationMessageWithProperty(default, default, Resources.ValidationKeys.MultiEqual_WithProperty, null))
		{
			ValuesToCompare = ValuesToCompare,
			Comparer = Comparer
		};
}
