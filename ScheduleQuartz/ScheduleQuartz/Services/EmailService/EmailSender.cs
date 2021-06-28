using MimeKit;
using ScheduleQuartz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using ScheduleQuartz.Services.FileProviderService;
using System.IO;

namespace ScheduleQuartz.Services.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly ISmtpBuilder _smtpBuilder;
        private readonly IFileProvider _fileProvider;

        public EmailSender(ISmtpBuilder smtpBuilder, IFileProvider fileProvider)
        {
            _smtpBuilder = smtpBuilder;
            _fileProvider = fileProvider;
        }

        /// <summary>
        /// Create an file attachment for the specific file path
        /// </summary>
        /// <param name="filePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <returns>A leaf-node MIME part that contains an attachment.</returns>
        protected MimePart CreateMimeAttachment(string filePath, string attachmentFileName = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(attachmentFileName))
                attachmentFileName = Path.GetFileName(filePath);

            MimePart mimePart =  CreateMimeAttachment(
                    attachmentFileName,
                    _fileProvider.ReadAllBytes(filePath),
                    _fileProvider.GetCreationTime(filePath),
                    _fileProvider.GetLastWriteTime(filePath),
                    _fileProvider.GetLastAccessTime(filePath));

            return mimePart;
        }

        /// <summary>
        /// Create an file attachment for the binary data
        /// </summary>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="binaryContent">The array of unsigned bytes from which to create the attachment stream.</param>
        /// <param name="cDate">Creation date and time for the specified file or directory</param>
        /// <param name="mDate">Date and time that the specified file or directory was last written to</param>
        /// <param name="rDate">Date and time that the specified file or directory was last access to.</param>
        /// <returns>A leaf-node MIME part that contains an attachment.</returns>
        protected MimePart CreateMimeAttachment(string attachmentFileName, byte[] binaryContent, DateTime cDate, DateTime mDate, DateTime rDate)
        {
            if (!ContentType.TryParse(MimeTypes.GetMimeType(attachmentFileName), out var mimeContentType))
                mimeContentType = new ContentType("application", "octet-stream");

            return new MimePart(mimeContentType)
            {
                FileName = attachmentFileName,
                Content = new MimeContent(new MemoryStream(binaryContent), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition
                {
                    CreationDate = cDate,
                    ModificationDate = mDate,
                    ReadDate = rDate
                }
            };
        }

        public void SendEmail(EmailAccount emailAccount, string subject, string body,
        string fromAddress, string fromName, string toAddress, string toName,
        string replyTo = null, string replyToName = null,
        IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
        string attachmentFilePath = null, string attachmentFileName = null,
        int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress(toName, toAddress));

            if (!string.IsNullOrEmpty(replyTo))
            {
                message.ReplyTo.Add(new MailboxAddress(replyToName, replyTo));
            }

            //BCC =>  tạo ra bản sao email và gởi tới nhiều người khác , và họ sẽ không thấy được bbc khác
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                    message.Bcc.Add(new MailboxAddress(address.Trim()));
            }

            //CC =>  tạo ra bản sao email và gởi tới nhiều người khác , và họ cũng sẽ thấy được cc 
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                    message.Cc.Add(new MailboxAddress(address.Trim()));
            }

            //headers
            if (headers != null)
            {
                foreach (var header in headers)
                    message.Headers.Add(header.Key, header.Value);
            }

            //content
            message.Subject = subject;

            var multipart = new Multipart("mixed")
            {
                new TextPart(MimeKit.Text.TextFormat.Html) {Text = body}
            };
            message.Body = multipart;

            //create the file attachment for this e-mail message
            if (!string.IsNullOrEmpty(attachmentFilePath) && _fileProvider.FileExists(attachmentFilePath))
            {
                multipart.Add(CreateMimeAttachment(attachmentFilePath, attachmentFileName));
            }

            //send email
            using var smtpClient = _smtpBuilder.Build(emailAccount);
            smtpClient.Send(message);
            smtpClient.Disconnect(true);
        }
    }
}
