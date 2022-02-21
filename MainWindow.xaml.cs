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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace SequentialSerialWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SetPortListComboBox(new List<string>() { "COM1", "COM5" });
            SetBoudRateComboBox(new List<string>() { "9600", "115200" });

            SerialPortManager.SetSerialDataReceivedCallback(AddSerialReceivedData);
        }

        public void SetPortListComboBox(List<String> items)
        {
            PortListComboBox.ItemsSource = items;
        }

        public void SetBoudRateComboBox(List<String> items)
        {
            BaudRateComboBox.ItemsSource = items;
        }

        private void PortListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedPortName = PortListComboBox.SelectedValue.ToString();
            if (selectedPortName != null)
            {
                SerialPortManager.SetPortName((string)selectedPortName);
            }
        }

        private void BaudRateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedBaudRate = BaudRateComboBox.SelectedValue.ToString();
            if (selectedBaudRate != null)
            {
                int baudRate = 0;
                bool validString = int.TryParse(selectedBaudRate, out baudRate);
                if (validString) SerialPortManager.SetBaudRate(baudRate);
            }
        }

        public void AddSerialReceivedData(string text)
        {
            ReceivedData_TextBlock.Dispatcher.Invoke(new Action(() => {
                if (!text.EndsWith("\n")) text += "\n"; //改行コードがなければ付与する

                string timestamp_str = DateTime.Now.ToString(" HH:mm:ss.fff-> "); //タイムスタンプ
                ReceivedData_TextBlock.Text += timestamp_str + text; //新しく加える
            
            }));
        }
    }
}
