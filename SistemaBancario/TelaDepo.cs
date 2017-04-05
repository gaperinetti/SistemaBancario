﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaBancario
{
    public partial class TelaDepo : Form
    {
        public List<Conta> Contas { get; set; }
        public TelaOperacoes tela;
        public TelaDepo()
        {
            Contas = new List<Conta>();
            InitializeComponent();
        }
        public TelaDepo(TelaOperacoes te)
        {
            Contas = new List<Conta>();
            tela = te;
            InitializeComponent();
        }
        private void textBox3_Leave(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            Contas.Clear();
            var stringConexao = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=bd;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conexao = new SqlConnection(stringConexao);

            conexao.Open();

            SqlCommand comando = new SqlCommand("SELECT CONTAS.ID, CONTAS.TIPO, CONTAS.AGENCIA, CONTAS.CONTA, CONTAS.SALDO, CLIENTES.NOME FROM CONTAS JOIN CLIENTES ON CONTAS.IdCliente = CLIENTES.IdCliente WHERE CONTAS.IdCliente = '" + textBox3.Text.ToString() + "' ", conexao);
            SqlDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                Conta conta = new Conta();
                
                conta.IdConta = Convert.ToInt32(reader["Id"]);
                conta.Tipo = reader["Tipo"].ToString();
                conta.Agencia = reader["Agencia"].ToString();
                conta.Numero = reader["Conta"].ToString();
                conta.Saldo = Convert.ToDecimal(reader["Saldo"]);
                textBox4.Text = reader["Nome"].ToString();

             
                Contas.Add(conta);
            }

            dataGridView1.DataSource = Contas;
            dataGridView1.Columns["IdConta"].Visible = false;
            dataGridView1.Columns["IdCliente"].Visible = false;

            textBox1.Clear();
            textBox2.Clear();

            conexao.Close();
        }
        Conta linha = new Conta();
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            linha = dataGridView1.SelectedRows[0].DataBoundItem as Conta;
            textBox1.Text = linha.Agencia;
            textBox2.Text = linha.Numero;
        }

        private void TelaDepo_Load(object sender, EventArgs e)
        {
            textBox3.Focus();
            textBox3.Select();
            textBox4.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text) && !String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox4.Text) && !String.IsNullOrEmpty(textBox5.Text))
            {
                var stringConexao = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=bd;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection conexao = new SqlConnection(stringConexao);

                conexao.Open();

                decimal vdeposito;
                decimal saldod;
                saldod = linha.Saldo;
                vdeposito = Convert.ToDecimal(textBox5.Text);
                decimal result;
                result = saldod + vdeposito;
                string re = result.ToString(); ;
                re = re.Replace(",", ".");
               

                SqlCommand comando = new SqlCommand("UPDATE Contas SET Saldo = '" + re + "' WHERE Id = '" + linha.IdConta + "';", conexao);
                comando.ExecuteNonQuery();
                string ddd = String.Format("{0:C}", vdeposito);
                MessageBox.Show("Depósito de " + ddd + " Efetuado Com Sucesso");

                conexao.Close();


                this.Close();
                tela.carrega();
            }
            else
            {
                MessageBox.Show("Dados Incompletos");
            }
            
        }

    }
}
