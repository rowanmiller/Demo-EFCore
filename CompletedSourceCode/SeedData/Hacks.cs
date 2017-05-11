using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SeedData
{
    public class MyExecutor : MigrationCommandExecutor
    {
        private IDiagnosticsLogger<LoggerCategory.Database.Sql> _sqlLogger;
        private IDiagnosticsLogger<LoggerCategory.Database.DataReader> _readerLogger;

        public MyExecutor(
            IDiagnosticsLogger<LoggerCategory.Database.Sql> sqlLogger,
            IDiagnosticsLogger<LoggerCategory.Database.DataReader> readerLogger)
        {
            _sqlLogger = sqlLogger;
            _readerLogger = readerLogger;
        }

        public override void ExecuteNonQuery(IEnumerable<MigrationCommand> migrationCommands, IRelationalConnection connection)
        {
            migrationCommands = FixCommands(migrationCommands);
            base.ExecuteNonQuery(migrationCommands, connection);
        }

        public override Task ExecuteNonQueryAsync(IEnumerable<MigrationCommand> migrationCommands, IRelationalConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            migrationCommands = FixCommands(migrationCommands);
            return base.ExecuteNonQueryAsync(migrationCommands, connection, cancellationToken);
        }

        private IEnumerable<MigrationCommand> FixCommands(IEnumerable<MigrationCommand> migrationCommands)
        {
            var newCommands = new List<MigrationCommand>();
            foreach (var item in migrationCommands)
            {
                if (item.CommandText.Contains("SET IDENTITY_INSERT [Tags]"))
                {
                    newCommands.Add(
                        new MigrationCommand(
                            new RelationalCommand(
                                _sqlLogger,
                                _readerLogger,
                                item.CommandText
                                    .Replace("SET IDENTITY_INSERT [Tags] ON;", "")
                                    .Replace("SET IDENTITY_INSERT [Tags] OFF;", ""),
                                new List<IRelationalParameter>())));
                }
                else
                {
                    newCommands.Add(item);
                }
            }
            return newCommands;
        }
    }
}
