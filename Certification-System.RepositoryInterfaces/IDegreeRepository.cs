using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IDegreeRepository
    {
        ICollection<Degree> GetListOfDegrees();
        ICollection<SelectListItem> GetDegreesAsSelectList();
        void AddDegree(Degree degree);
        Degree GetDegreeById(string degreeIdentificator);
        ICollection<Degree> GetDegreesById(ICollection<string> degreeIdentificators);
        void UpdateDegree(Degree degree);
        ICollection<Degree> GetDegreesToDisposeByUserCompetences(ICollection<string> givenCertificates, ICollection<string> givenDegrees);
        ICollection<Degree> DeleteBranchFromDegrees(string branchIdentificator);
        ICollection<Degree> DeleteCertificateFromDegrees(string certificateIdentificator);
    }
}
