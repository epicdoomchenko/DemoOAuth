using System.Security.Claims;
using AuthorizationServer.Models;
using IdentityModel;

namespace AuthorizationServerDbSeeder;

internal static class SeedData
{
    internal static IEnumerable<(ApplicationUser, Claim[])> GetData()
    {
        yield return (new ApplicationUser
        {
            UserName = "alice",
            Email = "AliceSmith@email.com",
            EmailConfirmed = true
        }, [
            new Claim(JwtClaimTypes.Name, "Alice Smith"),
            new Claim(JwtClaimTypes.GivenName, "Alice"),
            new Claim(JwtClaimTypes.FamilyName, "Smith"),
            new Claim("position", "Analysis"),
            new Claim("team", "Team 1"),
            new Claim("location", "LA"),
            new Claim(JwtClaimTypes.WebSite, "http://alice.com")
        ]);
        yield return (new ApplicationUser
        {
            UserName = "bob",
            Email = "BobSmith@email.com",
            EmailConfirmed = true
        }, [
            new Claim(JwtClaimTypes.Name, "Bob Smith"),
            new Claim(JwtClaimTypes.GivenName, "Bob"),
            new Claim(JwtClaimTypes.FamilyName, "Smith"),
            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
            new Claim("position", "manager"),
            new Claim("team", "Team 1"),
            new Claim("location", "Unknown")
        ]);
        yield return (new ApplicationUser
        {
            UserName = "samebody",
            Email = "same@bo.dy",
            EmailConfirmed = true
        }, [
            new Claim(JwtClaimTypes.Name, "Sam Body"),
            new Claim(JwtClaimTypes.GivenName, "Sam"),
            new Claim(JwtClaimTypes.FamilyName, "Body"),
            new Claim(JwtClaimTypes.WebSite, "http://samb.com"),
            new Claim("position", "Developer"),
            new Claim("team", "Team 2"),
            new Claim("location", "Rio")
        ]);
    }
}