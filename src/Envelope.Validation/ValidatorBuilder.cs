using Envelope.Validation.Client;
using Envelope.Validation.Internal;
using Envelope.Validation.Validators;
using Envelope.Reflection.ObjectPaths;
using System.Linq.Expressions;

//#nullable disable

namespace Envelope.Validation;

//#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
public interface IValidatorBuilder<TBuilder, T> : IValidatorDescriptorBuilder
	where TBuilder : IValidatorBuilder<TBuilder, T>
{
	TBuilder Object(Validator<T> validator);

	IValidator<T> Build();

	public TBuilder ForProperty<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Action<PropertyValidator<T, TProperty>> propertyValidatorBuilder,
		Func<T?, bool>? serverCondition = null,
		Func<ClientCondition<T>, IClientConditionDefinition>? clientCondition = null,
		Func<T?, string>? failureInfoFunc = null);

	public TBuilder ForNavigation<TNavigation>(
		Expression<Func<T, TNavigation>> expression,
		Action<ValidatorBuilder<TNavigation>> validatorBuilder,
		Func<T?, bool>? serverCondition = null,
		Func<T?, string>? failureInfoFunc = null);

	public TBuilder ForEach<TItem>(
		Expression<Func<T, IEnumerable<TItem>>> expression,
		Action<ValidatorBuilder<TItem>> validatorBuilder,
		Func<T?, bool>? serverCondition = null,
		Func<T?, string>? detailInfoFunc = null);

	public TBuilder WithError(Func<T?, bool> serverCondition, Func<T?, string> errorMessage, Func<T?, string>? failureInfoFunc = null);

	public TBuilder WithPropertyError<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> serverCondition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null);

	public TBuilder WithPropertyError<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> serverCondition,
		Func<ClientCondition<T>, IClientConditionDefinition>? clientCondition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null);
}

public abstract class ValidatorBuilderBase<TBuilder, T> : IValidatorBuilder<TBuilder, T>, IValidatorDescriptorBuilder
	where TBuilder : ValidatorBuilderBase<TBuilder, T>
{
	protected readonly TBuilder _builder;
	protected Validator<T> _validator;

	public Type ObjectType { get; } = typeof(T);

	protected ValidatorBuilderBase(Validator<T> validator)
	{
		_validator = validator;
		_builder = (TBuilder)this;
	}

	public virtual TBuilder Object(Validator<T> validator)
	{
		_validator = validator;
		return _builder;
	}

	public IValidator<T> Build()
		=> _validator;

	public TBuilder ForProperty<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Action<PropertyValidator<T, TProperty>> propertyValidatorBuilder,
		Func<T?, bool>? serverCondition = null,
		Func<ClientCondition<T>, IClientConditionDefinition>? clientCondition = null,
		Func<T?, string>? failureInfoFunc = null)
	{
		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		if (propertyValidatorBuilder == null)
			throw new ArgumentNullException(nameof(propertyValidatorBuilder));

		var newObjectPath = _validator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp).AddProperty(expression);

		var cc = new ClientCondition<T>();
		var clientConditionDefinition = clientCondition?.Invoke(cc);

		var propertyValidator = new PropertyValidator<T, TProperty>(ValidatorType.PropertyValidator, PropertyAccessor.GetCachedAccessor(expression), newObjectPath, serverCondition, clientConditionDefinition, failureInfoFunc, null, null);

		((IValidator)_validator).AddValidatorInternal(propertyValidator);
		propertyValidatorBuilder.Invoke(propertyValidator);

		return _builder;
	}

	public TBuilder ForNavigation<TNavigation>(
		Expression<Func<T, TNavigation>> expression,
		Action<ValidatorBuilder<TNavigation>> validatorBuilder,
		Func<T?, bool>? serverCondition = null,
		Func<T?, string>? failureInfoFunc = null)
	{
		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		if (validatorBuilder == null)
			throw new ArgumentNullException(nameof(validatorBuilder));

		var newObjectPath = _validator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp).AddNavigation(expression);

		var navigationValidator = new NavigationValidator<T, TNavigation>(PropertyAccessor.GetCachedAccessor(expression), newObjectPath, serverCondition, failureInfoFunc);

		((IValidator)_validator).AddValidatorInternal(navigationValidator);
		validatorBuilder.Invoke(navigationValidator!);
		return _builder;
	}

	public TBuilder ForEach<TItem>(
		Expression<Func<T, IEnumerable<TItem>>> expression,
		Action<ValidatorBuilder<TItem>> validatorBuilder,
		Func<T?, bool>? serverCondition = null,
		Func<T?, string>? detailInfoFunc = null)
	{
		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		if (validatorBuilder == null)
			throw new ArgumentNullException(nameof(validatorBuilder));

		var newObjectPath = _validator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp).AddEnumerable(expression);

		var enumerableValidator = new EnumerableValidator<T, TItem>(PropertyAccessor.GetCachedAccessor(expression), newObjectPath, serverCondition, detailInfoFunc);

		((IValidator)_validator).AddValidatorInternal(enumerableValidator);
		validatorBuilder.Invoke(enumerableValidator!);
		return _builder;
	}

	public TBuilder WithError(Func<T?, bool> serverCondition, Func<T?, string> errorMessage, Func<T?, string>? failureInfoFunc = null)
	{
		if (serverCondition == null)
			throw new ArgumentNullException(nameof(serverCondition));

		if (errorMessage == null)
			throw new ArgumentNullException(nameof(errorMessage));

		var newObjectPath = _validator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp);

		var validator = new ErrorValidator<T>(newObjectPath, serverCondition, errorMessage, failureInfoFunc);
		((IValidator)_validator).AddValidatorInternal(validator);

		return _builder;
	}

	public TBuilder WithPropertyError<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> serverCondition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null)
		=> WithPropertyError(expression, serverCondition, null, messageGetter, messageWithPropertyGetter, failureInfoFunc);

	public TBuilder WithPropertyError<TProperty>(
		Expression<Func<T, TProperty>> expression,
		Func<T?, bool> serverCondition,
		Func<ClientCondition<T>, IClientConditionDefinition>? clientCondition,
		Func<T?, TProperty?, string, string?>? messageGetter,
		Func<T?, TProperty?, string, string?>? messageWithPropertyGetter = null,
		Func<T?, string>? failureInfoFunc = null)
	{
		if (expression == null)
			throw new ArgumentNullException(nameof(expression));

		if (serverCondition == null)
			throw new ArgumentNullException(nameof(serverCondition));

		if (messageGetter == null)
			throw new ArgumentNullException(nameof(messageGetter));

		var newObjectPath = _validator.ObjectPath.Clone(ObjectPathCloneMode.BottomUp).AddProperty(expression);

		var cc = new ClientCondition<T>();
		var clientConditionDefinition = clientCondition?.Invoke(cc);

		var errorValidator =
			new Validators.PropertyValidators.ErrorValidator<T, TProperty>(
				PropertyAccessor.GetCachedAccessor(expression),
				newObjectPath,
				serverCondition,
				clientConditionDefinition,
				failureInfoFunc,
				messageGetter,
				messageWithPropertyGetter ?? messageGetter);

		((IValidator)_validator).AddValidatorInternal(errorValidator);

		return _builder;
	}

	public virtual IValidatorDescriptor ToDescriptor(IServiceProvider serviceProvider)
		=> Build().ToDescriptor();

	public virtual IValidatorDescriptor ToDescriptor(IServiceProvider serviceProvider, object? state = null)
		=> Build().ToDescriptor();
}

public class ValidatorBuilder<T> : ValidatorBuilderBase<ValidatorBuilder<T>, T>
{
	public ValidatorBuilder()
		: base(new Validator<T>(ValidatorType.Validator, ObjectPath<T>.Create(), false, null))
	{
	}

	internal protected ValidatorBuilder(Validator<T> validator)
		: base(validator)
	{
	}

	public static implicit operator Validator<T>?(ValidatorBuilder<T> builder)
	{
		if (builder == null)
			return null;

		return builder._validator;
	}

	public static implicit operator ValidatorBuilder<T>?(Validator<T> validator)
	{
		if (validator == null)
			return null;

		return new ValidatorBuilder<T>(validator);
	}
}
//#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
