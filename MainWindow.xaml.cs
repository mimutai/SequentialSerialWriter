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
using System.Diagnostics;

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

            List<string> portList = SerialPortManager.GetPortNames().ToList(); //ポート一覧を取得する
            SetPortListComboBox(portList); //ポート一覧をComboBoxに設定する
            if (portList.Count > 0) PortListComboBox.SelectedIndex = portList.Count - 1; // ComboBoxの初期値を設定
            // ComboBoxを展開したときのイベントを設定する
            PortListComboBox.DropDownOpened += (object? sender, EventArgs e) =>
            {
                SetPortListComboBox(SerialPortManager.GetPortNames().ToList());
            };
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
            int selectedIndex = PortListComboBox.SelectedIndex;
            if (selectedIndex < 0 && PortListComboBox.Items.Count > 0) //選択されていたポートが存在しなくなった時
            {
                //存在するポートを選択する
                selectedIndex = PortListComboBox.Items.Count - 1; //インデックスの更新
                PortListComboBox.SelectedIndex = selectedIndex; //表示の更新
            }

            string? selectedPortName = PortListComboBox.Items[selectedIndex].ToString();
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
            ReceivedData_TextBlock.Dispatcher.Invoke(new Action(() =>
            {
                if (!text.EndsWith("\n")) text += "\n"; //改行コードがなければ付与する

                string timestamp_str = DateTime.Now.ToString(" HH:mm:ss.fff-> "); //タイムスタンプ
                ReceivedData_TextBlock.Text += timestamp_str + text; //新しく加える

            }));
        }

        // シリアルポートの開閉を行う
        private void OpenClose_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!SerialPortManager.IsOpen())
            {
                SerialPortManager.Open();
                PortListComboBox.IsEnabled = false;
                OpenClose_Button.Content = "Close";
            }
            else
            {
                SerialPortManager.Close();
                PortListComboBox.IsEnabled = true;
                OpenClose_Button.Content = "Open";
            }
        }
    }
}
