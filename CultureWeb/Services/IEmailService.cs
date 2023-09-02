
using CultureWeb.Models;

namespace CultureWeb.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
