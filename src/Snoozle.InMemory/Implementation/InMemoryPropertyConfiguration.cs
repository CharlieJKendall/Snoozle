using Snoozle.Abstractions;
using System.Threading;

namespace Snoozle.InMemory.Implementation
{
    public class InMemoryPropertyConfiguration : BasePropertyConfiguration, IInMemoryPropertyConfiguration
    {
        private int _currentPrimaryKeyValue;

        public int GetNextPrimaryKeyValue()
        {
            return Interlocked.Increment(ref _currentPrimaryKeyValue);
        }
    }
}
