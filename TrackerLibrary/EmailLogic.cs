﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TrackerLibrary
{
    public static class EmailLogic
    {
        public static void SendEmail(List<string> to, string subject, string body)
        {
            SendEmail(to, new List<string>(), subject, body);
        }

        public static void SendEmail(List<string> to, List<string> bcc, string subject, string body)
        {
            MailAddress fromMailAddress = new MailAddress(GlobalConfig.AppKeyLookup("senderEmail"), GlobalConfig.AppKeyLookup("senderDisplayName"));
            MailMessage mail = new MailMessage();

            mail.From = fromMailAddress;
            foreach (string toAddress in to)
            {
                mail.To.Add(toAddress);
            }
            foreach (string bccAddress in bcc)
            {
                mail.Bcc.Add(bccAddress);
            }
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient client = new SmtpClient();
            client.Send(mail);
        }
    }
}
