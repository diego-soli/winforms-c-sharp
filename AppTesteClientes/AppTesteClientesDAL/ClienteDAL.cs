using AppTesteClientesDTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppTesteClientesDAL
{
    public class ClienteDAL
    {
        private string sql;
        public bool IncluirCliente(ClienteDTO dto, out long codigoCliente, out string mensagem)
        {
            mensagem = string.Empty;
            codigoCliente = 0;

            try
            {

                sql = "Insert into Cliente (nome,cpf,sexo,email,telefone,data_inclusao) " +
                "values ('" + dto.Nome +  "','" + dto.Cpf + "', " + dto.Sexo + ",'" + dto.Email + "', '" + dto.Telefone + "','" + dto.DataInclusao + "'); ";

                sql += "SELECT CAST(scope_identity() AS int)"; // obtem o id do registro inserido.


                codigoCliente = ConexaoBanco.ExecutarSqlAdoRetornoId(sql,out mensagem);

                if (codigoCliente != 0) // caso a inclusão for bem sucedida insere o endereco.
                {
                    sql = "Insert into Endereco (id_cliente,logradouro,numero,bairro,cidade,estado,cep)";
                    sql += " values (" + codigoCliente + ",'" +
                        dto.endereco.Logradouro + "', '" +
                        dto.endereco.Numero + "', '" +
                        dto.endereco.Bairro + "', '" +
                        dto.endereco.Cidade + "', '" +
                        dto.endereco.Estado + "', '" +
                        dto.endereco.Cep + "')";
                        long idEndereco = ConexaoBanco.ExecutarSqlAdoRetornoId(sql, out mensagem);
                    
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                mensagem = ex.InnerException.ToString();
                return false;
            }

            return true;
        }
        public bool AtualizarCliente(ClienteDTO dto, out string mensagem)
        {
            mensagem = string.Empty;
            
            try
            {
                 sql = "Update Cliente  set " +
                    "nome = '" + dto.Nome + "'," +
                    "cpf = '" + dto.Cpf + "'," +
                    "sexo = " + dto.Sexo + "," +
                    "email = '" + dto.Email + "'," +
                    "telefone ='" + dto.Telefone + "' " +
                    "where id = " + dto.Id ;

               if (!ConexaoBanco.ExecutarSqlAdo(sql,out mensagem)) { return false;  }

                
                sql = "Update Endereco set " +
                    "logradouro= '" + dto.endereco.Logradouro + "'," +
                    "numero= '" + dto.endereco.Numero + "'," +
                    "bairro= '" + dto.endereco.Bairro + "'," +
                    "cidade= '" + dto.endereco.Cidade + "'," +
                    "estado= '" + dto.endereco.Estado + "'," +
                    "cep= '" +  dto.endereco.Cep + "' " +
                    "where id_cliente = " + dto.Id;

                if (!ConexaoBanco.ExecutarSqlAdo(sql, out mensagem)) { return false; }

                GC.Collect();

            }
            catch (Exception ex)
            {
                mensagem = ex.InnerException.ToString();
                return false;
            }

            return true;
        }

        public bool ExcluirCliente(long idCliente, out string mensagem)
        {
            mensagem = string.Empty;
            try
            {
                sql = "delete from Cliente where id= " + idCliente;
                ConexaoBanco.ExecutarSqlAdo(sql, out mensagem);
                GC.Collect();
            }
            catch (Exception ex)
            {
                mensagem = ex.InnerException.ToString();
                return false;
            }
            return true;
        }

        public List<ListaClienteGridDTO>ListarCliente(out string mensagem)
        {
            mensagem = string.Empty;
            var lista = new List<ListaClienteGridDTO>();

            sql = "select cli.id, cli.cpf, cli.nome,ende.cidade,ende.estado from Cliente cli join Endereco ende on cli.id = ende.id_cliente";
            sql += " order by cli.nome asc;";

            var consulta = ConexaoBanco.executarReader(sql,out mensagem);
            while(consulta.Read())
            {
                lista.Add(new ListaClienteGridDTO()
                {
                    Id = Convert.ToInt64(consulta["id"]),
                    Nome = consulta["nome"].ToString(),
                    Cidade = consulta["cidade"].ToString() ,
                    Estado = consulta["estado"].ToString(),
                    Cpf = consulta["cpf"].ToString(),
                });

            }
            consulta.Close();
            GC.Collect();
            return lista.ToList();
        }
        public ClienteDTO BuscarCliente(long idCliente,out string mensagem)
        {
            mensagem = string.Empty;
            
            ClienteDTO cliente = new ClienteDTO();
            EnderecoDTO ender = new EnderecoDTO();

            sql = "select cli.id, cli.nome,cli.cpf,cli.sexo,cli.email,cli.data_inclusao,cli.telefone," +
                " ende.logradouro,ende.numero,ende.bairro,ende.cidade,ende.estado, ende.cep " +
                " from Cliente cli join Endereco ende on cli.id = ende.id_cliente";
            sql += " where cli.id = " + idCliente;
            sql += " order by cli.nome asc;";

            var consulta = ConexaoBanco.executarReader(sql, out mensagem);
            if (consulta.Read())
            {
                cliente.Id = Convert.ToInt64(consulta["id"]);
                cliente.Nome = consulta["nome"].ToString();
                cliente.Cpf = consulta["cpf"].ToString();
                cliente.Sexo = Convert.ToInt32(consulta["sexo"].ToString());
                cliente.Email = consulta["email"] == DBNull.Value ? "" :  consulta["email"].ToString();
                cliente.Telefone = consulta["telefone"] == DBNull.Value ? "" : consulta["telefone"].ToString();
                cliente.DataInclusao = Convert.ToDateTime(consulta["data_inclusao"].ToString());
                ender.Cidade = consulta["cidade"].ToString();
                ender.Estado = consulta["bairro"] == DBNull.Value ? "" : consulta["estado"].ToString();
                ender.Logradouro = consulta["logradouro"].ToString();
                ender.Bairro = consulta["bairro"] == DBNull.Value ? "" : consulta["bairro"].ToString();
                ender.Numero = consulta["numero"] == DBNull.Value ? "" : consulta["numero"].ToString() ;
                ender.Cep = consulta["cep"].ToString();
                cliente.endereco = ender;
            }
            consulta.Close();
            GC.Collect();
            return cliente;
        }
    }
}
