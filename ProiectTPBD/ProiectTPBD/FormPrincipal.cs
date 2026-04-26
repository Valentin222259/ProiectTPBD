using System;
using System.Windows.Forms;

namespace ProiectTPBD
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
            ConstruiesteeMeniu();
        }

        private void ConstruiesteeMeniu()
        {
            MenuStrip menuStrip = new MenuStrip();

            // AJUTOR
            ToolStripMenuItem ajutor = new ToolStripMenuItem("AJUTOR");
            ajutor.Click += (s, e) => MessageBox.Show(
                "Aplicatie calcul salarii\n\nOptiuni disponibile:\n- INTRODUCERE DATE: adaugare, modificare, stergere angajati\n- TIPARIRE: stat plata si fluturasi\n- MODIF_PROCENTE: modificare procente CAS/CASS/Impozit\n- IESIRE: parasire program",
                "Ajutor", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // INTRODUCERE DATE
            ToolStripMenuItem introducereDate = new ToolStripMenuItem("INTRODUCERE DATE");
            ToolStripMenuItem actualizare = new ToolStripMenuItem("Actualizare date");
            ToolStripMenuItem adaugare = new ToolStripMenuItem("Adaugare angajati");
            ToolStripMenuItem stergere = new ToolStripMenuItem("Stergere angajati");
            ToolStripMenuItem calcul = new ToolStripMenuItem("Calcul salarii");
            introducereDate.DropDownItems.AddRange(new ToolStripItem[] { actualizare, adaugare, stergere, calcul });

            // TIPARIRE
            ToolStripMenuItem tiparire = new ToolStripMenuItem("TIPARIRE");
            ToolStripMenuItem statPlata = new ToolStripMenuItem("Stat plata");
            ToolStripMenuItem fluturasi = new ToolStripMenuItem("Fluturasi");
            tiparire.DropDownItems.AddRange(new ToolStripItem[] { statPlata, fluturasi });

            // MODIF_PROCENTE
            ToolStripMenuItem modifProcente = new ToolStripMenuItem("MODIF_PROCENTE");
            modifProcente.Click += (s, e) => MessageBox.Show("Functie in dezvoltare", "Info");

            // IESIRE
            ToolStripMenuItem iesire = new ToolStripMenuItem("IESIRE");
            iesire.Click += (s, e) => Application.Exit();

            menuStrip.Items.AddRange(new ToolStripItem[] { ajutor, introducereDate, tiparire, modifProcente, iesire });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            if (DBHelper.TestConnection())
                this.Text = "Sistem Calcul Salarii - Conectat la Oracle";
        }
    }
}