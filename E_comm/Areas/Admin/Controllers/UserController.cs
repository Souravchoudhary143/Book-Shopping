using E_comm_DataAccess.Data;
using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using E_comm_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace E_comm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
                _context= context;
            _unitOfWork= unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList(); //ASPNETUSERS from here we take user id
            var roles = _context.Roles.ToList(); //ASPNETROLES from here we take role name
            var userRoles = _context.UserRoles.ToList(); //ASPNETUSERROLES role id
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId; //here we get the role id
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name; // from roleid we get the role name
                if (user.CompanyId != null)
                {
                    user.Company = new Company()
                        {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
            if(user.CompanyId == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            //remove admin user
            //var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            var adminUser = userList.FirstOrDefault(u =>u.Role == SD.Role_Admin);
            userList.Remove(adminUser);
            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            bool isLocked = false;
            var userInDb = _context.ApplicationUsers.FirstOrDefault(au=>au.Id== id);
            if (userInDb == null)
                return Json(new { success = false, message = "Somehting went wrong while lock or unlock user" });
            if(userInDb !=null && userInDb.LockoutEnd>DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked= true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message=isLocked==true? 
                "User successfully Locked": "User successfully Unlocked"});
        }
        #endregion
    }
}
