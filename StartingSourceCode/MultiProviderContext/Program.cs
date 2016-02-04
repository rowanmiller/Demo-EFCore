using Microsoft.Data.Entity;
using System;
using System.Linq;

namespace MultiProviderContext
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupDatabase();

            while (true)
            {
                Console.Write("Enter note text, 'sync', or 'exit': ");
                var input = Console.ReadLine();

                if (input == "exit")
                {
                    break;
                }
                else if (input == "sync")
                {
                    Sync();
                }
                else
                {
                    SaveLocal(input);
                }

                PrintData();
            }
        }

        public static void SaveLocal(string note)
        {
            using (var db = new NotesContext(useLocalDatabase: true))
            {
                db.Notes.Add(new Note { NoteText = note, NoteTaken = DateTime.Now });
                db.SaveChanges();
            }
        }

        public static void Sync()
        {
            using (var localDb = new NotesContext(useLocalDatabase: true))
            {
                using (var remoteDb = new NotesContext(useLocalDatabase: false))
                {
                    var pendingUploads = localDb.Notes.Where(n => !n.Uploaded).ToList();

                    foreach (var note in pendingUploads)
                    {
                        // TODO Add to remote context and mark as uploaded
                    }

                    // TODO Save all changes
                }
            }
        }

        public static void PrintData()
        {
            Console.WriteLine();
            Console.WriteLine("--------------- CURRENT STATE ---------------");
            using (var localDb = new NotesContext(useLocalDatabase: true))
            {
                Console.WriteLine("Local Notes:");
                foreach (var note in localDb.Notes.OrderBy(n => n.NoteTaken))
                {
                    Console.WriteLine($" - {note.NoteText} [Uploaded: {note.Uploaded}]");
                }
            }

            using (var remoteDb = new NotesContext(useLocalDatabase: false))
            {
                Console.WriteLine();
                Console.WriteLine("Remote Notes:");
                foreach (var note in remoteDb.Notes.OrderBy(n => n.NoteTaken))
                {
                    Console.WriteLine($" - {note.NoteText}");
                }
            }

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

        }

        private static void SetupDatabase()
        {
            using (var db = new NotesContext(useLocalDatabase: true))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            using (var db = new NotesContext(useLocalDatabase: false))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }
    }

    public class Note
    {
        public Guid NoteId { get; set; }
        public string NoteText { get; set; }
        public DateTime NoteTaken { get; set; }
        public bool Uploaded { get; set; }
    }

    public class NotesContext : DbContext
    {
        private bool _useLocalDatabase;

        public NotesContext(bool useLocalDatabase)
        {
            _useLocalDatabase = useLocalDatabase;
        }

        public DbSet<Note> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO Connect to SQLite when running in local mode
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Demo.MultiProviderContext;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO Perform any provider specific configuration
        }
    }
}
