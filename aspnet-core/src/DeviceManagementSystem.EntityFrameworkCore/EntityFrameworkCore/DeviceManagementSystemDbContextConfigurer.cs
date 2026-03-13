using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace DeviceManagementSystem.EntityFrameworkCore
{
    public static class DeviceManagementSystemDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<DeviceManagementSystemDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<DeviceManagementSystemDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
