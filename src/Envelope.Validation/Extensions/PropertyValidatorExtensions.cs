using Envelope.Validation.Validators;
using Envelope.Validation.Validators.PropertyValidators;
using Envelope.Extensions;
using Envelope.Reflection.ObjectPaths;
using System.Collections;

//#nullable disable

namespace Envelope.Validation;

//#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
public static class PropertyValidatorExtensions
{
	public static PropertyValidator<T, string> EmailAddress<T>(
		this PropertyValidator<T, string> propertyValidator,
		Func<T?, string?, string, string?>? messageGetter = null,
		Func<T?, string?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new EmailValidator<T>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> DefaultOrEmpty<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new DefaultOrEmptyValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> NotDefaultOrEmpty<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new NotDefaultOrEmptyValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> EqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? value,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		((IValidator)propertyValidator).AddValidatorInternal(new EqualValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			value,
			comparer,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> EqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? value,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		((IValidator)propertyValidator).AddValidatorInternal(new EqualValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			value,
			comparer,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> NotEqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? value,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		((IValidator)propertyValidator).AddValidatorInternal(new NotEqualValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			value,
			comparer,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> NotEqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? value,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		((IValidator)propertyValidator).AddValidatorInternal(new NotEqualValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			value,
			comparer,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> EqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		IEnumerable<TProperty>? values,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		if (values == null || values.Count() < 2)
			((IValidator)propertyValidator).AddValidatorInternal(new EqualValidator<T, TProperty?>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
				values?.FirstOrDefault(),
				comparer,
				messageGetter,
				messageWithPropertyGetter));
		else
			((IValidator)propertyValidator).AddValidatorInternal(new MultiEqualValidator<T, TProperty?>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
				values?.Cast<IComparable?>(),
				comparer,
				messageGetter,
				messageWithPropertyGetter));

		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> EqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		IEnumerable<TProperty>? values,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		if (values == null || values.Count() < 2)
			((IValidator)propertyValidator).AddValidatorInternal(new EqualValidator<T, TProperty>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
#pragma warning disable IDE0031 // Use null propagation
				values == null ? null : values.FirstOrDefault(),
#pragma warning restore IDE0031 // Use null propagation
				comparer,
				messageGetter,
				messageWithPropertyGetter));
		else
			((IValidator)propertyValidator).AddValidatorInternal(new MultiEqualValidator<T, TProperty>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
				values?.Cast<IComparable?>(),
				comparer,
				messageGetter,
				messageWithPropertyGetter));

		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> NotEqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		IEnumerable<TProperty>? values,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		if (values == null || values.Count() < 2)
			((IValidator)propertyValidator).AddValidatorInternal(new NotEqualValidator<T, TProperty?>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
				values?.FirstOrDefault(),
				comparer,
				messageGetter,
				messageWithPropertyGetter));
		else
			((IValidator)propertyValidator).AddValidatorInternal(new MultiNotEqualValidator<T, TProperty?>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
				values?.Cast<IComparable?>(),
				comparer,
				messageGetter,
				messageWithPropertyGetter));

		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> NotEqualsTo<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		IEnumerable<TProperty>? values,
		IEqualityComparer? comparer = null,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		if (values == null || values.Count() < 2)
			((IValidator)propertyValidator).AddValidatorInternal(new NotEqualValidator<T, TProperty>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
#pragma warning disable IDE0031 // Use null propagation
				values == null ? null : values.FirstOrDefault(),
#pragma warning restore IDE0031 // Use null propagation
				comparer,
				messageGetter,
				messageWithPropertyGetter));
		else
			((IValidator)propertyValidator).AddValidatorInternal(new MultiNotEqualValidator<T, TProperty>(
				propertyValidator.ValueGetter,
				propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
				propertyValidator.Condition,
				propertyValidator.ClientConditionDefinition,
				propertyValidator.FailureInfoFunc,
				values?.Cast<IComparable?>(),
				comparer,
				messageGetter,
				messageWithPropertyGetter));

		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> GreaterThan<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			value,
			false,
			null,
			false,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> GreaterThan<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			value,
			false,
			null,
			false,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> GreaterThanOrEqual<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			value,
			true,
			null,
			false,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> GreaterThanOrEqual<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			value,
			true,
			null,
			false,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> LessThan<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			null,
			false,
			value,
			false,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> LessThan<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			null,
			false,
			value,
			false,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> LessThanOrEqual<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			null,
			false,
			value,
			true,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> LessThanOrEqual<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? value,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			null,
			false,
			value,
			true,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty?> Range<T, TProperty>(
		this PropertyValidator<T, TProperty?> propertyValidator,
		TProperty? from,
		bool inclusiveFrom,
		TProperty? to,
		bool inclusiveTo,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : struct, IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			from,
			inclusiveFrom,
			to,
			inclusiveTo,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> Range<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		TProperty? from,
		bool inclusiveFrom,
		TProperty? to,
		bool inclusiveTo,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
		where TProperty : IComparable<TProperty>, IComparable
	{
		var defaultValue = typeof(TProperty).GetDefaultNullableValue();
		((IValidator)propertyValidator).AddValidatorInternal(new RangeValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			defaultValue,
			from,
			inclusiveFrom,
			to,
			inclusiveTo,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> Null<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new NullValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, TProperty> NotNull<T, TProperty>(
		this PropertyValidator<T, TProperty> propertyValidator,
		Func<T?, TProperty?, string, string?>? messageGetter = null,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new NotNullValidator<T, TProperty>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, string> RegEx<T>(
		this PropertyValidator<T, string> propertyValidator,
		string? pattern,
		Func<T?, string?, string, string?>? messageGetter = null,
		Func<T?, string?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new RegExValidator<T>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			pattern,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, string> MinLength<T>(
		this PropertyValidator<T, string> propertyValidator,
		int minLength,
		Func<T?, string?, string, string?>? messageGetter = null,
		Func<T?, string?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new LengthValidator<T>(
			LengthValidator<T>.LengthTypeValidatorEnum.Min,
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			minLength,
			int.MaxValue,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, string> MaxLength<T>(
		this PropertyValidator<T, string> propertyValidator,
		int maxLength,
		Func<T?, string?, string, string?>? messageGetter = null,
		Func<T?, string?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new LengthValidator<T>(
			LengthValidator<T>.LengthTypeValidatorEnum.Max,
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			0,
			maxLength,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, string> Length<T>(
		this PropertyValidator<T, string> propertyValidator,
		int minLength,
		int maxLength,
		Func<T?, string?, string, string?>? messageGetter = null,
		Func<T?, string?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new LengthValidator<T>(
			LengthValidator<T>.LengthTypeValidatorEnum.Range,
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			minLength,
			maxLength,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, string> ExactLength<T>(
		this PropertyValidator<T, string> propertyValidator,
		int length,
		Func<T?, string?, string, string?>? messageGetter = null,
		Func<T?, string?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new ExactLengthValidator<T>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			length,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, decimal> PrecisionScale<T>(
		this PropertyValidator<T, decimal> propertyValidator,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, decimal, string, string?>? messageGetter = null,
		Func<T?, decimal, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new PrecisionScaleDecimalValidator<T, decimal>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			precision,
			scale,
			ignoreTrailingZeros,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, decimal?> PrecisionScale<T>(
		this PropertyValidator<T, decimal?> propertyValidator,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, decimal?, string, string?>? messageGetter = null,
		Func<T?, decimal?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new PrecisionScaleDecimalValidator<T, decimal?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			precision,
			scale,
			ignoreTrailingZeros,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, double> PrecisionScale<T>(
		this PropertyValidator<T, double> propertyValidator,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, double, string, string?>? messageGetter = null,
		Func<T?, double, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new PrecisionScaleDoubleValidator<T, double>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			precision,
			scale,
			ignoreTrailingZeros,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, double?> PrecisionScale<T>(
		this PropertyValidator<T, double?> propertyValidator,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, double?, string, string?>? messageGetter = null,
		Func<T?, double?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new PrecisionScaleDoubleValidator<T, double?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			precision,
			scale,
			ignoreTrailingZeros,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, float> PrecisionScale<T>(
		this PropertyValidator<T, float> propertyValidator,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, float, string, string?>? messageGetter = null,
		Func<T?, float, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new PrecisionScaleFloatValidator<T, float>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			precision,
			scale,
			ignoreTrailingZeros,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}

	public static PropertyValidator<T, float?> PrecisionScale<T>(
		this PropertyValidator<T, float?> propertyValidator,
		int precision,
		int scale,
		bool ignoreTrailingZeros,
		Func<T?, float?, string, string?>? messageGetter = null,
		Func<T?, float?, string, string?>? messageWithPropertyGetter = null)
	{
		((IValidator)propertyValidator).AddValidatorInternal(new PrecisionScaleFloatValidator<T, float?>(
			propertyValidator.ValueGetter,
			propertyValidator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp),
			propertyValidator.Condition,
			propertyValidator.ClientConditionDefinition,
			propertyValidator.FailureInfoFunc,
			precision,
			scale,
			ignoreTrailingZeros,
			messageGetter,
			messageWithPropertyGetter));
		return propertyValidator;
	}
}
//#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
