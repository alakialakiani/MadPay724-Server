﻿using System;
using System.Collections.Generic;
using System.Text;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Models;
using MadPay724.Repo.Infrastructure;
using MadPay724.Repo.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace MadPay724.Repo.Repositories.Repo
{
  public  class TokenRepository : Repository<Token>, ITokenRepository
    {
      private readonly DbContext _db;
      public TokenRepository(DbContext dbContext) : base(dbContext)
      {
          _db ??= (MadpayDbContext)_db;
      }
    }
}