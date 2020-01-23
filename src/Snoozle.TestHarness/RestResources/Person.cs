using Snoozle.Core;
using Snoozle.RestResourceConfiguration;

namespace Snoozle.TestHarness.RestResources
{
    public class Person : IRestResource
    {
        public int Id { get; set; }
    }

    public class PersonResourceConfiguration : AbstractResourceConfigurationBuilder<Person>
    {
        public override void Configure()
        {

        }
    }
}
