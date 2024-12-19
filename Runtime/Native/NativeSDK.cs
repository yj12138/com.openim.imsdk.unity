using System;
using System.IO;
using System.Runtime.InteropServices;
using Google.Protobuf;
using OpenIM.Proto;

namespace OpenIM.IMSDK.Native
{
    delegate void OnRecvEvent(IntPtr dataPtr, int len);
    class NativeSDK
    {
#if (UNITY_IPHONE || UNITY_TVOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        const string IMDLLName = "__Internal";
#else
        const string IMDLLName = "openimsdk";
#endif

        [DllImport(IMDLLName, CallingConvention = CallingConvention.Cdecl)]
        static extern void ffi_init(OnRecvEvent handler, int protocolType);

        [DllImport(IMDLLName, CallingConvention = CallingConvention.Cdecl)]
        static extern void ffi_request(byte[] data, int length);

        [DllImport(IMDLLName, CallingConvention = CallingConvention.Cdecl)]
        static extern void ffi_drop_handle(ulong handleId);


        public static void Init(OnRecvEvent handler)
        {
            // 1 :json 2: use Protobuf
            ffi_init(handler, 2);
        }
        static int operationId = 0;
        static string GetOperationId()
        {
            operationId++;
            return string.Format("{0}", operationId);
        }
        public static void CallAPI<T>(ulong handleId, FuncRequestEventName apiKey, T req) where T : IMessage
        {
            try
            {
                FfiRequest request = new FfiRequest()
                {
                    OperationID = GetOperationId(),
                    HandleID = handleId,
                    FuncName = apiKey,
                    Data = ByteString.CopyFrom(req.ToByteArray()),
                };
                byte[] byteArray;
                using (var memoryStream = new MemoryStream())
                {
                    request.WriteTo(memoryStream);
                    byteArray = memoryStream.ToArray();
                    ffi_request(byteArray, byteArray.Length);
                }
            }
            catch (Exception e)
            {
                Util.Utils.Log(e.ToString());
            }
        }
        public static void DropHandle(ulong handleId)
        {
            ffi_drop_handle(handleId);
        }
    }
}