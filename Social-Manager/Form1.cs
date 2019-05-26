using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Chrome
using CefSharp;
using CefSharp.WinForms;


namespace Social_Manager
{
    

    public partial class Form1 : Form
    {
        IDictionary<string, ChromiumWebBrowser> dict = new Dictionary<string, ChromiumWebBrowser>();
        IDictionary<string, Button> btnDict = new Dictionary<string, Button>();
        IDictionary<string, Image> btnImage = new Dictionary<string, Image>();
        string currentWindow = "";

        public Form1()
        {
            InitializeComponent();
            this.SizeChanged += new EventHandler(Form1_Resize);

            dict.Add( "Trello", newBrowser("http://trello.com") );
            dict.Add( "Facebook", newBrowser("http://facebook.com") );
            dict.Add( "Gmail", newBrowser("https://accounts.google.com/AccountChooser?service=mail&continue=https://mail.google.com/mail/") );

            
            btnDict.Add( "Trello", NewButton("Trello", 20) );
            btnDict.Add( "Facebook", NewButton("Facebook", 90) );
            btnDict.Add( "Gmail", NewButton("Gmail", 160) );

            
            btnImage.Add("Trello", Properties.Resources.Trello);
            btnImage.Add("Facebook", Properties.Resources.Facebook);
            btnImage.Add("Gmail", Properties.Resources.Gmail);


            foreach (KeyValuePair<string, Button> item in btnDict)
            {
                btnPanel.Controls.Add(item.Value);
                System.Diagnostics.Debug.WriteLine(item.Value.Top);
                item.Value.BackgroundImage = btnImage[item.Key];
                item.Value.BackgroundImageLayout = ImageLayout.Stretch;
            }

            //Chrome
            if (!Cef.IsInitialized)
            {
                var settings = new CefSettings();

                settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.157 Safari/537.36";
                settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";
                settings.UserDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF\User data\";
                settings.PersistSessionCookies = true;
                settings.PersistUserPreferences = true;
                Cef.Initialize(settings);
            }
                
        }

        private void resizeControls(int h, int w)
        {
            if (currentWindow == "")
                return;
            btnPanel.Height = h;
            mPanel.Height = h;
            mPanel.Width = w - 70;
            mPanel.Left = 70;
            mPanel.Controls.Add(dict[currentWindow]);
        }
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = sender as Control;
            resizeControls(ClientRectangle.Height, ClientRectangle.Width);

        }

        private void readconsolemessage(object sender, CefSharp.ConsoleMessageEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("New console message:");
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
        
        private ChromiumWebBrowser newBrowser(string url)
        {
            // Turn the url into something useable
            Uri uri = new System.Uri(url, System.UriKind.Absolute);

            // Create a new empty browser
            ChromiumWebBrowser wb = new ChromiumWebBrowser("");
            
            // Load the url
            wb.Load(url);

            // Same dimensions as the panel
            wb.Top = mPanel.Top;
            wb.Left = mPanel.Left;

            // Start hidden
            wb.Visible = false;
            
            // Prints browser console messages to the  console
            wb.ConsoleMessage += readconsolemessage;

            // Handles popups and new tabs/new windows
            wb.LifeSpanHandler = new BrowserLifeSpanHandler();
            return wb;
        }

        private Button NewButton(string name, int top)
        {
            Button db = new Button();

            // Set Button properties
            db.Location = new Point(0, 0);
            db.Top = top;
            db.Left = 10;
            db.Width = 50;
            db.Height = 50;
            db.FlatStyle = FlatStyle.Flat;
            db.FlatAppearance.BorderSize = 0;
            db.FlatAppearance.BorderColor = Color.Red;
            db.Name = name;
            db.Font = new Font("Georgia", 16);
            db.Click += new EventHandler( BtnClick );
            db.MouseEnter += BtnEnter;
            db.MouseLeave += BtnLeave;



            // Add Button to the Form. Placement of the Button

            // will be based on the Location and Size of button



            return db;
        }
        void BtnClick(object sender, EventArgs e)
        {
            if (currentWindow != "")
            {
                dict[currentWindow].Visible = false;
                dict[currentWindow].SendToBack();
                mPanel.Controls.Remove(dict[currentWindow]);
            }

            Button btn = sender as Button;
            dict[btn.Name].Visible = true;
            dict[btn.Name].BringToFront();
            currentWindow = btn.Name;

            resizeControls(ClientRectangle.Height, ClientRectangle.Width);
        }
        void BtnEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatAppearance.BorderSize = 1;
        }
        void BtnLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatAppearance.BorderSize = 0;
        }
    }

    /*
     * This class handles popups
     */
    public class BrowserLifeSpanHandler : ILifeSpanHandler
    {
        //http://cefsharp.github.io/api/57.0.0/html/T_CefSharp_WindowOpenDisposition.htm
        // The types of popups we want to load, all others will be cancelled
        int[] allowedDispositions = { 1, 2, 3, 4};

        public bool OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName,
            WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo,
            IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            if (allowedDispositions.Contains((int)targetDisposition))
            {
                browserControl.Load(targetUrl);   
            }
            return true;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
            //
        }

        public bool DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return false;
        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
            //nothing
        }
    }
}
