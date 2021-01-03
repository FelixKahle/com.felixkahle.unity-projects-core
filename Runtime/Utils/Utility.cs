// Copyright 2020 Felix Kahle. All rights reserved.

using Unity.Mathematics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Collections;

namespace FelixKahle.UnityProjectsCore
{
    /// <summary>
    /// Collection of utility functions.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Shuffles an array.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShuffleArray<T>(ref T[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                T tempItem = array[i];
                int randomIndex = UnityEngine.Random.Range(i, array.Length);
                array[i] = array[randomIndex];
                array[randomIndex] = tempItem;
            }
        }

        /// <summary>
        /// Shuffles a List.
        /// </summary>
        /// <typeparam name="T">The type of the elements inside the list.</typeparam>
        /// <param name="array">The list to shuffle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShuffleList<T>(ref List<T> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                T tempItem = list[i];
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = tempItem;
            }
        }

        public static void EraseSwap<T>(ref List<T> list, int index)
        {
            int lastIndex = list.Count - 1;
            list[index] = list[lastIndex];
            list.RemoveAt(lastIndex);
        }

        /// <summary>
        /// Returns the index of an element inside an array.
        /// </summary>
        /// <typeparam name="T">The type of the array elements and the element to find the index of in the array.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="needle">The element to find the index of.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(ref T[] array, T needle)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(needle))
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// Clears an array.
        /// </summary>
        /// <typeparam name="T">The type of the array elements.</typeparam>
        /// <param name="array">The array to clear.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(ref T[] array)
        {
            System.Array.Clear(array, 0, array.Length);
        }

        //
        // Array extension methods.
        //

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AfterLast(this string str, string sub)
        {
            int idx = str.LastIndexOf(sub);
            return idx < 0 ? "" : str.Substring(idx + sub.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BeforeLast(this string str, string sub)
        {
            int idx = str.LastIndexOf(sub);
            return idx < 0 ? "" : str.Substring(0, idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AfterFirst(this string str, string sub)
        {
            int idx = str.IndexOf(sub);
            return idx < 0 ? "" : str.Substring(idx + sub.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BeforeFirst(this string str, string sub)
        {
            int idx = str.IndexOf(sub);
            return idx < 0 ? "" : str.Substring(0, idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PrefixMatch(this string str, string prefix)
        {
            int l = 0, slen = str.Length, plen = prefix.Length;
            while (l < slen && l < plen)
            {
                if (str[l] != prefix[l])
                {
                    break;
                }
                l++;
            }
            return l;
        }
    }
}
