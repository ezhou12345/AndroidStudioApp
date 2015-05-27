using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vrNLU
{
    public class Win32Interop
    {
        [DllImport("msvcrt.dll")]
        public static extern int _kbhit();

        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint timeGetTime();

        [DllImport(@"D:\vhtoolkit\core\smartbody\sbgui\bin\SmartBody.dll")]
        public static extern void constructBML();
    }

    public class NLU
    {
        private IPAddress _serverIP = IPAddress.Parse("128.2.213.163");
        private AsynchronousClient _tcpClient;
        private VHMsg.Client _vhmsgClient;
        private int _serverPort = 9096, _agentID = 0;
        private string[] _subscribedMessages = { "vrDialogue", "vrNLU" };
        private string _newText;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            NLU e = args.Length < 2 ? new NLU() : new NLU(args[0], int.Parse(args[1]));
            e.run();
        }

        public NLU() { }

        public NLU(string ipAddress, int port)
        {
            _serverIP = IPAddress.Parse(ipAddress);
            _serverPort = port;
        }

        ~NLU() { }

        public void run()
        {
            //Connect to the server
            _tcpClient = new AsynchronousClient(this, _serverIP, _serverPort);

            _tcpClient.StartClient();

            using (_vhmsgClient = new VHMsg.Client())
            {
                _vhmsgClient.OpenConnection();

                _vhmsgClient.MessageEvent += new VHMsg.Client.MessageEventHandler(MessageAction);

                for (int i = 0; i < _subscribedMessages.Length; i++)
                {
                    _vhmsgClient.SubscribeMessage(_subscribedMessages[i]);
                }

                //Announce availability
                _vhmsgClient.SendMessage("vrComponent DialogueManager all");

                while (Win32Interop._kbhit() == 0) { }
            }
        }

        //The tcp client calls this function to update current text
        public void newTextReceiver(string newText) {
            _newText = newText;

            //Message format:
            /*The message will be: userid^text
            e.g. fa7683^Hello Alex
            Please send me back the following message if you believe that you are going
            to handle this text (I guess you will handle all text you get for now):
            userid^handling
            e.g. fa7683^handling
            Please send me back the following message when you reply to the user:
            userid^replying
            e.g. fa7683^replying*/

            _vhmsgClient.SendMessage("vrNLU " + _agentID.ToString() + " " + _newText);
        }

        private void print(string msg)
        {
            System.Console.WriteLine("\nNLU " + System.Diagnostics.Process.GetCurrentProcess().Id + ", " + _agentID + ": " + msg);
        }

        private void MessageAction(object sender, VHMsg.Message args)
        {
            String[] arguments = args.s.Split(' ');
            String vrNLUMsg = "vrNLU " + _agentID + " ";

            Random rand = new Random();

            if (arguments.Length <= 1)
            {
                return;
            }

            //Discard messages not directed for this agent
            if (arguments[1] != _agentID.ToString()) { return; }

            //DEBUG:
            //print("Received message: " + args.s);

            //If it is a ping message
            if (arguments[0].Equals("vrAllCall"))
            {
                _vhmsgClient.SendMessage("vrComponent NLU all");
            }
            //Gracefully kill self
            else if (arguments[0] == "vrKillComponent")
            {
                if (arguments[1] == "NLU")
                {
                    //Inform that ConversationManager is exiting
                    _vhmsgClient.SendMessage("vrProcEnd NLU all");

                    System.Environment.Exit(0);
                }
            }
        }

    }
}


