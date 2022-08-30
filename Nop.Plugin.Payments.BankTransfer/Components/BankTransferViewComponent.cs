using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.BankTransfer.Components
{
    [ViewComponent(Name = "BankTransfer")]
    public class BankTransferViewComponent : NopViewComponent
    {
        private readonly BankTransferSettings _bankTransferSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        public BankTransferViewComponent(BankTransferSettings bankTransferSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _bankTransferSettings = bankTransferSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            var model = new PaymentInfoModel
            {
                DescriptionText = await _localizationService.GetLocalizedSettingAsync(_bankTransferSettings,
                    x => x.DescriptionText, (await _workContext.GetWorkingLanguageAsync()).Id, store.Id),
                AllowedFileExtensions = _bankTransferSettings.AllowedFileExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList()
        };

            return View("~/Plugins/Nop.Plugin.Payments.BankTransfer/Views/PaymentInfo.cshtml", model);
        }
    }
}
