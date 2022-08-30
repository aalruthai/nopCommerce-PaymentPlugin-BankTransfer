using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Data.Extensions;
using Nop.Plugin.Payments.BankTransfer.Domains;

namespace Nop.Plugin.Payments.BankTransfer.Migrations
{
    [NopMigration("2022-01-14 00:00:00", "BankTransfer schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        private readonly IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            Create.TableFor<BankTransferRecord>();
        }
    }
}
