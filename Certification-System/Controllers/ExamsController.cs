using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public ActionResult ExamDetails(string examIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);
            var ExamTerms = _context.examTermRepository.GetExamsTermsById(Exam.ExamTerms);
            var ExamResults = _context.examResultRepository.GetExamsResultsById(Exam.ExamResults);

            List<DisplayExamTermViewModel> ListOfExamTerms = _mapper.Map<List<DisplayExamTermViewModel>>(ExamTerms);

            List<string> ExaminersIdentificators = Exam.Examiners.ToList();
            List<string> UsersIdentificators = Exam.EnrolledUsers.ToList();

            foreach (var examTerm in ExamTerms)
            {
                ExaminersIdentificators.AddRange(examTerm.Examiners);
                UsersIdentificators.AddRange(examTerm.EnrolledUsers);
            }
            ExaminersIdentificators.Distinct();
            UsersIdentificators.Distinct();

            var Examiners = _context.userRepository.GetUsersById(ExaminersIdentificators);
            List<DisplayCrucialDataWithContactUserViewModel> ListOfExaminers = _mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Examiners);

            var Users = _context.userRepository.GetUsersById(UsersIdentificators);
            List<DisplayUserWithExamResults> ListOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var user in Users)
            {
                var userExamResult = ExamResults.Where(z => z.User == user.Id).FirstOrDefault();

                var UserWithExamResult = Mapper.Map<DisplayUserWithExamResults>(user);
                UserWithExamResult = _mapper.Map<DisplayUserWithExamResults>(userExamResult);
            }

            bool ExamNotMarked = false;

            if (ExamResults.Count() != 0)
            {
                ExamNotMarked = true;
            }

            var Course = _context.courseRepository.GetCourseByExamId(examIdentificator);
            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(Course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            ExamDetailsViewModel ExamDetails = _mapper.Map<ExamDetailsViewModel>(Exam);
            ExamDetails.ExamTerms = ListOfExamTerms;
            ExamDetails.Course = courseViewModel;

            ExamDetails.Examiners = ListOfExaminers;
            ExamDetails.EnrolledUsers = ListOfUsers;

            ExamDetails.ExamNotMarked = ExamNotMarked;

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

                DisplayExamWithTermsViewModel modifiedExam = _mapper.Map<DisplayExamWithTermsViewModel>(Exam);

                if (Exam.ExamTerms.Count() != 0)
                {
                    var ExamTerms = _context.examTermRepository.GetExamsTermsById(Exam.ExamTerms);
                    modifiedExam.ExamTerms = _mapper.Map<List<DisplayExamTermViewModel>>(ExamTerms);

                    foreach (var examTerm in modifiedExam.ExamTerms)
                    {
                        var SingleExamTermExaminers = _context.userRepository.GetUsersById(ExamTerms.Where(z => z.ExamTermIdentificator == examTerm.ExamTermIdentificator).Select(z=> z.Examiners).FirstOrDefault());

                        examTerm.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(SingleExamTermExaminers);
                    }
                }

                modifiedExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                modifiedExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Examiners);

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

            else if(addExamTypeOfAction.SelectedOption == "addExamPeriod" && addExamTypeOfAction.ExamTermsQuantity == 0)
                return RedirectToAction("AddNewExamPeriod", "Exams", new { courseIdentificator = addExamTypeOfAction.CourseIdentificator, examIdentificator = addExamTypeOfAction.ExamIdentificator, quantityOfExamTerms = addExamTypeOfAction.ExamTermsQuantity });

            else if(addExamTypeOfAction.SelectedOption == "addExamPeriod" && addExamTypeOfAction.ExamTermsQuantity != 0)
                return RedirectToAction("AddNewExamPeriodWithExamTerms", "Exams", new { courseIdentificator = addExamTypeOfAction.CourseIdentificator, examIdentificator = addExamTypeOfAction.ExamIdentificator, quantityOfExamTerms = addExamTypeOfAction.ExamTermsQuantity });

            return RedirectToAction("BlankMenu", "Certificates");
            }

            addExamTypeOfAction.AvailableOptions = _context.examRepository.GetAddExamMenuOptions().ToList();

            return View(addExamTypeOfAction);
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

                singleExamResult.MaxAmountOfPoints = RelatedExam.MaxAmountOfPointsToEarn;

                singleExamResult.Exam = _mapper.Map<DisplayCrucialDataExamViewModel>(RelatedExam);
                singleExamResult.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(RelatedCourse);
                singleExamResult.User = _mapper.Map<DisplayCrucialDataUserViewModel>(RelatedUser);

                ListOfExamsResults.Add(singleExamResult);
            }

            return View(ListOfExamsResults);
        }
    }
}
