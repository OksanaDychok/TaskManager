using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task_manager.Controllers;
using System.Web.Routing;
using Task_manager.Models;
using System.Web;
using Moq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;

namespace Task_manager.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private AccountController Controller { get; set; }
        private RouteCollection Routes { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }
        private Mock<IOAuthWebSecurity> OAuthWebSecurity { get; set; }
        private Mock<HttpResponseBase> Response { get; set; }
        private Mock<HttpRequestBase> Request { get; set; }
        private Mock<HttpContextBase> Context { get; set; }
        private Mock<UsersContext> UsersContext { get; set; }
        private Mock<IPrincipal> User { get; set; }
        private Mock<IIdentity> Identity { get; set; }

        public AccountControllerTest()
        {
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);
            OAuthWebSecurity = new Mock<IOAuthWebSecurity>(MockBehavior.Strict);

            Context = new Mock<HttpContextBase>(MockBehavior.Strict);

            Controller = new AccountController(WebSecurity.Object, OAuthWebSecurity.Object);

        }

        [TestMethod]
        public void Login_ReturnUrlSet()
        {
            string returnUrl = "/Task/ViewTasks";
            var result = Controller.Login(returnUrl) as ViewResult;
            Assert.IsNotNull(result);

            Assert.AreEqual(returnUrl, Controller.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_UserCanLogin()
        {
            string returnUrl = "/Task/ViewTasks";
            string userName = "oxana";
            string password = "123456";

            WebSecurity.Setup(s => s.Login(userName, password, false)).Returns(true);
            var model = new LoginModel
            {
                UserName = userName,
                Password = password
            };

            var result = Controller.Login(model, returnUrl) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Task", result.RouteValues["controller"]);
            Assert.AreEqual("ViewTasks", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Login_InvalidCredentialsRedisplaysLoginScreen()
        {
            string returnUrl = "/Task/ViewTasks";
            string userName = "user";
            string password = "password";

            WebSecurity.Setup(s => s.Login(userName, password, false)).Returns(false);
            var model = new LoginModel
            {
                UserName = userName,
                Password = password
            };

            var result = Controller.Login(model, returnUrl) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LogOff_UserCanLogOut()
        {
            WebSecurity.Setup(s => s.Logout()).Verifiable();

            var result = Controller.LogOff() as RedirectToRouteResult;
            Assert.IsNotNull(result);

            WebSecurity.Verify(s => s.Logout(), Times.Exactly(1));
        }

        [TestMethod]
        public void Register()
        {
            var result = Controller.Register() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Register_UserCanRegister()
        {
            string userName = "user";
            string password = "password";
            string email = "oxana.dychok@gmail.com";

            WebSecurity.Setup(s => s.CreateUserAndAccount(userName, password, new { Email = email }, false)).Returns(userName);
            WebSecurity.Setup(s => s.Login(userName, password, false)).Returns(true);

            var model = new RegisterModel
            {
                UserName = userName,
                Password = password,
                ConfirmPassword = password
            };

            var result = Controller.Register(model) as RedirectToRouteResult;
            Assert.IsNotNull(result);

            WebSecurity.Verify(s => s.CreateUserAndAccount(userName, password, null, false), Times.Exactly(1));
            WebSecurity.Verify(s => s.Login(userName, password, false), Times.Exactly(1));
        }

        [TestMethod]
        public void Register_ErrorWhileRegisteringCausesFormToBeRedisplayed()
        {
            string userName = "user";
            string password = "password";
            string email = "email@gmail.com";

            WebSecurity.Setup(s => s.CreateUserAndAccount(userName, password, new { email} , false)).Returns(userName);
            WebSecurity.Setup(s => s.Login(userName, password, false)).Throws(
                new MembershipCreateUserException(MembershipCreateStatus.InvalidEmail));

            var model = new RegisterModel
            {
                UserName = userName,
                Email = email,
                Password = password,
                ConfirmPassword = password
            };

            var result = Controller.Register(model) as ViewResult;
            Assert.IsNotNull(result);

            Assert.IsFalse(Controller.ModelState.IsValid);
        }
    }
}

