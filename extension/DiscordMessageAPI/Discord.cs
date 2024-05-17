using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DiscordMessageAPI
{
    static class Discord
    {
        internal static async void HandlerJson(string[] args)
        {
            //- ["url","json"]
            try 
            {
                string url = Tools.DecryptString(args[0]);
                string json = Tools.ParseJson(args[1]);
                using (MultipartFormDataContent package = new MultipartFormDataContent())
                {
                    package.Add(new StringContent(json, Encoding.UTF8), "payload_json");
                    await DiscordMsg(url, package);
                }
            }
            catch (Exception e)
            {
                Tools.Logger(e,$"{e}");
            }
        }
        internal static async void HandlerJsonFormat(string[] args)
        {
            //- ["url","json"]
            try 
            {
                string url = Tools.DecryptString(args[0]);
                string json = args[1];
                using (MultipartFormDataContent package = new MultipartFormDataContent())
                {
                    Tools.Logger(null, json);
                    package.Add(new StringContent(json, Encoding.UTF8), "payload_json");
                    await DiscordMsg(url, package);
                }
            }
            catch (Exception e)
            {
                Tools.Logger(e,$"{e}");
            }
        }
        internal static async void HandleRequest(string[] args)
        {
            try
            {
                using (MultipartFormDataContent package = new MultipartFormDataContent())
                {
                    string url = Tools.DecryptString(args[0]);
                    string content = args[1];
                    string username = args[2];
                    string avatar = args[3];
                    string tts = args[4];

                    //- File Stream
                    string filePath = $"{args[5]}";

                    // Discord 2000 character limit
                    if (content.Length > 1999) content = content.Substring(0, 1999);

                    // Build embeds array
                    //- Turn Data into List<List<string>> e.g [ ["TITLE","DESC"] , ["11","22] ]
                    List<List<string>> embedsData = ParseStringToList(args[6]);
                    List<List<string>> FieldsData = ParseStringToList(args[7].Replace("[[]]", ""));

                    foreach (var embed in embedsData)
                    {
                        Resize(embed, 11, "");
                        foreach (var field in FieldsData)
                        {
                            Resize(field, 3, "");
                            embed.AddRange(field);
                        }
                    }

                    //- pass Data into "class Types.EmbedData"
                    List<Types.EmbedData> embeds = embedsData.Select(data => new Types.EmbedData(data)).ToList();

                    // Prepare the embeds JSON data
                    string embedsJson = BuildEmbedsJson(embeds);

                    // Bare bones
                    package.Add(new StringContent(content), "content");
                    package.Add(new StringContent(tts), "tts");

                    //- Send File .png
                    if (filePath.Length > 0)
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                        {
                            byte[] fileBytes = new byte[fileStream.Length];
                            await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length);
                            package.Add(new ByteArrayContent(fileBytes), "file", Path.GetFileName(filePath));
                        }
                    }
                    if (username.Length > 0) package.Add(new StringContent(username), "username");
                    if (avatar.Length > 0) package.Add(new StringContent(avatar), "avatar_url");
                    if (embeds.Count > 0) package.Add(new StringContent(embedsJson, Encoding.UTF8), "payload_json");

                    await DiscordMsg(url, package);
                }
            }
            catch (Exception e)
            {
                Tools.Logger(e);
            }
        }

        internal static async Task DiscordMsg(string url, MultipartFormDataContent package)
        {
            // Execute webhook
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                 SecurityProtocolType.Tls |
                 SecurityProtocolType.Tls11 |
                 SecurityProtocolType.Ssl3;

            using (HttpClient APIClient = new HttpClient())
            {
                url = $"https://discord.com/api/webhooks/{url}";
                HttpResponseMessage response = await APIClient.PostAsync(url, package);
            }
        }

        private static void Resize<T>(this List<T> list, int sz, T c)
        {
            int cur = list.Count;
            if (sz < cur)
                list.RemoveRange(sz, cur - sz);
            else if (sz > cur)
            {
                if (sz > list.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                    list.Capacity = sz;
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }

        //- Translating Data
        private static List<List<string>> ParseStringToList(string input)
        {
            List<List<string>> result = new List<List<string>>();

            if (input.StartsWith("[[") && input.EndsWith("]]"))
            {
                input = input.Substring(2, input.Length - 4);

                string[] innerLists = input.Split(new string[] { "],[" }, StringSplitOptions.None);

                foreach (string innerList in innerLists)
                {
                    string[] elements = innerList.Split(',');
                    List<string> innerResult = new List<string>();

                    foreach (string element in elements)
                    {
                        // Convert each element to string and add to the inner list
                        innerResult.Add(element.Trim('"', '[', ']'));
                    }

                    result.Add(innerResult);
                }
            }

            return result;
        }

        private static string BuildEmbedsJson(List<Types.EmbedData> embeds)
        {
            var embedsJson = new StringBuilder();
            embedsJson.Append("{ \"embeds\": [");

            foreach (var embed in embeds)
            {
                embedsJson.Append(BuildEmbedJson(embed));
                embedsJson.Append(",");
            }

            // Remove the last comma
            if (embedsJson[embedsJson.Length - 1] == ',')
            {
                embedsJson.Remove(embedsJson.Length - 1, 1);
            }

            embedsJson.Append("]}");

            return embedsJson.ToString();
        }

        private static string BuildEmbedJson(Types.EmbedData embed)
        {
            return $@"
            {{
                ""title"": ""{embed.Title}"",
                ""description"": ""{embed.Description}"",
                ""color"": ""{embed.Color}"",
                ""timestamp"": ""{embed.timestamp}"",
                ""author"": {{
                ""name"": ""{embed.AuthorName}"",
                ""url"": ""{embed.AuthorUrl}"",
                ""icon_url"": ""{embed.AuthorIconUrl}""
                }},
                ""image"": {{
                ""url"": ""{embed.ImageUrl}""
                }},
                ""thumbnail"": {{
                ""url"": ""{embed.ThumbnailUrl}""
                }},
                ""footer"": {{
                ""text"": ""{embed.FooterText}"",
                ""icon_url"": ""{embed.FooterIconUrl}""
                }},
                ""fields"": [{embed.BuildFields()}]
            }}";
        }
    }
}
