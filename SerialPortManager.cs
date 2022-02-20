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

        private static void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null) return;
            if (serialPort.IsOpen == false) return;

            try
            {
                string readMessage = serialPort.ReadLine();
            }
            catch
            {

            }
        }
    }
}
