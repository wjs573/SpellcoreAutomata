using System;
using System.Collections.Generic;

namespace JinShan
{
    public static class ArrayHelper
    {
        //查找满足条件的所有对象
        public static T[] FindAll<T>(this T[] array, Func<T, bool> conditoin)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                if (conditoin(array[i]))
                {
                    list.Add(array[i]);
                }
            }
            return list.ToArray();
        }

        //查找
        public static T Find<T>(this T[] array, Func<T, bool> conditoin)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (conditoin(array[i]))
                {
                    return array[i];
                }
            }
            return default(T);
        }

        //排序 升序
        public static void OrderByAsc<T, Q>(this T[] arr, Func<T, Q> condition) where Q : IComparable
        {

            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (condition(arr[j]).CompareTo(condition(arr[j + 1])) > 0)
                    {
                        T temp = arr[j + 1];
                        arr[j + 1] = arr[j];
                        arr[j] = temp;
                    }
                }

            }

        }

        public static void OrderByDes<T, Q>(this T[] arr, Func<T, Q> condition) where Q : IComparable
        {

            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (condition(arr[j]).CompareTo(condition(arr[j + 1])) < 0)
                    {
                        T temp = arr[j + 1];
                        arr[j + 1] = arr[j];
                        arr[j] = temp;
                    }
                }

            }

        }


        //最大值 最小值
        public static T GetMax<T, Q>(this T[] array, Func<T, Q> conditoin) where Q : IComparable
        {

            T max = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (conditoin(max).CompareTo(conditoin(array[i])) < 0)
                {
                    max = array[i];
                }
            }
            return max;
        }

        public static T GetMin<T, Q>(this T[] array, Func<T, Q> conditoin) where Q : IComparable
        {

            T min = array[0];
            for (int i = 0; i < array.Length; i++)
            {
                if (conditoin(min).CompareTo(conditoin(array[i])) > 0)
                {
                    min = array[i];
                }
            }
            return min;
        }


        //筛选 设置条件 筛选出数组中的元素
        public static Q[] Select<T, Q>(this T[] array, Func<T, Q> condition)
        {
            Q[] result = new Q[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = condition(array[i]);

            }

            return result;
        }
    }
}


