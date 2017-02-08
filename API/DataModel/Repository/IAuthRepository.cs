using DataModel.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataModel.Repository
{
    public interface IAuthRepository
    {
        Task<string> RegisterUser(ApplicationUser applicationUser, string password);
        Task<bool> ConfirmEmail(string userId, string code);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(string userId, string code);
        bool ChangeEmail(string oldEmail, string newEmail);
        Task<bool> ChangePassword(string userName, string currentPassword, string newPassword);
        bool DeleteUser(string userName);
        bool CheckIfFavouriteExists(int listId, string userId);
        bool AddListToFavourites(string userName, int listId);
        List<int> GetUserFavourites(string userName);
        bool RemoveUserFavourite(int listId, string userName);
        List<List> GetUserCreatedLists(string userName);
        bool AssignRolesToUser(string userName, List<string> rolesToAssign);
        List<string> GetUserRoles(string userName);
        List<string> GetUserRolesByUserId(string userId);
        string GetUserEmail(string userName);
        List<ApplicationUser> GetAllUsers();
        ApplicationUser FindUserByUserName(string userName);
        string GetUserId(string userName);
        Task<IdentityUser> FindUser(string userName, string password);
        Task<bool> CheckCredentials(string userName, string password);
        Task<bool> CheckIfRoleExists(string role);
        Task<bool> CheckIfUserExists(string userName);
        bool CheckIfEmailExists(string email);
        void SendEmail(string subject, string emailBody, string email);
    }
}
