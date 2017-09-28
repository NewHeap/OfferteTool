using Microsoft.EntityFrameworkCore;
using OffertTemplateTool.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OffertTemplateTool.DAL.Context
{
    public class DataBaseContext : DbContext
    {
        DbContextOptions<DataBaseContext> optie;

        public static string ConnectionString { get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<CompanyInfo> CompanyInfo { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }




    }
}

