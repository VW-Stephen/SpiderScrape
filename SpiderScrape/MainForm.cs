using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// The main form used by the spider to crawl a site
/// </summary>
public partial class MainForm : Form
{
    #region Members
    /// <summary>
    /// The list of URLs that have already been visited
    /// </summary>
    private List<string> _visited_urls;
    /// <summary>
    /// The list of URLs that are pending a visit
    /// </summary>
    private List<string> _pending_urls;
    /// <summary>
    /// The list of data that's been scraped from the site
    /// </summary>
    private List<string> _scraped_data;
    /// <summary>
    /// If we need to pause the spider or not
    /// </summary>
    private bool _flag_pause;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a new instance of the main dump form
    /// </summary>
    public MainForm()
    {
        InitializeComponent();

        // Set up the initial URL lists, and unflag the pause mechanism
        _pending_urls = new List<string>();
        _visited_urls = new List<string>();
        _scraped_data = new List<string>();
        _flag_pause = false;

        // Disable the pause button until the user clicks start
        pause_button.Enabled = false;

        // Read the configuration data, and any existing URL information we may have from disk
        ReadConfig();
        ReadURLs();

        // If, after reading the data from disk, we don't have any pending/visited URLs, we start fresh
        if (_pending_urls.Count == 0 && _visited_urls.Count == 0)
        {
            AddLogText("Starting New Scrape...");
            _pending_urls.Add(Settings.BrowserStartingURL);

            if (File.Exists(Settings.ScrapedDataFile))
            {
                AddLogText("Deleting Old Data...");
                File.Delete(Settings.ScrapedDataFile);
            }
        }
    }
    #endregion

    #region Private Funcitons
    /// <summary>
    /// Adds the given text to the log
    /// </summary>
    private void AddLogText(string text)
    {
        text_log.Text = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + text + "\r\n" + text_log.Text;
        if (text_log.Text.Length > 10000)
            text_log.Text = text_log.Text.Substring(0, 5000);
        Application.DoEvents();
    }

    /// <summary>
    /// Performs the site crawl/dump/scrape
    /// </summary>
    private void CrawlSite()
    {
        DateTime request_start;
        string tmp, current;
        bool am_online;
        int visited_this;

        visited_this = 0;
        while (true)
        {
            // Pop a pending URL off the stack, put it in the visited. Oh, and visit it.
            current = _pending_urls[0];
            _visited_urls.Add(current);
            _pending_urls.RemoveAt(0);

            am_online = false;
            while (!am_online)
            {
                web_browser.Navigate(current);
                request_start = DateTime.Now;
                while (web_browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    // If the request is taking too long, make the request again please!
                    if ((DateTime.Now - request_start).TotalSeconds >= Settings.HttpRequestTimeout)
                    {
                        request_start = DateTime.Now;
                        web_browser.Navigate(current);
                        AddLogText("Timeout Reached, Making Request Again...");
                    }

                    Thread.Sleep(100);
                    Application.DoEvents();
                }
                Application.DoEvents();

                // If we don't get the 404 page, then continue on. Otherwise, try again...<h1 id="mainTitle">This program cannot display the webpage</h1>
                // TODO: There's gotta be a better way to do this... very hackish
                if (!web_browser.DocumentText.Contains("<h1 id=\"mainTitle\">This program cannot display the webpage</h1>"))
                    am_online = true;
            }

            // Once there, scrape for new profile and get some URLs
            foreach (HtmlElement link in web_browser.Document.Links)
            {
                // Strip out any strings in the URL that could make it unique when it really isn't
                tmp = link.GetAttribute("href");
                if (tmp.Contains("?"))
                    tmp = tmp.Substring(0, tmp.IndexOf("?"));
                if (tmp.Contains("#"))
                    tmp = tmp.Substring(0, tmp.IndexOf("#"));

                // Check to see if the URL is a bad one to ignore
                if (!Regex.IsMatch(tmp, Settings.BadURLRegex, RegexOptions.IgnoreCase))
                {
                    // If we haven't seen this URL before, add it to the list
                    if (Regex.IsMatch(tmp, Settings.GoodURLRegex, RegexOptions.IgnoreCase) && !_visited_urls.Contains(tmp) && !_pending_urls.Contains(tmp))
                        _pending_urls.Add(tmp);
                }
            }

            // Scrape the data from the page, if we have any IDs to scrape
            if (Settings.IDsToScrape.Count != 0)
            {
                try { ScrapePage(current); }
                catch { AddLogText("Error Saving Entry!"); }
            }

            visited_this++;
            AddLogText("Visited: " + _visited_urls.Count + ", Pending: " + _pending_urls.Count);

            // If we're out of stuff to crawl, be done
            if (_pending_urls.Count == 0)
            {
                AddLogText("Dump complete!");
                SaveData();
                break;
            }

            // If we've been told to pause, save the data and pause the scrape
            if (_flag_pause)
            {
                _flag_pause = false;

                AddLogText("Saving URLs...");
                SaveData();
                AddLogText("Done Saving URLs!");

                AddLogText("Dump Paused!");
                break;
            }
            
            // If we need to flush the info to the disk, then do it
            if (visited_this % Settings.PagesPerDump == 0)
            {
                AddLogText("Saving Data...");
                SaveData();
                AddLogText("Done Saving Data!");
            }
        }
    }

    /// <summary>
    /// Reads the config file, sets the configuration data
    /// </summary>
    private void ReadConfig()
    {
        string property, value;
        int tmp_i;  

        // Read in the config line by line, setting the values along the way
        foreach(string line in File.ReadAllLines("config.txt"))
        {
            if(line.Contains("="))
            {
                property = line.Substring(0, line.IndexOf('=')).Trim();
                value = line.Substring(line.IndexOf('=') + 1).Trim().ToLower();

                if (property == "browser_start_url")
                    Settings.BrowserStartingURL = value;
                else if (property == "pages_per_dump" && Int32.TryParse(value, out tmp_i))
                    Settings.PagesPerDump = tmp_i;
                else if (property == "http_request_timeout" && Int32.TryParse(value, out tmp_i))
                    Settings.HttpRequestTimeout = tmp_i;
                else if (property == "visited_url_file")
                    Settings.VisitedURLFile = value;
                else if (property == "pending_url_file")
                    Settings.PendingURLFile = value;
                else if (property == "scraped_data_file")
                    Settings.ScrapedDataFile = value;
                else if (property == "good_url_regex")
                    Settings.GoodURLRegex = value;
                else if (property == "bad_url_regex")
                    Settings.BadURLRegex = value;
                else if (property == "ids_to_scrape")
                    Settings.IDsToScrape.AddRange(value.Split(','));
                else if (!property.StartsWith("#") && property != "")
                    AddLogText("Unknown setting '" + property + "' in config file, ignoring line...");
            }
            else if (!line.StartsWith("#") && line != "")
            {
                AddLogText("Unknown setting '" + line + "' in config file, ignoring line...");
            }
            
        }
    }

    /// <summary>
    /// Reads in the pending/visited URLs from the files
    /// </summary>
    private void ReadURLs()
    {
        int i = 0;
        // Clear the URL lists
        _pending_urls.Clear();
        _visited_urls.Clear();

        // Read the URLs in to the lists
        if (File.Exists(Settings.PendingURLFile))
        {
            foreach (string s in File.ReadAllLines(Settings.PendingURLFile))
            {
                _pending_urls.Add(s);
                i++;
            }
        }
        AddLogText("Read " + i + " pending URLs from disk.");

        i = 0;
        if (File.Exists(Settings.VisitedURLFile))
        {
            foreach (string s in File.ReadAllLines(Settings.VisitedURLFile))
            {
                _visited_urls.Add(s);
                i++;
            }
        }
        AddLogText("Read " + i + " visited URLs from disk.");
    }

    /// <summary>
    /// Adds an entry to the log file for the given URL
    /// </summary>
    private void ScrapePage(string url)
    {
        HtmlDocument doc;
        int len, i;
        string tmp;

        // Prep the document and sources
        doc = web_browser.Document;
        len = Settings.IDsToScrape.Count;
        tmp = "";

        // Set up the CSV string
        for (i = 0; i < len; i++)
        {
            // Get the content from each of the tags, if we can
            try { tmp += "\"" + doc.GetElementById(Settings.IDsToScrape[i]).InnerHtml.Replace('"', '\'') + "\""; }
            catch { }

            if(i != len - 1)
                tmp += ",";
        }

        tmp += "\r\n";

        // Add the string to the list of scraped data
        _scraped_data.Add(tmp);
    }

    /// <summary>
    /// Saves the list of pending/visited urls to files, and all the scraped data
    /// </summary>
    private void SaveData()
    {
        // Copy the files that we wrote earlier (if they exist) so we don't lose data
        if(File.Exists(Settings.VisitedURLFile + ".old"))
            File.Delete(Settings.VisitedURLFile + ".old");
        if (File.Exists(Settings.VisitedURLFile))
            File.Move(Settings.VisitedURLFile, Settings.VisitedURLFile + ".old");

        if (File.Exists(Settings.PendingURLFile + ".old"))
            File.Delete(Settings.PendingURLFile + ".old");
        if (File.Exists(Settings.PendingURLFile))
            File.Move(Settings.PendingURLFile, Settings.PendingURLFile + ".old");

        if (File.Exists(Settings.ScrapedDataFile + ".old"))
            File.Delete(Settings.ScrapedDataFile + ".old");
        if (File.Exists(Settings.ScrapedDataFile))
            File.Copy(Settings.ScrapedDataFile, Settings.ScrapedDataFile + ".old");

        // Save the data to the files now
        File.WriteAllLines(Settings.VisitedURLFile, _visited_urls.ToArray());
        File.WriteAllLines(Settings.PendingURLFile, _pending_urls.ToArray());

        foreach (string s in _scraped_data)
            File.AppendAllText(Settings.ScrapedDataFile, s);
        _scraped_data.Clear();

    }
    #endregion

    #region UI Functions
    /// <summary>
    /// Handles the pause button's click event
    /// </summary>
    private void pause_button_click(object sender, EventArgs e)
    {
        // Flag the pause function to stop after the given item
        _flag_pause = true;
        pause_button.Enabled = false;
    }

    /// <summary>
    /// Handles the start button's click event
    /// </summary>
    private void start_button_click(object sender, EventArgs e)
    {
        // Perform the crawl
        start_button.Enabled = false;
        pause_button.Enabled = true;

        try
        {
            CrawlSite();
        }
        catch
        {
            AddLogText("Error Dumping, Check Connection..?");
        }

        start_button.Enabled = true;
        pause_button.Enabled = false;
    }
    #endregion
}