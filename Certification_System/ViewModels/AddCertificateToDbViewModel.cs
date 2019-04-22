using Certification_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Certification_System.ViewModels
{
    public class AddCertificateToDbViewModel
    {
        public string CertificateIdentificator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Branch> Branches { get; set; }
        public IEnumerable<SelectListItem> BranchesChosen { get; set; }

        //public IEnumerable<SelectListItem> FlavorItems
        //{
        //    get { return new SelectList(Branches, "Name"); 
        //}
    }
}