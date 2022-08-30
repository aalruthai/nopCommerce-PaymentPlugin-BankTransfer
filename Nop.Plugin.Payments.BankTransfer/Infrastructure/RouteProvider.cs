using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.BankTransfer.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(BankTransferDefaults.UploadFileRouteName,
                "Plugins/PaymentBankTransfer/UploadFile",
                new { controller = "PaymentBankTransferProcessor", action = "UploadFileBankTransfer" });
        }
    }
}
