using System;

namespace SDLWithOpenGL.Services
{
    /// <summary>
    /// Provides ability to manage an asynchrounous task.
    /// </summary>
    public interface ITaskManagerService : IDisposable
    {
        #region Props
        /// <summary>
        /// Gets a value indicating if the task is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Returns a value indicating if the task has been reqeusted to be cancelled.
        /// </summary>
        bool CancelRequested { get; }
        #endregion


        #region Methods
        /// <summary>
        /// Starts the task and executes the given <paramref name="taskAction"/>
        /// </summary>
        /// <param name="taskAction">The work to be performed by the task.</param>
        void StartTask(Action taskAction);


        /// <summary>s
        /// Stops the task.
        /// </summary>
        void StopTask();


        /// <summary>
        /// Waits for the current task to finish before continuing.
        /// </summary>
        void Wait();


        /// <summary>
        /// Waits for the current task to finish before continuing.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or Timeout.Inifinite (-1) to wait indefinitely.</param>
        void Wait(int millisecondsTimeout);
        #endregion
    }
}
