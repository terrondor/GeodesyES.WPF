using System;
using System.Windows;
using System.Windows.Controls;
using GeodesyApp;

namespace GeodesyApp.WPF
{
    public partial class EditSchemeWindow: Window
    {
        private Scheme _originalScheme;

        // Конструктор принимает схему для редактирования
        public EditSchemeWindow(Scheme scheme)
        {
            InitializeComponent();
            _originalScheme = scheme;

            // Заполняем поля текущими значениями
            NameTextBox.Text = scheme.Name;
            AxesTextBox.Text = scheme.Axes;
            MarkTextBox.Text = scheme.Mark;
            VolumeTextBox.Text = scheme.Volume?.ToString() ?? "";

            // Выбираем текущий статус в ComboBox
            foreach(ComboBoxItem item in StatusComboBox.Items)
            {
                if(item.Content.ToString() == scheme.Status)
                {
                    StatusComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        // Свойства для получения данных из формы
        public string SchemeName => NameTextBox.Text.Trim();
        public string SchemeAxes => AxesTextBox.Text.Trim();
        public string SchemeMark => MarkTextBox.Text.Trim();
        public string SchemeStatus
        {
            get {
                if(StatusComboBox.SelectedItem is ComboBoxItem selected)
                    return selected.Content.ToString();
                return "Новая";
            }
        }
        public decimal? SchemeVolume
        {
            get {
                if(decimal.TryParse(VolumeTextBox.Text.Trim(), out decimal volume))
                    return volume;
                return null;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if(string.IsNullOrEmpty(SchemeName))
            {
                MessageBox.Show("Введите название схемы!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NameTextBox.Focus();
                return;
            }

            if(string.IsNullOrEmpty(SchemeAxes))
            {
                MessageBox.Show("Введите оси!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                AxesTextBox.Focus();
                return;
            }

            if(string.IsNullOrEmpty(SchemeMark))
            {
                MessageBox.Show("Введите отметку!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                MarkTextBox.Focus();
                return;
            }

            // Всё заполнено — закрываем окно с результатом OK
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}