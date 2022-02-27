using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace SequentialSerialWriter
{
    internal class SendTextListController
    {
        private static readonly int SEND_WAIT_TIME = 1100; //次の送信までの時間[ms]

        /// <summary>
        /// 送信テキストリスト（ListBoxにバインド）
        /// </summary>
        internal static ObservableCollection<SendTextListBoxItem> SendTextList = new ObservableCollection<SendTextListBoxItem>();

        internal static void AddItem()
        {
            SendTextList.Add(new SendTextListBoxItem());
        }

        /// <summary>
        /// 特定の要素を削除する
        /// </summary>
        /// <param name="item"></param>
        internal static void RemoveItem(SendTextListBoxItem item) => SendTextList.Remove(item);

        /// <summary>
        /// リストの最後の要素を削除する
        /// </summary>
        internal static void RemoveLastItem() => SendTextList.Remove(SendTextList.Last());

        internal static int Count() => SendTextList.Count;

        internal static async void SendAll()
        {
            MainWindow.Instance.SendAll_Button_IsEnabled(false);

            for (int idx = 0; idx < SendTextList.Count(); idx++)
            {
                await Task.Delay(SEND_WAIT_TIME);
                Debug.Print($"{idx+1}/{SendTextList.Count()}");
            }

            MainWindow.Instance.SendAll_Button_IsEnabled(true);
        }
    }

    internal class SendTextListBoxItem
    {
        private string _sendText;

        public string SendText
        {
            get { return _sendText; }
            set { _sendText = value; }
        }

        public SendTextListBoxItem()
        {
            _sendText = String.Empty;
        }
    }
}
