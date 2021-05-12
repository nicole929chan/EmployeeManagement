using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            throw new Exception("Error in Details View");

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

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photo != null)
                {
                    // 刪除舊圖檔
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, 
                            "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    // 重新上傳圖檔
                    employee.PhotoPath = ProcessUploadedFile(model);
                }               

                // 員工資料真正更新到資料庫
                _employeeRepository.Update(employee);
                
                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
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
            return uniqueFileName;
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
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

                // 表單的欄位(屬性)指定給物件, PhotoPath是最後一個檔名
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
