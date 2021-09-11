using System;
using System.Collections.Generic;

namespace GodsWalkAmongUs
{
    class ReuseableList<T> : List<T>, IDisposable
    {
        private static readonly List<ReuseableList<T>> pool = new List<ReuseableList<T>>();
        public static ReuseableList<T> GetInstance()
        {
            ReuseableList<T> list;
            if (0 < pool.Count)
            {
                list = pool.Pop();
                list.Clear();
            }
            else
            {
                list = new ReuseableList<T>(10);
            }
            return list;
        }

        public ReuseableList(int capacity) : base(capacity) {}

        void IDisposable.Dispose()
        {
            Clear();
            pool.Add(this);
        }
    }

    public static class ListExtensions
    {
        public static void SwapRemoveAt<T> (this IList<T> list, int index)
        {
            int end = list.Count - 1;
            list[index] = list[end];
            list.RemoveAt(end);
        }
        
        public static T Pop<T>(this IList<T> list)
        {
            int end = list.Count - 1;
            var value = list[end];
            list.RemoveAt(end);
            return value;
        }

        public static void Filter<T>(this IList<T> list, Func<T, bool> filter)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (!filter(list[i]))
                {
                    list.SwapRemoveAt(i--);
                }
            }
        }
    }
}