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

        public async Task SendTicketReceiptAsync(string toEmail, string customerName, List<TicketDetail> tickets, string sessionId)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Zoolirante Zoo", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress(customerName, toEmail));
            message.Subject = "Your Zoolirante Ticket Receipt";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = BuildTicketReceiptHtml(customerName, tickets, sessionId);

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["Email:SmtpHost"],
                int.Parse(_configuration["Email:SmtpPort"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            await client.AuthenticateAsync(
                _configuration["Email:SmtpUser"],
                _configuration["Email:SmtpPassword"]
            );

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

