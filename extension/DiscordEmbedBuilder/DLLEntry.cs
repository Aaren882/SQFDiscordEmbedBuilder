using RGiesecke.DllExport;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordMessageAPI
{
    public class DllEntry
    {
        private static readonly string SessionKey = Tools.GenTimeEncode();
        private static bool InitComplete = false;

        #region Misc RVExtension Requirements
#if IS_x64
        [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionVersion@8", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtensionVersion(StringBuilder output, int outputSize)
        {
            outputSize--;
            output.Append("1.0.0");
        }

#if IS_x64
        [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtension@12", CallingConvention = CallingConvention.Winapi)]
#endif
        public static void RvExtension(StringBuilder output, int outputSize,
            [MarshalAs(UnmanagedType.LPStr)] string function)
        {
            outputSize--;
            if (function == "init")
            {
                if (!InitComplete)
                {
                    InitComplete = true;
                    //Tools.Logger(null, "Initialized");
                    output.Append(SessionKey);
                }
                else
                    Tools.Logger(null, "Attempted re-initialization");
            }
        }

#if IS_x64
        [DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
#else
        [DllExport("_RVExtensionArgs@20", CallingConvention = CallingConvention.Winapi)]
#endif
        #endregion
        public static int RvExtensionArgs(StringBuilder output, int outputSize,
            [MarshalAs(UnmanagedType.LPStr)] string inputKey,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
        {
            outputSize--;
            try
            {
                if (inputKey == SessionKey)
                {
                    if (args.Length == 8) // async without await because we don't expect a reply
                        Discord.HandleRequest(args);
                    else
                        output.Append("INCORRECT NUMBER OF ARGUMENTS");
                        
                }
                else
                {
                    Tools.Logger(null, $"Incorrect key used: {inputKey}");
                    output.Append("INCORRECT SESSION KEY");
                }
            }
            catch (Exception e)
            {
                Tools.Logger(e);
            };
            return 1;
        }
        private static List<List<string>> ParseStringToList(string input)
        {
            input = input.Trim('"');
            List<List<string>> result = new List<List<string>>();

            if (input.StartsWith("[[") && input.EndsWith("]]"))
            {
                // Remove the leading and trailing brackets
                input = input.Substring(2, input.Length - 4);

                // Split the string by "],["
                string[] innerLists = input.Split(new string[] { "],[" }, StringSplitOptions.None);

                foreach (string innerList in innerLists)
                {
                    // Split each inner list by ","
                    string[] elements = innerList.Split(',');

                    // Trim the quotes from each element and add to the result list
                    result.Add(elements.Select(e => e.Trim('"')).ToList());
                }
            }

            return result;
        }
    }
}
