﻿using Certification_System.Entities;
using System.Collections.Generic;

namespace Certification_System.RepositoryInterfaces
{
    public interface IExamRepository
    {
        ICollection<Exam> GetListOfExams();
        void AddExam(Exam exam);
    }
}
