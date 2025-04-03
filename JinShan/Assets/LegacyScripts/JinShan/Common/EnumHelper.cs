using System;

namespace JinShan
{
    public static class EnumHelper
    {
        public static T RandomEnum<T>()
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[new System.Random().Next(0, values.Length)];
        }
    }

}
