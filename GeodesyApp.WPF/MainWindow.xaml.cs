using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using GeodesyApp;

namespace GeodesyApp.WPF
{
    public partial class MainWindow: Window
    {
        // Строка подключения к твоей базе данных
        private string connectionString = "Server=.\\SQLEXPRESS;Database=GeodesyRegistry;Trusted_Connection=True;TrustServerCertificate=true;";

        private DatabaseHelper dbHelper;
        private ExcelExporter exporter;

        public MainWindow()
        {
            InitializeComponent();

            dbHelper = new DatabaseHelper(connectionString);
            exporter = new ExcelExporter();
        }

        // Кнопка "Загрузить"
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Scheme> schemes = dbHelper.GetAllSchemes();
                SchemesDataGrid.ItemsSource = schemes;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "Добавить"
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Добавление схемы — будет реализовано на следующем шаге",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Кнопка "Изменить статус"
        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Изменение статуса — будет реализовано на следующем шаге",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Кнопка "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Удаление — будет реализовано на следующем шаге",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Кнопка "Экспорт в Excel"
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if(SchemesDataGrid.ItemsSource is List<Scheme> schemes && schemes.Count > 0)
            {
                try
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = Path.Combine(desktopPath, "РеестрСхем.xlsx");

                    // Проверяем, существует ли файл
                    if(File.Exists(filePath))
                    {
                        var result = MessageBox.Show(
                            $"Файл '{Path.GetFileName(filePath)}' уже существует. Перезаписать?",
                            "Подтверждение",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if(result != MessageBoxResult.Yes)
                        {
                            MessageBox.Show("Экспорт отменён.", "Информация",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    var workbook = exporter.CreateExcelWorkbook(schemes);
                    exporter.SaveWorkbook(workbook, filePath);

                    MessageBox.Show($"Файл сохранён на Рабочем столе!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch(IOException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Сначала загрузите данные кнопкой 'Загрузить'!",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}