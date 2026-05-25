using System;
using Microsoft.AspNetCore.Identity;

namespace main_datacontext.Identity;

public class User: IdentityUser<long>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
