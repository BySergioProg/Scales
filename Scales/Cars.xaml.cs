using QRCoder;
using Scales.Context;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Логика взаимодействия для Cars.xaml
    /// </summary>
    public partial class Cars : Window
    {
        List<CarType> carTypes = new List<CarType>();
        List<Context.Car> cars = new List<Context.Car>();
        private DispatcherTimer ordersWorker = new DispatcherTimer();
        public Cars()
        {
            InitializeComponent();
            ordersWorker.Interval = new TimeSpan(0, 0, 0, 1, 0);
            ordersWorker.Tick += WeightUpdate;
            ordersWorker.Start();
            DataUpdate();
        }
        private void WeightUpdate(object sender, EventArgs e)//Обновление веса
        {
            CarW.Content= StaticData.Weight;
        }
        public void DataUpdate()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                carTypes=context.CarTypes.ToList();
                cars=context.Cars.ToList();
                Types.Items.Clear();
                foreach (var v in carTypes)
                {
                    Types.Items.Add(v.Type);
                }
               
                Types.SelectedIndex = 0;
                CarList.ItemsSource=context.Cars.ToList();
            }
        }
        private void ChangeCarTypes(object sender, RoutedEventArgs e)
        {
            CarsTypes carsTypes = new CarsTypes(this);
            carsTypes.Owner = this;
            carsTypes.Show();
        }

        private void SelectCar(object sender, MouseButtonEventArgs e)
        {
            Car cars = CarList.SelectedItem as Car;
            if (cars!=null)
            {
                Types.Text=cars.CarType;
                CarNo.Text=cars.CarNo;
                CarWeight.Text=Convert.ToString(cars.CarWeight);
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            if ((Types.Text!="")&&(CarNo.Text!="")&&(CarWeight.Text!=""))
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    foreach (var types in context.Cars)
                    {
                        if (types.CarNo==CarNo.Text)
                        {
                            context.Cars.Remove(types);
                        }
                    }
                    context.SaveChanges();
                }
                DataUpdate();
            }
        }

        private void AddRecord(object sender, RoutedEventArgs e)
        {
            if ((Types.Text!="")&&(CarNo.Text!="")&&(CarWeight.Text!=""))
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    bool Present = false;
                    foreach (var car in context.Cars)
                    {
                        if (car.CarNo==CarNo.Text) {
                            Present = true;
                            car.CarWeight=Convert.ToDouble(CarWeight.Text);
                            break; }
                    }
                    if (!Present)
                    {
                        double Tmp = 0;
                        if (double.TryParse(CarWeight.Text, out Tmp))
                        {
                            context.Cars.Add(new Car { CarType =Types.Text, CarNo=CarNo.Text, CarWeight=Tmp });
                        }
                        else
                        { MessageBox.Show("Ошибка преобразования массы автомобиля!"); }
                    }
                    context.SaveChanges();
                }
                DataUpdate();
            }
        }

        private void ControlDouble(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ",")
            && (!CarWeight.Text.Contains(",")
            && CarWeight.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }

        private void SetWeightFromScales(object sender, RoutedEventArgs e)
        {
            CarWeight.Text=Convert.ToString(StaticData.Weight);
        }

        private void GenerateQr(object sender, RoutedEventArgs e)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"{Types.Text};{CarNo.Text};{CarWeight.Text}", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            QrCar qrCar = new QrCar(qrCodeImage, Types.Text, CarNo.Text, CarWeight.Text);
            qrCar.Owner=this;
            qrCar.Show();
        }
    }
}
