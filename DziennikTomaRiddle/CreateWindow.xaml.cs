using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Finisar.SQLite;

namespace DziennikTomaRiddle
{
    /// <summary>
    /// Logika interakcji dla klasy Tworzenie.xaml
    /// </summary>
    public partial class CreateWindow : Window
    {
        private readonly SQLiteAccess _sqliteAccess = new SQLiteAccess();

        public Note Note { get; set; } = new Note();
        public CreateWindow()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Date_chosen = Convert.ToDateTime(Calendar1.SelectedDate).ToString("yyyy-MM-dd");
                if (_sqliteAccess.Add(Note))
                {
                    this.Close();
                    MessageBox.Show("Dodano wpis!");
                }
                else
                    MessageBox.Show("Notatka z tego dnia już istnieje!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Calendar1_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Calendar1.DisplayDateEnd = DateTime.Today;
                var datesToBlackout = _sqliteAccess.DatesFromNotes();

                foreach (var date in datesToBlackout)
                {
                    if (date == DateTime.Today)
                        continue;
                    Calendar1.BlackoutDates.Add(new CalendarDateRange(date));

                }

                Calendar1.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
