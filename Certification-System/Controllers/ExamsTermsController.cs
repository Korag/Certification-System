using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Extensions;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Controllers
{
    public class ExamsTermsController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public ExamsTermsController(
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

        // GET: AddNewExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewExamTerm(string examIdentificator)
        {
            AddExamTermViewModel newExamTerm = new AddExamTermViewModel
            {
                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetExamsWhichAreDividedToTermsAsSelectList().ToList()
            };

            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                newExamTerm.SelectedExam = examIdentificator;
            }

            return View(newExamTerm);
        }

        // POST: AddNewExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddNewExamTerm(AddExamTermViewModel newExamTerm)
        {
            if (ModelState.IsValid)
            {
                var exam = _context.examRepository.GetExamById(newExamTerm.SelectedExam);

                var examTerm = _mapper.Map<ExamTerm>(newExamTerm);
                examTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();
                examTerm.ExamTermIndexer = _keyGenerator.GenerateExamTermEntityIndexer(exam.ExamIndexer);

                examTerm.DurationDays = (int)examTerm.DateOfEnd.Subtract(examTerm.DateOfStart).TotalDays;
                examTerm.DurationMinutes = (int)examTerm.DateOfEnd.Subtract(examTerm.DateOfStart).TotalMinutes;

                exam.ExamTerms.Add(examTerm.ExamTermIdentificator);
                _context.examRepository.UpdateExam(exam);

                var logInfoExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddExamLog(exam, logInfoExam);

                _context.examTermRepository.AddExamTerm(examTerm);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                _logger.AddExamTermLog(examTerm, logInfo);

                return RedirectToAction("ConfirmationOfActionOnExamTerm", new { examTermIdentificator = examTerm.ExamTermIdentificator, TypeOfAction = "Add" });
            }

            newExamTerm.AvailableExams = _context.examRepository.GetExamsWhichAreDividedToTermsAsSelectList().ToList();
            newExamTerm.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExamTerm);
        }

        // GET: DisplayAllExamsTerms
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllExamsTerms(string message = null)
        {
            ViewBag.Message = message;

            var Exams = _context.examRepository.GetListOfExams();
            var ExamsTerms = _context.examTermRepository.GetListOfExamsTerms();

            List<DisplayExamTermViewModel> ListOfExams = new List<DisplayExamTermViewModel>();

            foreach (var examTerm in ExamsTerms)
            {
                DisplayExamTermViewModel singleExamTerm = _mapper.Map<DisplayExamTermViewModel>(examTerm);
                singleExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examTerm.Examiners));
                singleExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(Exams.Where(z => z.ExamTerms.Contains(examTerm.ExamTermIdentificator)).Select(z => z.ExamIdentificator).FirstOrDefault());

                ListOfExams.Add(singleExamTerm);
            }

            return View(ListOfExams);
        }

        // GET: EditExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult EditExamTerm(string examTermIdentificator)
        {
            var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            EditExamTermViewModel examTermToUpdate = _mapper.Map<EditExamTermViewModel>(ExamTerm);
            examTermToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examTermToUpdate.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();
            examTermToUpdate.SelectedExam = Exam.ExamIdentificator;

            return View(examTermToUpdate);
        }

        // POST: EditExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditExamTerm(EditExamTermViewModel editedExamTerm)
        {
            var OriginExamTerm = _context.examTermRepository.GetExamTermById(editedExamTerm.ExamTermIdentificator);
            var OriginExam = _context.examRepository.GetListOfExams().Where(z => z.ExamTerms.Contains(editedExamTerm.ExamTermIdentificator)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (editedExamTerm.SelectedExam != OriginExam.ExamIdentificator)
                {
                    OriginExam.ExamTerms.Remove(editedExamTerm.ExamTermIdentificator);
                    _context.examRepository.UpdateExam(OriginExam);

                    var Exam = _context.examRepository.GetExamById(editedExamTerm.SelectedExam);
                    Exam.ExamTerms.Add(editedExamTerm.ExamTermIdentificator);

                    _context.examRepository.UpdateExam(Exam);

                    var logInfoExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                    _logger.AddExamLog(Exam, logInfoExam);
                }

                OriginExamTerm = _mapper.Map<EditExamTermViewModel, ExamTerm>(editedExamTerm, OriginExamTerm);
                OriginExamTerm.DurationDays = (int)OriginExamTerm.DateOfEnd.Subtract(OriginExamTerm.DateOfStart).TotalDays;
                OriginExamTerm.DurationMinutes = (int)OriginExamTerm.DateOfEnd.Subtract(OriginExamTerm.DateOfStart).TotalMinutes;

                _context.examTermRepository.UpdateExamTerm(OriginExamTerm);

                var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                _logger.AddExamTermLog(OriginExamTerm, logInfo);

                return RedirectToAction("ConfirmationOfActionOnExamTerm", "ExamsTerms", new { examTermIdentificator = editedExamTerm.ExamTermIdentificator, TypeOfAction = "Update" });
            }

            editedExamTerm.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            editedExamTerm.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();

            return View(editedExamTerm);
        }

        // GET: ConfirmationOfActionOnExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnExamTerm(string examTermIdentificator, string TypeOfAction)
        {
            if (examTermIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
                var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

                DisplayExamTermViewModel modifiedExamTerm = _mapper.Map<DisplayExamTermViewModel>(ExamTerm);
                modifiedExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(Exam);
                modifiedExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(ExamTerm.Examiners).Select(z => z.Id).ToList());

                return View(modifiedExamTerm);
            }

            return RedirectToAction(nameof(AddNewExamTerm));
        }

        // GET: ExamTermDetails
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult ExamTermDetails(string examTermIdentificator, string message)
        {
            ViewBag.message = message;

            var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);
            var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var Examiners = _context.userRepository.GetUsersById(ExamTerm.Examiners);

            var EnrolledUsers = _context.userRepository.GetUsersById(ExamTerm.EnrolledUsers);
            var ExamTermResults = _context.examResultRepository.GetExamsResultsByExamTermId(examTermIdentificator);

            var Course = _context.courseRepository.GetCourseByExamId(Exam.ExamIdentificator);

            DisplayExamWithoutCourseViewModel examViewModel = _mapper.Map<DisplayExamWithoutCourseViewModel>(Exam);
            examViewModel.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(Exam.Examiners).Select(z => z.Id).ToList());

            List<DisplayCrucialDataWithContactUserViewModel> examinersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Examiners);

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(Course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            List<DisplayCrucialDataWithContactUserViewModel> usersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(EnrolledUsers);

            ExamTermDetailsViewModel ExamTermDetails = _mapper.Map<ExamTermDetailsViewModel>(ExamTerm);
            ExamTermDetails.Exam = examViewModel;
            ExamTermDetails.Examiners = examinersViewModel;

            ExamTermDetails.Course = courseViewModel;
            ExamTermDetails.Users = usersViewModel;

            if (this.User.IsInRole("Examiner"))
            {
                return View("ExaminerExamTermDetails", ExamTermDetails);
            }

            return View(ExamTermDetails);
        }

        // GET: AssignUserToExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserToExamTerm(string examIdentificator, string userIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);

            AssignUserToExamTermViewModel userToAssignToExamTerm = new AssignUserToExamTermViewModel
            {
                AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetActiveExamsAsSelectList().ToList(),
                AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList().ToList(),

            UserIdentificator = userIdentificator,
            };

            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                userToAssignToExamTerm.ExamIdentificator = examIdentificator;
                userToAssignToExamTerm.SelectedExams = examIdentificator;
                userToAssignToExamTerm.AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList(Exam).ToList();
            }

            return View(userToAssignToExamTerm);
        }

        // POST: AssignUserToExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AssignUserToExamTerm(AssignUserToExamTermViewModel userAssignedToExamTerm)
        {
            if (ModelState.IsValid)
            {
                var exam = _context.examRepository.GetExamById(userAssignedToExamTerm.ExamIdentificator);
                var examTerm = _context.examTermRepository.GetExamTermById(userAssignedToExamTerm.SelectedExamTerm);
                var user = _context.userRepository.GetUserById(userAssignedToExamTerm.UserIdentificator);

                if (!examTerm.EnrolledUsers.Contains(userAssignedToExamTerm.UserIdentificator))
                {
                    var VacantSeats = examTerm.UsersLimit - examTerm.EnrolledUsers.Count();

                    if (VacantSeats < 1)
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranego użytkownika");
                    }
                    else
                    {
                        _context.examRepository.AddUserToExam(userAssignedToExamTerm.ExamIdentificator, userAssignedToExamTerm.UserIdentificator);
                        _context.examTermRepository.AddUserToExamTerm(userAssignedToExamTerm.SelectedExamTerm, userAssignedToExamTerm.UserIdentificator);

                        exam.EnrolledUsers.Add(userAssignedToExamTerm.UserIdentificator);
                        examTerm.EnrolledUsers.Add(userAssignedToExamTerm.UserIdentificator);

                        var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                        _logger.AddExamLog(exam, logInfo);
                        _logger.AddExamTermLog(examTerm, logInfo);

                        return RedirectToAction("ExamTermDetails", new { examTermIdentificator = userAssignedToExamTerm.SelectedExamTerm, message = "Zapisano nowego użytkownika na turę egzaminu" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", user.FirstName + " " + user.LastName + " już jest zapisany/a na wybraną turę egzaminu");
                }
            }

            if (string.IsNullOrWhiteSpace(userAssignedToExamTerm.ExamIdentificator))
            {
                var Exam = _context.examRepository.GetExamById(userAssignedToExamTerm.ExamIdentificator);
                userAssignedToExamTerm.AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList(Exam).ToList();
                userAssignedToExamTerm.AvailableExams = _context.examRepository.GetActiveExamsAsSelectList().ToList();
                userAssignedToExamTerm.AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();

                return View(userAssignedToExamTerm);
            }
            else
            {
                return RedirectToAction("AssingUserToExam", "Exams");
            }
        }

        // GET: DeleteUsersFromExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromExamTerm(string examTermIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            if (ExamTerm.DateOfStart > DateTime.Now)
            {
                var EnrolledUsersList = _context.userRepository.GetUsersById(ExamTerm.EnrolledUsers);

                List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (EnrolledUsersList.Count != 0)
                {
                    ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(EnrolledUsersList);
                }

                DeleteUsersFromExamTermViewModel deleteUsersFromExamViewModel = _mapper.Map<DeleteUsersFromExamTermViewModel>(ExamTerm);
                deleteUsersFromExamViewModel.AllExamTermParticipants = ListOfUsers;

                var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
                deleteUsersFromExamViewModel.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(ExamTerm);

                deleteUsersFromExamViewModel.UsersToDeleteFromExamTerm = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(ListOfUsers);

                return View(deleteUsersFromExamViewModel);
            }

            return RedirectToAction("ExamTermDetails", new { examTermIdentificator = examTermIdentificator });
        }

        // POST: DeleteUsersFromExamTerm
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUsersFromExamTerm(DeleteUsersFromExamTermViewModel deleteUsersFromExamTermViewModel)
        {
            if (ModelState.IsValid)
            {
                if (deleteUsersFromExamTermViewModel.DateOfStart > DateTime.Now)
                {
                    var UsersToDeleteFromExamTermIdentificators = deleteUsersFromExamTermViewModel.UsersToDeleteFromExamTerm.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                    _context.examTermRepository.DeleteUsersFromExamTerm(deleteUsersFromExamTermViewModel.ExamTermIdentificator, UsersToDeleteFromExamTermIdentificators);
                    _context.examRepository.DeleteUsersFromExam(deleteUsersFromExamTermViewModel.Exam.ExamIdentificator, UsersToDeleteFromExamTermIdentificators);

                    var exam = _context.examRepository.GetExamByExamTermId(deleteUsersFromExamTermViewModel.ExamTermIdentificator);
                    var examTerm = _context.examTermRepository.GetExamTermById(deleteUsersFromExamTermViewModel.ExamTermIdentificator);

                    var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                    _logger.AddExamLog(exam, logInfo);
                    _logger.AddExamTermLog(examTerm, logInfo);

                    if (deleteUsersFromExamTermViewModel.UsersToDeleteFromExamTerm.Count() == 1)
                    {
                        return RedirectToAction("UserDetails", "Users", new { userIdentificator = deleteUsersFromExamTermViewModel.UsersToDeleteFromExamTerm.FirstOrDefault().UserIdentificator, message = "Usunięto użytkownika z tury egzaminu" });
                    }
                    else
                    {
                        return RedirectToAction("ExamTermDetails", new { examTermIdentificator = deleteUsersFromExamTermViewModel.ExamTermIdentificator, message = "Usunięto grupę użytkowników z tury egzaminu" });
                    }
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = deleteUsersFromExamTermViewModel.ExamTermIdentificator });
            }

            return RedirectToAction("DeleteUsersFromExamTerm", new { examTermIdentificator = deleteUsersFromExamTermViewModel.ExamTermIdentificator });
        }

        // GET: AssignUsersFromCourseToExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUsersFromCourseToExamTerm(string examTermIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            if (ExamTerm.DateOfStart > DateTime.Now)
            {
                var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
     
                var Course = _context.courseRepository.GetCourseByExamId(Exam.ExamIdentificator);

                var UsersNotEnrolledInExamIdentificators = Course.EnrolledUsers.Where(z => !Exam.EnrolledUsers.Contains(z)).ToList();

                List<string> UsersWithPassedExamIdentificators = new List<string>();

                if (Exam.OrdinalNumber != 1)
                {
                    var PreviousExamPeriods = _context.examRepository.GetExamPeriods(Exam.ExamIndexer);

                    foreach (var examPeriod in PreviousExamPeriods)
                    {
                        var ExamResults = _context.examResultRepository.GetExamsResultsById(examPeriod.ExamResults);

                        var UsersWithPassedExamInThatPeriod = ExamResults.Where(z => z.ExamPassed).Select(z => z.User).ToList();

                        if (UsersWithPassedExamInThatPeriod.Count() != 0)
                        {
                            UsersWithPassedExamIdentificators.AddRange(UsersWithPassedExamInThatPeriod);
                        }
                    }
                }

                var UsersNotEnrolledInWithoutPassedExamIdentificators = UsersNotEnrolledInExamIdentificators.Where(z => !UsersWithPassedExamIdentificators.Contains(z)).ToList();
                var UsersNotEnrolledInWithoutPassedExam = _context.userRepository.GetUsersById(UsersNotEnrolledInWithoutPassedExamIdentificators);

                List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (UsersNotEnrolledInWithoutPassedExam.Count != 0)
                {
                    ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(UsersNotEnrolledInWithoutPassedExam);

                    var VacantSeats = ExamTerm.UsersLimit - ExamTerm.EnrolledUsers.Count();

                    AssignUsersFromCourseToExamTermViewModel addUsersToExamTermViewModel = _mapper.Map<AssignUsersFromCourseToExamTermViewModel>(Exam);
                    addUsersToExamTermViewModel.CourseParticipants = ListOfUsers;
                    addUsersToExamTermViewModel.VacantSeats = VacantSeats;

                    addUsersToExamTermViewModel.UsersToAssignToExamTerm = _mapper.Map<AddUsersFromCheckBoxViewModel[]>(ListOfUsers);

                    return View(addUsersToExamTermViewModel);
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = examTermIdentificator, message = "Wszyscy nieposiadający zaliczonego egzaminu zostali już na niego zapisani" });
            }

            return RedirectToAction("ExamTermDetails", new { examTermIdentificator = examTermIdentificator });
        }

        // POST: AssignUsersFromCourseToExamTerm
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUsersFromCourseToExamTerm(AssignUsersFromCourseToExamTermViewModel addUsersToExamTermViewModel)
        {
            if (ModelState.IsValid)
            {
                if (addUsersToExamTermViewModel.DateOfStart > DateTime.Now)
                {
                    var Exam = _context.examRepository.GetExamByExamTermId(addUsersToExamTermViewModel.ExamTermIdentificator);

                    if (addUsersToExamTermViewModel.UsersToAssignToExamTerm.Count() <= addUsersToExamTermViewModel.VacantSeats)
                    {
                        var UsersToAddToExamIdentificators = addUsersToExamTermViewModel.UsersToAssignToExamTerm.ToList().Where(z => z.IsToAssign == true).Select(z => z.UserIdentificator).ToList();

                        _context.examRepository.AddUsersToExam(addUsersToExamTermViewModel.Exam.ExamIdentificator, UsersToAddToExamIdentificators);
                        _context.examTermRepository.AddUsersToExamTerm(addUsersToExamTermViewModel.ExamTermIdentificator, UsersToAddToExamIdentificators);

                        var examTerm = _context.examTermRepository.GetExamTermById(addUsersToExamTermViewModel.ExamTermIdentificator);

                        Exam.EnrolledUsers.ToList().AddRange(UsersToAddToExamIdentificators);
                        examTerm.EnrolledUsers.ToList().AddRange(UsersToAddToExamIdentificators);

                        var logInfo = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                        _logger.AddExamLog(Exam, logInfo);
                        _logger.AddExamTermLog(examTerm, logInfo);

                        return RedirectToAction("ExamTermDetails", new { examTermIdentificator = addUsersToExamTermViewModel.ExamTermIdentificator, message = "Dodano grupę użytkowników do tury egzaminu" });
                    }

                    ModelState.AddModelError("", "Brak wystarczającej ilości wolnych miejsc");
                    ModelState.AddModelError("", $"Do egzaminu można dodać maksymalnie {addUsersToExamTermViewModel.VacantSeats} użytkowników");
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = addUsersToExamTermViewModel.ExamTermIdentificator });
            }

            return View(addUsersToExamTermViewModel);
        }

        // GET: MarkExamTerm
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExamTerm(string examTermIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            if (ExamTerm.DateOfStart < DateTime.Now)
            {
                var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
                var UsersEnrolledInExam = _context.userRepository.GetUsersById(Exam.EnrolledUsers);
                var ExamResults = _context.examResultRepository.GetExamsResultsByExamTermId(examTermIdentificator);

                List<MarkUserViewModel> ListOfUsers = new List<MarkUserViewModel>();

                if (UsersEnrolledInExam.Count != 0)
                {
                    foreach (var user in UsersEnrolledInExam)
                    {
                        var singleUser = _mapper.Map<MarkUserViewModel>(user);

                        var userExamResult = ExamResults.Where(z => z.User == user.Id);

                        if (userExamResult != null)
                        {
                            singleUser = _mapper.Map<MarkUserViewModel>(userExamResult);
                        }

                        ListOfUsers.Add(singleUser);
                    }
                    ListOfUsers = _mapper.Map<List<MarkUserViewModel>>(UsersEnrolledInExam);
                }

                MarkExamTermViewModel markExamTermViewModel = new MarkExamTermViewModel();

                markExamTermViewModel.ExamTerm = _mapper.Map<DisplayExamTermViewModel>(ExamTerm);
                markExamTermViewModel.ExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(Exam);

                markExamTermViewModel.Users = ListOfUsers;

                return View(markExamTermViewModel);
            }

            return RedirectToAction("ExamTermDetails", new { examTermIdentificator = examTermIdentificator });
        }

        // POST: MarkExamTerm
        [HttpPost]
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExamTerm(MarkExamTermViewModel markedExamTermViewModel)
        {
            if (ModelState.IsValid)
            {
                if (markedExamTermViewModel.ExamTerm.DateOfStart < DateTime.Now)
                {
                    List<ExamResult> usersExamResults = new List<ExamResult>();

                    var exam = _context.examRepository.GetExamById(markedExamTermViewModel.ExamTerm.Exam.ExamIdentificator);

                    foreach (var user in markedExamTermViewModel.Users)
                    {
                        ExamResult singleUserExamResult = _mapper.Map<ExamResult>(user);
                        singleUserExamResult.ExamResultIdentificator = _keyGenerator.GenerateNewId();
                        singleUserExamResult.ExamResultIndexer = _keyGenerator.GenerateExamResultEntityIndexer(exam.ExamIndexer);

                        singleUserExamResult.ExamTerm = markedExamTermViewModel.ExamTerm.ExamTermIdentificator;

                        _context.examResultRepository.AddExamResult(singleUserExamResult);

                        usersExamResults.Add(singleUserExamResult);
                    }

                    var logInfoAdd = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0]);
                    _logger.AddExamsResultsLogs(usersExamResults, logInfoAdd);

                    _context.examRepository.SetMaxAmountOfPointsToEarn(markedExamTermViewModel.ExamTerm.Exam.ExamIdentificator, markedExamTermViewModel.MaxAmountOfPointsToEarn);

                    var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);
                    _logger.AddExamLog(exam, logInfoUpdate);
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = markedExamTermViewModel.ExamTerm.ExamTermIdentificator, message = "Dokonano oceny tury egzaminu" });
            }

            return View(markedExamTermViewModel);
        }

        // GET: DisplayExamTermSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayExamTermSummary(string examTermIdentificator)
        {
            var Exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var ExamTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            var ExamResults = _context.examResultRepository.GetExamsResultsById(Exam.ExamResults);
            var ExamTermResults = ExamResults.Where(z => z.ExamTerm == examTermIdentificator);

            var Users = _context.userRepository.GetUsersById(ExamTerm.EnrolledUsers);
            List<DisplayUserWithExamResults> ListOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var user in Users)
            {
                var userExamResult = ExamTermResults.Where(z => z.User == user.Id).FirstOrDefault();
                DisplayUserWithExamResults UserWithExamResult = new DisplayUserWithExamResults();

                if (userExamResult != null)
                {
                    UserWithExamResult = Mapper.Map<DisplayUserWithExamResults>(user);
                    UserWithExamResult = _mapper.Map<DisplayUserWithExamResults>(userExamResult);
                    UserWithExamResult.MaxAmountOfPointsToEarn = Exam.MaxAmountOfPointsToEarn;
                }
                else
                {
                    UserWithExamResult.HasExamResult = false;
                }

                ListOfUsers.Add(UserWithExamResult);
            }

            return View(ListOfUsers);
        }

        // GET: DeleteExamTermHub
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteExamTermHub(string examTermIdentificator, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
                var generatedCode = _keyGenerator.GenerateUserTokenForEntityDeletion(user);

                var url = Url.DeleteExamTermEntityLink(examTermIdentificator, generatedCode, Request.Scheme);
                var emailMessage = _emailSender.GenerateEmailMessage(user.Email, user.FirstName + " " + user.LastName, "authorizeAction", url);
                _emailSender.SendEmailAsync(emailMessage);

                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 5, returnUrl });
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // GET: DeleteExamTerm
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult DeleteExamTerm(string examTermIdentificator, string code)
        {
            if (!string.IsNullOrWhiteSpace(examTermIdentificator) && !string.IsNullOrWhiteSpace(code))
            {
                DeleteEntityViewModel examTermToDelete = new DeleteEntityViewModel
                {
                    EntityIdentificator = examTermIdentificator,
                    Code = code,

                    ActionName = this.ControllerContext.RouteData.Values["action"].ToString(),
                    FormHeader = "Usuwanie wyniku z egzaminu"
                };

                return View("DeleteEntity", examTermToDelete);
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        // POST: DeleteExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteExamTerm(DeleteEntityViewModel examTermToDelete)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);
            var examTerm = _context.examTermRepository.GetExamTermById(examTermToDelete.EntityIdentificator);

            if (examTerm == null)
            {
                return RedirectToAction("UniversalConfirmationPanel", "Account", new { messageNumber = 6, returnUrl = Url.BlankMenuLink(Request.Scheme) });
            }

            if (ModelState.IsValid && _keyGenerator.ValidateUserTokenForEntityDeletion(user, examTermToDelete.Code))
            {
                var logInfoDelete = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2]);
                var logInfoUpdate = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1]);

                _context.examTermRepository.DeleteExamTerm(examTermToDelete.EntityIdentificator);
                _logger.AddExamTermLog(examTerm, logInfoDelete);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResultsByExamTermId(examTermToDelete.EntityIdentificator);
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDelete);

                var updatedExam = _context.examRepository.DeleteExamTermFromExam(examTermToDelete.EntityIdentificator);
                _logger.AddExamLog(updatedExam, logInfoUpdate);

                return RedirectToAction("DisplayAllExamsTerms", "ExamsTerms", new { message = "Usunięto wskazaną turę egzaminu" });
            }

            return View("DeleteEntity", examTermToDelete);
        }

        #region AjaxQuery
        // GET: GetUserAvailableToEnrollExamsTermsByExamId
        [Authorize(Roles = "Examiner")]
        public string[][] GetUserAvailableToEnrollExamsTermsByExamId(string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);
            var examsTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms).ToList();

            string[][] examsTermsArray = new string[examsTerms.Count()][];

            for (int i = 0; i < examsTerms.Count(); i++)
            {
                examsTermsArray[i] = new string[2];

                examsTermsArray[i][0] = examsTerms[i].ExamTermIdentificator;
                examsTermsArray[i][1] = examsTerms[i].DateOfStart + " - " + examsTerms[i].DateOfEnd;
            }

            return examsTermsArray;
        }
        #endregion
    }
}
