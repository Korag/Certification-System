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

                var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddExamLog(exam, logInfoAdd);

                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(course, logInfoUpdate);

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
                    }

                    _context.examTermRepository.AddExamsTerms(examsTerms);

                    var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                    _logger.AddExamsTermsLogs(examsTerms, logInfo);
                }

                var ExamsTermsIdentificators = examsTerms.Select(z => z.ExamTermIdentificator);

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddExamLog(exam, logInfoAdd);

                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(course, logInfoUpdate);

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

                var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddExamLog(exam, logInfoAdd);

                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(course, logInfoUpdate);

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
                    }

                    _context.examTermRepository.AddExamsTerms(examsTerms);

                    var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                    _logger.AddExamsTermsLogs(examsTerms, logInfo);
                }

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddExamLog(exam, logInfoAdd);

                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddCourseLog(course, logInfoUpdate);

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

                    singleExam.DurationDays = (int)examModel.DateOfEnd.Subtract(examModel.DateOfStart).TotalDays;
                    singleExam.DurationMinutes = (int)examModel.DateOfEnd.Subtract(examModel.DateOfStart).TotalMinutes;

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

            List<DisplayExamTermViewModel> listOfExamTerms = _mapper.Map<List<DisplayExamTermViewModel>>(examTerms);

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

            examDetails.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
            examDetails.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;

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
                modifiedExam.Exam.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                modifiedExam.Exam.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;

                modifiedExam.ExamTerms = new List<DisplayExamTermWithoutExamWithLocationViewModel>();

                if (exam.ExamTerms.Count() != 0)
                {
                    var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                    modifiedExam.ExamTerms = _mapper.Map<List<DisplayExamTermWithoutExamWithLocationViewModel>>(examTerms);

                    foreach (var examTerm in modifiedExam.ExamTerms)
                    {
                        var singleExamTermExaminers = _context.userRepository.GetUsersById(examTerms.Where(z => z.ExamTermIdentificator == examTerm.ExamTermIdentificator).Select(z => z.Examiners).FirstOrDefault());

                        examTerm.DurationDays = (int)examTerm.DateOfEnd.Subtract(examTerm.DateOfStart).TotalDays;
                        examTerm.DurationMinutes = (int)examTerm.DateOfEnd.Subtract(examTerm.DateOfStart).TotalMinutes;

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

            if (ModelState.IsValid)
            {
                if (!editedExam.ExamDividedToTerms)
                {
                    originExam.ExamTerms.Clear();

                    _context.examTermRepository.DeleteExamsTerms(originExamTerms.Select(z => z.ExamTermIdentificator).ToList());
                }

                originExam = _mapper.Map<EditExamViewModel, Exam>(editedExam, originExam);
                _context.examRepository.UpdateExam(originExam);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddExamLog(originExam, logInfo);

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
            examToUpdate.ExamTerms = _mapper.Map<List<EditExamTermViewModel>>(examTerms);

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
            var originExamTerms = _context.examTermRepository.GetExamsTermsById(originExam.ExamTerms);

            if (ModelState.IsValid)
            {
                originExam = _mapper.Map<EditExamWithExamTermsViewModel, Exam>(editedExam, originExam);

                if (!editedExam.ExamDividedToTerms)
                {
                    originExam.ExamTerms.Clear();

                    _context.examTermRepository.DeleteExamsTerms(originExamTerms.Select(z => z.ExamTermIdentificator).ToList());

                    var logInfoDelete = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                    _logger.AddExamsTermsLogs(originExamTerms, logInfoDelete);
                }
                else if (editedExam.ExamDividedToTerms)
                {
                    originExamTerms = _mapper.Map<List<EditExamTermViewModel>, List<ExamTerm>>(editedExam.ExamTerms.ToList(), originExamTerms.ToList());

                    originExam.UsersLimit = editedExam.ExamTerms.Select(z => z.UsersLimit).Sum();
                    originExam.ExamDividedToTerms = true;

                    originExam.Examiners = editedExam.ExamTerms.SelectMany(z => z.SelectedExaminers).Distinct().ToList();
                    originExam.ExamTerms = editedExam.ExamTerms.Select(z => z.ExamTermIdentificator).ToList();

                    _context.examTermRepository.UpdateExamsTerms(originExamTerms);

                    var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                    _logger.AddExamsTermsLogs(originExamTerms, logInfoUpdate);

                    //List<string> NewExamTermsIdentificators = new List<string>();

                    //if (editedExam.ExamTerms.Count() != OriginExamTerms.Count())
                    //{
                    //    for (int i = OriginExamTerms.Count(); i < editedExam.ExamTerms.Count(); i++)
                    //    {
                    //        ExamTerm singleExamTerm = _mapper.Map<ExamTerm>(editedExam.ExamTerms.ElementAt(i));
                    //        singleExamTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();

                    //        NewExamTermsIdentificators.Add(singleExamTerm.ExamTermIdentificator);

                    //        _context.examTermRepository.AddExamTerm(singleExamTerm);
                    //    }
                    //}
                    //OriginExam.ExamTerms.ToList().AddRange(NewExamTermsIdentificators);
                }

                _context.examRepository.UpdateExam(originExam);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddExamLog(originExam, logInfo);

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

                        var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                        _logger.AddExamLog(exam, logInfo);

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

                deleteUsersFromExamViewModel.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                deleteUsersFromExamViewModel.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;

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

                    var logInfoExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                    _logger.AddExamLog(exam, logInfoExam);

                    if (deleteUsersFromExamViewModel.ExamDividedToTerms)
                    {
                        var examTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

                        _context.examTermRepository.DeleteUsersFromExamTerms(exam.ExamTerms, usersToDeleteFromExamIdentificators);

                        var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                        _logger.AddExamsTermsLogs(examTerms, logInfo);
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

                    addUsersToExamViewModel.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                    addUsersToExamViewModel.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;

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

                    if (addUsersToExamViewModel.UsersToAssignToExam.Count() <= addUsersToExamViewModel.VacantSeats)
                    {
                        var usersToAddToExamIdentificators = addUsersToExamViewModel.UsersToAssignToExam.ToList().Where(z => z.IsToAssign == true).Select(z => z.UserIdentificator).ToList();

                        _context.examRepository.AddUsersToExam(addUsersToExamViewModel.ExamIdentificator, usersToAddToExamIdentificators);

                        foreach (var user in usersToAddToExamIdentificators)
                        {
                            exam.EnrolledUsers.Add(user);
                        }

                        var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                        _logger.AddExamLog(exam, logInfo);

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

                markExamViewModel.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                markExamViewModel.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;

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

                        if (previousUserExamResult !=  null)
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

                    var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                    var logAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);

                    if (usersExamsResultsToAdd.Count() == 0 && usersExamsResultsToUpdate.Count() == 0 && exam.MaxAmountOfPointsToEarn == markedExamViewModel.MaxAmountOfPointsToEarn)
                    {
                        return RedirectToAction("ExamDetails", new { examIdentificator = markedExamViewModel.ExamIdentificator, message = "Nie zmieniono żadnej oceny egzaminu" });
                    }

                    if (exam.MaxAmountOfPointsToEarn != markedExamViewModel.MaxAmountOfPointsToEarn)
                    {
                        exam.MaxAmountOfPointsToEarn = markedExamViewModel.MaxAmountOfPointsToEarn;
                        _context.examRepository.SetMaxAmountOfPointsToEarn(markedExamViewModel.ExamIdentificator, markedExamViewModel.MaxAmountOfPointsToEarn);

                        _logger.AddExamLog(exam, logInfo);
                    }

                    if (usersExamsResultsToUpdate.Count() != 0)
                    {
                        _context.examResultRepository.UpdateExamsResults(usersExamsResultsToUpdate);
                        _logger.AddExamsResultsLogs(usersExamsResultsToUpdate, logInfo);

                        _logger.AddExamLog(exam, logInfo);
                    }
                    if (usersExamsResultsToAdd.Count() != 0)
                    {
                        _context.examResultRepository.AddExamsResults(usersExamsResultsToAdd);
                        _context.examRepository.AddExamsResultsToExam(exam.ExamIdentificator, usersExamsResultsToAdd.Select(z => z.ExamResultIdentificator).ToList());

                        _logger.AddExamsResultsLogs(usersExamsResultsToAdd, logAdd);
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

            return View(listOfUsers);
        }

        // GET: ExaminerExams
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerExams()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var exams = _context.examRepository.GetExamsByExaminerId(user.Id);
            var courses = _context.courseRepository.GetCoursesByExamsId(exams.Select(z => z.ExamIdentificator).ToList());

            List<DisplayExamViewModel> listOfExams = new List<DisplayExamViewModel>();

            foreach (var course in courses)
            {
                foreach (var exam in course.Exams)
                {
                    Exam examModel = exams.Where(z => z.ExamIdentificator == exam).FirstOrDefault();

                    DisplayExamViewModel singleExam = _mapper.Map<DisplayExamViewModel>(examModel);
                    singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examModel.Examiners));
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    singleExam.DurationDays = (int)examModel.DateOfEnd.Subtract(examModel.DateOfStart).TotalDays;
                    singleExam.DurationMinutes = (int)examModel.DateOfEnd.Subtract(examModel.DateOfStart).TotalMinutes;

                    listOfExams.Add(singleExam);
                }
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
                var logInfoDelete = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.examResultRepository.DeleteExamResult(examResultToDelete.EntityIdentificator);
                _logger.AddExamResultLog(examResult, logInfoDelete);

                var updatedExam = _context.examRepository.DeleteExamResultFromExam(examResultToDelete.EntityIdentificator);
                _logger.AddExamLog(updatedExam, logInfoUpdate);

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
                var logInfoDelete = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.examRepository.DeleteExam(examToDelete.EntityIdentificator);
                _logger.AddExamLog(exam, logInfoDelete);

                var deletedExamsTerms = _context.examTermRepository.DeleteExamsTerms(exam.ExamTerms);
                _logger.AddExamsTermsLogs(deletedExamsTerms, logInfoDelete);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResults(exam.ExamResults);
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDelete);

                var updatedCourse = _context.courseRepository.DeleteExamFromCourse(examToDelete.EntityIdentificator);
                _logger.AddCourseLog(updatedCourse, logInfoUpdate);

                return RedirectToAction("DisplayAllExams", "Exams", new { message = "Usunięto wskazany egzamin" });
            }

            return View("DeleteEntity", examToDelete);
        }

        #region AjaxQuery
        // GET: GetUserAvailableToEnrollExamsByUserId
        [Authorize(Roles = "Admin, Examiner")]
        public string[][] GetUserAvailableToEnrollExamsByUserId(string userIdentificator)
        {
            var user = _context.userRepository.GetUserById(userIdentificator);

            var examsIdentificators = _context.courseRepository.GetCoursesById(user.Courses).SelectMany(z => z.Exams).ToList();
            var exams = _context.examRepository.GetOnlyActiveExamsById(examsIdentificators).ToList();

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
