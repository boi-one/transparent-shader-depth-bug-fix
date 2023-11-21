using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

    public static class ListExtensions
    {
        /// <summary>
        /// Checks if the list is empty 
        /// </summary>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IList<T> target)
        {
            return target.Count == 0;
        }

        /// <summary>
        /// Checks if compareTarget includes target List.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="compareTarget"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Contains<T>(this IList<T> target, IList<T> compareTarget)
        {
            foreach (var tag in compareTarget)
            {
                if (!target.Contains(tag)) return false;
            }
            
            return true;
        }

        /// <summary>
        /// Shuffle the list 
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    
        /// <summary>
        /// Return a random item from the list.
        /// Sampling with replacement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list)
        {
            if (list.IsEmpty()) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Removes a random item from the list, returning that item.
        /// Sampling without replacement.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RemoveRandom<T>(this IList<T> list)
        {
            if (list.IsEmpty()) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
            int index = UnityEngine.Random.Range(0, list.Count);
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(this IList<T> list, int index)
        {
            if (index < 0) index = list.Count + index;
            else if (index > list.Count - 1) index = index % list.Count;

            return list[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetAt<T>(this IList<T> list, int index, T item)
        {
            if (index < 0) index = list.Count + index;
            else if (index > list.Count - 1) index = index % list.Count;

            list.Insert(index, item);
        }

        /// <summary>
        /// Add list from same type to current list 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="otherList"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddList<T>(this IList<T> list, List<T> otherList)
        {
            foreach (var item in otherList)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// remove list from same type from current list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="otherList"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveList<T>(this IList<T> list, IList<T> otherList)
        {
            foreach (var item in otherList)
            {
                if(!list.Contains(item)) continue;
                bool removed = list.Remove(item);
            }
        }
        
    }