using AutoMapper;
using Certification_System.Repository.DAL;
using Certification_System.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Certification_System.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly MongoOperations _context;

        private readonly IMapper _mapper;
        private readonly IKeyGenerator _keyGenerator;
        private readonly ILogService _logger;
        private readonly IEmailSender _emailSender;

        public NotificationsController(
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

        // GET: AdminNotificationManager
        [Authorize(Roles = "Admin")]
        public ActionResult AdminNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            // get CourseQueue into dataTable with 2 actions: Remove, Assign and checkbox if 2 weeks gone since someone enrolled to course and not paid
            // list of courses which are old -> they ended but noone do EndCourseAndDispenseGivenCertificates

            return View();
        }

        // GET: InstructorNotificationManager
        [Authorize(Roles = "Instructor")]
        public ActionResult InstructorNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: ExaminerNotificationManager
        [Authorize(Roles = "Examiner")]
        public ActionResult ExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: InstructorExaminerNotificationManager
        [Authorize(Roles = "Instructor")]
        [Authorize(Roles = "Examiner")]
        public ActionResult InstructorExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: CompanyNotificationManager
        [Authorize(Roles = "Company")]
        public ActionResult CompanyExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);


            return View();
        }

        // GET: WorkerNotificationManager
        [Authorize(Roles = "Worker")]
        public ActionResult WorkerExaminerNotificationManager()
        {
            var user = _context.userRepository.GetUserByEmail(this.User.Identity.Name);

            return View();
        }
    }
}