using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ABD.Authorization;
using ABD.Entities;
using ABD.Customers.Dto;
using ABD.Users.Dto;
using Abp.Authorization;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.BackgroundJobs;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using ABD.Hangfire;
using ABD.Hangfire.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ABD.Customers
{
    [AbpAuthorize]
    public class CustomerAppService : AsyncCrudAppService<UserProfile, CustomerDto, int, GetCustomerInput, CreateCustomerDto, CustomerDto>, ICustomerAppService
    {
        private readonly string[] _acceptedFileTypes = new[] { ".jpg", ".jpeg", ".png" };
        private readonly IRepository<UserProfile> _customerRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IAbpSession _session;

        public CustomerAppService(IRepository<UserProfile> customerRepository,
                                IHostingEnvironment hostingEnvironment,
                                IBackgroundJobManager backgroundJobManager,
                                IAbpSession session) : base(customerRepository)
        {
            _customerRepository = customerRepository;
            _hostingEnvironment = hostingEnvironment;
            _backgroundJobManager = backgroundJobManager;
            _session = session;
        }

        public override async Task<CustomerDto> Create(CreateCustomerDto input)
        {
            var customer = ObjectMapper.Map<UserProfile>(input);
            var userProfile = await _customerRepository.InsertAsync(customer);
            var customerDto = MapToEntityDto(userProfile);
            if (customerDto != null)
            {
                await SendEmail(customerDto);
            }
            return customerDto;
        }

        [AbpAuthorize(PermissionNames.Pages_Roles)]
        public async Task<PagedResultDto<UserProfileDto>> GetAllCustomers(PagedCustomerResultRequestDto input)
        {
            var query = CreateCustomerFilteredQuery(input);
            var customerCount = await query.CountAsync();
            var customers = await query.OrderByDescending(p => p.CreationTime).Select(x => new UserProfileDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.UserName,
                Name = x.FName,
                Surname = x.LName,
                EmailAddress = x.Email,
                IsActive = x.User.IsActive,
                FullName = x.User.FullName,
                RoleIds = x.User.Roles.Select(y => y.RoleId).ToArray(),
                AdActiveDate = x.ADActiveDate,
                AdExpireDate = x.ADExpiresDate,
                BdActiveDate = x.BDActiveDate,
                SalesUser = x.SUSERID,
                CompanyName = x.CompanyName,
                ImagePath = x.ImagePath,
                IsSalesPerson = x.IsSalesPerson
            }).PageBy(input).ToListAsync();

            return new PagedResultDto<UserProfileDto>(
                customerCount,
                customers
            );
        }

        public async Task<PagedResultDto<UserProfileDto>> GetUsers(PagedCustomerResultRequestDto input)
        {
            var query = CreateCustomerFilteredQuery(input);
            var userQuery = query.Where(x => x.CreatorUserId == _session.UserId);
            var customerCount = await userQuery.CountAsync();
            var customers = await userQuery.OrderByDescending(p => p.CreationTime).Select(x => new UserProfileDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.UserName,
                Name = x.FName,
                Surname = x.LName,
                EmailAddress = x.Email,
                IsActive = x.User.IsActive,
                FullName = x.User.FullName,
                RoleIds = x.User.Roles.Select(y => y.RoleId).ToArray(),
                AdActiveDate = x.ADActiveDate,
                AdExpireDate = x.ADExpiresDate,
                BdActiveDate = x.BDActiveDate,
                SalesUser = x.SUSERID,
                CompanyName = x.CompanyName,
                ImagePath = x.ImagePath,
                IsSalesPerson = x.IsSalesPerson
            }).PageBy(input).ToListAsync();

            return new PagedResultDto<UserProfileDto>(
                customerCount,
                customers
            );
        }

        public async Task<CustomerDto> GetCustomerByUserid(long id)
        {
            var userProfile = await Repository.FirstOrDefaultAsync(x => x.UserId == id);
            var customerDto = new CustomerDto();
            //customerDto
            ObjectMapper.Map(userProfile, customerDto);

            if (customerDto == null)
            {
                throw new EntityNotFoundException(typeof(UserProfile), id);
            }

            return customerDto;
        }

        public async Task<List<SalespersonDto>> GetSalesperson()
        {
            var query = Repository.GetAllIncluding(x => x.User)
                .Where(x => x.User.Roles.Any(ur => ur.RoleId == (int) UserRoleEnum.Admin && x.IsSalesPerson == true));
            var salesperson = await query.OrderByDescending(p => p.FName).Select(x => new SalespersonDto
            {
                Id = x.Id,
                Name = x.FName + ' ' + x.LName,
            }).ToListAsync();

            return salesperson;
        }


        public async Task PostImage([FromForm]IFormFile file, long userId)
        {
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

            if (file == null)
                throw new UserFriendlyException(L("File is null"));
            if (file.Length == 0)
            {
                throw new UserFriendlyException(L("File is empty"));
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                throw new ApplicationException("Max file size exceeded.");
            }

            if (_acceptedFileTypes.All(s => s != Path.GetExtension(file.FileName).ToLower()))
            {
                throw new ApplicationException("Uploaded file is not an accepted image file !");
            }

            var fileName = userId.ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName.ToString());

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var userProfile = await Repository.FirstOrDefaultAsync(x => x.UserId == userId);
            userProfile.ImagePath = fileName;
            await Repository.UpdateAsync(userProfile);
        }

        protected IQueryable<UserProfile> CreateCustomerFilteredQuery(PagedCustomerResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.User)
                .WhereIf(input.RoleId.HasValue, x => x.User.Roles.Any(ur => ur.RoleId == input.RoleId))
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.User.UserName.Contains(input.Keyword) || x.CompanyName.Contains(input.Keyword) || x.FName.Contains(input.Keyword) || x.Email.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.User.IsActive == input.IsActive);
        }

        public async Task SendEmail(CustomerDto input)
        {
            var webRootPath = _hostingEnvironment.WebRootPath;
            var adOrderMail = EmailBuilder.EmailBuilder.GenerateCustomerSignUpMail(input, webRootPath);
            await _backgroundJobManager.EnqueueAsync<SendEmailJob, SendEmailJobArgs>(adOrderMail);
        }

    }
}
