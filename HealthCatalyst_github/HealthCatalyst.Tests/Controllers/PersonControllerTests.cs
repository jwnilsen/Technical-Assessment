 
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HealthCatalyst.Controllers;
using HealthCatalyst.Models;

namespace HealthCatalyst.Tests.Controllers
{
    // -------------------------------------------------------------------------------------
    // All functionality invoked by the controllers has been tested by the PersonAgentTests.
    // -------------------------------------------------------------------------------------
    // These test will verify that the controller routing actions are operational.
    // If a controller requires no inputs, its view result will be evaluated for NotNull
    // Else
    //    no input will be provided and it will fail with a redirect to an error view.

    [TestClass]
    public class PersonControllerTests
    {

        [TestMethod]
        public void PersonController_Create()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PersonController_CreateWithViewModelUpdate()
        {
            // Arrange
            PersonController controller = new PersonController();
            PersonViewModel viewModel = new PersonViewModel();

            // Act
            // viewModel is null - will redirect to error page
            var result = (RedirectToRouteResult) controller.Create(viewModel);

            // Assert
            Assert.IsTrue(result.RouteValues["action"].ToString() == "FatalError");
        }

        [TestMethod]
        public void PersonController_Delete()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            // this person does not exist - will redirect to error page
            var result = (RedirectToRouteResult)controller.Delete(0);

            // Assert
            Assert.IsTrue(result.RouteValues["action"].ToString() == "FatalError");
        }

        [TestMethod]
        public void PersonController_DeleteConfirmed()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            // this person does not exist - will redirect to error page
            var result = (RedirectToRouteResult)controller.DeleteConfirmed(0);

            // Assert
            Assert.IsTrue(result.RouteValues["action"].ToString() == "FatalError");
        }

        [TestMethod]
        public void PersonController_Details()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            // this person does not exist - will redirect to error page
            var result = (RedirectToRouteResult)controller.Details(0);

            // Assert
            Assert.IsTrue(result.RouteValues["action"].ToString() == "FatalError");
        }

        [TestMethod]
        public void PersonController_Edit()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            // this person does not exist - will redirect to error page
            var result = (RedirectToRouteResult)controller.Edit(0);

            // Assert
            Assert.IsTrue(result.RouteValues["action"].ToString() == "FatalError");
        }

        [TestMethod]
        public void PersonController_EditWithViewModelUpdate()
        {
            // Arrange
            PersonController controller = new PersonController();
            PersonViewModel viewModel = new PersonViewModel();

            // Act
            // this person does not exist - will redirect to error page
            var result = (RedirectToRouteResult)controller.Edit(viewModel);

            // Assert
            Assert.IsTrue(result.RouteValues["action"].ToString() == "FatalError");
        }

        [TestMethod]
        public void PersonController_FatalError()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            ViewResult result = controller.FatalError() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PersonController_Index()
        {
            // Arrange
            PersonController controller = new PersonController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
