using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DiscordEmbedBuilder
{
    internal class Discord
    {
        internal static async Task HandleRequest(string[] args)
        {
            try
            {
                using (MultipartFormDataContent package = new MultipartFormDataContent())
                {
                    // Remove arma quotations
                    string url = args[0].Trim('"',' ').Replace("\"\"", "\"");
                    string content = args[1].Trim('"',' ').Replace("\"\"", "\"");
                    string username = args[2].Trim('"',' ').Replace("\"\"", "\"");
                    string avatar = args[3].Trim('"',' ').Replace("\"\"", "\"");
                    string tts = args[4];

                    //- File Stream
                    string filePath = $"{args[5]}".Trim('"',' ').Replace("\"\"", "\"");

                    // Discord 2000 character limit
                    if (content.Length > 1999) content = content.Substring(0, 1999);

                    // Build embeds array
                    //- Turn Data into List<List<string>> e.g [ ["TITLE","DESC"] , ["11","22] ]
                    List<List<string>> embedsData = ParseStringToList(args[6]);
                    List<List<string>> FieldsData = ParseStringToList(args[7]);

                    for (int i = 0; i < embedsData.Count; i++)
                    {
                        for (int j = 0; j < fieldsData.Count; j++)
                        {
                            embedsData[i].AddRange(fieldsData[j]);
                        }
                    }

                    //- pass Data into "class Types.EmbedData"
                    List<Types.EmbedData> embeds = embedsData.Select(data => new Types.EmbedData(data)).ToList();

                    string embedsJson = BuildEmbedsJson(embeds);

                    // Execute webhook
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                         SecurityProtocolType.Tls |
                         SecurityProtocolType.Tls11 |
                         SecurityProtocolType.Ssl3;
                    
                    using (HttpClient APIClient = new HttpClient())
                    {
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

                        HttpResponseMessage response = await APIClient.PostAsync(url, package);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.Logger(e);
            }
        }
        
        //- Translating Data
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

        static string BuildEmbedsJson(List<Types.EmbedData> embeds)
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

        static string BuildEmbedJson(Types.EmbedData embed)
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
