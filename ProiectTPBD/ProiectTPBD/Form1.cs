using System;
using System.Windows.Forms;

namespace ProiectTPBD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DBHelper.TestConnection())
                MessageBox.Show("Conexiune Oracle reusita!", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}