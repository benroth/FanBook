using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Globalization;

namespace FanBook
{
    class TTHFanficBook : FanBook
    {
        public override string GrabStoryVariables(HtmlAgilityPack.HtmlDocument doc)
        {
            dateFormat = "d MMM yy";
            try
            {
                try { storyChapters = (ushort)(doc.DocumentNode.SelectNodes("//select[@id='chapnav']/option").Count()); }
                catch { storyChapters = 1; }
                //Checks for chapter dropdown, sets to 1 if not present.

                storyTitle = doc.DocumentNode.SelectSingleNode("//h2").InnerText;
                authorName = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'AuthorStories')]").InnerText;
                //Grabs title and name.

                authorLink = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'AuthorStories')]").Attributes["href"].Value;
                authorLink = authorLink.Insert(0, "<a href='http://www.tthfanfic.org");
                authorLink = authorLink + "'>" + authorName + "</a>";
                //Creates author link.

                try { storySummary = doc.DocumentNode.SelectSingleNode("//div[@class='storysummary formbody defaultcolors']/p[2]").InnerText; }
                catch { storySummary = doc.DocumentNode.SelectSingleNode("//div[@class='storysummary formbody defaultcolors']/p[1]").InnerText; }
                storySummary = storySummary.Remove(0, 9);
                //If story is part of series, 2nd paragraph is summary, if not, 1st is. Remove built-in "Summary: ".

                storyWords = doc.DocumentNode.SelectSingleNode("//table[@class='verticaltable']/tr[2]/td[5]").InnerText;

                dt = DateTime.ParseExact(doc.DocumentNode.SelectSingleNode("//table[@class='verticaltable']/tr[2]/td[9]").InnerHtml.Replace("&nbsp;", " ")
                    , dateFormat, CultureInfo.InvariantCulture);
                storyPublished = "Published: " + dt.ToString("d");
                //Grab table section with publish date, strip formatting, read it in as US-format date.

                dt = DateTime.ParseExact(doc.DocumentNode.SelectSingleNode("//table[@class='verticaltable']/tr[2]/td[10]").InnerHtml.Replace("&nbsp;", " ")
                    , dateFormat, CultureInfo.InvariantCulture);
                storyUpdated = "Updated: " + dt.ToString("d");
                //Same with update date.

                if (doc.DocumentNode.SelectSingleNode("//table[@class='verticaltable']/tr[2]/td[11]").InnerText.Contains("Yes"))
                    storyStatus = "Complete";
                else
                    storyStatus = "In Progress";
                //Check for completion status.

                if (storyChapters > 1)
                    foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//select[@id='chapnav']/option"))
                    {
                        string title = node.NextSibling.InnerText;
                        chapterTitles.Add(title.Substring(title.IndexOf(" ")));
                    }
                //Grab chapter titles.
                //*[@id="storyinnerbody"]
                storyBody = "(//*[@id='storyinnerbody'])";
                //Set path to story body for later extraction.

                return "Story grab success.";
            }
            catch
            { return "Story grab failed! Please verify URL provided is for a story."; }
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
            HtmlNodeCollection h3Node = doc.DocumentNode.SelectNodes("//h3");
            if (h3Node != null)
                foreach (HtmlNode node in h3Node)
                    { node.ParentNode.RemoveChild(node); }
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
                storyURL = "http://www.tthfanfic.org/" + addressParts[3] + "-";
                //Sets the storyURL to the base URL for whichever site plus the storyID.
                return "Download test passed.";
            }
        }
    }
}
