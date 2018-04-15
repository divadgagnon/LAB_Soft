using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Messaging;
using System.IO.Ports;
using LAB.Model;
using SerialComm.PacketEncoder.Interfaces;
using SerialComm.SerialPortData;
using SerialComm.PacketEncoder;

namespace SerialComm
{
 public class LSPProtocol : IDisposable
 {

  #region Fields

  private IPacketEncoder m_encoder;
  private Queue<byte[]> m_rxQueue;
  private SerialPort m_serialPort;

  #endregion

  #region Constructors

  /// <summary>
  /// Default constructor of LSPProtocol.
  /// </summary>
  public LSPProtocol()
   :this(new DefaultPacketEncoder())
  {
  }

  /// <summary>
  /// Constructor of LSPProtocol with specified encoder.
  /// </summary>
  /// <param name="encoder">Encoder to be used with this serial port protocol decoder.</param>
  public LSPProtocol(IPacketEncoder encoder)
  {
   m_encoder=encoder;
   m_rxQueue=new Queue<byte[]>();
   m_serialPort=null;
  }

  #endregion

  #region Public events

  /// <summary>
  /// Event fired when a new data packet is received.
  /// </summary>
  public event EventHandler DataPacketReceived;

  #endregion

  #region Public methods

  /// <summary>
  /// Closes the serial port connexion and releases resources.
  /// </summary>
  public void Close()
  {
   if(m_serialPort!=null)
   {
    m_serialPort.DataReceived-=DataReceivedHandler;
    m_serialPort.Close();
    m_serialPort.Dispose();
    m_serialPort=null;
   }
  }

  /// <summary>
  /// Disposes of the underlying unmanaged resources.
  /// </summary>
  public void Dispose()
  {
   Dispose(true);
   GC.SuppressFinalize(this);
   Close();
   m_serialPort.Dispose();
  }

  /// <summary>
  /// Opens the serial port specified in settings.
  /// </summary>
  /// <param name="portName">Name of the serial port to open .</param>
  /// <param name="settings">Settings of the port.</param>
  public void Open(string portName,SerialPortSettings settings)
  {
        if(m_serialPort!=null)
            { 
                Messenger.Default.Send(true, "SafeModeUpdate");
                System.Windows.MessageBox.Show("The LSP Protocol object couldn't open a serial port, since it already had a serial port open.");
            }

        m_serialPort=new SerialPort(portName,settings.BaudRate,settings.Parity,settings.DataBits,settings.StopBits);
        if(!m_serialPort.IsOpen)
        {
            m_serialPort.Open();
        }
        m_serialPort.DataReceived+=new SerialDataReceivedEventHandler(DataReceivedHandler);
        m_encoder.Reset();
  }

  /// <summary>
  /// Queues the oldest receive packet.
  /// </summary>
  /// <returns>The oldest received packet's payload or null if none was received.</returns>
  public byte[] ReceivePacketData()
  {
        if(m_rxQueue.Count>0)
        {
            return m_rxQueue.Dequeue();
        }

        return default(byte[]);
  }

  /// <summary>
  /// Sends provided data as a packet on the serial port.
  /// </summary>
  /// <param name="packetData">Data to be sent as a packet on the serial port.</param>
  public void SendPacketData(byte[] packetData)
  {
            try
            {
                if (m_serialPort == null)
                {
                    Messenger.Default.Send(true, "SafeModeUpdate");
                    System.Windows.MessageBox.Show("Serial port is not open or available. Couldn't sent serial data. Reconnect the brewery and resume");
                }

                m_encoder.SendPacket(packetData);
                byte[] buffer = new byte[256];
                int count;

                do
                {
                    count = m_encoder.Read(buffer, 0, 256);
                    m_serialPort.Write(buffer, 0, count);
                }
                while (count > 0);
            }
            catch
            {
                Messenger.Default.Send(true, "SafeModeUpdate");
                System.Windows.MessageBox.Show("An error occured while trying to send data to the serial port. Try to reconnect the brewery and resume");
            }
}

  #endregion

  #region Protected virtual methods

  /// <summary>
  /// Disposes of the resources of the object if disposing.
  /// </summary>
  /// <param name="disposing">True if disposing of the object, false otherwise.</param>
  protected virtual void Dispose(bool disposing)
  {
   if(disposing)
   {
    Close();
   }
  }

  /// <summary>
  /// Fires the DataPacketReceived event.
  /// </summary>
  protected virtual void OnDataPacketReceived()
  {
   if(DataPacketReceived!=null)
   {
    DataPacketReceived(this,EventArgs.Empty);
   }
  }

  #endregion

  #region Private methods

  /// <summary>
  /// Handles data received on serial port.
  /// </summary>
  /// <param name="sender">Sender object.</param>
  /// <param name="e">Arguments of the event.</param>
  private void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
  {
   byte[] packet;
   byte[] rxBuffer=new byte[256];

   while(m_serialPort.BytesToRead>0)
   {
    int count=m_serialPort.Read(rxBuffer,0,256);
    do
    {
     m_encoder.Write(rxBuffer);
     packet=m_encoder.ReceivePacket();
     if(packet!=null)
     {
      m_rxQueue.Enqueue(packet);
      OnDataPacketReceived();
     }
    }
    while(m_rxQueue.Count!=0);
   }
  }

  #endregion

 }
}
