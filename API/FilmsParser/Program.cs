using System;

namespace FilmsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();
            string rankingLink = "http://www.filmweb.pl/rankings/film/country/genre/";
            //genres id
            int[] categoryId = {28,2,66,42,55,3,47,4,41,57,5,6,59,37,65,69,46,64,7,45,70,8,
                9,75,27,53,60,11,12,54,40,13,74,58,29,30,14,50,15,16,17,44,67,18,19,62,43,
                52,76,20,73,38,68,51,32,39,33,22,61,10,63,72,24,25,26,71};

            string allFilmsLink = "http://www.filmweb.pl/search/film?q=&type=&startYear=&endYear=&countryIds=null&genreIds=null&startRate=&endRate=&startCount=&endCount=&sort=COUNT&sortAscending=false&c=portal&page=";

            //foreach (var id in categoryId)
            //{
            //    rankingLink += id;
            //    parser.GetRankingFilmLinks(rankingLink);
            //    parser.LoadFilmsInfo();
            //    rankingLink = "http://www.filmweb.pl/rankings/film/country/genre/";
            //}

            for (int i = 1; i <= 3000; i++)
            {
                allFilmsLink += i;
                parser.GetAllLinks(allFilmsLink);
                parser.LoadFilmsInfo();
                allFilmsLink = "http://www.filmweb.pl/search/film?q=&type=&startYear=&endYear=&countryIds=null&genreIds=null&startRate=&endRate=&startCount=&endCount=&sort=COUNT&sortAscending=false&c=portal&page=";
            }

            parser.SerializeFilmsToCSV();
            parser.SerializeDirectorsToCSV();
            parser.SerializeActorsToCSV();

            Console.ReadLine();
        }
    }
}

