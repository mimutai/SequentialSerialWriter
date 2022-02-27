using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SequentialSerialWriter
{
    internal class SendTextListController
    {
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
