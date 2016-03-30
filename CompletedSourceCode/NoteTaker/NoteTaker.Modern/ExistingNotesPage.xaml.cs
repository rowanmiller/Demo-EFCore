using System.Linq;
using Windows.UI.Xaml.Controls;

namespace NoteTaker.Modern
{
    public sealed partial class ExistingNotesPage : Page
    {
        public ExistingNotesPage()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            using (var db = new NoteContext())
            {
                Notes.ItemsSource = db.Notes
                    .OrderByDescending(n => n.Created)
                    .ToList();
            }
        }
    }
}
