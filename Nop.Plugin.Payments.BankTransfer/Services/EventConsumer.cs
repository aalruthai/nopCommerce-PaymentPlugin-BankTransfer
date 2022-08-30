using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Plugin.Payments;
using Nop.Plugin.Payments.BankTransfer.Services.Interfaces;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Web.Framework.Events;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.BankTransfer.Services
{
    public class EventConsumer : IConsumer<OrderPaidEvent>, IConsumer<PageRenderingEvent>
    {
        #region "Fields"
        private readonly IBankTransferService _bankTransferService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        #endregion

        #region "ctor"
        public EventConsumer(IBankTransferService bankTransferService, IPaymentPluginManager paymentPluginManager)
        {
            _bankTransferService = bankTransferService;
            _paymentPluginManager = paymentPluginManager;
        }
        #endregion
        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            if (eventMessage.Order.PaymentMethodSystemName == BankTransferDefaults.PluginSystemName)
            {
                await _bankTransferService.GenerateRechrgeCode(eventMessage.Order);
            }
        }

        public async Task HandleEventAsync(PageRenderingEvent eventMessage)
        {
            if (!await _paymentPluginManager.IsPluginActiveAsync(BankTransferDefaults.PluginSystemName))
            {
                return;
            }

            if (eventMessage.GetRouteName()?.Equals(BankTransferDefaults.OnePageCheckoutRouteName) ?? false)
            {
                eventMessage.Helper?.AddCssFileParts(
                    "~/Plugins/Nop.Plugin.Payments.BankTransfer/Scripts/fine-uploader/fine-uploader/fine-uploader.min.css", 
                    "", 
                    true);

                eventMessage.Helper?.AddScriptParts(
                    Web.Framework.UI.ResourceLocation.Footer,
                    "~/Plugins/Nop.Plugin.Payments.BankTransfer/Scripts/fine-uploader/jquery.fine-uploader/jquery.fine-uploader.min.js", 
                    excludeFromBundle: true);
            }
        }
    }
}
