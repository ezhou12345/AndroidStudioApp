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

namespace InMind
{
    public enum RAPPORT_STRATEGY { NONE = 0, INIT_SELF_DISC, REFER_SHARED_EXP, INTIMATE_PERSONAL_INFO, RECIPROCAL_APPRECIATION, ADHERE_TO_NORMS, VIOLATE_NORMS, RECIPROCATE_PREV_ACT, ACKNOWLEDGE, PRAISE, NEGATIVE_SELF_DISC, EMB_LAUGHTER, RS_ITEMS };
    public enum RAPPORT_STRATEGY_DEMO { NONE = 0, QUESTION_ELICIT_SD, SELF_DISC_POSITIVE, SELF_DISC_NEGATIVE, PRAISE, REFER_TO_SHARED_EXPERIENCE, ACKNOWLEDGE, ADHERE_TO_NORMS, RSD_ITEMS }

    class RapportManager
    {
        private ConversationManager _parent;
        private VHMsg.Client _vhmsgClient;
        private string[] _subscribedMessages = { "vrDialogue", "vrGoal", "vrGlobalData" };
        private SparseMatrix<DialogueState, DialogueAction> _dialoguePolicy;
        private DialogueState _dialogueState;
        private Stack<Tuple<DialogueState, SparseMatrix<DialogueState, DialogueAction>>> _DSStack = new Stack<Tuple<DialogueState, SparseMatrix<DialogueState, DialogueAction>>>();
        private MDP _mdp = new MDP();
        private DM_TYPE _type = DM_TYPE.REINFORCEMENT_LEARNING; //Defined in DialogueManager.cs
        private int _agentID;
        private string _userName = "testUser";
        private vrGlobalData.UserModel _userModel;

        public RapportManager(ConversationManager parent, DM_TYPE type = DM_TYPE.RULE_BASED)
        {
            _parent = parent;
            _type = type;
            _dialoguePolicy = new SparseMatrix<DialogueState, DialogueAction>();
            _dialogueState = new DialogueState();
            _agentID = parent.getAgentID();

            _parent.DSUpdated += new DSUpdatedEventHandler(DSUpdated);
        }

        ~RapportManager()
        {
            if (_vhmsgClient != null)
            {
                _vhmsgClient.CloseConnection();
            }

            print("### DESTROYED. ###\n");
        }

        private void print(string msg)
        {
            System.Console.WriteLine("\nRAPPORT MGR " + System.Diagnostics.Process.GetCurrentProcess().Id + ", " + _agentID + ": " + msg);
        }

        public void run()
        {
            using (_vhmsgClient = new VHMsg.Client())
            {
                _vhmsgClient.OpenConnection();

                switch (_type)
                {
                    case DM_TYPE.RULE_BASED:
                        {
                            _vhmsgClient.MessageEvent += new VHMsg.Client.MessageEventHandler(MessageActionRuleBased);
                            break;
                        }
                    case DM_TYPE.REINFORCEMENT_LEARNING:
                        {
                            runRL();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Error! Unknown Rapport Manager Type: " + _type);
                            break;
                        }
                }

                for (int i = 0; i < _subscribedMessages.Length; i++)
                {
                    _vhmsgClient.SubscribeMessage(_subscribedMessages[i]);
                }

                //Announce availability
                _vhmsgClient.SendMessage("vrComponent RapportManager all");

                //Log user in
                _vhmsgClient.SendMessage("vrGlobalData " + _agentID + " login " + _userName);

                //Suspend self, message-halding thread will take over
                print(_type.ToString() + " listening for incoming messages...");

                while (Win32Interop._kbhit() == 0) { }
            }
        }

        public void runRL()
        {
            //CODE BELOW IS ONLY FOR TESTING RL
            int NStates = 5, NActions = (int)COMMUNICATIVE_ACT.COMM_ITEMS, NEpisodes = 50, NIterations = 20;
            Random rand = new Random();

            SparseMatrix<DialogueState, DialogueAction> policy = new SparseMatrix<DialogueState, DialogueAction>();

            for (int s = 0; s < NStates; s++)
            {
                //Create new State
                DialogueState ds = new DialogueState();
                ds.CurrentUtterance = "state" + s;
                policy.addState(ds);
            }

            for (int a = 0; a < NActions; a++)
            {
                DialogueAction da = new DialogueAction();
                da.setVerbalAct(new CommunicativeAct((COMMUNICATIVE_ACT)(a % (int)COMMUNICATIVE_ACT.COMM_ITEMS)));
                policy.addAction(da);
            }

            DialogueState currState = policy.getStateList()[0];
            DialogueAction currAct = policy.getActionList()[0];

            //QLearner<DialogueState, DialogueAction> RLearner = new QLearner<DialogueState, DialogueAction>(0.95, 0.7, 0.15, policy, rewardFunction);
            QLambdaLearner<DialogueState, DialogueAction> RLearner = new QLambdaLearner<DialogueState, DialogueAction>(0.95, 0.9, 0.15, 0.8, policy, rewardFunction);
            //SARSALearner<DialogueState, DialogueAction> RLearner = new SARSALearner<DialogueState, DialogueAction>(0.95, 0.9, 0.15, 0.8, policy, rewardFunction);

            RLearner.setStartState(currState);

            //Iterate
            for (int episode = 0; episode < NEpisodes; episode++)
            {
                currState = policy.getStateList()[0];
                currAct = policy.getActionList()[0];
                int stateIndex = 0;

                for (int iteration = 0; (iteration < NIterations) && (currState.CurrentUtterance != "state4"); iteration++)
                {
                    currAct = RLearner.nextAction();

                    //Observe new state
                    if (currAct.getCommunicativeAct().getCommActType() == COMMUNICATIVE_ACT.NO_COMM)
                    {
                        currState = policy.getStateList()[(++stateIndex) % NStates];
                    }

                    //Update (and observe reward)
                    RLearner.update(currState, currAct);
                }

                //Final update to get final reward
                //qLearner.update(currState, currAct);

                RLearner.newEpisode();

                Console.WriteLine("EPISODE " + episode + ": " + RLearner.getEpisodeReward(episode));
            }

            Console.WriteLine("\nPolicy items: " + policy.getPolicy().Count + "\n");

            foreach (Tuple<DialogueState, DialogueAction> t in policy.getPolicy().Keys)
            {
                Console.WriteLine("State: " + t.Item1.CurrentUtterance + ", Action: " + t.Item2.getCommunicativeAct().getCommActType() + "\n");
            }

            printPolicy(policy);
        }

        //Handler for RL training, that bypasses all other communication and connects the two agents' dialogue managers
        private void MessageActionTrain(object sender, VHMsg.Message args)
        {
            String[] arguments = args.s.Split(' ');

            if (arguments.Length <= 1)
            {
                return;
            }

            //If it is a ping message
            if (arguments[0].Equals("vrAllCall"))
            {
                _vhmsgClient.SendMessage("vrComponent ConversationManager all");
            }
            else if (arguments[0].Equals("vrDMRLTrain"))
            {
                //Ignore vrDMRLTrain messages to other agents
                if (arguments[1] == _agentID.ToString())
                {
                    //Retrieve arguments
                    DialogueAction otherDialogueAction = DialogueAction.StringToDialogueAction(arguments);
                }
            }
        }

        //THIS IS FOR DEMO - DEBUG: In the final version, the IM will request a response from the DM and the DM will look at the DState rather than listen to a message
        //This handler implements the handcrafted dialogue policy
        private void MessageActionRuleBased(object sender, VHMsg.Message args)
        {
            String[] arguments = args.s.Split(' ');
            String vrRapportMsg = "vrRapport " + _agentID + " ";
            TASK_GOAL otherTaskGoal;
            SOCIAL_GOAL otherSocialGoal;
            RAPPORT_STRATEGY_DEMO otherRapportStrategy, nextRapportStrategy = RAPPORT_STRATEGY_DEMO.NONE;
            COMMUNICATIVE_ACT otherVerbalAct;

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
                _vhmsgClient.SendMessage("vrComponent RapportManager all");
            }
            else if (arguments[0].Equals("vrGoal"))
            {
                _dialogueState.TaskGoal = (TASK_GOAL)Enum.Parse(typeof(TASK_GOAL), arguments[2]);
                _dialogueState.SocialGoal = (SOCIAL_GOAL)Enum.Parse(typeof(SOCIAL_GOAL), arguments[3]);
                _parent.updateDialogueState(_dialogueState);
            }
            else if (arguments[0].Equals("vrDialogue"))
            {
                if (arguments[2].Equals("vrIntention"))
                {
                    if (arguments[3] == _agentID.ToString())
                    {
                        //Retrieve arguments
                        otherTaskGoal = (TASK_GOAL)Enum.Parse(typeof(TASK_GOAL), arguments[4]);
                        otherSocialGoal = (SOCIAL_GOAL)Enum.Parse(typeof(SOCIAL_GOAL), arguments[5]);
                        otherRapportStrategy = (RAPPORT_STRATEGY_DEMO)Enum.Parse(typeof(RAPPORT_STRATEGY_DEMO), arguments[6]);
                        otherVerbalAct = (COMMUNICATIVE_ACT)Enum.Parse(typeof(COMMUNICATIVE_ACT), arguments[7]);

                        #region Handcrafted Rapport Selection Strategy FOR INMIND DEMO

                        //Select next action
                        //TODO: Have cases for friends. Rules below assume agents are strangers.
                        switch (otherRapportStrategy)
                        {
                            case RAPPORT_STRATEGY_DEMO.QUESTION_ELICIT_SD:
                                {

                                    switch (_dialogueState.SocialGoal)
                                    {
                                        //This agent's social goal
                                        case SOCIAL_GOAL.ENHANCE_RAPPORT:
                                            {
                                                //The other agent's goal
                                                switch (otherSocialGoal)
                                                {
                                                    case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.PRAISE, RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                    case SOCIAL_GOAL.ENHANCE_RAPPORT:   //Same as maintain
                                                    case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.PRAISE, RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE, RAPPORT_STRATEGY_DEMO.REFER_TO_SHARED_EXPERIENCE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        //Other cases here
                                    }

                                    break;
                                }
                            case RAPPORT_STRATEGY_DEMO.SELF_DISC_POSITIVE:
                                {

                                    switch (_dialogueState.SocialGoal)
                                    {
                                        //This agent's social goal
                                        case SOCIAL_GOAL.ENHANCE_RAPPORT:
                                            {
                                                //The other agent's goal
                                                switch (otherSocialGoal)
                                                {
                                                    case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.PRAISE, RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                    case SOCIAL_GOAL.ENHANCE_RAPPORT:   //Same as maintain
                                                    case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.PRAISE, RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE, RAPPORT_STRATEGY_DEMO.SELF_DISC_POSITIVE, RAPPORT_STRATEGY_DEMO.REFER_TO_SHARED_EXPERIENCE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        //Other cases here
                                    }

                                    break;
                                }
                            case RAPPORT_STRATEGY_DEMO.PRAISE:
                                {

                                    switch (_dialogueState.SocialGoal)
                                    {
                                        //This agent's social goal
                                        case SOCIAL_GOAL.ENHANCE_RAPPORT:
                                            {
                                                //The other agent's goal
                                                switch (otherSocialGoal)
                                                {
                                                    case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE, RAPPORT_STRATEGY_DEMO.SELF_DISC_POSITIVE, RAPPORT_STRATEGY_DEMO.QUESTION_ELICIT_SD };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                    case SOCIAL_GOAL.ENHANCE_RAPPORT:   //Same as maintain
                                                    case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE, RAPPORT_STRATEGY_DEMO.QUESTION_ELICIT_SD, RAPPORT_STRATEGY_DEMO.REFER_TO_SHARED_EXPERIENCE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        //Other cases here
                                    }

                                    break;
                                }
                            /*case RAPPORT_STRATEGY.RECIPROCATE_PREV_ACT: {

                                switch (_dialogueState.SocialGoal)
                                {
                                    //This agent's social goal
                                    case SOCIAL_GOAL.DESTROY_RAPPORT:
                                        {
                                            //The other agent's goal
                                            switch (otherSocialGoal)
                                            {
                                                case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                case SOCIAL_GOAL.ENHANCE_RAPPORT:
                                                case SOCIAL_GOAL.MAINTAIN_RAPPORT: { nextRapportStrategy = RAPPORT_STRATEGY.VIOLATE_NORMS; break; }
                                            }

                                            break;
                                        }
                                    //This agent's social goal
                                    case SOCIAL_GOAL.ENHANCE_RAPPORT:
                                        {
                                            //The other agent's goal
                                            switch (otherSocialGoal)
                                            {
                                                case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                    {
                                                        //List of possible rapport strategies
                                                        RAPPORT_STRATEGY[] validStrategies = { RAPPORT_STRATEGY.INIT_SELF_DISC, RAPPORT_STRATEGY.PRAISE, RAPPORT_STRATEGY.NEGATIVE_SELF_DISC, RAPPORT_STRATEGY.EMB_LAUGHTER };
                                                        nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                        break;
                                                    }
                                                case SOCIAL_GOAL.ENHANCE_RAPPORT:   //Same as maintain
                                                case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                                    {
                                                        //List of possible rapport strategies
                                                        RAPPORT_STRATEGY[] validStrategies = { RAPPORT_STRATEGY.INIT_SELF_DISC, RAPPORT_STRATEGY.PRAISE, RAPPORT_STRATEGY.NEGATIVE_SELF_DISC, RAPPORT_STRATEGY.RECIPROCAL_APPRECIATION, RAPPORT_STRATEGY.REFER_SHARED_EXP };
                                                        nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                        break;
                                                    }
                                            }

                                            break;
                                        }
                                    //This agent's social goal
                                    case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                        {
                                            //The other agent's goal
                                            switch (otherSocialGoal)
                                            {
                                                case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                    {
                                                        //List of possible rapport strategies
                                                        RAPPORT_STRATEGY[] validStrategies = { RAPPORT_STRATEGY.INIT_SELF_DISC, RAPPORT_STRATEGY.ACKNOWLEDGE, RAPPORT_STRATEGY.ADHERE_TO_NORMS };
                                                        nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                        break;
                                                    }
                                                case SOCIAL_GOAL.ENHANCE_RAPPORT:   //Same as maintain
                                                case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                                    {
                                                        //List of possible rapport strategies
                                                        RAPPORT_STRATEGY[] validStrategies = { RAPPORT_STRATEGY.INIT_SELF_DISC, RAPPORT_STRATEGY.ACKNOWLEDGE, RAPPORT_STRATEGY.ADHERE_TO_NORMS, RAPPORT_STRATEGY.RECIPROCAL_APPRECIATION, RAPPORT_STRATEGY.REFER_SHARED_EXP };
                                                        nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];
                                                        break;
                                                    }
                                            }

                                            break;
                                        }
                                }

                                break;
                            }*/
                            case RAPPORT_STRATEGY_DEMO.REFER_TO_SHARED_EXPERIENCE:
                                {
                                    switch (_dialogueState.SocialGoal)
                                    {
                                        //This agent's social goal
                                        case SOCIAL_GOAL.ENHANCE_RAPPORT:
                                            {
                                                //The other agent's goal
                                                switch (otherSocialGoal)
                                                {
                                                    case SOCIAL_GOAL.DESTROY_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.QUESTION_ELICIT_SD, RAPPORT_STRATEGY_DEMO.SELF_DISC_NEGATIVE, RAPPORT_STRATEGY_DEMO.PRAISE, RAPPORT_STRATEGY_DEMO.REFER_TO_SHARED_EXPERIENCE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                    case SOCIAL_GOAL.ENHANCE_RAPPORT:   //Same as maintain
                                                    case SOCIAL_GOAL.MAINTAIN_RAPPORT:
                                                        {
                                                            //List of possible rapport strategies
                                                            RAPPORT_STRATEGY_DEMO[] validStrategies = { RAPPORT_STRATEGY_DEMO.QUESTION_ELICIT_SD, RAPPORT_STRATEGY_DEMO.PRAISE, RAPPORT_STRATEGY_DEMO.REFER_TO_SHARED_EXPERIENCE };
                                                            nextRapportStrategy = validStrategies[rand.Next(0, validStrategies.Length - 1)];

                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                    }
                                    break;
                                }


                            case RAPPORT_STRATEGY_DEMO.NONE:
                                {
                                    break;
                                }



                            default: { Console.WriteLine("Error! Unknown rapport strategy."); break; }
                        }
                    
                        #endregion

                        //Create a task goal
                        vrRapportMsg += _dialogueState.TaskGoal.ToString() + " ";

                        //Create a social goal
                        vrRapportMsg += _dialogueState.SocialGoal.ToString() + " ";

                        vrRapportMsg += nextRapportStrategy.ToString() + " ";

                        //Send message to conversation manager
                        _vhmsgClient.SendMessage(vrRapportMsg);

                        print("SENT message: " + vrRapportMsg);
                    }
                }
            }
            else if(arguments[0].Equals("vrGlobalData")){
                if(arguments[1].Equals(_agentID.ToString()) && arguments[2].Equals("Response")){
                    switch(arguments[3]){
                        case "failed_userExists": {
                            //Try to log in instead

                            _vhmsgClient.SendMessage("vrGlobalData " + _agentID + " login " + _userName);
                            break;
                        }
                        case "failed_noUser": { 
                            //Try to create a new user instead
                            _vhmsgClient.SendMessage("vrGlobalData " + _agentID + " new " + _userName);
                            break;
                        }
                        case "userModel": { 
                            //TODO: Check if a user model was requested?
                            


                            break;
                        }
                        default: { 
                            //Print the message
                            print(args.s);
                            break;
                        }
                    }
                }
            }
        }

        private void DSUpdated(Object sender, EventArgs e)
        {
            //print("DS Notification received.");
            updatedDialogueState();
        }

        public double rewardFunction(DialogueState s, DialogueAction a)
        {
            return (s.CurrentUtterance.Equals("state4") && a.getCommunicativeAct().getCommActType() == COMMUNICATIVE_ACT.NO_COMM) ? 0 : -10;
        }

        public void updatedDialogueState()
        {
            _dialogueState = _parent.getDialogueState();
        }

        public DialogueAction nextAction()
        {
            return new DialogueAction();
        }

        //FOR TESTING ONLY, TO BE REMOVED!
        public string utterance()
        {
            return _dialogueState.CurrentUtterance;
        }

        public bool loadDialoguePolicy() { return false; }

        public bool saveDialoguePolicy() { return false; }

        //TODO: FOR DEBUG ONLY
        public void printPolicy(SparseMatrix<DialogueState, DialogueAction> policy)
        {
            Console.Write("       ");
            //Print actions
            foreach (DialogueAction act in policy.getActionList())
            {
                Console.Write(act.getCommunicativeAct().getCommActType() + " ");
            } Console.WriteLine();

            //Print State and values
            foreach (DialogueState st in policy.getStateList())
            {
                Console.Write(st.CurrentUtterance + "   ");
                double[] values = policy[st];

                for (int a = 0; a < policy.getActionList().Count; a++)
                {
                    Console.Write(values[a].ToString("#.000") + "    ");    //#.000 - 3 decimal points
                } Console.WriteLine();
            } Console.WriteLine();
        }
    }
}
