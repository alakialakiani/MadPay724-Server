﻿
using System.Linq;
using System.Threading.Tasks;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MadPay724.Repo.Repositories.Repo
{
   public class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        private readonly DbContext _db;
        public DocumentRepository(DbContext dbContext) : base(dbContext)
        {
            _db ??= (MadpayDbContext)_db;
        }

        public async Task<int> DocumentCountAsync(string userId)
        {
            return (await GetManyAsync(p => p.Id == userId, null, "")).Count();
        }
    }
}
