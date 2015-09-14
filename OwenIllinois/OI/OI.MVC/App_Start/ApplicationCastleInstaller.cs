using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using OI.Entities;
using OI.Entities.Models;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace OI.MVC
{
    public class ApplicationCastleInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Register working dependencies
            container.Register(Component.For<IDataContextAsync>().ImplementedBy<OIDataContext>().LifestylePerWebRequest());
            container.Register(Component.For<IUnitOfWorkAsync>().ImplementedBy<UnitOfWork>().LifestylePerWebRequest());
           container.Register(Component.For<IRepositoryAsync<Employee>>().ImplementedBy<Repository<Employee>>().LifestylePerWebRequest());
            container.Register(Component.For<IRepositoryAsync<Account>>().ImplementedBy<Repository<Account>>().LifestylePerWebRequest());
            container.Register(Component.For<IRepositoryAsync<Document>>().ImplementedBy<Repository<Document>>().LifestylePerWebRequest());
            container.Register(Component.For<IRepositoryAsync<Role>>().ImplementedBy<Repository<Role>>().LifestylePerWebRequest());

            var contollers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Controller)).ToList();
            foreach (var controller in contollers)
            {
                container.Register(Component.For(controller).LifestylePerWebRequest());
            }

        }
    }
}