using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace ABD.EntityFrameworkCore
{
    public static class ABDDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ABDDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<ABDDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
