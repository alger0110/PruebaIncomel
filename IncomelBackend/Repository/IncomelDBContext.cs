using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class IncomelDBContext: DbContext
    {
        public IncomelDBContext(DbContextOptions<IncomelDBContext> options) : base(options)
        {

        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<PasswordToken> PasswordTokens { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Wages> Wages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(x => new
            {
                x.UserId,
                x.RoleId
            });

            modelBuilder.Entity<UserRoleOption>().HasKey(x => new
            {
                x.UserId,
                x.RoleId,
                x.OptionId
            });

        }
    }
}
