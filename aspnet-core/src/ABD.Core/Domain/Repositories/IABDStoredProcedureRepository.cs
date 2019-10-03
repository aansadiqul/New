using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace ABD.Domain.Repositories
{
    public interface IABDStoredProcedureRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        List<TEntity> ExecuteSP(string query, params object[] parameters);
    }

    public interface IABDStoredProcedureRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        List<TEntity> ExecuteSP(string query, params object[] parameters);
    }

}
