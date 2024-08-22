using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.Filters;

namespace X_ZIGZAG_SERVER_WEB_API.Controllers
{
    [Route("[controller]")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ApiController]
    public class AdminController : ControllerBase
    {
        IAdminService adminService;
        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAdmins()
        {
            var result = await adminService.GetAll();
            return Ok(result.Admins);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminVM authInfo)
        {            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var result = await adminService.Create(authInfo);
            if (result.Token != null && !result.Token.Equals(""))
            {
                return StatusCode(201,new { result.Token });
            }
            return Conflict(new { result.Message });
        }
        [HttpPatch("{username}/username")]
        public async Task<IActionResult> UpdateUsername(string username, [FromBody] AdminUsernameVM nusername)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            var result = await adminService.UpdateUsername(username, nusername.Username);
            if (result.Message != null && !result.Message.Equals(""))
            {
                if(result.Message.Contains("Another"))
                {
                    return Conflict(result.Message);
                }
                return NotFound(new { result.Message });
            }
            return Ok();
        }
        [HttpPatch("{username}/password")]
        public async Task<IActionResult> UpdatePassword(string username, [FromBody] AdminPasswordVM password)
        {
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }
            var result = await adminService.UpdatePassword(username, password.Password);
            if (result.Message != null && !result.Message.Equals(""))
            {
                return NotFound(new { result.Message });
            }
            return Ok();
        }
        [HttpDelete("{username}")]
        public async Task<IActionResult> Delete(string username)
        {
            var result = await adminService.Delete(username);
            if(result.Message != null && !result.Equals(""))
            {
                return NotFound(new {result.Message});
            }
            return Ok();
        }
    }
}
