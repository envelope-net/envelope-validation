using Envelope.Validation.Client;
using Envelope.Reflection.ObjectPaths;
using System.Collections;
using System.Text;

namespace Envelope.Validation;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IValidatorDescriptor
{
	Type ObjectType { get; }

	IObjectPath ObjectPath { get; }

	ValidatorType ValidatorType { get; }

	string ValidatorTypeInfo { get; }

	bool HasServerCondition { get; }

	IClientConditionDefinition? ClientConditionDefinition { get; }

	IReadOnlyList<IValidatorDescriptor> Validators { get; }

	//DefaultOrEmpty, NotDefaultOrEmpty
	object? DefaultValue { get; }

	//Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual
	IComparable? ValueToCompare { get; }

	//MultiEqual, MultiNotEqual
	IEnumerable<IComparable?>? ValuesToCompare { get; }

	//Equal, NotEqual
	IEqualityComparer? Comparer { get; }

	//ExclusiveBetween, InclusiveBetween
	IComparable? From { get; }

	bool InclusiveFrom { get; }

	//ExclusiveBetween, InclusiveBetween
	IComparable? To { get; }

	bool InclusiveTo { get; }

	//Length
	int MinLength { get; }

	//Length
	int MaxLength { get; }

	//PrecisionScaleDecimal
	int Scale { get; }

	//PrecisionScaleDecimal
	int Precision { get; }

	//PrecisionScaleDecimal
	bool IgnoreTrailingZeros { get; }

	//RegEx
	string? Pattern { get; }

	string? Message { get; }

	string? MessageWithPropertyName { get; }

	bool IsEqualTo(IValidatorDescriptor other);

	string Print();

	void PrintInternal(StringBuilder sb, int indent);
}
