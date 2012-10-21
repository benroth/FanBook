using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace FanBook
{
    class FFNetBook : FanBook
    {
        public override void GrabStoryVariables(HtmlAgilityPack.HtmlDocument doc)
        {//Takes first chapter and fills story variables with its info.
            try { storyChapters = (ushort)(doc.DocumentNode.SelectNodes("//select[@id='chap_select']/option").Count() / 2); }
            catch { storyChapters = 1; }
            //Checks for chapter selection dropdown. If not present, story is 1 chapter long.

            storyTitle = doc.DocumentNode.SelectSingleNode("//tr[@class='alt2']//b").InnerText;
            authorName = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/u/')]").InnerText;
            authorLink = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/u/')]").OuterHtml;
            authorLink = authorLink.Insert(9, "http://www.fanfiction.net");
            storySummary = doc.DocumentNode.SelectSingleNode("//div[@style='margin-top:2px']").InnerText;

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
                    chapterTitles.Add(node.NextSibling.InnerText.Remove(0, node.NextSibling.InnerText.IndexOf(' ')));

            //Generates array of chapter titles.
            storyBody = "(//*[contains(@id, 'storytext')])";
        }

        public override void DownloadStory(int i)
        {//Loops through number of chapters and downloads each.
            string[] addressParts = storyURL.Split('/');
            if (storyChapters > 1)
            {//Builds TOC for stories with multiple storyChapters.
                if (i == 0)
                    BuildTOC();

                if (i < storyChapters)
                {//Creates an anchor of each chapter title, followed by the chapter itself. 
                    storyContent.AppendLine("<h2><a name=\"" + chapterTitles[i] + "\">" + chapterTitles[i] + "</h2>" + getChapter(storyURL + (i + 1).ToString()));
                    storyContent.AppendLine("<br />");
                }
            }
            else
                storyContent.AppendLine(getChapter(storyURL + "/" + 1));
        }

        public override string getChapter(string address)
        {//Grabs the actual chapter content from the URL provided.

            string htmlContent = grabHtml(address);
            if (htmlContent == "")
                return "Unable to fetch chapter.";
            //Passback to original function in case of grabHTML failure.

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);
            return (doc.DocumentNode.SelectSingleNode(storyBody).InnerHtml);
        }

        public override string TestURLDownload()
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
                storyURL = "http://www.fanfiction.net/s/" + addressParts[4] + "/";
                //Sets the storyURL to the base URL for whichever site plus the storyID.
                return "Download test passed.";
            }
        }
    }
}
