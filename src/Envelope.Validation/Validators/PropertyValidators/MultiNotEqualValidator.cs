using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Collections;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class MultiNotEqualValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must not be equal to any of the values '[{ValuesToCompare}]'.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must not be equal to any of the values '[{ValuesToCompare}]'.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public IEnumerable<IComparable?>? ValuesToCompare { get; }
	public IEqualityComparer? Comparer { get; }

	public MultiNotEqualValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		IEnumerable<IComparable?>? valuesToCompare,
		IEqualityComparer? comparer,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.MultiNotEqual, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
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
				? new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.MultiNotEqual, options?.MultiNotEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.MultiNotEqual_WithProperty, options?.MultiNotEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)))
				: null;

		if (ValuesToCompare == null)
			return null;

		if (Comparer == null)
			return ValuesToCompare.Any(x => Equals(x, ctx.ValueToValidate))
				? new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.MultiNotEqual, options?.MultiNotEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.MultiNotEqual_WithProperty, options?.MultiNotEqualMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)))
				: null;
		else
			return ValuesToCompare.Any(x => Comparer.Equals(x, ctx.ValueToValidate))
				? new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						context,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.MultiNotEqual, options?.MultiNotEqualMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.MultiNotEqual_WithProperty, options?.MultiNotEqualMessageWithPropertyGetter),
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
			GetValidationMessage(default, default, Resources.Validation.__Keys.MultiNotEqual, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.MultiNotEqual_WithProperty, null))
		{
			ValuesToCompare = ValuesToCompare,
			Comparer = Comparer
		};
}
