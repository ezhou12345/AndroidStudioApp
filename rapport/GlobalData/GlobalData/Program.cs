/*
 * Copyright (c) 
 * ArticuLab,
 * Human Computer Interaction Institute,
 * Carnegie Mellon University
 * 
 * Author: Alexandros Papangelis, 2014
 * 
 */

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace vrGlobalData
{
    public class Win32Interop
    {
        [DllImport("msvcrt.dll")]
        public static extern int _kbhit();

        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint timeGetTime();
    }

    class vrGlobalData
    {
        //TODO: HAVE ONE LOCK FOR EACH OBJECT IN THE GLOBAL DATA
        private static Object _update_lock = new Object();
        private UserModel _userModel = new UserModel();

        public int numMessagesReceived = 0;
        public int m_testSpecialCases = 0;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            int receiveMode = 0;

            if (args.Length > 0)
            {
                receiveMode = Convert.ToInt32(args[0]);
            }



            vrGlobalData e = new vrGlobalData();
            e.Run(receiveMode);
        }


        public void Run(int receiveMode)
        {
            VHMsg.Client vhmsg;
            using (vhmsg = new VHMsg.Client())
            {
                vhmsg.OpenConnection();

                Console.WriteLine("GLOBAL DATA MODULE");
                Console.WriteLine("VHMSG_SERVER: {0}", vhmsg.Server);
                Console.WriteLine("VHMSG_SCOPE: {0}", vhmsg.Scope);

                int NUM_MESSAGES = 200;


                if (receiveMode == 1)
                {
                    vhmsg.MessageEvent += new VHMsg.Client.MessageEventHandler(MessageAction);
                    vhmsg.SubscribeMessage("vrGlobalData");

                    Console.WriteLine("Receive Mode");


                    uint timeBefore = 0;
                    uint timeAfter;

                    while (Win32Interop._kbhit() == 0)
                    {
                        // we've received our first message
                        if (numMessagesReceived > 0 && timeBefore == 0)
                        {
                            timeBefore = Win32Interop.timeGetTime();
                        }

                        if (numMessagesReceived >= NUM_MESSAGES)
                        {
                            timeAfter = Win32Interop.timeGetTime();

                            Console.WriteLine("Time to receive {0} messages: {1}", NUM_MESSAGES, timeAfter - timeBefore);

                            numMessagesReceived = 0;
                            timeBefore = 0;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Send Mode");


                    long timeBefore = Win32Interop.timeGetTime();

                    for (int i = 0; i < NUM_MESSAGES; i++)
                    {
                        vhmsg.SendMessage("vrConversationMgr " + i + " Test Message");

                        if (i % 2000 == 0)
                        {
                            Console.WriteLine(i + " messages sent");
                        }
                    }

                    long timeAfter = Win32Interop.timeGetTime();

                    Console.WriteLine("Time to send {0} messages: {1}", NUM_MESSAGES, timeAfter - timeBefore);
                }

            }
        }

        //Handle incoming messages
        private void MessageAction(object sender, VHMsg.Message args)
        {
            //Console.WriteLine( "Received Message '" + e.toString() + "'" );

            //Ict.ElvinUtility eu = (Ict.ElvinUtility)sender;

            //Update message format: vrGlobalData update <dataStructure> <newData>
            //Get message format   : vrGlobalData get <dataStructure>

            String[] arguments = args.s.Split(' ');

            if(arguments.Length <= 1){
                sendMsg("error_args");
                return;
            }

            if(arguments[0] != "vrGlobalData"){
                sendMsg("error_args");
                return;
            }

            //Attempt to update data
            if (arguments[1] == "update")
            {
                if (!update(arguments))
                {
                    sendMsg("ok");
                }
                else
                {
                    sendMsg("busy");
                }
            }
            //Attempt to retrieve data
            else if (arguments[1] == "get")
            {
                if (!getData())
                {
                    sendMsg("busy");
                }
            }
            
            if (m_testSpecialCases == 1)
            {
                Console.WriteLine("received - '" + args.s + "'");
            }
            else
            {
                numMessagesReceived++;
                
                if (numMessagesReceived % 20 == 0)
                {
                    Console.WriteLine(numMessagesReceived + " messages received - '" + args.s + "'");
                }
            }
        }

        //TODO: KEEP A VH MSG CLIENT OPEN AT ALL TIMES TO AVOID OPENNING / CLOSING?

        //Send boolean vh messages to indicate (un)sucessful attempts to update
        private void sendMsg(String msg) { 
            VHMsg.Client vhmsg;

            using (vhmsg = new VHMsg.Client()){
                vhmsg.OpenConnection();

                //Sends a single message
                vhmsg.SendMessage("vrConversationMgr " + msg);
            }
        }

        //Update global data - does not block
        private bool update(String[] args) {
            //Thread-safe update. Attempt to get the lock
            if (Monitor.TryEnter(_update_lock))
            {
                //Perform the update
                _userModel.setData(Int32.Parse(args[2]));

                //Release the lock
                Monitor.Exit(_update_lock);
            }
            else
            {
                //Failure
                return false;
            }

            //Success
            return true;
        }

        private bool getData() {
            //Thread-safe retrieve. Attempt to get the lock
            if (Monitor.TryEnter(_update_lock))
            {
                sendMsg(_userModel.ToString());

                //Release the lock
                Monitor.Exit(_update_lock);
            }
            else
            {
                //Failure
                return false;
            }

            //Success
            return true;
        }
    }
}
