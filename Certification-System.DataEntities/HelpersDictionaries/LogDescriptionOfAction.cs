using System.Collections.Generic;

namespace Certification_System.Entities
{
    public static class LogTypeOfAction
    {
        public static readonly Dictionary<int, string> TypesOfActions= new Dictionary<int, string>()
         {
           {0, "Dodaj"},
           {1, "Edytuj"},
           {2, "Usuń"},
        };
    }
}
