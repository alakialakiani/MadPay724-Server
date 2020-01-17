﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using MadPay724.Common.Helpers.Interface;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Site.Panel.Common;
using MadPay724.Presentation.Helpers.Filters;
using MadPay724.Presentation.Routes.V1;
using MadPay724.Repo.Infrastructure;
using MadPay724.Services.Upload.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MadPay724.Presentation.Controllers.Site.V1.Common
{
    [ApiExplorerSettings(GroupName = "v1_Site_Panel_Common")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork<Main_MadPayDbContext> _db;
        private readonly IUnitOfWork<Financial_MadPayDbContext> _dbFinancial;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardController> _logger;
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IUtilities _utilities;

        public DashboardController(IUnitOfWork<Main_MadPayDbContext> dbContext, IMapper mapper,
            ILogger<DashboardController> logger, IUploadService uploadService,
            IWebHostEnvironment env, IUtilities utilities, IUnitOfWork<Financial_MadPayDbContext> dbFinancial)
        {
            _db = dbContext;
            _dbFinancial = dbFinancial;
            _mapper = mapper;
            _logger = logger;
            _uploadService = uploadService;
            _env = env;
            _utilities = utilities;
        }
        //------------------------------
        [Authorize(Policy = "RequireUserRole")]
        [ServiceFilter(typeof(UserCheckIdFilter))]
        [HttpGet(ApiV1Routes.Dashboard.GetUserDashboard)]
        public async Task<IActionResult> GetUserDashboard(string userId)
        {
            var res = new UserDashboardDto();

            res.UnClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => p.UserId == userId && !p.Closed);
            res.ClosedTicketCount = await _db.TicketRepository.GetCountAsync(p => p.UserId == userId && p.Closed);
            res.Last5Tickets = await _db.TicketRepository.GetManyAsync(p => p.UserId == userId, null, "TicketContents", 5);
            res.TotalInventory = await _db.WalletRepository.GetSumAsync(p=> p.UserId == userId , p => p.Inventory);
            res.Inventory5Days = new DaysForReturnDto
            {
                Day1 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = res.TotalInventory - await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.TotalInterMoney = await _db.WalletRepository.GetSumAsync(p => p.UserId == userId, p => p.InterMoney);
            res.InterMoney5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.Status, p => p.Price),
                Day2 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.Status, p => p.Price),
                Day3 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.Status, p => p.Price),
                Day4 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.Status, p => p.Price),
                Day5 = await _dbFinancial.FactorRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.Status, p => p.Price),
            };
            res.TotalExitMoney = await _db.WalletRepository.GetSumAsync(p => p.UserId == userId, p => p.ExitMoney);
            res.ExitMoney5Days = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.Date && p.IsPardakht, p => p.Price),
                Day2 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-1).Date && p.IsPardakht, p => p.Price),
                Day3 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-2).Date && p.IsPardakht, p => p.Price),
                Day4 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-3).Date && p.IsPardakht, p => p.Price),
                Day5 = await _dbFinancial.EntryRepository
                .GetSumAsync(p => p.UserId == userId && p.DateModified.Date == DateTime.Now.AddDays(-4).Date && p.IsPardakht, p => p.Price),
            };
            res.TotalSuccessFactor = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.Status, p => p.EndPrice);
            res.Factor12Months = new DaysForReturnDto
            {
                Day1 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.Year &&
                p.DateModified.Month == DateTime.Now.Month && p.Status, p => p.EndPrice),

                Day2 =await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-1).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-1).Month && p.Status, p => p.EndPrice),

                Day3 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-2).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-2).Month && p.Status, p => p.EndPrice),

                Day4 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-3).Year &&
                p.DateModified.Month == DateTime.Now.AddMonths(-3).Month && p.Status, p => p.EndPrice),

                Day5 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-4).Year 
                && p.DateModified.Month == DateTime.Now.AddMonths(-4).Month && p.Status, p => p.EndPrice),

                Day6 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-5).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-5).Month && p.Status, p => p.EndPrice),

                Day7 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-6).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-6).Month && p.Status, p => p.EndPrice),

                Day8 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-7).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-7).Month && p.Status, p => p.EndPrice),

                  Day9 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-8).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-8).Month && p.Status, p => p.EndPrice),

                    Day10 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-9).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-9).Month && p.Status, p => p.EndPrice),

                      Day11 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-10).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-10).Month && p.Status, p => p.EndPrice),

                        Day12 = await _dbFinancial.FactorRepository.GetSumAsync(p => p.UserId == userId && p.DateModified.Year == DateTime.Now.AddMonths(-11).Year
               && p.DateModified.Month == DateTime.Now.AddMonths(-11).Month && p.Status, p => p.EndPrice),
            };
            res.Last7Factors = await _dbFinancial.FactorRepository.GetManyAsync(p => p.UserId == userId, null, "", 7);
            res.TotalSuccessEntry = await _dbFinancial.EntryRepository.GetSumAsync(p => p.UserId == userId && p.IsPardakht, p => p.Price);
            res.Last10Entries = await _dbFinancial.EntryRepository.GetManyAsync(p => p.UserId == userId, null, "", 10);



            return Ok(res);
        }
    }
}