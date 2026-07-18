using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Web.Areas.Admin
{
    /// <summary>
    /// Đăng ký Area "Admin" để hệ thống nhận diện route theo pattern /Admin/ControllerName/ActionName
    /// </summary>
    public class AdminAreaRegistration
    {
        public static void RegisterArea(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "admin_default",
                pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
                defaults: new { area = "Admin" });
        }
    }
}
