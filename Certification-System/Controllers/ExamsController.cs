﻿using AutoMapper;
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
                ExamTerms = new List<AddExamTermViewModel>()
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

                _context.examRepository.AddExam(exam);
                _context.examTermRepository.AddExamsTerms(examsTerms);

                return RedirectToAction("ConfirmationOfActionOnExam", new { examIdentificator = exam.ExamIdentificator, TypeOfAction = "Add" });
            }

            newExam.AvailableCourses = _context.courseRepository.GetActiveCoursesAsSelectList().ToList();
            newExam.AvailableExaminers = _context.userRepository.GetExaminersAsSelectList().ToList();

            return View(newExam);
        }

        // GET: GetAddExamTermViewComponent
        [Authorize(Roles = "Admin")]
        public ActionResult GetAddExamTermViewComponent(string orderNumber)
        {
            return ViewComponent("AddExamTerm", new { orderNumber = orderNumber });
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
    }
}

