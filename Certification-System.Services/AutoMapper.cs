using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Branches
            CreateMap<Branch, DisplayBranchViewModel>();

            CreateMap<EditBranchViewModel, Branch>()
                .ForMember(dest => dest.BranchIdentificator, opts => opts.Ignore());

            CreateMap<Branch, EditBranchViewModel>();

            CreateMap<AddBranchViewModel, Branch>();
            #endregion

            #region Certificates
            CreateMap<AddCertificateViewModel, Certificate>()
                     .ForMember(dest => dest.CertificateIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches));

            CreateMap<Certificate, DisplayCertificateViewModel>();

            CreateMap<Certificate, DisplayCrucialDataCertificateViewModel>();

            CreateMap<EditCertificateViewModel, Certificate>()
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches));

            CreateMap<Certificate, EditCertificateViewModel>()
                     .ForMember(dest => dest.SelectedBranches, opts => opts.MapFrom(src => src.Branches));

            CreateMap<Certificate, CertificateDetailsViewModel>()
                     .ForMember(dest => dest.Branches, opts => opts.Ignore())
                     .ForMember(dest => dest.CoursesWhichEndedWithCertificate, opts => opts.Ignore())
                     .ForMember(dest => dest.UsersWithCertificate, opts => opts.Ignore());

            CreateMap<Certificate, CompanyCertificateDetailsViewModel>()
                     .ForMember(dest => dest.Branches, opts => opts.Ignore())
                     .ForMember(dest => dest.UsersWithCertificate, opts => opts.Ignore());

            CreateMap<Certificate, UserGivenCertificatePossessionConfirmationViewModel>()
                     .ForMember(dest => dest.CertificateName, opts => opts.MapFrom(src => src.Name))
                     .ForMember(dest => dest.CertificateDescription, opts => opts.MapFrom(src => src.Description));
            #endregion

            #region Companies
            CreateMap<Company, DisplayCompanyViewModel>();

            CreateMap<AddCompanyViewModel, Company>()
                     .ForMember(dest => dest.CompanyIdentificator, opts => opts.Ignore());

            CreateMap<Company, CompanyDetailsViewModel>()
                     .ForMember(dest => dest.UsersConnectedToCompany, opts => opts.Ignore());

            CreateMap<Company, CompanyWithAccountDetailsViewModel>()
                     .ForMember(dest => dest.UsersConnectedToCompany, opts => opts.Ignore())
                     .ForMember(dest => dest.UserAccount, opts => opts.Ignore());

            CreateMap<EditCompanyViewModel, Company>();

            CreateMap<Company, EditCompanyViewModel>();
            #endregion

            #region Courses
            CreateMap<Course, DisplayCourseViewModel>()
                     .ForMember(dest => dest.EnrolledUsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()));

            CreateMap<Course, DisplayCourseWithPriceViewModel>()
                     .ForMember(dest => dest.EnrolledUsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()));

            CreateMap<Course, DisplayCourseOfferViewModel>();

            CreateMap<Course, DisplayCourseNotificationViewModel>()
                     .ForMember(dest => dest.DaysAfterEndDate, opts => opts.MapFrom(src => DateTime.Now.Subtract(src.DateOfEnd).Days));

            CreateMap<Course, DisplayCourseWithUserRoleViewModel>()
                     .ForMember(dest => dest.EnrolledUsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.Roles, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Course, DisplayCrucialDataCourseViewModel>();

            CreateMap<Course, CourseDetailsViewModel>()
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.Meetings, opts => opts.Ignore())
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore())
                     .ForMember(dest => dest.Exams, opts => opts.Ignore());

            CreateMap<AddCourseViewModel, Course>()
                     .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches))
                     .ForMember(dest => dest.Meetings, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Exams, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddCourseWithMeetingsViewModel, Course>()
                     .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches))
                     .ForMember(dest => dest.Meetings, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Exams, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Course, EditCourseViewModel>()
                     .ForMember(dest => dest.SelectedBranches, opts => opts.MapFrom(src => src.Branches));

            CreateMap<Course, EditCourseWithMeetingsViewModel>()
                     .ForMember(dest => dest.SelectedBranches, opts => opts.MapFrom(src => src.Branches))
                     .ForMember(dest => dest.Meetings, opts => opts.Ignore());

            CreateMap<EditCourseWithMeetingsViewModel, Course>()
                   .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
                   .ForMember(dest => dest.CourseIndexer, opts => opts.Ignore())
                   .ForMember(dest => dest.Meetings, opts => opts.Ignore())
                   .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore())
                   .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches));

            CreateMap<EditCourseViewModel, Course>()
                     .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Meetings, opts => opts.Ignore())
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore())
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches));

            CreateMap<Course, DispenseGivenCertificatesViewModel>()
                    .ForMember(dest => dest.DispensedGivenCertificates, opts => opts.Ignore())
                    .ForMember(dest => dest.AvailableCertificates, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.AllCourseParticipants, opts => opts.Ignore());

            CreateMap<Course, DisplayCourseSummaryViewModel>();

            CreateMap<Course, DisplayCompanyCourseSummaryViewModel>();

            CreateMap<Course, DeleteUsersFromCourseViewModel>()
                     .ForMember(dest => dest.UsersToDeleteFromCourse, opts => opts.Ignore())
                     .ForMember(dest => dest.EnrolledUsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.AllCourseParticipants, opts => opts.Ignore());

            CreateMap<Course, UserGivenCertificatePossessionConfirmationViewModel>()
                    .ForMember(dest => dest.CourseName, opts => opts.MapFrom(src => src.Name))
                    .ForMember(dest => dest.CourseDateOfStart, opts => opts.MapFrom(src => src.DateOfStart))
                    .ForMember(dest => dest.CourseDateOfEnd, opts => opts.MapFrom(src => src.DateOfEnd))
                    .ForMember(dest => dest.Exams, opts => opts.Ignore());

            CreateMap<Course, CourseOfferDetailsViewModel>();
            #endregion

            #region Degrees
            CreateMap<AddDegreeViewModel, Degree>()
                     .ForMember(dest => dest.DegreeIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.RequiredCertificates, opts => opts.MapFrom(src => src.SelectedCertificates))
                     .ForMember(dest => dest.RequiredDegrees, opts => opts.MapFrom(src => src.SelectedDegrees))
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches));

            CreateMap<Degree, DisplayDegreeViewModel>()
                     .ForMember(dest => dest.RequiredCertificates, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.RequiredDegrees, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Degree, EditDegreeViewModel>()
                     .ForMember(dest => dest.SelectedBranches, opts => opts.MapFrom(src => src.Branches))
                     .ForMember(dest => dest.SelectedCertificates, opts => opts.MapFrom(src => src.RequiredCertificates))
                     .ForMember(dest => dest.SelectedDegrees, opts => opts.MapFrom(src => src.RequiredDegrees));

            CreateMap<EditDegreeViewModel, Degree>()
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches))
                     .ForMember(dest => dest.RequiredCertificates, opts => opts.MapFrom(src => src.SelectedCertificates))
                     .ForMember(dest => dest.RequiredDegrees, opts => opts.MapFrom(src => src.SelectedDegrees));

            CreateMap<Degree, DegreeDetailsViewModel>()
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.RequiredCertificates, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.RequiredDegrees, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.UsersWithDegree, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Degree, CompanyDegreeDetailsViewModel>()
                    .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.RequiredCertificates, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.RequiredDegrees, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.UsersWithDegree, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Degree, DisplayCrucialDataDegreeViewModel>();

            CreateMap<Degree, DisplayDegreeWithoutRequirementsViewModel>();

            CreateMap<Degree, UserGivenDegreePossessionConfirmationViewModel>()
                     .ForMember(dest => dest.Branches, opts => opts.Ignore())
                     .ForMember(dest => dest.DegreeName, opts => opts.MapFrom(src => src.Name))
                     .ForMember(dest => dest.DegreeDescription, opts => opts.MapFrom(src => src.Description));
            #endregion

            #region Exams
            CreateMap<Exam, AddExamViewModel>();

            CreateMap<Exam, AddExamWithExamTermsViewModel>();

            CreateMap<AddExamWithExamTermsViewModel, Exam>()
                     .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddExamViewModel, Exam>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                    .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Exam, DisplayExamViewModel>()
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, DisplayExamWithLocationViewModel>()
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, MarkExamViewModel>()
                    .ForMember(dest => dest.Users, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))     
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, DisplayExamWithoutCourseViewModel>()
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, DisplayExamWithoutExaminerViewModel>();

            CreateMap<Exam, DisplayCrucialDataExamViewModel>();

            CreateMap<Exam, DisplayCrucialDataExamWithDatesViewModel>();

            CreateMap<Exam, ExamDetailsViewModel>()
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore())
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore())
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, WorkerExamDetailsViewModel>()
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamResult, opts => opts.Ignore())
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, CompanyWorkersExamDetailsViewModel>()
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamResults, opts => opts.Ignore())
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, EditExamWithExamTermsViewModel>()
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore());

            CreateMap<EditExamWithExamTermsViewModel, Exam>()
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore());

            CreateMap<Exam, EditExamViewModel>();

            CreateMap<EditExamViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore());

            CreateMap<AddExamPeriodViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddExamPeriodWithExamTermsViewModel, Exam>()
                     .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Exam, DeleteUsersFromExamViewModel>()
                     .ForMember(dest => dest.UsersToDeleteFromExam, opts => opts.Ignore())
                     .ForMember(dest => dest.AllExamParticipants, opts => opts.Ignore())
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, AssignUsersFromCourseToExamViewModel>()
                    .ForMember(dest => dest.UsersToAssignToExam, opts => opts.Ignore())
                    .ForMember(dest => dest.CourseParticipants, opts => opts.Ignore())
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<Exam, DisplayExamNameTypeViewModel>();

            CreateMap<Exam, DisplayExamIndexerWithOrdinalNumberViewModel>();

            CreateMap<Exam, DisplayExamResultWithExamIdentificator>()
                     .ForMember(dest => dest.ExamOrdinalNumber, opts => opts.MapFrom(src => src.OrdinalNumber));
            #endregion

            #region ExamTerms
            CreateMap<ExamTerm, AddExamTermWithoutExamViewModel>();

            CreateMap<AddExamTermWithoutExamViewModel, ExamTerm>()
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddExamTermViewModel, ExamTerm>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                    .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<ExamTerm, DisplayExamTermWithoutExaminerViewModel>()
                     .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, DisplayExamTermViewModel>()
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, DisplayCrucialDataExamTermViewModel>();

            CreateMap<ExamTerm, DisplayExamTermWithLocationViewModel>()
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes))
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()));

            CreateMap<ExamTerm, DisplayExamTermWithoutExamViewModel>()
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, DisplayExamTermWithoutExamWithLocationViewModel>()
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<EditExamTermViewModel, ExamTerm>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers));

            CreateMap<ExamTerm, EditExamTermViewModel>()
                    .ForMember(dest => dest.SelectedExaminers, opts => opts.MapFrom(src => src.Examiners));

            CreateMap<EditExamTermWithoutCourseViewModel, ExamTerm>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                    .ForMember(dest => dest.ExamTermIdentificator, opts => opts.Ignore())
                    .ForMember(dest => dest.ExamTermIndexer, opts => opts.Ignore())
                    .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore());

            CreateMap<ExamTerm, EditExamTermWithoutCourseViewModel>()
                    .ForMember(dest => dest.SelectedExaminers, opts => opts.MapFrom(src => src.Examiners));

            CreateMap<ExamTerm, ExamTermDetailsViewModel>()
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.Exam, opts => opts.Ignore())
                    .ForMember(dest => dest.Course, opts => opts.Ignore())
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.Users, opts => opts.Ignore())
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, WorkerExamTermDetailsViewModel>()
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.Exam, opts => opts.Ignore())
                    .ForMember(dest => dest.Course, opts => opts.Ignore())
                    .ForMember(dest => dest.ExamResult, opts => opts.Ignore())
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, CompanyWorkersExamTermDetailsViewModel>()
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.Exam, opts => opts.Ignore())
                    .ForMember(dest => dest.Course, opts => opts.Ignore())
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.ExamResults, opts => opts.Ignore())
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, DeleteUsersFromExamTermViewModel>()
                    .ForMember(dest => dest.Exam, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersToDeleteFromExamTerm, opts => opts.Ignore())
                    .ForMember(dest => dest.AllExamTermParticipants, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, AssignUsersFromCourseToExamTermViewModel>()
                    .ForMember(dest => dest.UsersToAssignToExamTerm, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.CourseParticipants, opts => opts.Ignore())
                    .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalDays))
                    .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => (int)src.DateOfEnd.Subtract(src.DateOfStart).TotalMinutes));

            CreateMap<ExamTerm, DisplayExamTermIndexerViewModel>();
            #endregion

            #region ExamResults
            CreateMap<ExamResult, AddExamResultViewModel>();

            CreateMap<ExamResult, DisplayUserWithExamResults>();

            CreateMap<ExamResult, DisplayExamResultViewModel>()
                     .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<ExamResult, DisplayExamResultToUserViewModel>()
                     .ForMember(dest => dest.Exam, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamTerm, opts => opts.Ignore());

            CreateMap<ExamResult, DisplayExamResultWithExamIdentificator>();

            CreateMap<ExamResult, MarkUserViewModel>();

            CreateMap<MarkUserViewModel, ExamResult>()
                     .ForMember(dest => dest.User, opts => opts.MapFrom(src => src.UserIdentificator));
            #endregion

            #region GivenCertificates
            CreateMap<AddGivenCertificateViewModel, GivenCertificate>();

            CreateMap<GivenCertificate, DisplayGivenCertificateViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<GivenCertificate, DisplayGivenCertificateWithoutCourseViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<GivenCertificate, DisplayGivenCertificateToUserViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.Course, opts => opts.Ignore());

            CreateMap<GivenCertificate, DisplayGivenCertificateToUserWithoutCourseViewModel>()
                 .ForMember(dest => dest.Certificate, opts => opts.Ignore());

            CreateMap<GivenCertificate, DisplayGivenCertificateToUserWithoutCourseExtendedViewModel>()
             .ForMember(dest => dest.Certificate, opts => opts.Ignore());

            CreateMap<EditGivenCertificateViewModel, GivenCertificate>()
                 .ForMember(dest => dest.GivenCertificateIdentificator, opts => opts.Ignore())
                 .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                 .ForMember(dest => dest.Course, opts => opts.Ignore());

            CreateMap<GivenCertificate, EditGivenCertificateViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.Course, opts => opts.Ignore());

            CreateMap<GivenCertificate, UserGivenCertificatePossessionConfirmationViewModel>();
            #endregion

            #region GivenDegrees
            CreateMap<GivenDegree, DisplayGivenDegreeViewModel>()
                      .ForMember(dest => dest.Degree, opts => opts.Ignore())
                      .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<AddGivenDegreeViewModel, GivenDegree>()
                     .ForMember(dest => dest.Degree, opts => opts.MapFrom(src => src.SelectedDegree));

            CreateMap<GivenDegree, EditGivenDegreeViewModel>()
                      .ForMember(dest => dest.User, opts => opts.Ignore())
                      .ForMember(dest => dest.Degree, opts => opts.Ignore());

            CreateMap<EditGivenDegreeViewModel, GivenDegree>()
                     .ForMember(dest => dest.Degree, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegreeIdentificator, opts => opts.Ignore());

            CreateMap<GivenDegree, DisplayGivenDegreeToUserViewModel>()
                     .ForMember(dest => dest.Degree, opts => opts.Ignore());

            CreateMap<GivenDegree, DisplayGivenDegreeToUserExtendedViewModel>()
                     .ForMember(dest => dest.Degree, opts => opts.Ignore());

            CreateMap<GivenDegree, UserGivenDegreePossessionConfirmationViewModel>();
            #endregion

            #region Meetings
            CreateMap<Meeting, DisplayMeetingViewModel>()
                     .ForMember(dest => dest.Instructors, opts => opts.Ignore());

            CreateMap<AddMeetingViewModel, Meeting>()
                     .ForMember(dest => dest.MeetingIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.AttendanceList, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Instructors, opts => opts.MapFrom(src => src.SelectedInstructors));

            CreateMap<AddMeetingWithoutCourseViewModel, Meeting>()
                     .ForMember(dest => dest.MeetingIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Instructors, opts => opts.MapFrom(src => src.SelectedInstructors));

            CreateMap<Meeting, EditMeetingViewModel>()
                     .ForMember(dest => dest.SelectedInstructors, opts => opts.MapFrom(src => src.Instructors));

            CreateMap<EditMeetingViewModel, Meeting>()
                     .ForMember(dest => dest.MeetingIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Instructors, opts => opts.MapFrom(src => src.SelectedInstructors));

            CreateMap<Meeting, MeetingDetailsViewModel>()
                     .ForMember(dest => dest.Instructors, opts => opts.Ignore())
                     .ForMember(dest => dest.AttendanceList, opts => opts.Ignore())
                     .ForMember(dest => dest.AllCourseParticipants, opts => opts.Ignore());

            CreateMap<Meeting, CheckMeetingPresenceViewModel>()
                     .ForMember(dest => dest.AttendanceList, opts => opts.Ignore())
                     .ForMember(dest => dest.AllCourseParticipants, opts => opts.Ignore());

            CreateMap<Meeting, DisplayMeetingWithoutInstructorViewModel>()
                     .ForMember(dest => dest.Course, opts => opts.Ignore());

            CreateMap<Meeting, DisplayMeetingWithoutCourseViewModel>()
                    .ForMember(dest => dest.Instructors, opts => opts.Ignore());

            CreateMap<Meeting, DisplayMeetingWithUserPresenceInformation>()
                    .ForMember(dest => dest.IsUserPresent, opts => opts.Ignore());
            #endregion

            #region Users
            CreateMap<CertificationPlatformUser, DisplayUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayCrucialDataUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<RegisterViewModel, CertificationPlatformUser>()
                     .ForMember(dest => dest.GivenCertificates, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Courses, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.GivenDegrees, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleManager, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleWorker, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.Email));

            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithCompaniesRoleUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithContactUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithBirthDateUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<CertificationPlatformUser, DisplayAllUserInformationViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.CompanyRoleManager, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleWorker, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<AddUserViewModel, CertificationPlatformUser>()
                     .ForMember(dest => dest.GivenCertificates, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Courses, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.GivenDegrees, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleManager, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleWorker, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.Email));

            CreateMap<CertificationPlatformUser, EditUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.SelectedRole, opts => opts.MapFrom(src => src.Roles))
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<EditUserViewModel, CertificationPlatformUser>()
                     .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.EmailConfirmed, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, UserDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenCertificates, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegrees, opts => opts.Ignore())
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<CertificationPlatformUser, CompanyWorkerDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenCertificates, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegrees, opts => opts.Ignore())
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<CertificationPlatformUser, AccountDetailsViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                    .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<CertificationPlatformUser, UserDetailsForAnonymousViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.GivenCertificates, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegrees, opts => opts.Ignore())
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));
           
            CreateMap<CertificationPlatformUser, DisplayUserWithCourseResultsViewModel>()
                     .ForMember(dest => dest.ExamsResults, opts => opts.MapFrom(src => new List<ExamResult>()))
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, EditAccountViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                    .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<EditAccountViewModel, CertificationPlatformUser>()
                    .ForMember(dest => dest.Id, opts => opts.Ignore())
                    .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                    .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()))
                    .ForMember(dest => dest.EmailConfirmed, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, ExaminerDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.Exams, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamsTerms, opts => opts.Ignore())
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<CertificationPlatformUser, InstructorExaminerDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.Exams, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamsTerms, opts => opts.Ignore())
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<CertificationPlatformUser, DisplayUserWithExamResults>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, MarkUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, UserIdentificatorWithQRViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, GetUserImageViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, UserGivenCertificatePossessionConfirmationViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, UserGivenDegreePossessionConfirmationViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayUserWithCourseExamPeriodsResultsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.LastingExamsIndexers, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamsIndexersPassed, opts => opts.MapFrom(src => new List<string>()));

            #endregion

            CreateMap<PersonalLogInformation, DisplayLogInformationViewModel>();

            CreateMap<PersonalLogInformation, DisplayLogInformationExtendedViewModel>();

            #region ViewModels to ViewModels 
            CreateMap<DisplayCrucialDataUserViewModel, PresenceCheckBoxViewModel>()
                     .ForMember(dest => dest.IsPresent, opts => opts.Ignore());

            CreateMap<DisplayCrucialDataUserViewModel, AddUsersFromCheckBoxViewModel>()
                     .ForMember(dest => dest.IsToAssign, opts => opts.Ignore());

            CreateMap<DisplayUserWithCourseResultsViewModel, DispenseGivenCertificateCheckBoxViewModel>()
                     .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());

            CreateMap<DisplayCrucialDataUserViewModel, DispenseGivenCertificateCheckBoxViewModel>()
                     .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, InstructorDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.Meetings, opts => opts.Ignore())
                     .ForMember(dest => dest.DateOfBirth, opts => opts.MapFrom(src => src.DateOfBirth));

            CreateMap<DisplayCrucialDataUserViewModel, DeleteUsersFromCheckBoxViewModel>()
                     .ForMember(dest => dest.IsToDelete, opts => opts.Ignore());

            CreateMap<CourseOfferDetailsViewModel, CompanyCourseOfferDetailsViewModel>();

            #endregion
        }
    }
}