using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace NAudio.Utils
{
    /// <summary>
    /// Support for Marshal Methods in both UWP and .NET 3.5
    /// </summary>
    public static class MarshalHelpers
    {
        /// <summary>
        /// SizeOf a structure
        /// </summary>
        public static int SizeOf<T>()
        {
#if NET35
            return Marshal.SizeOf(typeof (T));
#else
            return Marshal.SizeOf(typeof(T));
#endif
        }

        /// <summary>
        /// Offset of a field in a structure
        /// </summary>
        public static IntPtr OffsetOf<T>(string fieldName)
        {
#if NET35
            return Marshal.OffsetOf(typeof(T), fieldName);
#else
            return Marshal.OffsetOf(typeof(T), fieldName);
#endif
        }

        /// <summary>
        /// Pointer to Structure
        /// </summary>
        public static T PtrToStructure<T>(IntPtr pointer)
        {
#if NET35
            return (T)Marshal.PtrToStructure(pointer, typeof(T));
#else
            try
            {
                Type t = typeof(Marshal);
                MethodInfo f = t.GetMethod("PtrToStructure", new Type[] { typeof(IntPtr), typeof(Type) });
                return (T)f.Invoke(null, new object[] { pointer, typeof(T) });
            }
            catch
            {
                Type t = typeof(Marshal);
                MethodInfo f = t.GetMethod("PtrToStructure", new Type[] { typeof(IntPtr) });
                return (T)f.GetGenericMethodDefinition().MakeGenericMethod(typeof(T)).Invoke(null, new object[] { pointer });
            }
//            return Marshal.PtrToStructure<T>(pointer);
#endif
        }
    }
}
