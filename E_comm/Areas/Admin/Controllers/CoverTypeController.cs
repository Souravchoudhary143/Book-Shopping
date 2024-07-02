using Dapper;
using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using E_comm_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace E_comm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
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
            return Json(new { data = _unitOfWork.SPCALL.List<CoverType>(SD.SP_GetCoverTypes) });
            //return Json(new { data = _unitOfWork.CoverType.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitOfWork.CoverType.Get(id);
            if(coverTypeInDb ==null)
                    return Json(new { success = false, message = "Something Went Wrong While Deleting Data!!" });
            DynamicParameters param= new DynamicParameters();
            param.Add("id", id);
            _unitOfWork.SPCALL.Execute(SD.SP_DeleteCoverType,param);
           // _unitOfWork.CoverType.Remove(coverTypeInDb);
           // _unitOfWork.Save();
            return Json(new { success = true, message = "Data Deleted!!!" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            CoverType coverType= new CoverType();
            if(id==null) return View (coverType);
            DynamicParameters param= new DynamicParameters();
            param.Add("id",id.GetValueOrDefault());
            coverType = _unitOfWork.SPCALL.OneRecord<CoverType>(SD.SP_GetCoverType,param);

            //coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault());
            if(coverType ==null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if(coverType==null) return BadRequest();
            if (!ModelState.IsValid) return View(coverType);
            DynamicParameters param= new DynamicParameters();
            param.Add("name", coverType.Name);
            if (coverType.Id == 0) {
                // _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.SPCALL.Execute(SD.SP_CreateCoverType, param);
            }
              
            else {
                param.Add("id", coverType.Id);
                _unitOfWork.SPCALL.Execute(SD.SP_UpdateCoverType, param);
            }
                     
                    
              //  _unitOfWork.CoverType.Update(coverType);
          //_unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
