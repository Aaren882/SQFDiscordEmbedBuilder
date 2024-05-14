using RGiesecke.DllExport;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace DiscordMessageAPI
{
    public class DllEntry
    {
        //private static readonly string SessionKey = Tools.GenTimeEncode();
        public static string InitTime = null;
        public static bool ExtensionInit = false;
        public static Webhooks_Storage ALLWebhooks = null;

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
            if (inputKey == "init_server" || inputKey == "init_player" || inputKey == "Refresh_Webhooks")
            {
                // Use time as Key (for Server , Player)
                if (ExtensionInit && inputKey != "Refresh_Webhooks")
                {
                    output.Append("Extension has already been initiated.");
                    return -1;
                }

                // Get all Webhooks
                if (inputKey == "init_server" || inputKey == "Refresh_Webhooks")
                {
                    string jsonString = File.ReadAllText($@"{Tools.AssemblyPath}\Webhooks.json");
                    ALLWebhooks = JsonSerializer.Deserialize<Webhooks_Storage>(jsonString);
                    int webhook_sel = Int32.Parse(args[0]);
                    ExtensionInit = true;

                    output.Append($"[\"{ALLWebhooks.Webhooks[webhook_sel]}\",\"{InitTime}\"]");
                }
                else //- Initation for Clients (Players)
                    InitTime = args[0];

                if (ALLWebhooks.Webhooks.Length == 0)
                {
                    output.Append("No Webhook Exist.");
                    return -12;
                }
                return 1;
            }
            else
            {
                if (inputKey == InitTime)
                {
                    if (argCount == 8) // async without await because we don't expect a reply
                    {
                        Discord.HandleRequest(args);
                    }
                    else
                    {
                        output.Append("INCORRECT NUMBER OF ARGUMENTS");
                        return -2;
                    }
                }
                else
                    output.Append("Find No Key.");
            }

            return 0;
            /*try
            {
                
            }
            catch (Exception e)
            {
                Tools.Logger(e,$"{e}");
                output.Append("Error!! Check Log.");
                return -11;
            }*/
        }
    }
}
