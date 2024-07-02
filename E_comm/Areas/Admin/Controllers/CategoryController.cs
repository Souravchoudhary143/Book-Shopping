﻿using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using E_comm_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_comm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return Json(new {data=categoryList});
        }
        //api for delete
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if(categoryInDb == null)
                return Json(new {Success = false,message="Somethingwent wrong while deletedata!!!"});
            _unitOfWork.Category.Remove(categoryInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted Successfully!!!" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null) return View (category);
            //Edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null) return  NotFound();
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert (Category category) 
        {
            if (category == null) return BadRequest ();
            if (!ModelState.IsValid) return View(category);
            if(category.Id ==0)
                _unitOfWork.Category.Add(category);
            else 
                _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        
        }
    }
}
