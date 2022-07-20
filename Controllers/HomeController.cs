using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAPi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController: ControllerBase
    {
        /// <summary>
        /// Permite que vc entre mesmo sendo um anonimo
        /// </summary>
        [HttpGet]
        [Route("Anonimo")]
        [AllowAnonymous]
        public string Anonymos() => "Anonimo";

        [HttpGet]
        [Route("Autentificado")]
        [Authorize]
        public string Authenticaded() => $"{User.Identity.Name} - Autentificado.";

        [HttpGet]
        [Route("Vilan")]
        [Authorize(Roles = "Vilan")]
        public string VilanResponse() => "O vilão conseguio acesso.";

        [HttpGet]
        [Route("Hero")]
        [Authorize(Roles = "Hero")]
        public string HeroResponse() => "O heroi conseguio acesso.";



    }
}
