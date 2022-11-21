using CI_Platform.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace CI_Platform.Controllers
{
    public class UserController : Controller
    {   
        //Login
        //get
        User user = new User();
        public CI_PlatformContext _db = new CI_PlatformContext();
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User model)
        {   
            var user = _db.Users.FirstOrDefault(u => u.Email.Equals(model.Email.ToLower()) && u.Password.Equals(model.Password));

            if(user==null)
            {
                return View("Error");
            }
            return RedirectToAction("Index","Home");
        }
        //Login End



        //Registration 
        //get
        public async Task<IActionResult> Create()
        {
            return View();
        }


        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model)
        {

            if (ModelState.IsValid)
            {
                _db.Users.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        //Registration End
    }
}
