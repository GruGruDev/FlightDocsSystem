using MailKit.Security;
using MailKit;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using MailKit.Net.Smtp;
using System.Security.Authentication;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using MailKit.Net.Proxy;
using System.Text;
using System.Net.Sockets;
using FlightDocsSystem.Services.Auth.Interfaces;
using FlightDocsSystem.Utilities;
using Microsoft.Extensions.Options;

namespace FlightDocsSystem.Services.Auth
{
	public class MailService : SmtpClient, Interfaces.IMailService
	{
		public MailSettings _mailSettings { get; set; }
		public MailService(IOptions<MailSettings> mailSettings)
		{
			_mailSettings = mailSettings.Value;
		}
		public async Task SendEmailAsync(string fullName, string to, string subject, string body)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
			emailMessage.To.Add(new MailboxAddress(fullName, to));
			emailMessage.Subject = subject;

			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = body,
			};

			emailMessage.Body = bodyBuilder.ToMessageBody();

			await this.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
			await this.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
			await this.SendAsync(emailMessage);
			await this.DisconnectAsync(true);
		}
	}
}
