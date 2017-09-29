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
        public bool FindUserByEmail(string key)
        {
            var users = GetAll();
            var result = users.Any(x => x.Email == key);
            return result;
        }

        public async Task<bool> FindUserByEmailAsync(string key)
        {
            var users = await GetAllAsync();
            var result = users.Any(x => x.Email == key);
            return result;
        }
    }
    public class SettingsRepository : Repository<Settings>
    {
        public SettingsRepository(DataBaseContext databasecontext) : base(databasecontext)
        {

        }
    }
    public class OfferRepository : Repository<Offer>
    {
        public OfferRepository(DataBaseContext databasecontext) : base(databasecontext)
        {
            
        }
    }
}
