using System;
using System.Windows;
using System.Windows.Controls;

namespace GeodesyApp.WPF
{
    public partial class AddSchemeWindow: Window
    {
        public AddSchemeWindow()
        {
            InitializeComponent();
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
            // Отмена — закрываем окно с результатом false
            DialogResult = false;
            Close();
        }
    }
}