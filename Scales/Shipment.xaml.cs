using Scales.Context;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Scales
{
    /// <summary>
    /// Логика взаимодействия для Shipment.xaml
    /// </summary>
   
    public partial class Shipment : Window
    {
        public MainWindow _mainWindow;
        private DispatcherTimer ordersWorker = new DispatcherTimer();
        bool _Insert;
        int _Id;
        string _QrData;
        public Shipment(MainWindow mainWindow, bool Insert, int Id, string QrData)
        {
            InitializeComponent();
            CarTare.Text="0";
            FullWeight.Text="0";
            ProductWeight.Text="0";
            _mainWindow = mainWindow;
            _Insert = Insert;
            _Id = Id;
            _QrData = QrData;
            ordersWorker.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ordersWorker.Tick += WeightUpdate;
            ordersWorker.Start();
            DataUpdate();
        }
        public void DataUpdate()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if(!_Insert)
                {
                    foreach (var sh in context.Shipments.Where(x =>x.Id==_Id))
                    {
                        FullWeight.Text=sh.Brutto.ToString();
                        ProductWeight.Text=sh.Netto.ToString();
                        CarTare.Text=sh.CarWeight.ToString();
                        CarNo.Text=sh.CarNo;
                        CarModel.Text=sh.CarType;
                        Nom.Text=sh.NomenclatureName;
                    }
                }
                Nom.Items.Clear();
                CarModel.Items.Clear();
                foreach (var ct in context.CarTypes)
                {
                    CarModel.Items.Add(ct.Type);
                   
                }
                if (_Insert)
                {
                    CarModel.SelectedIndex = 0;
                   
                }
                CarUpdate();
                foreach (var v in context.Nomenclatures)
                {
                    Nom.Items.Add(v.NomenclatureName);
                   
                }
                if (_Insert)
                {
                    Nom.SelectedIndex = 0;
                  
                }
                if ((_Insert)&&(_QrData!=""))//Обработка при поступлении QR строки
                {
                    string[] QrString;
                    _QrData =_QrData.Remove(_QrData.Length-1);
                    QrString=_QrData.Split(';').ToArray();//Строку данных в массив
                    if (QrString.Length>0)
                    {
                        if (QrString[0]!="") CarModel.Text=QrString[0];
                    }
                    if (QrString.Length>1)
                    {
                        if (QrString[1]!="") CarNo.Text=QrString[1];
                    }
                    if (QrString.Length>2)
                    {
                        if (QrString[2]!="") CarTare.Text=QrString[2];
                    }
                    if (QrString.Length>4)
                    {
                        if (QrString[4]!="") Nom.Text=QrString[4];
                    }
                    if (QrString.Length>5)
                    {
                        if (QrString[5]!="") FullWeight.Text=QrString[5];
                    }
                    if (QrString.Length>6)
                    {
                        if (QrString[6]!="") ProductWeight.Text=QrString[6];
                    }
 
                }

            }
        }
        private void CarUpdate()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                CarNo.Items.Clear();
                string item = (string)CarModel.SelectedItem;
                foreach (var c in context.Cars.Where(x => x.CarType==item))
                {
                    CarNo.Items.Add(c.CarNo);

                }
                if (_Insert)
                {
                    CarNo.SelectedIndex = 0;
                }
            }
        }
        private void WeightUpdate(object sender, EventArgs e)//Обновление веса
        {
            CurrWeihgt.Content=$"Вес на весах {StaticData.Weight} т";
        }

        private void GetWeight(object sender, RoutedEventArgs e)
        {
            if (CarTare.Text!="")
            {
                double Tare = Convert.ToDouble(CarTare.Text);
                double Netto = 0;
                if (Tare<StaticData.Weight) { Netto = StaticData.Weight-Tare; }
               
                FullWeight.Text=StaticData.Weight.ToString();
                ProductWeight.Text=Netto.ToString();
            }
            else
            { MessageBox.Show("Отсутствует масса пустого автомобиля."); }
        }

        private void SaveData(object sender, RoutedEventArgs e)// Сохранение данных в БД
        {
            SaveToDb();
            
            this.Close();
        }
        private void SaveToDb()
        {
            if (_Insert)
            {
                if ((FullWeight.Text!="")&&(ProductWeight.Text!="")&&(CarTare.Text!="")&&(CarNo.Text!="")&&(CarModel.Text!="")&&(Nom.Text!=""))
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        var nom = context.Nomenclatures.Where(x => x.NomenclatureName==Nom.Text);
                        if (nom.Count()==0)
                        {
                            context.Nomenclatures.Add(new Context.Nomenclature { NomenclatureName=Nom.Text });
                        }
                        var car = context.Cars.Where(x => x.CarNo==CarNo.Text);
                        if (car.Count()==0)
                        {
                            double Tmp = 0;
                            if (double.TryParse(CarTare.Text, out Tmp))
                            {
                                context.Cars.Add(new Car { CarNo=CarNo.Text, CarType=CarModel.Text, CarWeight=Tmp });
                            }
                        }
                        var carType = context.CarTypes.Where(y => y.Type==CarModel.Text);
                        if (carType.Count()==0)
                        {
                            context.CarTypes.Add(new CarType { Type=CarModel.Text });
                        }
                        double Tare, Netto, Brutto = 0;
                        if (double.TryParse(FullWeight.Text, out Brutto))
                        {
                            if (double.TryParse(ProductWeight.Text, out Netto))
                            {
                                if (double.TryParse(CarTare.Text, out Tare))
                                {
                                    context.Shipments.Add(new Context.Shipment
                                    {

                                        ShipDateTime=DateTime.Now,
                                        Brutto=Brutto,
                                        Netto=Netto,
                                        CarWeight=Tare,
                                        CarNo=CarNo.Text,
                                        CarType=CarModel.Text,
                                        NomenclatureName=Nom.Text
                                    });
                                }
                            }
                        }
                        context.SaveChanges();
                    }
                }
                else
                { MessageBox.Show("Введены не все данные."); }
            }
            else
            {
                if ((FullWeight.Text!="")&&(ProductWeight.Text!="")&&(CarTare.Text!="")&&(CarNo.Text!="")&&(CarModel.Text!="")&&(Nom.Text!=""))
                {
                    using (ApplicationContext context = new ApplicationContext())
                    {
                        var nom = context.Nomenclatures.Where(x => x.NomenclatureName==Nom.Text);
                        if (nom.Count()==0)
                        {
                            context.Nomenclatures.Add(new Context.Nomenclature { NomenclatureName=Nom.Text });
                        }
                        var car = context.Cars.Where(x => x.CarNo==CarNo.Text);
                        if (car.Count()==0)
                        {
                            context.Cars.Add(new Car { CarNo=CarNo.Text, CarType=CarModel.Text });
                        }
                        var carType = context.CarTypes.Where(y => y.Type==CarModel.Text);
                        if (carType.Count()==0)
                        {
                            context.CarTypes.Add(new CarType { Type=CarModel.Text });
                        }
                        foreach (var sh in context.Shipments.Where(x => x.Id==_Id))
                        {
                            if (double.TryParse(FullWeight.Text, out double Brutto))
                            {
                                if (double.TryParse(ProductWeight.Text, out double Netto))
                                {
                                    if (double.TryParse(CarTare.Text, out double Tare))
                                    {
                                        sh.Brutto=Brutto;
                                        sh.Netto=Netto;
                                        sh.CarWeight=Tare;
                                        sh.CarNo=CarNo.Text;
                                        sh.CarType=CarModel.Text;
                                    }
                                }
                            }

                        }
                        //context.Shipments.Add(new Context.Shipment
                        //{
                        //    ShipDateTime=DateTime.Now,
                        //    Brutto=Convert.ToDouble(FullWeight.Text),
                        //    Netto=Convert.ToDouble(ProductWeight.Text),
                        //    CarWeight=Convert.ToDouble(CarTare.Text),
                        //    CarNo=CarNo.Text,
                        //    CarType=CarModel.Text,
                        //    NomenclatureName=Nom.Text
                        //});
                        context.SaveChanges();
                    }
                }
                else
                { MessageBox.Show("Введены не все данные."); }
            }
        }


        private void ChangeCarType(object sender, SelectionChangedEventArgs e)
        {
            CarUpdate();
        }

        private void SaveAndPrint(object sender, RoutedEventArgs e)// Сохранение данных в БД и печать накладной
        {
            SaveToDb();
            if (_Insert)
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    var dat=context.Shipments.ToArray();
                    int Count=dat.Count();
                    if (Count>0)
                    {
                        _Id=dat[Count-1].Id;
                    }
 
                }
            }

           PrintInvoice printInvoice = new PrintInvoice(_Id);
            printInvoice.Owner=_mainWindow;
            printInvoice.Show();
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _mainWindow.DataUpdate();
        }

        private void ControlDouble(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
           && (!CarTare.Text.Contains(",")
            && CarTare.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void FullWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
             && (!FullWeight.Text.Contains(",")
             && FullWeight.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void ProductWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
             && (!ProductWeight.Text.Contains(",")
             && ProductWeight.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void CgangeCarNo(object sender, SelectionChangedEventArgs e)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
               
                string item = (string)CarNo.SelectedItem;
                foreach (var c in context.Cars.Where(x => x.CarNo==item))
                {
                    CarTare.Text=c.CarWeight.ToString();

                }

            }
        }
    }
}
