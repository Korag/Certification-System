﻿using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.DTOViewModels.AccountViewModels;
using Certification_System.Entities;
using System.Collections.Generic;

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

            CreateMap<Company, CompanyDetailsViewModel>();

            CreateMap<EditCompanyViewModel, Company>();

            CreateMap<Company, EditCompanyViewModel>();
            #endregion

            #region Courses
            CreateMap<Course, DisplayCourseViewModel>();

            CreateMap<Course, DisplayCrucialDataCourseViewModel>();

            CreateMap<Course, CourseDetailsViewModel>()
                     .ForMember(dest => dest.Meetings, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore());

            CreateMap<Course, DisplayCourseWithMeetingsViewModel>()
                     .ForMember(dest => dest.Meetings, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddCourseViewModel, Course>()
                     .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches))
                     .ForMember(dest => dest.Meetings, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<Course, EditCourseViewModel>()
                      .ForMember(dest => dest.SelectedBranches, opts => opts.MapFrom(src => src.Branches));

            CreateMap<EditCourseViewModel, Course>()
                     .ForMember(dest => dest.CourseIdentificator, opts => opts.Ignore())
                     .ForMember(dest => dest.Meetings, opts => opts.Ignore())
                     .ForMember(dest => dest.EnrolledUsers, opts => opts.Ignore())
                     .ForMember(dest => dest.Branches, opts => opts.MapFrom(src => src.SelectedBranches));


            CreateMap<Course, DispenseGivenCertificatesViewModel>()
                    .ForMember(dest => dest.DispensedGivenCertificates, opts => opts.Ignore())
                    .ForMember(dest => dest.AvailableCertificates, opts => opts.Ignore())
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

            #region GivenCertificates
            CreateMap<AddGivenCertificateViewModel, GivenCertificate>();

            CreateMap<GivenCertificate, DisplayGivenCertificateViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.Course, opts => opts.Ignore())
                     .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<GivenCertificate, GivenCertificateDetailsForAnonymousViewModel>()
                    .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                    .ForMember(dest => dest.Companies, opts => opts.Ignore())
                    .ForMember(dest => dest.User, opts => opts.Ignore());

            CreateMap<GivenCertificate, GivenCertificateDetailsViewModel>()
                 .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                 .ForMember(dest => dest.Companies, opts => opts.Ignore())
                 .ForMember(dest => dest.User, opts => opts.Ignore())
                 .ForMember(dest => dest.Instructors, opts => opts.Ignore())
                 .ForMember(dest => dest.Course, opts => opts.Ignore())
                 .ForMember(dest => dest.Meetings, opts => opts.Ignore());

            CreateMap<GivenCertificate, DisplayGivenCertificateToUserViewModel>()
                     .ForMember(dest => dest.Certificate, opts => opts.Ignore())
                     .ForMember(dest => dest.Course, opts => opts.Ignore());

            CreateMap<GivenCertificate, DisplayGivenCertificateToUserWithoutCourseViewModel>()
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


            CreateMap<GivenDegree, GivenDegreeDetailsViewModel>()
                     .ForMember(dest => dest.Degree, opts => opts.Ignore())
                     .ForMember(dest => dest.RequiredDegreesWithGivenInstances, opts => opts.Ignore())
                     .ForMember(dest => dest.RequiredCertificatesWithGivenInstances, opts => opts.Ignore())
                     .ForMember(dest => dest.User, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore());

            CreateMap<GivenDegree, GivenDegreeDetailsForAnonymousViewModel>()
                  .ForMember(dest => dest.Degree, opts => opts.Ignore())
                  .ForMember(dest => dest.User, opts => opts.Ignore())
                  .ForMember(dest => dest.Companies, opts => opts.Ignore());
            #endregion

            #region Meetings
            CreateMap<Meeting, DisplayMeetingViewModel>();

            CreateMap<Meeting, DisplayMeetingWithInstructorsViewModel>();
   
            CreateMap<AddMeetingViewModel, Meeting>()
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

            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithContactUserViewModel>();

            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithBirthDateUserViewModel>();

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

            CreateMap<CertificationPlatformUser, UserDetailsForAnonymousViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.GivenCertificates, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore())
                     .ForMember(dest => dest.GivenDegrees, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, DisplayUserWithCourseResultsViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));
            #endregion

            #region ViewModels to ViewModels 
            CreateMap<DisplayCrucialDataUserViewModel, PresenceCheckBoxViewModel>()
                     .ForMember(dest => dest.IsPresent, opts => opts.Ignore());
       
            CreateMap<DisplayUserWithCourseResultsViewModel, DispenseGivenCertificateCheckBoxViewModel>()
                     .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());

            CreateMap<DisplayCrucialDataWithCompaniesRoleUserViewModel, DispenseGivenCertificateCheckBoxViewModel>()
              .ForMember(dest => dest.GivenCertificateIsEarned, opts => opts.Ignore());
            #endregion
        }
    }
}

 
