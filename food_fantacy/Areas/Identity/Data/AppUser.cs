using Microsoft.AspNetCore.Identity;

namespace food_fantacy.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser
{
    [PersonalData]
    public required string Name { get; set; }

    [PersonalData]
    public string ProfilePicture { get; set; } = string.Empty;
}

