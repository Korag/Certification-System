using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public ExamsController(MongoOperations context, IMapper mapper, IKeyGenerator keyGenerator)
        {
            _context = context;
            _mapper = mapper;
            _keyGenerator = keyGenerator;
        }

        // GET: AddNewExam
        [Authorize(Roles = "Admin")]
        public ActionResult AddNewExam(string courseIdentificator)
        {
            AddExamViewModel newExam = new AddExamViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
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

                course.Exams.Add(exam.ExamIdentificator);

                exam.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                exam.OrdinalNumber = 1;

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExam.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

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

                course.Exams.Add(exam.ExamIdentificator);

                exam.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                exam.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;

                exam.OrdinalNumber = 1;
                exam.UsersLimit = newExam.ExamTerms.Select(z => z.UsersLimit).Sum();

                exam.Examiners = newExam.ExamTerms.SelectMany(z => z.SelectedExaminers).Distinct().ToList();

                List<ExamTerm> examsTerms = new List<ExamTerm>();

                if (exam.ExamTerms.Count() != 0)
                {
                    foreach (var newExamTerm in newExam.ExamTerms)
                    {
                        ExamTerm examTerm = _mapper.Map<ExamTerm>(newExamTerm);
                        examTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();

                        examTerm.DurationDays = (int)newExamTerm.DateOfEnd.Subtract(newExamTerm.DateOfStart).TotalDays;
                        examTerm.DurationMinutes = (int)newExamTerm.DateOfEnd.Subtract(newExamTerm.DateOfStart).TotalMinutes;

                        exam.ExamTerms.Add(examTerm.ExamTermIdentificator);
                        examsTerms.Add(examTerm);
                    }

                    _context.examTermRepository.AddExamsTerms(examsTerms);
                }

                var ExamsTermsIdentificators = examsTerms.Select(z => z.ExamTermIdentificator);

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExam.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExam);
        }

        // GET: AddExamPeriod
        [Authorize(Roles = "Admin")]
        public ActionResult AddExamPeriod(string courseIdentificator, string examIdentificator)
        {
            AddExamPeriodViewModel newExamPeriod = new AddExamPeriodViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList(),

                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
            };

            if (!string.IsNullOrWhiteSpace(courseIdentificator))
            {
                newExamPeriod.SelectedCourse = courseIdentificator;
            }
            if (!string.IsNullOrWhiteSpace(courseIdentificator) && !string.IsNullOrWhiteSpace(examIdentificator))
            {
                newExamPeriod.SelectedExam = examIdentificator;
            }

            return View(newExamPeriod);
        }

        // POST: AddExamPeriod
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddExamPeriod(AddExamPeriodViewModel newExamPeriod)
        {
            if (ModelState.IsValid)
            {
                var course = _context.courseRepository.GetCourseById(newExamPeriod.SelectedCourse);
                var examsInCourse = _context.examRepository.GetExamsById(course.Exams);

                Exam exam = _mapper.Map<Exam>(newExamPeriod);
                exam.ExamIdentificator = _keyGenerator.GenerateNewId();

                course.Exams.Add(exam.ExamIdentificator);

                exam.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                exam.OrdinalNumber = examsInCourse.Where(z => z.ExamIndexer == exam.ExamIndexer).Count();

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExamPeriod.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExamPeriod.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();
            newExamPeriod.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExamPeriod);
        }

        // GET: AddExamPeriodWithExamTerms
        [Authorize(Roles = "Admin")]
        public ActionResult AddExamPeriodWithExamTerms(string courseIdentificator, string examIdentificator, int quantityOfExamTerms)
        {
            AddExamPeriodWithExamTermsViewModel newExamPeriod = new AddExamPeriodWithExamTermsViewModel
            {
                AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList(),
                AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList(),
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
            }
            if (!string.IsNullOrWhiteSpace(courseIdentificator) && !string.IsNullOrWhiteSpace(examIdentificator))
            {
                newExamPeriod.SelectedExam = examIdentificator;
            }

            return View(newExamPeriod);
        }

        // POST: AddExamPeriodWithExamTerms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddExamPeriodWithExamTerms(AddExamPeriodWithExamTermsViewModel newExamPeriod)
        {
            if (ModelState.IsValid)
            {
                var course = _context.courseRepository.GetCourseById(newExamPeriod.SelectedCourse);
                var examsInCourse = _context.examRepository.GetExamsById(course.Exams);

                Exam exam = _mapper.Map<Exam>(newExamPeriod);
                exam.ExamIdentificator = _keyGenerator.GenerateNewId();

                course.Exams.Add(exam.ExamIdentificator);

                exam.DurationDays = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalDays;
                exam.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;
                exam.OrdinalNumber = examsInCourse.Where(z => z.ExamIndexer == exam.ExamIndexer).Count();

                List<ExamTerm> examsTerms = new List<ExamTerm>();

                if (exam.ExamTerms.Count() != 0)
                {
                    foreach (var newExamTerm in newExamPeriod.ExamTerms)
                    {
                        ExamTerm examTerm = _mapper.Map<ExamTerm>(newExamTerm);
                        examTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();

                        examTerm.DurationDays = (int)newExamTerm.DateOfEnd.Subtract(newExamTerm.DateOfStart).TotalDays;
                        examTerm.DurationMinutes = (int)newExamTerm.DateOfEnd.Subtract(newExamTerm.DateOfStart).TotalMinutes;

                        exam.ExamTerms.Add(examTerm.ExamTermIdentificator);
                        examsTerms.Add(examTerm);
                    }

                    _context.examTermRepository.AddExamsTerms(examsTerms);
                }

                var ExamsTermsIdentificators = examsTerms.Select(z => z.ExamTermIdentificator);

                _context.courseRepository.UpdateCourse(course);
                _context.examRepository.AddExam(exam);

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExamPeriod.AvailableCourses = _context.courseRepository.GetActiveCoursesWhereExamIsRequiredAsSelectList().ToList();
            newExamPeriod.AvailableExams = _context.examRepository.GetExamsAsSelectList().ToList();
            newExamPeriod.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExamPeriod);
        }

        // GET: DisplayAllExams
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllExams()
        {
            var Exams = _context.examRepository.GetListOfExams();
            var Courses = _context.courseRepository.GetListOfCourses();

            List<DisplayExamViewModel> ListOfExams = new List<DisplayExamViewModel>();

            foreach (var course in Courses)
            {
                foreach (var exam in course.Exams)
                {
                    Exam examModel = Exams.Where(z => z.ExamIdentificator == exam).FirstOrDefault();

                    DisplayExamViewModel singleExam = _mapper.Map<DisplayExamViewModel>(examModel);
                    singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examModel.Examiners));
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    ListOfExams.Add(singleExam);
                }
            }

            return View(ListOfExams);
        }

        // GET: ExamDetails
        [Authorize(Roles = "Admin")]
        public ActionResult ExamDetails(string examIdentificator, string message)
        {
            ViewBag.Message = message;

            var Exam = _context.examRepository.GetExamById(examIdentificator);
            var ExamTerms = _context.examTermRepository.GetExamsTermsById(Exam.ExamTerms);
            var ExamResults = _context.examResultRepository.GetExamsResultsById(Exam.ExamResults);

            List<DisplayExamTermViewModel> ListOfExamTerms = _mapper.Map<List<DisplayExamTermViewModel>>(ExamTerms);

            //List<string> ExaminersIdentificators = Exam.Examiners.ToList();
            //List<string> UsersIdentificators = Exam.EnrolledUsers.ToList();

            //foreach (var examTerm in ExamTerms)
            //{
            //    ExaminersIdentificators.AddRange(examTerm.Examiners);
            //    UsersIdentificators.AddRange(examTerm.EnrolledUsers);
            //}
            //ExaminersIdentificators.Distinct();
            //UsersIdentificators.Distinct();

            //var Examiners = _context.userRepository.GetUsersById(ExaminersIdentificators);

            var Examiners = _context.userRepository.GetUsersById(Exam.Examiners);
            List<DisplayCrucialDataWithContactUserViewModel> ListOfExaminers = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Examiners);

            //var Users = _context.userRepository.GetUsersById(UsersIdentificators);

            var Users = _context.userRepository.GetUsersById(Exam.EnrolledUsers);
            List<DisplayCrucialDataWithContactUserViewModel> ListOfUsers = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Users);

            //foreach (var user in Users)
            //{
            //    var userExamResult = ExamResults.Where(z => z.User == user.Id).FirstOrDefault();
            //    DisplayUserWithExamResults UserWithExamResult = new DisplayUserWithExamResults();

            //    if (userExamResult != null)
            //    {
            //        UserWithExamResult = Mapper.Map<DisplayUserWithExamResults>(user);
            //        UserWithExamResult = _mapper.Map<DisplayUserWithExamResults>(userExamResult);
            //        UserWithExamResult.MaxAmountOfPointsToEarn = Exam.MaxAmountOfPointsToEarn;
            //    }
            //    else
            //    {
            //        UserWithExamResult.HasExamResult = false;
            //    }
            //}

            var Course = _context.courseRepository.GetCourseByExamId(examIdentificator);
            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(Course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            ExamDetailsViewModel ExamDetails = _mapper.Map<ExamDetailsViewModel>(Exam);
            ExamDetails.ExamTerms = ListOfExamTerms;
            ExamDetails.Course = courseViewModel;

            ExamDetails.Examiners = ListOfExaminers;
            ExamDetails.EnrolledUsers = ListOfUsers;

            return View(ExamDetails);
        }

        // GET: ConfirmationOfActionOnExam
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnExam(string examIdentificator, string TypeOfAction)
        {
            if (examIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Exam = _context.examRepository.GetExamById(examIdentificator);
                var Examiners = _context.userRepository.GetUsersById(Exam.Examiners);

                var Course = _context.courseRepository.GetCourseByExamId(examIdentificator);

                DisplayExamWithTermsViewModel modifiedExam = new DisplayExamWithTermsViewModel();

                modifiedExam.Exam = _mapper.Map<DisplayExamViewModel>(Exam);

                if (Exam.ExamTerms.Count() != 0)
                {
                    var ExamTerms = _context.examTermRepository.GetExamsTermsById(Exam.ExamTerms);
                    modifiedExam.ExamTerms = _mapper.Map<List<DisplayExamTermViewModel>>(ExamTerms);

                    foreach (var examTerm in modifiedExam.ExamTerms)
                    {
                        var SingleExamTermExaminers = _context.userRepository.GetUsersById(ExamTerms.Where(z => z.ExamTermIdentificator == examTerm.ExamTermIdentificator).Select(z => z.Examiners).FirstOrDefault());

                        examTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(SingleExamTermExaminers);
                    }
                }

                modifiedExam.Exam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                modifiedExam.Exam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Examiners);

                return View(modifiedExam);
            }

            return RedirectToAction(nameof(AddNewExam));
        }

        // GET: EditExamHub
        [Authorize(Roles = "Admin")]
        public ActionResult EditExamHub(string examIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);

            if (Exam.ExamDividedToTerms && Exam.ExamTerms.Count() != 0)
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
            var Exam = _context.examRepository.GetExamById(examIdentificator);
            var Course = _context.courseRepository.GetCourseByExamId(Exam.ExamIdentificator);

            EditExamViewModel examToUpdate = _mapper.Map<EditExamViewModel>(Exam);

            examToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examToUpdate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            examToUpdate.SelectedExaminers = _context.userRepository.GetUsersById(Exam.Examiners).Select(z => z.Id).ToList();
            examToUpdate.SelectedCourse = Course.CourseIdentificator;

            return View(examToUpdate);
        }

        // POST: EditExam
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditExam(EditExamViewModel editedExam)
        {
            var OriginExam = _context.examRepository.GetExamById(editedExam.ExamIdentificator);
            var OriginExamTerms = _context.examTermRepository.GetExamsTermsById(OriginExam.ExamTerms);

            if (ModelState.IsValid)
            {
                if (!editedExam.ExamDividedToTerms)
                {
                    OriginExam.ExamTerms.Clear();

                    _context.examTermRepository.DeleteExamsTerms(OriginExamTerms.Select(z => z.ExamTermIdentificator).ToList());
                }

                OriginExam = _mapper.Map<EditExamViewModel, Exam>(editedExam, OriginExam);
                _context.examRepository.UpdateExam(OriginExam);

                return RedirectToAction("ConfirmationOfActionOnExam", "Exams", new { examIdentificator = editedExam.ExamIdentificator, TypeOfAction = "Update" });
            }

            editedExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            editedExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            return View(editedExam);
        }

        // GET: EditExamWithExamTerms
        [Authorize(Roles = "Admin")]
        public ActionResult EditExamWithExamTerms(string examIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);
            var ExamTerms = _context.examTermRepository.GetExamsTermsById(Exam.ExamTerms);
            var Course = _context.courseRepository.GetCourseByExamId(Exam.ExamIdentificator);

            EditExamWithExamTermsViewModel examToUpdate = _mapper.Map<EditExamWithExamTermsViewModel>(Exam);
            examToUpdate.ExamTerms = _mapper.Map<List<EditExamTermViewModel>>(ExamTerms);

            examToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examToUpdate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            examToUpdate.SelectedExaminers = _context.userRepository.GetUsersById(Exam.Examiners).Select(z => z.Id).ToList();
            examToUpdate.SelectedCourse = Course.CourseIdentificator;

            return View(examToUpdate);
        }

        // POST: EditExamWithExamTerms
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditExamWithExamTerms(EditExamWithExamTermsViewModel editedExam)
        {
            var OriginExam = _context.examRepository.GetExamById(editedExam.ExamIdentificator);
            var OriginExamTerms = _context.examTermRepository.GetExamsTermsById(OriginExam.ExamTerms);

            if (ModelState.IsValid)
            {
                if (!editedExam.ExamDividedToTerms)
                {
                    OriginExam.ExamTerms.Clear();

                    _context.examTermRepository.DeleteExamsTerms(OriginExamTerms.Select(z => z.ExamTermIdentificator).ToList());
                }
                else if (editedExam.ExamDividedToTerms)
                {
                    OriginExamTerms = _mapper.Map<List<EditExamTermViewModel>, List<ExamTerm>>(editedExam.ExamTerms.ToList(), OriginExamTerms.ToList());

                    OriginExam.ExamTerms = editedExam.ExamTerms.Select(z => z.ExamTermIdentificator).ToList();

                    _context.examTermRepository.UpdateExamsTerms(OriginExamTerms);

                    List<string> NewExamTermsIdentificators = new List<string>();

                    if (editedExam.ExamTerms.Count() != OriginExamTerms.Count())
                    {
                        for (int i = OriginExamTerms.Count(); i < editedExam.ExamTerms.Count(); i++)
                        {
                            ExamTerm singleExamTerm = _mapper.Map<ExamTerm>(editedExam.ExamTerms.ElementAt(i));
                            singleExamTerm.ExamTermIdentificator = _keyGenerator.GenerateNewId();

                            NewExamTermsIdentificators.Add(singleExamTerm.ExamTermIdentificator);

                            _context.examTermRepository.AddExamTerm(singleExamTerm);
                        }
                    }
                    OriginExam.ExamTerms.ToList().AddRange(NewExamTermsIdentificators);
                }

                OriginExam = _mapper.Map<EditExamWithExamTermsViewModel, Exam>(editedExam, OriginExam);
                _context.examRepository.UpdateExam(OriginExam);

                return RedirectToAction("ConfirmationOfActionOnExam", "Exams", new { examIdentificator = editedExam.ExamIdentificator, TypeOfAction = "Update" });
            }

            editedExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            editedExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            return View(editedExam);
        }

        // GET: DisplayAllExamsResults
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayAllExamsResults()
        {
            var ExamsResults = _context.examResultRepository.GetListOfExamsResults();
            var Exams = _context.examRepository.GetListOfExams();
            var Courses = _context.courseRepository.GetListOfCourses();
            var Users = _context.userRepository.GetListOfUsers();

            List<DisplayExamResultViewModel> ListOfExamsResults = new List<DisplayExamResultViewModel>();

            foreach (var examResult in ExamsResults)
            {
                DisplayExamResultViewModel singleExamResult = _mapper.Map<DisplayExamResultViewModel>(examResult);

                var RelatedExam = _context.examRepository.GetExamByExamResultId(examResult.ExamResultIdentificator);
                var RelatedCourse = _context.courseRepository.GetCourseByExamId(RelatedExam.ExamIdentificator);
                var RelatedUser = _context.userRepository.GetUserById(examResult.User);

                singleExamResult.MaxAmountOfPointsToEarn = RelatedExam.MaxAmountOfPointsToEarn;

                singleExamResult.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(RelatedExam);
                singleExamResult.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(RelatedCourse);
                singleExamResult.User = _mapper.Map<DisplayCrucialDataUserViewModel>(RelatedUser);

                ListOfExamsResults.Add(singleExamResult);
            }

            return View(ListOfExamsResults);
        }

        // GET: AssignUserToExam
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUserToExam(string userIdentificator)
        {
            string ChosenUser = null;

            if (!string.IsNullOrWhiteSpace(userIdentificator))
                ChosenUser = _context.userRepository.GetUserById(userIdentificator).Id;

            var AvailableUsers = _context.userRepository.GetWorkersAsSelectList().ToList();
            var AvailableExams = _context.examRepository.GetActiveExamsWithVacantSeatsAsSelectList().ToList();

            AssignUserToExamViewModel userToAssignToExam = new AssignUserToExamViewModel
            {
                AvailableExams = AvailableExams,

                AvailableUsers = AvailableUsers,
                SelectedUser = ChosenUser
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
                var Exam = _context.examRepository.GetExamById(userAssignedToExam.SelectedExam);
                var user = _context.userRepository.GetUserById(userAssignedToExam.SelectedUser);

                if (!Exam.EnrolledUsers.Contains(userAssignedToExam.SelectedUser))
                {
                    if (Exam.ExamDividedToTerms)
                    {
                        return RedirectToAction("AssignUserToExamTerm", "ExamsTerms", new { examIdentificator = userAssignedToExam.SelectedExam, userIdentificator = userAssignedToExam.SelectedUser });
                    }

                    var VacantSeats = Exam.UsersLimit - Exam.EnrolledUsers.Count();

                    if (VacantSeats < 1)
                    {
                        ModelState.AddModelError("", "Brak wystarczającej liczby miejsc dla wybranego użytkownika");
                    }
                    else
                    {
                        _context.examRepository.AddUserToExam(userAssignedToExam.SelectedExam, userAssignedToExam.SelectedUser);

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

            var Exam = _context.examRepository.GetExamById(examIdentificator);

            if (Exam.DateOfStart > DateTime.Now)
            {
                var EnrolledUsersList = _context.userRepository.GetUsersById(Exam.EnrolledUsers);

                List<DisplayCrucialDataUserViewModel> ListOfUsers = new List<DisplayCrucialDataUserViewModel>();

                if (EnrolledUsersList.Count != 0)
                {
                    ListOfUsers = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(EnrolledUsersList);
                }

                DeleteUsersFromExamViewModel deleteUsersFromExamViewModel = _mapper.Map<DeleteUsersFromExamViewModel>(Exam);
                deleteUsersFromExamViewModel.AllExamParticipants = ListOfUsers;

                deleteUsersFromExamViewModel.UsersToDeleteFromExam = _mapper.Map<DeleteUsersFromCheckBoxViewModel[]>(ListOfUsers);

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
                if (deleteUsersFromExamViewModel.DateOfStart > DateTime.Now)
                {
                    var UsersToDeleteFromExamIdentificators = deleteUsersFromExamViewModel.UsersToDeleteFromExam.ToList().Where(z => z.IsToDelete == true).Select(z => z.UserIdentificator).ToList();

                    _context.examRepository.DeleteUsersFromExam(deleteUsersFromExamViewModel.ExamIdentificator, UsersToDeleteFromExamIdentificators);

                    if (deleteUsersFromExamViewModel.ExamDividedToTerms)
                    {
                        var Exam = _context.examRepository.GetExamById(deleteUsersFromExamViewModel.ExamIdentificator);
                        _context.examTermRepository.DeleteUsersFromExamTerms(Exam.ExamTerms, UsersToDeleteFromExamIdentificators);
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

            var Exam = _context.examRepository.GetExamById(examIdentificator);

            if (Exam.DateOfStart > DateTime.Now)
            {
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

                    var VacantSeats = Exam.UsersLimit - Exam.EnrolledUsers.Count();

                    AssignUsersFromCourseToExamViewModel addUsersToExamViewModel = _mapper.Map<AssignUsersFromCourseToExamViewModel>(Exam);
                    addUsersToExamViewModel.CourseParticipants = ListOfUsers;
                    addUsersToExamViewModel.VacantSeats = VacantSeats;

                    addUsersToExamViewModel.UsersToAssignToExam = _mapper.Map<AddUsersFromCheckBoxViewModel[]>(ListOfUsers);

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
                    var Exam = _context.examRepository.GetExamById(addUsersToExamViewModel.ExamIdentificator);

                    if (addUsersToExamViewModel.UsersToAssignToExam.Count() <= addUsersToExamViewModel.VacantSeats)
                    {
                        var UsersToAddToExamIdentificators = addUsersToExamViewModel.UsersToAssignToExam.ToList().Where(z => z.IsToAssign == true).Select(z => z.UserIdentificator).ToList();

                        _context.examRepository.AddUsersToExam(addUsersToExamViewModel.ExamIdentificator, UsersToAddToExamIdentificators);

                        return RedirectToAction("ExamDetails", new { examIdentificator = addUsersToExamViewModel.ExamIdentificator, message = "Dodano grupę użytkowników do egzaminu" });
                    }

                    ModelState.AddModelError("", "Brak wystarczającej ilości wolnych miejsc");
                    ModelState.AddModelError("", $"Do egzaminu można dodać maksymalnie {addUsersToExamViewModel.VacantSeats} użytkowników");
                }

                return RedirectToAction("ExamDetails", new { examIdentificator = addUsersToExamViewModel.ExamIdentificator });
            }

            return View(addUsersToExamViewModel);
        }

        // GET: MarkExam
        [Authorize(Roles = "Admin, Examiner")]
        public ActionResult MarkExam(string examIdentificator)
        {
            if (string.IsNullOrWhiteSpace(examIdentificator))
            {
                return RedirectToAction("BlankMenu", "Certificates");
            }

            var Exam = _context.examRepository.GetExamById(examIdentificator);

            if (Exam.DateOfStart < DateTime.Now)
            {
                var UsersEnrolledInExam = _context.userRepository.GetUsersById(Exam.EnrolledUsers);
                var ExamResults = _context.examResultRepository.GetExamsResultsById(Exam.ExamResults);

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

                MarkExamViewModel markExamViewModel = _mapper.Map<MarkExamViewModel>(Exam);

                markExamViewModel.Users = ListOfUsers;

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
                if (markedExamViewModel.DateOfStart < DateTime.Now)
                {
                    foreach (var user in markedExamViewModel.Users)
                    {
                        ExamResult userExamResult = _mapper.Map<ExamResult>(user);
                        userExamResult.ExamResultIdentificator = _keyGenerator.GenerateNewId();

                        _context.examResultRepository.AddExamResult(userExamResult);
                    }

                    _context.examRepository.SetMaxAmountOfPointsToEarn(markedExamViewModel.ExamIdentificator, markedExamViewModel.MaxAmountOfPointsToEarn);
                }

                return RedirectToAction("ExamDetails", new { examIdentificator = markedExamViewModel.ExamIdentificator, message = "Dokonano oceny egzaminu" });
            }

            return View(markedExamViewModel);
        }

        // GET: DisplayExamSummary
        [Authorize(Roles = "Admin")]
        public ActionResult DisplayExamSummary(string examIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);
            var ExamResults = _context.examResultRepository.GetExamsResultsById(Exam.ExamResults);

            var Users = _context.userRepository.GetUsersById(Exam.EnrolledUsers);
            List<DisplayUserWithExamResults> ListOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var user in Users)
            {
                var userExamResult = ExamResults.Where(z => z.User == user.Id).FirstOrDefault();
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
            }

            return View(ListOfUsers);
        }

        // GET: ExaminerExams
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerExams()
        {
            var User = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            var Exams = _context.examRepository.GetExamsByExaminerId(User.Id);
            var Courses = _context.courseRepository.GetCoursesByExamsId(Exams.Select(z=> z.ExamIdentificator).ToList());

            List<DisplayExamViewModel> ListOfExams = new List<DisplayExamViewModel>();

            foreach (var course in Courses)
            {
                foreach (var exam in course.Exams)
                {
                    Exam examModel = Exams.Where(z => z.ExamIdentificator == exam).FirstOrDefault();

                    DisplayExamViewModel singleExam = _mapper.Map<DisplayExamViewModel>(examModel);
                    singleExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(_context.userRepository.GetUsersById(examModel.Examiners));
                    singleExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(course);

                    ListOfExams.Add(singleExam);
                }
            }

            return View(ListOfExams);
        }
    }
}
