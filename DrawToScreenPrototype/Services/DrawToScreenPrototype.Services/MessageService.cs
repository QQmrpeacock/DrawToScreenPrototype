using DrawToScreenPrototype.Services.Interfaces;

namespace DrawToScreenPrototype.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
