using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projet_PPE_ver0._1
{
    public partial class FormPanel : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=.\sqlexpress;database=GM;Integrated Security=True");
        SqlCommand cmd;
        DataTable dtClient;
        DataTable dtMateriel;
        DataTable dtIntervention;
        SqlDataAdapter adptClient;
        SqlDataAdapter adptMateriel;
        SqlDataAdapter adptIntervention;
        int idClient;
        int idMateriel;
        int idIntervention;
        public FormPanel()
        {
            InitializeComponent();
            showdataClient();
            showdataMateriel();
            showdataIntervention();
            fillCombo();
            fillComboInter();
        }
        private void FormPanel_Load(object sender, EventArgs e)
        {   
            // close formlogin
            FormLogin formLogin = new FormLogin();
                formLogin.Close();
        }


        /*
        ----- START CLIENT PART -----
        */
        public void showdataClient()
        {
            con.Open();
            adptClient = new SqlDataAdapter("Select ID_CLIENT as ID,Nom as Nom,Adresse as Adresse,Mail as Mail,Tel as Téléphone from CLIENT", con);
            dtClient = new DataTable();
            adptClient.Fill(dtClient);
            dataGridViewClients.DataSource = dtClient;
            con.Close();
        }


        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            showdataClient();
            showdataMateriel();
            showdataIntervention();
            comboBoxMat.Items.Clear();
            comboBoxInter.Items.Clear();
            fillCombo();
            fillComboInter();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else
            {
                DataGridViewRow row = this.dataGridViewClients.Rows[e.RowIndex];
                if (row.Cells["ID"].Value.ToString() != "")
                {
                    idClient = Convert.ToInt32(row.Cells["ID"].Value.ToString());
                    textBoxNomClient.Text = row.Cells["Nom"].Value.ToString();
                    textBoxAdresseClient.Text = row.Cells["Adresse"].Value.ToString();
                    textBoxMailClient.Text = row.Cells["Mail"].Value.ToString();
                    textBoxTelClient.Text = row.Cells["Téléphone"].Value.ToString();
                    var NomClient = textBoxNomClient.Text;
                    var adresseClient = textBoxAdresseClient.Text;
                    var mailClient = textBoxMailClient.Text;
                    var telClient = textBoxTelClient.Text;
                }
                else
                {
                    textBoxNomClient.Text = "";
                    textBoxAdresseClient.Text = "";
                    textBoxMailClient.Text = "";
                    textBoxTelClient.Text = "";

                }
            }
        }
        private void buttonSupprimer_Click(object sender, EventArgs e)
        {



            if (MessageBox.Show("Voulez vous vraiment supprimer ce client ?" + "\n" +
                "Attention, toutes les interventions et matériaux liées à ce client seront supprimées"
                , "Supprimer un client",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {   
                con.Open();
                cmd = new SqlCommand("delete from CLIENT where ID_CLIENT='" + idClient + "'", con);
                
                cmd.ExecuteNonQuery();
                textBoxNomClient.Text = "";
                textBoxAdresseClient.Text = "";
                textBoxMailClient.Text = "";
                textBoxTelClient.Text = "";
                MessageBox.Show("Client supprimé");
                con.Close();
                showdataClient();
            }
        }

        private void buttonAjouter_Click(object sender, EventArgs e)
        {
            // check if fields already exist
            
            con.Open();
            SqlCommand check_mail = new SqlCommand("SELECT COUNT(*) FROM CLIENT WHERE Mail = @Mail", con);
            check_mail.Parameters.AddWithValue("@Mail", textBoxMailClient.Text);
            int UserExist = (int)check_mail.ExecuteScalar();
            con.Close();
            if (UserExist > 0)
            {
                MessageBox.Show("Cette adresse email existe déjà");
            }
            else
            {
                if (textBoxNomClient.Text != "" && textBoxAdresseClient.Text != "" && textBoxMailClient.Text != "" && textBoxTelClient.Text != "")
                {
                    Materiel selectedClient = (Materiel)comboBoxMat.SelectedItem;
                    con.Open();
                    cmd = new SqlCommand("insert into CLIENT(Nom,Adresse,Mail,Tel) values" +
                        "('" + textBoxNomClient.Text +
                        "','" + textBoxAdresseClient.Text +
                        "','" + textBoxMailClient.Text +
                        "','" + textBoxTelClient.Text + "')", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Client ajouté");
                    con.Close();
                    showdataClient();
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
            }
        }

        private void buttonModifier_Click(object sender, EventArgs e)
        {

            // check if fields changed
            
                if (textBoxNomClient.Text != "" && textBoxAdresseClient.Text != "" && textBoxMailClient.Text != "" && textBoxTelClient.Text != "")
                {
                    con.Open();
                    cmd = new SqlCommand("update CLIENT set Nom='" + textBoxNomClient.Text + "',Adresse='" +
                        textBoxAdresseClient.Text + "',Mail='" + textBoxMailClient.Text + "',Tel='" +
                        textBoxTelClient.Text + "' where ID_CLIENT='" + idClient + "'", con);
                    
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Les informations ont bien été modifié");
                    con.Close();
                    showdataClient();
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
            
        }
        /*
        ----- END CLIENT PART -----
        */





        /*
        ----- START MATERIEL PART -----
        */


        public void showComboBox()
        {
            // add client to combobox
            con.Open();
            cmd = new SqlCommand("Select Nom from CLIENT", con);
            SqlDataReader dr;
            dr = cmd.ExecuteReader();
            

        }

        public void fillCombo()
        {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM CLIENT", con);
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter dal = new SqlDataAdapter(cmd);

            dal.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                int id = Convert.ToInt32(dr["ID_CLIENT"].ToString());
                string nom = dr["Nom"].ToString();
                string adresse = dr["Adresse"].ToString();
                string mail = dr["Mail"].ToString();
                string tel = dr["Tel"].ToString();

                Materiel matos = new Materiel(id,nom,adresse,mail,tel);

                comboBoxMat.Items.Add(matos);


            }
            con.Close();
        }

        public void showdataMateriel()
        {
            con.Open();
            adptMateriel = new SqlDataAdapter("Select m.ID_MATERIEL as ID_MAT,m.Nom as Nom,m.NoSerie as " +
                "NoSerie,m.Date_Install as Date_Install,m.MTBF as MTBF,m.Type as Type,m.Marque as Marque,c.Nom as Client from " +
                "MATERIEL m join CLIENT c on m.ID_CLIENT = c.ID_CLIENT", con);  

            dtMateriel = new DataTable();
            adptMateriel.Fill(dtMateriel);
            dataGridViewMateriel.DataSource = dtMateriel;
            con.Close();
        }

        private void dataGridView2_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else
            {
                DataGridViewRow row = this.dataGridViewMateriel.Rows[e.RowIndex];
                if (row.Cells["ID_MAT"].Value.ToString() != "")
                {
                    idMateriel = Convert.ToInt32(row.Cells["ID_MAT"].Value.ToString());
                    textBoxNomMateriel.Text = row.Cells["Nom"].Value.ToString();
                    textBoxNoSerieMateriel.Text = row.Cells["NoSerie"].Value.ToString();
                    dateTimePicker1.Value = DateTime.Parse(row.Cells["Date_Install"].Value.ToString());
                    textBoxMTBFMateriel.Text = row.Cells["MTBF"].Value.ToString();
                    textBoxTypeMateriel.Text = row.Cells["Type"].Value.ToString();
                    textBoxMarqueMateriel.Text = row.Cells["Marque"].Value.ToString();
                    comboBoxMat.Text = row.Cells["Client"].Value.ToString();
                }
                else
                {
                    textBoxNomMateriel.Text = "";
                    textBoxNoSerieMateriel.Text = "";
                    dateTimePicker1.Value = DateTime.Now;
                    textBoxMTBFMateriel.Text = "";
                    textBoxTypeMateriel.Text = "";
                    textBoxMarqueMateriel.Text = "";
                    comboBoxMat.Text = "";
                }
            }
        }

        private void buttonAjouterMateriel_Click(object sender, EventArgs e)
        {
            // check if fields already exist

            con.Open();
            SqlCommand check_NoSerie = new SqlCommand("SELECT COUNT(*) FROM MATERIEL WHERE NoSerie = @NoSerie", con);
            check_NoSerie.Parameters.AddWithValue("@NoSerie", textBoxNoSerieMateriel.Text);
            int UserExist = (int)check_NoSerie.ExecuteScalar();
            con.Close();




            if (UserExist > 0)
            {
                MessageBox.Show("Ce matériel existe déjà");
            }
            else
            {

                if (textBoxNomMateriel.Text != "" && textBoxNoSerieMateriel.Text != "" && /*textBoxDateInstallMateriel.Text != "" &&*/ textBoxMTBFMateriel.Text != "" && textBoxTypeMateriel.Text != "" && textBoxMarqueMateriel.Text != "")
                {
                    Materiel selectedClient = (Materiel)comboBoxMat.SelectedItem;
                    int idClient = selectedClient.ID;

                    con.Open();
                    cmd = new SqlCommand("insert into MATERIEL(Nom,NoSerie,Date_Install,MTBF,Type,Marque,ID_CLIENT) values('" +
                        textBoxNomMateriel.Text + "','" + textBoxNoSerieMateriel.Text + "','" +
                        dateTimePicker1.Value + "','" +  textBoxMTBFMateriel.Text + "','" +
                        textBoxTypeMateriel.Text + "','" + textBoxMarqueMateriel.Text + "','" + idClient + "')", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Materiel ajouté");
                    con.Close();
                    showdataMateriel();
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
            }
        }

        private void buttonSupprimerMateriel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Voulez vous vraiment supprimer ce matériel ?", "Supprimer un matériel", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                con.Open();
                cmd = new SqlCommand("delete from MATERIEL where ID_MATERIEL='" + idMateriel + "'", con);
                cmd.ExecuteNonQuery();
                textBoxNomMateriel.Text = "";
                textBoxNoSerieMateriel.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                textBoxMTBFMateriel.Text = "";
                textBoxTypeMateriel.Text = "";
                textBoxMarqueMateriel.Text = "";
                comboBoxMat.Text = "";
                MessageBox.Show("Matériel supprimé");
                con.Close();
                showdataMateriel();
            }

            
        }

        private void buttonModifierMateriel_Click(object sender, EventArgs e)
        {
                if (textBoxNomMateriel.Text != "" && textBoxNoSerieMateriel.Text != "" && /*textBoxDateInstallMateriel.Text != "" &&*/ textBoxMTBFMateriel.Text != "" && textBoxTypeMateriel.Text != "" && textBoxMarqueMateriel.Text != "" )
                {
                
                Materiel selectedClient = (Materiel)comboBoxMat.SelectedItem;
                int idClient = selectedClient.ID;
                
                con.Open();
                    cmd = new SqlCommand
                    ("update MATERIEL set Nom='" + textBoxNomMateriel.Text +
                    "',NoSerie='" + textBoxNoSerieMateriel.Text +
                    "',MTBF='" + textBoxMTBFMateriel.Text +
                    "',Type='" + textBoxTypeMateriel.Text + 
                    "',Marque='" + textBoxMarqueMateriel.Text +
                    "',ID_CLIENT='" + idClient +
                    "' where ID_MATERIEL='" + idMateriel + "'", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Matériel modifié");
                    con.Close();
                    showdataMateriel();
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
        }




        /*
        ----- END MATERIEL PART -----
        */



        /*
        ----- START INTERVENTION PART -----
        */


        public void showdataIntervention()
        {
            con.Open();
            adptIntervention = new SqlDataAdapter("Select i.ID_INTER as ID_INTER,i.Date_Inter as Date_Inter," +
                "i.Commentaire as Commentaire,i.Technicien as Technicien," +
                "m.Nom as Materiel from INTERVENTION i join MATERIEL m on i.ID_MATERIEL = m.ID_MATERIEL", con);
            dtIntervention = new DataTable();
            adptIntervention.Fill(dtIntervention);
            dataGridViewIntervention.DataSource = dtIntervention;
            con.Close();
        }

        private void dataGridViewIntervention_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }
            else
            {
                DataGridViewRow row = this.dataGridViewIntervention.Rows[e.RowIndex];
                if (row.Cells["ID_INTER"].Value.ToString() != "")
                {
                    idIntervention = Convert.ToInt32(row.Cells["ID_INTER"].Value.ToString());
                    textBoxCommentaire.Text = row.Cells["Commentaire"].Value.ToString();
                    textBoxTechnicien.Text = row.Cells["Technicien"].Value.ToString();
                    dateTimePickerInter.Value = DateTime.Parse(row.Cells["Date_Inter"].Value.ToString());
                    comboBoxInter.Text = row.Cells["Materiel"].Value.ToString();
                }
                else
                {
                    dateTimePickerInter.Value = DateTime.Now;
                    textBoxCommentaire.Text = "";
                    textBoxTechnicien.Text= "";
                    comboBoxInter.Text = "";
                }
            }
        }

        public void fillComboInter()
        {
            con.Open();
            cmd = new SqlCommand("SELECT * FROM MATERIEL", con);
            cmd.ExecuteNonQuery();

            DataTable dt = new DataTable();
            SqlDataAdapter dal = new SqlDataAdapter(cmd);

            dal.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                comboBoxInter.Items.Add(dr["Nom"].ToString());
            }
            con.Close();
        }

        private void buttonAjouterInter_Click(object sender, EventArgs e)
        {
            con.Open();
            SqlCommand check_materiel = new SqlCommand("SELECT COUNT(*) FROM INTERVENTION WHERE ID_MATERIEL = @ID_MATERIEL", con);
            check_materiel.Parameters.AddWithValue("@ID_MATERIEL", comboBoxInter.Text);
            int UserExist = (int)check_materiel.ExecuteScalar();
            con.Close();

            if (UserExist > 0)
            {
                MessageBox.Show("Cette intervention est déjà en cours");
            }
            else
            {

                if (comboBoxInter.Text != "" && textBoxCommentaire.Text != "" &&  textBoxCommentaire.Text != "" && textBoxTechnicien.Text != "")
                {
                    Intervention selectedClient = (Intervention)comboBoxMat.SelectedItem;
                    int idMateriel = selectedClient.ID;

                    con.Open();
                    cmd = new SqlCommand("insert into INTERVENTION(Date_Inter,Commentaire,Technicien,ID_MATERIEL) values" +
                        "('" +dateTimePickerInter.Value +
                        "','" + textBoxCommentaire.Text +
                        "','" + textBoxTechnicien.Text +
                        "','" + idMateriel + "')", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Intervention ajoutée");
                    con.Close();
                    showdataIntervention();
                }
                else
                {
                    MessageBox.Show("Veuillez remplir tous les champs");
                }
                
            }
        }
    }
    
}
