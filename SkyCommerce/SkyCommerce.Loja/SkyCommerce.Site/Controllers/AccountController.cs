using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyCommerce.Site.Models;
using System;
using System.Threading.Tasks;

namespace SkyCommerce.Site.Controllers
{
    [Route("conta")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        [HttpGet]
        [Authorize]
        [AllowAnonymous]
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
            return SignOut("Cookies","oidc");
        }

        [Route("esqueci-senha")]
        public IActionResult EsqueciSenha()
        {
            return View();
        }

    }
}
