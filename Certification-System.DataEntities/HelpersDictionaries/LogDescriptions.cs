using System.Collections.Generic;

namespace Certification_System.Entities
{
    public static class LogDescriptions
    {
        public static readonly Dictionary<string, string> DescriptionOfActionOnEntity = new Dictionary<string, string>()
         {
           {"addBranch", "Dodano do systemu nowy obszar certyfikacji"},
           {"updateBranch", "Dokonano aktualizacji istniejącego obszaru certyfikacji"},
           {"deleteBranch",  "Obszar certyfikacji został usunięty z systemu"},
           
           {"addCourse",  "Dodano do systemu nowy kurs"},
           {"updateCourse", "Dokonano aktualizacji danych kursu"},
           {"addUserToCourse",  "Nowi użytkownicy systemu zostali zapisani na kurs"},
           {"removeGroupOfUsersFromCourse",  "Grupa użytkowników została usunięta z kursu"},
           {"deleteCourse",  "Dokonano usunięcia kursu z systemu"},
           {"addCourseQueue",  "Dodano kolejkę oczekujących na zapisanie do kurs"},
           {"updateCourseQueue",  "Dokonano aktualizacji kolejki zapisu do kursu"},
           {"deleteCourseQueue",  "Dokonano usunięcia kolejki zapisu na egzamin z systemu"},
           {"addMeetingToCourse",  "Dodano nowe spotkanie do kursu"},
           {"removeMeetingFromCourse",  "Dokonano usunięcia spotkania z kursu"},
           {"addExamToCourse",  "Dodano egzamin do kursu"},

           {"addCertificate",  "Dodano do systemu nowy certyfikat"},
           {"updateCertificate", "Dokonano aktualizacji danych certyfikatu"},
           {"deleteCertificate",  "Certyfikat został usunięty z systemu"},

           {"addDegree",  "Dokonano do systemu nowy stopień zawodowy"},
           {"updateDegree",  "Dokonano aktualizacji danych stopnia zawodowego"},
           {"deleteDegree",  "Dokonano usunięcia stopnia zawodowego z systemu"},

           {"registerUser",  "Nowy użytkownik zarejestrował się w systemie"},
           {"changePasswordUser",  "Użytkownik zmienił swoje hasło dostępowe do konta"},
           {"confirmEmail",  "Użytkownik potwierdził adres email połączony ze swoim kontem"},
           {"setAccountPassword",  "Użytkownik ustawił po raz 1 swoje hasło dostępowe do konta"},
           {"resetPassword",  "Użytkownik dokonał resetu hasła powiązanego ze swoim kontem"},
           {"updateUsers",  "Dokonano edycji danych użytkownika"},
           {"assignUserToCourse",  "Użytkownik został zapisany na kurs"},
           {"addGivenCertificateToUser",  "Nadano certyfikat użytkownikowi systemu"},
           {"removeUserFromCourse",  "Użytkownik został usunięty z kursu"},
           {"assingUserToCourseQueue",  "Użytkownik został dodany do kolejki zapisu do kursu"},
           {"removeUserFromCourseQueue",  "Użytkownik został usunięty z kolejki zapisu na egzamin"},
           {"deleteUserGivenDegree",  "Dokonano usunięcia jednego z nadanych stopni zawodowych użytkownika"},
           {"deleteUserGivenCertificate",  "Dokonano usunięcia jednego z nadanych certyfikatów użytkownika"},
           {"addUserGivenCertificate",  "Nadano użytkownikowi nowy certyfikat"},
           {"addUserGivenDegree",  "Nadano użytkownikowi nowy stopień zawodowy"},
           {"addUser",  "Dodano do systemu nowego użytkownika"},
           {"updateUser",  "Dokonano aktualizacji danych użytkownika"},
           {"deleteUser",  "Dokonano usunięcie użytkownika z systemu"},

           {"addCompany",  "Dodano do systemu nowe przedsiębiorstwo"},
           {"updateCompany",  "Dokonano aktualizacji istniejących danych przedsiębiorstwa"},
           {"deleteCompany",  "Przedsiębiorstwo zostało usunięte z systemu"},

           {"addMeeting",  "Dodano do systemu nowe spotkanie w ramach kursu"},
           {"updateMeeting",  "Dokonano aktualizacji spotkania w ramach kursu"},
           {"deleteMeeting",  "Dokonano usunięcia spotkania z systemu"},
           {"checkPresenceOnMeeting",  "Dokonano sprawdzenia obecności na spotkaniu"},
           {"removeUserFromMeeting",  "Dokonano usunięcia użytkownika ze spotkania"},

           {"addGivenCertificate",  "Dodano do systemu nowy nadany certyfikat"},
           {"deleteGivenCertificate",  "Dokonano usunięcia nadanego certyfikatu z systemu"},
           {"updateGivenCertificate",  "Dokonano aktualizacji nadanego certyfikatu"},

           {"deleteExam",  "Dokonano usunięcia egzaminu z systemu"},
           {"removeUserFromExam",  "Dokonano usunięcia użytkownika z egzaminu"},
           {"addExam",  "Dodano do systemu nowy egzamin"},
           {"updateExam",  "Dokonano aktualizacji danych egzaminu"},
           {"addUsersToExam",  "Dodano grupę użytkowników do egzaminu"},
           {"removeUsersFromExam",  "Grupa użytkowników została usunięta z egzaminu"},
           {"assignUsersToExam",  "Dodano grupę użytkowników do egzaminu"},
           {"removeExamResultFromCourse",  "Usunięto wynik z danego egzaminu"},
           {"removeExamFromCourse",  "Usunięto egzamin z kursu"},
           {"assignUserToExam",  "Dodano użytkownika do egzaminu"},
           {"addExamTermToExam",  "Dodano nową turę egzaminu do egzaminu"},

           {"deleteExamTerm",  "Dokonano usunięcia tury egzaminu z systemu"},
           {"removeUserFromExamTerm",  "Dokonano usunięcia użytkownika z tury egzaminu"},
           {"addExamTerm",  "Dodano do systemu nową turę egzaminu"},
           {"updateExamTerm",  "Dokonano aktualizacji danych tury egzaminu"},
           {"removeUsersFromExamTerm",  "Grupa użytkowników została usunięta z tury egzaminu"},
           {"assignUserToExamTerm",  "Dodano użytkownika do tury egzaminu"},
           {"assignUsersToExamTerm",  "Dodano grupę użytkowników do tury egzaminu"},
           {"removeExamTermFromExam",  "Usunięto turę egzaminu z wybranego egzaminu."},

           {"deleteExamResult",  "Dokonano usunięcia rezultatu z egzaminu z systemu"},
           {"updateExamResult",  "Dokonano aktualizacji wyniku z egzaminu"},
           {"addExamResult",  "Dodano do systemu nowy wynik z egzaminu"},

           {"addGivenDegree",  "Dodano do systemu nowy nadany stopień zawodowy"},
           {"deleteGivenDegree",  "Dokonano usunięcia nadanego stopnia zawodowego z systemu"},
           {"deleteRequiredDegreeFromDegree",  "Dokonano usunięcia wymaganego stopnia zawodowego ze stopnia zawodowego"},
           {"deleteRequiredGivenCertificateFromDegree",  "Dokonano usunięcia wymaganego certyfikatu ze stopnia zawodowego"},
           {"updateGivenDegree",  "Dokonano aktualizacji nadanego stopnia zawodowego"},
        };

        public static readonly Dictionary<string, string> DescriptionOfPersonalUserLog = new Dictionary<string, string>()
         {
           {"registerUser", "Zarejestrowałeś się w platformie Certification-System. Dziękujemy za okazane zaufanie"},
           {"changePassword", "Zmieniłeś swoje hasło dostępu do systemu"},
           {"confirmEmail",  "Potwierdziłeś adres email powiązany z Twoim kontem użytkownika systemu"},
           {"setPassword",  "Ustawiłeś swoje hasło po raz pierwszy"},
           {"resetPassword",  "Zresetowałeś hasło do swojego konta w systemie"},

           {"addBranch",  "Nowy obszar certyfikacji został dodany do systemu"},
           {"addCertificate",  "Nowy certyfikat został dodany do systemu"},
           {"updateCertificate",  "Dane certyfikatu zostały zaktualizowane"},
           {"deleteCertificate",  "Certyfikat został usunięty"},
           {"deleteBranch",  "Obszar certyfikacji został usunięty"},

           {"addCompany",  "Nowe przedsiębiorstwo zostało dodane do systemu"},
           {"updateCompany",  "Dane przedsiębiorstwa zostały zaktualizowane"},
           {"deleteCompany",  "Przedsiębiorstwo zostało usunięte z systemu"},
           {"updateDegree",  "Dane stopnia zawodowego zostały zaktualizowane"},
           {"addDegree",  "Nowy stopień zawodowy został dodany do systemu"},
           {"deleteDegree",  "Stopień zawodowy został usunięty"},
           {"addUserGivenCertificate",  "Otrzymałeś nowy certyfikat"},
           {"updateGivenCertificate",  "Dane nadanego certyfikatu zostały zaktualizowane"},
           {"updateUserGivenCertificate",  "Dane jednego z Twoich nadanych certyfikatów zostały zmodyfikowane"},
           {"deleteGivenCertificate",  "Nadany certyfikat został usunięty z systemu"},
           {"deleteUserGivenCertificate",  "Jeden z Twoich nadanych certyfikatów został usunięty z systemu"},
           {"updateGivenDegree",  "Dane nadanego stopnia zawodowego zostały zaktualizowane"},
           {"updateUserGivenDegree",  "Dane jednego z Twoich nadanych stopni zawodowych zostały zmodyfikowane"},
           {"addGivenDegree",  "Dane certyfikatu zostały zaktualizowane"},
           {"addUserGivenDegree",  "Otrzymałeś nowy stopień zawodowy"},
           {"deleteGivenDegree",  "Stopień zawodowy został usunięty"},
           {"deleteUserGivenDegree",  "Jeden z Twoich stopni zawodowych został usunięty z systemu"},
           {"updateMeetingUsersInformation",  "Dane spotkania w którym bierzesz udział zostały zmodyfikowane"},
           {"addMeeting",  "Nowe spotkanie w ramach kursu zostało dodane do systemu"},
           {"updateMeeting",  "Dane spotkania w ramach kursu zostały zaktualizowane"},
           {"checkMeetingPresence",  "Na spotkaniu została sprawdzona obecność użytkowników"},
           {"checkUserMeetingPresence",  "W spotkaniu w którym uczestniczysz została sprawdzona obecność użytkowników"},
           {"deleteMeeting",  "Spotkanie w ramach kursu zostało usunięte"},
           {"deleteUserMeeting",  "Spotkanie w kursie w którym uczestniczysz zostało usunięte"},
           {"addUser",  "Nowy użytkownik został dodany do systemu"},
           {"userCreation",  "Twoje konto użytkownika w systemie zostało utworzone"},
           {"updateUser",  "Dane użytkownika zostały zaktualizowane"},
           {"userDataModification",  "Twoje dane użytkownika zostały zmodyfikowane"},
           {"deleteUser",  "Użytkownik został usunięty"},
           {"deleteUserInformation",  "Twoje konto oraz wszystkie powiązane z nim dane zostało usunięte"},
           {"removeUserFromCourse",  "Usunięto użytkownika z kursu"},
           {"removeUserFromMeeting",  "Usunięto użytkownika ze spotkania"},
           {"removeUserFromExam",  "Usunięto użytkownika z egzaminu"},
           {"removeUserFromExamTerm",  "Usunięto użytkownika z tury egzaminu"},
           {"deleteExamResult",  "Wyniki egzaminów użytkoanika zostały usunięte "},

           {"updateCertificate",  "Dane certyfikatu zostały zaktualizowane"},
           {"updateCertificate",  "Dane certyfikatu zostały zaktualizowane"},
           {"updateCertificate",  "Dane certyfikatu zostały zaktualizowane"},
           {"updateCertificate",  "Dane certyfikatu zostały zaktualizowane"},

        };
    }
}

