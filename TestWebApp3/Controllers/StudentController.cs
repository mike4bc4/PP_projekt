using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestWebApp3.Data;

namespace TestWebApp3.Controllers
{
    public class StudentController : Controller
    {

		private readonly SchoolDbContext _context;

		public StudentController(SchoolDbContext context) {
			_context = context;
		}

        public IActionResult Index()
        {
            return View();
        }
	
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StudentModel model) {
			if (ModelState.IsValid) {
				Student student = new Student
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					SchoolName = model.SchoolName,
					EmailAddress = model.EmailAddress
				};
				_context.Add(student);
				_context.SaveChanges();
			}
			return View();
		}

		public ActionResult ListStudents() {
			return View(_context.Students.ToList());
		}

    }
}