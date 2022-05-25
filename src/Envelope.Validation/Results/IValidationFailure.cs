using Envelope.Validation.Client;
using Envelope.Validation;

namespace Envelope.Validation.Results;

public interface IValidationFailure : IBaseValidationFailure
{
	ValidatorType Type { get; }
	IClientConditionDefinition? ClientConditionDefinition { get; }
}
