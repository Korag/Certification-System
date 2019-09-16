using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IGivenDegreeRepository
    {
        ICollection<GivenDegree> GetListOfGivenDegrees();
        ICollection<GivenDegree> GetGivenDegreesByIdOfDegree(string degreeIdentificator);
        void AddGivenDegree(GivenDegree givenDegree);
        GivenDegree GetGivenDegreeById(string givenDegreeIdentificator);
        void UpdateGivenDegree(GivenDegree givenDegree);
        ICollection<GivenDegree> GetGivenDegreesById(ICollection<string> givenDegreeIdentificators);
        void DeleteGivenDegree(string givenDegreeIdentificator);
        ICollection<GivenDegree> DeleteGivenDegreesByDegreeId(string degreeIdentificator);
    }
}
