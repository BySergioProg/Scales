using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Scales
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        MainWindow _mainWindow;
        public Settings(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            ScanPort.SelectedIndex=Properties.Settings.Default.ScannerPort;
            ScanPresent.IsChecked=Properties.Settings.Default.ScanPresent;
            ScalePresent.IsChecked=Properties.Settings.Default.ScalesPresent;
            ScalePort.SelectedIndex=Properties.Settings.Default.ScalesPort;
            ScalePortSpeed.SelectedIndex=Properties.Settings.Default.ScalesBauldRate;
            Cam1Enable.IsChecked=Properties.Settings.Default.Cam1Present;
            Cam2Enable.IsChecked=Properties.Settings.Default.Cam2Present;
            Cam1Ip.Text=Properties.Settings.Default.Cam1Ip;
            Cam2Ip.Text=Properties.Settings.Default.Cam2Ip;
            Cam1Login.Text=Properties.Settings.Default.Cam1Login;
            Cam2Login.Text= Properties.Settings.Default.Cam2Login;
            Cam1Pass.Text=Properties.Settings.Default.Cam1Pass;
            Cam2Pass.Text= Properties.Settings.Default.Cam2Pass;
            ScalesName.Text = Properties.Settings.Default.ScalesName;
        }

        private bool CheckIp(string Ip)
        {
          
            //Ip=Ip.Replace(" ", string.Empty);
            //Ip=Ip.Replace(",", ".");
            IPAddress IPaddr;
            if (!IPAddress.TryParse(Ip, out IPaddr))
            {
               // MessageBox.Show("Некорректнвй IP адрес");
                return false;
            }
            else
            {
                return true;
            }
        }
        private void SaveData(object sender, RoutedEventArgs e)
        {
            if ((CheckIp(Cam1Ip.Text)|| !(bool)Cam1Enable.IsChecked) && (CheckIp(Cam1Ip.Text)||!(bool)Cam2Enable.IsChecked))
            {
                if (ScanPort.SelectedIndex!=ScalePort.SelectedIndex)
                {
                    Properties.Settings.Default.ScannerPort=ScanPort.SelectedIndex;
                    Properties.Settings.Default.ScanPresent=(bool)ScanPresent.IsChecked;
                    Properties.Settings.Default.ScalesPresent=(bool)ScalePresent.IsChecked;
                    Properties.Settings.Default.ScalesPort=ScalePort.SelectedIndex;
                    Properties.Settings.Default.ScalesBauldRate=ScalePortSpeed.SelectedIndex;
                    Properties.Settings.Default.Cam1Present=(bool)Cam1Enable.IsChecked;
                    Properties.Settings.Default.Cam2Present=(bool)Cam2Enable.IsChecked;
                    Properties.Settings.Default.Cam1Ip=Cam1Ip.Text;
                    Properties.Settings.Default.Cam2Ip=Cam2Ip.Text;
                    Properties.Settings.Default.Cam1Login=Cam1Login.Text;
                    Properties.Settings.Default.Cam2Login=Cam2Login.Text;
                    Properties.Settings.Default.Cam1Pass=Cam1Pass.Text;
                    Properties.Settings.Default.Cam2Pass=Cam2Pass.Text;
                    Properties.Settings.Default.ScalesName=ScalesName.Text;
                    Properties.Settings.Default.Save();
                    _mainWindow.DataUpdate();
                }
                else
                {
                    MessageBox.Show("Устройства не могут иметь одинаковый порт!");
                }
            }
            else
            {
                MessageBox.Show("Не корректный IP адрес камеры!");
            }
        }
    }
}
