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

namespace fBook
{
    public partial class form1 : Form
    {
        FanBook myBook = new FanBook();

        public form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {//Tests URL for validity and starts download of story with story info generation.
            string testResults;

            if (webBrowser1.DocumentText != "")
                ResetFormVariables();
            myBook.InitStoryVariables();
            //Resets form and myBook variables.

            testResults = myBook.TestURLFormat(textBox1.Text);
            if (testResults.Contains("Invalid URL!"))
            { webBrowser1.DocumentText = testResults; }
            else
            { 
                testResults = myBook.TestURLDownload();
                if (testResults.Contains("Unable"))
                    webBrowser1.DocumentText = testResults; //Tests to make sure URL is valid and reachable.
                else
                {//Sets story info to window and gives option to download.
                    webBrowser1.DocumentText = myBook.GenerateStoryInfo();                    
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
                if (!textBox1.AcceptsReturn)
                    button1.PerformClick();
        }

        private void ResetFormVariables()
        {//Reset form to base values.
            lblProgressText.Text = null;
            webBrowser1.DocumentText = null;
            btnSaveStory.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {//Download chapters and pass progress to updater.           
            int chapters = (int)myBook.GetChapterCount();
            for (int i = 1; i <= 100; i++)
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
