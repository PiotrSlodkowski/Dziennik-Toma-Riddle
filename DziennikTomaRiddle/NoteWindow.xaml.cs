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
using System.IO;
using System.Globalization;
using Finisar.SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DziennikTomaRiddle
{
    /// <summary>
    /// Logika interakcji dla klasy Notatka.xaml
    /// </summary>
    /// 
    public partial class NoteWindow : Window, INotifyPropertyChanged
    {
        private readonly SQLiteAccess _sqliteAccess = new SQLiteAccess();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Note Note { get; set; }


        public NoteWindow(Note note)
        {
            this.Note = note;
            this.DataContext = this;
            InitializeComponent();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var datesFromNotes = _sqliteAccess.DatesFromNotes().OrderByDescending(x => x);
                DateTime date = datesFromNotes.FirstOrDefault(x => x < Note.Date);
                var previousNote = _sqliteAccess.LoadNote(date);
                if (previousNote == null)
                {
                    MessageBox.Show("Wyświetlony wpis jest najstarszy");
                    return;
                }

                Note = previousNote;
                OnPropertyChanged(nameof(Note));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var datesFromNotes = _sqliteAccess.DatesFromNotes().OrderBy(x => x);
                DateTime date = datesFromNotes.FirstOrDefault(x => x > Note.Date);
                var nextNote = _sqliteAccess.LoadNote(date);
                if (nextNote == null)
                {
                    MessageBox.Show("Wyświetlony wpis jest najnowszy");
                    return;
                }

                Note = nextNote;
                OnPropertyChanged(nameof(Note));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Change_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string comm = string.Format("UPDATE Note SET text = '{0}' WHERE date = '{1}'", Note.Text, Note.Date.ToString("yyyy-MM-dd"));
                _sqliteAccess.Update(comm);
                MessageBox.Show("Zaktualizowano wpis");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string comm = string.Format("DELETE FROM Note WHERE text = '{0}' AND date = '{1}'", Note.Text, Note.Date.ToString("yyyy-MM-dd"));
                _sqliteAccess.Update(comm);
                this.Close();
                MessageBox.Show("Usunięto wpis");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
