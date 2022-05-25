using Envelope.Validation.Client;
using System.Linq.Expressions;

namespace Envelope.Validation;

public class ValidatorFactory
{
	public static IValidator<T> Create<T>(Action<ValidatorBuilder<T>> configure)
	{
		if (configure == null)
			throw new ArgumentNullException(nameof(configure));

		var builder = new ValidatorBuilder<T>();
		configure.Invoke(builder);
		var validator = builder.Build();
		return validator;
	}

	public static IValidator<T> WithError<T>(Func<T?, bool> condition, Func<T?, string> errorMessage, Func<T?, string>? failureInfoFunc = null)
		=> new ValidatorBuilder<T>()
			.WithError(condition, errorMessage, failureInfoFunc)
			.Build();

	public static IValidator<T> WithPropertyError<T, TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> condition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null)
		=> new ValidatorBuilder<T>()
			.WithPropertyError(expression, condition, null, messageGetter, messageWithPropertyGetter, failureInfoFunc)
			.Build();

	public static IValidator<T> WithPropertyError<T, TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> condition,
		Func<ClientCondition<T>, IClientConditionDefinition>? clientCondition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null)
		=> new ValidatorBuilder<T>()
			.WithPropertyError(expression, condition, clientCondition, messageGetter, messageWithPropertyGetter, failureInfoFunc)
			.Build();
}

public class ValidatorFactory<T>
{
	public static IValidator<T> Create(Action<ValidatorBuilder<T>> configure)
	{
		if (configure == null)
			throw new ArgumentNullException(nameof(configure));

		var builder = new ValidatorBuilder<T>();
		configure.Invoke(builder);
		var validator = builder.Build();
		return validator;
	}

	public static IValidator<T> WithError(Func<T?, bool> condition, Func<T?, string> errorMessage, Func<T?, string>? failureInfoFunc = null)
		=> new ValidatorBuilder<T>()
			.WithError(condition, errorMessage, failureInfoFunc)
			.Build();

	public static IValidator<T> WithPropertyError<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> condition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null)
		=> new ValidatorBuilder<T>()
			.WithPropertyError(expression, condition, null, messageGetter, messageWithPropertyGetter, failureInfoFunc)
			.Build();

	public static IValidator<T> WithPropertyError<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> condition,
		Func<ClientCondition<T>, IClientConditionDefinition>? clientCondition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null)
		=> new ValidatorBuilder<T>()
			.WithPropertyError(expression, condition, clientCondition, messageGetter, messageWithPropertyGetter, failureInfoFunc)
			.Build();
}
