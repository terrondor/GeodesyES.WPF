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
            // Создаём окно добавления схемы
            var addWindow = new AddSchemeWindow();

            // Показываем окно как модальный диалог (блокирует главное окно)
            bool? result = addWindow.ShowDialog();

            // Если пользователь нажал "Сохранить" (DialogResult = true)
            if(result == true)
            {
                try
                {
                    // Создаём объект Scheme из данных формы
                    var newScheme = new Scheme
                    {
                        Name = addWindow.SchemeName,
                        Axes = addWindow.SchemeAxes,
                        Mark = addWindow.SchemeMark,
                        Volume = addWindow.SchemeVolume,
                        Status = addWindow.SchemeStatus
                    };

                    // Добавляем в базу данных
                    bool success = dbHelper.AddScheme(newScheme);

                    if(success)
                    {
                        MessageBox.Show("Схема успешно добавлена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Обновляем таблицу
                        LoadButton_Click(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить схему. Проверьте данные.", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Кнопка "Редактировать"
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что строка выбрана
            if(SchemesDataGrid.SelectedItem is Scheme selectedScheme)
            {
                // Открываем окно редактирования с текущими данными
                var editWindow = new EditSchemeWindow(selectedScheme);
                editWindow.Owner = this;

                bool? result = editWindow.ShowDialog();

                if(result == true)
                {
                    try
                    {
                        // Создаём обновлённый объект Scheme
                        var updatedScheme = new Scheme
                        {
                            Id = selectedScheme.Id,
                            Name = editWindow.SchemeName,
                            Axes = editWindow.SchemeAxes,
                            Mark = editWindow.SchemeMark,
                            Volume = editWindow.SchemeVolume,
                            Status = editWindow.SchemeStatus
                        };

                        // Обновляем в базе данных
                        bool success = dbHelper.UpdateScheme(updatedScheme);

                        if(success)
                        {
                            MessageBox.Show($"Схема '{updatedScheme.Name}' успешно обновлена!",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Обновляем таблицу
                            LoadButton_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить схему.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите схему в таблице!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Кнопка "Удалить"
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что строка выбрана
            if(SchemesDataGrid.SelectedItem is Scheme selectedScheme)
            {
                // Показываем диалог подтверждения с названием схемы
                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить схему?\n\n" +
                    $"ID: {selectedScheme.Id}\n" +
                    $"Название: {selectedScheme.Name}\n" +
                    $"Оси: {selectedScheme.Axes}\n" +
                    $"Отметка: {selectedScheme.Mark}\n\n" +
                    $"Это действие нельзя отменить!",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Если пользователь нажал "Да"
                if(result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = dbHelper.DeleteScheme(selectedScheme.Id);

                        if(success)
                        {
                            MessageBox.Show($"Схема '{selectedScheme.Name}' успешно удалена!",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Обновляем таблицу
                            LoadButton_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить схему.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите схему в таблице!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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