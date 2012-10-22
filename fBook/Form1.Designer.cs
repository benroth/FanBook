namespace FanBook
{
    partial class form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addressBar = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.btnDownloadStory = new System.Windows.Forms.Button();
            this.lblProgressText = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnSaveStory = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // addressBar
            // 
            this.addressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressBar.Location = new System.Drawing.Point(77, 12);
            this.addressBar.Name = "addressBar";
            this.addressBar.Size = new System.Drawing.Size(487, 20);
            this.addressBar.TabIndex = 0;
            this.addressBar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Story URL:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Story:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(589, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(54, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Get Info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnGetInfo);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(15, 56);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(628, 483);
            this.webBrowser1.TabIndex = 5;
            // 
            // btnDownloadStory
            // 
            this.btnDownloadStory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownloadStory.Enabled = false;
            this.btnDownloadStory.Location = new System.Drawing.Point(15, 545);
            this.btnDownloadStory.Name = "btnDownloadStory";
            this.btnDownloadStory.Size = new System.Drawing.Size(90, 23);
            this.btnDownloadStory.TabIndex = 8;
            this.btnDownloadStory.Text = "Download Story";
            this.btnDownloadStory.UseVisualStyleBackColor = true;
            this.btnDownloadStory.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblProgressText
            // 
            this.lblProgressText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgressText.AutoSize = true;
            this.lblProgressText.Location = new System.Drawing.Point(494, 550);
            this.lblProgressText.Name = "lblProgressText";
            this.lblProgressText.Size = new System.Drawing.Size(0, 13);
            this.lblProgressText.TabIndex = 10;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "html";
            this.saveFileDialog1.Filter = "Web Page (*.htm;*.html)|*.htm;*.html|All files(*.*)|*.*\"";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // btnSaveStory
            // 
            this.btnSaveStory.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSaveStory.Enabled = false;
            this.btnSaveStory.Location = new System.Drawing.Point(256, 545);
            this.btnSaveStory.Name = "btnSaveStory";
            this.btnSaveStory.Size = new System.Drawing.Size(75, 23);
            this.btnSaveStory.TabIndex = 11;
            this.btnSaveStory.Text = "Save Story";
            this.btnSaveStory.UseVisualStyleBackColor = true;
            this.btnSaveStory.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 580);
            this.Controls.Add(this.btnSaveStory);
            this.Controls.Add(this.lblProgressText);
            this.Controls.Add(this.btnDownloadStory);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addressBar);
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "form1";
            this.Text = "FanBook";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox addressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button btnDownloadStory;
        private System.Windows.Forms.Label lblProgressText;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnSaveStory;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}

