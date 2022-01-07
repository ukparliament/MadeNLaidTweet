namespace MadeNLaidTweet
{
    using System;
    using System.Linq;

    class StatutoryInstrument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string WorkPackageId { get; set; }
        public string ProcedureId { get; set; }
        public string ProcedureName { get; set; }
        public string LayingBodyName { get; set; }
        public DateTimeOffset LaidDate { get; set; }
        public DateTimeOffset MadeDate { get; set; }
        public string Link { get; set; }
        public bool IsTweeted { get; set; }

        public string ShortTitle
        {
            get
            {
                if (Name.Length > 200)
                {
                    return Name.Substring(0, 198) + "..";
                }
                else
                {
                    return Name;
                }
            }
        }
        public string Url
        {
            get
            {
                return "https://statutoryinstruments.parliament.uk/instrument/" + Id.Split('/').Last() + "/timeline/" + WorkPackageId.Split('/').Last() + "/";
            }
        }

        public string TweetText
        {
            get
            {
                string tweet_text = "";
                tweet_text += ShortTitle;
                tweet_text += ". Made on ";
                tweet_text += MadeDate.ToString("dd-MM-yyyy");
                tweet_text += ", laid on ";
                tweet_text += LaidDate.ToString("dd-MM-yyyy");
                tweet_text += ". Subject to the ";
                tweet_text += ProcedureName;
                tweet_text += " procedure. ";
                tweet_text += Url;
                return tweet_text;
            }
        }
    }
}
