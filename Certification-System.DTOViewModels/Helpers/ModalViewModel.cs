using System;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class ModalViewModel
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public string Action { get; set; }
        public string Controller { get; set; }

        public string ButtonText { get; set; }
        public string ButtonClass { get; set; }
    }
}
