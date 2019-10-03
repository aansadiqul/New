using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using ABD.ADOrders.Dto;
using ABD.ADSearches.Dto;
using ABD.Authorization;
using ABD.Customers.Dto;
using ABD.Domain.Dtos;
using ABD.Domain.Repositories;
using ABD.Entities;
using ABD.EntityFrameworkCore;
using ABD.QueryBuilder;
using Microsoft.EntityFrameworkCore;
using Abp.Net.Mail;
using ABD.Authorization.Users;
using ABD.Hangfire;
using ABD.Hangfire.Dto;
using ABD.EmailBuilder;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using static Abp.Net.Mail.EmailSettingNames;

namespace ABD.ADOrders
{
    [AbpAuthorize]
    public class ADOrderAppService : ABDAppServiceBase, IADOrderAppService
    {
        private readonly IDbContextProvider<ABDDbContext> _dbContextProvider;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IHostingEnvironment _env;
        private readonly IConverter _converter;
        private readonly IConfiguration _iConfig;

        private ABDDbContext Context => _dbContextProvider.GetDbContext();
        private readonly IRepository<ADOrder> _adOrderRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IAbpSession _session;
        private readonly ICommonRepository _commonRepository;
        private readonly IRepository<User, long> _userRepository;


        public ADOrderAppService(IRepository<ADOrder> adOrderRepository,
                                IRepository<UserProfile> userProfileRepository,
                                IAbpSession session,                                
                                ICommonRepository commonRepository,
                                IDbContextProvider<ABDDbContext> dbContextProvider,
                                IBackgroundJobManager backgroundJobManager,
                                IRepository<User, long> userRepository,
                                IHostingEnvironment env,
                                IConverter converter,
                                IConfiguration iConfig)
        {
            _adOrderRepository = adOrderRepository;
            _userProfileRepository = userProfileRepository;
            _session = session;
            _commonRepository = commonRepository;
            _dbContextProvider = dbContextProvider;
            _backgroundJobManager = backgroundJobManager;
            _userRepository = userRepository;
            _env = env;
            _converter = converter;
            _iConfig = iConfig;
        }

        public async Task<PagedResultDto<ADOrderDto>> GetAll(GetOrderInput input)
        {
            var userQuery = CreateFilteredQuery(input);
            var userQueryCount = await userQuery.CountAsync();
            var userQueries = await userQuery.OrderByDescending(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<ADOrderDto>(
                userQueryCount,
                ObjectMapper.Map<List<ADOrderDto>>(userQueries)
            );
        }

        [AbpAuthorize(PermissionNames.AD, PermissionNames.Pages_Admin)]
        public async Task<AdReceiptDto> GetReceipt(int orderId, int paymentId)
        {
            ADOrderDto userOrder;
            CustomerDto customer;
            if (paymentId > 0)
            {
                userOrder = ObjectMapper.Map<ADOrderDto>(await _adOrderRepository.GetAll().Where(x => x.TransactionID == paymentId.ToString()).FirstOrDefaultAsync());
                customer = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == userOrder.CreatorUserId).FirstOrDefaultAsync());
            }
            else
            {
                userOrder = ObjectMapper.Map<ADOrderDto>(await _adOrderRepository.GetAsync(orderId));
                customer = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == _session.UserId).FirstOrDefaultAsync());
                if (userOrder.CreatorUserId != _session.UserId)
                {
                    throw new UserFriendlyException("Invalid order id!");
                }
            }
         
            var neilson = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == 426).FirstOrDefaultAsync());
            var adReceiptDto = new AdReceiptDto
            {
                Neilson = neilson,
                Customer = customer,
                Order = userOrder
            };
            return adReceiptDto;
        }

        public async Task Create(ADOrderInput input)
        {
            var orderId = await _adOrderRepository.InsertAndGetIdAsync(ObjectMapper.Map<ADOrder>(input));
            var customer = await _userRepository.GetAsync((long) _session.UserId);
            var receipt = GetReceipt(orderId, 0);
            await GeneratePdfAsync(receipt.Result);
            var adEmailInput = new AdOrderMailInput
            {
                OrderId = orderId,
                Description = input.Description,
                OrderDate = DateTime.Now,
                SalesUser = input.SalesUser,
                RecordCount = input.RecordCount,
                CtCount = input.CtCount,
                CustomerName = customer.FullName,
                PaymentType = input.TranType == 1 ? "Card" : "Check",
                CheckNo = input.CheckNo,
                CardNumber = input.CardNumber,
                OrderValue = input.OrderValue,
                TargetUser = "aan_jobaer@outlook.com" //customer.EmailAddress
            };
            await SendEmail(adEmailInput);
        }

        private Task GeneratePdfAsync(AdReceiptDto receipt)
        {
            return Task.Run(() => GeneratePdf(receipt));
        }

        private void GeneratePdf(AdReceiptDto receipt)
        {
            var webRootPath = _env.WebRootPath;
            var pathToFile = webRootPath
                             + Path.DirectorySeparatorChar
                             + "EmailTemplates"
                             + Path.DirectorySeparatorChar
                             + "AgencyDirectoryOrder.html";

            string body = string.Empty;
            using (StreamReader SourceReader = File.OpenText(pathToFile))
            {
                body = SourceReader.ReadToEnd();
            }

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = body
                    }
                }
            };
            var pdf = _converter.Convert(doc);
            FileStream fs = new FileStream(webRootPath
                                           + Path.DirectorySeparatorChar
                                           + "Orders"
                                           + Path.DirectorySeparatorChar
                                           + "ADReceipt_" + receipt.Order.Id + ".pdf", FileMode.OpenOrCreate);
            fs.Write(pdf, 0, pdf.Length);
            fs.Close();
        }

        [AbpAuthorize(PermissionNames.AD, PermissionNames.Pages_Admin)]
        public async Task<JSonResultDto> GetReportData(int orderId, ReportType reportType)
        {
            ADOrderDto adOrder = await GetOrderDetails(orderId);

            if (reportType == ReportType.ContactsReport)
            {
                if (!adOrder.IsCtPurchased)
                {
                    return new JSonResultDto(
                        OrderStatus.PurchaseRequired,
                        null);
                }
            }

            bool isExpired = IsDataExpired(adOrder.CreationTime);

            if (isExpired)
            {
                return new JSonResultDto(
                    OrderStatus.Expired,
                    null
                );
            }

            var query = GetQuery(adOrder, reportType);
            var data = await _commonRepository.GetData(query);
            return new JSonResultDto(
                OrderStatus.Valid,
                data
            );
        }

        [AbpAuthorize(PermissionNames.AD, PermissionNames.Pages_Admin)]
        public async Task<JSonResultDto> GetMapData(int orderId)
        {
            ADOrderDto adOrder = await GetOrderDetails(orderId);


            bool isExpired = IsDataExpired(adOrder.CreationTime);

            if (isExpired)
            {
                return new JSonResultDto(
                    OrderStatus.Expired,
                    null
                );
            }

            var mapQuery = ReportQueryBuilder.GetMapQuery(adOrder.QueryCriteria);
            var data = await _commonRepository.GetData(mapQuery);
            return new JSonResultDto(
                OrderStatus.Valid,
                data
            );
        }

        private string GetQuery(ADOrderDto adOrder, ReportType reportType)
        {
            if (reportType == ReportType.AgencyReport)
            {
                return ReportQueryBuilder.GetAgecyReportQuery(adOrder);
            }
            else if (reportType == ReportType.ContactsReport)
            {
                return ReportQueryBuilder.GetContactReportQuery(adOrder);
            }
            else
            {
                return ReportQueryBuilder.GetReportQuery(adOrder, reportType);
            }
        }

        private async Task<ADOrderDto> GetOrderDetails(int orderId)
        {
            var userOrder = await _adOrderRepository.GetAsync(orderId);
            if (userOrder.CreatorUserId != _session.UserId)
            {
                throw new UserFriendlyException("Invalid order id!");
            }
            var order = ObjectMapper.Map<ADOrderDto>(userOrder);
            return order;
        }

        private bool IsDataExpired(DateTime orderCreationTime)
        {
            var noOfDays = (DateTime.Today - orderCreationTime).TotalDays;
            if (noOfDays > 364)
            {
                return true;
            }
            return false;
        }

        [AbpAuthorize(PermissionNames.AD, PermissionNames.Pages_Admin)]
        public async Task<AccountDto> GetAccountDetails(string accountId)
        {
            var agency = await Context.Agencies.Where(x => x.AccountId == accountId).FirstOrDefaultAsync();
            var targetSectors = await Context.TargetSectors.Where(x => x.AccountId == accountId).ToListAsync();
            var affiliations = await Context.Affiliations.Where(x => x.AccountId == accountId).ToListAsync();
            var carriers = await Context.Carriers.Where(x => x.AccountId == accountId).ToListAsync();

            var accountDetails = new AccountDto
            {
                AgencyDetails = ObjectMapper.Map<AgencyDto>(agency),
                TargetSectors = ObjectMapper.Map<List<TargetSectorDto>>(targetSectors),
                SpecialAffiliations = ObjectMapper.Map<List<AffiliationDto>>(affiliations),
                Carriers = ObjectMapper.Map<List<CarrierDto>>(carriers)
            };
            return accountDetails;
        }

        private IQueryable<ADOrder> CreateFilteredQuery(GetOrderInput input)
        {
            return _adOrderRepository.GetAll().Where(x => x.CreatorUserId == _session.UserId)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Description.Contains(input.Keyword) || x.Id.ToString().Contains(input.Keyword));
        }

        public async Task SendEmail(AdOrderMailInput input)
        {
            string rootAddress = _iConfig.GetSection("App").GetSection("ServerRootAddress").Value;
            var webRootPath = _env.WebRootPath;
            var adOrderMail = EmailBuilder.EmailBuilder.GenerateAdOrderMail(input, webRootPath, rootAddress);
            await _backgroundJobManager.EnqueueAsync<SendEmailJob, SendEmailJobArgs>(adOrderMail);
        }
    }
}
