namespace AuctionHouse.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using System;

public class User : IdentityUser
{
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }

    public string ProfileImageUrl { get; set; }
}
