using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;

namespace Envelope.Validation.Validators.PropertyValidators;

internal class PrecisionScaleDecimalValidator<T, TProperty> : PropertyValidator<T, TProperty?>
{
	private const string DEFAULT_ValidationMessage = "Must  not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals.";
	private const string DEFAULT_ValidationMessageWithProperty = "'{PropertyName}' must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals.";

	protected override string DefaultValidationMessage => DEFAULT_ValidationMessage;
	protected override string DefaultValidationMessageWithProperty => DEFAULT_ValidationMessageWithProperty;

	public int Scale { get; }
	public int Precision { get; }
	public bool IgnoreTrailingZeros { get; }

	public PrecisionScaleDecimalValidator(
		Func<T, TProperty> valueGetter,
		IObjectPath objectPath,
		Func<T?, bool>? condition,
		IClientConditionDefinition? clientConditionDefinition,
		Func<T?, string?>? failureInfoFunc,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter)
		: base(ValidatorType.PrecisionScale, valueGetter, objectPath, condition, clientConditionDefinition, failureInfoFunc, messageGetter, messageWithPropertyGetter)
	{
		Scale = scale;
		Precision = precision;
		IgnoreTrailingZeros = ignoreTrailingZeros;

		if (Scale < 0)
			throw new ArgumentOutOfRangeException(nameof(scale), $"Scale must be a positive integer. [value:{Scale}].");

		if (Precision < 0)
			throw new ArgumentOutOfRangeException(nameof(precision), $"Precision must be a positive integer. [value:{Precision}].");

		if (Precision < Scale)
			throw new ArgumentOutOfRangeException(nameof(scale), $"Scale must be less than precision. [scale:{Scale}, precision:{Precision}].");
	}

	protected override IDictionary<string, object?> GetPlaceholderValues()
		=> new Dictionary<string, object?>
			{
				{ "ExpectedPrecision", Precision },
				{ "ExpectedScale", Scale },
				{ "PropertyName", GetDisplayName() }
			};

	internal override ValidationResult? Validate(ValidationContext context, ValidationOptions? options)
	{
		//if (string.IsNullOrWhiteSpace(ObjectPath.PropertyName))
		//	throw new InvalidOperationException($"{nameof(ObjectPath)}.{nameof(ObjectPath.PropertyName)} == null");

		if (context is not ValidationContext<T, TProperty?> ctx)
			throw new ArgumentException($"{nameof(context)} must be type of {typeof(ValidationContext<T>).FullName}", nameof(context));

		if (ctx.ValueToValidate is not decimal value)
			return null;

		var scale = GetScale(value);
		var precision = GetPrecision(value);
		var actualIntegerDigits = precision - scale;
		var expectedIntegerDigits = Precision - Scale;
		if (Scale < scale || expectedIntegerDigits < actualIntegerDigits)
		{
			new ValidationResult(
				new ValidationFailure(
					ObjectPath,
					context,
					ValidatorType,
					HasServerCondition,
					ClientConditionDefinition,
					GetValidationMessage(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.PrecisionScale, options?.PrecisionScaleMessageGetter),
					GetValidationMessageWithProperty(ctx.InstanceToValidate, ctx.ValueToValidate, Resources.Validation.__Keys.PrecisionScale_WithProperty, options?.PrecisionScaleMessageWithPropertyGetter),
					FailureInfoFunc?.Invoke(ctx.InstanceToValidate)));
		}

		return null;
	}

	public override IValidatorDescriptor ToDescriptor()
		=> new ValidationDescriptor(
			typeof(T),
			ObjectPath,
			ValidatorType,
			GetType().ToFriendlyFullName(),
			HasServerCondition,
			ClientConditionDefinition,
			GetValidationMessage(default, default, Resources.Validation.__Keys.PrecisionScale, null),
			GetValidationMessageWithProperty(default, default, Resources.Validation.__Keys.PrecisionScale_WithProperty, null))
		{
			Scale = Scale,
			Precision = Precision,
			IgnoreTrailingZeros = IgnoreTrailingZeros
		};

	private static uint[] GetBits(decimal @decimal)
	{
		return (uint[])(object)decimal.GetBits(@decimal);
	}

	private static decimal GetMantissa(decimal @decimal)
	{
		var bits = GetBits(@decimal);
		return (bits[2] * 4294967296m * 4294967296m) + (bits[1] * 4294967296m) + bits[0];
	}

	private static uint GetUnsignedScale(decimal @decimal)
	{
		var bits = GetBits(@decimal);
		uint scale = (bits[3] >> 16) & 31;
		return scale;
	}

	private int GetScale(decimal @decimal)
	{
		uint scale = GetUnsignedScale(@decimal);
		if (IgnoreTrailingZeros)
		{
			return (int)(scale - NumTrailingZeros(@decimal));
		}

		return (int)scale;
	}

	private static uint NumTrailingZeros(decimal @decimal)
	{
		uint trailingZeros = 0;
		uint scale = GetUnsignedScale(@decimal);
		for (decimal tmp = GetMantissa(@decimal); tmp % 10m == 0 && trailingZeros < scale; tmp /= 10)
		{
			trailingZeros++;
		}

		return trailingZeros;
	}

	private int GetPrecision(decimal @decimal)
	{
		uint precision = 0;
		for (decimal tmp = GetMantissa(@decimal); tmp >= 1; tmp /= 10)
		{
			precision++;
		}

		if (IgnoreTrailingZeros)
		{
			return (int)(precision - NumTrailingZeros(@decimal));
		}

		return (int)precision;
	}
}
