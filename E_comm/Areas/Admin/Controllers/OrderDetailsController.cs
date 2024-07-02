using E_comm_DataAccess.Data;
using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using Microsoft.AspNetCore.Mvc;
using static E_comm_Models.Models.OrderVM;

namespace E_comm.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderDetailsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork; //what we call this 
        private readonly ApplicationDbContext _context;
        public OrderDetailsController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int? id)
        {
            var orderHeader = _unitOfWork.OrderHeader.firstOrDefault(o => o.Id == id, includeProperties: "ApplicationUser");
            if(orderHeader == null)
            {
                return NotFound();
            }
            var orderItems = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == id, includeProperties: "Product");

            var orderDetailViewModel = new OrderVM
            {
                OrderId = orderHeader.Id,
                Name = orderHeader.ApplicationUser.Name,
                OrderDate = orderHeader.OrderDate.ToString("dd-MM-yyyy HH:mm"),
                OrderItems = orderItems.Select((item, index) => new OrderItem
                {
                    SerialNumber = index +1,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Title,
                    Quantity = item.Count,
                    Price = item.Price
                }).ToList()
            };
            return View(orderDetailViewModel);
        }
        public IActionResult GetAll() 
        {
            var orders = _unitOfWork.OrderHeader.GetAll();
            var orderList = new List<OrderVM>();
            foreach (var order in orders)
            {
                var userName = _context.ApplicationUsers.Where(u=>u.Id == order.ApplicationUserId)
                    .Select(u=>u.Name)
                    .FirstOrDefault();
                var orderViewModel = new OrderVM
                {
                    Name = userName,
                    OrderDate = order.OrderDate.ToString("dd-MM-yyyy HH:mm"),
                    OrderTotal = order.OrderTotal,
                    OrderId = order.Id
                };
                orderList.Add(orderViewModel);
            }
            return Json(new { data = orderList });
        }
    }
}
