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
using Abp.Dependency;
using Abp.EntityFrameworkCore;
using ABD.Domain.Dtos;
using ABD.Domain.Helper;
using ABD.Domain.Repositories;
using ABD.Entities;
using ABD.EntityFrameworkCore;
using ABD.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft;
using Newtonsoft.Json;

namespace ABD.Repositories
{
    public class CommonRepository : ICommonRepository, ITransientDependency
    {
        private readonly IActiveTransactionProvider _transactionProvider;
        private readonly IDbContextProvider<ABDDbContext> _dbContextProvider;
        private ABDDbContext Context => _dbContextProvider.GetDbContext();

        public CommonRepository(IDbContextProvider<ABDDbContext> dbContextProvider, IActiveTransactionProvider transactionProvider)
        {
            _transactionProvider = transactionProvider;
            _dbContextProvider = dbContextProvider;
        }

        public async Task<List<CommonDto>> GetProcedureData(string procedureName)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(procedureName, CommandType.StoredProcedure))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    List<CommonDto> data = dataTable.DataTableToList<CommonDto>();
                    return data;
                }
            }
        }

        public async Task ExecuteAdminStoredProcedure(string procedureName, bool isActive)
        {

            await Context.Database.ExecuteSqlCommandAsync(
                $"EXEC {procedureName} @IsActive",
                new SqlParameter("IsActive", isActive)
            );
        }

        public async Task<string> GetData(string query)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand(query, CommandType.Text))
            {
                command.CommandTimeout = 1000;
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    return DataTableToJson(dataTable);
                }
            }
        }

        private string DataTableToJson(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

       
        public async Task<string> GetBdReportData(int orderId, long? userId, int purchaseType)
        {
            EnsureConnectionOpen();

            using (var command = CreateCommand("GetBDRecords_Purchased",
                                                CommandType.StoredProcedure,
                                                new SqlParameter("OrderID", orderId),
                                                new SqlParameter("CreatorUserId", userId),
                                                new SqlParameter("PurchaseType", purchaseType)))
            {
                using (var dataReader = await command.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    return DataTableToJson(dataTable);
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
                {"MultiTenancySide", null }
            });
        }
    }
}
