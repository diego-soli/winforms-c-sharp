using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTesteClientesDTO
{
    public class ClienteDTO
    {
       
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public int Sexo { get; set; }
        public string Telefone { get; set; }
        public DateTime DataInclusao {  get; set; }

        public EnderecoDTO endereco { get; set; }
    }
}
