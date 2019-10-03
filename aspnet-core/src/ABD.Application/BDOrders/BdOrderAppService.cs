using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using ABD.ADOrders.Dto;
using ABD.ADSearches.Dto;
using ABD.Authorization;
using ABD.BDOrders;
using ABD.BDOrders.Dto;
using ABD.Customers.Dto;
using ABD.Domain.Repositories;
using ABD.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ABD.BDOrders
{
    [AbpAuthorize(PermissionNames.BD)]
    public class BdOrderAppService : ABDAppServiceBase, IBdOrderAppService
    {
        private readonly IRepository<BDOrder> _bdOrderRepository;
        private readonly IRepository<BdPurchasedData> _bdPurchaseRepository;
        private readonly IRepository<UserProfile> _userProfileRepository;
        private readonly IAbpSession _session;
        private readonly ICommonRepository _commonRepository;

        public BdOrderAppService(IRepository<BDOrder> bdOrderRepository,
                                IRepository<UserProfile> userProfileRepository,
                                IRepository<BdPurchasedData> bdPurchaseRepository,
                                IAbpSession session,
                                ICommonRepository commonRepository)
        {
            _bdOrderRepository = bdOrderRepository;
            _userProfileRepository = userProfileRepository;
            _session = session;
            _commonRepository = commonRepository;
            _bdPurchaseRepository = bdPurchaseRepository;
        }

        public async Task<PagedResultDto<BdOrderDto>> GetAll(GetOrderInput input)
        {
            var userQuery = CreateFilteredQuery(input);
            var userQueryCount = await userQuery.CountAsync();
            var userQueries = await userQuery.OrderByDescending(p => p.Id).PageBy(input).ToListAsync();

            return new PagedResultDto<BdOrderDto>(
                userQueryCount,
                ObjectMapper.Map<List<BdOrderDto>>(userQueries)
            );
        }

        public async Task<BdReceiptDto> GetReceipt(int orderId, int paymentId)
        {
            BdOrderDto userOrder;
            CustomerDto customer;
            if (paymentId > 0)
            {
                userOrder = ObjectMapper.Map<BdOrderDto>(await _bdOrderRepository.GetAll().Where(x => x.CCTransactionId == paymentId.ToString()).FirstOrDefaultAsync());
                customer = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == userOrder.CreatorUserId).FirstOrDefaultAsync());
            }
            else
            {
                userOrder = ObjectMapper.Map<BdOrderDto>(await _bdOrderRepository.GetAsync(orderId));
                customer = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == _session.UserId).FirstOrDefaultAsync());
                if (userOrder.CreatorUserId != _session.UserId)
                {
                    throw new UserFriendlyException("Invalid order id!");
                }
            }
            var neilson = ObjectMapper.Map<CustomerDto>(await _userProfileRepository.GetAll().Where(x => x.UserId == 426).FirstOrDefaultAsync());
            var bdReceiptDto = new BdReceiptDto
            {
                Neilson = neilson,
                Customer = customer,
                Order = userOrder
            };
            return bdReceiptDto;
        }

        public async Task<string> GetReportData(int orderId, PurchaseType purchaseType)
        {
            var userOrder = await _bdOrderRepository.GetAsync(orderId);
            if (userOrder.CreatorUserId != _session.UserId)
            {
                throw new UserFriendlyException("Invalid order id!");
            }
            var data = await _commonRepository.GetBdReportData(orderId, _session.UserId, (int)purchaseType);
            return data;
        }

        public async Task<int> Create(BdOrderInput input)
        {
            var orderId = await _bdOrderRepository.InsertAndGetIdAsync(ObjectMapper.Map<BDOrder>(input));
            return orderId;
        }

        public async Task CreatePurchase(PurchaseInput input)
        {
            if (input.PurchaseType == PurchaseType.Records)
            {
                await SavePurchaseRecords(input);
            }
            else if (input.PurchaseType == PurchaseType.XDates)
            {
                await SavePurchaseXDates(input);
            }
            else
            {
                await SavePurchaseRecords(input);
                await SavePurchaseXDates(input);
            }
        }

        private async Task SavePurchaseXDates(PurchaseInput input)
        {
            var query = BDQueryBuilder.GetXDatesQuery(input);
            var data = await _commonRepository.GetData(query);
            var dunsNumbers = JsonConvert.DeserializeObject<DunsData[]>(data);
            await SaveDunsNumbers(dunsNumbers, input.OrderId, "XDate");
        }

        private async Task SavePurchaseRecords(PurchaseInput input)
        {
            var query = BDQueryBuilder.GetRecordsQuery(input);
            var data = await _commonRepository.GetData(query);
            var dunsNumbers = JsonConvert.DeserializeObject<DunsData[]>(data);
            await SaveDunsNumbers(dunsNumbers, input.OrderId, "Record");
        }

        private async Task SaveDunsNumbers(DunsData[] dunsData, int orderId, string recordType)
        {
            if (dunsData == null)
            {
                throw new ArgumentNullException(nameof(dunsData));
            }

            if (dunsData.Length > 0)
            {
                foreach (var duns in dunsData)
                {
                    var bdPurchaseInput = new BdPurchaseInput
                    {
                        DUNSNumber = duns.DUNSNUMBER,
                        OrderID = orderId,
                        RecordType = recordType
                    };
                    await _bdPurchaseRepository.InsertAsync(ObjectMapper.Map<BdPurchasedData>(bdPurchaseInput));
                }
            }
        }




        private IQueryable<BDOrder> CreateFilteredQuery(GetOrderInput input)
        {
            return _bdOrderRepository.GetAll().Where(x => x.CreatorUserId == _session.UserId)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Description.Contains(input.Keyword) || x.Id.ToString().Contains(input.Keyword));
        }
    }
}
