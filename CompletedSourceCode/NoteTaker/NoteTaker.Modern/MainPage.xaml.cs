using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NoteTaker.Modern
{
    public sealed partial class MainPage : Page
    {
        private DbContextOptions _localDatabaseOptions;

        public MainPage()
        {
            InitializeComponent();

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite("Filename=Notes.db");
            _localDatabaseOptions = builder.Options;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new NoteContext(_localDatabaseOptions))
            {
                db.Database.EnsureCreated();
                Notes.ItemsSource = new ObservableCollection<Note>(db.Notes.ToList());
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var note = new Note { Created = DateTime.Now, Text = Note.Text };

            using (var db = new NoteContext(_localDatabaseOptions))
            {
                db.Notes.Add(note);
                db.SaveChanges();
            }

            ((ObservableCollection<Note>)Notes.ItemsSource).Insert(0, note);
            Note.Text = string.Empty;
        }
    }
}
