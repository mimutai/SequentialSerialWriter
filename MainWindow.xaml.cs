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
using System.Collections.ObjectModel;

namespace SequentialSerialWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        internal static MainWindow? Instance;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            List<string> portList = SerialPortManager.GetPortNames().ToList(); //ポート一覧を取得する
            SetPortListComboBox(portList); //ポート一覧をComboBoxに設定する
            if (portList.Count > 0) PortListComboBox.SelectedIndex = portList.Count - 1; // ComboBoxの初期値を設定
            // ComboBoxを展開したときのイベントを設定する
            PortListComboBox.DropDownOpened += (object? sender, EventArgs e) =>
            {
                SetPortListComboBox(SerialPortManager.GetPortNames().ToList());
            };


            SetBoudRateComboBox(new List<string>() { "9600", "115200" });
            BaudRateComboBox.SelectedValue = "115200"; //初期値を設定
            SerialPortManager.SetBaudRate(115200);

            SerialPortManager.SetSerialDataReceivedCallback(AddSerialReceivedData);

            // ファイルからデータを読み込む
            SaveDataManager.Load();
            SendTextListController.SetList(SaveDataManager.SaveData.SendTextList);

            Debug.WriteLine(SendTextListController.SendTextList.Count);
            SendTextListBox.ItemsSource = SendTextListController.SendTextList;

            SendProgress_TextBlock_SetText(string.Empty);
        }

        /// <summary>
        /// アプリケーション終了時のイベント
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //データを保存する
            SaveDataManager.SaveData.SendTextList = SendTextListController.SendTextList.Select(x => x.SendText).ToList();
            SaveDataManager.Save();
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
                ReceivedData_TextBlock.Text += text; //新しく加える
                ReceivedData_ScrollViewer.ScrollToBottom(); //自動で最下部にスクロールする
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

        /// <summary>
        /// 文字列を送信する
        /// </summary>
        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!SerialPortManager.IsOpen()) return;

            SerialPortManager.Send(SendString_TextBox.Text);
            SendString_TextBox.Text = String.Empty;
        }

        /// <summary>
        /// 受信した文字列をクリアする
        /// </summary>
        private void ClearOutput_Button_Click(object sender, RoutedEventArgs e)
        {
            ReceivedData_TextBlock.Text = String.Empty;
        }

        /// <summary>
        /// 送信テキストリストに追加
        /// </summary>
        private void ListBoxItem_Add_Click(object sender, RoutedEventArgs e)
        {
            SendTextListController.AddItem();
        }

        /// <summary>
        /// 送信テキストリストから削除
        /// </summary>
        private void ListBoxItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            SendTextListBoxItem selectedItem = (SendTextListBoxItem)SendTextListBox.SelectedItem;
            if (selectedItem != null)
            {
                //選択されている要素を削除する
                SendTextListController.RemoveItem(selectedItem);
            }
            else if (SendTextListController.Count() > 0)
            {
                // 何も選択されていないときには末尾の要素を削除する
                SendTextListController.RemoveLastItem();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendAll_Button_Click(object sender, RoutedEventArgs e)
        {
            SendTextListController.SendAll();
        }

        internal void SendAll_Button_IsEnabled(bool isEnable) => SendAll_Button.IsEnabled = isEnable;

        /// <summary>
        /// リストを制御するUIの有効無効を設定する
        /// </summary>
        /// <param name="enable"></param>
        internal void SetEnableControlUI(bool enable)
        {
            SendAll_Button.IsEnabled = enable;
            ListBoxItem_Add.IsEnabled = enable;
            ListBoxItem_Remove.IsEnabled = enable;
            ListBoxItem_Up.IsEnabled = enable;
            ListBoxItem_Down.IsEnabled = enable;
        }

        internal void SendProgress_TextBlock_SetText(string text) => SendProgress_TextBlock.Text = text;

        private void ListBoxItem_Up_Click(object sender, RoutedEventArgs e)
        {
            SendTextListBoxItem selectedItem = (SendTextListBoxItem)SendTextListBox.SelectedItem;
            if (selectedItem != null)
            {
                SendTextListController.SwapBackAndForth(selectedItem, true);
                SendTextListBox.SelectedItem = selectedItem;
            }

        }

        private void ListBoxItem_Down_Click(object sender, RoutedEventArgs e)
        {
            SendTextListBoxItem selectedItem = (SendTextListBoxItem)SendTextListBox.SelectedItem;
            if (selectedItem != null)
            {
                SendTextListController.SwapBackAndForth(selectedItem, false);
                SendTextListBox.SelectedItem = selectedItem;
            }
        }
    }
}
