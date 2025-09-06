using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;
using System;

namespace BioLinker.Data
{
    public partial class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

    }
}
