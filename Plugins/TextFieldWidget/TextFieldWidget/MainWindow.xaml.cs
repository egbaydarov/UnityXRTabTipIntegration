using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace TextFieldWidget
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppMainWindow : Window
    {
        MemoryMappedViewStream TextFieldDataStream = null;
        public static bool IsSharedMomeryReachable = true;

        public static int SIZE = 1024;

        public AppMainWindow()
        {
            InitializeComponent();
            this.Top = 0;
            this.Left = 0;

            Thread textUpdate = new Thread(new ThreadStart(SharedMemorySetup));
            textUpdate.IsBackground = true;
            textUpdate.Start();
        }

        public void SharedMemorySetup()
        {
            int MMF_MAX_SIZE = SIZE;
            int MMF_VIEW_SIZE = SIZE;
            
            using (var mmf = MemoryMappedFile.OpenExisting("TextField_Widget"))
            using (var mmvStream = mmf.CreateViewStream(0, MMF_VIEW_SIZE))
            {
                TextFieldDataStream = mmvStream;
                while (IsSharedMomeryReachable);
            }

        }

        private void textField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextFieldDataStream != null)
            {
                TextWriter textWriter = new StreamWriter(TextFieldDataStream);
                textWriter.WriteLine(textField.Text);
                textWriter.Flush();
                TextFieldDataStream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}
