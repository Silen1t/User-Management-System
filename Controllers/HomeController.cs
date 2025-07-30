using AspNetCoreHero.ToastNotification.Abstractions;
using UserManagementSystem.DataAccess;
using UserManagementSystem.Models;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly INotyfService Notyf;
        public HomeController(INotyfService notyf)
        {
            Notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Logout()
        {
            SessionManager.ResetUserIdSession();
            Notyf.Success("You have been logged out.");
            return RedirectToAction("Index");
        }

        public IActionResult AccountDashboard(bool edit = false)
        {
            if (!SessionManager.IsUserLoggedIn())
            {
                return View("NotSignIn");
            }

            string userId = SessionManager.GetUserId();

            AccountModel account = AccountDataHandler.GetAccount(userId);

            ViewData["isEditing"] = edit;

            return View(account);
        }


        public IActionResult SignUp()
        {
            return View();
        }


        public IActionResult NotSignIn()
        {
            return View();
        }


        public IActionResult NotFound()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel signIn)
        {

            if (!AccountDataHandler.IsValidEmail(signIn.Email))
            {
                Notyf.Error("Invalid email format.");
                return View();
            }

            // Check if the account is exists in the database.
            if (await AccountDataHandler.GetAccount(signIn.Email.ToLower(), signIn.Password)
                == null)
            {
                Notyf.Error("Invalid email or password.");
                return View();
            }

            Notyf.Success("Login successful!");
            return RedirectToAction("AccountDashboard");
        }


        [HttpPost]
        public async Task<IActionResult> EditAccount(AccountModel account)
        {
            account.Email = account.Email.ToLower();
            if (!AccountDataHandler.IsValidEmail(account.Email))
            {
                Notyf.Error("Invalid email format.");
                ViewData["isEditing"] = true;   
                return RedirectToAction("AccountDashboard");
            }

            if (await AccountDataHandler.IsEmailExist(account))
            {
                Notyf.Error("Email already exists.");
                ViewData["isEditing"] = true;
                return RedirectToAction("AccountDashboard");
            }

            ViewData["isEditing"] = false;
            

            await AccountDataHandler.UpdateAccount(account).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Notyf.Success("Account updated successfully.");
                    SessionManager.SetUserIdSession(account.Id);
                }
                else
                {
                    Notyf.Error("Failed to update account.");
                }
            });

            return RedirectToAction("AccountDashboard", account);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            AccountModel account = AccountDataHandler.GetAccount(SessionManager.GetUserId());
            await AccountDataHandler.DeleteAccount(account).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Notyf.Success("Account deleted successfully.");
                }
                else
                {
                    Notyf.Error("Failed to delete account.");
                }
            });

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(AccountModel account)
        {
            account.Email = account.Email.ToLower();

            if (!AccountDataHandler.IsValidEmail(account.Email))
            {
                Notyf.Error("Invalid email format.");
                return View();
            }

            if (await AccountDataHandler.IsEmailExist(account))
            {
                Notyf.Error("Email already exists.");
                return View();
            }

            await AccountDataHandler.CreateAccount(account).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Notyf.Success("Account created successfully.");
                    SessionManager.SetUserIdSession(account.Id);
                }
                else
                {
                    Notyf.Error("Failed to create account.");
                }
            });

            var signedAccount = new SignInModel() { Email = account.Email, Password = account.Password };

            return await SignIn(signedAccount);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("NotFound");
            //return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
