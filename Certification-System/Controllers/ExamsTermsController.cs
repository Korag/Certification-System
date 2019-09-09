using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
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

        public ExamsTermsController(MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
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
                var Exam = _context.examRepository.GetExamById(newExamTerm.SelectedExam);

                var ExamTerm = _mapper.Map<ExamTerm>(newExamTerm);
                ExamTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();

                ExamTerm.DurationDays = (int)ExamTerm.DateOfEnd.Subtract(ExamTerm.DateOfStart).TotalDays;
                ExamTerm.DurationMinutes = (int)ExamTerm.DateOfEnd.Subtract(ExamTerm.DateOfStart).TotalMinutes;

                _context.examTermRepository.AddExamTerm(ExamTerm);

                return RedirectToAction("ConfirmationOfActionOnExamTerm", new { examTermIdentificator = ExamTerm.ExamTermIdentificator, TypeOfAction = "Add" });
            }

            newExamTerm.AvailableExams = _context.examRepository.GetExamsWhichAreDividedToTermsAsSelectList().ToList();
            newExamTerm.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExamTerm);
        }

        // GET: DisplayAllExamsTerms
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllExamsTerms()
        {
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
                }

                OriginExamTerm = _mapper.Map<EditExamTermViewModel, ExamTerm>(editedExamTerm, OriginExamTerm);
                OriginExamTerm.DurationDays = (int)OriginExamTerm.DateOfEnd.Subtract(OriginExamTerm.DateOfStart).TotalDays;
                OriginExamTerm.DurationMinutes = (int)OriginExamTerm.DateOfEnd.Subtract(OriginExamTerm.DateOfStart).TotalMinutes;

                _context.examTermRepository.UpdateExamTerm(OriginExamTerm);

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
        [Authorize(Roles = "Admin")]
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

            List<DisplayUserWithExamResults> usersViewModel = new List<DisplayUserWithExamResults>();

            foreach (var user in EnrolledUsers)
            {
                DisplayUserWithExamResults singleUser = _mapper.Map<DisplayUserWithExamResults>(EnrolledUsers);

                var userExamResult = ExamTermResults.ToList().Where(z => z.User == user.Id).FirstOrDefault();

                if (userExamResult != null)
                {
                    singleUser = _mapper.Map<DisplayUserWithExamResults>(userExamResult);
                    singleUser.MaxAmountOfPointsToEarn = Exam.MaxAmountOfPointsToEarn;
                }
                else
                {
                    singleUser.HasExamResult = false;
                }

                usersViewModel.Add(singleUser);
            }

            ExamTermDetailsViewModel ExamTermDetails = _mapper.Map<ExamTermDetailsViewModel>(ExamTerm);
            ExamTermDetails.Exam = examViewModel;
            ExamTermDetails.Examiners = examinersViewModel;

            ExamTermDetails.Course = courseViewModel;
            ExamTermDetails.UsersWithResults = usersViewModel;

            return View(ExamTermDetails);
        }

        // GET: AssignUserToExamTerm
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserToExamTerm(string examIdentificator, string userIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);

            AssignUserToExamTermViewModel userToAssignToExamTerm = new AssignUserToExamTermViewModel
            {
                AvailableExamTerms = _context.examTermRepository.GetActiveExamTermsWithVacantSeatsAsSelectList(Exam).ToList(),

                UserIdentificator = userIdentificator,
                ExamIdentificator = examIdentificator
            };
            return View(userToAssignToExamTerm);
        }

        // POST: AssignUserToExamTerm
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AssignUserToExamTerm(AssignUserToExamTermViewModel userAssignedToExamTerm)
        {
            if (ModelState.IsValid)
            {
                var ExamTerm = _context.examTermRepository.GetExamTermById(userAssignedToExamTerm.SelectedExamTerm);
                var user = _context.userRepository.GetUserById(userAssignedToExamTerm.UserIdentificator);

                if (!ExamTerm.EnrolledUsers.Contains(userAssignedToExamTerm.UserIdentificator))
                {
                    var VacantSeats = ExamTerm.UsersLimit - ExamTerm.EnrolledUsers.Count();

                    if (VacantSeats < 1)
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranego użytkownika");
                    }
                    else
                    {
                        _context.examRepository.AddUserToExam(userAssignedToExamTerm.ExamIdentificator, userAssignedToExamTerm.UserIdentificator);
                        _context.examTermRepository.AddUserToExamTerm(userAssignedToExamTerm.SelectedExamTerm, userAssignedToExamTerm.UserIdentificator);

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
        [Authorize(Roles = "Admin")]
        public ActionResult MarkExamTerm(string examTermIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examTermIdentificator))
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
        [Authorize(Roles = "Admin")]
        public ActionResult MarkExamTerm(MarkExamTermViewModel markedExamTermViewModel)
        {
            if (ModelState.IsValid)
            {
                if (markedExamTermViewModel.ExamTerm.DateOfStart < DateTime.Now)
                {
                    foreach (var user in markedExamTermViewModel.Users)
                    {
                        ExamResult userExamResult = _mapper.Map<ExamResult>(user);
                        userExamResult.ExamResultIdentificator = _keyGenerator.GenerateNewId();

                        userExamResult.ExamTerm = markedExamTermViewModel.ExamTerm.ExamTermIdentificator;

                        _context.examResultRepository.AddExamResult(userExamResult);
                    }

                    _context.examRepository.SetMaxAmountOfPointsToEarn(markedExamTermViewModel.ExamTerm.Exam.ExamIdentificator, markedExamTermViewModel.MaxAmountOfPointsToEarn);
                }

                return RedirectToAction("ExamTermDetails", new { examTermIdentificator = markedExamTermViewModel.ExamTerm.ExamTermIdentificator, message = "Dokonano oceny tury egzaminu" });
            }

            return View(markedExamTermViewModel);
        }

        // GET: DisplayExamTermSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayExamSummary(string examTermIdentificator)
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
    }
}
