using System.Collections.Generic;

namespace Certification_System.ServicesInterfaces.Models
{
    public static class EmailMessageTypes
    {
        public static readonly Dictionary<string, string> EmailMessageBody = new Dictionary<string, string>
         {
           {"resetPassword", ""},
         };

        public static readonly Dictionary<string, string> EmailMessageHeader = new Dictionary<string, string>
         {
           {"resetPassword", ""},
         };

        public static readonly Dictionary<string, string> EmailMessageSubject = new Dictionary<string, string>
         {
           {"resetPassword", ""},
         };
    }
}
