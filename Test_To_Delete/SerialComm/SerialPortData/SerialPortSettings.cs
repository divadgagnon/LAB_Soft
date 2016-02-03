using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace SerialComm.SerialPortData
{
 /// <summary>
 /// Serial port settings data structure.
 /// </summary>
 public sealed class SerialPortSettings
 {

  #region Fields

  private int m_baudRate;
  private int m_dataBits;

  #endregion

  #region Properties

  /// <summary>
  /// Gets or sets the baud rate of the serial port.
  /// </summary>
  /// <remarks>Must be a positive value greater than 1.</remarks>
  public int BaudRate
  {
   get
   {
    return m_baudRate;
   }
   set
   {
    m_baudRate=(value<1)?1:value;
   }
  }

  /// <summary>
  /// Gets or sets the amount of data bits in a byte for serial transmission.
  /// </summary>
  /// <remarks>Must be ranging from 5 up to 8.</remarks>
  public int DataBits
  {
   get
   {
    return m_dataBits;
   }
   set
   {
    m_dataBits=(value<5)?5:(value>8)?8:value;
   }
  }

  /// <summary>
  /// Gets or sets the parity of the serial port.
  /// </summary>
  public Parity Parity
  {
   get;
   set;
  }

  /// <summary>
  /// Gets or sets the amount of stop bits for the serial port.
  /// </summary>
  public StopBits StopBits
  {
   get;
   set;
  }

  #endregion

  #region Constructor

  /// <summary>
  /// Default constructor of SerialPortSettings.
  /// </summary>
  public SerialPortSettings()
  {
   m_baudRate=9600;
   m_dataBits=8;
   Parity=Parity.None;
   StopBits=StopBits.One;
  }

  #endregion

 }
}
