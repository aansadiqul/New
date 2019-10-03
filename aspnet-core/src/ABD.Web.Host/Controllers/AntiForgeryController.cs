using Microsoft.AspNetCore.Antiforgery;
using ABD.Controllers;

namespace ABD.Web.Host.Controllers
{
    public class AntiForgeryController : ABDControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}
