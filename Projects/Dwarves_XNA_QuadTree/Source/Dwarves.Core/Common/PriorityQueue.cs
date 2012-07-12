// ----------------------------------------------------------------------------
// <copyright file="PriorityQueue.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// <summary>
// Based on code by Gustavo Franco:
//   http://www.codeguru.com/csharp/csharp/cs_misc/designtechniques/article.php/c12527/AStar-A-Implementation-in-C-Path-Finding-PathFinder.htm
// </summary>
// ----------------------------------------------------------------------------
namespace Dwarves.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// A 'semi-sorted' list which always has the smallest element up front.
    /// </summary>
    /// <typeparam name="T">The item contained in the list.</typeparam>    
    public class PriorityQueue<T> where T : IIndexedObject
    {
        #region Private Variables

        /// <summary>
        /// The internal list.
        /// </summary>
        private List<T> innerList;

        /// <summary>
        /// The item comparer.
        /// </summary>
        private IComparer<T> comparer;

        #endregion

        #region Contructor

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class.
        /// </summary>
        public PriorityQueue()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class.
        /// </summary>
        /// <param name="comparer">The item comparer.</param>
        public PriorityQueue(IComparer<T> comparer)
        {
            this.comparer = comparer;
            this.innerList = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class.
        /// </summary>
        /// <param name="comparer">The item comparer.</param>
        /// <param name="capacity">The initial capacity of the collection.</param>
        public PriorityQueue(IComparer<T> comparer, int capacity)
        {
            this.comparer = comparer;
            this.innerList = new List<T>(capacity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return this.innerList.Count; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Push an item onto the priority queue.
        /// </summary>
        /// <param name="item">The new object</param>
        /// <returns>The index in the list where the object is now. This will change when objects are taken from or put
        /// onto the priority queue.</returns>
        public int Push(T item)
        {
            int p = this.innerList.Count, p2;
            item.Index = this.innerList.Count;
            this.innerList.Add(item);

            do
            {
                if (p == 0)
                {
                    break;
                }

                p2 = (p - 1) / 2;
                if (this.Compare(p, p2) < 0)
                {
                    this.SwitchElements(p, p2);
                    p = p2;
                }
                else
                {
                    break;
                }
            }
            while (true);

            return p;
        }

        /// <summary>
        /// Get the smallest item and remove it.
        /// </summary>
        /// <returns>The smallest item.</returns>
        public T Pop()
        {
            T result = this.innerList[0];
            int p = 0, p1, p2, pn;

            this.innerList[0] = this.innerList[this.innerList.Count - 1];
            this.innerList[0].Index = 0;
            this.innerList.RemoveAt(this.innerList.Count - 1);
            result.Index = -1;

            do
            {
                pn = p;
                p1 = (2 * p) + 1;
                p2 = (2 * p) + 2;
                if (this.innerList.Count > p1 && this.Compare(p, p1) > 0)
                {
                    p = p1;
                }

                if (this.innerList.Count > p2 && this.Compare(p, p2) > 0)
                {
                    p = p2;
                }

                if (p == pn)
                {
                    break;
                }

                this.SwitchElements(p, pn);
            }
            while (true);

            return result;
        }

        /// <summary>
        /// Notify the priority queue that the item has changed and the queue needs to restore order.
        /// </summary>
        /// <param name="item">The item that changed.</param>
        public void Update(T item)
        {
            int count = this.innerList.Count;

            // Usually we only need to switch some elements, since estimation won't change that much
            while ((item.Index - 1 >= 0) && (this.Compare(item.Index - 1, item.Index) > 0))
            {
                this.SwitchElements(item.Index - 1, item.Index);
            }

            while ((item.Index + 1 < count) && (this.Compare(item.Index + 1, item.Index) < 0))
            {
                this.SwitchElements(item.Index + 1, item.Index);
            }
        }

        /// <summary>
        /// Get the smallest object without removing it.
        /// </summary>
        /// <returns>The smallest object</returns>
        public T Peek()
        {
            if (this.innerList.Count > 0)
            {
                return this.innerList[0];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear()
        {
            this.innerList.Clear();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Switch the position of the two elements.
        /// </summary>
        /// <param name="i">The index of the first element.</param>
        /// <param name="j">The index of the second element.</param>
        protected void SwitchElements(int i, int j)
        {
            T h = this.innerList[i];
            this.innerList[i] = this.innerList[j];
            this.innerList[j] = h;

            this.innerList[i].Index = i;
            this.innerList[j].Index = j;
        }

        /// <summary>
        /// Compare the two elements.
        /// </summary>
        /// <param name="i">The index of the first element.</param>
        /// <param name="j">The index of the second element.</param>
        /// <returns>Integer that represents the relative values of the elements at i and j.</returns>
        protected virtual int Compare(int i, int j)
        {
            return this.comparer.Compare(this.innerList[i], this.innerList[j]);
        }

        #endregion
    }
}