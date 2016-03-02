using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows;
using SerialComm;
using SerialComm.SerialPortData;
using SerialComm.PacketEncoder;
using GalaSoft.MvvmLight.Messaging;
using LAB.Model;

namespace LAB
{
    public class ArduinoCommands
    {

 # region Fields

        public LSPProtocol device;
        public DispatcherTimer Error_Timer;

        bool FirstPing = true;

 # endregion

 # region Constructor

        public ArduinoCommands(LSPProtocol _device)
        {
            // Create Instances
            device = _device;

            // Timer Setup
            device.DataPacketReceived += new EventHandler(device_DataPacketReceived);
            Error_Timer = new DispatcherTimer();
            Error_Timer.Interval = TimeSpan.FromMilliseconds(2500);    // 2,5s timeout
            Error_Timer.Tick += Error_Timer_Tick;
        }
        
# endregion

 # region Send Methods

        /// <summary>
        /// Opens the specified serial port
        /// </summary>
        /// <param name="comPort">Name of the port to open</param>

        public void Open(string comPort)
        {
            device.Open(comPort, new SerialPortSettings());      
        }

        /// <summary>
        /// Closes the serial port
        /// </summary>
        
        public void ClosePort()
        {
            device.Close();
        }

        /// <summary>
        /// Sends a request to the device to test connectivity
        /// </summary>
        
        public void Ping()
        {
            device.SendPacketData(new byte[] { 0x01 });
            StartTimeoutTimer();
        }


        /// <summary>
        ///  Sets the digital pin modes to output or input
        /// </summary>
        /// <param name="Pin"></param>
        /// <param name="pinMode"></param>
        public void SetPinMode(int Pin, PinModes pinMode)
        {
            byte pinByte = Convert.ToByte(Pin);
            device.SendPacketData(new byte[] { 0x02, pinByte, (byte)pinMode });
        }

        /// <summary>
        /// Set a digital pin high or low
        /// </summary>
        /// <param name="Pin">Pin to set</param>
        /// <param name="Level">Level of the pin (true or false)</param>

        public void DigitalWrite(int Pin, RelayState State)
        {
            byte pinByte = Convert.ToByte(Pin);
            byte LevelByte = Convert.ToByte(State);
            device.SendPacketData(new byte[3] {0x04, pinByte, LevelByte});
            StartTimeoutTimer();
        }

        /// <summary>
        /// Read a digital pin
        /// </summary>
        /// <param name="Pin">Pin to read</param>
        
        public void DigitalRead(int Pin)
        {
            byte pinByte = Convert.ToByte(Pin);
            device.SendPacketData(new byte[2] { 0x03, pinByte });
            StartTimeoutTimer();
        }

        public void AnalogRead(int Pin)
        {
            byte pinByte = Convert.ToByte(Pin);
            device.SendPacketData(new byte[2] { 0x05, pinByte });
            StartTimeoutTimer();
        }

        public void GetProbeAddress()
        {
            device.SendPacketData(new byte[1] { 0x06 });
            StartTimeoutTimer();
        }

        public void GetTemperatures()
        {
            device.SendPacketData(new byte[1] { 0x07 });
            //StartTimeoutTimer();
        }

        

 # endregion

 # region Timer Tick Events
        // ------------------------------------------------ Timer Tick Events -------------------------------------------------------------

        private void StartTimeoutTimer()
        {
            Error_Timer.Start();
        }

        private void Error_Timer_Tick(object sender, EventArgs e)
        {
            // Retry a second time if the command sent was Ping
            if (FirstPing)
            {
                FirstPing = false;
                Error_Timer.Stop();
                Ping();
            }

            //Timeout on command.
            device.Close();
            Error_Timer.Stop();
            Brewery brewery = new Brewery();
            brewery.IsConnected = false;
            Messenger.Default.Send<Brewery>(brewery, "ConnectionUpdate");
            MessageBox.Show("No answer was received from the device, make sure it is correctly connected");
        }

 # endregion

 # region Receive Command Dispatcher

        // --------------------------------------------- Incoming Packet Management --------------------------------------------------------

        public void device_DataPacketReceived(object sender, EventArgs e)
        {
            byte[] packet = device.ReceivePacketData();

            byte CommandByte = packet[0];
            
            switch (CommandByte)
            {
                case 0x01:
                {
                    returnPing(packet);
                    packet = null;
                    break;
                }

                case 0x04:
                {
                    returnDigitalWrite(packet);
                    packet = null;
                    break;
                }

                case 0x05:
                    {
                        returnAnalogRead(packet);
                        packet = null;
                        break;
                    }

                case 0x06:
                {
                    returnGetProbeAddress(packet);
                    packet = null;
                    break;
                }

                case 0x07:
                {
                    returnGetTemperatures(packet);
                    packet = null;
                    break;
                }

                default:
                {
                    // Set StateMachineState = CommError;
                    break;
                }
            }
            // Run stateMachine in view model?

        }

 # endregion

 # region Receive Methods

        // Ping return
        private void returnPing(byte[] packet)
        {
            if(packet.Count() != 2) { device.Close(); goto MessageError; }

            if(packet[1] == 0x67)
            {
                Error_Timer.Stop();
                Brewery brewery = new Brewery();
                brewery.IsConnected = true;
                Messenger.Default.Send<Brewery>(brewery, "ConnectionUpdate");
                return;
            }

            MessageError:
            MessageBox.Show("A device was detected but it is not the brewery. Please select the correct port.");
        }

        // Analog Read return
        private void returnAnalogRead(byte[] packet)
        {
            // Set the FirstPingSent variable back to false
            FirstPing = false;

            // Store data in volSensor class
            Error_Timer.Stop();
            AnalogReturn volSensor = new AnalogReturn();
            volSensor.Pin = packet[1];
            volSensor.Value = packet[2];

            // Send volSensor data to brewery command class
            Messenger.Default.Send<AnalogReturn>(volSensor, "VolumeUpdate");            
        }

        // Digital Write return
        private void returnDigitalWrite(byte[] packet)
        {
            Error_Timer.Stop();
            DigitalReturn pinstate = new DigitalReturn();
            pinstate.Pin = packet[1];
            if (packet[2] == 0x00) { pinstate.Value = false; }
            else if (packet[2] == 0x01) { pinstate.Value = true; }
            else { MessageBox.Show("There was a communication error when receiving a digital write return on pin" + (int)packet[1] + " Pin State value received is : " + (int)packet[2]); return; }

            // Send Digital return data to brewery command class
            Messenger.Default.Send<DigitalReturn>(pinstate);
        }

       // Get Probe Address return         
        private void returnGetProbeAddress(byte[] Packet)
        {
            Error_Timer.Stop();
            Probes probes = new Probes();

            int NbOfProbes = Packet.Count()-1;

            for (int i=1; i<=NbOfProbes; i++)
            {
                switch(Packet[i])
                {
                    case 0x9E:
                        {
                            //Probe Jaune et Rose
                            probes.YellowPink.IsConnected = true;
                            break;
                        }
                    case 0x4B:
                        {
                            //Probe Jaune et Orange
                            probes.YellowOrange.IsConnected = true;
                            break;
                        }
                    case 0x5A:
                        {
                            //Probe Orange
                            probes.Orange.IsConnected = true;
                            break;
                        }
                    case 0x4F:
                        {
                            //Probe Rose
                            probes.Pink.IsConnected = true;
                            break;
                        }
                    case 0x26:
                        {
                            //Probe Jaune
                            probes.Yellow.IsConnected = true;
                            break;
                        }
                }
            }

            Messenger.Default.Send<Probes>(probes, "GetConnectedProbes");
        }

        // Get Temperetures return
        private void returnGetTemperatures(byte[] Packet)
        {
            Error_Timer.Stop();
            Probes probes = new Probes();
            int NbOfProbes = (int)Packet.Count() - 1;

            for (int i = 1; i <= NbOfProbes; i+=2)
            {
                switch (Packet[i])
                {
                    case 0x9E:
                        {
                            //Probe Jaune et Rose
                            probes.YellowPink.Temp = (double)Packet[i + 1] * 0.46875;
                            break;
                        }
                    case 0x4B:
                        {
                            //Probe Jaune et Orange
                            probes.YellowOrange.Temp = (double)Packet[i + 1] * 0.46875;
                            break;
                        }
                    case 0x5A:
                        {
                            //Probe Orange
                            probes.Orange.Temp = (double)Packet[i + 1] * 0.46875;
                            break;
                        }
                    case 0x4F:
                        {
                            //Probe Rose
                            probes.Pink.Temp = (double)Packet[i + 1] * 0.46875;
                            break;
                        }
                    case 0x26:
                        {
                            //Probe Jaune
                            probes.Yellow.Temp = (double)Packet[i + 1] * 0.46875;
                            break;
                        }
                }
            }

            Messenger.Default.Send<Probes>(probes, "TemperatureUpdate");
        }
 #endregion
    }
}
