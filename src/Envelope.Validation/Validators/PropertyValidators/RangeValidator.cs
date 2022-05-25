using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class RangeValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_LessThan = "Must be less than '{ValueToCompare}'.";
	private const string DEFAULT_LessThan_WithProperty = "'{PropertyName}' must be less than '{ValueToCompare}'.";
	private const string DEFAULT_LessThanOrEqual = "Must be less than or equal to '{ValueToCompare}'.";
	private const string DEFAULT_LessThanOrEqual_WithProperty = "'{PropertyName}' must be less than or equal to '{ValueToCompare}'.";

	private const string DEFAULT_GreaterThan = "Must be greater than '{ValueToCompare}'.";
	private const string DEFAULT_GreaterThan_WithProperty = "'{PropertyName}' must be greater than '{ValueToCompare}'.";
	private const string DEFAULT_GreaterThanOrEqual = "Must be greater than or equal to '{ValueToCompare}'.";
	private const string DEFAULT_GreaterThanOrEqual_WithProperty = "'{PropertyName}' must be greater than or equal to '{ValueToCompare}'.";

	private const string DEFAULT_InclusiveBetween = "Must be between {From} and {To}..";
	private const string DEFAULT_InclusiveBetween_WithProperty = "'{PropertyName}' must be between {From} and {To}.";
	private const string DEFAULT_InclusiveExclusiveBetween = "Must be greater than or equal to {From} and less than {To}.";
	private const string DEFAULT_InclusiveExclusiveBetween_WithProperty = "'{PropertyName}' must be greater than or equal to {From} and less than {To}.";

	private const string DEFAULT_ExclusiveBetween = "Must be between {From} and {To} (exclusive).";
	private const string DEFAULT_ExclusiveBetween_WithProperty = "'{PropertyName}' must be between {From} and {To} (exclusive).";
	private const string DEFAULT_ExclusiveInclusiveBetween = "Must be greater than {From} and less than or equal to {To}.";
	private const string DEFAULT_ExclusiveInclusiveBetween_WithProperty = "'{PropertyName}' must be greater than {From} and less than or equal to {To}.";

	protected override string DefaultValidationMessage { get; }
	protected override string DefaultValidationMessageWithProperty { get; }

	private readonly string _validationResourceKey;
	private readonly string _validationResourceKeyWithProperty;

	private readonly object? _defaultValue;

	public IComparable? From { get; }
	public bool InclusiveFrom { get; }
	public IComparable? To { get; }
	public bool InclusiveTo { get; }

	public RangeValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		object? defaultValue,
		IComparable? from,
		bool inclusiveFrom,
		IComparable? to,
		bool inclusiveTo,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.Range, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		_defaultValue = defaultValue;
		From = from;
		To = to;
		InclusiveFrom = inclusiveFrom;
		InclusiveTo = inclusiveTo;

		if (From == null)
		{
			if (To == null)
			{
				_validationResourceKey = "RANGE";
				_validationResourceKeyWithProperty = "RANGE_WithProperty";
				DefaultValidationMessage = "RANGE";
				DefaultValidationMessageWithProperty = "RANGE_WithProperty";
			}
			else
			{
				if (InclusiveTo)
				{
					_validationResourceKey = Resources.ValidationKeys.LessThanOrEqual;
					_validationResourceKeyWithProperty = Resources.ValidationKeys.LessThanOrEqual_WithProperty;
					DefaultValidationMessage = DEFAULT_LessThanOrEqual;
					DefaultValidationMessageWithProperty = DEFAULT_LessThanOrEqual_WithProperty;
				}
				else
				{
					_validationResourceKey = Resources.ValidationKeys.LessThan;
					_validationResourceKeyWithProperty = Resources.ValidationKeys.LessThan_WithProperty;
					DefaultValidationMessage = DEFAULT_LessThan;
					DefaultValidationMessageWithProperty = DEFAULT_LessThan_WithProperty;
				}
			}
		}
		else
		{
			if (To == null)
			{
				if (InclusiveFrom)
				{
					_validationResourceKey = Resources.ValidationKeys.GreaterThanOrEqual;
					_validationResourceKeyWithProperty = Resources.ValidationKeys.GreaterThanOrEqual;
					DefaultValidationMessage = DEFAULT_GreaterThanOrEqual;
					DefaultValidationMessageWithProperty = DEFAULT_GreaterThanOrEqual_WithProperty;
				}
				else
				{
					_validationResourceKey = Resources.ValidationKeys.GreaterThan;
					_validationResourceKeyWithProperty = Resources.ValidationKeys.GreaterThan;
					DefaultValidationMessage = DEFAULT_GreaterThan;
					DefaultValidationMessageWithProperty = DEFAULT_GreaterThan_WithProperty;
				}
			}
			else
			{
				if (InclusiveFrom)
				{
					if (InclusiveTo)
					{
						_validationResourceKey = Resources.ValidationKeys.InclusiveBetween;
						_validationResourceKeyWithProperty = Resources.ValidationKeys.InclusiveBetween;
						DefaultValidationMessage = DEFAULT_InclusiveBetween;
						DefaultValidationMessageWithProperty = DEFAULT_InclusiveBetween_WithProperty;
					}
					else
					{
						_validationResourceKey = Resources.ValidationKeys.InclusiveExclusiveBetween;
						_validationResourceKeyWithProperty = Resources.ValidationKeys.InclusiveExclusiveBetween;
						DefaultValidationMessage = DEFAULT_InclusiveExclusiveBetween;
						DefaultValidationMessageWithProperty = DEFAULT_InclusiveExclusiveBetween_WithProperty;
					}
				}
				else
				{
					if (InclusiveTo)
					{
						_validationResourceKey = Resources.ValidationKeys.ExclusiveInclusiveBetween;
						_validationResourceKeyWithProperty = Resources.ValidationKeys.ExclusiveInclusiveBetween;
						DefaultValidationMessage = DEFAULT_ExclusiveInclusiveBetween;
						DefaultValidationMessageWithProperty = DEFAULT_ExclusiveInclusiveBetween_WithProperty;
					}
					else
					{
						_validationResourceKey = Resources.ValidationKeys.ExclusiveBetween;
						_validationResourceKeyWithProperty = Resources.ValidationKeys.ExclusiveBetween;
						DefaultValidationMessage = DEFAULT_ExclusiveBetween;
						DefaultValidationMessageWithProperty = DEFAULT_ExclusiveBetween_WithProperty;
					}
				}
			}
		}
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ nameof(From), From },
				{ nameof(To), To },
				{ "ValueToCompare", From ?? To },
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T, TProperty?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ValidationHelper.IsDefaultOrEmpty(ctx.ValueToValidate, _defaultValue) || (From == null && To == null))
			return null;

		if (ctx.ValueToValidate is IComparable value)
		{
			var ok = true;

			if (From != null)
			{
				if (InclusiveFrom)
				{
					ok = ok && 0 <= value.CompareTo(From);
				}
				else
				{
					ok = ok && 0 < value.CompareTo(From);
				}
			}

			if (To != null)
			{
				if (InclusiveTo)
				{
					ok = ok && value.CompareTo(To) <= 0;
				}
				else
				{
					ok = ok && value.CompareTo(To) < 0;
				}
			}

			if (ok)
				return null;
			else
				return new ValidationResult(
					new ValidationFailure(
						ObjectPath,
						ValidatorType,
						HasServerCondition,
						ClientConditionDefinition,
						GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, _validationResourceKey, options?.RangeMessageGetter),
						GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, _validationResourceKeyWithProperty, options?.RangeMessageWithPropertyGetter),
						FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		}

		throw new InvalidOperationException($"{nameof(ctx.ValueToValidate)} must implement {nameof(IComparable)}.");
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, _validationResourceKey, null),
			GetValidationMessageWithProperty(default, default, _validationResourceKeyWithProperty, null))
		{
			From = From,
			InclusiveFrom = InclusiveFrom,
			To = To,
			InclusiveTo = InclusiveTo
		};
}
