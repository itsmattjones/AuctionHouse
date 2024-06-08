using Microsoft.AspNetCore.Identity;
using System;

namespace AuctionHouse.Domain.Entities;

public class User : IdentityUser
{
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }

    public string ProfileImageUrl { get; set; }
}
