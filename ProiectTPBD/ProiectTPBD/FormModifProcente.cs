using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ProiectTPBD
{
    public partial class FormModifProcente : Form
    {
        private TextBox txtParola, txtCas, txtCass, txtImpozit, txtParolaNoua;
        private Button btnVerifica, btnSalveaza;
        private Panel panelProcente;

        public FormModifProcente()
        {
            InitializeComponent();
            ConstruiesteFormular();
        }

        private void ConstruiesteFormular()
        {
            this.Text = "Modificare Procente";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            int x1 = 20, x2 = 180, w = 150, y = 20, dy = 40;

            // Parola
            AdaugaLabel("Parola de acces:", y, x1);
            txtParola = new TextBox() { Location = new System.Drawing.Point(x2, y), Width = w, PasswordChar = '*' };
            this.Controls.Add(txtParola);
            y += dy;

            btnVerifica = new Button() { Text = "Verifica parola", Location = new System.Drawing.Point(x1, y), Width = 130 };
            btnVerifica.Click += BtnVerifica_Click;
            this.Controls.Add(btnVerifica);
            y += dy;

            // Panel cu procente (ascuns initial)
            panelProcente = new Panel() { Location = new System.Drawing.Point(0, y), Size = new System.Drawing.Size(400, 250), Visible = false };

            int py = 10;
            AdaugaLabelPanel("CAS % (pensie):", py, x1, panelProcente);
            txtCas = new TextBox() { Location = new System.Drawing.Point(x2, py), Width = w };
            panelProcente.Controls.Add(txtCas);
            py += dy;

            AdaugaLabelPanel("CASS % (sanatate):", py, x1, panelProcente);
            txtCass = new TextBox() { Location = new System.Drawing.Point(x2, py), Width = w };
            panelProcente.Controls.Add(txtCass);
            py += dy;

            AdaugaLabelPanel("Impozit %:", py, x1, panelProcente);
            txtImpozit = new TextBox() { Location = new System.Drawing.Point(x2, py), Width = w };
            panelProcente.Controls.Add(txtImpozit);
            py += dy;

            AdaugaLabelPanel("Parola noua (optional):", py, x1, panelProcente);
            txtParolaNoua = new TextBox() { Location = new System.Drawing.Point(x2, py), Width = w, PasswordChar = '*' };
            panelProcente.Controls.Add(txtParolaNoua);
            py += dy;

            btnSalveaza = new Button() { Text = "Salveaza", Location = new System.Drawing.Point(x1, py), Width = 100, BackColor = System.Drawing.Color.LightGreen };
            btnSalveaza.Click += BtnSalveaza_Click;
            panelProcente.Controls.Add(btnSalveaza);

            this.Controls.Add(panelProcente);
        }

        private void AdaugaLabel(string text, int y, int x)
        {
            var lbl = new Label() { Text = text, Location = new System.Drawing.Point(x, y + 3), Width = 160 };
            this.Controls.Add(lbl);
        }

        private void AdaugaLabelPanel(string text, int y, int x, Panel p)
        {
            var lbl = new Label() { Text = text, Location = new System.Drawing.Point(x, y + 3), Width = 160 };
            p.Controls.Add(lbl);
        }

        private void BtnVerifica_Click(object sender, EventArgs e)
        {
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    var cmd = new OracleCommand("SELECT CAS_PROC, CASS_PROC, IMPOZIT_PROC, PAROLA FROM PROCENTE", con);
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string parola = reader["PAROLA"].ToString();
                        if (txtParola.Text == parola)
                        {
                            txtCas.Text = reader["CAS_PROC"].ToString();
                            txtCass.Text = reader["CASS_PROC"].ToString();
                            txtImpozit.Text = reader["IMPOZIT_PROC"].ToString();
                            panelProcente.Visible = true;
                            this.Height = 550;
                            MessageBox.Show("Parola corecta! Puteti modifica procentele.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Parola incorecta!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSalveaza_Click(object sender, EventArgs e)
        {
            try
            {
                double cas = double.Parse(txtCas.Text);
                double cass = double.Parse(txtCass.Text);
                double impozit = double.Parse(txtImpozit.Text);

                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    string sql = "UPDATE PROCENTE SET CAS_PROC=:cas, CASS_PROC=:cass, IMPOZIT_PROC=:impozit";
                    if (txtParolaNoua.Text.Trim() != "")
                        sql += ", PAROLA=:parola";

                    var cmd = new OracleCommand(sql, con);
                    cmd.Parameters.Add("cas", cas);
                    cmd.Parameters.Add("cass", cass);
                    cmd.Parameters.Add("impozit", impozit);
                    if (txtParolaNoua.Text.Trim() != "")
                        cmd.Parameters.Add("parola", txtParolaNoua.Text.Trim());

                    cmd.ExecuteNonQuery();
                    // Recalculare salarii pentru toti angajatii
                    var cmdRecalc = new OracleCommand(@"UPDATE ANGAJATI SET
                    TOTAL_BRUT = ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE),
                    CAS = ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE) * :cas/100),
                    CASS = ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE) * :cass/100),
                    BRUT_IMPOZABIL = ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cas2/100) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cass2/100),
                    IMPOZIT = ROUND((ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cas3/100) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cass3/100)) * :impozit/100),
                    VIRAT_CARD = ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cas4/100) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cass4/100) - ROUND((ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cas5/100) - ROUND(ROUND(SALAR_BAZA*(1+SPOR_PROC/100)+PREMII_BRUTE)*:cass5/100))*:impozit2/100) - RETINERI", con);

                    cmdRecalc.Parameters.Add("cas", cas);
                    cmdRecalc.Parameters.Add("cass", cass);
                    cmdRecalc.Parameters.Add("cas2", cas);
                    cmdRecalc.Parameters.Add("cass2", cass);
                    cmdRecalc.Parameters.Add("cas3", cas);
                    cmdRecalc.Parameters.Add("cass3", cass);
                    cmdRecalc.Parameters.Add("impozit", impozit);
                    cmdRecalc.Parameters.Add("cas4", cas);
                    cmdRecalc.Parameters.Add("cass4", cass);
                    cmdRecalc.Parameters.Add("cas5", cas);
                    cmdRecalc.Parameters.Add("cass5", cass);
                    cmdRecalc.Parameters.Add("impozit2", impozit);
                    cmdRecalc.ExecuteNonQuery();
                    MessageBox.Show("Procente salvate cu succes!\nRecalculati salariile din meniu.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}