using Envelope.Extensions;
using Envelope.Validation.Internal;
using System.Text;

namespace Envelope.Validation.Client;

public class ClientCondition<T> : IClientConditionDefinition
{
	internal LogicalOperators? LogicalOperator { get; set; }
	internal string? PropertyName { get; set; }
	internal Func<object?, object?>? PropertyGetter { get; set; }
	internal Operators? Operator { get; set; }
	internal string? ValuePropertyName { get; set; }
	internal Func<object?, object?>? ValueGetter { get; set; }
	internal ClientCondition<T>? Left { get; set; }
	internal ClientCondition<T>? Right { get; set; }

	LogicalOperators? IClientConditionDefinition.LogicalOperator => LogicalOperator;
	string? IClientConditionDefinition.PropertyName => PropertyName;
	Func<object?, object?>? IClientConditionDefinition.PropertyGetter => PropertyGetter;
	Operators? IClientConditionDefinition.Operator => Operator;
	string? IClientConditionDefinition.ValuePropertyName => ValuePropertyName;
	Func<object?, object?>? IClientConditionDefinition.ValueGetter => ValueGetter;
	IClientConditionDefinition? IClientConditionDefinition.Left => Left;
	IClientConditionDefinition? IClientConditionDefinition.Right => Right;

	internal ClientCondition()
	{
	}

	bool IClientConditionDefinition.Execute(object? obj)
		=> Execute((T?)obj);

	public bool Execute(T? obj)
	{
		if (LogicalOperator.HasValue)
		{
			if (Left == null || Right == null)
				throw new InvalidOperationException($"{nameof(Left)} == null || {nameof(Right)} == null");

			if (LogicalOperator == LogicalOperators.And)
			{
				var leftResult = Left.Execute(obj);
				if (!leftResult)
					return false;

				return Right.Execute(obj);
			}
			else // OR
			{
				var leftResult = Left.Execute(obj);
				if (leftResult)
					return true;

				return Right.Execute(obj);
			}
		}

		if (!Operator.HasValue)
			throw new InvalidOperationException($"{nameof(Operator)} == null");

		if (PropertyGetter == null)
			throw new InvalidOperationException($"{nameof(PropertyGetter)} == null");

		if (ValueGetter == null)
			throw new InvalidOperationException($"{nameof(ValueGetter)} == null");

		var propertyValue = PropertyGetter.Invoke(obj);
		var valueToCompare = ValueGetter.Invoke(obj);

		return Operator.Value switch
		{
			Operators.EqualsTo => propertyValue == null ? valueToCompare == null : propertyValue.Equals(valueToCompare),
			Operators.NotEqualsTo => propertyValue == null ? valueToCompare != null : !propertyValue.Equals(valueToCompare),
			Operators.LessThan => propertyValue != null && ((IComparable)propertyValue).CompareTo(valueToCompare) < 0,
			Operators.LessThanOrEqualTo => propertyValue != null && ((IComparable)propertyValue).CompareTo(valueToCompare) <= 0,
			Operators.GreaterThan => propertyValue != null && ((IComparable)propertyValue).CompareTo(valueToCompare) > 0,
			Operators.GreaterThanOrEqualTo => propertyValue != null && ((IComparable)propertyValue).CompareTo(valueToCompare) >= 0,
			Operators.StartsWith => propertyValue?.ToString()?.StartsWithSafe(valueToCompare?.ToString()) ?? false,
			Operators.EndsWith => propertyValue?.ToString()?.EndsWithSafe(valueToCompare?.ToString()) ?? false,
			Operators.Contains => propertyValue?.ToString()?.ContainsSafe(valueToCompare?.ToString()) ?? false,
			Operators.NotStartsWith => !(propertyValue?.ToString()?.StartsWithSafe(valueToCompare?.ToString()) ?? false),
			Operators.NotEndsWith => !(propertyValue?.ToString()?.EndsWithSafe(valueToCompare?.ToString()) ?? false),
			Operators.NotContains => !(propertyValue?.ToString()?.ContainsSafe(valueToCompare?.ToString()) ?? false),
			Operators.IsNull => propertyValue == null,
			Operators.IsNotNull => propertyValue != null,
			Operators.DefaultOrEmpty => ValidationHelper.IsDefaultOrEmpty(propertyValue, valueToCompare),
			Operators.NotDefaultOrEmpty => !ValidationHelper.IsDefaultOrEmpty(propertyValue, valueToCompare),
			_ => throw new NotSupportedException($"Not supported {nameof(Operator)} = {Operator}"),
		};
	}

	public override string ToString()
	{
		return ToStringInternal();
	}

	internal string ToStringInternal()
	{
		var sb = new StringBuilder();
		sb.Append('(');

		if (LogicalOperator.HasValue)
		{
			if (LogicalOperator == LogicalOperators.And)
			{
				sb.Append($"{Left?.ToStringInternal()} {OperatorConverter.LogicalOperators[LogicalOperator.Value]} {Right?.ToStringInternal()}");
			}
			else //OR
			{
				sb.Append($"{Left?.ToStringInternal()} {OperatorConverter.LogicalOperators[LogicalOperator.Value]} {Right?.ToStringInternal()}");
			}
		}
		else
		{
			object? value = null;
			if (string.IsNullOrWhiteSpace(ValuePropertyName))
			{
				value = ValueGetter?.Invoke(null);
				if (value is string str)
					value = $"'{str}'";
			}
			else
			{
				value = ValuePropertyName;
			}

			if (Operator.HasValue)
			{
				sb.Append($"{PropertyName} {OperatorConverter.Operators[Operator.Value]} {value}");
			}
			else
			{
				sb.Append($"{PropertyName} ??? {value}");
			}
		}

		sb.Append(')');
		return sb.ToString();
	}
}
