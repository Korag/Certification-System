using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Extensions;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Certification_System.Controllers
{
    public class ExamsController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public ExamsController(
            MongoOperations context,
            IMapper mapper,
            IKeyGenerator keyGenerator,
            ILogService logger,
            IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
            _logger = logger;
            _emailSender = emailSender;
        }

        // GET: AddNewExam
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewExam(string courseIdentificator)
        {
            AddExamViewModel newExam = new AddExamViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
                AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList()
            };

            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                newExam.SelectedCourse = courseIdentificator;
            }

            return View(newExam);
        }

        // POST: AddNewExam
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewExam(AddExamViewModel newExam)
        {
            if (ModelState.IsValid)
            {
                var course = _context.courseRepository.GetCourseById(newExam.SelectedCourse);

                Exam exam = _mapper.Map<Exam>(newExam);
                exam.ExamIdentificator = _keyGenerator.GenerateNewId();
                exam.ExamIndexer = _keyGenerator.GenerateExamEntityIndexer(exam.Name);

                course.Exams.Add(exam.ExamIdentificator);

                exam.OrdinalNumber = 1;

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                #region EntityLogs

                var logInfoAddExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExam"]);
                _logger.AddExamLog(exam, logInfoAddExam);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addExamToCourse"]);
                _logger.AddCourseLog(course, logInfoUpdateCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExam"], "Indekser " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExam);

                var logInfoPersonalAddExamToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamToCourse"], "Indekser " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamToCourse);

                var logInfoPersonalAddExaminersToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExaminerToExam"], "Indekser " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(exam.Examiners, logInfoPersonalAddExaminersToExam);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExam.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            newExam.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            return View(newExam);
        }

        // GET: AddNewExamWithExamTerms
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewExamWithExamTerms(string courseIdentificator, int quantityOfExamTerms)
        {
            AddExamWithExamTermsViewModel newExam = new AddExamWithExamTermsViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
                AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList(),
                ExamDividedToTerms = true,
                ExamTerms = new List<AddExamTermWithoutExamViewModel>()
            };

            for (int i = 0; i < quantityOfExamTerms; i++)
            {
                newExam.ExamTerms.Add(new AddExamTermWithoutExamViewModel());
            }

            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                newExam.SelectedCourse = courseIdentificator;
            }

            return View(newExam);
        }

        // POST: AddNewExamWithExamTerms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewExamWithExamTerms(AddExamWithExamTermsViewModel newExam)
        {
            if (ModelState.IsValid)
            {
                var course = _context.courseRepository.GetCourseById(newExam.SelectedCourse);

                Exam exam = _mapper.Map<Exam>(newExam);
                exam.ExamIdentificator = _keyGenerator.GenerateNewId();
                exam.ExamIndexer = _keyGenerator.GenerateExamEntityIndexer(exam.Name);

                course.Exams.Add(exam.ExamIdentificator);

                exam.OrdinalNumber = 1;
                exam.UsersLimit = newExam.ExamTerms.Select(z => z.UsersLimit).Sum();
                exam.ExamDividedToTerms = true;

                exam.Examiners = newExam.ExamTerms.SelectMany(z => z.SelectedExaminers).Distinct().ToList();

                List<ExamTerm> examsTerms = new List<ExamTerm>();

                if (newExam.ExamTerms.Count() != 0)
                {
                    foreach (var newExamTerm in newExam.ExamTerms)
                    {
                        ExamTerm examTerm = _mapper.Map<ExamTerm>(newExamTerm);
                        examTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();
                        examTerm.ExamTermIndexer = _keyGenerator.GenerateExamTermEntityIndexer(exam.ExamIndexer);

                        exam.ExamTerms.Add(examTerm.ExamTermIdentificator);
                        examsTerms.Add(examTerm);

                        #region PersonalUserLogs

                        var logInfoPersonalAddExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamTerm"], "Indekser " + examTerm.ExamTermIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamTerm);

                        var logInfoPersonalAddExamTermToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamTermToExam"], "Indekser " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamTermToExam);

                        var logInfoPersonalAddExaminersToExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExaminerToExamTerm"], "Indekser " + examTerm.ExamTermIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(exam.Examiners, logInfoPersonalAddExaminersToExamTerm);

                        #endregion
                    }

                    _context.examTermRepository.AddExamsTerms(examsTerms);

                    #region EntityLogs

                    var logInfoAddExamsTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExamTerm"]);
                    _logger.AddExamsTermsLogs(examsTerms, logInfoAddExamsTerms);

                    #endregion
                }

                var ExamsTermsIdentificators = examsTerms.Select(z => z.ExamTermIdentificator);

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                #region EntityLogs

                var logInfoAddExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExam"]);
                _logger.AddExamLog(exam, logInfoAddExam);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addExamToCourse"]);
                _logger.AddCourseLog(course, logInfoUpdateCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExam"], "Indekser " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExam);

                var logInfoPersonalAddExamToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamToCourse"], "Indekser " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamToCourse);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExam.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            newExam.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            return View(newExam);
        }

        // GET: AddNewExamPeriod
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewExamPeriod(string courseIdentificator, string examIdentificator)
        {
            AddExamPeriodViewModel newExamPeriod = new AddExamPeriodViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList(),
                AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList(),

                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
            };

            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                newExamPeriod.SelectedCourse = courseIdentificator;

                var course = _context.courseRepository.GetCourseById(newExamPeriod.SelectedCourse);
                newExamPeriod.AvailableExams = _context.examRepository.GetExamsByIdAsSelectList(course.Exams);
            }
            if (!string.IsNullOrWhiteSpace(courseIdentificator) && !string.IsNullOrWhiteSpace(examIdentificator))
            {
                newExamPeriod.SelectedExam = examIdentificator;
            }

            return View(newExamPeriod);
        }

        // POST: AddNewExamPeriod
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewExamPeriod(AddExamPeriodViewModel newExamPeriod)
        {
            if (ModelState.IsValid)
            {
                var course = _context.courseRepository.GetCourseById(newExamPeriod.SelectedCourse);
                var examsInCourse = _context.examRepository.GetExamsById(course.Exams);

                Exam exam = _mapper.Map<Exam>(newExamPeriod);
                exam.ExamIdentificator = _keyGenerator.GenerateNewId();

                var firstExamPeriod = examsInCourse.Where(z => z.ExamIdentificator == newExamPeriod.SelectedExam).OrderBy(z => z.OrdinalNumber).FirstOrDefault();

                exam.ExamIndexer = firstExamPeriod.ExamIndexer;
                exam.Name = firstExamPeriod.Name;
                exam.Description = firstExamPeriod.Description;

                course.Exams.Add(exam.ExamIdentificator);

                exam.OrdinalNumber = examsInCourse.Where(z => z.ExamIndexer == exam.ExamIndexer).Count() + 1;

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                #region EntityLogs

                var logInfoAddExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExam"]);
                _logger.AddExamLog(exam, logInfoAddExam);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addExamToCourse"]);
                _logger.AddCourseLog(course, logInfoUpdateCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExam"], "Indekser " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExam);

                var logInfoPersonalAddExamToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamToCourse"], "Indekser " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamToCourse);

                var logInfoPersonalAddExaminersToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExaminerToExam"], "Indekser " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(exam.Examiners, logInfoPersonalAddExaminersToExam);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExamPeriod.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExamPeriod.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();
            newExamPeriod.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            newExamPeriod.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExamPeriod);
        }

        // GET: AddNewExamPeriodWithExamTerms
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewExamPeriodWithExamTerms(string courseIdentificator, string examIdentificator, int quantityOfExamTerms)
        {
            AddExamPeriodWithExamTermsViewModel newExamPeriod = new AddExamPeriodWithExamTermsViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetFirstPeriodExamsAsSelectList().ToList(),
                AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList(),

                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),

                ExamDividedToTerms = true,
                ExamTerms = new List<AddExamTermWithoutExamViewModel>()
            };

            for (int i = 0; i < quantityOfExamTerms; i++)
            {
                newExamPeriod.ExamTerms.Add(new AddExamTermWithoutExamViewModel());
            }

            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                newExamPeriod.SelectedCourse = courseIdentificator;

                var course = _context.courseRepository.GetCourseById(newExamPeriod.SelectedExam);
                newExamPeriod.AvailableExams = _context.examRepository.GetExamsByIdAsSelectList(course.Exams);
            }
            if (!string.IsNullOrWhiteSpace(courseIdentificator) && !string.IsNullOrWhiteSpace(examIdentificator))
            {
                newExamPeriod.SelectedExam = examIdentificator;
            }

            return View(newExamPeriod);
        }

        // POST: AddNewExamPeriodWithExamTerms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewExamPeriodWithExamTerms(AddExamPeriodWithExamTermsViewModel newExamPeriod)
        {
            if (ModelState.IsValid)
            {
                var course = _context.courseRepository.GetCourseById(newExamPeriod.SelectedCourse);
                var examsInCourse = _context.examRepository.GetExamsById(course.Exams);

                Exam exam = _mapper.Map<Exam>(newExamPeriod);
                exam.ExamIdentificator = _keyGenerator.GenerateNewId();

                var firstExamPeriod = examsInCourse.Where(z => z.ExamIdentificator == newExamPeriod.SelectedExam).OrderBy(z => z.OrdinalNumber).FirstOrDefault();

                exam.ExamIndexer = firstExamPeriod.ExamIndexer;
                exam.Name = firstExamPeriod.Name;
                exam.Description = firstExamPeriod.Description;

                course.Exams.Add(exam.ExamIdentificator);

                exam.OrdinalNumber = examsInCourse.Where(z => z.ExamIndexer == exam.ExamIndexer).Count();
                exam.UsersLimit = newExamPeriod.ExamTerms.Select(z => z.UsersLimit).Sum();
                exam.ExamDividedToTerms = true;

                exam.Examiners = newExamPeriod.ExamTerms.SelectMany(z => z.SelectedExaminers).Distinct().ToList();

                List<ExamTerm> examsTerms = new List<ExamTerm>();

                if (newExamPeriod.ExamTerms.Count() != 0)
                {
                    foreach (var newExamTerm in newExamPeriod.ExamTerms)
                    {
                        ExamTerm examTerm = _mapper.Map<ExamTerm>(newExamTerm);
                        examTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();
                        examTerm.ExamTermIndexer = _keyGenerator.GenerateExamTermEntityIndexer(exam.ExamIndexer);

                        exam.ExamTerms.Add(examTerm.ExamTermIdentificator);
                        examsTerms.Add(examTerm);

                        #region PersonalUserLogs

                        var logInfoPersonalAddExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamTerm"], "Indekser " + examTerm.ExamTermIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamTerm);

                        var logInfoPersonalAddExamTermToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamTermToExam"], "Indekser " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamTermToExam);

                        var logInfoPersonalAddExaminersToExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExaminerToExamTerm"], "Indekser " + examTerm.ExamTermIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(exam.Examiners, logInfoPersonalAddExaminersToExamTerm);

                        #endregion
                    }

                    _context.examTermRepository.AddExamsTerms(examsTerms);

                    #region EntityLogs

                    var logInfoAddExamsTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExamTerm"]);
                    _logger.AddExamsTermsLogs(examsTerms, logInfoAddExamsTerms);

                    #endregion
                }

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                #region EntityLogs

                var logInfoAddExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExam"]);
                _logger.AddExamLog(exam, logInfoAddExam);

                var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addExamToCourse"]);
                _logger.AddCourseLog(course, logInfoUpdateExam);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalAddExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExam"], "Indekser " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExam);

                var logInfoPersonalAddExamToCourse = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExamToCourse"], "Indekser " + course.CourseIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddExamToCourse);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExamPeriod.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExamPeriod.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();
            newExamPeriod.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            newExamPeriod.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExamPeriod);
        }

        // GET: DisplayAllExams
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllExams(string message = null)
        {
            ViewBag.Message = message;

            var exams = _context.examRepository.GetListOfExams();
            var courses = _context.courseRepository.GetListOfCourses();

            List<DisplayExamViewModel> listOfExams = new List<DisplayExamViewModel>();

            foreach (var course in courses)
            {
                foreach (var exam in course.Exams)
                {
                    Exam examModel = exams.Where(z => z.ExamIdentificator == exam).FirstOrDefault();

                    DisplayExamViewModel singleExam = _mapper.Map<DisplayExamViewModel>(examModel);
                    singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examModel.Examiners));
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    listOfExams.Add(singleExam);
                }
            }

            return View(listOfExams);
        }

        // GET: ExamDetails
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult ExamDetails(string examIdentificator, string message)
        {
            ViewBag.Message = message;

            var exam = _context.examRepository.GetExamById(examIdentificator);
            var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

            List<DisplayExamTermWithoutExamViewModel> listOfExamTerms = new List<DisplayExamTermWithoutExamViewModel>();

            foreach (var examTerm in examTerms)
            {
                DisplayExamTermWithoutExamViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExamViewModel>(examTerm);
                singleExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examTerm.Examiners));

                listOfExamTerms.Add(singleExamTerm);
            }

            var examiners = _context.userRepository.GetUsersById(exam.Examiners);
            List<DisplayCrucialDataWithContactUserViewModel> listOfExaminers = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(examiners);

            var users = _context.userRepository.GetUsersById(exam.EnrolledUsers);
            List<DisplayCrucialDataWithContactUserViewModel> listOfUsers = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(users);

            var course = _context.courseRepository.GetCourseByExamId(examIdentificator);
            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            ExamDetailsViewModel examDetails = _mapper.Map<ExamDetailsViewModel>(exam);
            examDetails.ExamTerms = listOfExamTerms;
            examDetails.Course = courseViewModel;

            examDetails.Examiners = listOfExaminers;
            examDetails.EnrolledUsers = listOfUsers;

            if (this.User.IsInRole("Examiner"))
            {
                return View("ExaminerExamDetails", examDetails);
            }

            return View(examDetails);
        }

        // GET: ConfirmationOfActionOnExam
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnExam(string examIdentificator, string TypeOfAction)
        {
            if (examIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var exam = _context.examRepository.GetExamById(examIdentificator);
                var examiners = _context.userRepository.GetUsersById(exam.Examiners);

                var course = _context.courseRepository.GetCourseByExamId(examIdentificator);

                DisplayExamWithTermsAndLocationViewModel modifiedExam = new DisplayExamWithTermsAndLocationViewModel();

                modifiedExam.Exam = _mapper.Map<DisplayExamWithLocationViewModel>(exam);
                modifiedExam.ExamTerms = new List<DisplayExamTermWithoutExamWithLocationViewModel>();

                if (exam.ExamTerms.Count() != 0)
                {
                    var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                    modifiedExam.ExamTerms = _mapper.Map<List<DisplayExamTermWithoutExamWithLocationViewModel>>(examTerms);

                    foreach (var examTerm in modifiedExam.ExamTerms)
                    {
                        var singleExamTermExaminers = _context.userRepository.GetUsersById(examTerms.Where(z => z.ExamTermIdentificator == examTerm.ExamTermIdentificator).Select(z => z.Examiners).FirstOrDefault());

                        examTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(singleExamTermExaminers);
                    }
                }

                modifiedExam.Exam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);
                modifiedExam.Exam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(examiners);

                return View(modifiedExam);
            }

            return RedirectToAction(nameof(AddNewExam));
        }

        // GET: EditExamHub
        [Authorize(Roles = "Admin")]
        public ActionResult EditExamHub(string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam.ExamDividedToTerms && exam.ExamTerms.Count() != 0)
            {
                return RedirectToAction("EditExamWithExamTerms", "Exams", new { examIdentificator = examIdentificator });
            }
            else
            {
                return RedirectToAction("EditExam", "Exams", new { examIdentificator = examIdentificator });
            }
        }

        // GET: AddExamMenu
        [Authorize(Roles = "Admin")]
        public ActionResult AddExamMenu(string courseIdentificator, string examIdentificator)
        {
            AddExamTypeOfActionViewModel addExamType = new AddExamTypeOfActionViewModel
            {
                AvailableOptions = _context.examRepository.GetAddExamMenuOptions().ToList(),

                CourseIdentificator = courseIdentificator,
                ExamIdentificator = examIdentificator
            };

            return View(addExamType);
        }

        // POST: AddExamMenu
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddExamMenu(AddExamTypeOfActionViewModel addExamTypeOfAction)
        {
            if (ModelState.IsValid)
            {
                if (addExamTypeOfAction.SelectedOption == "addExam" && addExamTypeOfAction.ExamTermsQuantity == 0)
                    return RedirectToAction("AddNewExam", "Exams", new { courseIdentificator = addExamTypeOfAction.CourseIdentificator, examIdentificator = addExamTypeOfAction.ExamIdentificator });

                else if (addExamTypeOfAction.SelectedOption == "addExam" && addExamTypeOfAction.ExamTermsQuantity != 0)
                    return RedirectToAction("AddNewExamWithExamTerms", "Exams", new { courseIdentificator = addExamTypeOfAction.CourseIdentificator, examIdentificator = addExamTypeOfAction.ExamIdentificator, quantityOfExamTerms = addExamTypeOfAction.ExamTermsQuantity });

                else if (addExamTypeOfAction.SelectedOption == "addExamPeriod" && addExamTypeOfAction.ExamTermsQuantity == 0)
                    return RedirectToAction("AddNewExamPeriod", "Exams", new { courseIdentificator = addExamTypeOfAction.CourseIdentificator, examIdentificator = addExamTypeOfAction.ExamIdentificator, quantityOfExamTerms = addExamTypeOfAction.ExamTermsQuantity });

                else if (addExamTypeOfAction.SelectedOption == "addExamPeriod" && addExamTypeOfAction.ExamTermsQuantity != 0)
                    return RedirectToAction("AddNewExamPeriodWithExamTerms", "Exams", new { courseIdentificator = addExamTypeOfAction.CourseIdentificator, examIdentificator = addExamTypeOfAction.ExamIdentificator, quantityOfExamTerms = addExamTypeOfAction.ExamTermsQuantity });

                return RedirectToAction("BlankMenu", "Certificates");
            }

            addExamTypeOfAction.AvailableOptions = _context.examRepository.GetAddExamMenuOptions().ToList();

            return View(addExamTypeOfAction);
        }

        // GET: EditExam
        [Authorize(Roles = "Admin")]
        public ActionResult EditExam(string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);
            var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

            EditExamViewModel examToUpdate = _mapper.Map<EditExamViewModel>(exam);

            examToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examToUpdate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            examToUpdate.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            examToUpdate.SelectedExaminers = _context.userRepository.GetUsersById(exam.Examiners).Select(z => z.Id).ToList();
            examToUpdate.SelectedCourse = course.CourseIdentificator;

            return View(examToUpdate);
        }

        // POST: EditExam
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditExam(EditExamViewModel editedExam)
        {
            var originExam = _context.examRepository.GetExamById(editedExam.ExamIdentificator);
            var originExamTerms = _context.examTermRepository.GetExamsTermsById(originExam.ExamTerms);

            var originExamExaminers = originExam.Examiners.ToList();

            if (ModelState.IsValid)
            {
                if (!editedExam.ExamDividedToTerms)
                {
                    originExam.ExamTerms.Clear();

                    _context.examTermRepository.DeleteExamsTerms(originExamTerms.Select(z => z.ExamTermIdentificator).ToList());
                }

                originExam = _mapper.Map<EditExamViewModel, Exam>(editedExam, originExam);

                if (editedExam.ExamDividedToTerms)
                {
                    originExam.UsersLimit = 0;
                    originExam.Examiners.Clear();
                }

                _context.examRepository.UpdateExam(originExam);

                #region EntityLogs

                var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateExam"]);
                _logger.AddExamLog(originExam, logInfoUpdateExam);

                #endregion

                #region PesonalUserLogs

                var logInfoPersonalUpdateExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateExam"], "Indekser: " + originExam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateExam);

                var addedExaminers = originExam.Examiners.Except(originExamExaminers).ToList();
                var removedExaminers = originExamExaminers.Except(originExam.Examiners).ToList();
                var stableExaminers = originExamExaminers.Intersect(originExam.Examiners).ToList();

                var logInfoPersonalUpdateUserExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserExam"], "Indekser: " + originExam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(originExam.EnrolledUsers, logInfoPersonalUpdateUserExam);
                _context.personalLogRepository.AddPersonalUsersLogs(stableExaminers, logInfoPersonalUpdateUserExam);

                var logInfoPersonalAddExaminersToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExaminerToExam"], "Indekser " + originExam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(addedExaminers, logInfoPersonalAddExaminersToExam);

                var logInfoPersonalRemoveExaminersFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeExaminerFromExam"], "Indekser " + originExam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(removedExaminers, logInfoPersonalRemoveExaminersFromExam);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnExam", "Exams", new { examIdentificator = editedExam.ExamIdentificator, TypeOfAction = "Update" });
            }

            editedExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            editedExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            editedExam.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            return View(editedExam);
        }

        // GET: EditExamWithExamTerms
        [Authorize(Roles = "Admin")]
        public ActionResult EditExamWithExamTerms(string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);
            var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
            var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

            EditExamWithExamTermsViewModel examToUpdate = _mapper.Map<EditExamWithExamTermsViewModel>(exam);
            examToUpdate.ExamTerms = _mapper.Map<List<EditExamTermWithoutCourseViewModel>>(examTerms);

            examToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examToUpdate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            examToUpdate.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            examToUpdate.SelectedCourse = course.CourseIdentificator;

            return View(examToUpdate);
        }

        // POST: EditExamWithExamTerms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditExamWithExamTerms(EditExamWithExamTermsViewModel editedExam)
        {
            var originExam = _context.examRepository.GetExamById(editedExam.ExamIdentificator);
            var originExamTerms = _context.examTermRepository.GetExamsTermsById(originExam.ExamTerms).ToList();

            if (ModelState.IsValid)
            {
                originExam = _mapper.Map<EditExamWithExamTermsViewModel, Exam>(editedExam, originExam);

                if (!editedExam.ExamDividedToTerms)
                {
                    originExam.ExamTerms.Clear();

                    _context.examTermRepository.DeleteExamsTerms(originExamTerms.Select(z => z.ExamTermIdentificator).ToList());

                    var logInfoDeleteExamTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExamTerm"]);
                    _logger.AddExamsTermsLogs(originExamTerms, logInfoDeleteExamTerms);
                }
                else if (editedExam.ExamDividedToTerms)
                {
                    ICollection<PersonalLogInformation> examTermLogInformation = new List<PersonalLogInformation>();
                    ICollection<PersonalLogInformation> examTermUserLogInformation = new List<PersonalLogInformation>();

                    for (int i = 0; i < originExamTerms.Count(); i++)
                    {
                        var originMeetingExaminers = originExamTerms[i].Examiners;

                        originExamTerms[i] = _mapper.Map<EditExamTermWithoutCourseViewModel, ExamTerm>(editedExam.ExamTerms[i], originExamTerms[i]);

                        #region PesonalUserLogs

                        examTermLogInformation.Add(_context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateExamTerm"], "Indekser: " + originExamTerms[i].ExamTermIndexer));
                        examTermUserLogInformation.Add(_context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserExamTerm"], "Indekser: " + originExamTerms[i].ExamTermIndexer));

                        _context.personalLogRepository.AddPersonalUsersLogs(originExamTerms[i].Examiners, _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserExamTerm"], "Indekser: " + originExamTerms[i].ExamTermIndexer));

                        var addedExaminers = originExamTerms[i].Examiners.Except(originMeetingExaminers).ToList();
                        var removedExaminers = originMeetingExaminers.Except(originExamTerms[i].Examiners).ToList();

                        var logInfoPersonalAdExaminerToExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addExaminerToExamTerm"], "Indekser " + originExamTerms[i].ExamTermIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(addedExaminers, logInfoPersonalAdExaminerToExamTerm);

                        var logInfoPersonalRemoveExaminerFromExamTerm = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeExaminerFromExamTerm"], "Indekser " + originExamTerms[i].ExamTermIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(removedExaminers, logInfoPersonalRemoveExaminerFromExamTerm);

                        #endregion
                    }

                    originExam.UsersLimit = editedExam.ExamTerms.Select(z => z.UsersLimit).Sum();
                    originExam.ExamDividedToTerms = true;

                    originExam.Examiners = editedExam.ExamTerms.SelectMany(z => z.SelectedExaminers).Distinct().ToList();
                    originExam.ExamTerms = editedExam.ExamTerms.Select(z => z.ExamTermIdentificator).ToList();

                    _context.examTermRepository.UpdateExamsTerms(originExamTerms);

                    #region EntityLogs

                    var logInfoUpdateExamTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateExamTerm"]);
                    _logger.AddExamsTermsLogs(originExamTerms, logInfoUpdateExamTerms);

                    #endregion
                }

                _context.examRepository.UpdateExam(originExam);

                #region EntityLogs

                var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateExam"]);
                _logger.AddExamLog(originExam, logInfoUpdateExam);

                #endregion

                #region PesonalUserLogs

                var logInfoPersonalUpdateExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateExam"], "Indekser: " + originExam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateExam);

                var logInfoPersonalUpdateUserExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserExam"], "Indekser: " + originExam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(originExam.EnrolledUsers, logInfoPersonalUpdateUserExam);
                _context.personalLogRepository.AddPersonalUsersLogs(originExam.Examiners, logInfoPersonalUpdateUserExam);

                #endregion

                return RedirectToAction("ConfirmationOfActionOnExam", "Exams", new { examIdentificator = editedExam.ExamIdentificator, TypeOfAction = "Update" });
            }

            editedExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            editedExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            editedExam.AvailableExamTypes = _context.examRepository.GetExamsTypesAsSelectList();

            return View(editedExam);
        }

        // GET: DisplayAllExamsResults
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllExamsResults(string message = null)
        {
            ViewBag.Message = message;

            var examsResults = _context.examResultRepository.GetListOfExamsResults();

            List<DisplayExamResultViewModel> listOfExamsResults = new List<DisplayExamResultViewModel>();

            foreach (var examResult in examsResults)
            {
                DisplayExamResultViewModel singleExamResult = _mapper.Map<DisplayExamResultViewModel>(examResult);

                var relatedExam = _context.examRepository.GetExamByExamResultId(examResult.ExamResultIdentificator);
                var relatedCourse = _context.courseRepository.GetCourseByExamId(relatedExam.ExamIdentificator);
                var relatedUser = _context.userRepository.GetUserById(examResult.User);

                singleExamResult.MaxAmountOfPointsToEarn = relatedExam.MaxAmountOfPointsToEarn;

                singleExamResult.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(relatedExam);
                singleExamResult.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(relatedCourse);
                singleExamResult.User = _mapper.Map<DisplayCrucialDataUserViewModel>(relatedUser);

                listOfExamsResults.Add(singleExamResult);
            }

            return View(listOfExamsResults);
        }

        // GET: AssignUserToExam
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserToExam(string userIdentificator)
        {
            string chosenUser = null;

            if (!string.IsNullOrWhiteSpace(userIdentificator))
                chosenUser = _context.userRepository.GetUserById(userIdentificator).Id;

            var availableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            var availableExams = _context.examRepository.GetActiveExamsWithVacantSeatsAsSelectList().ToList();

            AssignUserToExamViewModel userToAssignToExam = new AssignUserToExamViewModel
            {
                AvailableExams = availableExams,

                AvailableUsers = availableUsers,
                SelectedUser = chosenUser
            };

            return View(userToAssignToExam);
        }

        // POST: AssignUserToExam
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AssignUserToExam(AssignUserToExamViewModel userAssignedToExam)
        {
            if (ModelState.IsValid)
            {
                var exam = _context.examRepository.GetExamById(userAssignedToExam.SelectedExam);
                var user = _context.userRepository.GetUserById(userAssignedToExam.SelectedUser);

                if (!exam.EnrolledUsers.Contains(userAssignedToExam.SelectedUser))
                {
                    if (exam.ExamDividedToTerms)
                    {
                        return RedirectToAction("AssignUserToExamTerm", "ExamsTerms", new { examIdentificator = userAssignedToExam.SelectedExam, userIdentificator = userAssignedToExam.SelectedUser });
                    }

                    var vacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();

                    if (vacantSeats < 1)
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranego użytkownika");
                    }
                    else
                    {
                        _context.examRepository.AddUserToExam(userAssignedToExam.SelectedExam, userAssignedToExam.SelectedUser);

                        #region EntityLogs

                        var logInfoAddUsersToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["addUsersToExam"]);
                        _logger.AddExamLog(exam, logInfoAddUsersToExam);

                        #endregion

                        #region PersonalUserLogs

                        var logInfoPersonalAddUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserToExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserToExam);

                        var logInfoPersonalAssignUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLog(userAssignedToExam.SelectedUser, logInfoPersonalAssignUserToExam);

                        #endregion

                        return RedirectToAction("ExamDetails", new { examIdentificator = userAssignedToExam.SelectedExam, message = "Zapisano nowego użytkownika na egzamin" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", user.FirstName + " " + user.LastName + " już jest zapisany/a na wybrany egzamin");
                }
            }

            userAssignedToExam.AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            userAssignedToExam.AvailableExams = _context.examRepository.GetActiveExamsWithVacantSeatsAsSelectList().ToList();

            return View(userAssignedToExam);
        }

        // GET: DeleteUsersFromExam
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromExam(string examIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam.DateOfStart > DateTime.Now)
            {
                var enrolledUsersList = _context.userRepository.GetUsersById(exam.EnrolledUsers);

                List<DisplayCrucialDataUserViewModel> listOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (enrolledUsersList.Count != 0)
                {
                    listOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(enrolledUsersList);
                }

                DeleteUsersFromExamViewModel deleteUsersFromExamViewModel = _mapper.Map<DeleteUsersFromExamViewModel>(exam);
                deleteUsersFromExamViewModel.AllExamParticipants = listOfUsers;

                deleteUsersFromExamViewModel.UsersToDeleteFromExam = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(listOfUsers);

                return View(deleteUsersFromExamViewModel);
            }

            return RedirectToAction("ExamDetails", new { examIdentificator = examIdentificator });
        }

        // POST: DeleteUsersFromExam
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromExam(DeleteUsersFromExamViewModel deleteUsersFromExamViewModel)
        {
            if (ModelState.IsValid)
            {
                var usersToDeleteFromExamIdentificators = deleteUsersFromExamViewModel.UsersToDeleteFromExam.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                if (deleteUsersFromExamViewModel.DateOfStart > DateTime.Now && usersToDeleteFromExamIdentificators.Count() != 0)
                {
                    var exam = _context.examRepository.GetExamById(deleteUsersFromExamViewModel.ExamIdentificator);
                    _context.examRepository.DeleteUsersFromExam(deleteUsersFromExamViewModel.ExamIdentificator, usersToDeleteFromExamIdentificators);

                    #region EntityLogs

                    var logInfoDeleteUsersFromExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUsersFromExam"]);
                    _logger.AddExamLog(exam, logInfoDeleteUsersFromExam);

                    #endregion

                    #region PersonalUserLogs

                    var logInfoPersonalDeleteGroupOfUsersFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeGroupOfUsersFromExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGroupOfUsersFromExam);

                    var logInfoPersonalRemoveUserFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["axUserFromExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(usersToDeleteFromExamIdentificators, logInfoPersonalRemoveUserFromExam);

                    #endregion

                    if (deleteUsersFromExamViewModel.ExamDividedToTerms)
                    {
                        var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

                        _context.examTermRepository.DeleteUsersFromExamTerms(exam.ExamTerms, usersToDeleteFromExamIdentificators);

                        #region EntityLogs

                        var logInfoDeleteUsersFromExamTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUsersFromExamTerm"]);
                        _logger.AddExamsTermsLogs(examTerms, logInfoDeleteUsersFromExamTerms);

                        #endregion
                    }

                    if (deleteUsersFromExamViewModel.UsersToDeleteFromExam.Count() == 1)
                    {
                        return RedirectToAction("UserDetails", "Users", new { userIdentificator = deleteUsersFromExamViewModel.UsersToDeleteFromExam.FirstOrDefault().UserIdentificator, message = "Usunięto użytkownika z egzaminu" });
                    }
                    else
                    {
                        return RedirectToAction("ExamDetails", new { examIdentificator = deleteUsersFromExamViewModel.ExamIdentificator, message = "Usunięto grupę użytkowników z egzaminu" });
                    }
                }

                return RedirectToAction("ExamDetails", new { examIdentificator = deleteUsersFromExamViewModel.ExamIdentificator });
            }

            return RedirectToAction("DeleteUsersFromExam", new { examIdentificator = deleteUsersFromExamViewModel.ExamIdentificator });
        }

        // GET: AssignUsersFromCourseToExam
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUsersFromCourseToExam(string examIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam.DateOfStart > DateTime.Now)
            {
                var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

                var usersNotEnrolledInExamIdentificators = course.EnrolledUsers.Where(z => !exam.EnrolledUsers.Contains(z)).ToList();

                List<string> usersWithPassedExamIdentificators = new List<string>();

                if (exam.OrdinalNumber != 1)
                {
                    var previousExamPeriods = _context.examRepository.GetExamPeriods(exam.ExamIndexer);

                    foreach (var examPeriod in previousExamPeriods)
                    {
                        var examResults = _context.examResultRepository.GetExamsResultsById(examPeriod.ExamResults);

                        var usersWithPassedExamInThatPeriod = examResults.Where(z => z.ExamPassed).Select(z => z.User).ToList();

                        if (usersWithPassedExamInThatPeriod.Count() != 0)
                        {
                            usersWithPassedExamIdentificators.AddRange(usersWithPassedExamInThatPeriod);
                        }
                    }
                }

                var usersNotEnrolledInWithoutPassedExamIdentificators = usersNotEnrolledInExamIdentificators.Where(z => !usersWithPassedExamIdentificators.Contains(z)).ToList();
                var usersNotEnrolledInWithoutPassedExam = _context.userRepository.GetUsersById(usersNotEnrolledInWithoutPassedExamIdentificators);

                List<DisplayCrucialDataUserViewModel> listOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (usersNotEnrolledInWithoutPassedExam.Count != 0)
                {
                    listOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(usersNotEnrolledInWithoutPassedExam);

                    var vacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();

                    AssignUsersFromCourseToExamViewModel addUsersToExamViewModel = _mapper.Map<AssignUsersFromCourseToExamViewModel>(exam);
                    addUsersToExamViewModel.CourseParticipants = listOfUsers;
                    addUsersToExamViewModel.VacantSeats = vacantSeats;

                    addUsersToExamViewModel.UsersToAssignToExam = _mapper.Map<AddUsersFromCheckBoxViewModel[]>(listOfUsers);

                    return View(addUsersToExamViewModel);
                }
                return RedirectToAction("ExamDetails", new { examIdentificator = examIdentificator, message = "Wszyscy nieposiadający zaliczonego egzaminu zostali już na niego zapisani" });
            }

            return RedirectToAction("ExamDetails", new { examIdentificator = examIdentificator });
        }

        // POST: AssignUsersFromCourseToExam
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUsersFromCourseToExam(AssignUsersFromCourseToExamViewModel addUsersToExamViewModel)
        {
            if (ModelState.IsValid)
            {
                if (addUsersToExamViewModel.DateOfStart > DateTime.Now)
                {
                    var exam = _context.examRepository.GetExamById(addUsersToExamViewModel.ExamIdentificator);

                    var usersToAddToExamIdentificators = addUsersToExamViewModel.UsersToAssignToExam.ToList().Where(z => z.IsToAssign == true).Select(z => z.UserIdentificator).ToList();

                    if (usersToAddToExamIdentificators.Count() <= addUsersToExamViewModel.VacantSeats)
                    {
                        _context.examRepository.AddUsersToExam(addUsersToExamViewModel.ExamIdentificator, usersToAddToExamIdentificators);

                        foreach (var user in usersToAddToExamIdentificators)
                        {
                            exam.EnrolledUsers.Add(user);
                        }

                        #region EntityLogs

                        var logInfoAssignUsersToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUsersToExam"]);
                        _logger.AddExamLog(exam, logInfoAssignUsersToExam);

                        #endregion

                        #region PersonalUserLogs

                        var logInfoPersonalAddGroupOfUsersToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addGroupOfUsersToExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddGroupOfUsersToExam);

                        var logInfoPersonalAddUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(usersToAddToExamIdentificators, logInfoPersonalAddUserToExam);

                        #endregion

                        return RedirectToAction("ExamDetails", new { examIdentificator = addUsersToExamViewModel.ExamIdentificator, message = "Dodano grupę użytkowników do egzaminu" });
                    }

                    ModelState.AddModelError("", "Brak wystarczającej ilości wolnych miejsc");
                    ModelState.AddModelError("", $"Do egzaminu można dodać maksymalnie {addUsersToExamViewModel.VacantSeats} użytkowników");

                    return View(addUsersToExamViewModel);
                }

                return RedirectToAction("ExamDetails", new { examIdentificator = addUsersToExamViewModel.ExamIdentificator });
            }

            return View(addUsersToExamViewModel);
        }

        // GET: MarkExamOrExamTermHub
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExamOrExamTermHub(string userIdentificator, string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam != null)
            {
                if (exam.ExamDividedToTerms)
                {
                    var examTerm = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms).Where(z => z.EnrolledUsers.Contains(userIdentificator)).FirstOrDefault();

                    return RedirectToAction("MarkExamTerm", "ExamsTerms", new { examTermIdentificator = examTerm.ExamTermIdentificator });
                }
                else
                {
                    return RedirectToAction("MarkExam", "Exams", new { examIdentificator = exam.ExamIdentificator });
                }
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: MarkExam
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExam(string examIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam.DateOfStart > DateTime.Now)
            {
                var usersEnrolledInExam = _context.userRepository.GetUsersById(exam.EnrolledUsers).ToList();
                var examResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults);

                MarkUserViewModel[] listOfUsers = new MarkUserViewModel[usersEnrolledInExam.Count()];

                if (usersEnrolledInExam.Count != 0)
                {
                    for (int i = 0; i < usersEnrolledInExam.Count(); i++)
                    {
                        var singleUser = _mapper.Map<MarkUserViewModel>(usersEnrolledInExam[i]);

                        var userExamResult = examResults.Where(z => z.User == usersEnrolledInExam[i].Id && exam.ExamResults.Contains(z.ExamResultIdentificator));

                        if (userExamResult.Count() != 0)
                        {
                            singleUser = _mapper.Map<ExamResult, MarkUserViewModel>(userExamResult.FirstOrDefault(), singleUser);
                        }
                        else
                        {
                            singleUser.PointsEarned = null;
                        }

                        listOfUsers[i] = singleUser;
                    }
                }

                MarkExamViewModel markExamViewModel = _mapper.Map<MarkExamViewModel>(exam);

                markExamViewModel.Users = listOfUsers;

                return View(markExamViewModel);
            }

            return RedirectToAction("ExamDetails", new { examIdentificator = examIdentificator });
        }

        // POST: MarkExam
        [HttpPost]
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExam(MarkExamViewModel markedExamViewModel)
        {
            if (ModelState.IsValid)
            {
                if (markedExamViewModel.DateOfStart > DateTime.Now)
                {
                    List<ExamResult> usersExamsResultsToAdd = new List<ExamResult>();
                    List<ExamResult> usersExamsResultsToUpdate = new List<ExamResult>();

                    var exam = _context.examRepository.GetExamById(markedExamViewModel.ExamIdentificator);
                    var examResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults);

                    foreach (var user in markedExamViewModel.Users)
                    {
                        ExamResult newUserExamResult = new ExamResult();

                        var previousUserExamResult = examResults.Where(z => z.User == user.UserIdentificator && exam.ExamResults.Contains(z.ExamResultIdentificator)).FirstOrDefault();

                        if (previousUserExamResult != null)
                        {
                            newUserExamResult = previousUserExamResult.Clone();

                            newUserExamResult.PointsEarned = (double)user.PointsEarned;
                            newUserExamResult.PercentageOfResult = user.PercentageOfResult;
                            newUserExamResult.ExamPassed = user.ExamPassed;

                            if (previousUserExamResult.ExamPassed == newUserExamResult.ExamPassed && previousUserExamResult.PointsEarned == newUserExamResult.PointsEarned)
                            {
                                continue;
                            }

                            usersExamsResultsToUpdate.Add(newUserExamResult);
                        }
                        else
                        {
                            if (user.PointsEarned != null)
                            {
                                newUserExamResult = _mapper.Map<ExamResult>(user);
                                newUserExamResult.ExamResultIdentificator = _keyGenerator.GenerateNewId();
                                newUserExamResult.ExamResultIndexer = _keyGenerator.GenerateExamResultEntityIndexer(exam.ExamIndexer);

                                usersExamsResultsToAdd.Add(newUserExamResult);
                            }
                        }
                    }

                    #region EntityLogs

                    var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateExam"]);
                    var logInfoUpdateExamResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["updateExamResult"]);

                    var logInfoAddExamResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptions.DescriptionOfActionOnEntity["addExamResult"]);

                    #endregion

                    if (usersExamsResultsToAdd.Count() == 0 && usersExamsResultsToUpdate.Count() == 0 && exam.MaxAmountOfPointsToEarn == markedExamViewModel.MaxAmountOfPointsToEarn)
                    {
                        return RedirectToAction("ExamDetails", new { examIdentificator = markedExamViewModel.ExamIdentificator, message = "Nie zmieniono żadnej oceny egzaminu" });
                    }

                    if (exam.MaxAmountOfPointsToEarn != markedExamViewModel.MaxAmountOfPointsToEarn)
                    {
                        exam.MaxAmountOfPointsToEarn = markedExamViewModel.MaxAmountOfPointsToEarn;
                        _context.examRepository.SetMaxAmountOfPointsToEarn(markedExamViewModel.ExamIdentificator, markedExamViewModel.MaxAmountOfPointsToEarn);

                        _logger.AddExamLog(exam, logInfoUpdateExam);

                        #region PersonalUserLogs

                        var logInfoPersonalUpdateExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateExam);

                        #endregion
                    }

                    if (usersExamsResultsToUpdate.Count() != 0)
                    {
                        _context.examResultRepository.UpdateExamsResults(usersExamsResultsToUpdate);
                        _logger.AddExamsResultsLogs(usersExamsResultsToUpdate, logInfoUpdateExamResults);

                        #region PersonalUserLogs

                        var logInfoPersonalUpdateExamResultFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateExamResultsFromExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalUpdateExamResultFromExam);

                        var logInfoPersonalUpdateUserExamResult = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["updateUserExamResult"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(usersExamsResultsToUpdate.Select(z => z.User).ToList(), logInfoPersonalUpdateUserExamResult);

                        #endregion
                    }
                    if (usersExamsResultsToAdd.Count() != 0)
                    {
                        _context.examResultRepository.AddExamsResults(usersExamsResultsToAdd);
                        _context.examRepository.AddExamsResultsToExam(exam.ExamIdentificator, usersExamsResultsToAdd.Select(z => z.ExamResultIdentificator).ToList());

                        _logger.AddExamsResultsLogs(usersExamsResultsToAdd, logInfoAddExamResults);

                        #region PersonalUserLogs

                        var logInfoPersonalAddUserExamResults = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUsersExamsResults"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserExamResults);

                        var logInfoPersonalAddUserExamResult = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserExamResult"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(usersExamsResultsToAdd.Select(z => z.User).ToList(), logInfoPersonalAddUserExamResult);

                        #endregion
                    }
                }

                return RedirectToAction("ExamDetails", new { examIdentificator = markedExamViewModel.ExamIdentificator, message = "Dokonano oceny egzaminu" });
            }

            return View(markedExamViewModel);
        }

        // GET: DisplayExamSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayExamSummary(string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);
            var examResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults);

            var users = _context.userRepository.GetUsersById(exam.EnrolledUsers);
            List<DisplayUserWithExamResults> listOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var user in users)
            {
                var userExamResult = examResults.Where(z => z.User == user.Id).FirstOrDefault();
                DisplayUserWithExamResults userWithExamResult = new DisplayUserWithExamResults();

                userWithExamResult = _mapper.Map<DisplayUserWithExamResults>(user);
                userWithExamResult.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;

                if (userExamResult != null)
                {
                    userWithExamResult = _mapper.Map<ExamResult, DisplayUserWithExamResults>(userExamResult, userWithExamResult);
                    userWithExamResult.HasExamResult = true;
                }
                else
                {
                    userWithExamResult.HasExamResult = false;
                }

                listOfUsers.Add(userWithExamResult);
            }

            DisplayUserListWithExamResultsAndExamIdentificator examSummary = new DisplayUserListWithExamResultsAndExamIdentificator
            {
                ExamIdentificator = exam.ExamIdentificator,

                ResultsList = listOfUsers
            };

            return View(examSummary);
        }

        // GET: ExaminerExams
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerExams()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var exams = _context.examRepository.GetExamsByExaminerId(user.Id);
            var courses = _context.courseRepository.GetCoursesByExamsId(exams.Select(z => z.ExamIdentificator).ToList());

            List<DisplayExamViewModel> listOfExams = new List<DisplayExamViewModel>();

            foreach (var exam in exams)
            {
                Exam examModel = _context.examRepository.GetExamById(exam.ExamIdentificator);

                DisplayExamViewModel singleExam = _mapper.Map<DisplayExamViewModel>(examModel);
                singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examModel.Examiners));
                singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(courses.Where(z=> z.Exams.Contains(examModel.ExamIdentificator)).FirstOrDefault());

                listOfExams.Add(singleExam);
            }

            return View(listOfExams);
        }

        // GET: DeleteExamResultHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteExamResultHub(string examResultIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(examResultIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteExamResultEntityLink(examResultIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteExamResult
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteExamResult(string examResultIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(examResultIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel examResultToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = examResultIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie wyniku z egzaminu"
                };

                return View("DeleteEntity", examResultToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteExamResult
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteExamResult(DeleteEntityViewModel examResultToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var examResult = _context.examResultRepository.GetExamResultById(examResultToDelete.EntityIdentificator);

            if (examResult == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, examResultToDelete.Code))
            {
                _context.examResultRepository.DeleteExamResult(examResultToDelete.EntityIdentificator);

                #region EntityLogs

                var logInfoDeleteExamResult = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExamResult"]);
                var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeExamResultFromCourse"]);

                _logger.AddExamResultLog(examResult, logInfoDeleteExamResult);

                var updatedExam = _context.examRepository.DeleteExamResultFromExam(examResultToDelete.EntityIdentificator);
                _logger.AddExamLog(updatedExam, logInfoUpdateExam);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalDeleteExamResult = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteExamResult"], "Indekser: " + examResult.ExamResultIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteExamResult);

                var logInfoPersonalRemoveUserExamResult = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserExamResult"], "Indekser: " + examResult.ExamResultIndexer);
                _context.personalLogRepository.AddPersonalUserLog(examResult.User, logInfoPersonalRemoveUserExamResult);

                #endregion

                return RedirectToAction("DisplayAllExamsResults", "Exams", new { message = "Usunięto wskazany wynik z egzaminu" });
            }

            return View("DeleteEntity", examResultToDelete);
        }

        // GET: DeleteExamHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteExamHub(string examIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteExamEntityLink(examIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteExam
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteExam(string examIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(examIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel examToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = examIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie egzaminu"
                };

                return View("DeleteEntity", examToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteExam
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteExam(DeleteEntityViewModel examToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var exam = _context.examRepository.GetExamById(examToDelete.EntityIdentificator);

            if (exam == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, examToDelete.Code))
            {
                _context.examRepository.DeleteExam(examToDelete.EntityIdentificator);

                #region EntityLogs

                var logInfoDeleteExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExam"]);
                var logInfoDeleteExamTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteExamTerm"]);
                var logInfoDeleteExamResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptions.DescriptionOfActionOnEntity["deleteAllExamResults"]);

                var logInfoUpdateCourse = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeExamFromCourse"]);

                _logger.AddExamLog(exam, logInfoDeleteExam);

                var deletedExamsTerms = _context.examTermRepository.DeleteExamsTerms(exam.ExamTerms);
                _logger.AddExamsTermsLogs(deletedExamsTerms, logInfoDeleteExamTerms);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResults(exam.ExamResults);
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDeleteExamResults);

                var updatedCourse = _context.courseRepository.DeleteExamFromCourse(examToDelete.EntityIdentificator);
                _logger.AddCourseLog(updatedCourse, logInfoUpdateCourse);

                #endregion

                #region PersonalUserLogs

                var logInfoPersonalDeleteExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteExam"], "Indekser: " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteExam);

                var logInfoPersonalDeleteUserExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["deleteUserExam"], "Indekser: " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUsersLogs(exam.EnrolledUsers, logInfoPersonalDeleteUserExam);
                _context.personalLogRepository.AddPersonalUsersLogs(exam.Examiners, logInfoPersonalDeleteUserExam);

                #endregion

                return RedirectToAction("DisplayAllExams", "Exams", new { message = "Usunięto wskazany egzamin" });
            }

            return View("DeleteEntity", examToDelete);
        }

        // GET: WorkerExamDetails
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerExamDetails(string examIdentificator)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var exam = _context.examRepository.GetExamById(examIdentificator);
            var userExamResult = _context.examResultRepository.GetExamsResultsById(exam.ExamResults).Where(z => z.User == user.Id).FirstOrDefault();

            var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

            List<DisplayExamTermWithoutExamViewModel> listOfExamTerms = new List<DisplayExamTermWithoutExamViewModel>();

            foreach (var examTerm in examTerms)
            {
                DisplayExamTermWithoutExamViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExamViewModel>(examTerm);
                singleExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examTerm.Examiners));

                listOfExamTerms.Add(singleExamTerm);
            }

            var course = _context.courseRepository.GetCourseByExamId(examIdentificator);
            var courseExams = _context.examRepository.GetExamsById(course.Exams);

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            DisplayExamResultToUserViewModel examResultViewModel = null;

            if (userExamResult != null)
            {
                examResultViewModel = _mapper.Map<DisplayExamResultToUserViewModel>(userExamResult);

                examResultViewModel.Exam = _mapper.Map<DisplayCrucialDataExamWithDatesViewModel>(exam);
                examResultViewModel.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;
            }

            bool canUserAssignToExam = false;
            bool canUserResignFromExam = false;

            if (exam.EnrolledUsers.Contains(user.Id))
            {
                canUserAssignToExam = false;

                if (DateTime.Now < exam.DateOfStart && userExamResult == null)
                {
                    canUserResignFromExam = true;
                }
            }
            else if (courseExams.Where(z => z.EnrolledUsers.Contains(user.Id) && z.ExamIndexer == exam.ExamIndexer).Count() == 0)
            {
                canUserAssignToExam = true;
            }

            WorkerExamDetailsViewModel examDetails = _mapper.Map<WorkerExamDetailsViewModel>(exam);

            examDetails.ExamResult = examResultViewModel;
            examDetails.ExamTerms = listOfExamTerms;

            examDetails.Course = courseViewModel;

            examDetails.CanUserAssignToExam = canUserAssignToExam;
            examDetails.CanUserResignFromExam = canUserResignFromExam;

            return View(examDetails);
        }

        // GET: ResignFromExam
        [Authorize(Roles = "Worker")]
        public ActionResult ResignFromExam(string examIdentificator, string examTermIdentificator)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            Exam exam = new Exam();

            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                exam = _context.examRepository.GetExamById(examIdentificator);
            }
            else if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            }

            if (exam.EnrolledUsers.Contains(user.Id))
            {
                exam = _context.examRepository.DeleteUserFromExam(user.Id, exam.ExamIdentificator);

                #region EntityLogs

                var logInfoDeleteUserFromExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromExam"]);
                var logInfoDeleteUserFromExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUserFromExamTerm"]);

                _logger.AddExamLog(exam, logInfoDeleteUserFromExam);

                #endregion

                #region PersonalUserLog

                var logInfoPersonalResignFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["resignFromExam"], "Indekser: " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalResignFromExam);

                var logInfoPersonalUserResignFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["resignUserFromExam"], "Indekser: " + exam.ExamIndexer);
                _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalUserResignFromExam);

                #endregion

                if (exam.ExamDividedToTerms)
                {
                    var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                    var userExamTerm = examTerms.Where(z => z.EnrolledUsers.Contains(user.Id)).FirstOrDefault();

                    userExamTerm = _context.examTermRepository.DeleteUserFromExamTerm(userExamTerm.ExamTermIdentificator, user.Id);

                    _logger.AddExamTermLog(userExamTerm, logInfoDeleteUserFromExamTerm);
                }
            }

            var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

            return RedirectToAction("WorkerCourseDetails", "Courses", new { courseIdentificator = course.CourseIdentificator, message = "Zrezygnowałeś z wybranego egzaminu" });
        }

        // GET: SelfAssignUserToExamHub
        [Authorize(Roles = "Worker")]
        public ActionResult SelfAssignUserToExamHub(string examIndexer, string examIdentificator, string examTermIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examIndexer))
            {
                return RedirectToAction("SelfAssignUserToChosenExamPeriod", "Exams", new { examIndexer });
            }
            else if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("SelfAssignUserToExamTerm", "ExamsTerms", new { examTermIdentificator });
            }
            else if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                var exam = _context.examRepository.GetExamById(examIdentificator);

                if (exam.ExamDividedToTerms)
                {
                    return RedirectToAction("SelfAssignUserToExamNotDividedToExamTerms", "Exams", new { examIdentificator });
                }
                else
                {
                    return RedirectToAction("SelfAssignUserToExamDividedToExamTerms", "Exams", new { examIdentificator });
                }
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: SelfAssignUserToExamPeriod
        [Authorize(Roles = "Worker")]
        public ActionResult SelfAssignUserToChosenExamPeriod(string examIndexer)
        {
            if (!string.IsNullOrWhiteSpace(examIndexer))
            {
                var exams = _context.examRepository.GetExamsByIndexer(examIndexer).OrderBy(z => z.OrdinalNumber).ToList();
                var firstExamPeriod = exams.FirstOrDefault();

                var course = _context.courseRepository.GetCourseByExamId(firstExamPeriod.ExamIdentificator);

                SelfAssignUserToExamViewModel assignToExamViewModel = new SelfAssignUserToExamViewModel();

                assignToExamViewModel.SelectedExam = firstExamPeriod.ExamIndexer + " " + firstExamPeriod.Name;
                assignToExamViewModel.ExamIndexer = examIndexer;

                assignToExamViewModel.AvailableExamsPeriods = _context.examRepository.GeneraterateExamsPeriodsSelectListWithVacantSeats(exams.Select(z => z.ExamIdentificator).ToList()).ToList();

                assignToExamViewModel.CourseIdentificator = course.CourseIdentificator;

                return View(assignToExamViewModel);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: SelfAssignUserToExamPeriod
        [Authorize(Roles = "Worker")]
        [HttpPost]
        public ActionResult SelfAssignUserToChosenExamPeriod(SelfAssignUserToExamViewModel assignToExamViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var course = _context.courseRepository.GetCourseById(assignToExamViewModel.CourseIdentificator);

                if (!course.EnrolledUsers.Contains(user.Id))
                {
                    return RedirectToAction("BlankMenu", "Certificates");
                }

                var exam = _context.examRepository.GetExamById(assignToExamViewModel.SelectedExamPeriod);

                var exams = _context.examRepository.GetExamsByIndexer(assignToExamViewModel.ExamIndexer).OrderBy(z => z.OrdinalNumber).ToList();
                assignToExamViewModel.AvailableExamsPeriods = _context.examRepository.GeneraterateExamsPeriodsSelectListWithVacantSeats(exams.Select(z => z.ExamIdentificator).ToList()).ToList();
                assignToExamViewModel.AvailableExamsTerms = _context.examTermRepository.GeneraterateExamsTermsSelectListWithVacantSeats(exam.ExamTerms);

                if (exam.EnrolledUsers.Count() < exam.UsersLimit)
                {
                    var logInfoAssignUserToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToExam"]);
                    var logInfoAssignUserToExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToExamTerm"]);

                    if (assignToExamViewModel.SelectedExamTerm != "00000000" && exam.ExamTerms.Contains(assignToExamViewModel.SelectedExamTerm))
                    {
                        var examTerm = _context.examTermRepository.GetExamTermById(assignToExamViewModel.SelectedExamTerm);

                        if (examTerm.EnrolledUsers.Count() < examTerm.UsersLimit)
                        {
                            examTerm.EnrolledUsers.Add(user.Id);
                            _context.examTermRepository.AddUserToExamTerm(assignToExamViewModel.SelectedExamTerm, user.Id);

                            _logger.AddExamTermLog(examTerm, logInfoAssignUserToExamTerm);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Brak wolnych miejsc w wybranej turze egzaminu");

                            return View(assignToExamViewModel);
                        }
                    }

                    exam.EnrolledUsers.Add(user.Id);
                    _context.examRepository.AddUserToExam(assignToExamViewModel.SelectedExamPeriod, user.Id);

                    _logger.AddExamLog(exam, logInfoAssignUserToExam);

                    #region PersonalUserLog

                    var logInfoPersonalAddUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserToExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserToExam);

                    var logInfoPersonalUserAssignUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalUserAssignUserToExam);

                    #endregion

                    return RedirectToAction("WorkerCourseDetails", "Courses", new { courseIdentificator = course.CourseIdentificator, message = "Zostałeś zapisany na wybrany egzamin" });
                }
                else
                {
                    ModelState.AddModelError("", "Brak wolnych miejsc w wybranym egzaminie");

                    return View(assignToExamViewModel);
                }
            }

            return View(assignToExamViewModel);
        }

        // GET: SelfAssignUserToExamNotDividedToExamTerms
        [Authorize(Roles = "Worker")]
        public ActionResult SelfAssignUserToExamNotDividedToExamTerms(string examIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

                var exam = _context.examRepository.GetExamByExamTermId(examIdentificator);

                if (exam.EnrolledUsers.Contains(user.Id))
                {
                    return RedirectToAction("BlankMenu", "Certificates");
                }

                if (exam.EnrolledUsers.Count() < exam.UsersLimit)
                {
                    var logInfoAssignUserToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToExam"]);

                    exam.EnrolledUsers.Add(user.Id);
                    _context.examRepository.AddUserToExam(exam.ExamIdentificator, user.Id);

                    _logger.AddExamLog(exam, logInfoAssignUserToExam);

                    #region PersonalUserLogs

                    var logInfoPersonalAddUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserToExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserToExam);

                    var logInfoPersonalAssignUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalAssignUserToExam);

                    #endregion

                    var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

                    return RedirectToAction("WorkerCourseDetails", "Courses", new { courseIdentificator = course.CourseIdentificator, message = "Zostałeś zapisany na wybrany egzamin" });
                }
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: CompanyWorkersExamDetails
        [Authorize(Roles = "Company")]
        public ActionResult CompanyWorkersExamDetails(string examIdentificator, string message)
        {
            ViewBag.message = message;

            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());
            var companyWorkersIdentificators = companyWorkers.Select(z => z.Id).ToList();

            var exam = _context.examRepository.GetExamById(examIdentificator);
            var companyWorkersExamResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults).Where(z => companyWorkersIdentificators.Contains(z.User)).ToList();

            var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

            List<DisplayExamTermWithoutExamViewModel> listOfExamTerms = new List<DisplayExamTermWithoutExamViewModel>();

            foreach (var examTerm in examTerms)
            {
                DisplayExamTermWithoutExamViewModel singleExamTerm = _mapper.Map<DisplayExamTermWithoutExamViewModel>(examTerm);
                singleExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examTerm.Examiners));

                listOfExamTerms.Add(singleExamTerm);
            }

            var course = _context.courseRepository.GetCourseByExamId(examIdentificator);

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            ICollection<DisplayExamResultToUserViewModel> listOfExamResults = new List<DisplayExamResultToUserViewModel>();

            if (companyWorkersExamResults.Count() != 0)
            {
                foreach (var companyWorkerExamResult in companyWorkersExamResults)
                {
                    DisplayExamResultToUserViewModel singleExamResult = _mapper.Map<DisplayExamResultToUserViewModel>(companyWorkerExamResult);

                    singleExamResult.Exam = _mapper.Map<DisplayCrucialDataExamWithDatesViewModel>(exam);
                    singleExamResult.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;
                }
            }

            var companyWorkersEnrolledInExam = companyWorkers.Where(z => exam.EnrolledUsers.Contains(z.Id)).ToList();
            List<DisplayCrucialDataUserViewModel> listOfEnrolledExamCompanyWorkers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyWorkersEnrolledInExam);

            CompanyWorkersExamDetailsViewModel examDetails = _mapper.Map<CompanyWorkersExamDetailsViewModel>(exam);

            examDetails.ExamResults = listOfExamResults;
            examDetails.ExamTerms = listOfExamTerms;
            examDetails.Course = courseViewModel;

            examDetails.EnrolledCompanyWorkers = listOfEnrolledExamCompanyWorkers;

            return View(examDetails);
        }

        // GET: SelfAssignUserToExamDividedToExamTerms
        [Authorize(Roles = "Worker")]
        public ActionResult SelfAssignUserToExamDividedToExamTerms(string examIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                var exam = _context.examRepository.GetExamById(examIdentificator);

                var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

                SelfAssignUserToExamDividedToExamTermsViewModel assignToExamViewModel = new SelfAssignUserToExamDividedToExamTermsViewModel();

                assignToExamViewModel.SelectedExam = exam.ExamIndexer + " " + exam.Name;

                var vacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();
                assignToExamViewModel.SelectedExamPeriod = "|Term." + exam.OrdinalNumber + " |" + exam.Name + " |wm.: " + vacantSeats + " |" + exam.DateOfStart.ToShortDateString() + " " + exam.DateOfStart.ToShortTimeString() + " - " + exam.DateOfEnd.ToShortDateString() + ":" + exam.DateOfEnd.ToShortTimeString();

                assignToExamViewModel.AvailableExamsTerms = _context.examTermRepository.GeneraterateExamsTermsSelectListWithVacantSeats(exam.ExamTerms);

                assignToExamViewModel.CourseIdentificator = course.CourseIdentificator;

                return View(assignToExamViewModel);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: SelfAssignUserToExamDividedToExamTerms
        [Authorize(Roles = "Worker")]
        [HttpPost]
        public ActionResult SelfAssignUserToExamDividedToExamTerms(SelfAssignUserToExamDividedToExamTermsViewModel assignToExamViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var course = _context.courseRepository.GetCourseById(assignToExamViewModel.CourseIdentificator);

                if (!course.EnrolledUsers.Contains(user.Id))
                {
                    return RedirectToAction("BlankMenu", "Certificates");
                }

                var exam = _context.examRepository.GetExamById(assignToExamViewModel.SelectedExamPeriod);
                assignToExamViewModel.AvailableExamsTerms = _context.examTermRepository.GeneraterateExamsTermsSelectListWithVacantSeats(exam.ExamTerms);

                if (exam.EnrolledUsers.Count() < exam.UsersLimit)
                {
                    var logInfoAssignUserToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToExam"]);
                    var logInfoAssignUserToExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUserToExamTerm"]);

                    var examTerm = _context.examTermRepository.GetExamTermById(assignToExamViewModel.SelectedExamTerm);

                    if (examTerm.EnrolledUsers.Count() < examTerm.UsersLimit)
                    {
                        examTerm.EnrolledUsers.Add(user.Id);
                        _context.examTermRepository.AddUserToExamTerm(assignToExamViewModel.SelectedExamTerm, user.Id);

                        _logger.AddExamTermLog(examTerm, logInfoAssignUserToExamTerm);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Brak wolnych miejsc w wybranej turze egzaminu");

                        return View(assignToExamViewModel);
                    }

                    exam.EnrolledUsers.Add(user.Id);
                    _context.examRepository.AddUserToExam(assignToExamViewModel.SelectedExamPeriod, user.Id);

                    _logger.AddExamLog(exam, logInfoAssignUserToExam);

                    #region PersonalUserLog

                    var logInfoPersonalAddUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addUserToExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddUserToExam);

                    var logInfoPersonalUserAssignUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLog(user.Id, logInfoPersonalUserAssignUserToExam);

                    #endregion

                    return RedirectToAction("WorkerCourseDetails", "Courses", new { courseIdentificator = course.CourseIdentificator, message = "Zostałeś zapisany na wybrany egzamin" });
                }
                else
                {
                    ModelState.AddModelError("", "Brak wolnych miejsc w wybranym egzaminie");

                    return View(assignToExamViewModel);
                }
            }

            return View(assignToExamViewModel);
        }

        // GET: DisplayCompanyWorkersExamSummary
        [Authorize(Roles = "Company")]
        public ActionResult DisplayCompanyWorkersExamSummary(string examIdentificator)
        {
            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());
            var companyWorkersIdentificators = companyWorkers.Select(z => z.Id).ToList(); 

            var exam = _context.examRepository.GetExamById(examIdentificator);
            var examResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults);

            var companyWorkersEnrolledInExam = companyWorkers.Where(z => exam.EnrolledUsers.Contains(z.Id)).ToList();
            List<DisplayUserWithExamResults> listOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var companyWorker in companyWorkersEnrolledInExam)
            {
                var userExamResult = examResults.Where(z => z.User == companyWorker.Id).FirstOrDefault();
                DisplayUserWithExamResults userWithExamResult = new DisplayUserWithExamResults();

                userWithExamResult = _mapper.Map<DisplayUserWithExamResults>(companyWorker);
                userWithExamResult.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;

                if (userExamResult != null)
                {
                    userWithExamResult = _mapper.Map<ExamResult, DisplayUserWithExamResults>(userExamResult, userWithExamResult);
                    userWithExamResult.HasExamResult = true;
                }
                else
                {
                    userWithExamResult.HasExamResult = false;
                }

                listOfUsers.Add(userWithExamResult);
            }

            DisplayUserListWithExamResultsAndExamIdentificator examSummary = new DisplayUserListWithExamResultsAndExamIdentificator
            {
                ExamIdentificator = exam.ExamIdentificator,

                ResultsList = listOfUsers
            };

            return View(examSummary);
        }

        // GET: AssignCompanyWorkersToExam
        [Authorize(Roles = "Company")]
        public ActionResult AssignCompanyWorkersToExam(string examIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());
            var companyWorkersIdentificators = companyWorkers.Select(z => z.Id).ToList();

            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam.DateOfStart > DateTime.Now)
            {
                var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

                var companyWorkersEnrolledInExamIdentificators = companyWorkersIdentificators.Where(z => !exam.EnrolledUsers.Contains(z)).ToList();

                List<string> companyWorkersWithPassedExamIdentificators = new List<string>();

                if (exam.OrdinalNumber != 1)
                {
                    var previousExamPeriods = _context.examRepository.GetExamPeriods(exam.ExamIndexer);

                    foreach (var examPeriod in previousExamPeriods)
                    {
                        var examResults = _context.examResultRepository.GetExamsResultsById(examPeriod.ExamResults).Where(z=> companyWorkersEnrolledInExamIdentificators.Contains(z.User)).ToList();

                        var usersWithPassedExamInThatPeriod = examResults.Where(z => z.ExamPassed).Select(z => z.User).ToList();

                        if (usersWithPassedExamInThatPeriod.Count() != 0)
                        {
                            companyWorkersWithPassedExamIdentificators.AddRange(usersWithPassedExamInThatPeriod);
                        }
                    }
                }

                var companyWorkersNotEnrolledInWithoutPassedExamIdentificators = companyWorkersEnrolledInExamIdentificators.Where(z => !companyWorkersWithPassedExamIdentificators.Contains(z)).ToList();
                var companyWorkersNotEnrolledInWithoutPassedExam = _context.userRepository.GetUsersById(companyWorkersNotEnrolledInWithoutPassedExamIdentificators);

                List<DisplayCrucialDataUserViewModel> listOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (companyWorkersNotEnrolledInWithoutPassedExam.Count != 0)
                {
                    listOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(companyWorkersNotEnrolledInWithoutPassedExam);

                    var vacantSeats = exam.UsersLimit - exam.EnrolledUsers.Count();

                    AssignUsersFromCourseToExamViewModel addUsersToExamViewModel = _mapper.Map<AssignUsersFromCourseToExamViewModel>(exam);
                    addUsersToExamViewModel.CourseParticipants = listOfUsers;
                    addUsersToExamViewModel.VacantSeats = vacantSeats;

                    addUsersToExamViewModel.UsersToAssignToExam = _mapper.Map<AddUsersFromCheckBoxViewModel[]>(listOfUsers);

                    return View(addUsersToExamViewModel);
                }
                return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = examIdentificator, message = "Wszyscy nieposiadający zaliczonego egzaminu zostali już na niego zapisani" });
            }

            return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = examIdentificator });
        }

        // POST: AssignCompanyWorkersToExam
        [HttpPost]
        [Authorize(Roles = "Company")]
        public ActionResult AssignCompanyWorkersToExam(AssignUsersFromCourseToExamViewModel addCompanyWorkersToExamViewModel)
        {
            if (ModelState.IsValid)
            {
                if (addCompanyWorkersToExamViewModel.DateOfStart > DateTime.Now)
                {
                    var exam = _context.examRepository.GetExamById(addCompanyWorkersToExamViewModel.ExamIdentificator);

                    var companyWorkersToAddToExamIdentificators = addCompanyWorkersToExamViewModel.UsersToAssignToExam.ToList().Where(z => z.IsToAssign == true).Select(z => z.UserIdentificator).ToList();

                    if (companyWorkersToAddToExamIdentificators.Count() <= addCompanyWorkersToExamViewModel.VacantSeats)
                    {
                        _context.examRepository.AddUsersToExam(addCompanyWorkersToExamViewModel.ExamIdentificator, companyWorkersToAddToExamIdentificators);

                        foreach (var companyWorker in companyWorkersToAddToExamIdentificators)
                        {
                            exam.EnrolledUsers.Add(companyWorker);
                        }

                        #region EntityLogs

                        var logInfoAssignUsersToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["assignUsersToExam"]);
                        _logger.AddExamLog(exam, logInfoAssignUsersToExam);

                        #endregion

                        #region PersonalUserLogs

                        var logInfoPersonalAddGroupOfUsersToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["addGroupOfUsersToExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalAddGroupOfUsersToExam);

                        var logInfoPersonalAddUserToExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["assignUserToExam"], "Indekser: " + exam.ExamIndexer);
                        _context.personalLogRepository.AddPersonalUsersLogs(companyWorkersToAddToExamIdentificators, logInfoPersonalAddUserToExam);

                        #endregion

                        return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = addCompanyWorkersToExamViewModel.ExamIdentificator, message = "Dodano grupę użytkowników do egzaminu" });
                    }

                    ModelState.AddModelError("", "Brak wystarczającej ilości wolnych miejsc");
                    ModelState.AddModelError("", $"Do egzaminu można dodać maksymalnie {addCompanyWorkersToExamViewModel.VacantSeats} użytkowników");

                    return View(addCompanyWorkersToExamViewModel);
                }

                return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = addCompanyWorkersToExamViewModel.ExamIdentificator });
            }

            return View(addCompanyWorkersToExamViewModel);
        }

        // GET: RemoveCompanyWorkersFromExam
        [Authorize(Roles = "Company")]
        public ActionResult RemoveCompanyWorkersFromExam(string examIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var companyManager = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var companyWorkers = _context.userRepository.GetUsersWorkersByCompanyId(companyManager.CompanyRoleManager.FirstOrDefault());
            var companyWorkersIdentificators = companyWorkers.Select(z => z.Id).ToList();

            var exam = _context.examRepository.GetExamById(examIdentificator);

            if (exam.DateOfStart > DateTime.Now)
            {
                var enrolledCompanyWorkersList = _context.userRepository.GetUsersById(exam.EnrolledUsers.Where(z => examIdentificator.Contains(z)).ToList());

                List<DisplayCrucialDataUserViewModel> listOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (enrolledCompanyWorkersList.Count != 0)
                {
                    listOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(enrolledCompanyWorkersList);
                }

                DeleteUsersFromExamViewModel removeCompanyWorkersFromExamViewModel = _mapper.Map<DeleteUsersFromExamViewModel>(exam);
                removeCompanyWorkersFromExamViewModel.AllExamParticipants = listOfUsers;

                removeCompanyWorkersFromExamViewModel.UsersToDeleteFromExam = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(listOfUsers);

                return View(removeCompanyWorkersFromExamViewModel);
            }

            return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = examIdentificator });
        }

        // POST: RemoveCompanyWorkersFromExam
        [HttpPost]
        [Authorize(Roles = "Company")]
        public ActionResult RemoveCompanyWorkersFromExam(DeleteUsersFromExamViewModel removeCompanyWorkersFromExamViewModel)
        {
            if (ModelState.IsValid)
            {
                var companyWorkersToDeleteFromExamIdentificators = removeCompanyWorkersFromExamViewModel.UsersToDeleteFromExam.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                if (removeCompanyWorkersFromExamViewModel.DateOfStart > DateTime.Now && companyWorkersToDeleteFromExamIdentificators.Count() != 0)
                {
                    var exam = _context.examRepository.GetExamById(removeCompanyWorkersFromExamViewModel.ExamIdentificator);
                    _context.examRepository.DeleteUsersFromExam(removeCompanyWorkersFromExamViewModel.ExamIdentificator, companyWorkersToDeleteFromExamIdentificators);

                    #region EntityLogs

                    var logInfoDeleteUsersFromExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUsersFromExam"]);
                    _logger.AddExamLog(exam, logInfoDeleteUsersFromExam);

                    #endregion

                    #region PersonalUserLogs

                    var logInfoPersonalDeleteGroupOfUsersFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["removeGroupOfUsersFromExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUserLogToAdminGroup(logInfoPersonalDeleteGroupOfUsersFromExam);

                    var logInfoPersonalRemoveUserFromExam = _context.personalLogRepository.GeneratePersonalLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogDescriptions.DescriptionOfPersonalUserLog["axUserFromExam"], "Indekser: " + exam.ExamIndexer);
                    _context.personalLogRepository.AddPersonalUsersLogs(companyWorkersToDeleteFromExamIdentificators, logInfoPersonalRemoveUserFromExam);

                    #endregion

                    if (removeCompanyWorkersFromExamViewModel.ExamDividedToTerms)
                    {
                        var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

                        _context.examTermRepository.DeleteUsersFromExamTerms(exam.ExamTerms, companyWorkersToDeleteFromExamIdentificators);

                        #region EntityLogs

                        var logInfoDeleteUsersFromExamTerms = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptions.DescriptionOfActionOnEntity["removeUsersFromExamTerm"]);
                        _logger.AddExamsTermsLogs(examTerms, logInfoDeleteUsersFromExamTerms);

                        #endregion
                    }

                    if (removeCompanyWorkersFromExamViewModel.UsersToDeleteFromExam.Count() == 1)
                    {
                        return RedirectToAction("CompanyWorkersDetails", "Users", new { userIdentificator = removeCompanyWorkersFromExamViewModel.UsersToDeleteFromExam.FirstOrDefault().UserIdentificator, message = "Usunięto użytkownika z egzaminu" });
                    }
                    else
                    {
                        return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = removeCompanyWorkersFromExamViewModel.ExamIdentificator, message = "Usunięto grupę użytkowników z egzaminu" });
                    }
                }

                return RedirectToAction("CompanyWorkersExamDetails", new { examIdentificator = removeCompanyWorkersFromExamViewModel.ExamIdentificator });
            }

            return RedirectToAction("RemoveCompanyWorkersFromExam", new { examIdentificator = removeCompanyWorkersFromExamViewModel.ExamIdentificator });
        }

        #region AjaxQuery
        // GET: GetUserAvailableToEnrollExamsByUserId
        [Authorize(Roles = "Admin, Examiner")]
        public string[][] GetUserAvailableToEnrollExamsByUserId(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            var examsIdentificators = _context.courseRepository.GetCoursesById(user.Courses).SelectMany(z => z.Exams).ToList();
            var exams = _context.examRepository.GetOnlyActiveExamsDividedToTermsById(examsIdentificators).Where(z => !z.EnrolledUsers.Contains(userIdentificator)).ToList();

            string[][] examsArray = new string[exams.Count()][];

            for (int i = 0; i < exams.Count(); i++)
            {
                var vacantSeats = exams[i].UsersLimit - exams[i].EnrolledUsers.Count();

                examsArray[i] = new string[2];

                examsArray[i][0] = exams[i].ExamIdentificator;
                examsArray[i][1] = exams[i].ExamIndexer + " |Term." + exams[i].OrdinalNumber + " | " + exams[i].Name + " |wm.: " + vacantSeats;
            }

            return examsArray;
        }

        // GET: GetExamsByCourseId
        [Authorize(Roles = "Admin")]
        public string[][] GetExamsByCourseId(string courseIdentificator)
        {
            var course = _context.courseRepository.GetCourseById(courseIdentificator);
            var exams = _context.examRepository.GetExamsById(course.Exams).ToList();

            string[][] examsArray = new string[exams.Count()][];

            for (int i = 0; i < exams.Count(); i++)
            {
                examsArray[i] = new string[2];

                examsArray[i][0] = exams[i].ExamIdentificator;
                examsArray[i][1] = exams[i].ExamIndexer + " | " + exams[i].Name;
            }

            return examsArray;
        }
        #endregion
    }
}
