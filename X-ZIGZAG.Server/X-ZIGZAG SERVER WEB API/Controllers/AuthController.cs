using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;

namespace X_ZIGZAG_SERVER_WEB_API.Controllers
{
    [Route("Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IAuthService authService;
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        // Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AdminVM authInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
                
            }
            var result = await authService.Login(authInfo);
            if (result.Token != null && result.Token != "")
            {
                return Ok(new {result.Token});
            }
            return Unauthorized(new { result.Message });
        }
       
    }
}
