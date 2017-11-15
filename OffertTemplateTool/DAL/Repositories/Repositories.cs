using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OffertTemplateTool.DAL.Models;
using OffertTemplateTool.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace OffertTemplateTool.DAL.Repositories
{
    public class UsersRepository : Repository<Users>
    {
        public UsersRepository(DataBaseContext databasecontext) : base(databasecontext)
        {
            
        }
        public bool AnyUserByEmail(string key)
        {
            var users = GetAll();
            var result = users.Any(x => x.Email == key);
            return result;
        }

        public async Task<bool> AnyUserByEmailAsync(string key)
        {
            var users = await GetAllAsync();
            var result = users.Any(x => x.Email == key);
            return result;
        }

        public Users FindUserByEmail(string email)
        {
            var users = GetAll();
            var result = users.FirstOrDefault(x => x.Email == email);
            return result;
        }
    }
    public class SettingsRepository : Repository<Settings>
    {
        public SettingsRepository(DataBaseContext databasecontext) : base(databasecontext)
        {

        }
        public Settings getBTW()
        {
            var settings = GetAll();
            try
            {
                Settings btw = settings.FirstOrDefault(x => x.Key == "btw");
                return btw;
            }
            catch
            {
                return null;
            }
        }
    }
    public class OfferRepository : Repository<Offers>
    {
        public OfferRepository(DataBaseContext databasecontext) : base(databasecontext)
        {
            
        }
    }

    public class EstimateLinesRepository : Repository<EstimateLines>
    {
        public EstimateLinesRepository(DataBaseContext databasecontext) : base(databasecontext)
        {

        }
        public bool AnyLineExist(System.Guid id)
        {
            var lines = GetAll();
            return lines.Any(x => x.Id.Equals(id));
        }
    }

    public class EstimateRepository : Repository<Estimates>
    {
        public EstimateRepository(DataBaseContext databasecontext) : base(databasecontext)
        {
            
        }
    }
    public class EstimateConnectsRepository : Repository<EstimateConnects>
    {
        public EstimateConnectsRepository(DataBaseContext databasecontext) : base(databasecontext)
        {

        }

        public List<EstimateConnects> SelectEstimateLines(System.Guid Id)
        {
            var lines = GetAll();
            var estimatelines = lines.Where(x => x.Estimate.Id == Id).ToList();
            return estimatelines;
        }

    }
}
