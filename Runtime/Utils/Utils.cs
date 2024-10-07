using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
namespace OpenIM.IMSDK.Unity.Util
{
    public static class Utils
    {
        public static void Log(params object[] args)
        {
#if IMSDK_LOG_ENABLE
            var str = "IMSDK:";
            foreach (var v in args)
            {
                str += v.ToString() + " ";
            }
            UnityEngine.Debug.Log(str);
#endif
        }

        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static IntPtr String2IntPtr(string str)
        {
            if (str == null)
            {
                return Marshal.StringToHGlobalAnsi("");
            }
            return Marshal.StringToHGlobalAnsi(str);
        }
        public static string IntPtr2String(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return "";
            }
            return Marshal.PtrToStringAnsi(ptr) ?? "";
        }
        private static int randomIndex = 0;
        public static string GetOperationIndex()
        {
            return string.Format("{0}", randomIndex++);
        }
    }
};