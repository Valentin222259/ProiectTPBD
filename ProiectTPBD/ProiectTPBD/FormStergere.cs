using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ProiectTPBD
{
    public partial class FormStergere : Form
    {
        private TextBox txtNume, txtPrenume;
        private DataGridView dgv;
        private Button btnCauta, btnSterge;

        public FormStergere()
        {
            InitializeComponent();
            ConstruiesteFormular();
        }

        private void ConstruiesteFormular()
        {
            this.Text = "Stergere Angajat";
            this.Size = new System.Drawing.Size(900, 500);
            this.StartPosition = FormStartPosition.CenterScreen;

            var lblNume = new Label() { Text = "Nume:", Location = new System.Drawing.Point(10, 15), Width = 60 };
            txtNume = new TextBox() { Location = new System.Drawing.Point(75, 12), Width = 150 };
            var lblPrenume = new Label() { Text = "Prenume:", Location = new System.Drawing.Point(240, 15), Width = 65 };
            txtPrenume = new TextBox() { Location = new System.Drawing.Point(310, 12), Width = 150 };
            btnCauta = new Button() { Text = "Cauta", Location = new System.Drawing.Point(470, 11), Width = 80 };
            btnSterge = new Button() { Text = "Sterge selectat", Location = new System.Drawing.Point(560, 11), Width = 120, BackColor = System.Drawing.Color.LightCoral };

            btnCauta.Click += BtnCauta_Click;
            btnSterge.Click += BtnSterge_Click;

            dgv = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 45),
                Size = new System.Drawing.Size(860, 400),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.Controls.AddRange(new Control[] { lblNume, txtNume, lblPrenume, txtPrenume, btnCauta, btnSterge, dgv });
        }

        private void BtnCauta_Click(object sender, EventArgs e)
        {
            if (txtNume.Text.Trim() == "" && txtPrenume.Text.Trim() == "")
            {
                MessageBox.Show("Introduceti cel putin Numele sau Prenumele!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    string sql = "SELECT NR_CRT, NUME, PRENUME, FUNCTIE, SALAR_BAZA, TOTAL_BRUT, VIRAT_CARD FROM ANGAJATI WHERE 1=1";
                    if (txtNume.Text.Trim() != "")
                        sql += " AND UPPER(NUME) LIKE UPPER(:nume)";
                    if (txtPrenume.Text.Trim() != "")
                        sql += " AND UPPER(PRENUME) LIKE UPPER(:prenume)";

                    var cmd = new OracleCommand(sql, con);
                    if (txtNume.Text.Trim() != "")
                        cmd.Parameters.Add("nume", "%" + txtNume.Text.Trim() + "%");
                    if (txtPrenume.Text.Trim() != "")
                        cmd.Parameters.Add("prenume", "%" + txtPrenume.Text.Trim() + "%");

                    var da = new OracleDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                        MessageBox.Show("Nu a fost gasit niciun angajat!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        dgv.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSterge_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selectati un angajat din lista!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgv.SelectedRows[0];
            string nume = row.Cells["NUME"].Value.ToString();
            string prenume = row.Cells["PRENUME"].Value.ToString();
            int id = Convert.ToInt32(row.Cells["NR_CRT"].Value);

            var confirm = MessageBox.Show(
                $"Sigur doriti sa stergeti angajatul {nume} {prenume}?",
                "Confirmare stergere",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    using (var con = DBHelper.GetConnection())
                    {
                        con.Open();
                        var cmd = new OracleCommand("DELETE FROM ANGAJATI WHERE NR_CRT=:id", con);
                        cmd.Parameters.Add("id", id);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Angajat sters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgv.DataSource = null;
                        txtNume.Text = "";
                        txtPrenume.Text = "";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}