using System.Collections.Generic;

namespace Certification_System.ServicesInterfaces.Models
{
    public static class EmailMessageTypes
    {
        public static readonly Dictionary<string, string> EmailMessageBody = new Dictionary<string, string>
         {
           {"resetPassword", "Zarejestrowaliśmy chęć zresetowania hasła do konta użytkownika połączonego z tym adresem email. W przypadku, gdy chciałeś odzyskać swoje hasło wykorzystaj przycisk znajdujący się poniżej. Jeżeli ta akcja nie była autoryzowana przez Ciebie - zignoruj tę wiadomość."},
         };

        public static readonly Dictionary<string, string> EmailMessageHeader = new Dictionary<string, string>
         {
           {"resetPassword", "Zarejestrowana została prośba resetu hasła"},

         };

        public static readonly Dictionary<string, string> EmailMessageSubject = new Dictionary<string, string>
         {
           {"resetPassword", "Reset hasła do konta na Certification-Platform"},

         };

        public static readonly Dictionary<string, string> EmailMessageLinkText = new Dictionary<string, string>
         {
           {"resetPassword", "Resetuj hasło"},

         };
    }
}
