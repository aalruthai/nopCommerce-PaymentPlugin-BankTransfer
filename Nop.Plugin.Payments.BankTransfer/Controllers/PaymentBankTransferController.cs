using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.BankTransfer.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class PaymentBankTransferController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public PaymentBankTransferController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IDownloadService downloadService,
            INopFileProvider fileProvider)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _downloadService = downloadService;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Methods
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var bankTransferSettings = await _settingService.LoadSettingAsync<BankTransferSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = bankTransferSettings.DescriptionText
            };

            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.DescriptionText = await _localizationService
                    .GetLocalizedSettingAsync(bankTransferSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.AdditionalFee = bankTransferSettings.AdditionalFee;
            model.AdditionalFeePercentage = bankTransferSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = bankTransferSettings.ShippableProductRequired;
            model.AllowedFileExtensions = bankTransferSettings.AllowedFileExtensions;
            model.RechrgeCodeEncryptionKey = bankTransferSettings.RechargeCodeEncryptionKey;
            model.ActiveStoreScopeConfiguration = storeScope;
            model.MaxFileSize = bankTransferSettings.MaxFileSize;

            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.ShippableProductRequired, storeScope);
                model.AllowedFileExtensions_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.AllowedFileExtensions, storeScope);
                model.RechrgeCodeEncryptionKey_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.RechargeCodeEncryptionKey, storeScope);
                model.MaxFileSize_OverrideForStore = await _settingService.SettingExistsAsync(bankTransferSettings, x => x.MaxFileSize, storeScope);
            }

            return View("~/Plugins/Nop.Plugin.Payments.BankTransfer/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var bankTransferSettings = await _settingService.LoadSettingAsync<BankTransferSettings>(storeScope);

            //save settings
            bankTransferSettings.DescriptionText = model.DescriptionText;
            bankTransferSettings.AdditionalFee = model.AdditionalFee;
            bankTransferSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            bankTransferSettings.ShippableProductRequired = model.ShippableProductRequired;
            bankTransferSettings.AllowedFileExtensions = model.AllowedFileExtensions;
            bankTransferSettings.RechargeCodeEncryptionKey = model.RechrgeCodeEncryptionKey;
            bankTransferSettings.MaxFileSize = model.MaxFileSize;
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.AllowedFileExtensions, model.AllowedFileExtensions_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.RechargeCodeEncryptionKey, model.RechrgeCodeEncryptionKey_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(bankTransferSettings, x => x.MaxFileSize, model.MaxFileSize_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedSettingAsync(bankTransferSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }


        
        #endregion
    }
}
