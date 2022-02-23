using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Controls;

namespace SequentialSerialWriter
{
    public static class SerialPortManager
    {
        private static SerialPort serialPort;
        public delegate void SerialDataReceivedCallback(string data);
        private static SerialDataReceivedCallback? serialDataReceivedCallback = null;
        public static void SetSerialDataReceivedCallback(SerialDataReceivedCallback callback) => serialDataReceivedCallback = callback; //コールバックを設定

        static SerialPortManager()
        {
            serialPort = new SerialPort()
            {
                PortName = "NONE", //未定義の値にする
                BaudRate = 115200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Encoding = Encoding.UTF8,
            };

            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
        }

        public static void SetPortName(string portName)
        {
            serialPort.PortName = portName;
            Debug.Print(serialPort.PortName.ToString());
        }

        public static void SetBaudRate(int baudrate)
        {
            serialPort.BaudRate = baudrate;
            Debug.Print(serialPort.BaudRate.ToString());
        }

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public static bool IsOpen() => serialPort.IsOpen;

        public static void Open() => serialPort.Open();
        
        public static void Close() => serialPort.Close();

        public static void Send(string send_str)
        {
            if (IsOpen())
            {
                serialPort.Write(send_str);
            }
        }

        private static void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null) return;
            if (serialPort.IsOpen == false) return;

            try
            {
                string readMessage = serialPort.ReadLine();
                if(serialDataReceivedCallback != null)
                {
                    serialDataReceivedCallback(readMessage);
                }
            }
            catch
            {

            }
        }
    }
}
