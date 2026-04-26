using System;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ProiectTPBD
{
    public partial class FormAdaugare : Form
    {
        private TextBox txtNume, txtPrenume, txtFunctie, txtSalarBaza, txtSpor, txtPremii, txtRetineri;
        private Label lblTotalBrut, lblBrutImpozabil, lblCas, lblCass, lblImpozit, lblViratCard;
        private Button btnSalveaza, btnNou;

        public FormAdaugare()
        {
            InitializeComponent();
            ConstruiesteFormular();
        }

        private void ConstruiesteFormular()
        {
            this.Text = "Adaugare Angajat";
            this.Size = new System.Drawing.Size(500, 550);

            int x1 = 20, x2 = 200, w = 200, h = 25, dy = 35, y = 20;

            // Campuri editabile
            AdaugaLinie("Nume:", ref y, x1, x2, w, h, dy, out txtNume);
            AdaugaLinie("Prenume:", ref y, x1, x2, w, h, dy, out txtPrenume);
            AdaugaLinie("Functie:", ref y, x1, x2, w, h, dy, out txtFunctie);
            AdaugaLinie("Salar Baza:", ref y, x1, x2, w, h, dy, out txtSalarBaza);
            AdaugaLinie("Spor (%):", ref y, x1, x2, w, h, dy, out txtSpor);
            AdaugaLinie("Premii Brute:", ref y, x1, x2, w, h, dy, out txtPremii);
            AdaugaLinie("Retineri:", ref y, x1, x2, w, h, dy, out txtRetineri);

            txtSpor.Text = "0";
            txtPremii.Text = "0";
            txtRetineri.Text = "0";

            // Campuri calculate (read-only)
            AdaugaLabel("Total Brut:", y, x1); lblTotalBrut = AdaugaLabelValoare(y, x2); y += dy;
            AdaugaLabel("Brut Impozabil:", y, x1); lblBrutImpozabil = AdaugaLabelValoare(y, x2); y += dy;
            AdaugaLabel("CAS (25%):", y, x1); lblCas = AdaugaLabelValoare(y, x2); y += dy;
            AdaugaLabel("CASS (10%):", y, x1); lblCass = AdaugaLabelValoare(y, x2); y += dy;
            AdaugaLabel("Impozit (10%):", y, x1); lblImpozit = AdaugaLabelValoare(y, x2); y += dy;
            AdaugaLabel("Virat Card:", y, x1); lblViratCard = AdaugaLabelValoare(y, x2); y += dy;

            // Butoane
            btnSalveaza = new Button() { Text = "Salveaza", Location = new System.Drawing.Point(x1, y), Width = 100 };
            btnNou = new Button() { Text = "Angajat Nou", Location = new System.Drawing.Point(150, y), Width = 120 };
            btnSalveaza.Click += BtnSalveaza_Click;
            btnNou.Click += BtnNou_Click;
            this.Controls.AddRange(new Control[] { btnSalveaza, btnNou });

            // Calcul automat la modificare
            txtSalarBaza.TextChanged += (s, e) => Calculeaza();
            txtSpor.TextChanged += (s, e) => Calculeaza();
            txtPremii.TextChanged += (s, e) => Calculeaza();
            txtRetineri.TextChanged += (s, e) => Calculeaza();
        }

        private void AdaugaLinie(string label, ref int y, int x1, int x2, int w, int h, int dy, out TextBox txt)
        {
            AdaugaLabel(label, y, x1);
            txt = new TextBox() { Location = new System.Drawing.Point(x2, y), Width = w };
            this.Controls.Add(txt);
            y += dy;
        }

        private void AdaugaLabel(string text, int y, int x)
        {
            var lbl = new Label() { Text = text, Location = new System.Drawing.Point(x, y + 3), Width = 150 };
            this.Controls.Add(lbl);
        }

        private Label AdaugaLabelValoare(int y, int x)
        {
            var lbl = new Label() { Text = "0", Location = new System.Drawing.Point(x, y + 3), Width = 200, ForeColor = System.Drawing.Color.Blue, Font = new System.Drawing.Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Bold) };
            this.Controls.Add(lbl);
            return lbl;
        }

        private void Calculeaza()
        {
            try
            {
                double salar = double.Parse(txtSalarBaza.Text == "" ? "0" : txtSalarBaza.Text);
                double spor = double.Parse(txtSpor.Text == "" ? "0" : txtSpor.Text);
                double premii = double.Parse(txtPremii.Text == "" ? "0" : txtPremii.Text);
                double retineri = double.Parse(txtRetineri.Text == "" ? "0" : txtRetineri.Text);

                int totalBrut = (int)(salar * (1 + spor / 100) + premii);
                int cas = (int)(totalBrut * 0.25);
                int cass = (int)(totalBrut * 0.10);
                int brutImpozabil = totalBrut - cas - cass;
                int impozit = (int)(brutImpozabil * 0.10);
                int viratCard = totalBrut - impozit - cas - cass - (int)retineri;

                lblTotalBrut.Text = totalBrut.ToString();
                lblBrutImpozabil.Text = brutImpozabil.ToString();
                lblCas.Text = cas.ToString();
                lblCass.Text = cass.ToString();
                lblImpozit.Text = impozit.ToString();
                lblViratCard.Text = viratCard.ToString();
            }
            catch { }
        }

        private void BtnSalveaza_Click(object sender, EventArgs e)
        {
            if (txtNume.Text == "" || txtPrenume.Text == "" || txtSalarBaza.Text == "")
            {
                MessageBox.Show("Completati campurile obligatorii: Nume, Prenume, Salar Baza!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = DBHelper.GetConnection())
                {
                    con.Open();
                    string sql = @"INSERT INTO ANGAJATI (NUME, PRENUME, FUNCTIE, SALAR_BAZA, SPOR_PROC, PREMII_BRUTE, RETINERI, TOTAL_BRUT, BRUT_IMPOZABIL, CAS, CASS, IMPOZIT, VIRAT_CARD)
                        VALUES (:nume, :prenume, :functie, :salar, :spor, :premii, :retineri, :totalbrut, :brutimpozabil, :cas, :cass, :impozit, :viratcard)";
                    var cmd = new OracleCommand(sql, con);
                    cmd.Parameters.Add("nume", txtNume.Text);
                    cmd.Parameters.Add("prenume", txtPrenume.Text);
                    cmd.Parameters.Add("functie", txtFunctie.Text);
                    cmd.Parameters.Add("salar", int.Parse(txtSalarBaza.Text));
                    cmd.Parameters.Add("spor", double.Parse(txtSpor.Text));
                    cmd.Parameters.Add("premii", int.Parse(txtPremii.Text));
                    cmd.Parameters.Add("retineri", int.Parse(txtRetineri.Text));
                    cmd.Parameters.Add("totalbrut", int.Parse(lblTotalBrut.Text));
                    cmd.Parameters.Add("brutimpozabil", int.Parse(lblBrutImpozabil.Text));
                    cmd.Parameters.Add("cas", int.Parse(lblCas.Text));
                    cmd.Parameters.Add("cass", int.Parse(lblCass.Text));
                    cmd.Parameters.Add("impozit", int.Parse(lblImpozit.Text));
                    cmd.Parameters.Add("viratcard", int.Parse(lblViratCard.Text));
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Angajat adaugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare: " + ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNou_Click(object sender, EventArgs e)
        {
            txtNume.Text = "";
            txtPrenume.Text = "";
            txtFunctie.Text = "";
            txtSalarBaza.Text = "";
            txtSpor.Text = "0";
            txtPremii.Text = "0";
            txtRetineri.Text = "0";
            lblTotalBrut.Text = "0";
            lblBrutImpozabil.Text = "0";
            lblCas.Text = "0";
            lblCass.Text = "0";
            lblImpozit.Text = "0";
            lblViratCard.Text = "0";
            txtNume.Focus();
        }
    }
}