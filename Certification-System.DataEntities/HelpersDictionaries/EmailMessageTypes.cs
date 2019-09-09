using System.Collections.Generic;

namespace Certification_System.DataEntities
{
    public static class EmailMessageTypesDictionary
    {
        public static readonly Dictionary<string, string> EmailMessageBody = new Dictionary<string, string>
         {
           {"resetPassword", "Zarejestrowaliśmy chęć zresetowania hasła do konta użytkownika połączonego z tym adresem email. W przypadku, gdy chciałeś odzyskać swoje hasło wykorzystaj przycisk znajdujący się poniżej. Jeżeli ta akcja nie była autoryzowana przez Ciebie - zignoruj tę wiadomość."},
           {"emailConfirmation", "Zarejestrowaliśmy chęć zalogowania się do konta użytkownika połączonego z tym adresem email. Logowanie zostało przerwane ze względu na brak potwierdzenia adresu email. Czy chcesz umożliwić logowanie do swojego konta poprzez potwierdzenie tego adresu email ? W przypadku chęci zmiany adresu poczty elektronicznej bez dokonania potwierdzenia skontaktuj się z zespołem administratorów systemu."},
           {"register", "Właśnie odnotowaliśmy rejestrację nowego konta użytkownika powiązanego z tym adresem email. Witamy na podkładzie. W celu możliwości zalogowania niezbędne jest potwierdzenie adresu email poprzez kliknięcie w przycisk poniżej. Nie zakładałeś konta na naszej platformie ? Nie masz się czego obawiać Twój adres email nie będzie mógł służyć kontu innej osoby."},
           {"changePassword", "Informujemy, że dokonano właśnie zmiany hasła konta powiązanego z tym adresem email. W przypadku nieautoryzowanego dostępu do konta prosimy o pilny kontakt z administratorem systemu."},
           {"setPassword", "Otrzymujesz tę wiadomość ponieważ zostało dla Ciebie utworzone konto użytkownika na platformie Certification-System. W celu rozpoczęcia pracy z systemem musisz przypisać hasło do swojego konta użytkownika. W tym celu kliknij w przycisk znajdujący się poniżej. Bez ustanowienia hasła logowanie do systemu będzie niemożliwe. W przypadku trudności skontaktuj się z administratorem systemu."},
           {"resetPasswordWithoutEmailConfirmation", "Wykryto próbę zmiany hasła do konta powiązanego z tym adresem email. Próba ta została zablokowana ze względu na brak potwierdzenia z tego adresu email. Jeżeli ta próba była autoryzowana przez Ciebie najpierw potwierdź swój email. Jeżeli nie posiadasz stosownej wiadomości na swojej skrzynce skontaktuj się z administratorem systemu."},
           {"manuallySendEmailConfirmationMessage", "Administrator systemu prosi o dokonanie potwierdzenia tego adresu email, który jest powiązany z kontem w Certification-System. Prosimy o kliknięcie w link poniżej."},
           {"manuallySendResetPasswordMessage", "Administrator systemu uruchomił procedurę resetu hasła dla Twojego konta użytkownika. W celu dokończenia procesu należy kliknąć w poniższy link."},
        };

        public static readonly Dictionary<string, string> EmailMessageHeader = new Dictionary<string, string>
         {
           {"resetPassword", "Zarejestrowana została prośba resetu hasła"},
           {"emailConfirmation", "Adres email dla konta nie został potwierdzony"},
           {"register", "Konto zostało utworzone"},
           {"changePassword", "Hasło Twojego konta zostało zmienione"},
           {"setPassword", "Twoje konto użytkownika zostało utworzone przez osobę trzecią"},
           {"resetPasswordWithoutEmailConfirmation", "Ktoś próbował zresetować Twoje hasło"},
           {"manuallySendEmailConfirmationMessage", "Wykonano ręczną akcję wywołania procedury potwierdzenia adresu email"},
           {"manuallySendResetPasswordMessage", "Wykonano ręczną akcję wywołania procedury resetu hasła użytkownika"},
        };

        public static readonly Dictionary<string, string> EmailMessageSubject = new Dictionary<string, string>
         {
           {"resetPassword", "Reset hasła do konta w Certification-System"},
           {"emailConfirmation", "Potwierdzenie adresu email w Certification-System"},
           {"register", "Rejestracja i potwierdzenie adresu email w Certification-System"},
           {"changePassword", "Zmiana hasła do konta w Certification-System"},
           {"setPassword", "Utworzenia konta w Certification-System"},
           {"resetPasswordWithoutEmailConfirmation", "Próba zresetowania hasła do Twojego konta w Certification-System"},
           {"manuallySendEmailConfirmationMessage", "Potwierdzenie adresu email w Certification-System"},
           {"manuallySendResetPasswordMessage", "Reset hasła do konta w Certification-System"},
        };

        public static readonly Dictionary<string, string> EmailMessageLinkText = new Dictionary<string, string>
         {
           {"resetPassword", "Resetuj hasło"},
           {"emailConfirmation", "Potwierdź adres email"},
           {"register", "Potwierdź adres email"},
           {"changePassword", ""},
           {"setPassword", "Ustaw swoje hasło"},
           {"resetPasswordWithoutEmailConfirmation", ""},
           {"manuallySendEmailConfirmationMessage", "Potwierdź adres email"},
           {"manuallySendResetPasswordMessage", "Resetuj hasło"},
        };
    }
}
