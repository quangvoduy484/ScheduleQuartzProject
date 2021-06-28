using MailKit.Net.Smtp;
using MailKit.Security;
using ScheduleQuartz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ScheduleQuartz.Services.EmailService
{
    public class SmtpBuilder : ISmtpBuilder
    {
        public SmtpClient Build(EmailAccount emailAccount = null)
        {
            if (emailAccount is null)
                throw new ArgumentNullException("Email account could not be loaded");

            SmtpClient smtpClient = new SmtpClient();

            try
            {
                smtpClient.Connect(emailAccount.Host, emailAccount.Port, SecureSocketOptions.Auto);

                if (emailAccount.UseDefaultCredentials)
                {
                    smtpClient.Authenticate(CredentialCache.DefaultNetworkCredentials);
                }
                else if (!string.IsNullOrWhiteSpace(emailAccount.Username))
                {
                    smtpClient.Authenticate(new NetworkCredential(emailAccount.Username, emailAccount.Password));
                }

                return smtpClient;
            }
            catch (Exception ex)
            {
                smtpClient.Dispose();
                throw new Exception(ex.Message, ex);
            }
        }

        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
