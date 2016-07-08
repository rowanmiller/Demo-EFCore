using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NoteTaker.Classic
{
    public partial class MainForm : Form
    {
        private DbContextOptions _remoteDatabaseOptions;

        public MainForm()
        {
            InitializeComponent();

            var builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(ConfigurationManager.ConnectionStrings["RemoteDatabase"].ConnectionString);
            _remoteDatabaseOptions = builder.Options;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            using (var db = new NoteContext())
            {
                db.Database.EnsureCreated();
            }

            LoadExistingNotes();
        }

        private void save_Click(object sender, EventArgs e)
        {
            var note = new Note { Created = DateTime.Now, Text = this.note.Text };

            using (var db = new NoteContext())
            {
                db.Notes.Add(note);
                db.SaveChanges();
            }

            noteBindingSource.Insert(0, note);
            this.note.Text = string.Empty;
        }

        private void upload_Click(object sender, EventArgs e)
        {
            using (var localDb = new NoteContext())
            {
                // Find notes that are not uploaded
                var newNotes = localDb.Notes
                    .Where(n => !n.IsUploaded)
                    .ToList();

                // Upload to remote database
                using (var remoteDb = new NoteContext(_remoteDatabaseOptions))
                {
                    remoteDb.Database.EnsureCreated();
                    remoteDb.Notes.AddRange(newNotes);
                    remoteDb.SaveChanges();
                }

                // Mark as uploaded in local database
                newNotes.ForEach(n => n.IsUploaded = true);
                localDb.SaveChanges();

                MessageBox.Show($"Uploaded {newNotes.Count} notes.");
                LoadExistingNotes();
            }
        }

        private void LoadExistingNotes()
        {
            using (var db = new NoteContext())
            {
                noteBindingSource.DataSource = db.Notes
                    .OrderByDescending(n => n.Created)
                    .ToList();
            }
        }
    }
}
