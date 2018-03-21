using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;
using szzx.web.Models;
using Framework.MVC;

namespace szzx.web.DataAccess
{
    public abstract class BaseDal
    {
        private static readonly string _connStr = ConfigurationManager.ConnectionStrings["Default"].ToString();

        public IDbConnection Connection
        {
            get
            {
                if (string.IsNullOrEmpty(_connStr))
                {
                    throw new ArgumentException("connection string is null");
                }
                return new SqlConnection(_connStr);
            }
        }

        public T Get<T>(int id) where T : class
        {
            return Connection.Get<T>(id);
        }

        public T Get<T>(string id) where T : class
        {
            return Connection.Get<T>(id);
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            return Connection.GetAll<T>();
        }

        public int Insert<T>(T entity) where T : class
        {
            return (int)Connection.Insert(entity);
        }

        public int Insert<T>(IEnumerable<T> entities) where T : class
        {
            return (int)Connection.Insert(entities);
        }

        public bool Update<T>(T entity) where T : class
        {
            return Connection.Update(entity);
        }

        public bool Update<T>(IEnumerable<T> entities) where T : class
        {
            return Connection.Update(entities);
        }

        public bool Delete<T>(T entity) where T : class
        {
            return Connection.Delete(entity);
        }

        public bool Delete<T>(IEnumerable<T> entities) where T : class
        {
            return Connection.Delete(entities);
        }

        protected int GetTotal(string tableName, string whereStr, object obj)
        {
            var sql = $"select count(1) from {tableName} where 1=1 {whereStr}";
            return Connection.ExecuteScalar<int>(sql, obj);
        }

        public IEnumerable<T> GetPagedEntities<T>(string sql, DataTableAjaxConfig config, object parameters = null, string order = "id", bool isAsc = true) where T:class
        {
            config.recordCount = Connection.QueryFirstOrDefault<int>($"select count(1) from ({sql}) as t", parameters);

            var _sql = $@"with t as(
                        	select top ({config.start} + {config.length}) *, ROW_NUMBER() over(order by {order} {(isAsc ? "asc" : "desc")}) as num
                        	from ({sql}) as tt
                        )
                        select *
                        from t where t.num  > {config.start}";
            return Connection.Query<T>(_sql, parameters);
        }
    }
}