using E_comm_DataAccess.Data;
using E_comm_DataAccess.Repository;
using E_comm_DataAccess.Repository.IRepository;
using E_comm_Models.Models;
using E_comm_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Net.Mail;
using System.Net;

namespace E_comm.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static bool _isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ISMSSender _smsSender;

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager, IConfiguration configuration, ISMSSender smsSender)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailSender = emailSender;
            _userManager = userManager;
            _smsSender = smsSender;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            //*****
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.firstOrDefault(au => au.Id == claim.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
                }
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitOfWork.ApplicationUser.firstOrDefault(au => au.Id == claims.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is Empty");
            }
            else
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //
                _isEmailConfirm = true;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.firstOrDefault(c => c.Id == id);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.ShoppingCart.firstOrDefault(c => c.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.ShoppingCart.firstOrDefault(c => c.Id == id);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            //session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            //***
            return RedirectToAction("Index");
        }

        public IActionResult UpdateSelectedItem(int itemId, bool isChecked)
        {
            var cartItem = _unitOfWork.ShoppingCart.firstOrDefault(c => c.Id == itemId);
            if (cartItem != null)
            {
                cartItem.selectedItems = isChecked;
                _unitOfWork.Save(); // Assuming you have a method to save changes
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public IActionResult Summary()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var Address = _unitOfWork.OrderHeader.GetAll(ad => ad.ApplicationUserId == claims.Value).Select
                (ad => ad.StreetAddress + ", " + ad.City + ", " + ad.State + ", " + ad.PostalCode).Distinct().ToList();
            ShoppingCartVM = new ShoppingCartVM()
            {
                Address = Address,
                ListCart = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value && sc.selectedItems, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.firstOrDefault(au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
                    list.Product.Price50, list.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "....";
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            /*if (!_isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent please check your email"; //viewbsge
                ViewBag.EmailCSS = "text-success";
                _isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must be confiremed";
                ViewBag.EmailCSS = "text-danger";
              }*/

            return View(ShoppingCartVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            /*var user = _unitOfWork.ApplicationUser.firstOrDefault(au => au.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is Empty");
            }
            else
            {
                // Call SendEmailConfirmationAsync as a task
                Task.Run(() => SendEmailConfirmationAsync(user));
                _isEmailConfirm = true;
             }*/

            //****
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.firstOrDefault
                (au => au.Id == claim.Value);
            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll
                (sc => sc.ApplicationUserId == claim.Value && sc.selectedItems, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.Product.Price,
                    list.Product.Price50, list.Product.Price100);
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = list.ProductId,
                    Price = list.Price,
                    Count = list.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += (list.Count * list.Price);
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();

           /* try
            {
                await _emailSender.SendEmailAsync(ShoppingCartVM.OrderHeader.ApplicationUser.Email, "Confirm your email",$"confirmed:your order with Id: #{ShoppingCartVM.OrderHeader.Id} has been confirmed! we will send it as soon as possible");
            }
            catch(Exception ex)
            {

            }

            //sms twilio 
            try
            {
                TwilioClient.Init("AC38260f78ae55d9e6ba794af20da31a97", "52f63a31d441b5e360a7cc6b5c2e19bb");

                var toPhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
                var message = MessageResource.Create(
                    body: $"CONFIRMED: Your Order With ID:#{ShoppingCartVM.OrderHeader.Id} has been confirmed!We will send your order as soon as possible ",
                    from: new Twilio.Types.PhoneNumber("+18889606205"),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber));
                }
            catch (Exception ex)
            {

            }
            return RedirectToAction("OrderConfirmation", "Cart", new {id=ShoppingCartVM.OrderHeader.Id});*/


            //stripe
            #region Stripe
            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentstatusDelayPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "Order Id : " + ShoppingCartVM.OrderHeader.Id.ToString(),
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PymentStatusRejected;
                else
                    ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                if (charge.Status.ToLower() == "succeded")
                {
                    ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentStatus = SD.PymentstatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }
                _unitOfWork.Save();
            }
            #endregion

            //return RedirectToAction(nameof(Summary));
            return RedirectToAction("OrderConfirmation", "Cart",
                    new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            var order = _unitOfWork.OrderHeader.firstOrDefault(o=>o.Id == id);
            SendOrderConfirmationSms(order);
            SendConfirmationMail(order);
            //session count  refresh the cart after placing the order
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
           
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.SS_SessionCartCount, count);
            }
            return View();
        }

        /*private async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
          }*/
        public void SendOrderConfirmationSms(OrderHeader order)
        {
            _smsSender.SendSms(order.PhoneNumber, "ThankYou for shopping with us! " +
                "Your order with Order ID #" + order.Id + "will we shipped after 3 days");
        }

        public void SendConfirmationMail(OrderHeader order)
        {
            var claimIdentity = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            order.ApplicationUser = _unitOfWork.ApplicationUser.firstOrDefault(au=>au.Id == claim.Value);
            var emailSettings = _configuration.GetSection("EmailSettings");

            var fromAddres = new MailAddress(emailSettings["FromEmail"], "Your Name");
            var toAddress = new MailAddress(order.ApplicationUser.Email, order.Name);
            var ccAddress = new MailAddress(emailSettings["CcEmail"]);
           // var bccAddress = new MailAddress(emailSettings["BccEmail"]);
            string subject = "Order Confirmation - Order #" + order.Id;
            string body = "Dear " + order.ApplicationUser.Name + ",\n\n"
                + "Thank You For Shopping with us with Order Id #" + order.Id + " Has been Successfully placed.\n"
                + "Total Amount: $" + order.OrderTotal + "\n\n"
                + "For any queries or concerns, please contact our customer support.\n\n"
                + "Regards, \nShoppingCart";

            var smtp = new SmtpClient
            {
                Host = emailSettings["PrimaryDomain"],
                Port = Convert.ToInt32(emailSettings["PrimaryPort"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings["UsernameEmail"], emailSettings["UserPassword"])
            };
            using var message = new MailMessage(fromAddres, toAddress)
            {
                Subject = subject,
                Body = body,
            };
            message.CC.Add(ccAddress);
            //message.Bcc.Add(bccAddress);

            smtp.Send(message);
        }
    }

}

