using Snoozle.Abstractions;
using Snoozle.SqlServer.Implementation;

namespace Snoozle.TestHarness.RestResources
{
    public class Person : IRestResource
    {
        public int Id { get; set; }
    }

    public class PersonResourceConfiguration : SqlResourceConfigurationBuilder<Person>
    {
        public override void Configure()
        {
        }
    }
}
