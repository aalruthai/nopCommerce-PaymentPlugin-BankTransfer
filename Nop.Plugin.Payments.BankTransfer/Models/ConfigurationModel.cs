using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.BankTransfer.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.DescriptionText")]
        public string DescriptionText { get; set; }
        public bool DescriptionText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.ShippableProductRequired")]
        public bool ShippableProductRequired { get; set; }
        public bool ShippableProductRequired_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.AllowedFileExtensions")]
        public string AllowedFileExtensions { get; set; }
        public bool AllowedFileExtensions_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey")]
        public string RechrgeCodeEncryptionKey { get; set; }
        public bool RechrgeCodeEncryptionKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.BankTransfer.MaxFileSize")]
        public int MaxFileSize { get; set; }
        public bool MaxFileSize_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugins.Payment.BankTransfer.DescriptionText")]
            public string DescriptionText { get; set; }
        }

        #endregion
    }
}
