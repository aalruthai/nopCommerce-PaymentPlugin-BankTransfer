using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.BankTransfer.Models
{
    public class PaymentDetailsModel
    {
        public int OrderId { get; set; }
        public Guid OrderGuid { get; set; }
        public string TransactionReceipt { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BankTransfer.RechargeCode")]
        public string RechargeCode { get; set; }
        [NopResourceDisplayName("Plugins.Payment.BankTransfer.TransactionReceipt")]
        public string ReceiptDownloadUrl { get; set; }
    }
}
