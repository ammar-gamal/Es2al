using Es2al.Services.CustomException;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Es2al.Filters
{
    public class CustomeErrorHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                AppException? appException = context.Exception as AppException;
                if(appException != null)
                {
                    context.HttpContext.Response.StatusCode = appException.StatusCode;
                    context.Result = new ContentResult() { Content = context.Exception.Message };
                }
                else
                {
                    context.HttpContext.Response.StatusCode = 500;
                    context.Result = new ContentResult() { Content = "An unexpected error occurred." };
                }
                var controllerName = context.RouteData.Values["controller"];
                var actionName = context.RouteData.Values["action"];

                string message = $"\nTime: {DateTime.Now}, Controller: {controllerName}, Action: {actionName}, Exception: {context.Exception.Message}";
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(),"Log");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                File.AppendAllText(Path.Combine(folderPath,"Log.txt"), message);
                context.ExceptionHandled = true;
            }
        }
    }
}
