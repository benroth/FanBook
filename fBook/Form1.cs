//fBook by Ben Roth
//This program takes a URL for Fanfiction.net or FictionPress.com stories, then compiles the story info
//and all the story chapters into a single, well-formated HTML document. This is useful for achival purposes
//in case of those stories being pulled off the site or the site going down, but also useful if the user
//wants to have stories available when they do not have internet access.
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

namespace fBook
{
    public partial class form1 : Form
    {

        int chapters = 0;
        string storyURL, storyTitle, authorName, storyStatus, storyWords, storyPublished, storyUpdated;
        List<string> chapterTitles = new List<string>();
        StringBuilder storyContent = new StringBuilder();
        StringBuilder storyInfo = new StringBuilder();

        public form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {//Takes URL entered and launches main function.
            Navigate(textBox1.Text);
        }

        private void getStoryInfo(String htmlContent)
        { //Grabs # of chapters, story title, author name, story status, and chapter titles. Sets all that to storyInfo string.
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);

            try { chapters = doc.DocumentNode.SelectNodes("//select[@id='chap_select']/option").Count() / 2; }
            catch { chapters = 1; }
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
            //Checks story info at the top for Complete label.

            storyInfo.Append("Title: " + storyTitle);
            storyInfo.Append("<br />Author: " + doc.DocumentNode.SelectSingleNode("//a[contains(@href, '/u/')]").OuterHtml);
            storyInfo.Append("<br />Chapters: " + chapters.ToString());
            storyInfo.Append("<br />Story " + storyWords);
            storyInfo.Append("<br />" + storyPublished);
            if (storyUpdated != "")
                storyInfo.Append("<br />Last " + storyUpdated);
            storyInfo.Append("<br />Story Status: " + storyStatus);
            storyInfo.Append("<br /><br />Summary: " + doc.DocumentNode.SelectSingleNode("//div[@style='margin-top:2px']").InnerText);
            storyInfo.Append("<br /><br /><br />");
            //Builds storyInfo string with previously-fetched variables, sets up formatting.

            if (chapters > 1)
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//select[@id='chap_select']/option")) 
                    chapterTitles.Add(node.NextSibling.InnerText);
            //Grabs chapter title text from dropdown selection values.

            button2.Enabled = true;
            //Enables Download Story button.
        }

        private string grabHtml(string address)
        {//Grabs HTML from provided address, or returns blank string in case of failure.
            try
            {
                WebClient client = new WebClient();
                client.Proxy = null;
                Stream data = client.OpenRead(new Uri(address));
                StreamReader reader = new StreamReader(data, Encoding.GetEncoding("UTF-8"));
                string htmlContent = reader.ReadToEnd();
                data.Close();
                reader.Close();
                return htmlContent;
            }

            catch { return ""; }
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
        }

        private void downloadStory(string storyURL, int chapters)
        {//Compiles story chapters and buids formatting around them.
            button2.Enabled = false;
            storyContent.Clear();
            storyContent.Append("<html><body><meta http-equiv=\"content-type\" content=\"text/html;charset=UTF-8\" />" + storyInfo);
            //Disables Download Story button since it's been pressed. Clears storyContent string and puts storyInfo at the top.

            if (chapters > 1)
            {//Builds TOC for stories with multiple chapters.
                storyContent.Append("<h2>Table of Contents</h2>");

                for (int i = 0; i < chapters; i++)
                {//Strips out chapter number and just leaves title intact, then creates links for each chapter title.
                    chapterTitles[i] = chapterTitles[i].Remove(0, i.ToString().Length + 2);
                    storyContent.Append("<a href=\"#" + chapterTitles[i] + "\">" + chapterTitles[i] + "</a><br />");
                }
                storyContent.Append("<br /><br />");

                for (int i = 0; i < chapters; i++)
                {//Creates an anchor of each chapter title, followed by the chapter itself. Ticks progress counter.
                    storyContent.Append("<h2><a name=\"" + chapterTitles[i] + "\">" + chapterTitles[i] + "</h2>" + getChapter(storyURL + "/" + (i+1).ToString()));
                    if (i < chapters)
                        storyContent.Append("<br />");
                    label4.Text = "Download Progress: " + i + "/" + chapters;
                    Application.DoEvents();
                }
            }
            else
            {
                storyContent.Append(getChapter(storyURL + "/" + 1));
                label4.Text = "Download Progress: " + 0 + "/" + chapters;
                Application.DoEvents();
            }
   
            if (storyStatus == "Complete")
                storyContent.Append("<h3>End of story.</h3></body></html>");
            else
                storyContent.Append("<h3>End of posted chapters.</h3></body></html>");
            //Adds comment at end of story to indicate reason for ending.

            webBrowser1.DocumentText = storyContent.ToString();
            label4.Text = "Download Progress: Complete";
            button3.Enabled = true;
            //Puts story in window, finishes progress bar, and enables Save Story button.
        }

        private void Navigate(String address)
        {//Clears variables, verifies valid URL, then passes reformated URL to getStoryInfo and assigns the results to content window.
            label4.Text = "";
            storyInfo.Clear();
            chapterTitles.Clear();
            storyUpdated = "";
            button3.Enabled = false; 
            //Clears variables, sets Save Story to false

            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            if (!address.Contains("fanfiction.net") && !address.Contains("fictionpress.com"))
            {
                webBrowser1.DocumentText = "Invalid URL! Please enter URLs for fanfiction.net or fictionpress.com stories only.";
                return;
            }
            //Verifies HTML is entered, sets proper formatting, 
            string htmlContent = grabHtml(address);

            if (htmlContent == "")
            {//Passback to original function in case of grabHTML failure.
                webBrowser1.DocumentText = "Unable to fetch story.";
                return;
            }
            else if (htmlContent.Contains("Unable to locate story. Code 1."))
            {//Passback to original function in case of valid-formatted URL that doesn't point to a story.
                webBrowser1.DocumentText = "Unable to locate story.";
                return;
            }
            else
            {//Parses address to grab just the base URL and the story ID, then passes the grabHTML results to get the story info and set it to the content window.
                string[] addressParts = address.Split('/');
                if (address.Contains("fanfiction.net"))
                    storyURL = "http://www.fanfiction.net/s/" + addressParts[4];
                else
                    storyURL = "http://www.fictionpress.com/s/" + addressParts[4];
                //Sets the storyURL to the base URL for whichever site plus the storyID.

                getStoryInfo(htmlContent);
                webBrowser1.DocumentText = storyInfo.ToString();
            }    
        }

        private void button2_Click(object sender, EventArgs e)
        {//Passes the info grabbed in getStoryInfo to downloadStory to grab the story.
            downloadStory(storyURL, chapters);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {//Strips invalid characters from title and authorname, then gives save dialogue with those as the default.
            foreach (char character in Path.GetInvalidFileNameChars())
                storyTitle = storyTitle.Replace(character.ToString(), string.Empty);

            foreach (char character in Path.GetInvalidFileNameChars())
                authorName = authorName.Replace(character.ToString(), string.Empty);

            saveFileDialog1.FileName = authorName + " - " + storyTitle;
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {//Saves storyContent.
            string name = saveFileDialog1.FileName;
            File.WriteAllText(name, storyContent.ToString());
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {//Monitors URL field for enter key press, and if pressed, triggers Get Info button.
            if (e.KeyChar == 13)
                if (!textBox1.AcceptsReturn)
                    button1.PerformClick();
        }                     
    }
}
