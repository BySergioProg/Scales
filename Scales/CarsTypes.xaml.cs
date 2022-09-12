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

namespace Scales
{
    /// <summary>
    /// Логика взаимодействия для CarsTypes.xaml
    /// </summary>
    public partial class CarsTypes : Window
    {
        List<CarType> carTypes = new List<CarType>();
        public Cars mainWindow;
        public CarsTypes(Cars _mainWindow)
        {
            InitializeComponent();
            mainWindow = _mainWindow;
            DataUpdate();
        }
        private void DataUpdate()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                carTypes=context.CarTypes.ToList();
            }
            TypesGrid.ItemsSource = carTypes;
            mainWindow.DataUpdate();
        }
        private void AddType(object sender, RoutedEventArgs e)
        {
            if(Type.Text!="")
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    bool Present = false;
                    foreach (var car in carTypes)
                    {
                        if (car.Type==Type.Text) { Present = true; break; }
                    }
                    if (!Present)
                    {
                        context.CarTypes.Add(new CarType { Type = Type.Text });
                        context.SaveChanges();
                    }
                }
                DataUpdate();
            }
        }

        private void DeleteType(object sender, RoutedEventArgs e)
        {
            if (Type.Text!="")
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    foreach(var types in context.CarTypes)
                    {
                        if(types.Type==Type.Text)
                        {
                            context.CarTypes.Remove(types);
                        }
                    }
                    context.SaveChanges();
                }
                DataUpdate();
            }
        }

        private void SelectType(object sender, MouseButtonEventArgs e)
        {
            CarType cars = TypesGrid.SelectedItem as CarType;
            if (cars!=null)
            {
                Type.Text=cars.Type;
            }
        }
    }
}
