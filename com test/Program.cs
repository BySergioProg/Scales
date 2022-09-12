using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
namespace com_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
            //Название листа (вкладки снизу)
            sheet.Name = "Имя должно быть не больше 32сим";
            //Пример заполнения ячеек №1
            for (int i = 1; i <= 9; i++)
            {
                for (int j = 1; j < 9; j++)
                    sheet.Cells[i, j] = String.Format("nookery {0} {1}", i, j);
            }
            //Пример №2
            sheet.Range["A1"].Value = "Пример №2";
            //Пример №3
            sheet.get_Range("A2").Value2 = "Пример №3";
        }
    }
}
