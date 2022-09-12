using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
    /// Логика взаимодействия для QrCar.xaml
    /// </summary>
    public partial class QrCar : Window
    {
        Bitmap _bitmap;
        public QrCar(Bitmap bitmap, string CarType, string CarNo, string CarWeight)
        {
            InitializeComponent();
            _bitmap = bitmap;
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                Qr.Source= bitmapimage;
                Type.Content=CarType;
                No.Content= CarNo;
                Massa.Content= CarWeight;

            }
        }

        private void ShowPrintDialog(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // Определить поля
                int pageMargin = 5;

                // Получить размер страницы
                System.Windows.Size pageSize = new System.Windows.Size(printDialog.PrintableAreaWidth - pageMargin * 2,
                    printDialog.PrintableAreaHeight - 20);

                // Инициировать установку размера элемента
                canva.Measure(pageSize);
                canva.Arrange(new Rect(pageMargin, pageMargin, pageSize.Width, pageSize.Height));
                printDialog.PrintVisual(canva, "Распечатываем элемент Canvas");
            }
        }
    }
}
