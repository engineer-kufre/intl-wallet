using IntlWallet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntlWallet.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<UserMainCurrencyDetail> UserMainCurrencyDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Wallet>()
                        .Property(i => i.Balance)
                        .HasColumnType("money");

            modelBuilder.Entity<Transaction>()
                        .Property(i => i.Amount)
                        .HasColumnType("money");
        }
    }
}
