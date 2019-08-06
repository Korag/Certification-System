using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IGivenDegreeRepository
    {
        ICollection<GivenDegree> GetGivenDegreesByIdOfDegree(string degreeIdentificator);
        ICollection<GivenDegree> GetGivenDegrees();
        void AddGivenDegree(GivenDegree givenDegree);
    }
}
