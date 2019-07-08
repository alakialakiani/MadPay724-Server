﻿using MadPay724.Data.Models;
using Microsoft.EntityFrameworkCore;
using ZNetCS.AspNetCore.Logging.EntityFrameworkCore;

namespace MadPay724.Data.DatabaseContext
{
    public class LogDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            optionBuilder.UseSqlServer(@"Data Source=KEY1-LAB\MSSQLSERVER2016;Initial Catalog=Logdb;Integrated Security=True;MultipleActiveResultSets=True;");
        }

        public DbSet<ExtendedLog> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            LogModelBuilderHelper.Build(modelBuilder.Entity<ExtendedLog>());

            modelBuilder.Entity<ExtendedLog>().ToTable("ExtendedLog");
        }
    }
}
