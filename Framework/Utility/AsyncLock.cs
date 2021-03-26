using System;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Utility
{
    public class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<AsyncLock> LockAsync()
        {
            await _semaphoreSlim.WaitAsync();
            return this;
        }

        public int CurrentCount()
        {
            return _semaphoreSlim.CurrentCount;
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}
