using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accord.Video;
using Accord.Video.FFMPEG;
using System.Threading;
using System.Windows.Threading;
using Scales.Context;
using System.IO.Ports;

namespace Scales
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public class StaticData
    {
        public static double Weight;
    }
    public partial class MainWindow : Window
    {

        Thread thread;
        Thread thread2;
        private static Bitmap ImageCam1;
        SerialPort serial1;
        SerialPort serial2;
        private DispatcherTimer ordersWorker = new DispatcherTimer();
        VideoFileSource videoSource;
        VideoFileSource videoSource2;
        bool Cam1Present;
        bool Cam2Present;
        string Cam1Ip;
        string Cam2Ip;
        string Cam1Login;
        string Cam2Login;
        string Cam1Pass;
        string Cam2Pass;

        public MainWindow()
        {
            InitializeComponent();
            days1.SelectedIndex = 0;
            StaticData.Weight=0.0;
            ordersWorker.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ordersWorker.Tick += WeightUpdate;
            ordersWorker.Start();
            DataUpdate();
            StartReadQr();
            StartReadScales();

         
           
        }
        private void StartVideo()
        {
            if ((Cam1Present)&&(Cam1Login!="")&&(Cam1Pass!="")&&(Cam1Ip!=""))
            {
                videoSource = new VideoFileSource($@"rtsp://{Cam1Login}:{Cam1Pass}@{Cam1Ip}/Streaming/Channels/102");
                videoSource.NewFrame+=new NewFrameEventHandler(video_NewFrame);

                videoSource.Start();
            }
            if ((Cam2Present)&&(Cam2Login!="")&&(Cam2Pass!="")&&(Cam2Ip!=""))
            {
                videoSource2 = new VideoFileSource($@"rtsp://{Cam2Login}:{Cam2Pass}@{Cam2Ip}/Streaming/Channels/102");
                videoSource2.NewFrame+=new NewFrameEventHandler(video_NewFrame1);

                videoSource2.Start();
            }
        }
        private void StartReadQr()
        {
            if (Properties.Settings.Default.ScanPresent)
            {
                thread = new Thread(() => ReadQr());
                thread.Start();
            }
        }
        private void StartReadScales()
        {
            if (Properties.Settings.Default.ScalesPresent)
            {
                thread2 = new Thread(() => ReadScales());
                thread2.Start();
            }
        }
        private async void ReadScales()
        {
            try
            {
                string Data = "";

                while (true)
                {
                    
                    try
                    {
                        if (SerialPort.GetPortNames().Contains(serial2.PortName))
                        {
                            if (!serial2.IsOpen) serial2.Open();
                            // Data = serial2.ReadExisting();
                            Data = serial2.ReadTo("+");

                        }

                        if (Data.Length>=7)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                double Dat = 0.0;
                                //var Exit =Data.Split('+').ToArray();
                                //string y = Exit[ Exit.Count()-2];
                                string Plus = Data.Substring(0,8);
    
                                    Dat=Convert.ToDouble(Plus.Substring(1,6));
                                    int Stepen=Convert.ToInt32(Plus.Substring(7,1));
                                    if (Stepen==1) Dat=Dat/10.0;
                                    if (Stepen==2) Dat=Dat/100.0;
                                    if (Stepen==3) Dat=Dat/1000.0;
                                    if (Stepen==4) Dat=Dat/10000.0;
                                
                                StaticData.Weight=Dat/1000.0;
                                
                            });

                        }
                        else
                        {
                            StaticData.Weight=0.0;
                        }
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (ThreadAbortException)
            { }

        }
        private void ReadQr()
        {
            try
            {
                string Data = "";

                while (true)
                {
                    try
                    {
                        if (SerialPort.GetPortNames().Contains(serial1.PortName))
                        {
                           
                            if (!serial1.IsOpen) serial1.Open();
                            Data = serial1.ReadExisting();
                           // serial1.Dispose();
                        }
                      

                        if (Data.Length>0)
                        {

                            //  shipment.Show();
                            Dispatcher.Invoke(() =>
                            {
                                Shipment shipment = new Shipment(this, true, 0, Data);
                                shipment.Owner=this;
                                shipment.Show();
                            });

                        }
                        
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch(ThreadAbortException)
            { }
           
        }
        //private string SerialRead(SerialPort serialPort)
        //{
        //    try
        //    {
        //        if (SerialPort.GetPortNames().Contains(serialPort.PortName))
        //        {
        //            if (!serialPort.IsOpen) serialPort.Open();
        //            return serialPort.ReadLine();
        //        }
        //        return "";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }

        //}
        List<Context.Shipment> shipments = new List<Context.Shipment>();
        public void DataUpdate()
        {
            if (videoSource!=null) { if(videoSource.IsRunning) videoSource.Stop(); }
            if (videoSource2!=null) { if(videoSource2.IsRunning) videoSource2.Stop(); }
            shipments.Clear();
            using (ApplicationContext context = new ApplicationContext())
            {
                var cv = days1.SelectedItem as ListBoxItem;
                
                DateTime dat = DateTime.Now.AddDays(Convert.ToInt32(cv.Content.ToString()) *(-1));
                shipments=context.Shipments.Where(p => p.ShipDateTime>=dat).OrderByDescending(n => n.ShipDateTime).ToList();
            }
            Journal.ItemsSource = shipments;
            if (serial1 != null)
            {
                serial1.Close();
                serial1.Dispose();
            }
                serial1 = new SerialPort
                {
                    PortName = $"COM{Properties.Settings.Default.ScannerPort}",
                    ReadTimeout = 500,
                    Handshake = Handshake.RequestToSend,
                    Encoding = Encoding.UTF8

                };

            if (serial2 != null)
            {
                serial2.Close();
                serial2.Dispose();
            }
                serial2 = new SerialPort
                {
                    PortName = $"COM{Properties.Settings.Default.ScalesPort}",
                    BaudRate = (int)(600 * Math.Pow(2, Properties.Settings.Default.ScalesBauldRate)),
                    ReadTimeout = 1500,
                    Encoding = Encoding.UTF8

                };
            
            Cam1Present=Properties.Settings.Default.Cam1Present;
            Cam2Present=Properties.Settings.Default.Cam2Present;
            Cam2Ip=Properties.Settings.Default.Cam2Ip;
            Cam1Ip=Properties.Settings.Default.Cam1Ip;
            Cam1Login=Properties.Settings.Default.Cam1Login;
            Cam2Login=Properties.Settings.Default.Cam2Login;
            Cam1Pass=Properties.Settings.Default.Cam1Pass;
            Cam2Pass=Properties.Settings.Default.Cam2Pass;
            StartVideo();
        }
        private void WeightUpdate(object sender, EventArgs e)//Обновление веса
        {
            CurrDataFromScales.Content=$"{StaticData.Weight} т.";
        }
            private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
                Bitmap bitmap = eventArgs.Frame;
                Dispatcher.Invoke(() => Cam1.Source = BitmapToImageSource(bitmap));
        }
        private void video_NewFrame1(object sender, NewFrameEventArgs eventArgs)
        {
                 Bitmap bitmap = eventArgs.Frame;
                Dispatcher.Invoke(() => Cam2.Source = BitmapToImageSource(bitmap));
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        private void CarsEdit(object sender, RoutedEventArgs e)
        {
            Cars cars = new Cars();
            cars.Owner=this;
            cars.Show();
        }

        private void NomenclatureEdit(object sender, RoutedEventArgs e)
        {
            Nomenclature nomenclature = new Nomenclature();
            nomenclature.Owner=this;        
            nomenclature.Show();
        }

        private void ProgramStop(object sender, EventArgs e)
        {
            try
            {
                ordersWorker.Stop();
                if (videoSource!=null)
                {
                    if (videoSource.IsRunning) videoSource.Stop();
                }
                if (videoSource2!=null)
                {
                    if (videoSource2.IsRunning) videoSource2.Stop();
                }
                if(thread!=null)  thread.Abort();
               if(thread2!=null) thread2.Abort();
            }
            catch (Exception ex) { }
        }

        private void ShipmentShow(object sender, RoutedEventArgs e)
        {
            Shipment shipment = new Shipment(this, true, 0,"");
            shipment.Owner=this;
            shipment.Show();
        }

        private void Days_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataUpdate();
        }

        private void SelectData(object sender, MouseButtonEventArgs e)
        {

            int _Id = 0;
            Context.Shipment journal = Journal.SelectedItem as Context.Shipment;
            if (journal!=null)
            {
                Shipment shipment = new Shipment(this, false, journal.Id, "");
                shipment.Owner=this;
                shipment.Show();
                //PrintInvoice printInvoice = new PrintInvoice(journal.Id);
                //printInvoice.Owner=this;
                //printInvoice.Show();
            }
        }


        private void SetParams(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Owner = this;
            settings.Show();
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if (DeletingId.Text!="")
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    int i = Convert.ToInt32(DeletingId.Text);
                    foreach (var d in context.Shipments.Where(p => p.Id==i))
                    {
                        context.Shipments.Remove(d);
                    }
                    context.SaveChanges();
                    DataUpdate();
                }
            }
        }

        private void CheckInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void ReportsShow(object sender, RoutedEventArgs e)
        {
            GenerateReports generateReports = new GenerateReports();
            generateReports.Owner = this;
            generateReports.Show();
        }
    }
}
