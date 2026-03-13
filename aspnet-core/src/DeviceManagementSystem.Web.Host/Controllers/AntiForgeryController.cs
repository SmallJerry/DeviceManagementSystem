using Abp.Auditing;
using Abp.Web.Security.AntiForgery;
using DeviceManagementSystem.Controllers;
using Microsoft.AspNetCore.Antiforgery;

namespace DeviceManagementSystem.Web.Host.Controllers
{
    public class AntiForgeryController : DeviceManagementSystemControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IAbpAntiForgeryManager _antiForgeryManager;

        public AntiForgeryController(IAntiforgery antiforgery, IAbpAntiForgeryManager antiForgeryManager)
        {
            _antiforgery = antiforgery;
            _antiForgeryManager = antiForgeryManager;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }

        [DisableAuditing]
        public void SetCookie()
        {
            _antiForgeryManager.SetCookie(HttpContext);
        }
    }
}
