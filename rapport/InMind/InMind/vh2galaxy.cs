/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2015.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 */


//REF: https://msdn.microsoft.com/en-us/library/bew39x2a%28v=vs.110%29.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InMind
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    public class vh2galaxy
    {
        // The port number for the remote device.
        private const int port = 9096;              //TODO: Make this a large random integer?

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote devicClass1.cse.
        private static String response = String.Empty;

        private IPAddress _ipAddr;
        private int _port;
        private Socket _client;
        private static VHMsg.Client _vhmsgClient;
        private string[] _subscribedMessages = { "vrGalaxy", "vrKillComponent", "vrAllCall" };

        public vh2galaxy() {
            _ipAddr = IPAddress.Parse("127.0.0.1");

            Connect();
        }

        public vh2galaxy(IPAddress ipAddr, int port)
        {
            _ipAddr = ipAddr;
            _port = port;

            Connect();
        }

        ~vh2galaxy()
        {
            // Release the socket.
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();

            //Close vh client
            if (_vhmsgClient != null)
            {
                _vhmsgClient.CloseConnection();
            }
        }

        private void Connect(){
            // Connect to remote galaxy device.
            try
            {
                // Establish the remote endpoint for the socket.
                IPEndPoint remoteEP = new IPEndPoint(_ipAddr, _port);

                // Create a TCP/IP socket.
                _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                _client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), _client);
                connectDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            // Connect to vh network
            _vhmsgClient = new VHMsg.Client();

            _vhmsgClient.OpenConnection();

            for (int i = 0; i < _subscribedMessages.Length; i++)
            {
                _vhmsgClient.SubscribeMessage(_subscribedMessages[i]);
            }

            _vhmsgClient.MessageEvent += new VHMsg.Client.MessageEventHandler(MessageAction);

            //Announce availability
            _vhmsgClient.SendMessage("vrComponent vh2galaxy all");
        }

        private void MessageAction(object sender, VHMsg.Message args)
        {
            string[] arguments = args.s.Split(' ');

            if (arguments.Length <= 1)
            {
                return;
            }

            //DEBUG:
            //print("Received message: " + args.s);

            //If it is a ping message
            if (arguments[0].Equals("vrAllCall"))
            {
                _vhmsgClient.SendMessage("vrComponent RapportManager all");
            }
            //Gracefully kill self
            else if (arguments[0] == "vrKillComponent")
            {
                if (arguments[1] == "ConversationManager")
                {
                    //Inform that ConversationManager is exiting
                    _vhmsgClient.SendMessage("vrProcEnd ConversationManager all");

                    System.Environment.Exit(0);
                }
            }
            else if (arguments[0] == "vrGalaxy" && arguments[1] == "outgoing"){
                //Transmit message to galaxy network
                byte[] bytes = new byte[(args.s.Length - 2) * sizeof(char)];
                System.Buffer.BlockCopy(args.s.Substring(2, args.s.Length - 1).ToCharArray(), 0, bytes, 0, bytes.Length);
                _client.Send(bytes);
            }
        }

        public void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Receive the response from the remote device.
                Receive(_client);
                receiveDone.WaitOne();

                // Write the response to the console.
                Console.WriteLine("Response received : {0}", response);

                /*Response should be a handshake:
                 * {c handshake
                    :conn_type 1
                    :protocol_version 1 }
                 */
                //Respond to the handshake

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();

                    //Broadcast to vh network
                    _vhmsgClient.SendMessage("vrGalaxy incoming " + bytesRead.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

