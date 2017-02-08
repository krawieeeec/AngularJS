using DataModel.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataModel.Repository
{
    public class FilmRepository : IFilmRepository
    {
        string _connectionString;

        public FilmRepository(bool isTest)
        {
            if (isTest)
                _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=DB_A15604_sportoweswiry;Integrated Security=True;";
            else
                _connectionString = @"Data Source=SQL5025.SmarterASP.NET;Initial Catalog=DB_A15604_sportoweswiry;User Id=DB_A15604_sportoweswiry_admin;Password=haslo123;";
        }

        public IEnumerable<Film> GetAll()
        {
            List<Film> films = new List<Film>();
            string query = "select * from Film;";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        films.Add(new Film()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            OriginalName = (string)rdr["OriginalName"],
                            ReleaseDate = (string)rdr["ReleaseDate"],
                            Genre = (string)rdr["Genre"],
                            Description = (string)rdr["Description"],
                            Cover = (string)rdr["Cover"],
                            FilmwebUrl = (string)rdr["FilmwebUrl"]
                        });
                    }
                }
            }

            return films;
        }

        public List<Film> GetMany(List<int> ids)
        {
            List<Film> films = new List<Film>();
            string filmIds = "";
            foreach (var id in ids)
            {
                filmIds += id;
                filmIds += ",";
            }
            filmIds = filmIds.Remove(filmIds.Length - 1);
            List<List> lists = new List<List>();

            string query = "select * from Film where id IN (" + filmIds + ");";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        films.Add(new Film()
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"],
                            OriginalName = (string)rdr["OriginalName"],
                            ReleaseDate = (string)rdr["ReleaseDate"],
                            Genre = (string)rdr["Genre"],
                            Description = (string)rdr["Description"],
                            Cover = (string)rdr["Cover"],
                            FilmwebUrl = (string)rdr["FilmwebUrl"]
                        });
                    }
                }
            }

            return films;
        }

        public Film GetSingleFilm(int filmId)
        {
            Film film = new Film();
            string query = "select * from Film where Id=" + filmId;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        film.Id = (int)rdr["Id"];
                        film.Name = (string)rdr["Name"];
                        film.OriginalName = (string)rdr["OriginalName"];
                        film.Genre = (string)rdr["Genre"];
                        film.Description = (string)rdr["Description"];
                        film.Cover = (string)rdr["Cover"];
                        film.FilmwebUrl = (string)rdr["FilmwebUrl"];
                    }
                }
            }

            return film;
        }

        public ICollection<Actor> GetFilmActors(int filmId)
        {
            List<Actor> actors = new List<Actor>();
            string query = "select * from Actor where Film_Id=" + filmId + ";";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        actors.Add(new Actor
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"]
                        });
                    }
                }
            }

            return actors;
        }

        public List<int> GetActorFilmIds(string name)
        {
            List<int> filmIds = new List<int>();
            string query = "select Film_Id from Actor where Name='" + name + "'";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        filmIds.Add((int)rdr["Film_Id"]);
                    }
                }
            }

            return filmIds;
        }

        public ICollection<Director> GetFilmDirectors(int filmId)
        {
            List<Director> directors = new List<Director>();
            string query = "select * from Director where Film_Id=" + filmId + ";";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        directors.Add(new Director
                        {
                            Id = (int)rdr["Id"],
                            Name = (string)rdr["Name"]
                        });
                    }
                }
            }

            return directors;
        }

        public bool CheckIfFilmExists(int filmId)
        {
            string query = "select Id from Film where id=" + filmId + ";";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void UpdateFilm(Film film)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                SqlCommand updateCmd = new SqlCommand("update Film set Name=@name, OriginalName=@originalName," +
                    " ReleaseDate=@releaseDate, Genre=@genre, Description=@description, Cover=@cover, FilmwebUrl=@filmwebUrl where Id=@id", con);

                updateCmd.Parameters.AddWithValue("@id", film.Id);
                updateCmd.Parameters.AddWithValue("@name", film.Name);
                updateCmd.Parameters.AddWithValue("@originalName", film.OriginalName);
                updateCmd.Parameters.AddWithValue("@releaseDate", film.ReleaseDate);
                updateCmd.Parameters.AddWithValue("@genre", film.Genre);
                updateCmd.Parameters.AddWithValue("@description", film.Description);
                updateCmd.Parameters.AddWithValue("@cover", film.Cover);
                updateCmd.Parameters.AddWithValue("@filmwebUrl", film.FilmwebUrl);
                updateCmd.ExecuteNonQuery();
            }
        }
    }
}
