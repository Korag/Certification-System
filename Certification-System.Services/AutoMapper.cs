using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.DTOViewModels.AccountViewModels;
using Certification_System.Entities;
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
                     .ForMember(dest => dest.SelectedBranches, opts => opts.MapFrom(src => src.Branches));

            CreateMap<EditCourseWithMeetingsViewModel, Course>()
                   .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
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
                    .ForMember(dest => dest.AllCourseParticipants, opts => opts.Ignore());

            CreateMap<Course, DeleteUsersFromCourseViewModel>()
                     .ForMember(dest => dest.UsersToDeleteFromCourse, opts => opts.Ignore())
                     .ForMember(dest => dest.AllCourseParticipants, opts => opts.Ignore());
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

            CreateMap<Degree, DisplayCrucialDataDegreeViewModel>();

            CreateMap<Degree, DisplayDegreeWithoutRequirementsViewModel>();
            #endregion

            #region Exams
            CreateMap<Exam, AddExamViewModel>();

            CreateMap<Exam, AddExamWithExamTermsViewModel>();

            CreateMap<AddExamWithExamTermsViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddExamViewModel, Exam>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                    .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                    .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Exam, DisplayExamViewModel>()
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore());

            CreateMap<Exam, MarkExamViewModel>()
                    .ForMember(dest => dest.Users, opts => opts.Ignore());

            CreateMap<Exam, DisplayExamWithoutCourseViewModel>()
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore());

            CreateMap<Exam, DisplayExamWithoutExaminerViewModel>();

            CreateMap<Exam, DisplayCrucialDataExamViewModel>();

            CreateMap<Exam, ExamDetailsViewModel>()
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore())
                     .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore())
                     .ForMember(dest => dest.UsersQuantitiy, opts => opts.MapFrom(src=> src.EnrolledUsers.Count()));

            CreateMap<Exam, EditExamWithExamTermsViewModel>();

            CreateMap<EditExamWithExamTermsViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => src.DateOfEnd.Subtract(src.DateOfStart).Days))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => src.DateOfEnd.Subtract(src.DateOfStart).Minutes))
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore());

            CreateMap<Exam, EditExamViewModel>();

            CreateMap<EditExamViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.DurationDays, opts => opts.MapFrom(src => src.DateOfEnd.Subtract(src.DateOfStart).Days))
                     .ForMember(dest => dest.DurationMinutes, opts => opts.MapFrom(src => src.DateOfEnd.Subtract(src.DateOfStart).Minutes))
                     .ForMember(dest => dest.ExamTerms, opts => opts.Ignore());

            CreateMap<AddExamPeriodViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));
                   
            CreateMap<AddExamPeriodWithExamTermsViewModel, Exam>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.ExamTerms, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.ExamResults, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Exam, DeleteUsersFromExamViewModel>()
                     .ForMember(dest => dest.UsersToDeleteFromExam, opts => opts.Ignore())
                     .ForMember(dest => dest.AllExamParticipants, opts => opts.Ignore());

            CreateMap<Exam, AssignUsersFromCourseToExamViewModel>()
                    .ForMember(dest => dest.UsersToAssignToExam, opts => opts.Ignore())
                    .ForMember(dest => dest.CourseParticipants, opts => opts.Ignore());

            #endregion

            #region ExamTerms
            CreateMap<ExamTerm, AddExamTermWithoutExamViewModel>();

            CreateMap<AddExamTermWithoutExamViewModel, ExamTerm>()
                     .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddExamTermViewModel, ExamTerm>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers))
                    .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<ExamTerm, DisplayExamTermWithoutExaminerViewModel>()
                     .ForMember(dest => dest.UsersQuantitiy, opts => opts.MapFrom(src => src.EnrolledUsers.Count()));

            CreateMap<ExamTerm, DisplayExamTermViewModel>()
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()));

            CreateMap<ExamTerm, DisplayExamTermWithoutExamViewModel>()
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersQuantitiy, opts => opts.MapFrom(src => src.EnrolledUsers.Count()));

            CreateMap<EditExamTermViewModel, ExamTerm>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers));

            CreateMap<ExamTerm, EditExamTermViewModel>()
                    .ForMember(dest => dest.SelectedExaminers, opts => opts.MapFrom(src => src.Examiners));

            CreateMap<EditExamTermViewModel, ExamTerm>()
                    .ForMember(dest => dest.Examiners, opts => opts.MapFrom(src => src.SelectedExaminers));

            CreateMap<ExamTerm, ExamTermDetailsViewModel>()
                    .ForMember(dest => dest.UsersQuantity, opts => opts.MapFrom(src => src.EnrolledUsers.Count()))
                    .ForMember(dest => dest.Exam, opts => opts.Ignore())
                    .ForMember(dest => dest.Course, opts => opts.Ignore())
                    .ForMember(dest => dest.Examiners, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersWithResults, opts => opts.Ignore());

            CreateMap<Course, DeleteUsersFromExamTermViewModel>()
                    .ForMember(dest => dest.Exam, opts => opts.Ignore())
                    .ForMember(dest => dest.UsersToDeleteFromExamTerm, opts => opts.Ignore())
                    .ForMember(dest => dest.AllExamTermParticipants, opts => opts.Ignore());
            #endregion

            #region ExamResults
            CreateMap<ExamResult, AddExamResultViewModel>();

            CreateMap<ExamResult, DisplayUserWithExamResults>();

            CreateMap<ExamResult, DisplayUserWithCourseResultsViewModel>();

            CreateMap<ExamResult, DisplayUserWithExamResults>();

            CreateMap<ExamResult, DisplayExamResultViewModel>()
                     .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<ExamResult, MarkUserViewModel>();
            #endregion

            #region GivenCertificates
            CreateMap<AddGivenCertificateViewModel, GivenCertificate>();

            CreateMap<GivenCertificate, DisplayGivenCertificateViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
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
            #endregion

            #region Meetings
            CreateMap<Meeting, DisplayMeetingViewModel>()
                     .ForMember(dest => dest.Instructors, opts => opts.Ignore());

            CreateMap<AddMeetingViewModel, Meeting>()
                     .ForMember(dest => dest.MeetingIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Instructors, opts => opts.MapFrom(src => src.SelectedInstructors));

            CreateMap<AddMeetingWithoutCourseViewModel, Meeting>()
                     .ForMember(dest => dest.MeetingIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Instructors, opts => opts.MapFrom(src => src.SelectedInstructors));

            CreateMap<Meeting, EditMeetingViewModel>()
                     .ForMember(dest => dest.SelectedInstructors, opts => opts.MapFrom(src => src.Instructors));

            CreateMap<EditMeetingViewModel, Meeting>()
                     .ForMember(dest => dest.AttendanceList, opts => opts.Ignore())
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
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayAllUserInformationViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.CompanyRoleManager, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleWorker, opts => opts.MapFrom(src => new List<string>()));

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
                     .ForMember(dest => dest.SelectedRole, opts => opts.MapFrom(src => src.Roles));

            CreateMap<EditUserViewModel, CertificationPlatformUser>()
                     .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()));

            CreateMap<CertificationPlatformUser, UserDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenCertificates, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegrees, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, AccountDetailsViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, UserDetailsForAnonymousViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.GivenCertificates, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegrees, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, DisplayUserWithCourseResultsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, EditAccountViewModel>()
                    .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<EditAccountViewModel, CertificationPlatformUser>()
                    .ForMember(dest => dest.Id, opts => opts.Ignore());

            CreateMap<EditAccountViewModel, CertificationPlatformUser>()
                    .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                    .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()));

            CreateMap<CertificationPlatformUser, ExaminerDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.Exams, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamsTerms, opts => opts.Ignore());


            CreateMap<CertificationPlatformUser, InstructorExaminerDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.CoursesInstructor, opts => opts.Ignore())
                     .ForMember(dest => dest.CoursesExaminer, opts => opts.Ignore())
                     .ForMember(dest => dest.Exams, opts => opts.Ignore())
                     .ForMember(dest => dest.ExamsTerms, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, DisplayUserWithExamResults>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, MarkUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));
            #endregion

            #region ViewModels to ViewModels 
            CreateMap<DisplayCrucialDataUserViewModel, PresenceCheckBoxViewModel>()
                     .ForMember(dest => dest.IsPresent, opts => opts.Ignore());

            CreateMap<DisplayCrucialDataUserViewModel, AddUsersFromCheckBoxViewModel>()
                     .ForMember(dest => dest.IsToAssign, opts => opts.Ignore());

            CreateMap<DisplayUserWithCourseResultsViewModel, DispenseGivenCertificateCheckBoxViewModel>()
                     .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());

            //CreateMap<DisplayCrucialDataWithCompaniesRoleUserViewModel, DispenseGivenCertificateCheckBoxViewModel>()
            //         .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());

            CreateMap<DisplayCrucialDataUserViewModel, DispenseGivenCertificateCheckBoxViewModel>()
                     .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, InstructorDetailsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Courses, opts => opts.Ignore())
                     .ForMember(dest => dest.Meetings, opts => opts.Ignore());

            CreateMap<DisplayCrucialDataUserViewModel, DeleteUsersFromCheckBoxViewModel>()
                     .ForMember(dest => dest.IsToDelete, opts => opts.Ignore());
            #endregion
        }
    }
}

