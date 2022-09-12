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
using Excel = Microsoft.Office.Interop.Excel;


namespace Scales
{
    /// <summary>
    /// Логика взаимодействия для GenerateReports.xaml
    /// </summary>
    public partial class GenerateReports : Window
    {
        List<Context.Car> cars=new List<Context.Car>();
        List<Context.Nomenclature> nomenclatures = new List<Context.Nomenclature>();
        List<Context.Shipment> shipments=new List<Context.Shipment>();
        public GenerateReports()
        {
            InitializeComponent();
            StartDate.Value = DateTime.Now.AddDays(-1);
            EndDate.Value = DateTime.Now;
            using (ApplicationContext context = new ApplicationContext())
            {
                cars=context.Cars.ToList();
                nomenclatures=context.Nomenclatures.ToList();
            }
            foreach( var car in cars )
            {
                CarNoms.Items.Add(car.CarNo);
            }
            if(CarNoms.Items.Count > 0) CarNoms.SelectedIndex = 0;
            foreach( var nom in nomenclatures )
            {
                Noms.Items.Add(nom.NomenclatureName);
            }
            if(Noms.Items.Count > 0) Noms.SelectedIndex = 0;
           
        }

        private void GetReport(object sender, RoutedEventArgs e)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                if (FullRep.IsChecked == true)
                {
                    shipments = context.Shipments.Where(p => p.ShipDateTime >= StartDate.Value && p.ShipDateTime <= EndDate.Value).OrderBy(p=>p.NomenclatureName).ToList();
                    //Объявляем приложение
                    Excel.Application app = new Excel.Application
                    {
                        //Отобразить Excel
                        Visible = true,
                        //Количество листов в рабочей книге
                        SheetsInNewWorkbook = 2
                    };
                    //Добавить рабочую книгу
                    Excel.Workbook workBook = app.Workbooks.Add(Type.Missing);
                    //Отключить отображение окон с сообщениями
                    app.DisplayAlerts = false;
                    //Получаем первый лист документа (счет начинается с 1)
                    Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);
                    Excel.Range range = sheet.get_Range("A1", "B1");
                    range.EntireColumn.ColumnWidth = 20;
                    Excel.Range range1 = sheet.get_Range("C1", "E1");
                    range1.EntireColumn.ColumnWidth = 10;
                    
                    sheet.Range[$"A1"].Value = $"Весовой пункт: {Properties.Settings.Default.ScalesName}.";
                    sheet.Range[$"A2"].Value = $"Отчет за период с {StartDate.Value} по {EndDate.Value}.";
                    sheet.Range[$"A3"].Value = $"Модель а/м";
                    sheet.Range[$"B3"].Value = $"Номер а/м";
                    sheet.Range[$"C3"].Value = $"Тара т.";
                    sheet.Range[$"D3"].Value = $"Брутто т.";
                    sheet.Range[$"E3"].Value = $"Нетто т.";
                    int i = 4;
                    int FlightNo = 0;
                    int TotalFlightNo = 0;
                    double TotalNetto = 0.0;
                    double NomenclatureNetto=0.0;
                    string CurrNomName = "";
                    foreach( var item in shipments )
                    {
                        if (CurrNomName != item.NomenclatureName)
                        {
                          
                            if (CurrNomName != "")
                            {
                                sheet.Range[$"A{i}"].Value = "Итого:";
                                sheet.Range[$"A{i}"].Font.Bold = true;
                                sheet.Range[$"B{i}"].Value = $"{FlightNo} рейсов";
                                sheet.Range[$"B{i}"].Font.Bold = true;
                                FlightNo = 0;
                                sheet.Range[$"E{i}"].Value = NomenclatureNetto;
                                sheet.Range[$"E{i}"].Font.Bold = true;
                                NomenclatureNetto = 0.0;
                                i++;
                            }
                            CurrNomName = item.NomenclatureName;
                            Excel.Range range3 = sheet.get_Range($"A{i}", $"E{i}");
                            range3.Merge(Type.Missing);
                            sheet.Range[$"A{i}"].Value = item.NomenclatureName;
                            sheet.Range[$"A{i}"].Font.Bold = true;
                            i++;
                        }
                        sheet.Range[$"A{i}"].Value = item.CarType;
                        sheet.Range[$"B{i}"].Value = item.CarNo;
                        sheet.Range[$"C{i}"].Value = item.CarWeight;
                        sheet.Range[$"D{i}"].Value = item.Brutto;
                        sheet.Range[$"E{i}"].Value = item.Netto;
                        FlightNo++;
                        TotalFlightNo++;
                        NomenclatureNetto+=item.Netto;
                        TotalNetto += item.Netto;
                        i++;
                    }
                    sheet.Range[$"A{i}"].Value = "Итого:";
                    sheet.Range[$"A{i}"].Font.Bold = true;
                    sheet.Range[$"B{i}"].Value = $"{FlightNo} рейсов";
                    sheet.Range[$"B{i}"].Font.Bold = true;
                    FlightNo = 0;
                    sheet.Range[$"E{i}"].Value = NomenclatureNetto;
                    sheet.Range[$"E{i}"].Font.Bold = true;
                    NomenclatureNetto = 0.0;
                    i++;
                    sheet.Range[$"A{i}"].Value = "Итого в сумме:";
                    sheet.Range[$"A{i}"].Font.Bold = true;
                    sheet.Range[$"B{i}"].Value = $"{TotalFlightNo} рейсов";
                    sheet.Range[$"B{i}"].Font.Bold = true;
                    FlightNo = 0;
                    sheet.Range[$"E{i}"].Value = TotalNetto;
                    sheet.Range[$"E{i}"].Font.Bold = true;
                    Excel.Range rangeColor = sheet.get_Range("A3", $"E{i}");
                    rangeColor.Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);
                }
                if(CarRep.IsChecked==true)
                {
                    if(CarNoms.Text!="")
                    {
                        shipments = context.Shipments.Where(p => p.ShipDateTime >= StartDate.Value && p.ShipDateTime <= EndDate.Value && p.CarNo == CarNoms.Text).ToList();
                        //Объявляем приложение
                        Excel.Application app = new Excel.Application
                        {
                            //Отобразить Excel
                            Visible = true,
                            //Количество листов в рабочей книге
                            SheetsInNewWorkbook = 2
                        };
                        //Добавить рабочую книгу
                        Excel.Workbook workBook = app.Workbooks.Add(Type.Missing);
                        //Отключить отображение окон с сообщениями
                        app.DisplayAlerts = false;
                        //Получаем первый лист документа (счет начинается с 1)
                        Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);
                        Excel.Range range = sheet.get_Range("A1", "B1");
                        range.EntireColumn.ColumnWidth = 20;
                        Excel.Range range1 = sheet.get_Range("C1", "E1");
                        range1.EntireColumn.ColumnWidth = 10;

                        sheet.Range[$"A1"].Value = $"Весовой пункт: {Properties.Settings.Default.ScalesName}.";
                        sheet.Range[$"A2"].Value = $"Отчет за период с {StartDate.Value} по {EndDate.Value}.";
                        sheet.Range[$"A3"].Value = $"Автомобиль № {CarNoms.Text}";
                        sheet.Range[$"A3"].Font.Bold = true;
                        sheet.Range[$"A4"].Value = $"Модель а/м";
                        sheet.Range[$"B4"].Value = $"Номер а/м";
                        sheet.Range[$"C4"].Value = $"Тара т.";
                        sheet.Range[$"D4"].Value = $"Брутто т.";
                        sheet.Range[$"E4"].Value = $"Нетто т.";
                        int i = 5;
                      
                        int TotalFlightNo = 0;
                        double TotalNetto = 0.0;

                        foreach(var item in shipments)
                        {
                            sheet.Range[$"A{i}"].Value = item.CarType;
                            sheet.Range[$"B{i}"].Value = item.CarNo;
                            sheet.Range[$"C{i}"].Value = item.CarWeight;
                            sheet.Range[$"D{i}"].Value = item.Brutto;
                            sheet.Range[$"E{i}"].Value = item.Netto;
                            TotalNetto += item.Netto;
                            TotalFlightNo += 1;
                            i++;

                        }
                        sheet.Range[$"A{i}"].Value = "Итого в сумме:";
                        sheet.Range[$"A{i}"].Font.Bold = true;
                        sheet.Range[$"B{i}"].Value = $"{TotalFlightNo} рейсов";
                        sheet.Range[$"B{i}"].Font.Bold = true;
                        sheet.Range[$"E{i}"].Value = TotalNetto;
                        sheet.Range[$"E{i}"].Font.Bold = true;
                        Excel.Range rangeColor = sheet.get_Range("A4", $"E{i}");
                        rangeColor.Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);



                    }
                }
                if(NomRep.IsChecked==true)
                {
                    if(Noms.Text!="")
                    {
                        shipments = context.Shipments.Where(p => p.ShipDateTime >= StartDate.Value && p.ShipDateTime <= EndDate.Value && p.NomenclatureName == Noms.Text).ToList();
                        Excel.Application app = new Excel.Application
                        {
                            //Отобразить Excel
                            Visible = true,
                            //Количество листов в рабочей книге
                            SheetsInNewWorkbook = 2
                        };
                        //Добавить рабочую книгу
                        Excel.Workbook workBook = app.Workbooks.Add(Type.Missing);
                        //Отключить отображение окон с сообщениями
                        app.DisplayAlerts = false;
                        //Получаем первый лист документа (счет начинается с 1)
                        Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);
                        Excel.Range range = sheet.get_Range("A1", "B1");
                        range.EntireColumn.ColumnWidth = 20;
                        Excel.Range range1 = sheet.get_Range("C1", "E1");
                        range1.EntireColumn.ColumnWidth = 10;

                        sheet.Range[$"A1"].Value = $"Весовой пункт: {Properties.Settings.Default.ScalesName}.";
                        sheet.Range[$"A2"].Value = $"Отчет за период с {StartDate.Value} по {EndDate.Value}.";
                        sheet.Range[$"A3"].Value = $"Наименование номенклатуры: {Noms.Text}";
                        sheet.Range[$"A3"].Font.Bold = true;
                        sheet.Range[$"A4"].Value = $"Модель а/м";
                        sheet.Range[$"B4"].Value = $"Номер а/м";
                        sheet.Range[$"C4"].Value = $"Тара т.";
                        sheet.Range[$"D4"].Value = $"Брутто т.";
                        sheet.Range[$"E4"].Value = $"Нетто т.";
                        int i = 5;

                        int TotalFlightNo = 0;
                        double TotalNetto = 0.0;

                        foreach (var item in shipments)
                        {
                            sheet.Range[$"A{i}"].Value = item.CarType;
                            sheet.Range[$"B{i}"].Value = item.CarNo;
                            sheet.Range[$"C{i}"].Value = item.CarWeight;
                            sheet.Range[$"D{i}"].Value = item.Brutto;
                            sheet.Range[$"E{i}"].Value = item.Netto;
                            TotalNetto += item.Netto;
                            TotalFlightNo += 1;
                            i++;

                        }
                        sheet.Range[$"A{i}"].Value = "Итого в сумме:";
                        sheet.Range[$"A{i}"].Font.Bold = true;
                        sheet.Range[$"B{i}"].Value = $"{TotalFlightNo} рейсов";
                        sheet.Range[$"B{i}"].Font.Bold = true;
                        sheet.Range[$"E{i}"].Value = TotalNetto;
                        sheet.Range[$"E{i}"].Font.Bold = true;
                        Excel.Range rangeColor = sheet.get_Range("A4", $"E{i}");
                        rangeColor.Borders.Color = ColorTranslator.ToOle(System.Drawing.Color.Black);

                    }
                }
            }
        }
    }
}
