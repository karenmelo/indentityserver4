using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkyCommerce.Site.Controllers
{
    [Route("conta")]
    public class AccountController : Controller
    {
        [HttpGet]
        [Authorize]       
        [Route("entrar")]
        public IActionResult Login(string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        [Authorize, Route("minha-conta")]
        public IActionResult MinhaConta()
        {
            return View();
        }


        [Route("sair")]
        public IActionResult Sair()
        {
            // Clear the existing external cookie to ensure a clean login process
            return SignOut("Cookies", "oidc");
        }

        [Route("esqueci-senha")]
        public IActionResult EsqueciSenha()
        {
            return View();
        }

    }
}
