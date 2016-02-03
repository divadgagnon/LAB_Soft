using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialComm.PacketEncoder.Interfaces
{
 /// <summary>
 /// Protocol encoder interface.
 /// </summary>
 public interface IPacketEncoder
 {

  /// <summary>
  /// Receives the next packet parsed from the protocol.
  /// </summary>
  /// <returns>A byte array containing data inside the inbound packet.</returns>
  byte[] ReceivePacket();

  /// <summary>
  /// Sends a packet with provided data.
  /// </summary>
  /// <param name="packetData">Data to encode in a packet prior sending.</param>
  void SendPacket(byte[] packetData);

  /// <summary>
  /// Reads packed data to be sent through underlying protocol stack layer.
  /// </summary>
  /// <param name="buffer">Buffer to write to.</param>
  /// <param name="offset">Offset to write data to.</param>
  /// <param name="count">Count of data to be written.</param>
  /// <returns>Amount of data written.</returns>
  int Read(byte[] buffer,int offset,int count);

  /// <summary>
  /// Resets internal encoder state.
  /// </summary>
  void Reset();

  /// <summary>
  /// Writes packed data to be decoded.
  /// </summary>
  /// <param name="data">Data to be unpacked.</param>
  void Write(byte[] data);

 }
}
