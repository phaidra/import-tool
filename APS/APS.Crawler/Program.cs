using APS.Lib.Crawler;
using CLAP;
using System;

namespace APS.Crawler
{
    class CrawlerApp
    {
        [Verb(IsDefault = true)]
        public static void Crawl(string rootDir, string crawlSubDir, string csvPath, string extensions)
        {
            FileCrawler crawler = new FileCrawler();
            crawler.Initialize(rootDir, crawlSubDir, csvPath, extensions);

            if (!crawler.TestParameters())
            {
                return;
            }

            crawler.Crawl();
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return Parser.Run<CrawlerApp>(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return 0;
        }
    }
}
