using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppTesteClientesDAL
{
    internal class ConexaoBanco
    {
        /// <summary>
        /// Conexao com o banco de dados, utilizando o paramentro ConnectionString do App.Config
        /// retorna a conexao ativa ou no caso de erro trata como null.
        /// /// </summary>
        /// <returns>conSql</returns>
        internal static SqlConnection Conectar()
        {
            SqlConnection conSql = null;
            conSql = new SqlConnection();
            try { 
                string conexao = ConfigurationManager.ConnectionStrings["BdSqlServer"].ConnectionString;
    
                conSql.ConnectionString = conexao;
                conSql.Open();
                return conSql;
            } catch (Exception ex)
            {
                conSql = null;
                return null;
            }
        }

        /// <summary>
        /// Conecta e executa a instrução sql via ADO.NET, retorna o Id da tabela.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>Id</returns>
        internal static int ExecutarSqlAdoRetornoId(string sql, out string msg)
        {
            msg = string.Empty;
            Object id;
            try { 
                using (SqlConnection con = Conectar())
                {
                    using(SqlCommand cmd = con.CreateCommand()) 
                    { 
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = con;
                        id = Convert.ToInt64(cmd.ExecuteScalar());
                        cmd.Dispose();
                        return Convert.ToInt32(id);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="msg"></param>
        /// <returns>bool</returns>
        internal static bool ExecutarSqlAdo(string sql, out string msg)
        {
            msg = string.Empty;
            Object id;
            try { 
                using (SqlConnection con = Conectar())
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = con;
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        return true;
                    }
                }
            } 
            catch(Exception ex) 
            {
                msg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Efetua a leitura e mantem o datareader ativo para ser fechado depois de seu uso.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        internal static SqlDataReader executarReader(string sql, out string msg)
        {
            msg = string.Empty;

            SqlConnection con = new SqlConnection();
            try
            {
                con = Conectar();
                SqlCommand cmd = con.CreateCommand();
                    
                cmd.CommandText = sql;
                cmd.Connection = con;
                var retorno = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return retorno;                
            } 
            catch(Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }
        
    }
}
