namespace ChakraSharp_Sample
{
    using ChakraSharp;
    using System;
    using System.Diagnostics;

    class Program
    {
        static void Main(string[] args)
        {
            //This sample application demonstrates how to use the generated ChakraCore PInvoke layer to execute a sample script.
            //Although a higher level object model in .net can be easily created that eliminates the need for boilerplate code,
            //this sample focuses on direct, low-level usage of the ChakraCore API.


            //Create a new ChakraCore JS runtime.
            IntPtr runtimePtr;
            var result = ChakraCommon.JsCreateRuntime(_JsRuntimeAttributes.JsRuntimeAttributeNone, null, out runtimePtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Create a context
            IntPtr contextPtr;
            result = ChakraCommon.JsCreateContext(runtimePtr, out contextPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Set the current context on the runtime -- multiple contexts can be created, but only one can be active on a runtime at a time.
            result = ChakraCommon.JsSetCurrentContext(contextPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Create a string that contains a small description of our code. This string shows up when exceptions occur, in a full application this probably should indicate the physical location of the script.
            var sourceUrl = "[eval code]";
            IntPtr sourceUrlPtr;
            result = ChakraCore.JsCreateString(sourceUrl, (ulong)sourceUrl.Length, out sourceUrlPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Create the actual script that we're going to execute.
            var script = "6*7";
            IntPtr scriptPtr;
            result = ChakraCore.JsCreateString(script, (ulong)script.Length, out scriptPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Tell the JavaScript engine to execute our script, associating the sourceUrl we created with it.
            IntPtr resultPtr;
            result = ChakraCore.JsRun(scriptPtr, 0, sourceUrlPtr, _JsParseScriptAttributes.JsParseScriptAttributeNone, out resultPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Convert the result (which is a number) to a string.
            IntPtr stringResultPtr;
            result = ChakraCommon.JsConvertValueToString(resultPtr, out stringResultPtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Obtain and copy the string from the runtime to a local variable.
            string strResult;
            unsafe
            {
                uint written;
                sbyte[] buffer = new sbyte[0];
                ulong targetLength;
                //Determine the length of the string by invoking JsCopyString with a 0 length.
                fixed (sbyte* bufferPtr = buffer)
                {
                    result = ChakraCore.JsCopyString(stringResultPtr, bufferPtr, 0, out written);
                    Debug.Assert(result == _JsErrorCode.JsNoError);
                    targetLength = written;
                }

                //Create the buffer that will store the string and invoke JsCopyString with the length of our buffer.
                buffer = new sbyte[targetLength];

                fixed (sbyte* bufferPtr = buffer)
                {
                    result = ChakraCore.JsCopyString(stringResultPtr, bufferPtr, (ulong)buffer.LongLength, out written);
                    Debug.Assert(result == _JsErrorCode.JsNoError);

                    //Create a new string from the sbyte buffer.
                    strResult = new string(bufferPtr, 0, (int)written);
                }
            }

            //As we're all done, set the current context to nothing.
            result = ChakraCommon.JsSetCurrentContext(IntPtr.Zero);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Dispose of the runtime.
            result = ChakraCommon.JsDisposeRuntime(runtimePtr);
            Debug.Assert(result == _JsErrorCode.JsNoError);

            //Output what we got.
            Console.WriteLine(string.Format("The result of {0} is {1}", script, strResult));
            Console.WriteLine();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}
