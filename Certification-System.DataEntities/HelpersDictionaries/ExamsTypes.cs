using System.Collections.Generic;

namespace Certification_System.Entities
{
    public static class ExamsTypes
    {
        public static readonly Dictionary<int, string> TypesOfExams= new Dictionary<int, string>()
         {
           {0, "Pisemny"},
           {1, "Ustny"},
           {2, "Praktyczny"},
        };
    }
}
