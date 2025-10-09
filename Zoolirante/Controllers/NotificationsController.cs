using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Zoolirante.ViewModels;

namespace Zoolirante.Controllers
{
    public class NotificationsController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAnimalEventEmail(string animalName, string message)
        {
            // user info
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (string.IsNullOrEmpty(vmJson))
                return Unauthorized();

            var defaultVM = JsonSerializer.Deserialize<DefaultViewModel>(vmJson);
            var toEmail = defaultVM?.email;

            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest("No email found for this user.");

            // Gmail SMTP send logic
            string fromEmail = "lirantezoo@gmail.com";
            string appPassword = "dbdn jurz mcwr slmy ";

            using var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(fromEmail, appPassword)
            };

            var subject = $"Zoolirante, {animalName} Event Notification";
            var body = $"{message}";

            using var msg = new System.Net.Mail.MailMessage(fromEmail, toEmail, subject, body);
            await smtp.SendMailAsync(msg);

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}
