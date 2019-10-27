using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp3
{
 
    public partial class MainWindow : Window
    {
        string str = "server=localhost,1501;database=TestDb;user id=sa;password=**ukJ7854@*!;";
        public MainWindow()
        {
            InitializeComponent();

            // service broker altında service ve queue oluşturur.
            // stop ta bunları kaldırır
            SqlDependency.Start(str);

            

            verileriYukle();

            this.Closing += MainWindow_Closing;
 
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SqlDependency.Stop(str);
        }

        void verileriYukle()
        {
            var con = new SqlConnection(str);
            con.Open();

            var command = new SqlCommand("select Id,Ad from dbo.Kimlik", con);

            var dependency = new SqlDependency(command);

           

            dependency.OnChange -= new OnChangeEventHandler(OnDependencyChange);
            dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

            command.ExecuteReader();
        }


     
        private void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            var sonuc = e.Source.ToString()+ " " + e.Info.ToString() + " ";

            DispatcherOperation op = Dispatcher.BeginInvoke((Action)(() => {
                l1.Content = "çalıştı " + sonuc +  DateTime.Now;

                verileriYukle();
            }));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlDependency.Stop(str);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var con = new SqlConnection(str);
            con.Open();

            var command = new SqlCommand($"update dbo.Kimlik set Ad='{txtAd.Text}' where Id=1", con);

            int sonuc=command.ExecuteNonQuery();

            if(sonuc>0)
            {
                MessageBox.Show("1 nolu Id kayıt güncellendi");
                verileriYukle();
            }


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var con = new SqlConnection(str);
            con.Open();

            var command = new SqlCommand($"insert into dbo.Kimlik (Ad) values ('{txtAd.Text}')", con);

            int sonuc = command.ExecuteNonQuery();

            if (sonuc > 0)
            {
                MessageBox.Show("kayıt eklendi");
                verileriYukle();
            }

        }
    }
}
