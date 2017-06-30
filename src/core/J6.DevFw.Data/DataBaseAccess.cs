//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : DataBaseAccess.cs
//  Date : 8/19/2011
//  Author : ����
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using JR.DevFw.Data.Extensions;
using System.Threading;

namespace JR.DevFw.Data
{
    public class DataBaseAccess
    {
        private static readonly Object locker = new object();
        private readonly IDataBase dbFactory;
        private static readonly Regex procedureRegex = new Regex("\\s");
        private IList<String> _totalSqls;
        private bool _totalOpen;
        private int _commandTimeout = 30000;

        /// <summary>
        /// ʵ�������ݿ���ʶ���
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        public DataBaseAccess(DataBaseType type, string connectionString)
        {
            if (connectionString.IndexOf("$ROOT$") != -1)
            {
                connectionString = connectionString.Replace("$ROOT$", AppDomain.CurrentDomain.BaseDirectory);
            }

            this.DbType = type;

            switch (type)
            {
                case DataBaseType.OLEDB:
                    dbFactory = new OleDbFactory(connectionString);
                    break;
                case DataBaseType.SQLite:
                    dbFactory = new SQLiteFactory(connectionString);
                    break;
                case DataBaseType.MonoSQLite:
                    dbFactory = new MonoSQLiteFactory(connectionString);
                    break;
                case DataBaseType.SQLServer:
                    dbFactory = new SqlServerFactory(connectionString);
                    break;
                case DataBaseType.MySQL:
                    dbFactory = new MySqlFactory(connectionString);
                    break;
            }
        }

        /// <summary>
        /// ִ�����ʱʱ�䣬Ĭ��Ϊ30000(30��)
        /// </summary>
        public int CommandTimeout
        {
            get { return this._commandTimeout; }
            set
            {
                if (value <= 2000)
                {
                    throw new ArgumentException("��Ч��ֵ");
                }
                this._commandTimeout = value;
            }
        }

        private DbConnection CreateOpenedConnection()
        {
            DbConnection connection = dbFactory.GetConnection();
            connection.Open();
            return connection;
        }

        /// <summary>
        /// ����ͳ��
        /// </summary>
        public void StartNewTotal()
        {
            if (!_totalOpen)
                _totalOpen = true;
            if (_totalSqls == null)
                _totalSqls = new List<String>();
            else
                _totalSqls.Clear();
        }

        public IList<string> GetTotalSqls()
        {
            if (this._totalSqls == null)
                throw new Exception("��ʹ��StartNewTotal()��ʼͳ��");
            return this._totalSqls;
        }

        private void AddTotalSql(string sql)
        {
            _totalSqls.Add(sql);
        }


        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public DataBaseType DbType { get; private set; }

        /// <summary>
        /// ���ݿ�������
        /// </summary>
        public IDataBase DataBaseAdapter
        {
            get { return this.dbFactory; }
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter NewParameter(string name, object value)
        {
            return dbFactory.CreateParameter(name, value);
        }

        private DbCommand CreateCommand(string sql)
        {
            DbCommand cmd = this.dbFactory.CreateCommand(sql);
            cmd.CommandTimeout = this._commandTimeout;
            return cmd;
        }

        /// <summary>
        /// ����һ������,��ָ������������Ϊ������������������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public DbParameter NewParameter(string name, object value, ParameterDirection direction)
        {
            DbParameter parameter = dbFactory.CreateParameter(name, value);
            parameter.Direction = direction;
            return parameter;
        }

        /// <summary>
        /// ִ�в�ѯ
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            return this.ExecuteNonQuery(commandText, parameters);
        }

        /// <summary>
        /// ִ�в�ѯ
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            int result = 0;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(commandText) ? CommandType.Text : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                // SQLite��֧�ֲ�����д��
                if (this.DbType == DataBaseType.SQLite)
                {
                    Monitor.Enter(locker);
                    result = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    Monitor.Exit(locker);
                    return result;
                }
                result = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// ���ز�ѯ�ĵ�һ�е�һ��ֵ
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            return ExecuteScalar(commandText, parameters);
        }

        /// <summary>
        /// ���ز�ѯ�ĵ�һ�е�һ��ֵ
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            object obj;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(commandText) ? CommandType.Text : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                obj = cmd.ExecuteScalar();
                cmd.Dispose();
            }
            return obj;
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        private DbDataReader ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(commandText) ? CommandType.Text : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                DbDataReader rd = cmd.ExecuteReader();
                cmd.Dispose();
                return rd;
            }
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            ExecuteReader(commandText, func, parameters);
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(commandText);
                cmd.Connection = conn;
                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(commandText)
                    ? CommandType.Text
                    : CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                DbDataReader rd = cmd.ExecuteReader();
                func(rd);
                cmd.Dispose();
            }
        }

        /// <summary>
        /// �����ݿ��ж�ȡ���ݲ��������ڴ���
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);
            DbParameter[] parameters = null;
            return GetDataSet(commandText, parameters);
        }

        /// <summary>
        /// �����ݿ��ж�ȡ���ݲ��������ڴ���
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText, params DbParameter[] parameters)
        {
            if (this._totalOpen) this.AddTotalSql(commandText);

            DataSet ds = new DataSet();
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbDataAdapter adapter = dbFactory.CreateDataAdapter(conn, commandText);
                if (parameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(parameters);
                    //�Զ��ж���T-SQL���Ǵ洢����
                    adapter.SelectCommand.CommandType = procedureRegex.IsMatch(commandText)
                        ? CommandType.Text
                        : CommandType.StoredProcedure;
                }
                adapter.Fill(ds);
            }
            return ds;
        }

        public T ToEntity<T>(string commandText) where T : new()
        {
            return ToEntity<T>(commandText, null);
        }


        /// <summary>
        /// ����ѯ���ת��Ϊʵ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ToEntity<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(commandText);

            T t = default(T);
            ExecuteReader(commandText, (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            }, parameters);
            return t;
        }

        /// <summary>
        /// ��DataReader�������ݲ�ת����ʵ���༯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText) where T : new()
        {
            return ToEntityList<T>(commandText, null);
        }

        /// <summary>
        /// ��DataReader�������ݲ�ת����ʵ���༯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(commandText);

            IList<T> list = null;

            ExecuteReader(commandText, (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            }, parameters);

            return list ?? new List<T>();
        }

        /// <summary>
        /// ִ�нű�(��Mysql)
        /// </summary>
        /// <param name="sql">sql�ű�</param>
        /// <param name="delimiter">�ָ�����ɴ��ݿ�</param>
        /// <returns></returns>
        public int ExecuteScript(string sql, string delimiter)
        {
            int result = -1;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                result = dbFactory.ExecuteScript(conn, sql, delimiter);
            }

            return result;
        }

        #region  �µ����᷽ʽ

        /// <summary>
        /// ִ�в�ѯ
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(params SqlQuery[] sqls)
        {
            if (this._totalOpen)
                foreach (SqlQuery sql in sqls)
                    this.AddTotalSql(sql.Sql);

            if (sqls.Length == 0) throw new ArgumentOutOfRangeException("sqls", "SQLEntity����Ӧָ��һ��!");

            DbTransaction trans = null;
            DbCommand cmd;
            int result = 0;

            DbConnection conn = this.CreateOpenedConnection();

            //�����Ӳ���������
            trans = conn.BeginTransaction();


            SqlEntityHandler sh = s =>
            {
                //����Command,����������
                cmd = this.CreateCommand(s.Sql);
                cmd.Connection = conn;
                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(s.Sql)
                    ? CommandType.Text
                    : CommandType.StoredProcedure;

                //��Ӳ���
                if (s.Parameters != null) cmd.Parameters.AddRange(s.ToParams(dbFactory));
                //ʹ������
                cmd.Transaction = trans;
                //SQLite��֧�ֲ���д��
                if (this.DbType == DataBaseType.SQLite)
                {
                    Monitor.Enter(locker);
                    result += cmd.ExecuteNonQuery();
                    Monitor.Exit(locker);
                    cmd.Dispose();
                    return;
                }
                result += cmd.ExecuteNonQuery();
                cmd.Dispose();
            };

            try
            {
                foreach (SqlQuery sql in sqls)
                {
                    sh(sql);
                }
                //�ύ����

                trans.Commit();
            }
            catch (DbException ex)
            {
                //���������ִ��,��ع�
                trans.Rollback();

                //�����׳��쳣
                throw ex;
            }
            finally
            {
                //�ر�����
                conn.Close();
            }


            return result;
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        public void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(sql.Sql);
                cmd.Connection = conn;

                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(sql.Sql) ? CommandType.Text : CommandType.StoredProcedure;

                if (sql.Parameters != null)
                    cmd.Parameters.AddRange(sql.ToParams(dbFactory));
                func(cmd.ExecuteReader());
                cmd.Dispose();
            }
        }


        /// <summary>
        /// �����ݿ��ж�ȡ���ݲ��������ڴ���
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(SqlQuery sql)
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);
            DataSet ds = new DataSet();
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbDataAdapter adapter = dbFactory.CreateDataAdapter(conn, sql.Sql);

                if (sql.Parameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(sql.ToParams(dbFactory));
                    //�Զ��ж���T-SQL���Ǵ洢����
                    adapter.SelectCommand.CommandType = procedureRegex.IsMatch(sql.Sql)
                        ? CommandType.Text
                        : CommandType.StoredProcedure;
                }
                adapter.Fill(ds);
            }

            return ds;
        }


        /// <summary>
        /// ���ز�ѯ�ĵ�һ�е�һ��ֵ
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(SqlQuery sql)
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);

            object obj;
            using (DbConnection conn = this.CreateOpenedConnection())
            {
                DbCommand cmd = this.CreateCommand(sql.Sql);
                cmd.Connection = conn;

                //�Զ��ж���T-SQL���Ǵ洢����
                cmd.CommandType = procedureRegex.IsMatch(sql.Sql)
                    ? CommandType.Text
                    : CommandType.StoredProcedure;

                if (sql.Parameters != null) cmd.Parameters.AddRange(sql.ToParams(dbFactory));
                obj = cmd.ExecuteScalar();
                cmd.Dispose();
            }

            return obj;
        }

        /// <summary>
        /// ����ѯ���ת��Ϊʵ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ToEntity<T>(SqlQuery sql) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);

            T t = default(T);
            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            });


            return t;
        }

        /// <summary>
        /// ��DataReader�������ݲ�ת����ʵ���༯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(SqlQuery sql) where T : new()
        {
            if (this._totalOpen) this.AddTotalSql(sql.Sql);

            IList<T> list = null;

            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            });

            return list ?? new List<T>();
        }

        #endregion

        #region Hashtable��ȡ����

        public int ExecuteNonQuery(string commandText, Hashtable data)
        {
            var parameters = GetParametersFromHashTable(data);
            return this.ExecuteNonQuery(commandText, parameters);
        }

        public object ExecuteScalar(string commandText, Hashtable data)
        {
            return this.ExecuteScalar(commandText, this.GetParametersFromHashTable(data));
        }

        public void ExecuteReader(string commandText, Hashtable data, DataReaderFunc func)
        {
            this.ExecuteReader(commandText, func, this.GetParametersFromHashTable(data));
        }

        public DataSet GetDataSet(string commandText, Hashtable data)
        {
            return this.GetDataSet(commandText, this.GetParametersFromHashTable(data));
        }

        private DbParameter[] GetParametersFromHashTable(Hashtable data)
        {
            DbParameter[] parameters = new DbParameter[data.Keys.Count];

            int i = 0;
            foreach (DictionaryEntry d in data)
            {
                parameters[i++] = this.NewParameter("@" + d.Key, d.Value);
            }
            return parameters;
        }

        #endregion

        /*
        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            if (conn!=null && conn.State != ConnectionState.Closed)
            {
                conn.Dispose();
            }
        }

        ~DataBaseAccess()
        {
            this.Dispose();
        }
		 */
    }
}