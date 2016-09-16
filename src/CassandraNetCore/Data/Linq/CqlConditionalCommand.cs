﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Cassandra.Mapping;
using Cassandra.Mapping.Statements;
using Cassandra.Tasks;

namespace Cassandra.Data.Linq
{
    /// <summary>
    /// Represents an INSERT/UPDATE/DELETE command with support for Lightweight transactions.
    /// </summary>
    public class CqlConditionalCommand<TEntity>: CqlCommand
    {
        private readonly MapperFactory _mapperFactory;
        private readonly CqlCommand _origin;

        internal CqlConditionalCommand(CqlCommand origin, MapperFactory mapperFactory)
            : base(origin.Expression, origin.Table, origin.StatementFactory, origin.PocoData)
        {
            _mapperFactory = mapperFactory;
            _origin = origin;
            //Copy the Statement properties from origin
            _origin.CopyQueryPropertiesTo(this);
        }

        protected internal override string GetCql(out object[] values)
        {
            return _origin.GetCql(out values);
        }

        /// <summary>
        /// Asynchronously executes a conditional query and returns information whether it was applied.
        /// </summary>
        public new Task<AppliedInfo<TEntity>> ExecuteAsync()
        {
            object[] values;
            var cql = GetCql(out values);
            var session = GetTable().GetSession();
            return StatementFactory
                .GetStatementAsync(session, Cql.New(cql, values))
                .Continue(t1 =>
                {
                    var stmt = t1.Result;
                    this.CopyQueryPropertiesTo(stmt);
                    return session
                        .ExecuteAsync(stmt)
                        .Continue(t => AppliedInfo<TEntity>.FromRowSet(_mapperFactory, cql, t.Result));
                })
                .Unwrap();
        }

        /// <summary>
        /// Executes a conditional query and returns information whether it was applied.
        /// </summary>
        /// <returns>An instance of AppliedInfo{TEntity}</returns>
        public new AppliedInfo<TEntity> Execute()
        {
            var config = GetTable().GetSession().GetConfiguration();
            var task = ExecuteAsync();
            return TaskHelper.WaitToComplete(task, config.ClientOptions.QueryAbortTimeout);
        }

        /// <summary>
        /// Generates and returns the Cql query
        /// </summary>
        public override string ToString()
        {
            object[] _;
            return GetCql(out _);
        }
    }
}
