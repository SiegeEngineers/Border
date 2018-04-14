using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Border.View
{
    public partial class Update : Window
    {
        public string Message { get; set; }
        public string Link { get; set; }
        public Update(string message, string link)
        {
            Message = message;
            Link = link;
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
