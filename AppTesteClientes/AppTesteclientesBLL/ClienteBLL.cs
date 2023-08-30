using AppTesteClientesDAL;
using AppTesteClientesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTesteClientesBLL
{
    public class ClienteBLL
    {
        private ClienteDAL _clienteDAL;
        public ClienteBLL() { 
        
            _clienteDAL = new ClienteDAL();
        
        }
        /// <summary>
        /// Grava os dados do novo cliente no banco de dados.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="codigoCliente"></param>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public bool GravaCliente(ClienteDTO dto, out long codigoCliente, out string mensagem)
        {
            codigoCliente = 0;
            return _clienteDAL.IncluirCliente(dto, out codigoCliente, out mensagem);

        }

        /// <summary>
        /// Efetua a atualizacao dos dados do cliente na base de dados.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="mensagem"></param>
        /// <returns>bool</returns>
        public bool AtualizarCliente(ClienteDTO dto, out string mensagem)
        {            
            return _clienteDAL.AtualizarCliente(dto, out mensagem);

        }

        /// <summary>
        /// Efetua a exclusão individual do registro, retorna um valor booleano e uma mensagem caso 
        /// ocorra alguma exception, no endereço ha uma chave estrangeira com a opcao cascade
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="mensagem"></param>
        /// <returns>bool</returns>
        public bool ExcluirCliente(long idCliente, out string mensagem)
        {
            return _clienteDAL.ExcluirCliente(idCliente, out mensagem);

        }

        /// <summary>
        /// Busca na base todos os clientes para apresentar no grid
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns>List<ListaClienteGridDTO</returns>
        public List<ListaClienteGridDTO> ListarCliente(out string mensagem)
        {  //return null
            return _clienteDAL.ListarCliente(out mensagem);
        }

        /// <summary>
        /// Carrega todos os dados do cliente para popular nos campos na UI.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="mensagem"></param>
        /// <returns>ClienteDTO</returns>
        public ClienteDTO BuscarCliente(long idCliente, out string mensagem)
        {  
            return _clienteDAL.BuscarCliente(idCliente,out mensagem);
        }
    }
}
