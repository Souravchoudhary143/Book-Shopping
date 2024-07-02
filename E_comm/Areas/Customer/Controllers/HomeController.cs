using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using E_comm_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace E_comm.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;


        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity=(ClaimsIdentity)(User.Identity);
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll
                    (sc=>sc.ApplicationUserId==claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            }            
            var productList = _unitOfWork.Product.GetAll();

            foreach (var product in productList)
            {
                var totalBookSoled = _unitOfWork.OrderDetail.GetAll(od => od.ProductId == product.Id).Sum(od => od.Count);
                product.QyntBookSoled = totalBookSoled;
            }
            productList = productList.OrderByDescending(p=>p.QyntBookSoled).ToList();

            return View(productList);
        }
        public IActionResult Details(int id)
        {   
            var claimsIdentity=(ClaimsIdentity)(User.Identity) ;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll
                    (sc=>sc.ApplicationUser.Id == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCartCount,count);
            }
            ///****
            var productInDb = _unitOfWork.Product.firstOrDefault 
                (p=>p.Id== id, includeProperties: "category,coverType"); // get the product id and properties of category and covertype
            if(productInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,  //this line add product 
                ProductId = productInDb.Id, //this line add product id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)(User.Identity);
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if(claim == null) return NotFound();
                shoppingCart.ApplicationUserId = claim.Value;

                var shoppingCartInDb = _unitOfWork.ShoppingCart.firstOrDefault
                    (sc => sc.ApplicationUserId == claim.Value && sc.ProductId == shoppingCart.ProductId);

                if (shoppingCartInDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartInDb.Count += shoppingCart.Count;
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                var productInDb = _unitOfWork.Product.firstOrDefault
                (p => p.Id == shoppingCart.Id, includeProperties: "Category,CoverType"); // get the product id and properties of category and covertype
                if (productInDb == null) return NotFound();
                var shoppingCartEdit = new ShoppingCart()
                {
                    Product = productInDb,  //this line add product 
                    ProductId = productInDb.Id, //this line add product id
                };
                return View(shoppingCartEdit);
            }
           
            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}