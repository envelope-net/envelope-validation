using Envelope.Validation.Client;
using Envelope.Validation.Results;
using Envelope.Reflection.ObjectPaths;
using Envelope.Validation;

namespace Envelope.Validation.Internal;

internal class ValidationFailure : IValidationFailure
{
	public IObjectPath ObjectPath { get; }
	public ValidatorType Type { get; }
	public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
	public string Message { get; internal set; }
	public string MessageWithPropertyName { get; internal set; }
	public string? DetailInfo { get; internal set; }
	public bool HasServerCondition { get; }
	public IClientConditionDefinition? ClientConditionDefinition { get; }

	public ValidationFailure(
		IObjectPath objectPath,
		ValidationContext context,
		ValidatorType type,
		bool hasServerCondition,
		IClientConditionDefinition? clientConditionDefinition,
		string message,
		string messageWithPropertyName,
		string? detailInfo)
	{
		ObjectPath = objectPath?.Clone(ObjectPathCloneMode.BottomUp) ?? throw new ArgumentNullException(nameof(objectPath));

		if (context != null && 0 < context.Indexes.Count)
		{
			var currentObjectPath = ObjectPath;
			while (currentObjectPath != null)
			{
				if (context.Indexes.TryGetValue(currentObjectPath.Depth, out var index))
					currentObjectPath.Index = index;

				currentObjectPath = currentObjectPath.Parent;
			}
		}

		Type = type;
		HasServerCondition = hasServerCondition;
		ClientConditionDefinition = clientConditionDefinition;

		if (string.IsNullOrWhiteSpace(message))
			throw new ArgumentNullException(nameof(message));

		Message = message;
		MessageWithPropertyName = messageWithPropertyName;
		DetailInfo = detailInfo;
	}

	public override string ToString()
		=> $"{ObjectPath}: {Type}: {MessageWithPropertyName}";
}
