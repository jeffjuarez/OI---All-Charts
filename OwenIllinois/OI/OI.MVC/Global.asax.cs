using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using Castle.Windsor;
using OI.Entities;
using OI.Entities.Models;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace OI.MVC
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Initialize Castle & install application components
            var container = new WindsorContainer();
            container.Install(new ApplicationCastleInstaller());
 
            // Create the Controller Factory
            var castleControllerFactory = new CastleControllerFactory(container);
 
            // Add the Controller Factory into the MVC web request pipeline
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);        
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (FormsAuthentication.CookiesSupported != true) return;
            if (Request.Cookies[FormsAuthentication.FormsCookieName] == null) return;

            //let us take out the username now                
            var formsAuthenticationTicket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            if (formsAuthenticationTicket == null) return;
            
            var username = formsAuthenticationTicket.Name;
            var roles = String.Empty;

            using (IDataContextAsync context = new OIDataContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IRepositoryAsync<Account> accountRepository = new Repository<Account>(context, unitOfWork);
                var user =
                    accountRepository.Query(u => u.Username == username).Include(r => r.Role).Select().SingleOrDefault();

                if (user != null) roles = user.Role.RoleType;
            }

            //Let us set the Pricipal with our user specific details
            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
        }
    }
}
