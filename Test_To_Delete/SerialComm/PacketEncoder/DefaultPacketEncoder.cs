using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialComm.PacketEncoder.Interfaces;
using System.Collections;

namespace SerialComm.PacketEncoder
{
 /// <summary>
 /// Default packet encoder implementation.
 /// </summary>
 /// <remarks>This default encoder only forwards data as is without conversion.</remarks>
 public sealed class DefaultPacketEncoder: IPacketEncoder
 {

  #region Fields

  List<byte> m_rxBytes;
  List<byte> m_txBytes;

  #endregion

  #region Constructor

  /// <summary>
  /// Default constructor of DefaultPacketEncoder.
  /// </summary>
  public DefaultPacketEncoder()
  {
   m_rxBytes=new List<byte>();
   m_txBytes=new List<byte>();
  }

  #endregion

  #region Public methods

  /// <summary>
  /// Receives packets out of the encoder interface.
  /// </summary>
  /// <returns>All bytes received so far.</returns>
  public byte[] ReceivePacket()
  {
   byte[] data=m_rxBytes.ToArray();
   m_rxBytes.Clear();

   return data;
  }

  /// <summary>
  /// Sends provided packet data in the encoder.
  /// </summary>
  /// <param name="packetData">Packet data payload.</param>
  public void SendPacket(byte[] packetData)
  {
   m_txBytes.AddRange(packetData);
  }

  /// <summary>
  /// Reads bytes from sent pakcets that have been encoded.
  /// </summary>
  /// <param name="buffer">Buffer to write to.</param>
  /// <param name="offset">Offset to write to in buffer.</param>
  /// <param name="count">Maximum amount of byte to write in buffer.</param>
  /// <returns>Amount of bytes copied.</returns>
  public int Read(byte[] buffer,int offset,int count)
  {
   if(m_txBytes.Count<count)
   {
    m_txBytes.ToArray().CopyTo(buffer,offset);
    count=m_txBytes.Count;
    m_txBytes.Clear();

    return count;
   }

   Array.Copy(m_txBytes.ToArray(),0,buffer,offset,count);
   m_txBytes.RemoveRange(0,count);

   return count;
  }

  /// <summary>
  /// Resets the internal state of the encoder.
  /// </summary>
  public void Reset()
  {
   m_rxBytes.Clear();
   m_txBytes.Clear();
  }

  /// <summary>
  /// Writes data that have been received so that it is converted to "packet" data.
  /// </summary>
  /// <param name="data">Data to write.</param>
  public void Write(byte[] data)
  {
   m_rxBytes.AddRange(data);
  }

  #endregion

 }
}
