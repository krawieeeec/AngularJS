using FilmsParser.Models;
using HtmlAgilityPack;
using ServiceStack.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FilmsParser
{
    class Parser
    {
        public Parser()
        {
            AllLinks = new List<string>();
            BrokenLinks = new List<string>();
            HtmlWeb = new HtmlWeb
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            m_Films = new List<Film>();
            m_Actors = new List<Actor>();
            m_Directors = new List<Director>();
        }

        HtmlDocument Document { get; set; }
        IEnumerable<HtmlNode> Prepare_links { get; set; }
        List<string> AllLinks { get; set; }
        List<string> BrokenLinks { get; set; }
        HtmlWeb HtmlWeb { get; set; }

        //Film attributes
        IEnumerable<HtmlNode> Descriptions { get; set; }
        IEnumerable<HtmlNode> Titles { get; set; }
        IEnumerable<HtmlNode> OriginalTitles { get; set; }
        IEnumerable<HtmlNode> Genres { get; set; }
        IEnumerable<HtmlNode> Covers { get; set; }
        IEnumerable<HtmlNode> ReleaseDates { get; set; }
        IEnumerable<HtmlNode> Actors { get; set; }
        IEnumerable<HtmlNode> Directors { get; set; }
        IEnumerable<HtmlNode> FilmwebUrls { get; set; }

        List<Film> m_Films { get; set; }
        List<Actor> m_Actors { get; set; }
        List<Director> m_Directors { get; set; }

        public void GetRankingFilmLinks(string url)
        {
            AllLinks.Clear();
            System.Console.WriteLine("***********************************");
            System.Console.WriteLine("Pobieram linki filmow z rankingow\n" + url);
            System.Console.WriteLine("***********************************");
            // e.g. http://www.filmweb.pl/rankings/film/country/genre/
            Document = HtmlWeb.Load(url);

            Prepare_links = Document.DocumentNode.Descendants("a");
            foreach (var link in Prepare_links)
            {
                if (link.GetAttributeValue("class", "test") == "s-20")
                {
                    string Link = "http://filmweb.pl" + link.Attributes["href"].Value;
                    AllLinks.Add(Link);
                }
            }
            System.Console.WriteLine("Pobrano linki.");
        }

        public void GetAllLinks(string url)
        {
            AllLinks.Clear();
            System.Console.WriteLine("***********************************");
            System.Console.WriteLine("Pobieram linki filmow z ogolnego spisu \n" + url);
            System.Console.WriteLine("***********************************");
            // e.g. http://www.filmweb.pl/rankings/film/country/genre/
            Document = HtmlWeb.Load(url);

            Prepare_links = Document.DocumentNode.Descendants("a");
            foreach (var link in Prepare_links)
            {
                if (link.GetAttributeValue("class", "test") == "filmTitle gwt-filmPage")
                {
                    string Link = "http://filmweb.pl" + link.Attributes["href"].Value;
                    AllLinks.Add(Link);
                }
            }
            System.Console.WriteLine("Pobrano linki.");
        }

        public void LoadFilmsInfo()
        {
            System.Console.WriteLine("***********************************");
            System.Console.WriteLine("Wczytuje filmy.");
            System.Console.WriteLine("***********************************");
            foreach (var link in AllLinks)
            {
                Film film = new Film();

                Document = HtmlWeb.Load(link);

                if (Document.DocumentNode.HasChildNodes == false)
                {
                    BrokenLinks.Add(link);
                    continue;
                }

                film.FilmwebUrl = link;

                Titles = Document.DocumentNode.Descendants("a");
                foreach (var title in Titles)
                {
                    if (title.GetAttributeValue("typeof", "test") == "v:Review-aggregate")
                    {
                        film.Name = WebUtility.HtmlDecode(title.InnerText);
                    }
                }

                OriginalTitles = Document.DocumentNode.Descendants("h2");
                foreach (var title in OriginalTitles)
                {
                    if (title.GetAttributeValue("class", "test") == "cap s-16 top-5")
                    {
                        film.OriginalName = WebUtility.HtmlDecode(title.InnerText);
                    }
                }

                Genres = Document.DocumentNode.Descendants("ul");
                foreach (var genre in Genres)
                {
                    if (genre.GetAttributeValue("class", "test") == "inline sep-comma genresList")
                    {
                        film.Genre = WebUtility.HtmlDecode(genre.InnerText);
                        film.Genre = AddSpacesToSentence(film.Genre);
                    }
                }

                ReleaseDates = Document.DocumentNode.Descendants("a");
                foreach (var date in ReleaseDates)
                {
                    //http://www.filmweb.pl [21]
                    var shorterLink = link.Substring(17, link.Length - 17) + "/dates";
                    if (date.GetAttributeValue("href", "test") == shorterLink)
                    {
                        film.ReleaseDate = WebUtility.HtmlDecode(date.InnerText);
                    }
                }

                Descriptions = Document.DocumentNode.Descendants("div");
                foreach (var description in Descriptions)
                {
                    if (description.GetAttributeValue("class", "test") == "filmPlot bottom-15")
                    {
                        film.Description = WebUtility.HtmlDecode(description.InnerText);
                    }
                }

                Covers = Document.DocumentNode.Descendants("img");
                foreach (var cover in Covers)
                {
                    string imgContent = cover.GetAttributeValue("alt", "test");
                    string imgContentWithoutSpaces = imgContent.Replace(" ", string.Empty);
                    string titleWithoutSpaces = film.Name.Replace(" ", string.Empty);
                    if (imgContentWithoutSpaces.Contains(titleWithoutSpaces))
                    {
                        film.Cover = cover.GetAttributeValue("src", "test");
                    }
                }
                if (film.Name != null)
                {
                    m_Films.Add(film);
                    System.Console.WriteLine("Pobrano zawartosc filmu: " + film.Name);
                }

                int actorsCount = 0;
                Actors = Document.DocumentNode.Descendants("a");
                foreach (var actorName in Actors)
                {
                    if (actorName.GetAttributeValue("rel", "test") == "v:starring")
                    {
                        Actor actor = new Actor();
                        actor.Name = WebUtility.HtmlDecode(actorName.InnerText);
                        actor.FilmName = film.Name;

                        if (actor.Name != null)
                        {
                            m_Actors.Add(actor);
                            actorsCount++;
                        }

                        // we will display only 5 actors per film
                        if (actorsCount >= 5)
                        {
                            actorsCount = 0;
                            break;
                        }

                    }
                }

                Directors = Document.DocumentNode.Descendants("a");
                foreach (var directorName in Directors)
                {
                    if (directorName.GetAttributeValue("rel", "test") == "v:directedBy")
                    {
                        Director director = new Director();
                        director.Name = WebUtility.HtmlDecode(directorName.InnerText);
                        director.FilmName = film.Name;

                        if (director.Name != null)
                        {
                            m_Directors.Add(director);
                        }
                    }
                }
            }
        }

        public void SerializeFilmsToCSV()
        {
            ServiceStack.Text.CsvConfig.ItemSeperatorString = ";";
            var CsvDocument = CsvSerializer.SerializeToCsv(m_Films);

            var file = @"C:\Users\kula\Desktop\db\Films.csv";

            using (var w = new StreamWriter(file, false, Encoding.Unicode))
            {
                w.WriteLine(CsvDocument);
                w.Flush();
            }
            System.Console.WriteLine("***********************************");
            System.Console.WriteLine("Zapisano filmy do pliku.");
            System.Console.WriteLine("***********************************");

            if (BrokenLinks.Count > 0)
            {
                var brokenLinks = @"C:\Users\kula\Desktop\db\BrokenLinks.txt";

                using (var w = new StreamWriter(brokenLinks, false, Encoding.Unicode))
                {
                    foreach (string s in BrokenLinks)
                    {
                        w.WriteLine(s);
                    }

                    w.Flush();
                }
            }
        }

        public void SerializeActorsToCSV()
        {
            ServiceStack.Text.CsvConfig.ItemSeperatorString = ";";
            var CsvDocument = CsvSerializer.SerializeToCsv(m_Actors);

            var file = @"C:\Users\kula\Desktop\db\Actors.csv";

            using (var w = new StreamWriter(file, false, Encoding.Unicode))
            {
                w.WriteLine(CsvDocument);
                w.Flush();
            }
            System.Console.WriteLine("***********************************");
            System.Console.WriteLine("Zapisano aktorow do pliku.");
            System.Console.WriteLine("***********************************");
        }

        public void SerializeDirectorsToCSV()
        {
            ServiceStack.Text.CsvConfig.ItemSeperatorString = ";";
            var CsvDocument = CsvSerializer.SerializeToCsv(m_Directors);

            var file = @"C:\Users\kula\Desktop\db\Directors.csv";

            using (var w = new StreamWriter(file, false, Encoding.Unicode))
            {
                w.WriteLine(CsvDocument);
                w.Flush();
            }
            System.Console.WriteLine("***********************************");
            System.Console.WriteLine("Zapisano rezyserow do pliku.");
            System.Console.WriteLine("***********************************");
        }

        string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ' && text[i - 1] != '-')
                {
                    newText.Append(',');
                    newText.Append(' ');
                }

                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
