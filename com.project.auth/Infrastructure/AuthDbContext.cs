using com.project.auth.BuildingBlocks.Core;
using com.project.auth.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace com.project.auth.Infrastructure;

public class AuthDbContext: IdentityDbContext<User>, IUnitOfWork
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
        
    }
}