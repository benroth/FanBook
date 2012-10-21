//FanBook by Ben Roth
//This program takes a URL for Fanfiction.net or FictionPress.com stories, then compiles the story info
//and all the story storyChapters into a single, well-formated HTML document. This is useful for achival purposes
//in case of those stories being pulled off the site or the site going down, but also useful if the user
//wants to have stories available when they do not have internet access.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FanBook
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form1 myForm = new form1();
            Application.Run(myForm);
        }
    }
}
