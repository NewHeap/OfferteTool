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

        public static string ConnectionString { get; set; }

        public DbSet<Users> Users { get; set; }
        public DbSet<Offers> Offer { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<EstimateLines> EstimateLines { get; set; }
        public DbSet<Estimates> Estimates { get; set; }
        public DbSet<EstimateConnects> EstimateConnects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(ConnectionString);
           
        }




    }
}

