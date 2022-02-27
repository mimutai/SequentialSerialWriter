﻿using System;
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
            MainWindow.Instance.SendAll_Button_IsEnabled(false); // SendAllボタンを無効化する

            for (int idx = 0; idx < SendTextList.Count(); idx++)
            {
                MainWindow.Instance.SendProgress_TextBlock_SetText($"Sending: {idx + 1}/{SendTextList.Count()}"); // 送信状況をUIに表示する
                SerialPortManager.Send(SendTextList[idx].SendText); // シリアルポートで送信する

                await Task.Delay(SEND_WAIT_TIME); //送信遅延
            }

            MainWindow.Instance.SendAll_Button_IsEnabled(true); // SendAllボタンを有効化する
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

        public SendTextListBoxItem()
        {
            _sendText = String.Empty;
        }
    }
}
