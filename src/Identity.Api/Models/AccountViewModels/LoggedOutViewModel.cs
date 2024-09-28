﻿namespace Identity.Api.Models.AccountViewModels
{
    public record LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; init; }
        public string ClientName { get; init; }
        public string SignOutIframeUrl { get; init; }
        
        public bool AutomaticRedirectAfterSignOut { get; set; }

        public string LogoutId { get; set; }
    }
}
