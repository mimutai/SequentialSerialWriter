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
            SendTextList.Add(new SendTextListBoxItem(String.Empty));
        }

        internal static void AddItem(string text)
        {
            SendTextList.Add(new SendTextListBoxItem(text));
        }

        internal static void SetList(List<String> list)
        {
            SendTextList.Clear(); //リストを初期化する

            foreach (string str in list)
            {
                AddItem(str);
            }
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

        /// <summary>
        /// リスト内の任意の要素を、その前後の要素と交換する
        /// </summary>
        /// <param name="item">入れ替える要素</param>
        /// <param name="forward">指定した要素を前の(インデックスの小さい)要素と入れ替える（falseで後ろと入れ替え）</param>
        internal static void SwapBackAndForth(SendTextListBoxItem item, bool forward)
        {
            if (item == null) return; //処理しない

            int item_idx = SendTextList.IndexOf(item); //入れ替え対象のオブジェクトのインデックス

            if (forward)
            {
                //前と入れ替える
                if (item_idx > 0) //交換可能
                {
                    // 入れ替える
                    SendTextListBoxItem tmp = SendTextList[item_idx];
                    SendTextList[item_idx] = SendTextList[item_idx - 1];
                    SendTextList[item_idx - 1] = tmp;
                }
            }
            else
            {
                //後ろと入れ替える
                if (item_idx < SendTextList.Count - 1) //交換可能
                {
                    // 入れ替える
                    SendTextListBoxItem tmp = SendTextList[item_idx];
                    SendTextList[item_idx] = SendTextList[item_idx + 1];
                    SendTextList[item_idx + 1] = tmp;
                }
            }
        }

        internal static async void SendAll()
        {
            if (!SerialPortManager.IsOpen()) return; // ポートが開いていなかったら送信しない

            MainWindow.Instance.SetEnableControlUI(false); // リスト制御UIを無効化する

            for (int idx = 0; idx < SendTextList.Count(); idx++)
            {
                MainWindow.Instance.SendProgress_TextBlock_SetText($"Sending: {idx + 1}/{SendTextList.Count()}"); // 送信状況をUIに表示する
                SerialPortManager.Send(SendTextList[idx].SendText); // シリアルポートで送信する

                await Task.Delay(SEND_WAIT_TIME); //送信遅延
            }

            MainWindow.Instance.SetEnableControlUI(true); // リスト制御UIを有効化する
            MainWindow.Instance.SendProgress_TextBlock_SetText(string.Empty); // 送信状況のUIを非表示にする
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

        public SendTextListBoxItem(string text)
        {
            _sendText = text;
        }
    }
}
