using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
namespace download_images
{
    class Program
    {
        private static int page = 1, numberOfImage = 1;
        private static string locationDownload;
        private static string[] optionsTXT = { "Page:", "Image:" };
        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), @"options.txt");
            string[] lines = System.IO.File.ReadAllLines(path);

            if (Int32.Parse((lines[0].Split(":"))[1]) != 0)
            {
                page = Int32.Parse((lines[0].Split(":"))[1]);
                numberOfImage = Int32.Parse((lines[1].Split(":"))[1]);
                Console.WriteLine(" ----------------------------------------");
                Console.WriteLine("");
                Console.WriteLine("Resume from page: " + page + " ?");
                Console.WriteLine("");
                Console.WriteLine("Enter 1 to resume");
                Console.WriteLine("Enter 2 to start from begining");
                Console.WriteLine(" ----------------------------------------");
                Console.WriteLine("");
                var response = Console.ReadLine();
                if (response.Equals("2"))
                {
                    page = 1;
                    numberOfImage = 1;
                    optionsTXT[0] = "Page: 1";
                    optionsTXT[1] = "Image: 1";
                }
                Console.WriteLine("Insert location to download (ex: E:\\folderName):");
                locationDownload = Console.ReadLine();
                Console.WriteLine(" Started :) ");
            }
            else
            {
                Console.WriteLine(" ----------------------------------------");
                Console.WriteLine("");
                Console.WriteLine("You don't have a page set in options.txt and will download from begining.");
                Console.WriteLine("");
                Console.WriteLine(" ----------------------------------------");
                Console.WriteLine("");
                Console.WriteLine("Insert location to download (ex: E:\\folderName):");
                locationDownload = Console.ReadLine();
                optionsTXT[1] = "Location: " + locationDownload;
                Console.WriteLine(" Started :) ");

            }


            string[] articles, getLink;
            System.Net.WebClient wc = new System.Net.WebClient();
            string check = "id=\"post-";
            Dictionary<string, string> array = new Dictionary<string, string>();

            using (var writer = File.AppendText(Path.Combine(Directory.GetCurrentDirectory(), @"console-log.txt")))
            {
                writer.WriteLine(DateTime.Now);
            }
            for (int i = page; i <= 425; i++)
            {
                optionsTXT[0] = "Page: " + i;
                optionsTXT[1] = "Image: " + numberOfImage;
                string webData = wc.DownloadString("https://hentaiarena.com/images/page/" + i + "/");
                articles = webData.Split(new string[] { "id=\"post" }, StringSplitOptions.None);
                int number = 1;



                int ii = 0;

                foreach (string x in articles)
                {
                    articles = x.Split(new string[] { "\"" }, StringSplitOptions.None);
                    if (articles[4].Equals("><head><meta charset="))
                        continue;
                    //Console.WriteLine(articles[4]);
                    downloadImage(articles[4], i, number);
                    number++;

                }
            }


        }

        public static void downloadImage(string link, int pageNumber, int numberImageOnPage)
        {
            string logTXT;
            string[] image;
            System.Net.WebClient wc = new System.Net.WebClient();
            string webData = wc.DownloadString(link);
            image = webData.Split(new string[] { "entry-content" }, StringSplitOptions.None);
            image = image[1].Split(new string[] { "data-lazy-src=\"" }, StringSplitOptions.None);
            image = image[1].Split(new string[] { "\"" }, StringSplitOptions.None);

            link = "https:" + image[0];
            if (link.Contains("#038;"))
            {
                string[] redoLink = link.Split(new string[] { "#038;" }, StringSplitOptions.None);
                link = "";
                foreach (var o in redoLink)
                    link = link + o;
            }

            // FOR DEBUG
            //int i = 0;
            //foreach (var x in image)
            //{
            //    if (x.Contains("//i.redd.it/tgrkpx7vvyh41.png"))
            //        Console.WriteLine(x + "----" + i);
            //    i++;
            //}
            string currentFile = locationDownload + "\\" + numberOfImage + "a" + pageNumber + "b" + numberImageOnPage + ".jpg";
            string fileName = numberOfImage + "a" + pageNumber + "b" + numberImageOnPage + ".jpg";
            try
            {
                using (WebClient client = new WebClient())
                {
                    
                    if (!File.Exists(currentFile))
                    {
                        client.DownloadFile(new Uri(link), currentFile);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Black;
                        logTXT = "Success [" + fileName + "] [PAGE: " + pageNumber + "]: " + link;
                        Console.WriteLine(logTXT);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.BackgroundColor = ConsoleColor.Black;
                        logTXT = "File allready exists: [" + fileName + "] [PAGE: " + pageNumber + "]: " + link;
                        Console.WriteLine(logTXT);
                    }


                }
                numberOfImage++;
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), @"options.txt")))
                {
                    foreach (string line in optionsTXT)
                        outputFile.WriteLine(line);
                }
                using (var writer = File.AppendText(Path.Combine(Directory.GetCurrentDirectory(), @"console-log.txt")))
                {
                    writer.WriteLine(logTXT);
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                logTXT = "[ Can't download this image :( sorry. : [" + fileName + "] [PAGE: " + pageNumber + "]: " + link + " ]";
                Console.WriteLine(logTXT);
                using (var writer = File.AppendText(Path.Combine(Directory.GetCurrentDirectory(), @"console-log.txt")))
                {
                    writer.WriteLine(logTXT);
                }
            }


        }


    }
}
