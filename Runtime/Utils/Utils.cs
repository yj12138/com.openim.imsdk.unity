using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;
namespace OpenIM.IMSDK.Unity.Util
{
    public static class Utils
    {

        static string imLogPrefix = "[<color=#0000FF>IMSDK</color>]";
        static string imErrPrefix = "[<color=#FF0000>IMSDK</color>]";
        public static void Log(string str)
        {
#if IMSDK_LOG_ENABLE
            UnityEngine.Debug.Log(imLogPrefix + str);
#endif
        }

        public static void Error(string str)
        {
#if IMSDK_LOG_ENABLE
            UnityEngine.Debug.LogError(imErrPrefix + str);
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