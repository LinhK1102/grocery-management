using Microsoft.AspNetCore.Mvc;
using static GroceryWebApp.Constants.SystemMessages;

namespace GroceryWebApp.Helpers
{
    public static class ControllerExtensions
    {
        public static async Task<IActionResult> RedirectWithStatusAsync(this Controller controller,
            bool result,
            string successMessage,
            string errorMessage,
            string redirectAction,
            object routeValues = null,
            object model = null,
            Func<Task> loadViewData = null) 
        {
            controller.TempData[StatusConstants.TempDataStatus] = result ? StatusConstants.Success : StatusConstants.Error;
            controller.TempData[StatusConstants.TempDataMessage] = result ? successMessage : errorMessage;

            if (result)
                return controller.RedirectToAction(redirectAction, routeValues);

            if (loadViewData != null)
                await loadViewData();

            return controller.View(model);
        }

    }
}
