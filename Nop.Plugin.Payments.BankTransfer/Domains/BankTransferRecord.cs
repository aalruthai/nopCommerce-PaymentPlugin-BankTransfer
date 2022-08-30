using System;
using Nop.Core;

namespace Nop.Plugin.Payments.BankTransfer.Domains
{
    public class BankTransferRecord : BaseEntity
    {
        public int OrderId { get; set; }
        public Guid OrderGuid { get; set; }
        public string TransactionReceipt { get; set; }
        public string RechargeCode { get; set; }
    }
}
