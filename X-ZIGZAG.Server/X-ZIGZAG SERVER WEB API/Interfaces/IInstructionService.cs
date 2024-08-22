using X_ZIGZAG_SERVER_WEB_API.ViewModels.Request;
using X_ZIGZAG_SERVER_WEB_API.ViewModels.Response;

namespace X_ZIGZAG_SERVER_WEB_API.Interfaces
{
    public interface IInstructionService
    {
        public Task<InstructionResponse>? GetAllByClientId(string clientId);
        public Task<InstructionResponse?> NewIntruction(string clientId, InstructionVM inst);
        public Task<InstructionResponse?> DeleteInstruction(string clientId, long InstructionId);
        public Task<InstructionResponse?> DeleteAllInstructions(string clientId);

    }
}
