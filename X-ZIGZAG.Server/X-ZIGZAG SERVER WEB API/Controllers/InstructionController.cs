using Microsoft.AspNetCore.Mvc;
using X_ZIGZAG_SERVER_WEB_API.Filters;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;


namespace X_ZIGZAG_SERVER_WEB_API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(TokenValidationFilter))]
    public class InstructionController : ControllerBase
    {
        private IInstructionService instructionService;
        public InstructionController(IInstructionService instructionService) {
            this.instructionService = instructionService;
        }
        // Get Intructions By ID
        [HttpGet("id")]
        public async Task<IActionResult> GetInstructionID()
        {
            return Ok(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
        // Get Client Intructions By ID
        [HttpGet("{ClientId}")]
        public async Task<IActionResult> Get(string ClientId)
        {
            return Ok((await instructionService.GetAllByClientId(ClientId)).Instructions);
        }
        // New Instruction
        [HttpPost("{ClientId}")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        public async Task<IActionResult> Post(string ClientId,[FromBody] InstructionVM value)
        {
            if (!ModelState.IsValid){
                BadRequest();
            }
            var response = await instructionService.NewIntruction(ClientId, value);
            if (response == null)
            {
                return Ok();
            }
            return NotFound(new { response .Message});

        }
        // Delete Intruction By Client ID + Intstruction Id
        [HttpDelete("{Clientid}/{IntructionId}")]
        public async Task<IActionResult> Delete(string ClientId,long IntructionId)
        {
            var response = await instructionService.DeleteInstruction(ClientId, IntructionId);
            if (response == null)
            {
                return Ok();
            }
            return NotFound(new { response.Message });
        }
        [HttpDelete("{Clientid}")]
        public async Task<IActionResult> Delete(string ClientId)
        {
            var response = await instructionService.DeleteAllInstructions(ClientId);
            if (response == null) {
                return Ok();
            }
            return NotFound(new { response.Message });
        }
    }
}
