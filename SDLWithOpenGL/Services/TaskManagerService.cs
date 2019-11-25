using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SDLWithOpenGL.Services
{
    /// <summary>
    /// Provides the ability to manage an asynchrounous task.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TaskManagerService : ITaskManagerService
    {
        #region Privte Fields
        private Task? _loopTask;
        private CancellationTokenSource _tokenSrc;
        private bool _disposed;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="TaskManagerService"/>.
        /// </summary>
        public TaskManagerService() => _tokenSrc = new CancellationTokenSource();
        #endregion


        #region Props
        /// <summary>
        /// Gets a value indicating if the task is currently running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_loopTask == null)
                    return false;

                if (_loopTask.Status == TaskStatus.WaitingToRun || _loopTask.Status == TaskStatus.Running)
                    return true;

                if (_tokenSrc != null && !_tokenSrc.IsCancellationRequested)
                    return true;


                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating if the task has been reqeusted to be cancelled.
        /// </summary>
        public bool CancelRequested => _tokenSrc != null && _tokenSrc.IsCancellationRequested;
        #endregion


        #region Public Methods
        /// <summary>
        /// Starts the task and executes the given <paramref name="taskAction"/>.
        /// </summary>
        /// <param name="taskAction">The work to be performed by the task.</param>
        public void StartTask(Action taskAction)
        {
            if (IsRunning)
                return;

            _tokenSrc = new CancellationTokenSource();
            _loopTask = new Task(taskAction, _tokenSrc.Token);
            _loopTask.Start();
        }


        /// <summary>
        /// Stops the task.
        /// </summary>
        public void StopTask() => _tokenSrc?.Cancel();


        /// <summary>
        /// Disposes of the <see cref="TaskManagerService"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disposes of the internal resources if the given <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">True to dispose of internal resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            //Free managed resources
            if (disposing)
            {
                _tokenSrc?.Cancel();
                _tokenSrc?.Dispose();

                _loopTask?.Dispose();
                _loopTask = null;
            }

            _disposed = true;
        }


        /// <summary>
        /// Waits for the current task to finish before continuing.
        /// </summary>
        public void Wait() => Wait(0);


        /// <summary>
        /// Waits for the current task to finish before continuing.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Inifinite (-1) to wait indefinitely.</param>
        public void Wait(int millisecondsTimeout)
        {
            if (_loopTask?.Status == TaskStatus.Running)
            {
                if (millisecondsTimeout <= 0)
                    _loopTask?.Wait();
                else
                    _loopTask?.Wait(millisecondsTimeout);
            }
        }
        #endregion
    }
}
