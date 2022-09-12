using QRCoder;
using Scales.Context;
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
    /// Логика взаимодействия для PrintInvoice.xaml
    /// </summary>
    public partial class PrintInvoice : Window
    {
        private int _Id;
        public PrintInvoice(int Id)
        {
            InitializeComponent();
            _Id = Id;
            using (ApplicationContext context = new ApplicationContext())
            {
                var dat =context.Shipments.Where(p=>p.Id == _Id).ToList();
                if (dat.Count > 0)
                {
                    CardNo1.Text = CardNo2.Text = CardNo3.Text = dat[0].Id.ToString();
                    Date1.Text = Date2.Text = Date3.Text = dat[0].ShipDateTime.ToString("D");
                    CarType1.Text = CarType2.Text = CarType3.Text = dat[0].CarType;
                    CarNo1.Text= CarNo2.Text = CarNo3.Text=dat[0].CarNo;
                    NomenclatureName1.Text=NomenclatureName2.Text=NomenclatureName3.Text = dat[0].NomenclatureName;
                    Brutto1.Text = Brutto2.Text = Brutto3.Text = dat[0].Brutto.ToString();
                    Tare1.Text= Tare2.Text = Tare3.Text=dat[0].CarWeight.ToString();
                    Netto1.Text= Netto2.Text = Netto3.Text = dat[0].Netto.ToString();

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode($"{dat[0].CarType};{dat[0].CarNo};{dat[0].CarWeight};{dat[0].ShipDateTime}; {dat[0].NomenclatureName}; {dat[0].Brutto}; {dat[0].Netto}", QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    using (MemoryStream memory = new MemoryStream())
                    {
                        qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                        memory.Position = 0;
                        BitmapImage bitmapimage = new BitmapImage();
                        bitmapimage.BeginInit();
                        bitmapimage.StreamSource = memory;
                        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapimage.EndInit();
                        Qr1.Source= Qr2.Source= Qr3.Source =bitmapimage;
 

                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {



                flowDoc.PageHeight = printDialog.PrintableAreaHeight;
                flowDoc.PageWidth = printDialog.PrintableAreaWidth;
                flowDoc.PagePadding = new Thickness(25);

                flowDoc.ColumnGap = 0;

                flowDoc.ColumnWidth = (flowDoc.PageWidth -
                                       flowDoc.ColumnGap -
                                       flowDoc.PagePadding.Left -
                                       flowDoc.PagePadding.Right);

                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDoc)
                                         .DocumentPaginator,
                                         "Task Manager Print Job");

            }
        }
    }
}
