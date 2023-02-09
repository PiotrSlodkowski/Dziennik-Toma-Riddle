using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace DziennikTomaRiddle
{
    public class Note
    {
        public DateTime Date { get; set; } = DateTime.Now;
        public string Text { get; set; } = string.Empty;
        public string Spell { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;


    }
}
