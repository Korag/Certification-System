using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IGivenDegreeRepository
    {
        ICollection<GivenDegree> GetGivenDegreesByIdOfDegree(string degreeIdentificator);
    }
}
