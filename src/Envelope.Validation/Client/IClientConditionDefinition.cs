namespace Envelope.Validation.Client;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IClientConditionDefinition
{
	LogicalOperators? LogicalOperator { get; }
	string? PropertyName { get; }
	Func<object?, object?>? PropertyGetter { get; }
	Operators? Operator { get; }
	string? ValuePropertyName { get; }
	Func<object?, object?>? ValueGetter { get; }
	IClientConditionDefinition? Left { get; }
	IClientConditionDefinition? Right { get; }

	bool Execute(object? obj);
}
