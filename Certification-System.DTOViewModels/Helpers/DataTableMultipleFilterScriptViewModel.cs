using System.Collections.Generic;

namespace Certification_System.DTOViewModels
{
    public class DataTableMultipleFilterScriptViewModel
    {
        public string TableIdentificator { get; set; }
        public ICollection<string> NumberOfColumn { get; set; }
        public ICollection<string> SelectId { get; set; }
    }
}
