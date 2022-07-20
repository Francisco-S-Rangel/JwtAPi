using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAPi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController: ControllerBase
    {
        /// <summary>
        /// Permite que vc tenha acesso a informação mesmo sendo um anonimo
        /// </summary>
        [HttpGet]
        [Route("Anonimo")]
        [AllowAnonymous]
        public string Anonymos() => "Anonimo";

        /// <summary>
        /// Permite que vc tenha acesso a informação de que logou 
        /// </summary>
        [HttpGet]
        [Route("Autentificado")]
        [Authorize]
        public string Authenticaded() => $"{User.Identity.Name} - Autentificado.";

        /// <summary>
        /// Permite que vc tenha acesso se vc for Vilão
        /// </summary>
        [HttpGet]
        [Route("Vilan")]
        [Authorize(Roles = "Vilan")]
        public string VilanResponse() => "O vilão conseguio acesso.";

        /// <summary>
        /// Permite que vc tenha acesso se vc for Heroi
        /// </summary>
        [HttpGet]
        [Route("Hero")]
        [Authorize(Roles = "Hero")]
        public string HeroResponse() => "O heroi conseguio acesso.";



    }
}
