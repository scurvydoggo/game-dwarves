using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Performs parallel processing of queued tasks.
/// </summary>
/// <typeparam name="T">The work parameter type.</typeparam>
public class ParallelWorker<T> : IDisposable
{
    #region Private Members

    /// <summary>
    /// The worker threads.
    /// </summary>
    private Thread[] threads;

    /// <summary>
    /// The queued work.
    /// </summary>
    private Queue<Work> queuedWork;

    /// <summary>
    /// The executing work.
    /// </summary>
    private List<Work> executingWork;

    /// <summary>
    /// Synchronisation structure.
    /// </summary>
    private QueuedWorkerEvents syncEvents;

    /// <summary>
    /// Indicates whether the instance has been disposed.
    /// </summary>
    private bool isDisposed;

    #endregion

    #region Constructor and Finalizer

    /// <summary>
    /// Initializes a new instance of the ParallelWorker class.
    /// </summary>
    /// <param name="numberOfWorkerThreads">The number of worker threads used in this instance.</param>
    public ParallelWorker(int numberOfWorkerThreads)
    {
        if (numberOfWorkerThreads < 1)
        {
            throw new ArgumentOutOfRangeException("Number of worker threads cannot be less than 1.");
        }

        this.NumberOfWorkerThreads = numberOfWorkerThreads;

        this.threads = new Thread[numberOfWorkerThreads];
        this.queuedWork = new Queue<Work>();
        this.executingWork = new List<Work>();
        this.syncEvents = new QueuedWorkerEvents();
        this.isDisposed = false;

        // Spawn the worker threads
        for (int i = 0; i < this.NumberOfWorkerThreads; i++)
        {
            this.threads[i] = new Thread(this.DoWork);
            this.threads[i].IsBackground = true;
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Finalizes an instance of the ParallelWorker class.
    /// </summary>
    ~ParallelWorker()
    {
        // Call dispose in finalizer as a fall-back in case this hasn't been done
        this.Dispose(false);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of worker threads used in this instance.
    /// </summary>
    public int NumberOfWorkerThreads { get; private set; }

    #endregion

    #region Disposal

    /// <summary>
    /// Dispose the instance.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Dispose the instance.
    /// </summary>
    /// <param name="waitForThreadExit">Indicates that the calling thread will block until all worker threads have
    /// exited.</param>
    public void Dispose(bool waitForThreadExit)
    {
        if (!this.isDisposed)
        {
            // Signal the exit thread event
            if (this.syncEvents.ExitThreadEvent != null)
            {
                this.syncEvents.ExitThreadEvent.Set();
            }

            if (waitForThreadExit)
            {
                // Wait for the worker threads to complete
                foreach (Thread thread in this.threads)
                {
                    thread.Join();
                }
            }

            // Dispose the event handles
            this.syncEvents.Dispose();

            this.isDisposed = true;
        }
    }

    #endregion

    #region Queue-related Methods

    /// <summary>
    /// Enqueue a work item with an optional parameter.
    /// <para />
    /// As the parameter will be accessed from the worker thread, ensure that use of the parameter in workDelegate is
    /// thread-safe.
    /// </summary>
    /// <param name="workDelegate">The delegate method for executing the work.</param>
    /// <param name="parameter">The parameter that is executed for the delegate method.</param>
    public void Enqueue(Action<T> workDelegate, T parameter = default(T))
    {
        // Queue the work item
        lock ((this.queuedWork as ICollection).SyncRoot)
        {
            this.queuedWork.Enqueue(new Work(workDelegate, parameter));
        }

        // Signal that an item has been queued
        this.syncEvents.NewWorkEvent.Set();
    }

    /// <summary>
    /// Clear the queued work.
    /// </summary>
    public void ClearQueue()
    {
        lock ((this.queuedWork as ICollection).SyncRoot)
        {
            this.queuedWork.Clear();
        }
    }

    /// <summary>
    /// Gets an array of the work items that are queued at this point in time.
    /// </summary>
    /// <returns>The queued work.</returns>
    public Work[] GetQueuedWork()
    {
        lock ((this.queuedWork as ICollection).SyncRoot)
        {
            return this.queuedWork.ToArray();
        }
    }

    /// <summary>
    /// Gets an array of the work items that are executing at this point in time.
    /// </summary>
    /// <returns>The currently executing work.</returns>
    public Work[] GetExecutingWork()
    {
        lock ((this.executingWork as ICollection).SyncRoot)
        {
            return this.executingWork.ToArray();
        }
    }

    #endregion

    #region Work Execution

    /// <summary>
    /// This is the method used by the worker threads. The method waits for work to become available and then executes
    /// the work delegate. At completion, it then sleeps until more work is available.
    /// </summary>
    private void DoWork()
    {
        while (this.WaitForWork())
        {
            // Dequeue a work item
            Work work = null;
            lock ((this.queuedWork as ICollection).SyncRoot)
            {
                if (this.queuedWork.Count > 0)
                {
                    try
                    {
                        work = this.queuedWork.Dequeue();
                    }
                    catch (InvalidOperationException)
                    {
                        // Thrown if the queue is empty. Do nothing as the thread will just go back to sleep.
                    }
                }
            }

            // Execute the work
            if (work != null)
            {
                lock ((this.executingWork as ICollection).SyncRoot)
                {
                    this.executingWork.Add(work);
                }

                work.WorkDelegate(work.Parameter);

                lock ((this.executingWork as ICollection).SyncRoot)
                {
                    this.executingWork.Remove(work);
                }
            }
        }
    }

    /// <summary>
    /// Wait for an event to occur.
    /// </summary>
    /// <returns>True if work becomes available; False if the worker thread should exit.</returns>
    private bool WaitForWork()
    {
        if (this.syncEvents.IsDisposed)
        {
            return false;
        }

        // Check if there is work in the queue, in which case start immediately
        lock ((this.queuedWork as ICollection).SyncRoot)
        {
            if (this.queuedWork.Count > 0)
            {
                return true;
            }
        }

        // There is no work in the queue, so wait for a queue event to be set
        try
        {
            // Wait for an event and return true if it was a new work event
            return WaitHandle.WaitAny(this.syncEvents.EventArray) == QueuedWorkerEvents.NewWorkEventIndex;
        }
        catch
        {
            // A wait handle must have been disposed so return false
            return false;
        }
    }

    #endregion

    #region Inner Classes

    /// <summary>
    /// Represents an item of work to be executed.
    /// </summary>
    public class Work
    {
        /// <summary>
        /// Gets the delegate method for executing the work.
        /// </summary>
        public Action<T> WorkDelegate { get; private set; }

        /// <summary>
        /// Gets the parameter that is executed for the delegate method.
        /// </summary>
        public T Parameter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Work class.
        /// </summary>
        /// <param name="workDelegate">The delegate method for executing the work.</param>
        /// <param name="parameter">The parameter that is executed for the delegate method.</param>
        public Work(Action<T> workDelegate, T parameter)
        {
            this.WorkDelegate = workDelegate;
            this.Parameter = parameter;
        }
    }

    /// <summary>
    /// Manages the wait handles used to synchonise an instance of ParallelWorker.
    /// </summary>
    private class QueuedWorkerEvents : IDisposable
    {
        /// <summary>
        /// The index of the new work event in EventArray.
        /// </summary>
        public const int NewWorkEventIndex = 0;

        /// <summary>
        /// Gets the wait handle that signals new work added to the queue.
        /// </summary>
        public EventWaitHandle NewWorkEvent { get; private set; }

        /// <summary>
        /// Gets the wait handle that signals worker threads should exit.
        /// </summary>
        public EventWaitHandle ExitThreadEvent { get; private set; }

        /// <summary>
        /// Gets an array of the NewWorkItemEvetn and ExitThreadEvent handles. This is used by used by
        /// WaitHandle.WaitAny.
        /// </summary>
        public EventWaitHandle[] EventArray { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the QueuedWorkerEvents class.
        /// </summary>
        public QueuedWorkerEvents()
        {
            this.NewWorkEvent = new AutoResetEvent(false);
            this.ExitThreadEvent = new ManualResetEvent(false);
            this.EventArray = new EventWaitHandle[] { NewWorkEvent, ExitThreadEvent };
            this.IsDisposed = false;
        }

        /// <summary>
        /// Dispose the instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                this.IsDisposed = true;
				
                if (this.NewWorkEvent != null)
                {
                    this.NewWorkEvent.Close();
                    this.NewWorkEvent = null;
                }

                if (this.ExitThreadEvent != null)
                {
                    this.ExitThreadEvent.Close();
                    this.ExitThreadEvent = null;
                }
            }
        }
    }

    #endregion
}