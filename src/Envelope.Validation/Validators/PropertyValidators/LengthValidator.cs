using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class LengthValidator<T> : PropertyValidator<T, string?>
{
	public enum LengthTypeValidatorEnum
	{
		Min,
		Max,
		Range
	}

	private const string DEFAULT_Min_ValidationMessage = "Must be at least {MinLength} characters.";
	private const string DEFAULT_Min_ValidationMessageWithProperty = "'{PropertyName}' must be at least {MinLength} characters.";

	private const string DEFAULT_Max_ValidationMessage = "Must be at the most {MaxLength} characters.";
	private const string DEFAULT_Max_ValidationMessageWithProperty = "'{PropertyName}' must be at the most {MaxLength} characters.";

	private const string DEFAULT_Range_ValidationMessage = "Must be between {MinLength} and {MaxLength} characters.";
	private const string DEFAULT_Range_ValidationMessageWithProperty = "'{PropertyName}' must be between {MinLength} and {MaxLength} characters.";

	public LengthTypeValidatorEnum LengthTypeValidator { get; }
	public int MinLength { get; }
	public int MaxLength { get; }

	protected override string DefaultValidationMessage => LengthTypeValidator switch
	{
		LengthValidator<T>.LengthTypeValidatorEnum.Min => DEFAULT_Min_ValidationMessage,
		LengthValidator<T>.LengthTypeValidatorEnum.Max => DEFAULT_Max_ValidationMessage,
		_ => DEFAULT_Range_ValidationMessage,
	};

	protected override string DefaultValidationMessageWithProperty => LengthTypeValidator switch
	{
		LengthValidator<T>.LengthTypeValidatorEnum.Min => DEFAULT_Min_ValidationMessageWithProperty,
		LengthValidator<T>.LengthTypeValidatorEnum.Max => DEFAULT_Max_ValidationMessageWithProperty,
		_ => DEFAULT_Range_ValidationMessageWithProperty,
	};

	public LengthValidator(
		LengthTypeValidatorEnum lengthTypeValidator,
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

		LengthTypeValidator = lengthTypeValidator;
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
		{
			string resKey;
			string resKey_WithProperty;
			switch (LengthTypeValidator)
			{
				case LengthValidator<T>.LengthTypeValidatorEnum.Min:
					{
						resKey = Resources.Validation.__Keys.Length_Min;
						resKey_WithProperty = Resources.Validation.__Keys.Length_Min_WithProperty;
					}
					break;
				case LengthValidator<T>.LengthTypeValidatorEnum.Max:
					{
						resKey = Resources.Validation.__Keys.Length_Max;
						resKey_WithProperty = Resources.Validation.__Keys.Length_Max_WithProperty;
					}
					break;
				case LengthValidator<T>.LengthTypeValidatorEnum.Range:
				default:
					{
						resKey = Resources.Validation.__Keys.Length_Range;
						resKey_WithProperty = Resources.Validation.__Keys.Length_Range_WithProperty;
					}
					break;
			}

			return new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, resKey, options?.LengthMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, resKey_WithProperty, options?.LengthMessageWithPropertyGetter),
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		}
	}

	public override IValidatorDescriptor ToDescriptor()
	{
		string resKey;
		string resKey_WithProperty;
		switch (LengthTypeValidator)
		{
			case LengthValidator<T>.LengthTypeValidatorEnum.Min:
				{
					resKey = Resources.Validation.__Keys.Length_Min;
					resKey_WithProperty = Resources.Validation.__Keys.Length_Min_WithProperty;
				}
				break;
			case LengthValidator<T>.LengthTypeValidatorEnum.Max:
				{
					resKey = Resources.Validation.__Keys.Length_Max;
					resKey_WithProperty = Resources.Validation.__Keys.Length_Max_WithProperty;
				}
				break;
			case LengthValidator<T>.LengthTypeValidatorEnum.Range:
			default:
				{
					resKey = Resources.Validation.__Keys.Length_Range;
					resKey_WithProperty = Resources.Validation.__Keys.Length_Range_WithProperty;
				}
				break;
		}

		return new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, resKey, null),
			GetValidationMessageWithProperty(default, default, resKey_WithProperty, null))
		{
			MaxLength = MaxLength,
			MinLength = MinLength
		};
	}
}
