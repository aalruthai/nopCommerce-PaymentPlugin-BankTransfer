using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.BankTransfer.Validators
{
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        #region ctor
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DescriptionText).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.DescriptionText.Required"));
            RuleFor(x => x.RechrgeCodeEncryptionKey).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey.Required"));
            RuleFor(x => x.RechrgeCodeEncryptionKey).MinimumLength(32).WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey.KeyLength"));
            RuleFor(x => x.RechrgeCodeEncryptionKey).MaximumLength(32).WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.RechargeCodeEncryptionKey.KeyLength"));
            RuleFor(x => x.MaxFileSize).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.MaxFileSize.Required"));
            RuleFor(x => x.MaxFileSize).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.MaxFileSize.Required"));
        }
        #endregion
    }
}
