using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Orders;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Payments.BankTransfer.Domains;
using Nop.Data.Extensions;
using System.Data;

namespace Nop.Plugin.Payments.BankTransfer.Mapping.Builders
{
    public class PluginBuilder : NopEntityBuilder<BankTransferRecord>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(BankTransferRecord.OrderId)).AsInt32().ForeignKey<Order>(onDelete: Rule.Cascade)
                .WithColumn(nameof(BankTransferRecord.OrderGuid)).AsGuid()
                .WithColumn(nameof(BankTransferRecord.TransactionReceipt)).AsString(int.MaxValue)
                .WithColumn(nameof(BankTransferRecord.RechargeCode)).AsString(int.MaxValue).Nullable();
            
        }

        #endregion
    }
}