using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace main_datacontext;

public class MainDbContext : IdentityDbContext<Identity.User, Identity.Role, long>
{
    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    protected MainDbContext()
    {
        
    }

    public DbSet<Emails.DbEmail> Emails { get; set; }
}
