using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.BankTransfer.Domains;
using Nop.Plugin.Payments.BankTransfer.Models;

namespace Nop.Plugin.Payments.BankTransfer.Services.Interfaces
{
    public interface IBankTransferService
    {
        Task<BankTransferRecord> InsertAsync(BankTransferRecord record);
        Task<BankTransferRecord> UpdateAsync(BankTransferRecord record);
        Task<BankTransferRecord> CreateRecordAsync(PaymentInfoModel model, int orderId, Guid orderGuid);
        Task<BankTransferRecord> AddRechargeCodeByOrderIdAsync(string rechargeCode, int orderId);
        Task<BankTransferRecord> AddRechargeCodeByOrderGuidAsync(string rechargeCode, Guid orderGuid);
        Task<BankTransferRecord> GetBankTransferRecordByOrderIdAsync(int orderId);
        Task<BankTransferRecord> GetBankTransferRecordByOrderGuidAsync(Guid orderGuid);
        Task<BankTransferRecord> GetBankTransferRecordByRecordIdAsync(int recordId);
        Task GenerateRechrgeCode(Order order);
    }
}
