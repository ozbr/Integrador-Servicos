using Leitor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor.Email
{
    public interface IEmailLoader
    {
        DateTime LastRequestStartedOn { get; set; }
        DateTime RequestDate { get; set; }
        EmailInfo Info { get; set; }
        List<EmailData> LoadEmails(ReadEmailHandler readHandler);
    }

    public delegate bool ReadEmailHandler(EmailData email);
}
