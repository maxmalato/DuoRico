using Microsoft.AspNetCore.Identity;

namespace DuoRico.Data;

public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public required string Name { get; set; }

    [PersonalData]
    public Guid? CoupleId { get; set; }
}
