using Windows.UI.Xaml.Controls;

namespace NoteTaker.Modern
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var page = ((PivotItem)((Pivot)sender).SelectedItem).Content as ExistingNotesPage;
            if(page != null)
            {
                page.Refresh();
            }
        }
    }
}
