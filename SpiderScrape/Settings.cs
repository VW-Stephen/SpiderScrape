using System.Collections.Generic;

/// <summary>
/// Global application settings
/// </summary>
class Settings
{
    /// <summary>
    /// The number of pages to visit before dumping the data to disk
    /// </summary>
    public static int PagesPerDump = 50;

    /// <summary>
    /// The number of seconds before we give up on an HTTP request and try again
    /// </summary>
    public static int HttpRequestTimeout = 30;

    /// <summary>
    /// The initial URL to use in the scrape
    /// </summary>
    public static string BrowserStartingURL = "";

    /// <summary>
    /// The regex that matches good URLs to crawl
    /// </summary>
    public static string GoodURLRegex = "(http://*)";

    /// <summary>
    /// The regex that matches bad URLs to ignore when crawling
    /// </summary>
    public static string BadURLRegex = "";

    /// <summary>
    /// The file that contains all the visited URLs
    /// </summary>
    public static string VisitedURLFile = "visited.txt";

    /// <summary>
    /// The file that contains all the pending URLs
    /// </summary>
    public static string PendingURLFile = "pending.txt";

    /// <summary>
    /// The file that contains the scraped data
    /// </summary>
    public static string ScrapedDataFile = "data.txt";

    /// <summary>
    /// The list of IDs to scrape from each page
    /// </summary>
    public static List<string> IDsToScrape = new List<string>();
}