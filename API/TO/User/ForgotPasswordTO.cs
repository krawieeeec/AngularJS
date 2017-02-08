using System.ComponentModel.DataAnnotations;
namespace TO.User
{
    public class ForgotPasswordTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
