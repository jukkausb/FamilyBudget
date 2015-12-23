using FamilyBudget.Www.Controllers;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace FamilyBudget.Www
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        IUnityContainer _container;

        public UnityControllerFactory(IUnityContainer container)
        {
            _container = container;
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            if (controllerName == "ErrorHandler")
                return typeof (ErrorHandlerController);

            return base.GetControllerType(requestContext, controllerName);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) return null;

            if (!typeof(IController).IsAssignableFrom(controllerType))
                throw new ArgumentException(string.Format(
                  "Type requested is not a controller: {0}", controllerType.Name),
                  "controllerType");

            return _container.Resolve(controllerType) as IController;
        }
    }
}