using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialComm.PacketEncoder.Interfaces;

namespace SerialComm.PacketEncoder
{
 /// <summary>
 /// Simple packet protocol encoder.
 /// </summary>
 /// <remarks>Packet data is encoded in format [0x55][LENGTH][DATA][BCC].</remarks>
 public sealed class SimplePacketProtocolPacketEncoder: IPacketEncoder
 {

  #region Fields

  private List<byte> m_rxData;
  private Queue<byte[]> m_rxPacket;
  private List<byte> m_txData;

  #endregion

  #region Constructor

  /// <summary>
  /// Default constructor of SimplementPacketProtocolPacketEncoder.
  /// </summary>
  public SimplePacketProtocolPacketEncoder()
  {
   m_rxData=new List<byte>();
   m_rxPacket=new Queue<byte[]>();
   m_txData=new List<byte>();
  }

  #endregion


  #region IPacketEncoder Members

  /// <summary>
  /// Receives oldest packet.
  /// </summary>
  /// <returns>The oldest packet received or null if none is available.</returns>
  public byte[] ReceivePacket()
  {
   if(m_rxPacket.Count>0)
   {
    return m_rxPacket.Dequeue();
   }

   return default(byte[]);
  }

  /// <summary>
  /// Sends provided packet as raw bytes.
  /// </summary>
  /// <param name="packetData">Packet data to encode.</param>
  public void SendPacket(byte[] packetData)
  {
   byte bcc=0;

   if(packetData.Length>255)
   {
    throw new ArgumentException("Packet data was larger than maximum of 255 supported by SimplePacketProtocol.");
   }

   m_txData.Add(0x55);
   m_txData.Add((byte)packetData.Length);
   m_txData.AddRange(packetData);
   for(int i=0;i<packetData.Length;++i)
   {
    bcc^=packetData[i];
   }
   m_txData.Add(++bcc);
  }

  /// <summary>
  /// Reads data that have been encoded into packets.
  /// </summary>
  /// <param name="buffer">Buffer to write to.</param>
  /// <param name="offset">Offset to write to.</param>
  /// <param name="count">Maximum count of data to write.</param>
  /// <returns>Actual count of data written.</returns>
  public int Read(byte[] buffer,int offset,int count)
  {
   if(m_txData.Count<count)
   {
    m_txData.ToArray().CopyTo(buffer,offset);
    count=m_txData.Count;
    m_txData.Clear();
    
    return count;
   }

   Array.Copy(m_txData.ToArray(),0,buffer,offset,count);
   m_txData.RemoveRange(0,count);

   return count;
  }

  /// <summary>
  /// Resets encoder's internal state.
  /// </summary>
  public void Reset()
  {
   m_rxData.Clear();
   m_rxPacket.Clear();
   m_txData.Clear();
  }

  /// <summary>
  /// Writes data to be decoded in order to extract packet payload.
  /// </summary>
  /// <param name="data">Data to be decoded.</param>
  public void Write(byte[] data)
  {
   m_rxData.AddRange(data);

   while(m_rxData.Count>0)
   {
    if(m_rxData[0]!=0x55)
    {
     m_rxData.RemoveAt(0);
    }
    else
    {
     if(m_rxData.Count>=m_rxData[1]+3)
     {
      byte bcc=0;
      for(int i=m_rxData[1]+1;i>=2;--i)
      {
       bcc^=m_rxData[i];
      }
      ++bcc;
      if(m_rxData[m_rxData[1]+2]==bcc)
      {
       byte[] payload=new byte[m_rxData[1]];
       Array.Copy(m_rxData.ToArray(),2,payload,0,payload.Length);
       m_rxPacket.Enqueue(payload);
       m_rxData.RemoveRange(0,m_rxData[1]+3);
      }
      else
      {
       m_rxData.RemoveAt(0);
      }
     }
     else
     {
      break;
     }
    }
   }
  }

  #endregion
 }
}
