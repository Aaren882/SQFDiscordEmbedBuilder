namespace DiscordEmbedBuilder
{
    public class Types
    {
        public class EmbedData
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Color { get; set; }
            public string AuthorName { get; set; }
            public string AuthorUrl { get; set; }
            public string AuthorIconUrl { get; set; }
            public string ImageUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public string FooterText { get; set; }
            public string FooterIconUrl { get; set; }

            public EmbedData(List<object> data)
            {
                Title = data.Count > 0 ? (string)data[0] : "";
                Description = data.Count > 1 ? (string)data[1] : "";
                Color = data.Count > 2 ? (string)data[2] : "";

                AuthorName = data.Count > 3 ? (string)data[3] : "";
                AuthorUrl = data.Count > 4 ? (string)data[4] : "";
                AuthorIconUrl = data.Count > 5 ? (string)data[5] : "";
                ImageUrl = data.Count > 6 ? (string)data[6] : "";
                ThumbnailUrl = data.Count > 7 ? (string)data[7] : "";
                FooterText = data.Count > 8 ? (string)data[8] : "";
                FooterIconUrl = data.Count > 9 ? (string)data[9] : "";
            }
        }
        /*public class EmbedArray
        {
            public string title { get; set; } = "";
            public string description { get; set; } = "";
            public string url { get; set; } = "";
            public string color { get; set; } = "";
            public bool useTimestamp { get; set; } = false;
            public string thumbnail { get; set; } = "";
            public string image { get; set; } = "";
            public EmbedAuthor author { get; set; }
            public EmbedFooter footer { get; set; }
            public EmbedFields fields { get; set; }
        }
        public class EmbedAuthor
        {
            public string name { get; set; } = "";
            public string url { get; set; } = "";
            public string icon_url { get; set; } = "";
        }
        public class EmbedFooter
        {
            public string text { get; set; } = "";
            public string icon_url { get; set; } = "";
        }
        public class EmbedField
        {
            public string name { get; set; } = "";
            public string value { get; set; } = "";
            public bool inline { get; set; } = false;
        }

        // I don't know the best way to do this, I'm limited by my lack of C# knowledge and how I understand the deserializer to work
        public class EmbedsArray
        {
            public EmbedArray embed1 { get; set; }
            public EmbedArray embed2 { get; set; }
            public EmbedArray embed3 { get; set; }
            public EmbedArray embed4 { get; set; }
            public EmbedArray embed5 { get; set; }
            public EmbedArray embed6 { get; set; }
            public EmbedArray embed7 { get; set; }
            public EmbedArray embed8 { get; set; }
            public EmbedArray embed9 { get; set; }
            public EmbedArray embed10 { get; set; }
        }
        public class EmbedFields
        {
            public EmbedField field1 { get; set; }
            public EmbedField field2 { get; set; }
            public EmbedField field3 { get; set; }
            public EmbedField field4 { get; set; }
            public EmbedField field5 { get; set; }
            public EmbedField field6 { get; set; }
            public EmbedField field7 { get; set; }
            public EmbedField field8 { get; set; }
            public EmbedField field9 { get; set; }
            public EmbedField field10 { get; set; }
            public EmbedField field11 { get; set; }
            public EmbedField field12 { get; set; }
            public EmbedField field13 { get; set; }
            public EmbedField field14 { get; set; }
            public EmbedField field15 { get; set; }
            public EmbedField field16 { get; set; }
            public EmbedField field17 { get; set; }
            public EmbedField field18 { get; set; }
            public EmbedField field19 { get; set; }
            public EmbedField field20 { get; set; }
            public EmbedField field21 { get; set; }
            public EmbedField field22 { get; set; }
            public EmbedField field23 { get; set; }
            public EmbedField field24 { get; set; }
            public EmbedField field25 { get; set; }
        }*/
    }
}
