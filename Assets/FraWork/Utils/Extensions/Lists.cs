using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Namespace that contains extensions classes.
/// </summary>
namespace FraWork.Utils.Extensions
{
    /// <summary>
    /// Static class that extends the lists and add some new functions.
    /// </summary>
    public static class Lists
    {
        /// <summary>
        /// Struct used to save the runs in the merge algorithms
        /// </summary>
        private struct Run
        {
            public int BeginIndex { get; set; }
            public int EndIndex { get; set; }
        }

        /// <summary>
        /// Checks if the list is empty
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator</typeparam>
        /// <param name="_list">The list that you are checking</param>
        /// <returns></returns>
        public static bool Empty<T>(this List<T> _list) => _list.Count == 0;

        /// <summary>
        /// Copy one list to another
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_from">List that is gonna be copied from</param>
        /// <param name="_to">List that is gonna be copied to</param>
        private static void CopyList<T>(List<T> _from, List<T> _to) where T : IComparable
        {
            for (int i = 0; i < _to.Count; i++)
            {
                _to[i] = _from[i];
            }
        }

        #region SortingAlgorithms
        /// <summary>
        /// Implements the Bubble Sort on a List
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_list">The list that needs to be sorted</param>
        public static void BubbleSort<T>(this List<T> _list) where T : IComparable
        {
            T temp;

            for (int i = 0; i <= _list.Count - 2; i++)
            {
                for (int j = 0; j <= _list.Count - 2; j++)
                {
                    IComparable first = _list[j];
                    IComparable second = _list[j + 1];

                    int comparison = first.CompareTo(second);
                    // >0 means that first is after second
                    if (comparison > 0)
                    {
                        temp = _list[j + 1];
                        _list[j + 1] = _list[j];
                        _list[j] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Implements the Merge Sort on a List
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_list">The list that needs to be sorted</param>
        public static void MergeSort<T>(this List<T> _list) where T : IComparable
        {
            List<T> temp = new List<T>(new T[_list.Count]);

            for (int runLength = 1; runLength < _list.Count; runLength = runLength * 2)
            {
                for (int i = 0; i < _list.Count; i = i + 2 * runLength)
                {
                    Merge(_list, i, Mathf.Min(i + runLength, _list.Count), Mathf.Min(i + 2 * runLength, _list.Count), temp);
                }

                CopyList(temp, _list);
            }
        }

        /// <summary>
        /// Function that merges two runs
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_list">List that needs to be ordered</param>
        /// <param name="_iLeft">Index where the first run begins</param>
        /// <param name="_iRight">Index where the second run begins</param>
        /// <param name="_iEnd">Index where the second run ends</param>
        /// <param name="_temp">Helper temp list</param>
        private static void Merge<T>(List<T> _list, int _iLeft, int _iRight, int _iEnd, List<T> _temp) where T : IComparable
        {
            int i = _iLeft;
            int j = _iRight;

            for (int k = _iLeft; k < _iEnd; k++)
            {
                IComparable elementI = _list[i];
                IComparable elementJ;
                // trick to avoid IndexOutOfRangeException
                elementJ = j < _iEnd ? _list[j] : _list[i];

                if (i < _iRight && (j >= _iEnd || (elementI.CompareTo(elementJ) <= 0)))
                {
                    _temp[k] = _list[i];
                    i++;
                }
                else
                {
                    _temp[k] = _list[j];
                    j++;
                }
            }
        }

        /// <summary>
        /// Implements the Natural Merge Sort on a list
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_list">The list that needs to be sorted</param>
        public static void NaturalMergeSort<T>(this List<T> _list) where T : IComparable
        {
            List<T> temp = new List<T>(new T[_list.Count]);
            List<Run> runs = new List<Run>();
            CalculateRuns(_list, runs);
            CopyList(_list, temp);

            while (runs.Count > 1)
            {
                //DisplayRuns();

                for (int run = 0; run < runs.Count - 1; run += 2)
                {
                    NaturalMerge(_list, temp, runs[run], runs[run + 1]);
                    //DisplayTemperatures();
                }

                CopyList(temp, _list);
                runs.Clear();
                CalculateRuns(_list, runs);
            }
        }

        /// <summary>
        /// Calculate the runs of the Natural Merge Sort, i.e. every increasing sequence already present in the list
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_list">The list that needs to be sorted</param>
        /// <param name="_runs">The list of runs in the _list</param>
        private static void CalculateRuns<T>(List<T> _list, List<Run> _runs) where T : IComparable
        {
            int index = 0;

            // calculate the run indexes
            while (index < (_list.Count - 1))
            {
                Run run = new Run();
                run.BeginIndex = index;

                IComparable objectI = _list[index];
                IComparable objectJ = _list[index + 1];

                while (objectI.CompareTo(objectJ) <= 0)
                {
                    index++;

                    if (index == _list.Count - 1)
                        break;

                    objectI = _list[index];
                    objectJ = _list[index + 1];
                }

                run.EndIndex = index;
                _runs.Add(run);

                index++;

                if (index == _list.Count - 1)
                {
                    Run run2 = new Run();
                    run2.BeginIndex = run2.EndIndex = index;
                    _runs.Add(run2);
                }
            }
        }

        /// <summary>
        /// Function that merges two runs of the Natural Merge Sort
        /// </summary>
        /// <typeparam name="T">The type needs to have a comparator.</typeparam>
        /// <param name="_list">List that needs to be ordered</param>
        /// <param name="_temp">Helper temp list</param>
        /// <param name="_first">The first run that has to be merged</param>
        /// <param name="_second">The second run that has to be merged</param>
        private static void NaturalMerge<T>(List<T> _list, List<T> _temp, Run _first, Run _second) where T : IComparable
        {
            int i = _first.BeginIndex;
            int j = _second.BeginIndex;

            for (int k = i; k <= _second.EndIndex; k++)
            {
                IComparable objectI = _list[i];
                IComparable objectJ;
                // trick to avoid IndexOutOfRangeException
                objectJ = j <= _second.EndIndex ? _list[j] : _list[i];

                if (i < _second.BeginIndex && (j > _second.EndIndex || objectI.CompareTo(objectJ) <= 0))
                {
                    _temp[k] = _list[i];
                    i++;
                }
                else
                {
                    _temp[k] = _list[j];
                    j++;
                }
            }
        }

        #endregion

        #region SearchingAlgorithms
        /// <summary>
        /// Searching algorithm BinarySearch on Lists
        /// </summary>
        /// <typeparam name="T">Type of the objects in the List</typeparam>
        /// <typeparam name="C">Comparer for the object T type</typeparam>
        /// <param name="_list">The list that is being searched</param>
        /// <param name="_value">Value that you are searching</param>
        /// <returns>Index of the object searched, -1 if not found</returns>
        public static int BinarySearch<T, C>(this List<T> _list, T _value) where C : IComparer<T>, new()
        {
            int index = -1;

            int min = 0;
            int max = _list.Count;
            C comparer = new C();

            for (int i = 0; i < _list.Count; i++)
            {
                int midPoint = (min + max) / 2;

                int compared = comparer.Compare(_list[midPoint], _value);
                if (compared > 0)
                {
                    min = midPoint;
                    continue;
                }

                if (compared < 0)
                {
                    max = midPoint;
                    continue;
                }

                if (compared == 0)
                {
                    index = midPoint;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Linear search to find a value in the list
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list</typeparam>
        /// <param name="_list">List where the value is being searched</param>
        /// <param name="_value">Value that is searched</param>
        /// <returns></returns>
        public static int LinearSearch<T>(this List<T> _list, T _value) where T : IComparable
        {
            int index = -1;

            for (int i=0; i<_list.Count; i++)
            {
                T obj = _list[i];
                if (obj.CompareTo(_value) == 0)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// Jump Search to find a value in the list
        /// </summary>
        /// <typeparam name="T">Type of the objects in the list</typeparam>
        /// <param name="_list">List where the value is being searched</param>
        /// <param name="_value">Value that is searched</param>
        /// <param name="_blockSize">Size of the jump block</param>
        /// <returns></returns>
        public static int JumpSearch<T>(this List<T> _list, T _value, int? _blockSize = null) where T : IComparable
        {
            int blockSize = (_blockSize == null) ? Mathf.FloorToInt(Mathf.Sqrt(_list.Count)) : (int)_blockSize;
            int step = blockSize;

            T obj = _list[Math.Min(step, _list.Count-1)];
            int prev = 0;
            while (obj.CompareTo(_value) < 0)
            {
                prev = step;
                step += blockSize;
                if (prev >= _list.Count)
                    return -1;

                obj = _list[Math.Min(step, _list.Count-1)];
            }

            obj = _list[prev];
            while (obj.CompareTo(_value) < 0)
            {
                prev++;

                if (prev == Math.Min(step + 1, _list.Count))
                    return -1;

                obj = _list[prev];
            }

            obj = _list[prev];
            if (obj.CompareTo(_value) == 0)
                return prev;

            return -1;
        }

        #endregion
    }
}
