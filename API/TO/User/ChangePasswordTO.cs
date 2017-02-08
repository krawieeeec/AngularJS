namespace TO.User
{
    public class ChangePasswordTO
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
