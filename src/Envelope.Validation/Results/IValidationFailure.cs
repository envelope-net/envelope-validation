using Envelope.Validation.Client;

namespace Envelope.Validation.Results;

#if NET6_0_OR_GREATER
[Envelope.Serializer.JsonPolymorphicConverter]
#endif
public interface IValidationFailure : IBaseValidationFailure
{
	ValidatorType Type { get; }
	IClientConditionDefinition? ClientConditionDefinition { get; }
}
