using System;
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

        public static EventWaitHandle TextChangedEvent;
        public static EventWaitHandle TextFieldClearEvent;
        public static MemoryMappedFile mmf;

        public static int SIZE = 1024;

        public AppMainWindow()
        {
            InitializeComponent();
            this.Top = 0;
            this.Left = 0;
            IPCsetup();
        }

        void IPCsetup()
        {
            TextChangedEvent = EventWaitHandle.OpenExisting("ewhTabTipKeyboard");
            TextFieldClearEvent = EventWaitHandle.OpenExisting("ewhTabTipClear");
            mmf = MemoryMappedFile.CreateOrOpen("hookTabTip", SIZE, MemoryMappedFileAccess.ReadWrite);
            TextFieldDataStream = mmf.CreateViewStream(0, SIZE);

            var thread = new Thread(() =>
            {
                while (true)
                {
                    TextFieldClearEvent.WaitOne();
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        textField.Text = ""; 

                    }));
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void textField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextFieldDataStream != null)
            {
                TextWriter textWriter = new StreamWriter(TextFieldDataStream);
                textWriter.WriteLine(textField.Text);
                textWriter.Flush();
                TextFieldDataStream.Seek(0, SeekOrigin.Begin);
                TextChangedEvent.Set();
            }
        }
    }
}
