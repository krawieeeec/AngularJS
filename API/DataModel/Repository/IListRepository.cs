using DataModel.Models;
using System.Collections.Generic;

namespace DataModel.Repository
{
    public interface IListRepository
    {
        List GetSingle(int listId);
        List<List> GetMany(IEnumerable<int> ids);
        List<List> GetAll();
        List<List> GetTopLists();
        List<Film> GetListFilms(int listId);
        string GetListCreator(int listId);
        List<Film> SearchFilms(string searchString);
        List<List> SearchLists(string searchString);
        int Insert(List<int> films, string name, string userId);
        bool Delete(int id);
        bool UpdateList(int listId, string name, List<int> filmIds);
        Rating GetRating(int listId, string username);
        bool CheckIfListFilmVoteExists(int listId, int filmId, string userId);
        bool AddFilmVote(int listId, int filmId, string userId);
        bool DeleteFilmVote(int listId, int filmId, string userId);
        Dictionary<int, int> CalculateVotes(int listId, List<int> filmIds);
        List<int> CheckIfFilmsAreVoted(int listId, string userId);
        void UpdateRate(int listId, string userName, int rate);
        void AddRating(int listId, int rate, string userId);
        bool DeleteRating(int listId, string userName);
        void UpdateLikesDislikesCount(int listId, bool like, bool dislike, bool all);
        int GetLikesCount(int listId);
        int GetDislikesCount(int listId);
        int CheckIfListIsVoted(int listId, string userId);
        List<string> GetAllTitles();
        List<List> GetListsTitles();
    }
}
