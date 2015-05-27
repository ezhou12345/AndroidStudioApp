/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 * Use case description: 
 * 1. GlobalData connets to user model database (or loads dictionary)
 * 2. The user logs in, with a unique id. 
 * 3. GlobalData loads the corresponding user model and makes it available to consumers
 * 
 */

//TODO: Have error codes somewhere
//      Currently the _agentID messaging is set up for a SINGLE agent

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

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
        private Dictionary<Guid, UserModel> _userModels;
        private Dictionary<string, Guid> _passwords;   //TODO: Add a tiny bit more security
        private Guid _currentUser;
        private VHMsg.Client _vhmsgClient;
        private Interface _interface;
        private WozInterface _woz;
        private string[] _subscribedMessages = { "vrAllCall", "vrLogin", "vrGlobalData", "vrKillComponent" };
        private int _agentID;

        public int numMessagesReceived = 0;
        public int m_testSpecialCases = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            int receiveMode = 0;
            string startInterface = "";

            if (args.Length > 0)
            {
                receiveMode = Convert.ToInt32(args[0]);
            }
            if(args.Length > 1){
                startInterface = args[1];
            }

            vrGlobalData e = new vrGlobalData();
            e.Run(receiveMode, startInterface);
        }


        public void Run(int receiveMode, string startInterface)
        {
            _agentID = 0;

            if (startInterface == "i")
            {
                _interface = new Interface();
                _interface.ShowDialog();
            }
            else if (startInterface == "w"){
                _woz = new WozInterface();
                _woz.ShowDialog();
            }
            else
            {
                //Load passwords list
                loadPasswords(@"c:\passwordList.xml");

                //Load user models
                loadUserModels(@"c:\userModels.xml");

                _currentUser = Guid.Empty;

                using (_vhmsgClient = new VHMsg.Client())
                {
                    _vhmsgClient.OpenConnection();

                    Console.WriteLine("GLOBAL DATA MODULE");
                    Console.WriteLine("VHMSG_SERVER: {0}", _vhmsgClient.Server);
                    Console.WriteLine("VHMSG_SCOPE: {0}", _vhmsgClient.Scope);

                    //Announce availability
                    _vhmsgClient.SendMessage("vrComponent GlobalData all");
                    
                    if (receiveMode == 1)
                    {
                        _vhmsgClient.MessageEvent += new VHMsg.Client.MessageEventHandler(MessageAction);

                        for (int i = 0; i < _subscribedMessages.Length; i++)
                        {
                            _vhmsgClient.SubscribeMessage(_subscribedMessages[i]);
                        }

                        Console.WriteLine("Receive Mode");

                        //Suspend self, message-halding thread will take over
                        print(" listening for incoming messages...");

                        while (Win32Interop._kbhit() == 0) { }
                    }
                    else  //FOR DEBUG ONLY
                    {
                        Console.WriteLine("Send Mode");

                        string input = "";
                        int num_sent_msgs = 0;
                        Random rand = new Random();

                        while (input != "q")
                        {
                            if (input == "i")
                            {
                                //Give agent0 the initiative
                                _vhmsgClient.SendMessage("vrIntention 0 NONE ENHANCE_RAPPORT PHATIC_OPENER CONVENTION_GREETING");
                                System.Console.WriteLine("Sent vrIntention message - " + (++num_sent_msgs) + " messages so far)");
                            }
                            else if (input == "s")
                            {
                                _vhmsgClient.SendMessage("vrSpoke " + (num_sent_msgs % 2));
                                System.Console.WriteLine("Sent 'vrSpoke " + (num_sent_msgs % 2) + "' message - " + (++num_sent_msgs) + " messages so far)");
                            }
                            else if (input == "t")
                            {
                                _vhmsgClient.SendMessage("vrSpoke " + ((num_sent_msgs % 2) == 0 ? "Brad" : "Rachel"));
                                System.Console.WriteLine("Sent 'vrSpoke " + ((num_sent_msgs % 2) == 0 ? "Brad" : "Rachel") + "' message - " + (++num_sent_msgs) + " messages so far)");
                            }
                            else if (input == "g")
                            {
                                string msg = "vrGoal " + (num_sent_msgs % 2) + (rand.NextDouble() < 0.5 ? " LEARN_GEOMETRY" : " TEACH_GEOMETRY") + " MAINTAIN_RAPPORT";
                                _vhmsgClient.SendMessage(msg);
                                System.Console.WriteLine("Sent '" + msg + "' " + (++num_sent_msgs) + " messages so far)");
                            }

                            System.Console.WriteLine("Press 'i' to send a vrIntention, 's' to send a vrSpoke, 'g' to send a vrGoal and 'q' to quit...");
                            input = System.Console.ReadLine();
                        }
                    }
                }
            }
        }

        //Handle incoming messages
        private void MessageAction(object sender, VHMsg.Message args)
        {
            //Update message format: vrGlobalData update <dataStructure> <newData>
            //Get message format   : vrGlobalData get <dataStructure>

            String[] arguments = args.s.Split(' ');

            if(arguments.Length <= 1){
                sendMsg("error " + args.s);
                print("error " + args.s);
                return;
            }

            //If it is a ping message
            if (arguments[0] == "vrAllCall")
            {
                _vhmsgClient.SendMessage("vrComponent GlobalData all");
            }
            //Gracefully kill self
            else if (arguments[0] == "vrKillComponent")
            {
                if (arguments[1] == "GlobalData")
                {
                    //Save user models
                    saveUserModels();

                    //Inform that ConversationManager is exiting
                    _vhmsgClient.SendMessage("vrProcEnd GlobalData all");

                    System.Environment.Exit(0);
                }
            }
            else if(arguments[0] != "vrGlobalData" || arguments[1] != _agentID.ToString()){
                sendMsg("error_args");
                return;
            }

            //Attempt to update data
            if (arguments[2] == "update")
            {
                if (!update(arguments))
                {
                    sendMsg("ok");
                    print("update ok");
                }
                else
                {
                    sendMsg("busy");
                    print("busy");
                }
            }
            //Attempt to retrieve data
            else if (arguments[2] == "get")
            {
                if (!getData())
                {
                    sendMsg("busy");
                    print("busy");
                }
            }
            //User wants to login
            else if (arguments[2] == "login"){
                if (_passwords.ContainsKey(arguments[2]))
                {
                    _currentUser = _passwords[arguments[2]];
                    print("user " + arguments[3] + " logged in");
                }
                else {
                    sendMsg("failed_noUser");
                    print("failed to log in " + arguments[3]);
                }
            }
            //User wants to logout
            else if (arguments[2] == "logout")
            {
                if (_passwords.ContainsKey(arguments[2]))
                {
                    if(_currentUser.Equals(_passwords[arguments[2]])){
                        _currentUser = Guid.Empty;
                        print("user " + arguments[3] + " logged out");
                    }
                    else{
                        sendMsg("failed");
                        print("failed to log out " + arguments[3]);
                    }
                }
                else
                {
                    sendMsg("failed_noUser");
                    print("failed no user");
                }
            }
            //Create a new user
            else if(arguments[2] == "new"){
                if (_passwords.ContainsKey(arguments[3])) { sendMsg("failed_userExists"); }
                else
                {
                    Guid newUserGuid = new Guid();
                    _passwords.Add(arguments[3], newUserGuid);
                    _userModels.Add(newUserGuid, new UserModel());
                    _currentUser = newUserGuid;

                    print("user " + arguments[3] + " created");
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
                vhmsg.SendMessage("vrGlobalData " + _agentID + " Response " + msg);
            }
        }

        //Update global data - does not block
        //vrGlobalData agent_id userModel [get|update] <userModelXML>
        //TODO: In the future we'll have:   vrGlobalData agent_id userModel [get|update] userModelID <userModelXML>
        private bool update(String[] args) {
            //Thread-safe update. Attempt to get the lock
            if (Monitor.TryEnter(_update_lock))
            {
                if (!_currentUser.Equals(Guid.Empty))
                {
                    //Perform the update
                    _userModels[_currentUser] = UserModel.LoadFromXMLString(args[4]);
                }
                else {
                    sendMsg("failed_userNotPresent");
                }

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
                if (!_currentUser.Equals(Guid.Empty))
                {
                    sendMsg("userModel " + _userModels[_currentUser].ToXML());
                }
                else
                {
                    sendMsg("error userNotPresent");
                }

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

        private void saveUserModels() {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(_userModels.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\userModels.xml");

            xmlSerializer.Serialize(file, _userModels);
        }

        //True: loaded user model
        //False: created new user model
        private bool loadUserModels(string fileName) {
            _userModels = DeSerializeObject<Dictionary<Guid, UserModel>>(@"c:\userModels.xml");

            if (_userModels == null) { 
                _userModels = new Dictionary<Guid, UserModel>();
                return false;
            }

            return true;
        }

        private void savePasswords() {
            System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(_passwords.GetType());
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"c:\passwordList.xml");

            xmlSerializer.Serialize(file, _passwords);
        }

        //True: loaded password list
        //False: created new password list
        private bool loadPasswords(string fileName)
        {
            _passwords = DeSerializeObject<Dictionary<string,Guid>>(@"c:\passwordList.xml");

            if (_passwords == null) { 
                _passwords = new Dictionary<string, Guid>();
                return false;
            }

            return true;
        }

        private void print(string msg)
        {
            System.Console.WriteLine("\nGLOBAL DATA " + System.Diagnostics.Process.GetCurrentProcess().Id + ", " + _agentID + ": " + msg);
        }

        //REF: http://stackoverflow.com/questions/6115721/how-to-save-restore-serializable-object-to-from-file
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }


        //REF: http://stackoverflow.com/questions/6115721/how-to-save-restore-serializable-object-to-from-file
        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
    }
}
