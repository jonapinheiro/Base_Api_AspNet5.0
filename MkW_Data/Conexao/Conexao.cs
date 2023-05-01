using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MkW_Data.Conexao
{
    public class Conexao
    {
        public static string sConexao;
        private SqlConnection conn;
        private SqlCommand cmd;

        private IMapper _mapper;
        private IConfiguration _config;

        #region Endereço Base Api Antiga

        //private Uri BaseAddress = new Uri("http://localhost:52304/"); // Teste
        //private Uri BaseAddress = new Uri("http://172.16.0.105:8089/"); // Interno 
        private Uri BaseAddress = new Uri("https://sone.stetsom.com.br:8085/"); // Externo   

        #endregion

        #region Dependency_Injection
        public Conexao(IMapper mapper, IConfiguration Configuration)
        {
            _mapper = mapper;
            _config = Configuration;
        }

        #endregion

        #region Conexao sem usuario

        public Conexao()
        {
            sConexao = "data source=172.16.0.16\\FENIX;initial catalog=SATLSTETSOM;user id=db_web;password=3edc$RFV";
            conn = new SqlConnection(sConexao);
            cmd = conn.CreateCommand();
            cmd.CommandTimeout = 6000;

        }


        //public Conexao()
        //{
        //    foreach (string linha in GetStringConnection())
        //    {
        //        sConexao = linha;
        //        conn = new SqlConnection(sConexao);
        //        cmd = conn.CreateCommand();
        //        cmd.CommandTimeout = 6000;
        //        if (this.OpenTransaction())
        //        {
        //            this.FecharConexao();
        //            break;
        //        }
        //    }
        //}

        public Conexao(int servidor)
        {
            Servidor objServidor = new Servidor(servidor);
            sConexao = "data source=" + objServidor.dataSource + ";initial catalog=" + objServidor.baseDados + ";user id=" + objServidor.usuario + ";password=" + objServidor.senha;
            conn = new SqlConnection(sConexao);
            cmd = conn.CreateCommand();
        }
        #endregion

        #region Conexao com usuario
        public Conexao(string usu, string senha)
        {
            sConexao = "data source=dbserver\\SQL2008;initial catalog=SATLSTETSOMWEB;user id = " + usu + "; password=" + senha + "";
        }
        #endregion

        #region GetConnectionString e GetAddress Api Antiga
        public string GetAddress()
        {
            return BaseAddress.AbsoluteUri;
        }

        public List<string> GetStringConnection()
        {
            var cx1 = "data source=FENIX\\FENIX;initial catalog=SATLSTETSOM;user id=db_web;password=3edc$RFV";
            var cx2 = "data source=CENTRAL\\SQLEXPRESS;initial catalog=SATLPRODUCAO;user id=db_web;password=3edc$RFV";

            var teste = _config.GetConnectionString("Jwt");

            List<string> ConnString = new List<string>();

            ConnString.Add(cx1);
            ConnString.Add(cx2);

            return ConnString;
        }
        #endregion

        #region Abrir Conexão
        public bool AbrirConexao()
        {
            try
            {
                conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                //Utilidade.MessageError(null, "Não Conectou!", "Não foi possível conectar a base de dados especificada!\n "
                //    + ex.Message);               

            }
            return false;
        }
        #endregion
        #region Fechar conexão
        public void FecharConexao()
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                //Utilidade.MessageError(null, "Conexão", "Não foi possível encerrar a conexão com a base de dados especificada!\n "
                //+ ex.Message);
            }
        }
        #endregion
        #region Executa comando
        public bool executeComand(string sql)
        {
            bool ok = false;
            try
            {
                if (AbrirConexao())
                {
                    cmd.CommandText = sql;
                    ok = cmd.ExecuteNonQuery() > 0;
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return ok;
        }

        public int executeComand_LinesAffected(string sql)
        {
            int totalLines = -1;
            try
            {
                if (AbrirConexao())
                {
                    cmd.CommandText = sql;
                    totalLines = cmd.ExecuteNonQuery();
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                FecharConexao();
                totalLines = -1;
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return totalLines;
        }

        #endregion
        #region Executa transação
        public bool executeComand_Transaction(string sql)
        {
            bool ok = false;
            try
            {
                cmd.CommandText = sql;
                ok = cmd.ExecuteNonQuery() > 0;
            }
            catch (SqlException ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return ok;
        }

        public int executeComand_TransactionLinesAffected(string sql)
        {
            int totalLines = -1;
            try
            {
                cmd.CommandText = sql;
                totalLines = cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                FecharConexao();
                totalLines = -1;
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return totalLines;
        }
        #endregion
        #region Executa comando retorna DataTable
        public DataTable executaDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                if (AbrirConexao())
                {
                    cmd.CommandText = sql;
                    dt.Load(cmd.ExecuteReader());
                    FecharConexao();
                }
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return dt;
        }
        #endregion
        #region Executa transação
        public DataTable returnDT_Transaction(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd.CommandText = sql;
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return dt;
        }
        #endregion
        #region Abrir conexão da transação
        public bool OpenTransaction()
        {
            if (AbrirConexao())
            {
                cmd.Transaction = conn.BeginTransaction();
                return true;
            }
            return false;
        }

        public bool isOpen()
        {
            return conn.State == ConnectionState.Open;
        }

        public void COMMIT()
        {
            cmd.Transaction.Commit();
            FecharConexao();
        }

        public void ROLLBACK()
        {
            if (isOpen())
            {
                cmd.Transaction.Rollback();
                FecharConexao();
            }
        }
        #endregion
        #region executar um insert e obter a sequencia inserida
        public int executaComandoAI(string sql, string tabela)
        {
            int codigo = -1;
            try
            {
                cmd.CommandText = sql + "; select @@identity " + tabela + ";";
                AbrirConexao();
                codigo = int.Parse(cmd.ExecuteScalar().ToString());
                FecharConexao();
                return codigo;
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, null, ex.Message);
            }
            return codigo;
        }

        public int executaComandoAI2(string sql, string tabela)
        {
            int codigo = -1;
            try
            {
                cmd.CommandText = sql + "; select  SCOPE_IDENTITY() " + tabela + ";";
                AbrirConexao();
                codigo = int.Parse(cmd.ExecuteScalar().ToString());
                FecharConexao();
                return codigo;
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, null, ex.Message);
            }
            return codigo;
        }

        public int executaComandoAI_Transaction(string sql, string tabela)
        {
            int codigo = -1;
            try
            {
                cmd.CommandText = sql + "; select @@identity " + tabela + ";";
                codigo += int.Parse(cmd.ExecuteScalar().ToString());
                return codigo + 1;
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, null, ex.Message);
            }
            return codigo;
        }

        #region IA COM VALOR DA PRINCIPAL
        public int executaComandoAI_Transaction2(string sql, string tabela)
        {
            int codigo = -1;
            try
            {
                cmd.CommandText = sql + "; select SCOPE_IDENTITY() " + tabela + ";";
                codigo += int.Parse(cmd.ExecuteScalar().ToString());
                return codigo + 1;
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, null, ex.Message);
            }
            return codigo;
        }
        #endregion
        #endregion

        #region ADICIONAR PARAMETROS
        public void AddParameter(string parameter, object value)
        {
            if (value is int && !((int)value).Equals(InteiroNulo()))
            {
                cmd.Parameters.Add(parameter, SqlDbType.Int);
                cmd.Parameters[parameter].Value = (int)value;
                return;
            }
            if (value is long)
            {
                cmd.Parameters.Add(parameter, SqlDbType.BigInt);
                cmd.Parameters[parameter].Value = (long)value;
                return;
            }
            if (value is double)
            {
                cmd.Parameters.Add(parameter, SqlDbType.Decimal);
                cmd.Parameters[parameter].Value = (double)value;
                return;
            }
            if (value is decimal && !((decimal)value).Equals(DecimalNulo()))
            {
                cmd.Parameters.Add(parameter, SqlDbType.Decimal);
                cmd.Parameters[parameter].Value = (decimal)value;
                return;
            }
            if (value is string)
            {
                cmd.Parameters.Add(parameter, SqlDbType.NVarChar);
                cmd.Parameters[parameter].Value = (string)value;
                return;
            }

            if (value is char && !((char)value).Equals(CharNulo()))
            {
                cmd.Parameters.Add(parameter, SqlDbType.NChar);
                cmd.Parameters[parameter].Value = (char)value;
                return;
            }
            if (value is bool)
            {
                cmd.Parameters.Add(parameter, SqlDbType.NChar);
                cmd.Parameters[parameter].Value = ((bool)value) ? '1' : '0';
                return;
            }
            if (value is DateTime && !((DateTime)value).Equals(DataNulo()) && !((DateTime)value).Equals(new DateTime(0001, 01, 01)))
            {
                cmd.Parameters.Add(parameter, SqlDbType.DateTime);
                cmd.Parameters[parameter].Value = (DateTime)value;
                return;
            }
            if (value == null)
            {
                cmd.Parameters.AddWithValue(parameter, DBNull.Value);
                cmd.Parameters[parameter].Value = DBNull.Value;
                return;
            }
            if (value is int && ((int)value).Equals(InteiroNulo()))
            {
                cmd.Parameters.AddWithValue(parameter, DBNull.Value);
                cmd.Parameters[parameter].Value = DBNull.Value;
                return;
            }
            if (value is decimal && ((decimal)value).Equals(DecimalNulo()))
            {
                cmd.Parameters.AddWithValue(parameter, DBNull.Value);
                cmd.Parameters[parameter].Value = DBNull.Value;
                return;
            }

            if (value is DateTime && (((DateTime)value).Equals(DataNulo()) || ((DateTime)value).Equals(new DateTime(0001, 01, 01))))
            {
                cmd.Parameters.AddWithValue(parameter, DBNull.Value);
                cmd.Parameters[parameter].Value = DBNull.Value;
                return;
            }

            if (value is char && ((char)value).Equals(CharNulo()))
            {
                cmd.Parameters.AddWithValue(parameter, DBNull.Value);
                cmd.Parameters[parameter].Value = DBNull.Value;
                return;
            }

            if (value is byte[])
            {
                cmd.Parameters.Add(parameter, SqlDbType.Binary);
                cmd.Parameters[parameter].Value = value;
                return;
            }
            //Utilidade.MessageError(null, null, "O tipo do valor não foi especificado na base de dados!");
        }
        public void AddParameter(string parameter, object value, ref string sql)
        {
            if (value is List<string>)
            {
                List<string> lista = (List<string>)value;
                string IN = "('')";
                if (lista.Count > 0)
                    IN = "(";
                for (int posicao_parametro = 0; posicao_parametro < lista.Count; posicao_parametro++)
                {
                    IN += "'" + lista[posicao_parametro] + "'";
                    if (posicao_parametro + 1 < lista.Count)
                        IN += ",";
                }
                if (lista.Count > 0)
                    IN += ")";
                sql = sql.Replace(parameter, IN);
            }

            if (value is List<char>)
            {
                List<char> lista = (List<char>)value;
                string IN = "('')";
                if (lista.Count > 0)
                    IN = "(";
                for (int posicao_parametro = 0; posicao_parametro < lista.Count; posicao_parametro++)
                {
                    IN += "'" + lista[posicao_parametro] + "'";
                    if (posicao_parametro + 1 < lista.Count)
                        IN += ",";
                }
                if (lista.Count > 0)
                    IN += ")";
                sql = sql.Replace(parameter, IN);
            }

            if (value is List<int>)
            {
                List<int> lista = (List<int>)value;
                string IN = "('')";
                if (lista.Count > 0)
                    IN = "(";
                for (int posicao_parametro = 0; posicao_parametro < lista.Count; posicao_parametro++)
                {
                    IN += "'" + lista[posicao_parametro] + "'";
                    if (posicao_parametro + 1 < lista.Count)
                        IN += ",";
                }
                if (lista.Count > 0)
                    IN += ")";
                sql = sql.Replace(parameter, IN);
            }
        }
        #endregion

        #region LIMPAR PARAMETROS
        public void LimparParametros()
        {
            cmd.Parameters.Clear();
        }
        #endregion

        #region Executa comando retorna DataTable, sem fechar conexao
        public DataTable executaDataTable(string sql, Conexao conn)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd.CommandText = sql;
                dt.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                FecharConexao();
                //Utilidade.MessageError(null, "Erro de Banco de Dados!", ex.Message);
            }
            return dt;
        }
        #endregion

        #region RETORNA O MÁXIMO DA COLUNA DA TABELA
        public long getMAX(string tabela, string campo)
        {
            string sql = "SELECT COALESCE(MAX(" + campo + "),0) FROM " + tabela;
            DataTable dt;
            if (isOpen())
                dt = returnDT_Transaction(sql);
            else
                dt = executaDataTable(sql);
            if (dt.Rows.Count > 0)
                return long.Parse(dt.Rows[0][0].ToString());
            //Utilidade.MessageError(null, null, "Não foi possível obter o MAX da tabela e campo especificado!");
            return -1;
        }
        #endregion

        #region CLASSE SERVIDOR
        public class Servidor
        {
            public string dataSource { get; set; }
            public string baseDados { get; set; }
            public string usuario { get; set; }
            public string senha { get; set; }

            public static readonly int VENOM = 1;
            public static readonly int LYRA = 2;
            public static readonly int TESTE = 3;

            #region METODO - NOME STATUS POR CODIGO
            public Servidor(int servidor)
            {
                switch (servidor)
                {
                    case 1:
                        this.dataSource = "172.16.0.9\\SQL2014";
                        this.baseDados = "SATLSTETSOM";
                        this.usuario = "dbrepresentante";
                        this.senha = "DBr3p2015$T";
                        break;
                    case 2:
                        this.dataSource = "172.16.0.105\\SQLEXPRESS";
                        this.baseDados = "Proxy";
                        this.usuario = "sa";
                        this.senha = "C#f3c0mP!r#qu32016";
                        break;
                    case 3:
                        this.dataSource = "172.16.0.9\\SQL2014";
                        this.baseDados = "SATLSTETSOM_MAIO";
                        this.usuario = "db_web";
                        this.senha = "3edc$RFV";
                        break;
                    case 4:
                        this.dataSource = "172.16.0.16\\FENIX";
                        this.baseDados = "DBCEP";
                        this.usuario = "db_web";
                        this.senha = "3edc$RFV";
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region TransactionUncommit
        public bool OpenTransactionReadUncommit()
        {
            if (AbrirConexao())
            {
                cmd.Transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
                return true;
            }
            return false;
        }

        #endregion

        #region Uteis

        public DateTime DataNulo()
        {
            return new DateTime(1900, 1, 1);
        }
        public int InteiroNulo()
        {
            return -1;
        }
        public char CharNulo()
        {
            return '@';
        }
        public int DecimalNulo()
        {
            return -1;
        }

        #endregion
    }
}
