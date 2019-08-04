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
                     .ForMember(dest => dest.Meetings, opts => opts.MapFrom(src => new List<string>()));

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

            CreateMap<EditGivenCertificateViewModel, GivenCertificate>();

            CreateMap<GivenCertificate, EditGivenCertificateViewModel>();
            #endregion

            #region GivenDegrees

            #endregion

            #region Meetings
            CreateMap<Meeting, DisplayMeetingViewModel>();

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

            #endregion

            #region Users
            CreateMap<CertificationPlatformUser, DisplayUserViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayCrucialDataUsersViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<RegisterViewModel, CertificationPlatformUser>()
                     .ForMember(dest => dest.Certificates, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Courses, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Degrees, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleManager, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleWorker, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.NormalizedUserName, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.NormalizedEmail, opts => opts.MapFrom(src => src.Email.ToUpper()))
                     .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.Email));

            // +/- to check
            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithCompaniesRoleViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id));

            CreateMap<CertificationPlatformUser, DisplayCrucialDataWithContactUsersViewModel>();

            CreateMap<CertificationPlatformUser, DisplayAllUserInformationViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.CompanyRoleManager, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.CompanyRoleWorker, opts => opts.MapFrom(src => new List<string>()));

            CreateMap<AddUserViewModel, CertificationPlatformUser>()
                     .ForMember(dest => dest.Certificates, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Courses, opts => opts.MapFrom(src => new List<string>()))
                     .ForMember(dest => dest.Degrees, opts => opts.MapFrom(src => new List<string>()))
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
                     .ForMember(dest => dest.Certificates, opts => opts.Ignore())
                     .ForMember(dest => dest.Companies, opts => opts.Ignore());

            CreateMap<CertificationPlatformUser, UserDetailsForAnonymousViewModel>()
                     .ForMember(dest => dest.UserIdentificator, opts => opts.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Certificates, opts => opts.Ignore());

            //....
            CreateMap<Instructor, DisplayCrucialDataWithContactUsersViewModel>();
            #endregion
        }
    }
}
