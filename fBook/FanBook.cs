using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Globalization;

namespace FanBook
{
    public class FanBook
    {
        protected ushort storyChapters;
        protected string storyURL, storyTitle, authorName, authorLink, storySummary, storyStatus, storyWords, storyPublished, storyUpdated, storyBody, dateFormat;
        protected List<string> chapterTitles = new List<string>();
        protected StringBuilder storyContent = new StringBuilder();
        protected StringBuilder storyInfo = new StringBuilder();
        protected DateTime dt;

        public virtual string TestURLDownload()
        { return ""; }
        public virtual void GrabStoryVariables(HtmlAgilityPack.HtmlDocument doc)
        { }
        public virtual string getChapter(string address)
        { return ""; }
        public virtual void DownloadStory(int i)
        { }

        public void InitStoryVariables()
        {//Initializes variables for repeated story downloads.
            storyChapters = 0;
            storyURL = null;
            storyTitle = null;
            storySummary = null;
            authorName = null;
            authorLink = null;
            storyStatus = null;
            storyWords = null;
            storyPublished = null;
            storyUpdated = null;
            storyBody = null;
            dateFormat = null;
            chapterTitles.Clear();
            storyContent.Clear();
            storyInfo.Clear();
        }

        public string TestURLFormat(String address)
        {//Verifies proper URL formatting and that URL is for a supported site.
            storyURL = address;
            if (String.IsNullOrEmpty(storyURL)) return "Invalid URL!";
            if (storyURL.Equals("about:blank")) return "Invalid URL!";
            if (!storyURL.StartsWith("http://") &&
                !storyURL.StartsWith("https://"))
            {
                storyURL = "http://" + storyURL;
            }
            if (!storyURL.Contains("fanfiction.net") && !storyURL.Contains("fictionpress.com") && !storyURL.Contains("tthfanfic.org") && !storyURL.Contains("fanficauthors.net"))
            {
                return "Invalid URL! Please enter URLs for fanfiction.net or fictionpress.com stories only.";
            }
            return "Valid URL.";
        }        

        protected string grabHtml(string address)
        {//Grabs HTML from provided address, or returns blank string in case of failure.
            try
            {//Tries downloading story using default IE proxy settings.
                WebClient client = new WebClient();
                Stream data = client.OpenRead(new Uri(address));
                StreamReader reader = new StreamReader(data, Encoding.GetEncoding("UTF-8"));
                string htmlContent = reader.ReadToEnd();
                data.Close();
                reader.Close();
                return htmlContent;
            }
            catch
            {
                try
                {//If proxy failed, try without proxy.
                    WebClient client = new WebClient();
                    client.Proxy = null;
                    Stream data = client.OpenRead(new Uri(address));
                    StreamReader reader = new StreamReader(data, Encoding.GetEncoding("UTF-8"));
                    string htmlContent = reader.ReadToEnd();
                    data.Close();
                    reader.Close();
                    return htmlContent;
                }
                catch //If both failed, report connection error.
                { return "Unable to download story! Please check connection to site."; }
            }
        }

        public string GenerateStoryInfo()
        { //Downloads first chapter to get story info.
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(grabHtml(storyURL));
            GrabStoryVariables(doc);
            CreateStoryHeader();
            return storyInfo.ToString();
        }           

        private void CreateStoryHeader()
        {//Builds story header using previously fetched variables, sets up formatting. Ignores null variables.
            storyInfo.AppendLine("<html><body><meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />");
            if (storyInfo != null) { storyInfo.AppendLine("Title: " + storyTitle); }
            if (authorLink != null) { storyInfo.AppendLine("<br />Author: " + authorLink); }
            if (storyInfo != null) { storyInfo.AppendLine("<br />Chapters: " + storyChapters.ToString()); }
            if (storyWords != null) { storyInfo.AppendLine("<br />Story " + storyWords); }
            if (storyPublished != null) { storyInfo.Append("<br />" + storyPublished); }
            if (storyUpdated != null) { storyInfo.AppendLine("<br />Last " + storyUpdated); }
            if (storyStatus != null) { storyInfo.AppendLine("<br />Story Status: " + storyStatus); }
            if (storySummary != null) { storyInfo.AppendLine("<br /><br />Summary: " + storySummary); }
            storyInfo.AppendLine("<br /><br /><br />");
        }

        public ushort GetChapterCount()
        { //Lets number of stories be retrievable.
            return storyChapters;
        }

        public void BuildStory()
        {//Adds story header, story chapters, and end tags together.
            storyContent.Insert(0, storyInfo);
            if (storyStatus == "Complete")
                storyContent.AppendLine("<h3>The End.</h3></body></html>");
            else
                storyContent.AppendLine("<h3>End of posted story.</h3></body></html>");
        }

        protected void BuildTOC()
        {//Creates Table of Contents.
            storyContent.AppendLine("<h2>Table of Contents</h2>");
            
            for (int i = 0; i < storyChapters; i++)
                storyContent.AppendLine("<a href=\"#" + chapterTitles[i] + "\">" + chapterTitles[i] + "</a><br />");

            storyContent.AppendLine("<br /><br />");
        }

        public string GetFileTitle()
        {//Strips invalid characters from title and authorname and returns that for the story title.
            foreach (char character in Path.GetInvalidFileNameChars())
                storyTitle = storyTitle.Replace(character.ToString(), string.Empty);
            foreach (char character in Path.GetInvalidFileNameChars())
                authorName = authorName.Replace(character.ToString(), string.Empty);
            return storyTitle + " - " + authorName;
        }

        public string ReturnStory()
        {//Lets built story be passed back to form.
            return storyContent.ToString();
        }
    }
}