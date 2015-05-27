/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;


namespace InMind
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

    // A delegate type for hooking up dialogue state change notifications.
    public delegate void DSUpdatedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Summary description for ConversationManager.
    /// </summary>
    public class ConversationManager
    {
        private static Object _update_lock = new Object();
        private static int _count = 0;
        public int _numMessagesReceived = 0;
        private VHMsg.Client _vhmsgClient;
        private List<Thread> _activeThreads = new List<Thread>();
        private string[] _subscribedMessages = { "vrConversation", "vrGlobalData", "vrIntention", "vrInteraction" };
        private static RapportManager _rapportMgr;
        private static DialogueManager _dialogueMgr;
        private DialogueState _dialogueState = new DialogueState();
        public event DSUpdatedEventHandler DSUpdated;
        private bool _systemHasFloor;
        private string vrIntentionMsg;
        private int _agentID;

        public ConversationManager(int agentID = 0) { 
            _systemHasFloor = false; vrIntentionMsg = "";
            _agentID = agentID;

            //Initialize Dialogue state
            _dialogueState.DialogueTurn = 0;
            _dialogueState.SocialGoal = SOCIAL_GOAL.ENHANCE_RAPPORT;
            _dialogueState.TaskGoal = TASK_GOAL.NONE;
        }

        public ConversationManager(VHMsg.Client vhmsgClient, int agentID = 0) { 
            _vhmsgClient = vhmsgClient;
            _systemHasFloor = false;
            vrIntentionMsg = "";
            _agentID = agentID;

            //Initialize Dialogue state
            _dialogueState.DialogueTurn = 0;
            _dialogueState.SocialGoal = SOCIAL_GOAL.ENHANCE_RAPPORT;
            _dialogueState.TaskGoal = TASK_GOAL.NONE;
        }

        ~ConversationManager()
        {
            /*//Maintenance
            foreach (Thread t in _activeThreads)
            {
                if (!t.IsAlive) { _activeThreads.Remove(t); }
            }*/

            if (_vhmsgClient != null)
            {
                _vhmsgClient.CloseConnection();
            }

            print("### DESTROYED. ###\n");
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            ConversationManager e = new ConversationManager(0);
            e.run();
        }

        //Invoke Dialogue State Updated event
        protected virtual void OnDSUpdated(EventArgs e) {
            if (DSUpdated != null) { DSUpdated(this, e); }
        }

        public DialogueState getDialogueState() { return _dialogueState; }

        public int getAgentID() { return _agentID; }

        private void print(string msg)
        {
            System.Console.WriteLine("\nCONVERSATION MGR " + System.Diagnostics.Process.GetCurrentProcess().Id + ", " + _agentID + ": " + msg);
        }


        public void run()
        {
            using (_vhmsgClient = new VHMsg.Client())
            {
                _vhmsgClient.OpenConnection();

                _vhmsgClient.MessageEvent += new VHMsg.Client.MessageEventHandler(MessageAction);

                for (int i = 0; i < _subscribedMessages.Length; i++ ) {
                    _vhmsgClient.SubscribeMessage(_subscribedMessages[i]);
                }

                //Announce availability
                _vhmsgClient.SendMessage("vrComponent ConversationManager all");

                //Suspend self, message-halding thread will take over
                print("Listening for incoming messages...");

                //Activate the Dialogue Manager
                _dialogueMgr = new DialogueManager(this);
                Thread DMThread = new Thread(new ThreadStart(_dialogueMgr.run));
                _activeThreads.Add(DMThread);
                DMThread.Start();

                //Activate the Interaction Manager
                _rapportMgr = new RapportManager(this);
                Thread IMThread = new Thread(new ThreadStart(_rapportMgr.run));
                _activeThreads.Add(IMThread);
                IMThread.Start();

                //FOR DEMO ONLY
                _dialogueState.OtherAgentID = _agentID == 0 ? 1 : 0;

                //Notify Dialoge State Updated for initializations at the dialogue managers
                OnDSUpdated(EventArgs.Empty);

                while (Win32Interop._kbhit() == 0) { }
            }
        }

        public bool updateDialogueState(DialogueState tempDialogueState) {
            //Thread-safe update. Attempt to get the lock
            if (Monitor.TryEnter(_update_lock))
            {
                //Perform the update
                _dialogueState.DialogueTurn++;

                _dialogueState.PreviousUtterance = String.Copy(_dialogueState.CurrentUtterance);
                _dialogueState.PreviousASRConfidence = _dialogueState.CurrentASRConfidence;
                _dialogueState.PreviousEmotionConfidence = _dialogueState.CurrentEmotionConfidence;
                _dialogueState.PreviousToneConfidence = _dialogueState.CurrentToneConfidence;

                _dialogueState.CurrentUtterance = String.Copy(tempDialogueState.CurrentUtterance);
                _dialogueState.CurrentASRConfidence = tempDialogueState.CurrentASRConfidence;
                _dialogueState.CurrentEmotionConfidence = tempDialogueState.CurrentEmotionConfidence;
                _dialogueState.CurrentToneConfidence = tempDialogueState.CurrentToneConfidence;

                //Notify Dialoge State Updated
                OnDSUpdated(EventArgs.Empty);

                //print("Dialogue state updated.");

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

        //Handle incoming messages
        private void MessageAction(object sender, VHMsg.Message args)
        {
            _numMessagesReceived++;

            print(_numMessagesReceived + " messages received - '" + args.s + "'");

            String[] arguments = args.s.Split(' ');

            if (arguments.Length <= 1)
            {
                return;
            }

            //If it is a ping message
            if (arguments[0].Equals("vrAllCall")) {
                _vhmsgClient.SendMessage("vrComponent ConversationManager all");
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
            else if(arguments[0].Equals("vrInteraction")){
                //Ignore vrInteraction messages from other agents
                if (arguments[1] == _agentID.ToString())
                {
                    if (arguments[2].Equals("floorChanged") || arguments[2].Equals("floor"))
                    {
                        if ((_systemHasFloor = (arguments[3].Equals("system"))) == true)
                        {
                            //Foward any pending messages
                            if (vrIntentionMsg != "")
                            {
                                _vhmsgClient.SendMessage(vrIntentionMsg);
                                vrIntentionMsg = "";
                                updateDialogueState(_dialogueState);
                                //Release floor
                                _systemHasFloor = false;
                                _vhmsgClient.SendMessage("vrInteraction " + _agentID + " releaseFloor");
                            }
                            //Brad takes initiative (bypass the dialogue manager)
                            else if (_dialogueState.DialogueTurn == 0 && _agentID == 0)
                            {
                                //Create and send a Phatic Opener
                                _vhmsgClient.SendMessage("vrIntention " + _dialogueState.OtherAgentID.ToString() + " NONE ENHANCE_RAPPORT PHATIC_OPENER CONVENTION_GREETING");
                                vrIntentionMsg = "";
                                updateDialogueState(_dialogueState);
                                //Release floor
                                _systemHasFloor = false;
                                _vhmsgClient.SendMessage("vrInteraction " + _agentID + " releaseFloor");
                            }
                            else if (_dialogueState.DialogueTurn == 15)
                            {
                                //Create and send a Phatic End
                                _vhmsgClient.SendMessage("vrIntention " + _dialogueState.OtherAgentID.ToString() + " NONE MAINTAIN_RAPPORT PHATIC_END CONVENTION_GREETING");
                                vrIntentionMsg = "";
                                updateDialogueState(_dialogueState);
                                //Release floor
                                _systemHasFloor = false;
                                _vhmsgClient.SendMessage("vrInteraction " + _agentID + " releaseFloor");
                            }
                        }
                    }
                }
            }
            else if(arguments[0].Equals("vrIntention")){
                //Ignore vrIntention messages coming out of the Conversation Manager to the Realizer
                //Note: Outgoing vrIntention msgs have different agentID while incoming ones have this agent's ID
                if (arguments[1].Equals(_agentID.ToString()))
                {
                    //Store or forward vrIntention message to the dialogue manager
                    vrIntentionMsg = "vrDialogue " + _agentID + " " + args.s;   //overwrite old message

                    if (_systemHasFloor)
                    {
                        ///Forward message to dialogue manager
                        _vhmsgClient.SendMessage(vrIntentionMsg);
                        vrIntentionMsg = "";
                    }
                }
            }
            else if (arguments[0].Equals("vrConversation"))
            {
                //Ignore vrInteraction messages from other agents
                if (arguments[1].Equals(_agentID.ToString()))
                {
                    //This comes from the dialogue manager
                    if (arguments[2].Equals("vrIntention"))
                    {
                        string msg = "";    //overwrite old message

                        //Discard the first two arguments
                        for (int i = 2; i < arguments.Length; i++)
                        {
                            msg += arguments[i] + " ";
                        }

                        _vhmsgClient.SendMessage(msg);

                        print("*SENT* : " + msg);
                        print("===== DIALOGUE TURN: " + _dialogueState.DialogueTurn + " =====");

                        updateDialogueState(_dialogueState);

                        //Release floor
                        _systemHasFloor = false;
                        _vhmsgClient.SendMessage("vrInteraction " + _agentID + " releaseFloor");
                    }
                }
            }
        }

        public bool loadConfigurationFile() { return false; }

        public void sendMsg()
        {
            Random rand = new Random();

            using (_vhmsgClient = new VHMsg.Client())
            {
                _vhmsgClient.OpenConnection();

                Thread.Sleep(rand.Next(3000));

                //Sends a single message
                _vhmsgClient.SendMessage("vrGlobalData update " + _count++);

                Console.WriteLine("Sent: vrGlobalData update " + (_count - 1));

                _vhmsgClient.CloseConnection();
            }
        }
    }
}
