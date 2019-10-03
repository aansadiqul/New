using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ABD.Configuration;
using ABD.Web;

namespace ABD.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class ABDDbContextFactory : IDesignTimeDbContextFactory<ABDDbContext>
    {
        public ABDDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ABDDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            ABDDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ABDConsts.ConnectionStringName));

            return new ABDDbContext(builder.Options);
        }
    }
}
