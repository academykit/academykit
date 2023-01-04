namespace Lingtren.Infrastructure.Common
{
    /// <summary>
    /// Disposable
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Terminator
        /// </summary>
        ~Disposable()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            Dispose(true);
            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// release
        /// </summary>
        /// <param name="disposing"></param>
        public abstract void Dispose(bool disposing);
    }
}