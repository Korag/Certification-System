using AutoMapper;
using Certification_System.DTOViewModels;
using Certification_System.Entities;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList(),
                AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList(),
                ExamTerms = new List<AddExamTermWithoutExamViewModel>()
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
                exam.DurationMinutes = (int)exam.DateOfEnd.Subtract(exam.DateOfStart).TotalMinutes;
                exam.OrdinalNumber = course.Exams.Count();

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
                }
                var ExamsTermsIdentificators = examsTerms.Select(z => z.ExamTermIdentificator);

                _context.examRepository.AddExam(exam);
                _context.examTermRepository.AddExamsTerms(examsTerms);

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, examsTermsIdentificators = ExamsTermsIdentificators, TypeOfAction = "Add" });
            }

            newExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            newExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExam);
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
            List<DisplayCrucialDataWithContactUserViewModel> ListOfExaminers = Mapper.Map<List<DisplayCrucialDataWithContactUserViewModel>>(Examiners);

            var Users = _context.userRepository.GetUsersById(UsersIdentificators);
            List<DisplayUserWithExamResults> ListOfUsers = new List<DisplayUserWithExamResults>();

            foreach (var user in Users)
            {
                var userExamResult = ExamResults.Where(z => z.User == user.Id).FirstOrDefault();

                var UserWithExamResult = Mapper.Map<DisplayUserWithExamResults>(user);
                UserWithExamResult = _mapper.Map<DisplayUserWithExamResults>(userExamResult);
            }

            var Course = _context.courseRepository.GetCourseByExamId(examIdentificator);
            DisplayCourseViewModel courseViewModel = _mapper.Map<DisplayCourseViewModel>(Course);
            courseViewModel.Branches = _context.branchRepository.GetBranchesById(Course.Branches);

            ExamDetailsViewModel ExamDetails = _mapper.Map<ExamDetailsViewModel>(Exam);
            ExamDetails.ExamTerms = ListOfExamTerms;
            ExamDetails.Course = courseViewModel;

            ExamDetails.Examiners = ListOfExaminers;
            ExamDetails.EnrolledUsers = ListOfUsers;

            return View();
        }

        // GET: ConfirmationOfActionOnExam
        [Authorize(Roles = "Admin")]
        public ActionResult ConfirmationOfActionOnExam(string examIdentificator, ICollection<string> examsTermsIdentificators, string TypeOfAction)
        {
            if (examIdentificator != null)
            {
                ViewBag.TypeOfAction = TypeOfAction;

                var Exam = _context.examRepository.GetExamById(examIdentificator);
                var Examiners = _context.userRepository.GetUsersById(Exam.Examiners);

                var Course = _context.courseRepository.GetCourseByExamId(examIdentificator);

                DisplayExamWithTermsViewModel modifiedExam = _mapper.Map<DisplayExamWithTermsViewModel>(Exam);

                if (examsTermsIdentificators.Count() != 0)
                {
                    var ExamTerms = _context.examTermRepository.GetExamsTermsById(examsTermsIdentificators);
                    modifiedExam.ExamTerms = _mapper.Map<List<DisplayExamTermViewModel>>(ExamTerms);

                    foreach (var examTerms in ExamTerms)
                    {
                        List<string> ExamTermsExaminersIdentificators = new List<string>();
                        var SingleExamTermExaminers = _context.userRepository.GetUsersById(examTerms.Examiners);

                        modifiedExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(SingleExamTermExaminers);
                    }
                }

                modifiedExam.Course = _mapper.Map<DisplayCrucialDataCourseViewModel>(Course);
                modifiedExam.Examiners = _mapper.Map<List<DisplayCrucialDataUserViewModel>>(Examiners);

                return View(modifiedExam);
            }

            return RedirectToAction(nameof(AddNewExam));
        }

        // GET: EditExam
        [Authorize(Roles = "Admin")]
        public ActionResult EditExam(string examIdentificator)
        {
            var Exam = _context.examRepository.GetExamById(examIdentificator);
            var ExamTerms = _context.examTermRepository.GetExamsTermsById(Exam.ExamTerms);

            EditExamViewModel examToUpdate = _mapper.Map<EditExamViewModel>(Exam);
            examToUpdate.ExamTerms = _mapper.Map<List<EditExamTermViewModel>>(ExamTerms);

            examToUpdate.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            examToUpdate.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

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
                    //todo delete related ExamTerms
                }

                OriginExam = _mapper.Map<EditExamViewModel, Exam>(editedExam, OriginExam);
                OriginExamTerms = _mapper.Map<List<EditExamTermViewModel>, List<ExamTerm>>(editedExam.ExamTerms.ToList(), OriginExamTerms.ToList());

                //todo archive ExamTerms when it is in deletion process

                OriginExam.ExamTerms = editedExam.ExamTerms.Select(z => z.ExamTermIdentificator).ToList();

                _context.examRepository.UpdateExam(OriginExam);
                _context.examTermRepository.UpdateExamsTerms(OriginExamTerms);

                return RedirectToAction("ConfirmationOfActionOnExam", "Exams", new { examIdentificator = editedExam.ExamIdentificator, examsTermsIdentificators = editedExam.ExamTerms.Select(z=> z.ExamTermIdentificator).ToList(), TypeOfAction = "Update" });
            }

            editedExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();
            editedExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();

            return View(editedExam);
        }
    }
}
