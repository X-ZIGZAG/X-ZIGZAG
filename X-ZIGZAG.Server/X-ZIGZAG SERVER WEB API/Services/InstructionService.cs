using Microsoft.EntityFrameworkCore;
using X_ZIGZAG_SERVER_WEB_API.Data;
using X_ZIGZAG_SERVER_WEB_API.Interfaces;
using X_ZIGZAG_SERVER_WEB_API.Models;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

namespace X_ZIGZAG_SERVER_WEB_API.Services
{
    public class InstructionService:IInstructionService
    {
        private readonly MyDbContext _context ;
        public InstructionService(MyDbContext context) { 
            _context = context;
        }
        public async Task<InstructionResponse>? GetAllByClientId(string clientId)
        {
            var instructions = await _context.Instructions
                .Where(u => u.ClientId.Equals(clientId))
                .Select(i => new InstructionResponseVM
                {
                    InstructionId = i.InstructionId,
                    Code = i.Code,
                    FunctionArgs = i.FunctionArgs
                })
                .ToListAsync();

            return new InstructionResponse
            {
                Instructions = instructions
            };
        }
        public async Task<InstructionResponse?> NewIntruction(string clientId, InstructionVM inst){
            var checkIfClientExist = await _context.CheckSettings.Where(u=>u.Id.Equals(clientId)).FirstOrDefaultAsync();
            if (checkIfClientExist!=null)
            {
                var newInstruction = new Instruction
                {
                    ClientId = clientId,
                    InstructionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Code = inst.Code,
                    FunctionArgs = inst.FunctionArgs,
                };
                checkIfClientExist.CheckCmds = true;
                _context.Instructions.Add(newInstruction);
                await _context.SaveChangesAsync();
                return null;
            }
            return new InstructionResponse { Message = "The Client Doesnt Exists" };
        }
        public async Task<InstructionResponse?> DeleteInstruction(string clientId, long InstructionId)
        {
            int deletedCount = await _context.Instructions.Where(inst => inst.ClientId.Equals(clientId) && inst.InstructionId==InstructionId).ExecuteDeleteAsync();
            if(deletedCount > 0)
            {
                return null;
            }
            return new InstructionResponse {Message = "Not Found"};
        }
        public async Task<InstructionResponse?> DeleteAllInstructions(string clientId)
        {
            int deletedCount = await _context.Instructions.Where(inst => inst.ClientId.Equals(clientId)).ExecuteDeleteAsync();
            if (deletedCount > 0)
            {
                return null;
            }
            return new InstructionResponse { Message = "Not Found" };
        }
    }
}
