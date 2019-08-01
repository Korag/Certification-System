﻿using Certification_System.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IDegreeRepository
    {
        ICollection<Degree> GetDegrees();
        ICollection<SelectListItem> GetDegreesAsSelectList();
        void AddDegree(Degree degree);
        Degree GetDegreeById(string degreeIdentificator);
        ICollection<Degree> GetDegreesById(ICollection<string> degreeIdentificators);
        void UpdateDegree(Degree degree);
    }
}