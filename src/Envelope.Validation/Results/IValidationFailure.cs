using Envelope.Validation.Client;

namespace Envelope.Validation.Results;

public interface IValidationFailure : IBaseValidationFailure
{
	ValidatorType Type { get; }
	IClientConditionDefinition? ClientConditionDefinition { get; }
}
