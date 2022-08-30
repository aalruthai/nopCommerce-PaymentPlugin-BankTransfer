using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.BankTransfer.Models
{
    public record PaymentInfoModel : BaseNopModel
    {
        public string DescriptionText { get; set; }
        
        public string TransactionReceipt { get; set; }
        public List<string> AllowedFileExtensions { get; set; }
    }
}
