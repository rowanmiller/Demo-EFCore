using Microsoft.EntityFrameworkCore;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NoteTaker.Modern
{
    public sealed partial class NewNotePage : Page
    {
        public NewNotePage()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var note = new Note { Created = DateTime.Now, Text = Note.Text };

            using (var db = new NoteContext())
            {
                db.Notes.Add(note);
                db.SaveChanges();
            }

            Note.Text = string.Empty;
        }
    }
}
