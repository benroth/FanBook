using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace FanBook
{
    class FanFicAuthorsBook : FanBook
    {
        public override string GrabStoryVariables(HtmlAgilityPack.HtmlDocument doc)
        {
            string[] addressParts = storyURL.Split('/');
        
            try
            {
                storyChapters = (ushort)(doc.DocumentNode.SelectNodes("//div[@id='chapterList']//li").Count() - 1);
                storyTitle = addressParts[3].Replace("_", " ");

                try { authorName = doc.DocumentNode.SelectSingleNode("//h2[@class='textCenter']/a").InnerText; }
                catch { authorName = doc.DocumentNode.SelectSingleNode("//h1[@class='textCenter']/a").InnerText; }

                authorLink = "<a href=\"http://" + addressParts[2] + "\">" + authorName + "</a>";

                HtmlNodeCollection summaryNodes = doc.DocumentNode.SelectNodes("//div[@id='chapterList']//li[1]/p");
                foreach (HtmlNode node in summaryNodes)
                {
                    if (!(node.InnerText.Contains("Word count:") || node.InnerText.Contains("Also available as:")))
                        storySummary += node.InnerText;
                }

                string[] splitHeader = doc.DocumentNode.SelectSingleNode("//div[@id='chapterList']//li[1]/p[1]").InnerText.Split(' ');
                //Grabs individual variables from story, then splits header up for further parsing.

                for (int i = 0; i < splitHeader.Length; i++)
                {//Looks for keywords in header, and if found, assigns them and following section to appropriate story variable.
                    if (splitHeader[i].Contains("Word") && splitHeader[i + 1].Contains("count"))
                        storyWords = "Words: " + splitHeader[i + 2];
                    else if (splitHeader[i].Contains("Status:"))
                    {
                        if (splitHeader[i + 1].Contains("Complete"))
                            storyStatus = "Complete";
                        else
                            storyStatus = "In Progress";
                    }
                }

                splitHeader = doc.DocumentNode.SelectSingleNode("//div[@id='chapterList']//li[2]").InnerText.Split(' ');
                for (int i = 0; i < splitHeader.Length; i++)
                    if (splitHeader[i].Contains("Uploaded"))
                        storyPublished = "Published: " + splitHeader[i + 2] + " " + splitHeader[i + 3] + " " + splitHeader[i + 4];

                splitHeader = doc.DocumentNode.SelectSingleNode("//div[@id='chapterList']//li[" + (storyChapters + 1) + "]").InnerText.Split(' ');
                for (int i = 0; i < splitHeader.Length; i++)
                    if (splitHeader[i].Contains("Uploaded"))
                        storyUpdated = "Updated: " + splitHeader[i + 2] + " " + splitHeader[i + 3] + " " + splitHeader[i + 4];

                for (int i = 2; i <= storyChapters + 1; i++)
                {
                    addressParts = doc.DocumentNode.SelectSingleNode("//div[@id='chapterList']//li[" + i + "]/a[1]").Attributes["href"].Value.Split('/');
                    addressParts[2] = addressParts[2].Replace("__", " - ");
                    addressParts[2] = addressParts[2].Replace("_", " ");
                    chapterTitles.Add(addressParts[2]);
                }
                storyBody = "(//li[@class='chapterDisplay'])";
                return "Story grab success.";
            }
            catch
            { return "Story grab failed! Please verify URL provided is for a story."; }
        } 

        public override void DownloadStory(int i)
        {//Loops through number of chapters and downloads each.
            string[] addressParts = storyURL.Split('/');
            string title = chapterTitles[i];
            title = title.Replace(" - ", "__");
            title = title.Replace(" ", "_");
            addressParts[addressParts.Length - 2] = title;
            storyURL = string.Join("/", addressParts);
            //Rebuilds chapter title to create original URL for downloading.

            if (storyChapters > 1)
            {//Builds TOC for stories with multiple storyChapters.
                if (i == 0)
                    base.BuildTOC();

                if (i < storyChapters)
                {//Creates an anchor of each chapter title, followed by the chapter itself. 
                    storyContent.AppendLine("<h2><a name=\"" + chapterTitles[i] + "\">" + chapterTitles[i] + "</h2>" + getChapter(storyURL));
                    storyContent.AppendLine("<br />");
                }
            }
            else
                storyContent.AppendLine(getChapter(storyURL + "/" + 1));
        }

        public override string getChapter(string address)
        {//Grabs the actual chapter content from the URL provided.
            string htmlContent;

            try { htmlContent = grabHtml(address, "passhash=1f504b24e7c096a5b04ac0b7da44afc5"); }
            catch { return "Unable to download Mature stories."; }
 
            if (htmlContent == "")
                return "Unable to fetch chapter.";
            //Passback to original function in case of grabHTML failure.

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);
            HtmlNodeCollection h3Node = doc.DocumentNode.SelectNodes("//h3");
            if (h3Node != null)
                foreach (HtmlNode node in h3Node)
                    { node.ParentNode.RemoveChild(node); }

            return (doc.DocumentNode.SelectSingleNode(storyBody).InnerHtml);
        }        

        public override string TestURLDownload()
        {
            string[] addressParts = storyURL.Split('/');

            if (addressParts.Length < 5)
                return "Invalid URL! Please enter a story URL for a supported archive.";
            storyURL = string.Join("/", addressParts, 0, 4) + "/index/";
            string htmlContent = grabHtml(storyURL);

            if (htmlContent == "Unable to download story! Please check connection to site.")
            {//Passback to original function in case of grabHTML failure.
                return htmlContent;
            }
            else if (htmlContent.Contains("The requested file has not been found."))
            {//Passback to original function in case of valid-formatted URL that doesn't point to a story.
                return "Unable to locate story.";
            }
            else
            {//Parses address to grab just the base URL and the story ID.
                return "Download test passed.";
            }
        }
    }
}
