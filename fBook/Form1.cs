using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace FanBook
{
    public partial class form1 : Form
    {
        private FanBook myBook;

        private void CreateBook()
        {
            addressBar.Text.ToLower();
            if (addressBar.Text.Contains("fanfiction.net/s/"))
                myBook = new FFNetBook();
            else if (addressBar.Text.Contains("fictionpress.com/s/"))
                myBook = new FictionPressBook();
            else if (addressBar.Text.Contains("fanficauthors"))
                myBook = new FanFicAuthorsBook();
            else if (addressBar.Text.Contains("tthfanfic.org/Story"))
                myBook = new TTHFanficBook();
        }

        public form1()
        {
            InitializeComponent();
        }
        
        private void btnGetInfo(object sender, EventArgs e)
        {//Tests URL for validity and starts download of story with story info generation.
            string testResults;

            CreateBook();                
            if (myBook == null)
            {
                webBrowser1.DocumentText = "Invalid URL! Please enter a story URL for a supported archive.";
                return;
            }

            if (webBrowser1.DocumentText != "")
            {
                ResetFormVariables();
                myBook.InitStoryVariables();
            }
            //Resets form and myBook variables.

            testResults = myBook.TestURLFormat(addressBar.Text);
            if (testResults.Contains("Invalid URL!"))
            { webBrowser1.DocumentText = testResults; }
            else
            { 
                testResults = myBook.TestURLDownload();
                if (testResults != "Download test passed.")
                    webBrowser1.DocumentText = testResults; //Tests to make sure URL is valid and reachable.
                else
                {//Sets story info to window and gives option to download.
                    webBrowser1.DocumentText = myBook.GenerateStoryInfo();   
                    if (!webBrowser1.DocumentText.Contains("Story grab failed!"))
                        btnDownloadStory.Enabled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {//Passes the info grabbed in GenerateStoryInfo to downloadStory to grab the story.
            btnDownloadStory.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
            
        }

        private void button3_Click_1(object sender, EventArgs e)
        {//Strips invalid characters from title and authorname, then gives save dialogue with those as the default.
            saveFileDialog1.FileName = myBook.GetFileTitle();
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {//Saves storyContent.
            string name = saveFileDialog1.FileName;
            File.WriteAllText(name, myBook.ReturnStory());
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {//Monitors URL field for enter key press, and if pressed, triggers Get Info button.
            if (e.KeyChar == 13)
                if (!addressBar.AcceptsReturn)
                    button1.PerformClick();
        }

        private void ResetFormVariables()
        {//Reset form to base values.
            lblProgressText.Text = null;
            webBrowser1.DocumentText = "";
            btnSaveStory.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {//Download chapters and pass progress to updater.           
            int chapters = (int)myBook.GetChapterCount();
            for (int i = 1; i <= chapters; i++)
            {
                myBook.DownloadStory(i-1);
                backgroundWorker1.ReportProgress(i);               
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {//Build story from downloaded components, set to browser window, offer save option.
            myBook.BuildStory();
            webBrowser1.DocumentText = myBook.ReturnStory();
            lblProgressText.Text = "Download Progress: Complete";
            btnSaveStory.Enabled = true;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {//Update progress as chapters download.
            lblProgressText.Text = "Download Progress: " + e.ProgressPercentage.ToString() + "/" + myBook.GetChapterCount();
        }
    }
}
