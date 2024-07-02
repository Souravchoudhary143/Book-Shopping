using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using E_comm_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_comm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll();
            return Json(new {data=productList});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitOfWork.Product.Get(id);
            if(productInDb ==null)
                return Json(new {success=false,
            message = "Something Went Wrong While delete the data!!"});
            //image delete
            var webRootPath = _webHostEnvironment.WebRootPath;
            var imagepath = Path.Combine(webRootPath,productInDb.ImageURL.Trim('\\'));
            if(System.IO.File.Exists(imagepath))
            {
                System.IO.File.Delete(imagepath);
            }
            _unitOfWork.Product.Remove(productInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data deleted successfully !!" });
        }

        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList=_unitOfWork.CoverType.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                })
            };
            if(id==null) return View(productVM);
            productVM.Product=_unitOfWork.Product.Get(id.GetValueOrDefault());
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVm)
        { 
            if(ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath; //used to give the image path
                var files = HttpContext.Request.Form.Files; 
                
                if (files.Count() > 0)
                {
                    var fileName=Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads=Path.Combine(webRootPath, @"Image\Products");
                    if(productVm.Product.Id != 0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVm.Product.Id).ImageURL;
                        productVm.Product.ImageURL = imageExists;
                    }
                    if(productVm.Product.ImageURL !=null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVm.Product.ImageURL.Trim('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {   
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var fileStream=new FileStream(Path.Combine(uploads,fileName+extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVm.Product.ImageURL = @"\Image\Products\" + fileName + extension;    
                }
                else
                {
                    if(productVm.Product.Id !=0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVm.Product.Id).ImageURL;
                        productVm.Product.ImageURL = imageExists;
                    }
                }
                if(productVm.Product.Id==0) 
                    _unitOfWork.Product.Add(productVm.Product);
                else
                    _unitOfWork.Product.Update(productVm.Product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
               
            }
            else
            {
                productVm = new ProductVM()
                {
                    Product = new Product(),
                    CategoryList = _unitOfWork.Category.GetAll().Select
                    (cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select
                    (cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    })
                };
                if(productVm.Product.Id!=0)
                {
                    productVm.Product = _unitOfWork.Product.Get(productVm.Product.Id);
                }
                return View(productVm);
            }

        }
    }
}
