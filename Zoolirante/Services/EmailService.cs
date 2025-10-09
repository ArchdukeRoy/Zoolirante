using MailKit.Net.Smtp;
using MimeKit;

namespace Zoolirante.Services
{
    public interface IEmailService
    {
        Task SendTicketReceiptAsync(string toEmail, string customerName, List<TicketDetail> tickets, string sessionId);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IQrCodeService _qrCodeService;

        public EmailService(IConfiguration configuration, IQrCodeService qrCodeService)
        {
            _configuration = configuration;
            _qrCodeService = qrCodeService;
        }

            bodyBuilder.HtmlBody = BuildTicketReceiptHtml(customerName, tickets, sessionId);
