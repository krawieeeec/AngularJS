using DataModel.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataModel.Repository
{
    public class ListRepository : IListRepository
    {
        string _connectionString;

        public ListRepository(bool isTest)
        {
            if (isTest)
                _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=DB_A15604_sportoweswiry;Integrated Security=True;";
            else
                _connectionString = @"Data Source=SQL5025.SmarterASP.NET;Initial Catalog=DB_A15604_sportoweswiry;User Id=DB_A15604_sportoweswiry_admin;Password=haslo123;";
        }

        public List GetSingle(int listId)
        {
            List list = new List();
            string query = "select * from List where id=" + listId + ";";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Id = (int)rdr["Id"];
                        list.Name = (string)rdr["Name"];
                        list.Likes = (int)rdr["Likes"];
                        list.Dislikes = (int)rdr["Dislikes"];
                    }
                }
            }

            return list;
        }

        public List<List> GetTopLists()
        {
            List<List> lists = new List<List>();
            string query = "select top(3) * from List order by Likes desc;";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        List list = new List()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            Likes = (int)rdr["Likes"],
                            Dislikes = (int)rdr["Dislikes"]
                        };
                        lists.Add(list);
                    }
                }
            }

            return lists;
        }

        public List<List> GetMany(IEnumerable<int> ids)
        {
            string listIds = "";
            foreach (var id in ids)
            {
                listIds += id;
                listIds += ",";
            }
            listIds = listIds.Remove(listIds.Length - 1);
            List<List> lists = new List<List>();

            string query = "select * from List where id IN (" + listIds + ");";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        List list = new List()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            Likes = (int)rdr["Likes"],
                            Dislikes = (int)rdr["Dislikes"]
                        };
                        lists.Add(list);
                    }
                }
            }

            return lists;
        }

        public List<List> GetAll()
        {
            List<List> lists = new List<List>();
            string query = "select * from List;";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        List list = new List()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            Likes = (int)rdr["Likes"],
                            Dislikes = (int)rdr["Dislikes"]
                        };
                        lists.Add(list);
                    }
                }
            }

            return lists;
        }

        public List<Film> GetListFilms(int listId)
        {
            List<Film> films = new List<Film>();
            string query = "select * from Film where id IN (select Film_Id from ListFilm where List_Id=" + listId + ");";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Film film = new Film()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            OriginalName = (string)rdr["OriginalName"],
                            ReleaseDate = (string)rdr["ReleaseDate"],
                            Genre = (string)rdr["Genre"],
                            Description = (string)rdr["Description"],
                            Cover = (string)rdr["Cover"],
                            FilmwebUrl = (string)rdr["FilmwebUrl"]
                        };
                        films.Add(film);
                    }
                }
            }

            return films;
        }

        public string GetListCreator(int listId)
        {
            string query = "select UserName from AspNetUsers where id=(select User_Id from List where Id=" + listId + ");";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        return (string)rdr["UserName"];
                    }
                }
            }

            return null;
        }

        public List<Film> SearchFilms(string searchString)
        {
            List<Film> films = new List<Film>();
            string query = "select * from Film where Name like '%" + searchString + "%';";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Film film = new Film()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            OriginalName = (string)rdr["OriginalName"],
                            ReleaseDate = (string)rdr["ReleaseDate"],
                            Genre = (string)rdr["Genre"],
                            Description = (string)rdr["Description"],
                            Cover = (string)rdr["Cover"],
                            FilmwebUrl = (string)rdr["FilmwebUrl"]
                        };
                        films.Add(film);
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("select top(30) * from Film where Genre like '%" + searchString + "%'", con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Film film = new Film()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            OriginalName = (string)rdr["OriginalName"],
                            ReleaseDate = (string)rdr["ReleaseDate"],
                            Genre = (string)rdr["Genre"],
                            Description = (string)rdr["Description"],
                            Cover = (string)rdr["Cover"],
                            FilmwebUrl = (string)rdr["FilmwebUrl"]
                        };
                        films.Add(film);
                    }
                }
            }

            return films;
        }

        public List<List> SearchLists(string searchString)
        {
            List<List> lists = new List<List>();
            string[] words = searchString.Split(' ');
            List<string> searchWords = new List<string>();
            // remove single letter words
            foreach (var word in words)
            {
                if (word.Length > 1)
                    searchWords.Add(word);
            }
            string likeQuery = "";
            foreach (var word in searchWords)
            {
                likeQuery += "or Name like N'%" + word + "%' ";
            }

            // delete first 'or'
            int index = likeQuery.IndexOf("or");
            string likeCondition = (index < 0)
                ? likeQuery
                : likeQuery.Remove(index, 2);

            string query = "select * from List where " + likeCondition;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        List list = new List()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            Likes = (int)rdr["Likes"],
                            Dislikes = (int)rdr["Dislikes"]
                        };

                        lists.Add(list);
                    }
                }
            }

            return lists;
        }

        public int Insert(List<int> films, string name, string userId)
        {
            int listId = 0;
            int rowsAffected = 0;
            string query = @"insert into List(Name, User_Id, Likes, Dislikes) values (@name, @userId, 0, 0);";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@userId", userId);
                rowsAffected = cmd.ExecuteNonQuery();
            }

            if (rowsAffected > 0)
            {
                // get created list id
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select top(1) Id from List order by Id desc;", con);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            listId = (int)rdr["Id"];
                        }
                    }
                }

                string sql = "insert into ListFilm(List_Id, Film_Id) values ";
                foreach (var filmId in films)
                {
                    sql += "(" + listId + "," + filmId + "),";
                }
                // change last comma to ;
                sql = sql.Remove(sql.Length - 1, 1) + ";";

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    rowsAffected = cmd.ExecuteNonQuery();
                }

                if (rowsAffected == 0)
                    return 0;

                return listId;
            }

            return 0;
        }

        public bool Delete(int id)
        {
            int rowsAffected = 0;

            string deleteUserFavourite = "delete from UserFavourite where ListId=" + id;
            string deleteListFilm = "delete from ListFilm where List_Id=" + id;
            string deleteList = "delete from List where id=" + id;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand deleteUF = new SqlCommand(deleteUserFavourite, con);
                deleteUF.ExecuteNonQuery();

                SqlCommand deleteListFilmCmd = new SqlCommand(deleteListFilm, con);
                rowsAffected = deleteListFilmCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    SqlCommand deleteListCmd = new SqlCommand(deleteList, con);
                    rowsAffected = deleteListCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return true;
                }
            }
            return false;
        }

        public bool UpdateList(int listId, string name, List<int> filmIds)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                if (filmIds != null)
                {
                    //get previous state of list
                    //list films ids
                    List<int> ids = new List<int>();
                    string query = "select Film_Id from ListFilm where List_Id=" + listId;

                    SqlCommand cmd = new SqlCommand(query, con);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ids.Add((int)rdr["Film_Id"]);
                        }
                    }

                    #region films to remove
                    List<int> filmsToDelete = new List<int>();
                    foreach (var id in ids)
                    {
                        if (!filmIds.Contains(id))
                            filmsToDelete.Add(id);
                    }
                    if (filmsToDelete.Count > 0)
                    {
                        string deleteQuery = "";
                        foreach (var id in filmsToDelete)
                        {
                            deleteQuery += id;
                            deleteQuery += ",";
                        }
                        //remove last comma
                        deleteQuery = deleteQuery.Remove(deleteQuery.Length - 1);
                        SqlCommand deleteListVoteCmd = new SqlCommand("delete from FilmVote where List_Id=@listId and Film_Id IN (" + deleteQuery + ")", con);
                        deleteListVoteCmd.Parameters.AddWithValue("@listId", listId);
                        deleteListVoteCmd.ExecuteNonQuery();

                        SqlCommand deleteListFilmCmd = new SqlCommand("delete from ListFilm where List_Id=" + listId + " and Film_Id IN (" + deleteQuery + ")", con);
                        deleteListFilmCmd.Parameters.AddWithValue("@listId", listId);
                        deleteListFilmCmd.ExecuteNonQuery();

                    }
                    #endregion

                    #region films to add
                    List<int> filmsToAdd = new List<int>();
                    foreach (var id in filmIds)
                    {
                        if (!ids.Contains(id))
                            filmsToAdd.Add(id);
                    }

                    if (filmsToAdd.Count > 0)
                    {
                        string addQuery = "";
                        foreach (var id in filmsToAdd)
                        {
                            addQuery += "(" + listId + "," + id + "),";
                        }
                        //remove last comma
                        addQuery = addQuery.Remove(addQuery.Length - 1);
                        SqlCommand insertListFilmCmd = new SqlCommand("insert into ListFilm(List_Id, Film_Id) values " + addQuery + ";", con);
                        insertListFilmCmd.ExecuteNonQuery();

                    }
                    #endregion
                }

                #region update name
                if (name != "")
                {
                    SqlCommand updateNameCmd = new SqlCommand("update List set Name=@name where Id=@listId", con);
                    updateNameCmd.Parameters.AddWithValue("@name", name);
                    updateNameCmd.Parameters.AddWithValue("@listId", listId);
                    int result = updateNameCmd.ExecuteNonQuery();
                    if (result == 1)
                        return true;
                    else
                        return false;

                }
                #endregion
            }
            return true;
        }

        public void UpdateRate(int listId, string userName, int rate)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string userId = "";
                var query = "select Id from AspNetUsers where UserName=@userName";
                SqlCommand userIdCmd = new SqlCommand(query, con);
                userIdCmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = userIdCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userId = (string)rdr["Id"];
                    }
                }

                SqlCommand cmd = new SqlCommand("update Rating set Rate=@rate where List_Id=@listId and User_Id=@userId", con);
                cmd.Parameters.AddWithValue("@rate", rate);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();
            }
        }

        public bool DeleteRating(int listId, string userName)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string userId = "";
                var query = "select Id from AspNetUsers where UserName=@userName";
                SqlCommand userIdCmd = new SqlCommand(query, con);
                userIdCmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = userIdCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userId = (string)rdr["Id"];
                    }
                }

                SqlCommand cmd = new SqlCommand("Delete from Rating where List_Id=@listId and User_Id=@userId", con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                int result = cmd.ExecuteNonQuery();
                if (result == 1)
                    return true;
                return false;
            }
        }

        public int GetLikesCount(int listId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // get likes count
                SqlCommand likesCountCmd = new SqlCommand("select Count(*) as Count from Rating where List_Id=@listId and Rate=1", con);
                likesCountCmd.Parameters.AddWithValue("@listId", listId);
                using (SqlDataReader rdr = likesCountCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        return (int)rdr["Count"];
                    };
                }
            }
            return 0;
        }
        public int GetDislikesCount(int listId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // get likes count
                SqlCommand likesCountCmd = new SqlCommand("select Count(*) as Count from Rating where List_Id=@listId and Rate=-1", con);
                likesCountCmd.Parameters.AddWithValue("@listId", listId);
                using (SqlDataReader rdr = likesCountCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        return (int)rdr["Count"];
                    };
                }
            }
            return 0;
        }

        public void UpdateLikesDislikesCount(int listId, bool like, bool dislike, bool all)
        {
            int likesCount = 0;
            int dislikesCount = 0;

            // get list likes and dislikes 
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // get likes count
                SqlCommand likesCountCmd = new SqlCommand("select Count(*) as Count from Rating where List_Id=@listId and Rate=1", con);
                likesCountCmd.Parameters.AddWithValue("@listId", listId);
                using (SqlDataReader rdr = likesCountCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        likesCount = (int)rdr["Count"];
                    };
                }

                // get dislikes count
                SqlCommand dislikesCountCmd = new SqlCommand("select Count(*) as Count from Rating where List_Id=@listId and Rate=-1", con);
                dislikesCountCmd.Parameters.AddWithValue("@listId", listId);
                using (SqlDataReader rdr = dislikesCountCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        dislikesCount = (int)rdr["Count"];
                    };
                }

                if (like)
                {
                    SqlCommand updateLikesCmd = new SqlCommand("update List set Likes=@likes where Id=@listId", con);
                    updateLikesCmd.Parameters.AddWithValue("@listId", listId);
                    updateLikesCmd.Parameters.AddWithValue("@likes", likesCount);
                    updateLikesCmd.ExecuteNonQuery();
                }
                else if (dislike)
                {
                    SqlCommand updateDislikesCmd = new SqlCommand("update List set Dislikes=@dislikes where Id=@listId", con);
                    updateDislikesCmd.Parameters.AddWithValue("@listId", listId);
                    updateDislikesCmd.Parameters.AddWithValue("@dislikes", dislikesCount);
                    updateDislikesCmd.ExecuteNonQuery();
                }
                else if (all)
                {
                    SqlCommand updateCmd = new SqlCommand("update List set Likes=@likes, Dislikes=@dislikes where Id=@listId", con);
                    updateCmd.Parameters.AddWithValue("@listId", listId);
                    updateCmd.Parameters.AddWithValue("@dislikes", dislikesCount);
                    updateCmd.Parameters.AddWithValue("@likes", likesCount);
                    updateCmd.ExecuteNonQuery();
                }
            }
        }

        public Rating GetRating(int listId, string userName)
        {
            var result = new Rating();
            result.User = new ApplicationUser();
            result.List = new List();
            string userId = "";
            var query = "select Id from AspNetUsers where UserName=@userName";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand userIdCmd = new SqlCommand(query, con);
                userIdCmd.Parameters.AddWithValue("@userName", userName);
                using (SqlDataReader rdr = userIdCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        userId = (string)rdr["Id"];
                    }
                }

                SqlCommand cmd = new SqlCommand("select List_Id, User_Id, Rate from Rating where List_Id=@listId and User_Id=@userId", con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        result.User.Id = (string)rdr["User_Id"];
                        result.User.UserName = userName;
                        result.List.Id = (int)rdr["List_Id"];
                        result.Rate = (int)rdr["Rate"];
                    }
                }
            }

            return result;
        }

        public bool CheckIfListFilmVoteExists(int listId, int filmId, string userId)
        {
            int exists = 0;
            var query = "select Id from FilmVote where List_Id=" + listId + " and Film_Id=" + filmId + " and User_Id='" + userId + "';";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        exists = (int)rdr["Id"];
                    }
                }
            }

            if (exists != 0)
                return true;

            return false;
        }

        public void AddRating(int listId, int rate, string userId)
        {
            int result = 0;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into Rating(Rate, List_id, User_id) values (@rate, @listId, @userId)", con);
                cmd.Parameters.AddWithValue("@rate", rate);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                result = cmd.ExecuteNonQuery();
            }

            if (result > 0)
            {
                //like added
                if (rate == 1)
                {
                    UpdateLikesDislikesCount(listId, true, false, false);
                }
                //dislike added
                else if (rate == -1)
                {
                    UpdateLikesDislikesCount(listId, false, true, false);
                }
            }
        }

        public bool AddFilmVote(int listId, int filmId, string userId)
        {
            var query = "insert into FilmVote(List_id, Film_Id, User_id) values (@listId, @filmId, @userId)";
            int rowsAffected = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@filmId", filmId);
                cmd.Parameters.AddWithValue("@userId", userId);
                rowsAffected = cmd.ExecuteNonQuery();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }

        public bool DeleteFilmVote(int listId, int filmId, string userId)
        {
            int rowsAffected = 0;
            var query = "delete from FilmVote where List_Id=@listId and Film_Id=@filmId and User_Id=@userId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@filmId", filmId);
                cmd.Parameters.AddWithValue("@userId", userId);
                rowsAffected = cmd.ExecuteNonQuery();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }

        public Dictionary<int, int> CalculateVotes(int listId, List<int> filmIds)
        {
            string query = "select Film_Id, Count(Film_Id) VotesCount from FilmVote where List_Id=" + listId + " group by Film_Id";
            Dictionary<int, int> filmsVotes = new Dictionary<int, int>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        filmsVotes.Add((int)rdr["Film_Id"], (int)rdr["VotesCount"]);
                    };
                }
            }

            return filmsVotes;
        }

        public List<int> CheckIfFilmsAreVoted(int listId, string userId)
        {
            string query = "select Film_Id from FilmVote where List_Id=" + listId + " and User_Id='" + userId + "';";
            List<int> filmsVoted = new List<int>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        filmsVoted.Add((int)rdr["Film_Id"]);
                    };
                }
            }

            return filmsVoted;
        }

        public int CheckIfListIsVoted(int listId, string userId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("select Rate from Rating where List_Id=@listId and User_Id=@userId", con);
                cmd.Parameters.AddWithValue("@listId", listId);
                cmd.Parameters.AddWithValue("@userId", userId);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        return (int)rdr["Rate"];
                    }
                }
            }

            return 0;
        }

        public List<string> GetAllTitles()
        {
            List<string> names = new List<string>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand filmsCmd = new SqlCommand("select Name from Film", con);
                using (SqlDataReader rdr = filmsCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        names.Add((string)rdr["Name"]);
                    };
                }

                SqlCommand listsCmd = new SqlCommand("select Name from List", con);
                using (SqlDataReader rdr = listsCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        names.Add((string)rdr["Name"]);
                    };
                }
            }

            return names;
        }

        public List<List> GetListsTitles()
        {
            List<List> listsIdNames = new List<List>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand listsCmd = new SqlCommand("select Id, Name from List", con);
                using (SqlDataReader rdr = listsCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        listsIdNames.Add(new List() { Id = (int)rdr["Id"], Name = (string)rdr["Name"] });
                    };
                }
            }

            return listsIdNames;
        }
    }
}
