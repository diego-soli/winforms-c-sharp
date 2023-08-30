using AppTesteClientesBLL;
using AppTesteClientesDTO;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace AppTesteClientesUI
{
    public partial class CadastroClienteUI : Form
    {

        const string _mascaraTelefone = "(99) 00000-0000"; //padrao máscara celular;
        public CadastroClienteUI()
        {
            InitializeComponent();
        }


        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();   
        }


        /// <summary>
        /// CONSULTA DE CEP consumindo API da viacep.com.br, utilizado Newtonsoft.Json para desserialização
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mskCep_KeyUp(object sender, KeyEventArgs e)
        {
            if (RemoveCaractere(mskCep.Text) == "") return;

            string cep = mskCep.Text.Replace(".", "").Replace("-", "");
            if (cep.Length == 8)
            {
                string jsonResult = string.Empty;
                bool consulta = new ConsultaCepBll().ConsultaCep(cep, out jsonResult);
                if (consulta)
                {
                    dynamic json = JObject.Parse(jsonResult);
                    txtLogradouro.Text = (string)json.logradouro;
                    txtCidade.Text = (string)json.localidade;
                    txtBairro.Text = (string)json.bairro;
                    txtEstado.Text = (string)json.uf;
                    txtNumero.Focus();
                    return;
                }               
            }
        }
        private void btnNovoCliente_Click(object sender, EventArgs e)
        {
            string mensagem = string.Empty;

            if (!ValidaCampos(out mensagem,false))
            {
                MessageBox.Show(mensagem, "Valida Campos",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            if ( MessageBox.Show("Confirma inclusão de novo cliente?","Inclusão de Cliente",MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.No ) 
            {
                return;
            }

            if (!GravaNovoCliente(out mensagem))
            {
                MessageBox.Show("Falha na gravação do cadastro!" + "\n" + mensagem, "Erro de Inclusão",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Cadastro incluído com sucesso!", "Confirmação de Inclusão", MessageBoxButtons.OK, MessageBoxIcon.None);
            LimparTela();
            
        }

        private void LimparTela()
        {
            txtNomeCliente.Text = string.Empty;
            txtId.Text = string.Empty;
            txtCpf.Text = string.Empty;
            mskTelefone.Text = string.Empty;
            mskTelefone.Mask = _mascaraTelefone; 
            txtEmail.Text = string.Empty;
            cmbGenero.SelectedIndex = -1;

            mskCep.Text = "";
            txtLogradouro.Text = string.Empty;
            txtNumero.Text = string.Empty;
            txtBairro.Text = string.Empty;
            txtCidade.Text = string.Empty;
            txtEstado.Text = string.Empty;
            cmbFiltro.SelectedIndex = -1;
            
            btnNovoCliente.Enabled = false;
            btnNovoCliente.BackColor = SystemColors.Control;
            btnExcluirCliente.Enabled = false;
            btnAtualizarCliente.Enabled = false;

            dtgCliente.DataSource = null;
        }

        /// <summary>
        /// Monta os campos para envio a DAL
        /// </summary>
        /// <returns>ClienteDTO</returns>
        private ClienteDTO PreparaDadosCliente(long idCliente)
        {
            ClienteDTO cliente = new ClienteDTO();
            if (idCliente > 0) cliente.Id = idCliente;
            cliente.Nome = txtNomeCliente.Text;
            cliente.Email = txtEmail.Text;
            cliente.Telefone = RemoveCaractere(mskTelefone.Text);
            cliente.Cpf = RemoveCaractere(txtCpf.Text);
            cliente.Sexo = (int)cmbGenero.SelectedIndex;
            cliente.DataInclusao = DateTime.Now;

            EnderecoDTO endereco = new EnderecoDTO();
            endereco.Logradouro = txtLogradouro.Text;
            endereco.Cidade = txtCidade.Text;
            endereco.Cep = RemoveCaractere(mskCep.Text); 
            endereco.Estado = txtEstado.Text;
            endereco.Bairro = txtBairro.Text;
            endereco.Numero = txtNumero.Text;
            cliente.endereco = endereco;

            return cliente;
        }

        private bool GravaNovoCliente(out string mensagem)
        {
            var cliente = PreparaDadosCliente(0);

            long codigoCliente = 0;
            mensagem = string.Empty;
            bool resultado = new ClienteBLL().GravaCliente(cliente, out codigoCliente, out mensagem);
            return resultado;

        }

        private bool AtualizaCliente(out string mensagem)
        {
            long codigoCliente = Convert.ToInt64(txtId.Text);
            var cliente = PreparaDadosCliente(codigoCliente);
            
            mensagem = string.Empty;
            bool resultado = new ClienteBLL().AtualizarCliente(cliente,  out mensagem);
            return resultado;
        }

        private void ListaCliente(out string mensagem)
        {         
            var lista = new ClienteBLL().ListarCliente(out mensagem);
            dtgCliente.AutoGenerateColumns = false;
            dtgCliente.DataSource = lista;
        }

        private void btnLimparTela_Click(object sender, EventArgs e)
        {
            LimparTela();
        }

        private void btnListarCliente_Click(object sender, EventArgs e)
        {
            string mensagem = string.Empty;
            ListaCliente(out mensagem);

        }

        private bool ExcluiCliente(out string mensagem)
        {
            long codigoCliente = Convert.ToInt64(txtId.Text);

            mensagem = string.Empty;
            bool resultado = new ClienteBLL().ExcluirCliente(codigoCliente, out mensagem);
            return resultado;
        }

        private void CadastroClienteUI_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
        }

        private bool ValidaCampos(out string mensagem, bool idRequerido)
        {
            mensagem = string.Empty;

            if (idRequerido && string.IsNullOrEmpty(txtId.Text))
            {
                mensagem = "Um cliente deve estar carregado na tela para continuar";
                return false;
            }

            string telefone = RemoveCaractere(mskTelefone.Text);
            if (telefone.Length < 10)
            {
                mensagem = "Número de telefone incompleto, informe (prefixo) mais o número com 8 ou 9 dígitos.";
                return false;
            }

            string cpf = RemoveCaractere(txtCpf.Text);
            if (cpf.Length != 11)
            {
                mensagem = "Número de CPF incompleto, o correto são 11 dígitos.";
                return false;
            }

            if (string.IsNullOrEmpty(txtCidade.Text))
            {
                mensagem = "Informe uma cidade.";
                return false;
            }

            if (string.IsNullOrEmpty(txtBairro.Text))
            {
                mensagem = "Informe um bairro.";
                return false;
            }
            if (string.IsNullOrEmpty(txtEstado.Text))
            {
                mensagem = "Informe um estado.";
                return false;
            }

            string cep = RemoveCaractere(mskCep.Text);
            if (cep.Length != 8)
            {
                mensagem = "CEP incompleto, informe os 8 dígitos.";
                return false;
            }

            return true;
        }

        private void mskTelefone_Leave(object sender, EventArgs e)
        {
            string telefone = RemoveCaractere(mskTelefone.Text); 
            if (telefone.Length == 10)
                mskTelefone.Mask = "(99) 0000-0000";

            if (telefone.Length == 11)
                mskTelefone.Mask = _mascaraTelefone; 
        }

        private void mskTelefone_Enter(object sender, EventArgs e)
        {
            mskTelefone.Mask = _mascaraTelefone;
        }

        private void txtNomeCliente_TextChanged(object sender, EventArgs e)
        {
            if (txtNomeCliente.Text.Length == 0)
            {
                btnNovoCliente.Enabled = false;
                btnNovoCliente.BackColor = SystemColors.Control;
            } else
            {
                if (string.IsNullOrEmpty(txtId.Text)) {

                    btnNovoCliente.Enabled = true;
                    btnNovoCliente.BackColor = Color.LightGreen;
                } 
            }
        }

        private void txtNomeCliente_Leave(object sender, EventArgs e)
        {
            if (txtNomeCliente.Text.Length == 0 && btnNovoCliente.Enabled)
            {
                MessageBox.Show("Informar um nome para continuar.");
                txtNomeCliente.Focus();
                return;
            }                
            
        }
        private string  RemoveCaractere(string texto)
        {
            string resultado = texto.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace(".", "").Replace("-", ""); ;
            return resultado;
        }

        private void btnAtualizarCliente_Click(object sender, EventArgs e)
        {
            string mensagem = string.Empty;

            if (!ValidaCampos(out mensagem,true))
            {
                MessageBox.Show(mensagem, "Valida Campos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            if (MessageBox.Show("Confirma alteração do cliente?", "Alteração de Cliente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            if (!AtualizaCliente(out mensagem))
            {
                MessageBox.Show("Falha na atualização do cadastro!" + "\n" + mensagem, "Erro de Atualização", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Cadastro atualizado com sucesso!", "Confirmação de Atualização", MessageBoxButtons.OK, MessageBoxIcon.None);
            LimparTela();
        }

        private void btnExcluirCliente_Click(object sender, EventArgs e)
        {
            string mensagem = string.Empty;

            if (string.IsNullOrEmpty(txtId.Text))
            {
                MessageBox.Show("Um cliente deve estar carregado na tela para continuar");
                return;                
            }

            if (MessageBox.Show("Confirma exclusão do cliente?", "Exclusão de Cliente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            if (!ExcluiCliente(out mensagem))
            {
                MessageBox.Show("Falha na exclusão do cliente!" + "\n" + mensagem, "Erro de Exclusão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Cadastro excluído com sucesso!", "Confirmação de Exclusão", MessageBoxButtons.OK, MessageBoxIcon.None);
            LimparTela();
        }

        private void dtgCliente_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ( e.RowIndex >= 0  && dtgCliente.Rows[e.RowIndex].Cells[0].Value.ToString() != "" )
            {
                long idCliente = Convert.ToInt64(dtgCliente.Rows[e.RowIndex].Cells[0].Value);
                string mensagem = string.Empty;
                CarregaCliente(idCliente, out mensagem);
            }
        }

        private void CarregaCliente(long idCliente ,out string mensagem)
        {
            var cliente = new ClienteBLL().BuscarCliente(idCliente, out mensagem);

            txtNomeCliente.Text = cliente.Nome;
            txtId.Text = cliente.Id.ToString();
            txtCpf.Text = cliente.Cpf;
            mskTelefone.Text = cliente.Telefone;
            txtEmail.Text = cliente.Email;
            cmbGenero.SelectedIndex = cliente.Sexo;

            mskCep.Text = cliente.endereco.Cep; 
            txtLogradouro.Text = cliente.endereco.Logradouro;
            txtNumero.Text = cliente.endereco.Numero;
            txtBairro.Text = cliente.endereco.Bairro;
            txtCidade.Text = cliente.endereco.Cidade;
            txtEstado.Text = cliente.endereco.Estado;
            
            btnNovoCliente.Enabled = false;
            btnExcluirCliente.Enabled = true;
            btnAtualizarCliente.Enabled = true;




        }
    }
}
