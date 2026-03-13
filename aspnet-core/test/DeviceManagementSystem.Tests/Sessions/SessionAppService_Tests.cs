using DeviceManagementSystem.Sessions;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace DeviceManagementSystem.Tests.Sessions
{
    public class SessionAppService_Tests : DeviceManagementSystemTestBase
    {
        private readonly ISessionAppService _sessionAppService;

        public SessionAppService_Tests()
        {
            _sessionAppService = Resolve<ISessionAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_Current_User_When_Logged_In_As_Host()
        {
            // Arrange
            LoginAsHostAdmin();

            // Act
            var output = await _sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await GetCurrentUserAsync();
            output.Data.User.ShouldNotBe(null);
            output.Data.User.Name.ShouldBe(currentUser.Name);

            output.Data.Tenant.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Get_Current_User_And_Tenant_When_Logged_In_As_Tenant()
        {
            // Act
            var output = await _sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await GetCurrentUserAsync();
            var currentTenant = await GetCurrentTenantAsync();

            output.Data.User.ShouldNotBe(null);
            output.Data.User.Name.ShouldBe(currentUser.Name);

            output.Data.Tenant.ShouldNotBe(null);
            output.Data.Tenant.Name.ShouldBe(currentTenant.Name);
        }
    }
}
