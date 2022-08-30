using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.BankTransfer.Domains;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Plugin.Payments.BankTransfer.Services;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Order;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Payments.BankTransfer.Services.Interfaces;

namespace Nop.Plugin.Payments.BankTransfer.Components
{
    [ViewComponent(Name = "OrderDetails")]
    public class OrderDetailsViewComponent : NopViewComponent
    {
        private readonly BankTransferSettings _bankTransferSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IBankTransferService _bankTransferService;
        public OrderDetailsViewComponent(BankTransferSettings bankTransferSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IBankTransferService bankTransferService)
        {
            _bankTransferSettings = bankTransferSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
            _bankTransferService = bankTransferService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            PaymentDetailsModel model = new PaymentDetailsModel();
            if (widgetZone.Equals(PublicWidgetZones.OrderDetailsBillingAddress))
            {
                OrderDetailsModel data = (OrderDetailsModel)additionalData;
                var record = await _bankTransferService.GetBankTransferRecordByOrderIdAsync(data.Id);
                if (record != null)
                {
                    model = GenerateModel(record);
                    
                    return View("~/Plugins/Nop.Plugin.Payments.BankTransfer/Views/OrderPaymentInfo.cshtml", model);
                }
                else
                {
                    return Content(string.Empty);
                }
            }
            else if(widgetZone.Equals(PublicWidgetZones.OrderDetailsPageOverview))
            {
                OrderDetailsModel data = (OrderDetailsModel)additionalData;
                var record = await _bankTransferService.GetBankTransferRecordByOrderIdAsync(data.Id);
                if (record != null)
                {
                    model = GenerateModel(record);

                    return View("~/Plugins/Nop.Plugin.Payments.BankTransfer/Views/OrderRechargeCode.cshtml", model);
                }
                else
                {
                    return Content(string.Empty);
                }
            }
            else if (widgetZone.Equals(AdminWidgetZones.OrderDetailsBlock))
            {
                OrderModel data = (OrderModel)additionalData;
                var record = await _bankTransferService.GetBankTransferRecordByOrderIdAsync(data.Id);
                if (record != null)
                {
                    model = GenerateModel(record, "DownloadFile");

                    return View("~/Plugins/Nop.Plugin.Payments.BankTransfer/Views/AdminOrderPaymentInfo.cshtml", model);
                }
                else
                {
                    return Content(string.Empty);
                }
            }
            else
            {
                return Content(string.Empty);
            }
            //var model = new PaymentInfoModel
            //{
            //    DescriptionText = await _localizationService.GetLocalizedSettingAsync(_bankTransferSettings,
            //        x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, store.Id),
            //    AllowedFileExtensions = _bankTransferSettings.AllowedFileExtensions
            //            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            //            .ToList()
            //};

            return View("~/Plugins/Nop.Plugin.Payments.BankTransfer/Views/OrderPaymentInfo.cshtml", model);
        }

        private PaymentDetailsModel GenerateModel(BankTransferRecord record, string downloadAction = "GetFileUpload")
        {
            PaymentDetailsModel model = new PaymentDetailsModel();
            if (record != null)
            {
                
                model.OrderGuid = record.OrderGuid;
                model.TransactionReceipt = record.TransactionReceipt;
                model.OrderId = record.OrderId;
                model.RechargeCode = record.RechargeCode;
                object actionParams;
                switch (downloadAction)
                {
                    case "DownloadFile":
                        actionParams = new { downloadGuid = record.TransactionReceipt };
                        break;
                    default:
                        actionParams = new { downloadId = record.TransactionReceipt };
                        break;
                }
                if (!string.IsNullOrWhiteSpace(record.TransactionReceipt))
                {
                    model.ReceiptDownloadUrl = Url.Action(downloadAction, "Download", actionParams);
                }
            }

            return model;
        }
    }
}
