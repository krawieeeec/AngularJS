using System.Collections.Generic;
using TO.List;

namespace Service
{
    public interface IListServices
    {
        ListDescriptionTO GetListById(string userName, int listId);
        IEnumerable<ListDescriptionTO> GetTopLists(string userName);
        IEnumerable<ListDescriptionTO> GetAllLists(string userName);
        SearchTO Search(string searchString, string userName);
        bool CheckIfListExists(int listId);
        RatingShortDescriptionTO GetRating(int listId, string userName);
        string GetListCreator(int listId);
        bool CheckIfListFilmRatingExists(int listId, int filmId, string userId);
        int CreateList(ListCreateTO listCreate);
        bool DeleteList(int listId);
        bool UpdateList(int listId, ListUpdateTO listUpdate);
        bool UpdateRate(RatingShortDescriptionTO rating);
        bool DeleteRating(RatingShortDescriptionTO rating);
        bool AddRating(int listId, int rate, string userId);
        bool AddFilmVote(int listId, int filmId, string userId);
        bool DeleteFilmVote(int listId, int filmId, string userId);
        List<string> GetAllTitles();
        List<ListIdTitleTO> GetListsTitles();
        void InitializeListsCache();
    }
}
