using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Certification_System.DTOViewModels
{
    public class DisplayCompanyCourseSummaryViewModel
    {
        public string CourseIdentificator { get; set; }

        [Display(Name = "Identyfikator")]
        public string CourseIndexer { get; set; }

        [Display(Name = "Nazwa kursu")]
        public string Name { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }

        [Display(Name = "Data rozpoczęcia")]
        public DateTime DateOfStart { get; set; }

        [Display(Name = "Data zakończenia")]
        public DateTime DateOfEnd { get; set; }

        [Display(Name = "Limit uczestników")]
        public int EnrolledUsersLimit { get; set; }

        [Display(Name = "Liczba uczestników")]
        public int EnrolledUsersQuantity { get; set; }

        [Display(Name = "Kończy się egzaminem")]
        public bool ExamIsRequired { get; set; }

        [Display(Name = "Kurs zakończony")]
        public bool CourseEnded { get; set; }

        [Display(Name = "Długość kursu [dni]")]
        public int CourseLength { get; set; }

        [Display(Name = "Ilość egzaminów")]
        public int ExamsQuantity { get; set; }

        [Display(Name = "Obszary certyfikacji")]
        public ICollection<string> Branches { get; set; }

        [Display(Name = "Egzaminy")]
        public ICollection<DisplayExamIndexerWithOrdinalNumberViewModel> AllCourseExams { get; set; }

        [Display(Name = "Uczestnicy kursu")]
        public ICollection<DisplayUserWithCourseResultsViewModel> AllCourseCompanyWorkers{ get; set; }

        [Display(Name = "Uczestnicy kursu z informacją o zaliczonych egzaminach")]
        public ICollection<DisplayUserWithCourseExamPeriodsResultsViewModel> CompanyWorkersWithExamPeriodStatus { get; set; }    

        [Display(Name = "Nadane certyfikaty dla pracowników za wybrany kurs")]
        public ICollection<DisplayGivenCertificateWithoutCourseViewModel> GivenCertificates { get; set; }

        [Display(Name = "Użytkownicy z nadanymi certyfikatami")]
        public DispenseGivenCertificateCheckBoxViewModel[] DispensedGivenCertificates { get; set; }

        [Display(Name = "Egzaminy kursu")]
        public ICollection<DisplayExamIndexerWithOrdinalNumberViewModel> LastExamsPeriods { get; set; }
    }
}
