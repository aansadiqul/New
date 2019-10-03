using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using ABD.Authorization.Roles;
using ABD.Authorization.Users;
using ABD.Entities;
using ABD.MultiTenancy;

namespace ABD.EntityFrameworkCore
{
    public class ABDDbContext : AbpZeroDbContext<Tenant, Role, User, ABDDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<MetroArea> MetroAreas { get; set; }
        public DbSet<ZipCode> ZipCodes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<DNBFIPSCountyCode> DNBFIPSCountyCodes { get; set; }
        public DbSet<DNBSIC> DNBSICs { get; set; }
        public DbSet<DNBZipCode> DNBZipCodes { get; set; }
        public DbSet<DNBSMSA> DNBSMSAs { get; set; }
        public DbSet<RsState> RsStates { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<RsSICCode> RsSICCodes { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<BDSearch> BDSearches { get; set; }
        public DbSet<BDOrder> BDOrders { get; set; }
        public DbSet<BdPurchasedData> BdPurchasedData { get; set; }
        public DbSet<AMGList> AMGLists { get; set; }
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactsListEmailCount> ContactsListEmailCounts { get; set; }
        public DbSet<PricingRule> PricingRules { get; set; }
        public DbSet<Affiliation> Affiliations { get; set; }
        public DbSet<TargetSector> TargetSectors { get; set; }
        public DbSet<UserExcludeAccount> UserExcludeAccounts { get; set; }
        public DbSet<ADOrder> ADOrders { get; set; }
        public DbSet<ADSearch> ADSearches { get; set; }
        public DbSet<CompanyType> CompanyTypes { get; set; }
        public DbSet<ProductLine> ProductLines { get; set; }
        public DbSet<ContactTitle> ContactTitles { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SpecialAffiliation> SpecialAffiliations { get; set; }
        public DbSet<CarrierLine> CarrierLines { get; set; }

        public ABDDbContext(DbContextOptions<ABDDbContext> options)
            : base(options)
        {

        }
    }
}
