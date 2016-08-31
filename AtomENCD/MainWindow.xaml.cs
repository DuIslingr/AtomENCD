using System;
using System.IO;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using BespokeFusion;
using Microsoft.Win32;

namespace AtomENCD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public bool _CanRun = true;

        private string _FilesFolderPath = AppDomain.CurrentDomain.BaseDirectory + "files";
        private string _PathAAEDLL { get { return Path.Combine(_FilesFolderPath, "AsyncAudioEncoder.dll"); } }
        private string _PathASDLL { get { return Path.Combine(_FilesFolderPath, "AudioStream.dll"); } }
        private string _PathCAEEXE { get { return Path.Combine(_FilesFolderPath, "criatomencd.exe"); } }
        private string _PathCAECONF { get { return Path.Combine(_FilesFolderPath, "criatomencd.exe.config"); } }
        private string _PathCAECDLL { get { return Path.Combine(_FilesFolderPath, "CriAtomEncoderComponent.dll"); } }
        private string _PathCAGDLL { get { return Path.Combine(_FilesFolderPath, "CriAtomGears.dll"); } }
        private string _PathCSRCDLL { get { return Path.Combine(_FilesFolderPath, "CriSamplingRateConverter.dll"); } }
        private string _PathHCENCDLL { get { return Path.Combine(_FilesFolderPath, "hcaenc.dll"); } }
        private string _PathHCENCLITEDLL { get { return Path.Combine(_FilesFolderPath, "hcaenc_lite.dll"); } }
        private string _PathVSTDLL { get { return Path.Combine(_FilesFolderPath, "vsthost.dll"); } }
        private string inputPath = "";
        public MainWindow()
        {
            InitializeComponent();

            string localUserPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            CheckFiles();
        }

        private void CheckFiles()
        {
            string pathToCAE = Path.Combine(_FilesFolderPath, _PathAAEDLL, _PathASDLL, _PathCAEEXE, _PathCAECONF, _PathCAECDLL, _PathCAGDLL, _PathCSRCDLL, _PathHCENCDLL, _PathHCENCLITEDLL, _PathVSTDLL);
            bool bNoCAEFiles = !File.Exists(pathToCAE);
            if (bNoCAEFiles)
            {
                var msg = new CustomMaterialMessageBox
                {
                    TxtMessage = { Text = "You are missing one or more of the following files.\nAsyncAudioEncoder.dll\nAudioStream.dll\ncriatomencd.exe\ncriatomencd.exe.config\nCriAtomEncoderComponent.dll\nCriAtomGears.dll\nCriSamplingRateConverter.dll\nhcaenc.dll\nhcaenc_lite.dll\nvsthost.dll\n \nRemember these files must be inside the files folder.", Foreground = Brushes.Black },
                    TxtTitle = { Text = "Error", Foreground = Brushes.White },
                    BtnOk = { Content = "Ok" },
                    MainContentControl = { Background = Brushes.White },
                    TitleBackgroundPanel = { Background = Brushes.Red },

                    BorderBrush = Brushes.Red
                };

                msg.Show();
                _CanRun = false;
                Environment.Exit(exitCode: 1);
            }
        }
        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OutputFilePath = new OpenFileDialog();
            OutputFilePath.Multiselect = true;
            OutputFilePath.Filter = "Audio Files (*.wav,*.wave,*.aiff,*.aif)|*.wav;*.wave;*.aiff;*.aif|All files (*.*)|*.*";
            OutputFilePath.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (OutputFilePath.ShowDialog() == true)

            {
                string OutputString = OutputFilePath.FileName;
                FileTextBox.Text = OutputString;
                inputPath = OutputFilePath.FileName;
            }
        }

        private void encodebutton_Click(object sender, RoutedEventArgs e)
        {
            if (inputPath != "")
            {
                var sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == true)
                {
                    string rate;
                    rate = ratebox.Text;
                    var p = new System.Diagnostics.Process();
                    p.StartInfo.FileName = _PathCAEEXE;
                    p.StartInfo.Arguments = " \"" + inputPath + "\"" + " \"" + sfd.FileName + "\"" + string.Format(getSelectedCodec(), getSelectedEncQlty(), getSelectedSmplQlty()) + " -rate=" + rate;
                    if ((bool)LoopEnable.IsChecked == true)
                    {
                        if ((bool)loopmodetoggle.IsChecked == true)
                        {
                            string startloop;
                            string endloop;
                            startloop = loopstartbox.Text;
                            endloop = loopendbox.Text;
                            p.StartInfo.Arguments +=  " -lps=" + startloop + " -lpe=" + endloop + " -nodelterm";
                        }
                        else
                        {
                            p.StartInfo.Arguments += " -lpa";
                        }
                    }
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                }
            }
            else
            {
                MaterialMessageBox.ShowError(@"Please select an audio file to encode.");
            }
        }

        public string getSelectedCodec()
        {
            string codec = "";
            if (adxradio.IsChecked == true)
            {     
                    codec= " -codec=ADX";
            }
            else if (hcaradio.IsChecked == true)
            {
                codec = " -codec=HCA";
            }
            else if (hcamxradio.IsChecked == true)
            {
                codec = " -codec=HCAMX";
            }
            return codec;
        }

        private string getSelectedEncQlty()
        {
            string encqlty = "";
            if (lowestencradio.IsChecked == true)
            {
                encqlty = " -encquality=LOWEST";
            }
            else if (lowencradio.IsChecked == true)
            {
                encqlty = " -encquality=LOW";
            }
            else if (middleencradio.IsChecked == true)
            {
                encqlty = " -encquality=MIDDLE";
            }
            else if (highencradio.IsChecked == true)
            {
                encqlty = " -encquality=HIGH";
            }
            else if (highestencradio.IsChecked == true)
            {
                encqlty = " -encquality=HIGHEST";
            }
            return encqlty;
        }

        private string getSelectedSmplQlty()
        {
            string smplqlty = "";
            if ((bool)sampletoggle.IsChecked == true)
            {
                smplqlty = " -quality=HIQUALITY";
            }
            else
            {
                smplqlty = " -quality=DEFAULT";
            }
            return smplqlty;
        }
    }
}
