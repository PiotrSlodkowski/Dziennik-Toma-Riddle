using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using Finisar.SQLite;

namespace DziennikTomaRiddle
{

    public class SQLiteAccess
    {
        private readonly SQLiteConnection _sqliteConn = new SQLiteConnection("Data Source=Note.db;Version=3;");

        public Note LoadNote(DateTime date)
        {
            _sqliteConn.Open();
            Note note = null;
            var sqliteCmd = _sqliteConn.CreateCommand();
            sqliteCmd.CommandText = string.Format("SELECT Note.*, Spell.spell FROM Note LEFT JOIN SpellOfDay ON Note.date = SpellOfDay.date LEFT JOIN Spell ON Spell.id = SpellOfDay.id_spell WHERE Note.date = '{0}'", date.ToString("yyyy-MM-dd"));
            sqliteCmd.ExecuteNonQuery();
            var sqliteDataReader = sqliteCmd.ExecuteReader();
            if (sqliteDataReader.Read())
            {
                var text = sqliteDataReader["text"].ToString();
                note = new Note() { Date = date, Text = text };

                var spell = sqliteDataReader["spell"].ToString();
                if (!string.IsNullOrEmpty(spell))
                    note.Spell = spell;
            }

            _sqliteConn.Close();
            return note;
        }

        public List<DateTime> DatesFromNotes()
        {
            _sqliteConn.Open();
            List<DateTime> dates = new List<DateTime>();
            var sqliteCmd = _sqliteConn.CreateCommand();
            sqliteCmd.CommandText = "SELECT date FROM Note ORDER BY date ASC";
            var sqliteDataReader = sqliteCmd.ExecuteReader();
            while (sqliteDataReader.Read())
            {
                var date = DateTime.Parse(sqliteDataReader["date"].ToString());
                dates.Add(date);
            }
            _sqliteConn.Close();
            return dates;
        }

        public string GetSpellOnDay(DateTime date)
        {
            _sqliteConn.Open();
            var sqliteCmd = _sqliteConn.CreateCommand();
            sqliteCmd.CommandText = string.Format("SELECT spell FROM SpellOfDay JOIN Spell ON SpellOfDay.id_spell = Spell.id WHERE date = '{0}'", date.ToString("yyyy-MM-dd"));
            var sqliteDataReader = sqliteCmd.ExecuteReader();
            string spell = string.Empty;
            while (sqliteDataReader.Read())
            {
                spell = sqliteDataReader["spell"].ToString();
            }
            _sqliteConn.Close();

            return spell;
        }

        public void Update(string comm)
        {
            _sqliteConn.Open();
            var sqlitecmd = _sqliteConn.CreateCommand();
            sqlitecmd.CommandText = comm;
            sqlitecmd.ExecuteNonQuery();
            _sqliteConn.Close();
        }

        public bool Add(Note note)
        {
            bool success = false;
            _sqliteConn.Open();
            var sqliteCmd = _sqliteConn.CreateCommand();
            sqliteCmd.CommandText = string.Format("SELECT date FROM Note WHERE date = '{0}'", note.Date.ToString("yyyy-MM-dd"));
            sqliteCmd.ExecuteNonQuery();
            var sqliteDataReader = sqliteCmd.ExecuteReader();
            if (!sqliteDataReader.Read())
            {
                sqliteCmd = _sqliteConn.CreateCommand();
                sqliteCmd.CommandText = string.Format("INSERT INTO Note (date, text) VALUES ('{0}', '{1}')", note.Date.ToString("yyyy-MM-dd"), note.Text);
                sqliteCmd.ExecuteNonQuery();
                success = true;
            }
            _sqliteConn.Close();
            return success;
        }

        public void RandSpell(string humor)
        {
            _sqliteConn.Open();
            var sqliteCmd = _sqliteConn.CreateCommand();
            sqliteCmd.CommandText = string.Format("SELECT id FROM Spell WHERE humor = '{0}' ORDER BY RANDOM() LIMIT 1", humor);
            var sqliteDataReader = sqliteCmd.ExecuteReader();
            string spellId = string.Empty;
            while (sqliteDataReader.Read())
            {
                spellId = sqliteDataReader["id"].ToString();
            }
            sqliteCmd = _sqliteConn.CreateCommand();
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            sqliteCmd.CommandText = string.Format("INSERT INTO SpellOfDay (date, humor, id_spell) VALUES ('{0}', {1}, '{2}')", date, Int32.Parse(humor), spellId);
            sqliteCmd.ExecuteNonQuery();
            _sqliteConn.Close();

        }

    }
}
