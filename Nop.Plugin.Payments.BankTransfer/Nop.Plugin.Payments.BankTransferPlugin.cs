using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Plugin.Payments.BankTransfer.Services;
using Nop.Plugin.Payments.BankTransfer.Validators;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Plugin.Payments.BankTransfer.Services.Interfaces;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.BankTransfer
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class BankTransferPlugin : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        private readonly BankTransferSettings _bankTransferSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IBankTransferService _bankTransferService;
        private readonly ILogger _logger;
        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => false;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => false;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        public bool HideInWidgetList => false;

        #endregion

        #region Ctor

        public BankTransferPlugin(BankTransferSettings bankTransferSettings,
            ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            IShoppingCartService shoppingCartService,
            IWebHelper webHelper,
            IPaymentService paymentService,
            IOrderService orderService,
            IBankTransferService bankTransferService,
            ILogger logger)
        {
            _bankTransferSettings = bankTransferSettings;
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _shoppingCartService = shoppingCartService;
            _webHelper = webHelper;
            _paymentService = paymentService;
            _orderService = orderService;
            _bankTransferService = bankTransferService;
            _logger = logger;
        }

        #endregion

        public override async Task InstallAsync()
        {
            //settings
            var settings = new BankTransferSettings
            {
                DescriptionText = "<p>Deposit Personal or Business Check, Cashier's Check or money order to:</p><p><br /><b>TCC Bank account details here...</b> <br /><b>Recharge code will be sent after money deposit verification</b>",
                RechargeCodeEncryptionKey = "EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE",
                MaxFileSize = 10240 // 10MBs
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payment.BankTransfer.AdditionalFee"] = "Additional fee",
                ["Plugins.Payment.BankTransfer.AdditionalFee.Hint"] = "The additional fee.",
                ["Plugins.Payment.BankTransfer.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                ["Plugins.Payment.BankTransfer.AdditionalFeePercentage.Hint"] = "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                ["Plugins.Payment.BankTransfer.DescriptionText"] = "Description",
                ["Plugins.Payment.BankTransfer.DescriptionText.Hint"] = "Enter info that will be shown to customers during checkout",
                ["Plugins.Payment.BankTransfer.DescriptionText.Required"] = "Description is required",
                ["Plugins.Payment.BankTransfer.PaymentMethodDescription"] = "Pay by bank transfer",
                ["Plugins.Payment.BankTransfer.ShippableProductRequired"] = "Shippable product required",
                ["Plugins.Payment.BankTransfer.ShippableProductRequired.Hint"] = "An option indicating whether shippable products are required in order to display this payment method during checkout.",

                
                
                ["Plugins.Payment.BankTransfer.TransactionReceipt"] = "Transaction receipt",
                ["Plugins.Payment.BankTransfer.TransactionReceipt.Hint"] = "Please upload transaction receipt",
                ["Plugins.Payment.BankTransfer.TransactionReceipt.Required"] = "Please upload transaction receipt",

                ["Plugins.Payment.BankTransfer.AllowedFileExtensions"] = "Allowed Receipt file extensions",
                ["Plugins.Payment.BankTransfer.AllowedFileExtensions.Hint"] = "Please enter allowed extensions, leave empty to allow any file extension",

                ["Plugins.Payment.BankTransfer.PaymentDetails"] = "Payment Details",
                ["Plugins.Payment.BankTransfer.RechargeCode"] = "Recharge Code",

                ["Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey"] = "Recharge code encryption key",
                ["Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey.Hint"] = "Encryption key used in generation and validation of recharge codes",
                ["Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey.Required"] = "Encryption key is required",
                ["Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey.KeyLength"] = "Encryption key length must be 32 characters",

                ["Plugins.Payment.BankTransfer.MaxFileSize"] = "Maximum file size (KB)",
                ["Plugins.Payment.BankTransfer.MaxFileSize.Hint"] = "Specify maximum file size in kilobytes.",
                ["Plugins.Payment.BankTransfer.MaxFileSize.Required"] = "Maximum file size is required.",
            });
            await base.InstallAsync();
        }

        public override Task PreparePluginToUninstallAsync()
        {
            return base.PreparePluginToUninstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<BankTransferSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payment.BankTransfer");

            await base.UninstallAsync();
        }

        public override Task UpdateAsync(string currentVersion, string targetVersion)
        {
            return base.UpdateAsync(currentVersion, targetVersion);
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentBankTransfer/Configure";
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            return Task.FromResult(result);
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var values = _paymentService.DeserializeCustomValues(postProcessPaymentRequest.Order);
            object temp;
            if(!values.TryGetValue("BankTransfer", out temp))
            {
                await _logger.ErrorAsync("Could not get bank transfer record from order custom values, order " + postProcessPaymentRequest.Order.OrderGuid);
                return;
            }
            PaymentInfoModel paymentInfo = JsonConvert.DeserializeObject<PaymentInfoModel>(temp.ToString());
            await _bankTransferService.CreateRecordAsync(paymentInfo, postProcessPaymentRequest.Order.Id, postProcessPaymentRequest.Order.OrderGuid);
            var order = await _orderService.GetOrderByGuidAsync(postProcessPaymentRequest.Order.OrderGuid);
            order.CustomValuesXml = null;
            await _orderService.UpdateOrderAsync(order);
            return;
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return Task.FromResult(false);
        }

        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart,
                _bankTransferSettings.AdditionalFee, _bankTransferSettings.AdditionalFeePercentage);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //it's not a redirection payment method. So we always return false
            return Task.FromResult(false);
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            var warnings = new List<string>();

            //validate
            var validator = new PaymentInfoValidator(_localizationService);
            var model = GetPaymentInfoModel(form);
            
            var validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
                warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

            return Task.FromResult<IList<string>>(warnings);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            var model = GetPaymentInfoModel(form);
            var request = new ProcessPaymentRequest();
            request.CustomValues.Add("BankTransfer", model);

            return Task.FromResult(request);
        }

        public string GetPublicViewComponentName()
        {
            return "BankTransfer";
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.PaymentMethodDescription");
        }

        private PaymentInfoModel GetPaymentInfoModel(IFormCollection form)
        {
            var model = new PaymentInfoModel();
            try
            {
                model.TransactionReceipt = form["TransactionReceipt"];
            }
            catch (Exception ex)
            {   
            }
            
            return model;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            var result = new List<string>
            {
                PublicWidgetZones.OrderSummaryBillingAddress,
                PublicWidgetZones.OrderDetailsBillingAddress,
                PublicWidgetZones.OrderDetailsPageOverview,

                AdminWidgetZones.OrderDetailsBlock
            };
            return Task.FromResult(result as IList<string>);
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "OrderDetails";
        }
    }
}
