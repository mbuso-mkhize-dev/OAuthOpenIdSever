using Microsoft.AspNetCore.Mvc;
using OAuthOpenIdServer.ApplicationLogic.Boundaries.ApplicationLogic.Services;
using OAuthOpenIdServer.ApplicationLogic.Entities.Users;
using OAuthOpenIdServer.Models;
using System.Threading.Tasks;

namespace OAuthOpenIdServer.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok();
        }
        // POST: User/Create
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserViewModel model)
        {
            var user = await _userService.CreateUserAsync(new UserEntity { Username = model.Email, Email = model.Email, Password = model.Password, FirstName = model.Name }, string.Empty);

            return Ok(user);
        }
    }
}