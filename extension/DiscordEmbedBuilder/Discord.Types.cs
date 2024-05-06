using System.Collections.Generic;
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

            public EmbedData(List<string> data)
            {
                Title = data.Count > 0 ? (string)data[0] : "";
                Description = data.Count > 1 ? (string)data[1] : "";
                Color = data.Count > 2 ? (string)data[2] : "14177041";

                AuthorName = data.Count > 3 ? (string)data[3] : "";
                AuthorUrl = data.Count > 4 ? (string)data[4] : "";
                AuthorIconUrl = data.Count > 5 ? (string)data[5] : "";
                
                ImageUrl = data.Count > 6 ? (string)data[6] : "";
                ThumbnailUrl = data.Count > 7 ? (string)data[7] : "";
                FooterText = data.Count > 8 ? (string)data[8] : "";
                FooterIconUrl = data.Count > 9 ? (string)data[9] : "";
            }
        }
    }
}
