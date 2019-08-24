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
    }
}