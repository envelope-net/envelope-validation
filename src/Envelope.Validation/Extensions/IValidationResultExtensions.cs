using Envelope.Reflection.ObjectPaths;
using Envelope.Validation.Internal;
using Envelope.Validation.Results;
using System.Linq.Expressions;

namespace Envelope.Validation;

public static class IValidationResultExtensions
{
	public static IValidationResult AddFailure<T>(
		this IValidationResult validationResult,
		T instanceToValidate,
		string message,
		string? detailLogInfo = null)
		=> AddFailure<T>(
			validationResult,
			message,
			detailLogInfo);

	public static IValidationResult AddFailure<T>(
		this IValidationResult validationResult,
		string message,
		string? detailLogInfo = null)
	{
		if (validationResult == null)
			throw new ArgumentNullException(nameof(validationResult));

		if (validationResult is not ValidationResult vr)
			throw new InvalidOperationException($"{typeof(ValidationResult).FullName} is required");

		if (string.IsNullOrWhiteSpace(message))
			throw new ArgumentNullException(nameof(message));

		vr.AddFailure(
			new ValidationFailure(
				ObjectPath<T>.Create(),
				(Dictionary<int, int>?)null,
				ValidatorType.ErrorObject,
				true,
				null,
				message,
				message,
				detailLogInfo));

		return validationResult;
	}

	public static IValidationResult AddFailure<T, TProperty>(
		this IValidationResult validationResult,
		T instanceToValidate,
		Expression<Func<T, TProperty>> expression,
		Dictionary<int, int>? objectPathIndexes,
		ValidatorType type,
		string message,
		string messageWithPropertyName,
		string? detailLogInfo = null)
		=> AddFailure(
			validationResult,
			expression,
			objectPathIndexes,
			type,
			message,
			messageWithPropertyName,
			detailLogInfo);

	public static IValidationResult AddFailure<T, TProperty>(
		this IValidationResult validationResult,
		Expression<Func<T, TProperty>> expression,
		Dictionary<int, int>? objectPathIndexes,
		ValidatorType type,
		string message,
		string messageWithPropertyName,
		string? detailLogInfo = null)
	{
		if (validationResult == null)
			throw new ArgumentNullException(nameof(validationResult));

		if (validationResult is not ValidationResult vr)
			throw new InvalidOperationException($"{typeof(ValidationResult).FullName} is required");

		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		if (string.IsNullOrWhiteSpace(message))
			throw new ArgumentNullException(nameof(message));

		if (string.IsNullOrWhiteSpace(messageWithPropertyName))
			throw new ArgumentNullException(nameof(messageWithPropertyName));

		//var vc = new ValidationContext<T, TProperty>(instanceToValidate, expression.Compile().Invoke(instanceToValidate), null);

		vr.AddFailure(
			new ValidationFailure(
				ObjectPath<T>.Create().AddProperty(expression),
				objectPathIndexes,
				type,
				true,
				null,
				message,
				messageWithPropertyName,
				detailLogInfo));

		return validationResult;
	}

	public static IValidationResult Merge(this IValidationResult validationResult, IValidationResult? result)
	{
		if (validationResult == null)
			throw new ArgumentNullException(nameof(validationResult));

		if (result == null)
			return validationResult;

		if (validationResult is not ValidationResult vr)
			throw new InvalidOperationException($"{typeof(ValidationResult).FullName} is required");

		vr.Merge(result);

		return validationResult;
	}
}
