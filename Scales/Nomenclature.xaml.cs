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
    /// Логика взаимодействия для Nomenclature.xaml
    /// </summary>
    public partial class Nomenclature : Window
    {
        List<Context.Nomenclature> nomenclatures = new List<Context.Nomenclature>();
        public Nomenclature()
        {
            InitializeComponent();
            DataUpdate();
        }
        private void DataUpdate()
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                nomenclatures=context.Nomenclatures.ToList();
            }
            TypesGrid.ItemsSource = nomenclatures;
           
        }
        private void SelectType(object sender, MouseButtonEventArgs e)
        {
            Context.Nomenclature nom = TypesGrid.SelectedItem as Context.Nomenclature;
            if (nom!=null)
            {
                Type.Text=nom.NomenclatureName;
            }
        }

        private void AddType(object sender, RoutedEventArgs e)
        {
            if (Type.Text!="")
            {
                using (ApplicationContext context = new ApplicationContext())
                {
                    bool Present = false;
                    foreach (var nom in context.Nomenclatures)
                    {
                        if (nom.NomenclatureName==Type.Text) { Present = true; break; }
                    }
                    if (!Present)
                    {
                        context.Nomenclatures.Add(new Context.Nomenclature { NomenclatureName= Type.Text });
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
                    foreach (var types in context.Nomenclatures)
                    {
                        if (types.NomenclatureName==Type.Text)
                        {
                            context.Nomenclatures.Remove(types);
                        }
                    }
                    context.SaveChanges();
                }
                DataUpdate();
            }
        }
    }
}
