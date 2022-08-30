using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Payments.BankTransfer.Domains;
using Nop.Plugin.Payments.BankTransfer.Models;
using Nop.Plugin.Payments.BankTransfer.Services.Interfaces;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.BankTransfer.Services
{
    
    public class BankTransferService : IBankTransferService
    {
        private readonly IRepository<BankTransferRecord> _bankTransferRecordRepository;
        private readonly BankTransferSettings _bankTransferSettings;
        private readonly IRechargeKeyGenerator _rechargeKeyGenerator;
        private readonly ILogger _logger;

        public BankTransferService(IRepository<BankTransferRecord> repository,
            BankTransferSettings bankTransferSettings,
            IRechargeKeyGenerator rechargeKeyGenerator,
            ILogger logger)
        {
            _bankTransferRecordRepository = repository;
            _bankTransferSettings = bankTransferSettings;
            _rechargeKeyGenerator = rechargeKeyGenerator;
            _logger = logger;
        }
        public async Task<BankTransferRecord> AddRechargeCodeByOrderGuidAsync(string rechargeCode, Guid orderGuid)
        {
            var record = await GetBankTransferRecordByOrderGuidAsync(orderGuid);
            if (record == null)
            {
                return null;
            }
            record.RechargeCode = rechargeCode;
            return await UpdateAsync(record);
        }

        public async Task<BankTransferRecord> AddRechargeCodeByOrderIdAsync(string rechargeCode, int orderId)
        {
            var record = await GetBankTransferRecordByOrderIdAsync(orderId);
            if (record == null)
            {
                return null;
            }
            record.RechargeCode = rechargeCode;
            return await UpdateAsync(record);
        }

        public async Task<BankTransferRecord> CreateRecordAsync(PaymentInfoModel model, int orderId, Guid orderGuid)
        {
            if (model == null || orderId == 0 || orderGuid == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(model));
            }
            BankTransferRecord record = new BankTransferRecord 
            {
                OrderGuid = orderGuid,
                OrderId = orderId,
                TransactionReceipt = model.TransactionReceipt
            };

            return await InsertAsync(record);
        }

        public async Task GenerateRechrgeCode(Order order)
        {
            try
            {
                if (order == null || order.PaymentMethodSystemName != BankTransferDefaults.PluginSystemName)
                {
                    return;
                }

                int orderId = order.Id;
                int amount = Decimal.ToInt32(Math.Ceiling(order.OrderSubtotalExclTax));
                string encryptionKey = _bankTransferSettings.RechargeCodeEncryptionKey;
                if (string.IsNullOrWhiteSpace(encryptionKey))
                {
                    return;
                }
                string rechargeCode = _rechargeKeyGenerator.GenerateRechargeKey(amount.ToString(), encryptionKey);
                await AddRechargeCodeByOrderIdAsync(rechargeCode, orderId);
            }
            catch (Exception ex)
            {

                await _logger.ErrorAsync("Error while generating recharge code.", ex);
            }
            
        }

        public async Task<BankTransferRecord> GetBankTransferRecordByOrderGuidAsync(Guid orderGuid)
        {
            return await _bankTransferRecordRepository.Table.FirstOrDefaultAsync(x => x.OrderGuid == orderGuid);
        }

        public async Task<BankTransferRecord> GetBankTransferRecordByOrderIdAsync(int orderId)
        {
            return await _bankTransferRecordRepository.Table.FirstOrDefaultAsync(x => x.OrderId == orderId);    
        }

        public async Task<BankTransferRecord> GetBankTransferRecordByRecordIdAsync(int recordId)
        {
            return await _bankTransferRecordRepository.GetByIdAsync(recordId);
        }

        public async Task<BankTransferRecord> InsertAsync(BankTransferRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            await _bankTransferRecordRepository.InsertAsync(record);

            return record;
        }

        public async Task<BankTransferRecord> UpdateAsync(BankTransferRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }
            await _bankTransferRecordRepository.UpdateAsync(record);
            return record;
        }
    }
}
