using System.Collections.Generic;
using System.Threading.Tasks;
using TO.List;
using TO.User;

namespace Service
{
    public interface IAuthServices
    {
        Task<bool> RegisterUser(RegisterUserTO applicationUserEntity);
        Task<string> FindUser(string userName, string password);
        Task<bool> CheckIfRoleExists(string role);
        Task<bool> CheckCredentials(string userName, string password);
        Task<bool> ConfirmEmail(string userId, string code);
        Task<bool> ForgotPassword(string userEmail);
        Task<bool> ResetPassword(string userId, string code);
        bool CheckIfFavouriteExists(int listId, string userName);
        bool AddListToFavourites(string userName, int listId);
        List<ListFavouriteTO> GetUserFavourites(string userName);
        bool RemoveUserFavourite(int listId, string userName);
        List<ListUserCreated> GetUserCreatedLists(string userName);
        bool ChangeEmail(ChangeEmailTO emailDetails);
        UserInfoTO GetUserInfo(string userName);
        string GetUserId(string userName);
        List<UserInfoTO> GetAllUsers();
        List<UserIdNameTO> GetAllUsersNames();
        bool AssignRolesToUser(AddRoleToUserTO rolesToAssign);
        List<string> GetUserRoles(string userName);
        List<string> GetUserRolesByUserId(string userId);
        string GetUserEmail(string userName);
        Task<bool> ChangePassword(ChangePasswordTO passwordDetails);
        Task<bool> CheckIfUserExists(string userName);
        bool DeleteUser(string userName);
        bool CheckIfEmailExists(string email);
        void InitializeUsersCache();
    }
}
