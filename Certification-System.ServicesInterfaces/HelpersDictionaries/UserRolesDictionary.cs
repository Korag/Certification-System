using System;
using System.Collections.Generic;

namespace Certification_System.ServicesInterfaces.Models
{
    public static class UserRolesDictionary
    {
        public static readonly Dictionary<string, string> TranslationDictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
         {
           {"Admin", "Administrator"},
           {"Worker", "Pracownik"},
           {"Company", "Pracodawca"},
           {"Instructor", "Instruktor"},
           {"Examiner", "Egzaminator"},
           {"Instructor&Examiner", "Instruktor i Egzaminator"},
        };
    }
}
