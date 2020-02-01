namespace Snoozle.Abstractions
{
    public interface IPropertyConfigurationBuilderCore<TProperty, TPropertyConfiguration>
    {
        TPropertyConfiguration PropertyConfiguration { get; }
    }

    public interface IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> : IPropertyConfigurationBuilderCore<TProperty, TPropertyConfiguration>
    {
        IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsReadOnlyColumn();

        IPropertyConfigurationBuilder<TProperty, TPropertyConfiguration> IsPrimaryIdentifier();

        IComputedValueBuilder<TProperty, TPropertyConfiguration> HasComputedValue();
    }

    public interface IComputedValueBuilder<TProperty, TPropertyConfiguration> : IPropertyConfigurationBuilderCore<TProperty, TPropertyConfiguration>
    {
    }
}
