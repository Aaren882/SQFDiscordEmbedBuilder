using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Maca134.Arma.Serializer;
using System.Text.RegularExpressions;
using System.Net;
using static DiscordEmbedBuilder.Types;

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
                    string url = args[0].Trim('"').Replace("\"\"", "\"");
                    string content = args[1].Trim('"').Replace("\"\"", "\"");
                    string username = args[2].Trim('"').Replace("\"\"", "\"");
                    string avatar = args[3].Trim('"').Replace("\"\"", "\"");
                    string tts = args[4];

                    //- File Stream
                    string filePath = $"{args[5]}".Trim('"').Replace("\"\"", "\"");

                    // Discord 2000 character limit
                    if (content.Length > 1999) content = content.Substring(0, 1999);

                    // Bare bones
                    package.Add(new StringContent(content), "content");
                    package.Add(new StringContent(tts), "tts");

                    if (username.Length > 0) package.Add(new StringContent(username), "username");
                    if (avatar.Length > 0) package.Add(new StringContent(avatar), "avatar_url");

                    //- Send File .png
                    if (filePath.Length > 0)
                    {
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                        {
                            byte[] fileBytes = new byte[fileStream.Length];
                            await fileStream.ReadAsync(fileBytes, 0, fileBytes.Length);
                            package.Add(new ByteArrayContent(fileBytes), "file", "file.png");
                        }
                    }

                    // Build embeds array
                    List<Types.EmbedData> embeds = BuildEmbedList(args[6]);
                    string embedsJson = BuildEmbedsJson(embeds);


                    if (embeds.Count > 0) package.Add(new StringContent(embedsJson, Encoding.UTF8), "payload_json");

                    // Build embeds array
                    //Types.EmbedData embeds = DeserializeObject<Types.EmbedData>(args[6]);
                    //List<Types.EmbedData> embedList = BuildEmbedList(embeds);
                    // JArray embedProperty = new JArray();
                    // for (int i = 0; i < 10; i++)
                    // {
                    //     Types.EmbedArray embed = embedList.ElementAt(i);
                    //     if (embed == null) break;
                    //     JObject embedObject = BuildEmbedObject(embed);
                    //     if (embedObject.Count > 0) embedProperty.Add(embedObject);
                    // }
                    // if (embedProperty.Count() > 0) package.Add(new JProperty("embeds", embedProperty));

                    // if (embedProperty.Count() > 0) package.Add(new JProperty("embeds", embedProperty));


                    // Execute webhook
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                         SecurityProtocolType.Tls |
                         SecurityProtocolType.Tls11 |
                         SecurityProtocolType.Ssl3;
                    using (HttpClient APIClient = new HttpClient())
                    {
                        HttpResponseMessage response = await APIClient.PostAsync(url, package);
                    }
                }
            }
            catch (Exception e)
            {
                Tools.Logger(e);
            }
        }

        /*private static T DeserializeObject<T>(string value)
        {
            value = value.Replace("\"\"", "\\\"\\\"");
            value = new Regex("([[,]?)nil([],]+)").Replace(value, "$1null$2");
            Tools.Logger(null, value);
            return JsonConvert.DeserializeObject<T>(value, new JsonConverter[]
            {
                new ArmaJsonConverter()
            });
        }*/

        private static List<Types.EmbedData> BuildEmbedList(string input)
        {
            string Rawdata = input.Trim('[', ']');
            List<string> embedsData = new List<string>(Rawdata.Split(new string[] { "],[" }, StringSplitOptions.None));

            var embeds = new List<Types.EmbedData>();

            foreach (var data in embedsData)
            {
                embeds.Add(new EmbedData(new List<string>(data.Split(new string[] { "," }, StringSplitOptions.None))));
            }
            
            Tools.Logger(null, embeds);

            return embeds;
            //string embedsJson = BuildEmbedsJson(embeds);
            /*return new List<Types.EmbedData>() {
                input.Title,
                input.Description,
                input.Color,
                input.AuthorName,
                input.AuthorUrl,
                input.AuthorIconUrl,
                input.ImageUrl,
                input.ThumbnailUrl,
                input.FooterText,
                input.FooterIconUrl
            };*/
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
                }}
            }}";
        }

        // I don't know the best way to do this, I'm limited by my lack of C# knowledge and how I understand the deserializer to work

        // The arma array deserializer doesnt like empty strings and I dont know how to fix it, so heres a shit work around
        /*private static string RemoveReservedString(string input) => input == $"{(char)1}" ? "" : input;

        private static JObject BuildEmbedObject(Types.EmbedArray embed)
        {
            JObject embedObject = new JObject();
            Types.EmbedAuthor embedAuthor = embed.author;
            Types.EmbedFields embedFields = embed.fields;
            Types.EmbedFooter embedFooter = embed.footer;

            // Adding the basics
            embed.title = RemoveReservedString(embed.title).Replace("\"\"", "\""); ;
            embed.description = RemoveReservedString(embed.description).Replace("\"\"", "\""); ;
            embed.url = RemoveReservedString(embed.url).Replace("\"\"", "\""); ;
            embed.color = RemoveReservedString(embed.color).Replace("\"\"", "\""); ;
            embed.thumbnail = RemoveReservedString(embed.thumbnail).Replace("\"\"", "\""); ;
            embed.image = RemoveReservedString(embed.image).Replace("\"\"", "\""); ;
            if (embed.title.Length > 0) embedObject.Add(new JProperty("title", embed.title));
            if (embed.description.Length > 0) embedObject.Add(new JProperty("description", embed.description));
            if (embed.url.StartsWith("https://")) embedObject.Add(new JProperty("url", embed.url));
            if (embed.color.Length == 6) embedObject.Add(new JProperty("color", Convert.ToInt32(embed.color, 16)));
            if (embed.useTimestamp) embedObject.Add(new JProperty("timestamp", DateTime.UtcNow.ToString("s")));
            if (embed.thumbnail.StartsWith("https://")) embedObject.Add(new JProperty("thumbnail", new JObject(new JProperty("url", embed.thumbnail))));
            if (embed.image.StartsWith("https://")) embedObject.Add(new JProperty("image", new JObject(new JProperty("url", embed.image))));

            // Handle additional objects
            embedAuthor.name = RemoveReservedString(embedAuthor.name).Replace("\"\"", "\""); ;
            embedAuthor.url = RemoveReservedString(embedAuthor.url).Replace("\"\"", "\""); ;
            embedAuthor.icon_url = RemoveReservedString(embedAuthor.icon_url).Replace("\"\"", "\""); ;
            if (embedAuthor.name.Length > 0)
            {
                JObject embedObjectAuthor = new JObject(new JProperty("name", embedAuthor.name));
                if (embedAuthor.url.StartsWith("https://")) embedObjectAuthor.Add(new JProperty("url", embedAuthor.url));
                if (embedAuthor.icon_url.StartsWith("https://")) embedObjectAuthor.Add(new JProperty("icon_url", embedAuthor.icon_url));

                embedObject.Add(new JProperty("author", embedObjectAuthor));
            }

            embedFooter.text = RemoveReservedString(embedFooter.text).Replace("\"\"", "\""); ;
            embedFooter.icon_url = RemoveReservedString(embedFooter.icon_url).Replace("\"\"", "\""); ;
            if (embedFooter.text.Length > 0)
            {
                JObject embedObjectFooter = new JObject(new JProperty("text", embedFooter.text));
                if (embedFooter.icon_url.StartsWith("https://")) embedObjectFooter.Add(new JProperty("icon_url", embedFooter.icon_url));

                embedObject.Add(new JProperty("footer", embedObjectFooter));
            }

            // Build fields array
            List<Types.EmbedField> fieldList = BuildFieldList(embedFields);
            JArray fieldProperty = new JArray();
            for (int i = 0; i < 25; i++)
            {
                Types.EmbedField field = fieldList.ElementAt(i);
                if (field == null) break;
                JObject fieldObject = BuildFieldObject(field);
                if (fieldObject != null) fieldProperty.Add(fieldObject);
            }
            if (fieldProperty.Count() > 0) embedObject.Add(new JProperty("fields", fieldProperty));

            return embedObject;
        }

        private static JObject BuildFieldObject(Types.EmbedField field)
        {
            field.name = RemoveReservedString(field.name).Replace("\"\"", "\""); ;
            field.value = RemoveReservedString(field.value).Replace("\"\"", "\""); ;
            if (field.name.Length == 0 || field.value.Length == 0) return null;

            return new JObject(
                new JProperty("name", field.name),
                new JProperty("value", field.value),
                new JProperty("inline", field.inline)
            );
        }

        private static List<Types.EmbedField> BuildFieldList(Types.EmbedFields input)
        {
            return new List<Types.EmbedField>() {
                input.field1,
                input.field2,
                input.field3,
                input.field4,
                input.field5,
                input.field6,
                input.field7,
                input.field8,
                input.field9,
                input.field10,
                input.field11,
                input.field12,
                input.field13,
                input.field14,
                input.field15,
                input.field16,
                input.field17,
                input.field18,
                input.field19,
                input.field20,
                input.field21,
                input.field22,
                input.field23,
                input.field24,
                input.field25
            };
        }*/
    }
}
