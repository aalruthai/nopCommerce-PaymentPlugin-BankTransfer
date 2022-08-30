using System;
using FluentValidation;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Payments.BankTransfer.Validators
{
    public partial class PaymentInfoValidator : BaseNopValidator<PaymentInfoModel>
    {
        public PaymentInfoValidator(ILocalizationService localizationService)
        {
            //useful links:
            //http://fluentvalidation.codeplex.com/wikipage?title=Custom&referringTitle=Documentation&ANCHOR#CustomValidator
            //http://benjii.me/2010/11/credit-card-validator-attribute-for-asp-net-mvc-3/

            RuleFor(x => x.TransactionReceipt).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.Payment.BankTransfer.TransactionReceipt.Required"));
            
        }
    }
}