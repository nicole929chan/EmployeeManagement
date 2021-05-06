using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace EmployeeManagement.Controllers
{    
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        public HomeController(IEmployeeRepository employeeRepository,
                              IWebHostEnvironment webHostEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();

            return View(model);       
        }

        public ViewResult Details(int? id)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id ?? 1),
                PageTitle = "Employee Detail"
            };

            return View(homeDetailsViewModel);
        }        

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    // 資料夾路徑
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");

                    // 檔名
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;

                    // 完整路徑
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 上傳檔案
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                // 表單的欄位(屬性)指定給物件
                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                // 員工資料真正新增到資料庫
                _employeeRepository.Add(newEmployee);
            
                // EFCore在儲存資料後會自動把Id再加到物件
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }
    }
}
