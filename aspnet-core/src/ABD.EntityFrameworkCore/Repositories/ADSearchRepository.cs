using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.Data;
using Abp.EntityFrameworkCore;
using ABD.Domain.Dtos;
using ABD.Domain.Helper;
using ABD.Domain.Repositories;
using ABD.Entities;
using ABD.EntityFrameworkCore;
using ABD.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ABD.Repositories
{
    public class ADSearchRepository : ABDRepositoryBase<ADSearch>, IADSearchRepository
    {
        private readonly IActiveTransactionProvider _transactionProvider;

        public ADSearchRepository(IDbContextProvider<ABDDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider)
            : base(dbContextProvider)
        {
            _transactionProvider = transactionProvider;
        }

        public async Task<List<BreakdownDto>> ExecuteAdAnalyzeQuery(string input)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(input, CommandType.Text))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    List<BreakdownDto> breakdownList = dataTable.DataTableToList<BreakdownDto>();
                    return breakdownList;
                }
            }
        }

        public async Task<List<BreakdownBDDto>> ExecuteBDAnalyzeQuery(string input)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(input, CommandType.Text))
            {
                command.CommandTimeout = 600;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    List<BreakdownBDDto> breakdownList = dataTable.DataTableToList<BreakdownBDDto>();
                    return breakdownList;
                }
            }
        }

        public async Task<List<ADName>> ExecuteAdNamesQuery(string input)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(input, CommandType.Text))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    List<ADName> adNames = dataTable.DataTableToList<ADName>();
                    return adNames;
                }
            }
        }
        public async Task<List<XDateBreakdownDto>> ExecuteXDateBreakdownQuery(string input)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(input, CommandType.Text))
            {
                command.CommandTimeout = 600;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    List<XDateBreakdownDto> bdXDateBreakdown = dataTable.DataTableToList<XDateBreakdownDto>();
                    return bdXDateBreakdown;
                }
            }
        }

        public async Task<string> GetQueryResult(string input)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(input, CommandType.Text))
            {
                command.CommandTimeout = 600;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var result = "";
                    while (dataReader.Read())
                    {
                        result = dataReader[0].ToString();
                    }
                    return result;
                }
            }
        }

        private void EnsureConnectionOpen()
        {
            var connection = Context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private DbCommand CreateCommand(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            var command = Context.Database.GetDbConnection().CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = GetActiveTransaction();

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        private DbTransaction GetActiveTransaction()
        {
            return (DbTransaction)_transactionProvider.GetActiveTransaction(new ActiveTransactionProviderArgs
            {
                {"ContextType", typeof(ABDDbContext) },
                {"MultiTenancySide", MultiTenancySide }
            });
        }
    }
}
