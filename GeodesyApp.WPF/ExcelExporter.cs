using System;
using System.Collections.Generic;
using System.Text;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;

namespace GeodesyApp
{
    public class ExcelExporter
    {
        // Метод 1: Создаёт workbook и возвращает его
        public XLWorkbook CreateExcelWorkbook(List<Scheme> schemes)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Реестр схем");

            // Заголовки
            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Название";
            worksheet.Cell(1, 3).Value = "Оси";
            worksheet.Cell(1, 4).Value = "Отметка";
            worksheet.Cell(1, 5).Value = "Объём, м³";
            worksheet.Cell(1, 6).Value = "Статус";

            // Данные
            int row = 2;
            foreach(var scheme in schemes)
            {
                worksheet.Cell(row, 1).Value = scheme.Id;
                worksheet.Cell(row, 2).Value = scheme.Name;
                worksheet.Cell(row, 3).Value = scheme.Axes;
                worksheet.Cell(row, 4).Value = scheme.Mark;

                if(scheme.Volume.HasValue)
                {
                    worksheet.Cell(row, 5).Value = scheme.Volume.Value;
                }
                else
                {
                    worksheet.Cell(row, 5).Value = "";
                }

                worksheet.Cell(row, 6).Value = scheme.Status;

                // Цвет статуса
                string status = scheme.Status;
                XLColor color;

                if(status == "Подписана")
                    color = XLColor.LightGreen;
                else if(status == "На подписании")
                    color = XLColor.LightYellow;
                else if(status == "Готова к подписанию")
                    color = XLColor.LightBlue;
                else
                    color = XLColor.White;

                worksheet.Cell(row, 6).Style.Fill.BackgroundColor = color;

                row++;
            }

            // Форматирование заголовков
            for(int col = 1; col <= 6; col++)
            {
                var headerCell = worksheet.Cell(1, col);
                headerCell.Style.Font.Bold = true;
                headerCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Границы
            var dataRange = worksheet.Range(1, 1, row - 1, 6);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Автоширина
            worksheet.Columns(1, 6).AdjustToContents();

            return workbook;  // возвращаем workbook, но НЕ сохраняем
        }

        // Метод 2: Сохраняет workbook с проверками
        // Метод 2: Сохраняет workbook с обработкой ошибок
        public void SaveWorkbook(XLWorkbook workbook, string filePath)
        {
            // Пытаемся сохранить
            try
            {
                workbook.SaveAs(filePath);
                workbook.Dispose();
            }
            catch(IOException)
            {
                workbook.Dispose();
                throw new IOException("Файл открыт в Excel или другой программе. Закройте файл и попробуйте снова.");
            }
            catch(Exception ex)
            {
                workbook.Dispose();
                throw new Exception($"Ошибка при сохранении: {ex.Message}");
            }
        }
        private void ApplyColorCoding(IXLWorksheet worksheet, int totalRows)
        {
            // Проходим по всем строкам данных (начиная со 2-й)
            for (int row = 2; row <= totalRows; row++)
            {
                // Получаем статус из 5-й колонки
                string status = worksheet.Cell(row, 5).Value.ToString();

                // Определяем цвет в зависимости от статуса
                XLColor color;

                if (status == "Подписана")
                {
                    color = XLColor.LightGreen;
                }
                else if (status == "На подписании")
                {
                    color = XLColor.LightYellow;
                }
                else if(status == "Готова к подписанию")
                {
                    color = XLColor.LightBlue;
                }
                else
                {
                    color = XLColor.White;
                }
                // Применяем цвет ко всем 5 ячейкам строки
                for (int col = 1; col <= 5; col++)
                {
                    worksheet.Cell(row, col).Style.Fill.BackgroundColor = color;
                }
            }
        }
    }
}
