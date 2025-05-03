using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("auctionApp", "Auction App Api"),
    ];

    public static IEnumerable<Client> Clients =>
    [
        new Client
        {
            ClientId = "postman",
            ClientName = "Postman",
            AllowedScopes = {"openid", "profile", "auctionApp"},
            RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
            ClientSecrets = [new Secret("NotASecret".Sha256())],
            AllowedGrantTypes =  GrantTypes.ResourceOwnerPassword 
        }
    ];
}