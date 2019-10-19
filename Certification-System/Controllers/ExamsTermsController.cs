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

                exam.ExamTerms.Add(examTerm.ExamTermIdentificator);

                var examTermsFromParentExam = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);

                exam.UsersLimit = examTermsFromParentExam.Select(z => z.UsersLimit).Sum() + newExamTerm.UsersLimit;

                foreach (var examiner in newExamTerm.SelectedExaminers)
                {
                    if (!exam.Examiners.Contains(examiner))
                    {
                        exam.Examiners.Add(examiner);
                    }
                }

                exam.Examiners.Distinct();

                _context.examRepository.UpdateExam(exam);

                var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["addExamTermToExam"]);
                _logger.AddExamLog(exam, logInfoUpdateExam);

                _context.examTermRepository.AddExamTerm(examTerm);

                var logInfoAddExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptionOfAction.DescriptionOfActions["addExamTerm"]);
                _logger.AddExamTermLog(examTerm, logInfoAddExamTerm);

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

            var exams = _context.examRepository.GetListOfExams();
            var examsTerms = _context.examTermRepository.GetListOfExamsTerms();

            List<DisplayExamTermViewModel> listOfExams = new List<DisplayExamTermViewModel>();

            foreach (var examTerm in examsTerms)
            {
                DisplayExamTermViewModel singleExamTerm = _mapper.Map<DisplayExamTermViewModel>(examTerm);

                singleExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examTerm.Examiners));
                singleExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(exams.Where(z => z.ExamTerms.Contains(examTerm.ExamTermIdentificator)).FirstOrDefault());

                listOfExams.Add(singleExamTerm);
            }

            return View(listOfExams);
        }

        // GET: EditExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult EditExamTerm(string examTermIdentificator)
        {
            var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            EditExamTermViewModel examTermToUpdate = _mapper.Map<EditExamTermViewModel>(examTerm);
            examTermToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examTermToUpdate.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();
            examTermToUpdate.SelectedExam = exam.ExamIdentificator;

            return View(examTermToUpdate);
        }

        // POST: EditExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditExamTerm(EditExamTermViewModel editedExamTerm)
        {
            var originExamTerm = _context.examTermRepository.GetExamTermById(editedExamTerm.ExamTermIdentificator);
            var originExam = _context.examRepository.GetListOfExams().Where(z => z.ExamTerms.Contains(editedExamTerm.ExamTermIdentificator)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                originExam.ExamTerms.Remove(editedExamTerm.ExamTermIdentificator);
                _context.examRepository.UpdateExam(originExam);

                var exam = _context.examRepository.GetExamById(editedExamTerm.SelectedExam);
                exam.ExamTerms.Add(editedExamTerm.ExamTermIdentificator);

                var examTermsFromParentExam = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms);
                exam.UsersLimit = examTermsFromParentExam.Select(z => z.UsersLimit).Sum() + editedExamTerm.UsersLimit;

                foreach (var examiner in editedExamTerm.SelectedExaminers)
                {
                    if (!exam.Examiners.Contains(examiner))
                    {
                        exam.Examiners.Add(examiner);
                    }
                }

                exam.Examiners.Distinct();

                _context.examRepository.UpdateExam(exam);

                var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["updateExam"]);
                _logger.AddExamLog(exam, logInfoUpdateExam);

                originExamTerm = _mapper.Map<EditExamTermViewModel, ExamTerm>(editedExamTerm, originExamTerm);

                _context.examTermRepository.UpdateExamTerm(originExamTerm);

                var logInfoUpdateExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["updateExamTerm"]);
                _logger.AddExamTermLog(originExamTerm, logInfoUpdateExamTerm);

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

                var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
                var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

                DisplayExamTermWithLocationViewModel modifiedExamTerm = _mapper.Map<DisplayExamTermWithLocationViewModel>(examTerm);

                modifiedExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(exam);
                modifiedExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examTerm.Examiners).ToList());

                return View(modifiedExamTerm);
            }

            return RedirectToAction(nameof(AddNewExamTerm));
        }

        // GET: ExamTermDetails
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult ExamTermDetails(string examTermIdentificator, string message)
        {
            ViewBag.message = message;

            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);
            var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var examiners = _context.userRepository.GetUsersById(examTerm.Examiners);

            var enrolledUsers = _context.userRepository.GetUsersById(examTerm.EnrolledUsers);
            //var examTermResults = _context.examResultRepository.GetExamsResultsByExamTermId(examTermIdentificator);

            var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

            DisplayExamWithoutCourseViewModel examViewModel = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
            examViewModel.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners).ToList());

            List<DisplayCrucialDataWithContactUserViewModel> examinersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(examiners);

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            List<DisplayCrucialDataWithContactUserViewModel> usersViewModel = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(enrolledUsers);

            ExamTermDetailsViewModel examTermDetails = _mapper.Map<ExamTermDetailsViewModel>(examTerm);

            examTermDetails.Exam = examViewModel;
            examTermDetails.Examiners = examinersViewModel;

            examTermDetails.Course = courseViewModel;
            examTermDetails.Users = usersViewModel;

            if (this.User.IsInRole("Examiner"))
            {
                return View("ExaminerExamTermDetails", examTermDetails);
            }

            return View(examTermDetails);
        }

        // GET: AssignUserToExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserToExamTerm(string examIdentificator, string userIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);

            AssignUserToExamTermViewModel userToAssignToExamTerm = new AssignUserToExamTermViewModel
            {
                AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetActiveExamsAsSelectList().ToList(),
                AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList().ToList(),

                SelectedUser = userIdentificator,
            };

            if (!string.IsNullOrWhiteSpace(examIdentificator))
            {
                userToAssignToExamTerm.SelectedExam = examIdentificator;
                userToAssignToExamTerm.AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList(exam).ToList();
            }

            return View(userToAssignToExamTerm);
        }

        // POST: AssignUserToExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AssignUserToExamTerm(AssignUserToExamTermViewModel userAssignedToExamTerm)
        {
            var exam = _context.examRepository.GetExamById(userAssignedToExamTerm.SelectedExam);

            if (ModelState.IsValid)
            {
                var examTerm = _context.examTermRepository.GetExamTermById(userAssignedToExamTerm.SelectedExamTerm);
                var user = _context.userRepository.GetUserById(userAssignedToExamTerm.SelectedUser);

                if (!exam.EnrolledUsers.Contains(userAssignedToExamTerm.SelectedUser))
                {
                    var vacantSeats = examTerm.UsersLimit - examTerm.EnrolledUsers.Count();

                    if (vacantSeats < 1)
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranego użytkownika");
                    }
                    else
                    {
                        _context.examRepository.AddUserToExam(userAssignedToExamTerm.SelectedExam, userAssignedToExamTerm.SelectedUser);
                        _context.examTermRepository.AddUserToExamTerm(userAssignedToExamTerm.SelectedExamTerm, userAssignedToExamTerm.SelectedUser);

                        exam.EnrolledUsers.Add(userAssignedToExamTerm.SelectedUser);
                        examTerm.EnrolledUsers.Add(userAssignedToExamTerm.SelectedUser);

                        var logInfoAssignUsersToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["assignUsersToExam"]);
                        var logInfoAssignUsersToExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["assignUsersToExamTerm"]);

                        _logger.AddExamLog(exam, logInfoAssignUsersToExam);
                        _logger.AddExamTermLog(examTerm, logInfoAssignUsersToExamTerm);

                        return RedirectToAction("ExamTermDetails", new { examTermIdentificator = userAssignedToExamTerm.SelectedExamTerm, message = "Zapisano nowego użytkownika na turę egzaminu" });
                    }
                }
                else
                {
                    ModelState.AddModelError("", user.FirstName + " " + user.LastName + " już jest zapisany/a na wybraną turę egzaminu");
                }
            }

            if (string.IsNullOrWhiteSpace(userAssignedToExamTerm.SelectedExam))
            {
                if (exam != null)
                {
                    userAssignedToExamTerm.AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList(exam).ToList();
                }
                else
                {
                    userAssignedToExamTerm.AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList().ToList();
                }

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
            if (string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            if (examTerm.DateOfStart < DateTime.Now)
            {
                var enrolledUsersList = _context.userRepository.GetUsersById(examTerm.EnrolledUsers);

                List<DisplayCrucialDataUserViewModel> listOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (enrolledUsersList.Count != 0)
                {
                    listOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(enrolledUsersList);
                }

                DeleteUsersFromExamTermViewModel deleteUsersFromExamViewModel = _mapper.Map<DeleteUsersFromExamTermViewModel>(examTerm);
                deleteUsersFromExamViewModel.AllExamTermParticipants = listOfUsers;

                var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
                deleteUsersFromExamViewModel.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(exam);

                deleteUsersFromExamViewModel.UsersToDeleteFromExamTerm = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(listOfUsers);

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
                if (deleteUsersFromExamTermViewModel.DateOfStart < DateTime.Now)
                {
                    var usersToDeleteFromExamTermIdentificators = deleteUsersFromExamTermViewModel.UsersToDeleteFromExamTerm.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                    _context.examTermRepository.DeleteUsersFromExamTerm(deleteUsersFromExamTermViewModel.ExamTermIdentificator, usersToDeleteFromExamTermIdentificators);
                    _context.examRepository.DeleteUsersFromExam(deleteUsersFromExamTermViewModel.Exam.ExamIdentificator, usersToDeleteFromExamTermIdentificators);

                    var exam = _context.examRepository.GetExamByExamTermId(deleteUsersFromExamTermViewModel.ExamTermIdentificator);
                    var examTerm = _context.examTermRepository.GetExamTermById(deleteUsersFromExamTermViewModel.ExamTermIdentificator);

                    var logInfoRemoveUsersFromExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["removeUsersFromExam"]);
                    var logInfoRemoveUsersFromExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["removeUsersFromExamTerm"]);

                    _logger.AddExamLog(exam, logInfoRemoveUsersFromExam);
                    _logger.AddExamTermLog(examTerm, logInfoRemoveUsersFromExamTerm);

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
            if (string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            if (examTerm.DateOfStart < DateTime.Now)
            {
                var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);

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

                    var vacantSeats = examTerm.UsersLimit - examTerm.EnrolledUsers.Count();

                    AssignUsersFromCourseToExamTermViewModel addUsersToExamTermViewModel = _mapper.Map<AssignUsersFromCourseToExamTermViewModel>(examTerm);
                    addUsersToExamTermViewModel.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(exam);

                    addUsersToExamTermViewModel.CourseParticipants = listOfUsers;
                    addUsersToExamTermViewModel.VacantSeats = vacantSeats;

                    addUsersToExamTermViewModel.UsersToAssignToExamTerm = _mapper.Map<AddUsersFromCheckBoxViewModel[]>(listOfUsers);

                    return View(addUsersToExamTermViewModel);
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = examTermIdentificator, message = "Wszyscy uczestnicy kursu nieposiadający zaliczonego egzaminu zostali już na niego zapisani" });
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
                if (addUsersToExamTermViewModel.DateOfStart < DateTime.Now)
                {
                    var exam = _context.examRepository.GetExamByExamTermId(addUsersToExamTermViewModel.ExamTermIdentificator);

                    if (addUsersToExamTermViewModel.UsersToAssignToExamTerm.Where(z => z.IsToAssign == true).Count() <= addUsersToExamTermViewModel.VacantSeats)
                    {
                        var usersToAddToExamIdentificators = addUsersToExamTermViewModel.UsersToAssignToExamTerm.ToList().Where(z => z.IsToAssign == true).Select(z => z.UserIdentificator).ToList();

                        _context.examRepository.AddUsersToExam(addUsersToExamTermViewModel.Exam.ExamIdentificator, usersToAddToExamIdentificators);
                        _context.examTermRepository.AddUsersToExamTerm(addUsersToExamTermViewModel.ExamTermIdentificator, usersToAddToExamIdentificators);

                        var examTerm = _context.examTermRepository.GetExamTermById(addUsersToExamTermViewModel.ExamTermIdentificator);

                        foreach (var user in usersToAddToExamIdentificators)
                        {
                            exam.EnrolledUsers.Add(user);
                            examTerm.EnrolledUsers.Add(user);
                        }

                        var logInfoAssignUsersToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["assignUsersToExam"]);
                        var logInfoAssignUsersToExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["assignUsersToExamTerm"]);

                        _logger.AddExamLog(exam, logInfoAssignUsersToExam);
                        _logger.AddExamTermLog(examTerm, logInfoAssignUsersToExamTerm);

                        return RedirectToAction("ExamTermDetails", new { examTermIdentificator = addUsersToExamTermViewModel.ExamTermIdentificator, message = "Dodano grupę użytkowników do tury egzaminu" });
                    }

                    ModelState.AddModelError("", "Brak wystarczającej ilości wolnych miejsc");
                    ModelState.AddModelError("", $"Do tury egzaminu można dodać maksymalnie {addUsersToExamTermViewModel.VacantSeats} użytkowników");
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = addUsersToExamTermViewModel.ExamTermIdentificator });
            }

            return View(addUsersToExamTermViewModel);
        }

        // GET: MarkExamTerm
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExamTerm(string examTermIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            if (examTerm.DateOfStart < DateTime.Now)
            {
                var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
                var usersEnrolledInExamTerm = _context.userRepository.GetUsersById(examTerm.EnrolledUsers).ToList();
                var examResults = _context.examResultRepository.GetExamsResultsByExamTermId(examTermIdentificator);
                var examTermExaminers = _context.userRepository.GetUsersById(examTerm.Examiners);

                MarkUserViewModel[] listOfUsers = new MarkUserViewModel[usersEnrolledInExamTerm.Count()];

                if (usersEnrolledInExamTerm.Count != 0)
                {
                    for (int i = 0; i < usersEnrolledInExamTerm.Count(); i++)
                    {
                        var singleUser = _mapper.Map<MarkUserViewModel>(usersEnrolledInExamTerm[i]);

                        var userExamResult = examResults.Where(z => z.User == usersEnrolledInExamTerm[i].Id && exam.ExamResults.Contains(z.ExamResultIdentificator));

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

                MarkExamTermViewModel markExamTermViewModel = new MarkExamTermViewModel();

                markExamTermViewModel.ExamTerm = _mapper.Map<DisplayExamTermWithLocationViewModel>(examTerm);
                markExamTermViewModel.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;

                markExamTermViewModel.ExamTerm.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(exam);
                markExamTermViewModel.ExamTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(examTermExaminers);

                markExamTermViewModel.Users = listOfUsers;

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
                    List<ExamResult> usersExamsTermsResultsToAdd = new List<ExamResult>();
                    List<ExamResult> usersExamsTermsResultsToUpdate = new List<ExamResult>();

                    var exam = _context.examRepository.GetExamById(markedExamTermViewModel.ExamTerm.Exam.ExamIdentificator);
                    var examResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults);

                    foreach (var user in markedExamTermViewModel.Users)
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

                            usersExamsTermsResultsToUpdate.Add(newUserExamResult);
                        }
                        else
                        {
                            if (user.PointsEarned != null)
                            {
                                newUserExamResult = _mapper.Map<ExamResult>(user);
                                newUserExamResult.ExamResultIdentificator = _keyGenerator.GenerateNewId();
                                newUserExamResult.ExamResultIndexer = _keyGenerator.GenerateExamResultEntityIndexer(exam.ExamIndexer);
                                newUserExamResult.ExamTerm = markedExamTermViewModel.ExamTerm.ExamTermIdentificator;

                                usersExamsTermsResultsToAdd.Add(newUserExamResult);
                            }
                        }
                    }

                    var logInfoUpdateExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["updateExam"]);
                    var logInfoUpdateExamResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["updateExamResult"]);

                    var logInfoAddExamResults = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[0], LogDescriptionOfAction.DescriptionOfActions["addExamResult"]);

                    if (usersExamsTermsResultsToAdd.Count() == 0 && usersExamsTermsResultsToUpdate.Count() == 0 && exam.MaxAmountOfPointsToEarn == markedExamTermViewModel.MaxAmountOfPointsToEarn)
                    {
                        return RedirectToAction("ExamTermDetails", new { examTermIdentificator = markedExamTermViewModel.ExamTerm.ExamTermIdentificator, message = "Nie zmieniono żadnej oceny tury egzaminu" });
                    }

                    if (exam.MaxAmountOfPointsToEarn != markedExamTermViewModel.MaxAmountOfPointsToEarn)
                    {
                        exam.MaxAmountOfPointsToEarn = markedExamTermViewModel.MaxAmountOfPointsToEarn;
                        _context.examRepository.SetMaxAmountOfPointsToEarn(markedExamTermViewModel.ExamTerm.Exam.ExamIdentificator, markedExamTermViewModel.MaxAmountOfPointsToEarn);

                        _logger.AddExamLog(exam, logInfoUpdateExam);
                    }

                    if (usersExamsTermsResultsToUpdate.Count() != 0)
                    {
                        _context.examResultRepository.UpdateExamsResults(usersExamsTermsResultsToUpdate);
                        _logger.AddExamsResultsLogs(usersExamsTermsResultsToUpdate, logInfoUpdateExamResults);

                        //_logger.AddExamLog(exam, logInfo);
                    }
                    if (usersExamsTermsResultsToAdd.Count() != 0)
                    {
                        _context.examResultRepository.AddExamsResults(usersExamsTermsResultsToAdd);
                        _context.examRepository.AddExamsResultsToExam(exam.ExamIdentificator, usersExamsTermsResultsToAdd.Select(z => z.ExamResultIdentificator).ToList());

                        _logger.AddExamsResultsLogs(usersExamsTermsResultsToAdd, logInfoAddExamResults);
                    }
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = markedExamTermViewModel.ExamTerm.ExamTermIdentificator, message = "Dokonano oceny tury egzaminu" });
            }

            return View(markedExamTermViewModel);
        }

        // GET: DisplayExamTermSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayExamTermSummary(string examTermIdentificator)
        {
            var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);

            var examResults = _context.examResultRepository.GetExamsResultsById(exam.ExamResults);
            var examTermResults = examResults.Where(z => z.ExamTerm == examTermIdentificator);

            var users = _context.userRepository.GetUsersById(examTerm.EnrolledUsers);
            List<DisplayUserWithExamResults> listOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var user in users)
            {
                var userExamResult = examTermResults.Where(z => z.User == user.Id).FirstOrDefault();
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

            DisplayUserListWithExamResultsAndExamIdentificator examTermSummary = new DisplayUserListWithExamResultsAndExamIdentificator
            {
                ExamIdentificator = exam.ExamIdentificator,
                ExamTermIdentificator = examTerm.ExamTermIdentificator,

                ResultsList = listOfUsers
            };

            return View(examTermSummary);
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
                var logInfoDeleteExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptionOfAction.DescriptionOfActions["deleteExamTerm"]);
                var logInfoDeleteExamTermFromExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["removeExamTermFromExam"]);
                var logInfoDeleteExamResult= _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[2], LogDescriptionOfAction.DescriptionOfActions["deleteExamResult"]);

                _context.examTermRepository.DeleteExamTerm(examTermToDelete.EntityIdentificator);
                _logger.AddExamTermLog(examTerm, logInfoDeleteExamTerm);

                var deletedExamsResults = _context.examResultRepository.DeleteExamsResultsByExamTermId(examTermToDelete.EntityIdentificator);
                _logger.AddExamsResultsLogs(deletedExamsResults, logInfoDeleteExamResult);

                var updatedExam = _context.examRepository.DeleteExamTermFromExam(examTermToDelete.EntityIdentificator);
                _logger.AddExamLog(updatedExam, logInfoDeleteExamTermFromExam);

                return RedirectToAction("DisplayAllExamsTerms", "ExamsTerms", new { message = "Usunięto wskazaną turę egzaminu" });
            }

            return View("DeleteEntity", examTermToDelete);
        }


        // GET: WorkerExamTermDetails
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerExamTermDetails(string examTermIdentificator)
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);
            var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);
            var userExamResult = _context.examResultRepository.GetExamsResultsById(exam.ExamResults).Where(z => z.User == user.Id && z.ExamTerm == examTermIdentificator).FirstOrDefault();

            var examiners = _context.userRepository.GetUsersById(examTerm.Examiners);

            var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);
            var courseExams = _context.examRepository.GetExamsById(course.Exams);

            DisplayExamWithoutCourseViewModel examViewModel = _mapper.Map<DisplayExamWithoutCourseViewModel>(exam);
            examViewModel.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(exam.Examiners).ToList());

            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(course.Branches);

            DisplayExamResultToUserViewModel examResultViewModel = null;

            if (userExamResult != null)
            {
                examResultViewModel = _mapper.Map<DisplayExamResultToUserViewModel>(userExamResult);

                examResultViewModel.Exam = _mapper.Map<DisplayCrucialDataExamWithDatesViewModel>(exam);
                examResultViewModel.MaxAmountOfPointsToEarn = exam.MaxAmountOfPointsToEarn;
            }

            bool canUserAssignToExamTerm = false;
            bool canUserResignFromExamTerm = false;

            if (exam.EnrolledUsers.Contains(user.Id))
            {
                canUserAssignToExamTerm = false;

                if (DateTime.Now < exam.DateOfStart && userExamResult == null)
                {
                    canUserResignFromExamTerm = true;
                }
            }
            else if (courseExams.Where(z => z.EnrolledUsers.Contains(user.Id) && z.ExamIndexer == exam.ExamIndexer).Count() == 0)
            {
                canUserAssignToExamTerm = true;
            }

            WorkerExamTermDetailsViewModel examTermDetails = _mapper.Map<WorkerExamTermDetailsViewModel>(examTerm);

            examTermDetails.Exam = examViewModel;
            examTermDetails.ExamResult = examResultViewModel;

            examTermDetails.Course = courseViewModel;

            examTermDetails.CanUserAssignToExamTerm = canUserAssignToExamTerm;
            examTermDetails.CanUserResignFromExamTerm = canUserResignFromExamTerm;

            return View(examTermDetails);
        }

        // GET: SelfAssignUserToExamTerm
        [Authorize(Roles = "Worker")]
        public ActionResult SelfAssignUserToExamTerm(string examTermIdentificator)
        {
            if (!string.IsNullOrWhiteSpace(examTermIdentificator))
            {
                var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

                var examTerm = _context.examTermRepository.GetExamTermById(examTermIdentificator);
                var exam = _context.examRepository.GetExamByExamTermId(examTermIdentificator);

                if (examTerm.EnrolledUsers.Contains(user.Id))
                {
                    return RedirectToAction("BlankMenu", "Certificates");
                }

                if (examTerm.EnrolledUsers.Count() < examTerm.UsersLimit && exam.EnrolledUsers.Count() < exam.UsersLimit)
                {
                    var logInfoAssignUserToExam = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["assignUserToExam"]);
                    var logInfoAssignUserToExamTerm = _logger.GenerateLogInformation(this.User.Identity.Name, this.ControllerContext.RouteData.Values["action"].ToString(), LogTypeOfAction.TypesOfActions[1], LogDescriptionOfAction.DescriptionOfActions["assignUserToExamTerm"]);

                    examTerm.EnrolledUsers.Add(user.Id);
                    _context.examTermRepository.AddUserToExamTerm(examTerm.ExamTermIdentificator, user.Id);

                    _logger.AddExamTermLog(examTerm, logInfoAssignUserToExamTerm);

                    exam.EnrolledUsers.Add(user.Id);
                    _context.examRepository.AddUserToExam(exam.ExamIdentificator, user.Id);

                    _logger.AddExamLog(exam, logInfoAssignUserToExam);

                    var course = _context.courseRepository.GetCourseByExamId(exam.ExamIdentificator);

                    return RedirectToAction("WorkerCourseDetails", "Courses", new { courseIdentificator = course.CourseIdentificator, message = "Zostałeś zapisany na wybraną turę egzaminu" });
                }
            }

            return RedirectToAction("BlankMenu", "Certificates");
        }

        #region AjaxQuery
        // GET: GetUserAvailableToEnrollExamsTermsByExamId
        [Authorize(Roles = "Admin, Examiner, Worker")]
        public string[][] GetUserAvailableToEnrollExamsTermsByExamId(string examIdentificator)
        {
            var exam = _context.examRepository.GetExamById(examIdentificator);
            var examsTerms = _context.examTermRepository.GetExamsTermsById(exam.ExamTerms).ToList();

            string[][] examsTermsArray = new string[examsTerms.Count()][];

            if (!exam.ExamDividedToTerms)
            {
                examsTermsArray[0] = new string[2];
                examsTermsArray[0][0] = "00000000";
                examsTermsArray[0][1] = "Tura dla całego egzaminu";
            }

            for (int i = 0; i < examsTerms.Count(); i++)
            {
                var vacantSeats = examsTerms[i].UsersLimit - examsTerms[i].EnrolledUsers.Count();

                examsTermsArray[i] = new string[2];

                examsTermsArray[i][0] = examsTerms[i].ExamTermIdentificator;

                examsTermsArray[i][1] = examsTerms[i].ExamTermIndexer + " |" + examsTerms[i].DateOfStart + " - " + examsTerms[i].DateOfEnd + " |wm.: " + vacantSeats;
            }

            return examsTermsArray;
        }
        #endregion
    }
}
