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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;
using Finisar.SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DziennikTomaRiddle
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly SQLiteAccess _sqliteAccess = new SQLiteAccess();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool spellNotExist = true;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string SpellOnToday { get; set; }

        public MainWindow()
        {
            try
            {
                SpellOnToday = _sqliteAccess.GetSpellOnDay(DateTime.Now);

                if (spellNotExist = string.IsNullOrEmpty(SpellOnToday))
                    SpellOnToday = "Nie wylosowano żadnego zaklęcia na ten dzień.";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            this.DataContext = this;
            InitializeComponent();

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Calendar1.SelectedDate == null)
                    MessageBox.Show("Nie wybrano daty");

                var note = _sqliteAccess.LoadNote((DateTime)Calendar1.SelectedDate);
                if (note != null)
                {
                    NoteWindow noteWindow = new NoteWindow(note);
                    noteWindow.ShowDialog();
                    LoadDates();
                }
                else
                    MessageBox.Show("Brak notatki do wyświetlenia!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CreateWindow createWindow = new CreateWindow();
            createWindow.ShowDialog();
            LoadDates();
        }

        private void Rand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (spellNotExist)
                {
                    _sqliteAccess.RandSpell(Humor.Text);
                    SpellOnToday = _sqliteAccess.GetSpellOnDay(DateTime.Now);
                    spellNotExist = false;
                    OnPropertyChanged(nameof(SpellOnToday));
                }
                else
                    MessageBox.Show("Wylosowano już zaklęcie na ten dzień!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Calendar1_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDates();
        }

        private void LoadDates()
        {
            try
            {
                var datesToBlackout = _sqliteAccess.DatesFromNotes();
                Calendar1.BlackoutDates.Clear();
                Calendar1.DisplayDateStart = datesToBlackout.FirstOrDefault();
                Calendar1.DisplayDateEnd = datesToBlackout.LastOrDefault();
                for (int i = 0; i < datesToBlackout.Count - 1; i++)
                {
                    if (datesToBlackout[i] < datesToBlackout[i + 1].AddDays(-1))
                        Calendar1.BlackoutDates.Add(new CalendarDateRange(datesToBlackout[i].AddDays(1), datesToBlackout[i + 1].AddDays(-1)));
                }
                Calendar1.DisplayDate = datesToBlackout.LastOrDefault();
                Calendar1.SelectedDate = datesToBlackout.LastOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
