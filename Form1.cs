using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace İmalat
{
    public partial class Form1 : Form
    {

        SqlConnection connectionString = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);

        private DataTable dt1;
        private DataTable dt2;

        public Form1()
        {
            InitializeComponent();

            //Sadece rakam girilebilir.
            İsEmriNo.KeyPress += new KeyPressEventHandler(isEmriNo_KeyPress);
            İsEmriNo.KeyDown += İsEmriNo_KeyDown;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            FillIsEmriNo1ComboBox();

            dt1 = new DataTable();
            dt2 = new DataTable();
            
            //LoadData(); --> FORM YÜKLENDİĞİNDE İŞ EMRİ SEÇİLMEMİŞ OLUR BU YÜZDEN HENÜZ ÇAĞIRMAMALIYIZ
        }

        //---------TabPage1---------//

        #region İşEmriNo Combobox1
        private void FillIsEmriNo1ComboBox()
        {
            try
            {
                connectionString.Open();

                //SQL
                string query = "SELECT İsEmriNo FROM IsTB";

                SqlCommand command = new SqlCommand(query, connectionString);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    İsEmriNo.Items.Add(reader["İsEmriNo"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler akınırken hata oluştu (FşkkIsEmriNo1ComboBox()): " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
        }
        #endregion

        #region Tabloları Çek Butonu
        private void TabloGor_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        #endregion

        #region LoadData (Datagrid)
        private void LoadData()
        {
            try
            {
                string selectedIsEmriNo = İsEmriNo.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(selectedIsEmriNo))
                {
                    MessageBox.Show("Lütfen bir İş Emri No seçiniz.");
                    return;
                }

                connectionString.Open();

                // İlk DataGridView için veri çekme
                string queryImalatMalz = "SELECT * FROM ImalatMalz_GerceklesenTB WHERE IsEmriNo = @IsEmriNo";
                SqlCommand command1 = new SqlCommand(queryImalatMalz, connectionString);
                command1.Parameters.AddWithValue("@IsEmriNo", selectedIsEmriNo);
                SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                dt1.Clear();
                adapter1.Fill(dt1);
                ImalatMalzGrid.DataSource = dt1;

                // İkinci DataGridView için veri çekme
                string queryImalatSuresi = "SELECT * FROM Gerceklesen_Sure WHERE IsEmriNo = @IsEmriNo";
                SqlCommand command2 = new SqlCommand(queryImalatSuresi, connectionString);
                command2.Parameters.AddWithValue("@IsEmriNo", selectedIsEmriNo);
                SqlDataAdapter adapter2 = new SqlDataAdapter(command2);
                dt2.Clear();
                adapter2.Fill(dt2);
                ImalatSuresiGrid.DataSource = dt2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler alınırken hata oluştu (From1 LoadData()): " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
        }
        #endregion

        #region Sadece Rakam için keypress
        private void isEmriNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Sadece rakam giriniz.");
            }
        }
        #endregion

        #region Enter ile diğerlerine gelsin
        private void İsEmriNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                İsEmriNo2.Text = İsEmriNo.Text;
                İsEmriNo3.Text = İsEmriNo.Text;

                e.SuppressKeyPress = true;
            }
        }
        #endregion

        //---------TabPage2---------//

        #region BilgiGetirButonu
        private void BilgiGetir_Click(object sender, EventArgs e)
        {
            try
            {
                connectionString.Open();

                // ImalatSure_Onay --> Gerceklesen_Sure ' den ToplamSure
                string query1 = "SELECT ToplamSure FROM Gerceklesen_Sure WHERE IsEmriNo = @IsEmriNo";
                SqlCommand command1 = new SqlCommand(query1, connectionString);
                command1.Parameters.AddWithValue("@IsEmriNo", İsEmriNo.Text);
                object result1 = command1.ExecuteScalar();
                if (result1 != null)
                {
                    ImalatSure_Onay.Text = result1.ToString();
                }

                // GerceklesenKayitTarihi_Onay --> Ongoru_Sure ' den KayitTarihi
                string query2 = "SELECT CONVERT(VARCHAR(10), KayitTarihi, 104) FROM Ongoru_Sure WHERE IsEmriNo = @IsEmriNo"; //Bu sayede sadece tarihi getirdik.
                SqlCommand command2 = new SqlCommand(query2, connectionString);
                command2.Parameters.AddWithValue("@IsEmriNo", İsEmriNo.Text);
                object result2 = command2.ExecuteScalar();
                if (result2 != null)
                {
                    GerceklesenKayitTarihi_Onay.Text = result2.ToString();
                }

                // TeslimEden_Onay --> IsTB' de İsiTalepEden
                string query3 = "SELECT İsiTalepEden FROM IsTB WHERE İsEmriNo = @IsEmriNo";
                SqlCommand command3 = new SqlCommand(query3, connectionString);
                command3.Parameters.AddWithValue("@IsEmriNo", İsEmriNo.Text);
                object result3 = command3.ExecuteScalar();
                if (result3 != null)
                {
                    TeslimEden_Onay.Text = result3.ToString();
                }

                // TeslimAlan_Onay --> OnayTB'de İsTalepOnayiVeren
                string query4 = "SELECT İsTalepOnayiVeren FROM OnayTB WHERE İsEmriNo = @IsEmriNo";
                SqlCommand command4 = new SqlCommand(query4, connectionString);
                command4.Parameters.AddWithValue("@IsEmriNo", İsEmriNo.Text);
                object result4 = command4.ExecuteScalar();
                if (result4 != null)
                {
                    TeslimAlan_Onay.Text = result4.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler alınırken hata oluştu (BilgiGetir_Click): " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
        }
        #endregion

        //Onay Sekmesi için:
        #region Kaydet ve Tezmile
        private void KaydetBTN_Click(object sender, EventArgs e)
        {
            Kaydet();

            //Temizle:
            ImalatSure_Onay.Text = string.Empty;
            GerceklesenKayitTarihi_Onay.Text = string.Empty;
            TeslimEden_Onay.Text = string.Empty;
            TeslimAlan_Onay.Text = string.Empty;
            KontrolEden_Onay.Text = string.Empty;
            İsEmriNo2.Text = string.Empty;
        }
        #endregion

        #region Onay Kaydet
        private void Kaydet()
        {
            try
            {
                connectionString.Open();

                //SQL
                string query = @"INSERT INTO ImalatOnayTB   (İmalatSure,
                                                            İsinTeslimTarihi,
                                                            TeslimEden,
                                                            TeslimAlan,
                                                            KontrolEden,                                                                    
                                                            İsEmriNo,
                                                            KayitTarihi)
                                        VALUES (@İmalatSure,
                                                @İsinTeslimTarihi,
                                                @TeslimEden,
                                                @TeslimAlan,
                                                @KontrolEden,                                                                    
                                                @İsEmriNo,
                                                GETDATE())";

                SqlCommand cmd = new SqlCommand(query, connectionString);

                cmd.Parameters.AddWithValue("@İmalatSure", ImalatSure_Onay.Text);
                cmd.Parameters.AddWithValue("@İsinTeslimTarihi",GerceklesenKayitTarihi_Onay.Text);
                cmd.Parameters.AddWithValue("@TeslimEden",TeslimEden_Onay.Text);
                cmd.Parameters.AddWithValue("@TeslimAlan",TeslimAlan_Onay.Text);
                cmd.Parameters.AddWithValue("@KontrolEden",KontrolEden_Onay.Text);
                cmd.Parameters.AddWithValue("@İsEmriNo",İsEmriNo2.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanına kaydedilirken hata oluştu (Kaydet): " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
        }
        #endregion

        //---------TabPage3---------//

        //Final Sekmesi için:
        #region Kaydet
        private void KaydetFinalKontTxt_Click(object sender, EventArgs e)
        {
            Kaydet2();
            TemizleFinalKontrol();
        }
        #endregion

        #region Temizle
        private void TemizleFinalKontrol()
        {
            FinalKontrolYapan.Text = string.Empty;
            FinalOnayBtn.Checked = false;
            İsEmriNo3.Text = string.Empty;
            FinalOnayBtn.Checked = false;
            FinalNotOnayBtn.Checked = false;
        }
        #endregion

        #region Kaydet2 (Son Onay)
        private void Kaydet2()
        {
            try
            {
                connectionString.Open();

                //SQL
                string query = @"INSERT INTO FinalKontTB    (FinalKontrolYapan,
                                                            KayitTarihi,
                                                            İsEmriNo,
                                                            Onay)
                                        VALUES (@FinalKontrolYapan,
                                                GETDATE(),
                                                @İsEmriNo,
                                                @Onay)";

                SqlCommand cmd = new SqlCommand(query, connectionString);

                cmd.Parameters.AddWithValue("@FinalKontrolYapan", FinalKontrolYapan.Text);
                cmd.Parameters.AddWithValue("@İsEmriNo", İsEmriNo3.Text);
               
                if (FinalOnayBtn.Checked)
                    cmd.Parameters.AddWithValue("@Onay", "Var");
                else if (FinalNotOnayBtn.Checked)
                    cmd.Parameters.AddWithValue("@Onay", "Yok");
                else
                    cmd.Parameters.AddWithValue("@Onay", DBNull.Value); // NULL

                cmd.ExecuteNonQuery();
                MessageBox.Show("Veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanına kaydedilirken hata oluştu (Kaydet2): " + ex.Message);
            }
            finally
            {
                connectionString.Close();
            }

            RaporButonu.Visible = true;
        }
        #endregion


        //Kaydet butonuna bastığında gelen rapor butonu
        #region RaporButonu
        private void RaporButonu_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(İsEmriNo.Text))
            {
                Form2 form2 = new Form2(İsEmriNo.Text);
                form2.Show();
            }
            else
            {
                MessageBox.Show("Lütfen geçerli bir İş Emri No giriniz.");
            }
        }
        #endregion
    }
}
