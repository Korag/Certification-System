using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayExamResultViewModel
    {
        [Display(Name = "Identyfikator wyniku")]
        public string ExamResultIndexer { get; set; }

        [Display(Name = "Ilość punktów")]
        public string PointsEarned { get; set; }

        [Display(Name = "Maks. liczba punktów")]
        public int MaxAmountOfPoints { get; set; }

        [Display(Name = "Wynik procentowy")]
        public double PercentageOfResult { get; set; }

        [Display(Name = "Egzamin zaliczony")]
        public bool ExamPassed { get; set; }

        [Display(Name = "Osoba")]
        public DisplayCrucialDataUserViewModel User { get; set; }

        [Display(Name = "Kurs")]
        public DisplayCrucialDataCourseViewModel Course { get; set; }

        [Display(Name = "Egzamin")]
        public DisplayCrucialDataExamViewModel Exam { get; set; }
    }
}
