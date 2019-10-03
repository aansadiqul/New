using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Domain.Entities;
using Abp.EntityFrameworkCore;
using ABD.Domain.Repositories;
using ABD.EntityFrameworkCore;
using ABD.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ABD.Repositories
{
    public class ABDStoredProcedureRepository<TEntity> : ABDRepositoryBase<TEntity, int>, IABDStoredProcedureRepository<TEntity> where TEntity : class, IEntity<int>
    {
        public ABDStoredProcedureRepository(IDbContextProvider<ABDDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public List<TEntity> ExecuteSP(string query, params object[] parameters)
        {
            var type = Context.Set<TEntity>().FromSql(query, parameters).AsNoTracking().ToList<TEntity>();
            return type;
        }
    }
}
