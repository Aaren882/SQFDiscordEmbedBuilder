using RGiesecke.DllExport;
using System;
using System.IO;
using System.Linq;
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
            try
            {
                // Remove arma quotations
                args = args.Select(arg => arg.Trim('"', ' ').Replace("\"\"", "\"")).ToArray();

                //Entry
                switch (inputKey == "init_player" || inputKey == "Refresh_Webhooks")
                {
                    //- Init Functions 
                    case true:
                    {
                        // Use time as Key (for Server , Player)
                        if (ExtensionInit && inputKey != "Refresh_Webhooks")
                        {
                            output.Append("Extension has already been initiated.");
                            return -1;
                        }

                        // Get all Webhooks
                        if (inputKey == "Refresh_Webhooks")
                        {
                            string jsonString = Tools.ParseJson("Webhooks.json");
                            ALLWebhooks = JsonSerializer.Deserialize<Webhooks_Storage>(jsonString);
                            int webhooksCount = ALLWebhooks.Webhooks.Length;
                            int webhook_sel = Math.Min(Int32.Parse(args[0]), webhooksCount - 1);
                            ExtensionInit = true;

                            //- Exit if there's no Webhook
                            if (webhooksCount == 0)
                            {
                                output.Append("No Webhook Exist.");
                                return 0;
                            }

                            if (webhook_sel < 0) // output can be like ["ww", "ww"]
                                output.Append($"[[\"{string.Join("\",\"", ALLWebhooks.Webhooks)}\"],\"{InitTime}\"]");
                            else
                                output.Append($"[\"{ALLWebhooks.Webhooks[webhook_sel]}\",\"{InitTime}\"]");

                            return webhooksCount;
                        }
                        else //- Initation for Clients (Players)
                            InitTime = args[0];
                        break;
                    }
                    default:
                    {
                        if (inputKey == "ParseJson")
                        {
                            byte[] utf8 = Encoding.UTF8.GetBytes(Tools.ParseJson(args[0]));
                            string utf8_String = Encoding.UTF8.GetString(utf8)
                            output.Append(utf8_String);
                            break;
                        }
                        if (InitTime == null)
                        {
                            output.Append("Find No Key.");
                            break;
                        }

                        //- args[0] :
                            //- Http(s) Handlers ["url", HandlerType<int>, Optional :[Necessary Payload] ]
                        switch (inputKey)
                        {
                            //- Load Json as Message format
                            case "HandlerJson":
                            {
                                Discord.HandlerJson(args);
                                break;
                            }
                            case "HandlerJsonFormat":
                            {
                                Discord.HandlerJsonFormat(args);
                                break;
                            }
                            case "SendMessage":
                            {
                                if (argCount == 8) // async without await because we don't expect a reply
                                    Discord.HandleRequest(args);
                                else
                                {
                                    output.Append("INCORRECT NUMBER OF ARGUMENTS");
                                    return -2;
                                }
                                break;
                            }
                            default: //- Other conditions
                                break;
                        }

                        break; //- Exit
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                Tools.Logger(e,$"{e}");
                output.Append("Error!! Check Log.");
                return -11;
            }
        }
    }
}
