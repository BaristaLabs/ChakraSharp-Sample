namespace ChakraSharp_Sample
{
    using ChakraSharp;
    using System;
    using System.Diagnostics;

    class Program
    {
        static void Main(string[] args)
        {
            IntPtr runtimePtr;
            var result = ChakraCommon.JsCreateRuntime(_JsRuntimeAttributes.JsRuntimeAttributeNone, null, out runtimePtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            IntPtr contextPtr;
            result = ChakraCommon.JsCreateContext(runtimePtr, out contextPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            result = ChakraCommon.JsSetCurrentContext(contextPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            var sourceUrl = "[eval code]";
            IntPtr sourceUrlPtr;
            result = ChakraCore.JsCreateString(sourceUrl, (ulong)sourceUrl.Length, out sourceUrlPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            var script = "6*7";
            IntPtr scriptPtr;
            result = ChakraCore.JsCreateString(script, (ulong)script.Length, out scriptPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            IntPtr resultPtr;
            result = ChakraCore.JsRun(scriptPtr, 0, sourceUrlPtr, _JsParseScriptAttributes.JsParseScriptAttributeNone, out resultPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            IntPtr stringResultPtr;
            result = ChakraCommon.JsConvertValueToString(resultPtr, out stringResultPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            string strResult;
            unsafe
            {
                ulong written = 0;
                sbyte[] buffer = new sbyte[100];
                fixed (sbyte* bufferPtr = buffer)
                {
                    result = ChakraCore.JsCopyString(stringResultPtr, bufferPtr, (ulong)buffer.LongLength, ref written);
                    Debug.Assert(result == _JsErrorCode.JsNoError);

                    strResult = new string(bufferPtr, 0, (int)written);
                }
            }

            result = ChakraCommon.JsSetCurrentContext(IntPtr.Zero);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            result = ChakraCommon.JsDisposeRuntime(runtimePtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            Console.WriteLine(string.Format("The result of {0} is {1}", script, strResult));
            Console.WriteLine();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}
