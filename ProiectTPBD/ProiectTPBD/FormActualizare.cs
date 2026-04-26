using System;
using System.Data;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ProiectTPBD
{
    public partial class FormActualizare : Form
    {
        private DataGridView dgv;
        private TextBox txtCautare;
        private Button btnCauta, btnSalveaza, btnRefresh;

        public FormActualizare()
        {
            InitializeComponent();
            ConstruiesteFormular();
            IncarcaDate();
        }

        private void ConstruiesteFormular()
        {
            this.Text = "Actualizare Date Angajati";
            this.Size = new System.Drawing.Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Bara de cautare
            var lblCauta = new Label() { Text = "Cauta dupa nume:", Location = new System.Drawing.Point(10, 15), Width = 130 };
            txtCautare = new TextBox() { Location = new System.Drawing.Point(145, 12), Width = 200 };
            btnCauta = new Button() { Text = "Cauta", Location = new System.Drawing.Point(355, 11), Width = 80 };
            btnRefresh = new Button() { Text = "Toti angajatii", Location = new System.Drawing.Point(445, 11), Width = 110 };
            btnSalveaza = new Button() { Text = "Salveaza modificarile", Location = new System.Drawing.Point(565, 11), Width = 160, BackColor = System.Drawing.Color.LightGreen };

            btnCauta.Click += BtnCauta_Click;
            btnRefresh.Click += (s, e) => IncarcaDate();
            btnSalveaza.Click += BtnSalveaza_Click;

            // DataGridView
            dgv = new DataGridView()
            {
                Location = new System.Drawing.Point(10, 45),
                Size = new System.Drawing.Size(1060, 500),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.AddRange(new Control[] { lblCauta, txtCautare, btnCauta, btnRefresh, btnSalveaza, dgv });
        }

        private void IncarcaDate(string filtru = "")
        {
            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    string sql = "SELECT NR_CRT, NUME, PRENUME, FUNCTIE, SALAR_BAZA, SPOR_PROC, PREMII_BRUTE, RETINERI, TOTAL_BRUT, BRUT_IMPOZABIL, CAS, CASS, IMPOZIT, VIRAT_CARD FROM ANGAJATI";
                    if (filtru != "")
                        sql += " WHERE UPPER(NUME) LIKE UPPER(:filtru)";
                    sql += " ORDER BY NR_CRT";

                    var cmd = new OracleCommand(sql, con);
                    if (filtru != "")
                        cmd.Parameters.Add("filtru", "%" + filtru + "%");

                    var da = new OracleDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;

                    // Coloane read-only
                    string[] readOnly = { "NR_CRT", "TOTAL_BRUT", "BRUT_IMPOZABIL", "CAS", "CASS", "IMPOZIT", "VIRAT_CARD" };
                    foreach (string col in readOnly)
                    {
                        if (dgv.Columns.Contains(col))
                        {
                            dgv.Columns[col].ReadOnly = true;
                            dgv.Columns[col].DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCauta_Click(object sender, EventArgs e)
        {
            IncarcaDate(txtCautare.Text.Trim());
        }

        private void BtnSalveaza_Click(object sender, EventArgs e)
        {
            try
            {
                var dt = (DataTable)dgv.DataSource;
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    foreach (DataRow row in dt.Rows)
                    {
                        double salar = Convert.ToDouble(row["SALAR_BAZA"]);
                        double spor = Convert.ToDouble(row["SPOR_PROC"]);
                        double premii = Convert.ToDouble(row["PREMII_BRUTE"]);
                        double retineri = Convert.ToDouble(row["RETINERI"]);

                        int totalBrut = (int)(salar * (1 + spor / 100) + premii);
                        int cas = (int)(totalBrut * 0.25);
                        int cass = (int)(totalBrut * 0.10);
                        int brutImpozabil = totalBrut - cas - cass;
                        int impozit = (int)(brutImpozabil * 0.10);
                        int viratCard = totalBrut - impozit - cas - cass - (int)retineri;

                        string sql = @"UPDATE ANGAJATI SET 
                            NUME=:nume, PRENUME=:prenume, FUNCTIE=:functie,
                            SALAR_BAZA=:salar, SPOR_PROC=:spor, PREMII_BRUTE=:premii, RETINERI=:retineri,
                            TOTAL_BRUT=:totalbrut, BRUT_IMPOZABIL=:brutimpozabil,
                            CAS=:cas, CASS=:cass, IMPOZIT=:impozit, VIRAT_CARD=:viratcard
                            WHERE NR_CRT=:id";

                        var cmd = new OracleCommand(sql, con);
                        cmd.Parameters.Add("nume", row["NUME"]);
                        cmd.Parameters.Add("prenume", row["PRENUME"]);
                        cmd.Parameters.Add("functie", row["FUNCTIE"]);
                        cmd.Parameters.Add("salar", (int)salar);
                        cmd.Parameters.Add("spor", spor);
                        cmd.Parameters.Add("premii", (int)premii);
                        cmd.Parameters.Add("retineri", (int)retineri);
                        cmd.Parameters.Add("totalbrut", totalBrut);
                        cmd.Parameters.Add("brutimpozabil", brutImpozabil);
                        cmd.Parameters.Add("cas", cas);
                        cmd.Parameters.Add("cass", cass);
                        cmd.Parameters.Add("impozit", impozit);
                        cmd.Parameters.Add("viratcard", viratCard);
                        cmd.Parameters.Add("id", row["NR_CRT"]);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Date salvate cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                IncarcaDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}