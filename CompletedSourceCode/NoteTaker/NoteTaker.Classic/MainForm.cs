using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NoteTaker.Classic
{
    public partial class MainForm : Form
    {
        private DbContextOptions _localDatabaseOptions;

        public MainForm()
        {
            InitializeComponent();

            var localFile = Path.Combine(Application.UserAppDataPath, "Notes.db");
            var builder = new DbContextOptionsBuilder();
            builder.UseSqlite($"Filename={localFile}");
            _localDatabaseOptions = builder.Options;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            using (var db = new NoteContext(_localDatabaseOptions))
            {
                db.Database.EnsureCreated();
                notes.Items.AddRange(db.Notes.ToArray());
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            var note = new Note { Created = DateTime.Now, Text = this.note.Text };

            using (var db = new NoteContext(_localDatabaseOptions))
            {
                db.Notes.Add(note);
                db.SaveChanges();
            }

            notes.Items.Insert(0, note);
            this.note.Text = string.Empty;
        }
    }
}
