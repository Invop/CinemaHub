namespace Identity.Api.Models.AccountViewModels
{
    public class LogoutInputModel
    {
        public string LogoutId { get; set; }
    }
    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; } = true;
    }

}
