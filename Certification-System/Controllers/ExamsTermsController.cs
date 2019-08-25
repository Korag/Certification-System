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

            if (string.IsNullOrWhiteSpace(examIdentificator))
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
        public ActionResult ExamTermDetails(string examTermIdentificator)
        {
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

            bool ExamTermNotMarked = false;

            if (ExamTermResults.Count() != 0)
            {
                ExamTermNotMarked = true;
            }

            foreach (var user in EnrolledUsers)
            {
                DisplayUserWithExamResults singleUser = _mapper.Map<DisplayUserWithExamResults>(EnrolledUsers);

                if (!ExamTermNotMarked)
                {
                    ExamResult userResult = ExamTermResults.ToList().Where(z => z.User == user.Id).FirstOrDefault();

                    singleUser = _mapper.Map<DisplayUserWithExamResults>(userResult);
                }

                usersViewModel.Add(singleUser);
            }

            ExamTermDetailsViewModel ExamTermDetails = _mapper.Map<ExamTermDetailsViewModel>(ExamTerm);
            ExamTermDetails.Exam = examViewModel;
            ExamTermDetails.Examiners = examinersViewModel;

            ExamTermDetails.Course = courseViewModel;
            ExamTermDetails.UsersWithResults = usersViewModel;

            ExamTermDetails.MaxAmountOfPointsToEarn = Exam.MaxAmountOfPointsToEarn;
            ExamTermDetails.ExamTermNotMarked = ExamTermNotMarked;

            return View(ExamTermDetails);
        }
    }
}