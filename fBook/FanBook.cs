﻿using System;
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

namespace fBook
{
    public class FanBook
    {
        private ushort storyChapters;
        private string storyURL, storyTitle, authorName, storyStatus, storyWords, storyPublished, storyUpdated;
        private List<string> chapterTitles = new List<string>();
        private StringBuilder storyContent = new StringBuilder();
        private StringBuilder storyInfo = new StringBuilder();

        public void InitStoryVariables()
        {//Initializes variables for repeated story downloads.
            storyChapters = 0;
            storyURL = null;
            storyTitle = null;
            authorName = null;
            storyStatus = null;
            storyWords = null;
            storyPublished = null;
            storyUpdated = null;
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
            if (!storyURL.Contains("fanfiction.net") && !storyURL.Contains("fictionpress.com"))
            {
                return "Invalid URL! Please enter URLs for fanfiction.net or fictionpress.com stories only.";
            }
            return "Valid URL.";
        }

        public string TestURLDownload()
        {
            string htmlContent = grabHtml(storyURL);

            if (htmlContent == "Unable to download story! Please check connection to site.")
            {//Passback to original function in case of grabHTML failure.
                return htmlContent;
            }
            else if (htmlContent.Contains("Unable to locate story. Code 1."))
            {//Passback to original function in case of valid-formatted URL that doesn't point to a story.
                return "Unable to locate story.";
            }
            else
            {//Parses address to grab just the base URL and the story ID.
                string[] addressParts = storyURL.Split('/');
                if (storyURL.Contains("fanfiction.net"))
                    storyURL = "http://www.fanfiction.net/s/" + addressParts[4];
                else
                    storyURL = "http://www.fictionpress.com/s/" + addressParts[4];
                //Sets the storyURL to the base URL for whichever site plus the storyID.
                return "Download test passed.";
            }
        }

        private string grabHtml(string address)
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
            CreateStoryHeader(doc);
            return storyInfo.ToString();
        }

        private void GrabStoryVariables(HtmlAgilityPack.HtmlDocument doc)
        {//Takes first chapter and fills story variables with its info.
            try { storyChapters = (ushort)(doc.DocumentNode.SelectNodes("//select[@id='chap_select']/option").Count() / 2); }
            catch { storyChapters = 1; }
            //Checks for chapter selection dropdown. If not present, story is 1 chapter long.

            storyTitle = doc.DocumentNode.SelectSingleNode("//tr[@class='alt2']//b").InnerText;
            authorName = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/u/')]").InnerText;
            string greyHeader = doc.DocumentNode.SelectSingleNode("//div[@style='color:gray;']").InnerText;
            string[] splitHeader = greyHeader.Split(' ');
            //Grabs individual variables from story, then splits header up for further parsing.

            for (int i = 0; i < splitHeader.Length; i++)
            {//Looks for keywords in header, and if found, assigns them and following section to appropriate story variable.
                if (splitHeader[i].Contains("Words:"))
                    storyWords = splitHeader[i] + " " + splitHeader[i + 1];
                else if (splitHeader[i].Contains("Published:"))
                    storyPublished = splitHeader[i] + " " + splitHeader[i + 1];
                else if (splitHeader[i].Contains("Updated:"))
                    storyUpdated = splitHeader[i] + " " + splitHeader[i + 1];
            }

            if (greyHeader.Contains("Complete"))
                storyStatus = "Complete";
            else
                storyStatus = "In Progress";
            //Checks header for whether story is complete.

            if (storyChapters > 1)
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//select[@id='chap_select']/option"))
                    chapterTitles.Add(node.NextSibling.InnerText);
            //Generates array of chapter titles.
        }

        private void CreateStoryHeader(HtmlAgilityPack.HtmlDocument doc)
        {//Builds story header using previously fetched variables, sets up formatting.
            storyInfo.AppendLine("<html><body><meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />");
            storyInfo.AppendLine("Title: " + storyTitle);
            storyInfo.AppendLine("<br />Author: " + doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/u/')]").OuterHtml);
            storyInfo.AppendLine("<br />Chapters: " + storyChapters.ToString());
            storyInfo.AppendLine("<br />Story " + storyWords);
            storyInfo.Append("<br />" + storyPublished);
            if (storyUpdated != "")
                storyInfo.AppendLine("<br />Last " + storyUpdated);
            storyInfo.AppendLine("<br />Story Status: " + storyStatus);
            storyInfo.AppendLine("<br /><br />Summary: " + doc.DocumentNode.SelectSingleNode("//div[@style='margin-top:2px']").InnerText);
            storyInfo.AppendLine("<br /><br /><br />");
        }

        private string getChapter(string address)
        {//Grabs the actual chapter content from the URL provided.
            string htmlContent = grabHtml(address);
            if (htmlContent == "")
                return "Unable to fetch chapter.";
            //Passback to original function in case of grabHTML failure.

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);
            if (address.Contains("fanfiction.net"))
                return doc.DocumentNode.SelectSingleNode("//*[contains(@id, 'storytext')]").InnerHtml;
            else
                return doc.DocumentNode.SelectSingleNode("//*[contains(@id, 'storytextp')]").InnerHtml;
            //Differentiates between location of chapter content for FF.net FictionPress.com.
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

        public void DownloadStory(int i)
        {//Loops through number of chapters and downloads each.
            if (storyChapters > 1)
            {//Builds TOC for stories with multiple storyChapters.
                if (i == 0)
                    BuildTOC();

                if (i < storyChapters)
                {//Creates an anchor of each chapter title, followed by the chapter itself. 
                    storyContent.AppendLine("<h2><a name=\"" + chapterTitles[i] + "\">" + chapterTitles[i] + "</h2>" + getChapter(storyURL + "/" + (i + 1).ToString()));
                    storyContent.AppendLine("<br />");
                }                
            }
            else
                storyContent.AppendLine(getChapter(storyURL + "/" + 1));
        }

        private void BuildTOC()
        {//Creates Table of Contents.
            storyContent.AppendLine("<h2>Table of Contents</h2>");

            for (int i = 0; i < storyChapters; i++)
            {//Strips out chapter number and just leaves title intact, then creates links for each chapter title.
                chapterTitles[i] = chapterTitles[i].Remove(0, i.ToString().Length + 2);
                storyContent.AppendLine("<a href=\"#" + chapterTitles[i] + "\">" + chapterTitles[i] + "</a><br />");
            }
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