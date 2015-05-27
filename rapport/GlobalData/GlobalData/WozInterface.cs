using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vrGlobalData
{
    public partial class WozInterface : Form
    {
        private VHMsg.Client _vhmsgClient;
        private int _rapportState;

        public WozInterface()
        {
            InitializeComponent();

            _rapportState = 3;  //Range: [0, 7]

            //Initialize selections to NONE, to avoid sending empty messages
            rapportStrategyListBox.SelectedItem = rapportStrategyListBox.Items[rapportStrategyListBox.Items.Count-1];
            taskGoalListBox.SelectedItem = taskGoalListBox.Items[taskGoalListBox.Items.Count - 1];
            socialGoalListBox.SelectedItem = socialGoalListBox.Items[socialGoalListBox.Items.Count - 1];
            speechActListBox.SelectedItem = speechActListBox.Items[speechActListBox.Items.Count - 1];

            _vhmsgClient = new VHMsg.Client();
            _vhmsgClient.OpenConnection();

            Console.WriteLine("GLOBAL DATA MODULE");
            Console.WriteLine("VHMSG_SERVER: {0}", _vhmsgClient.Server);
            Console.WriteLine("VHMSG_SCOPE: {0}", _vhmsgClient.Scope);

            //Announce availability
            _vhmsgClient.SendMessage("vrComponent GlobalData all");
        }

        ~WozInterface() {
            _vhmsgClient.CloseConnection();
            _vhmsgClient.Dispose();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string _rapportStrategy = "", _socialGoal = "", _taskGoal = "", _speechAct = ""; //TODO: If needed, change string to vrConversationMgr enums

            //Retrieve arguments from woz interface
            switch(rapportStrategyListBox.SelectedItem.ToString()){
                case "ELICIT SELF DISCLOSURE":{
                    _rapportStrategy = "INIT_SELF_DISC";
                    break;
                }
                case "POSITIVE SELF DISCLOSURE":{
                    _rapportStrategy = "INTIMATE_PERSONAL_INFO";
                    break;
                }
                case "NEGATIVE SELF DISCLOSURE":{
                    _rapportStrategy = "NEGATIVE_SELF_DISC";
                    break;
                }
                case "REFER TO SHARED EXPERIENCE":{
                    _rapportStrategy = "REFER_SHARED_EXP";
                    break;
                }
                case "ADHERE TO SOCIAL NORMS": {
                    _rapportStrategy = "ADHERE_TO_NORMS";
                    break;
                }
                case "ACKNOWLEDGE":{
                    _rapportStrategy = "ACKNOWLEDGE";
                    break;
                }
                case "PRAISE":{
                    _rapportStrategy = "PRAISE";
                    break;
                }
                case "NONE": {
                    _rapportStrategy = "NONE";
                    break;
                }
                default:{
                    _rapportStrategy = "";
                    break;
                }
            }

            switch(taskGoalListBox.SelectedItem.ToString()){
                case "GET NEWS":{
                    _taskGoal = "GET_NEWS";
                    break;
                }
                case "NEXT NEWS ARTICLE":{
                    _taskGoal = "NEXT_ARTICLE";
                    break;
                }
                case "STOP":{
                    _taskGoal = "STOP";
                    break;
                }
                case "QUIT":{
                    _taskGoal = "QUIT";
                    break;
                }
                case "NONE": {
                    _taskGoal = "NONE";
                    break;
                }
                default: {
                    _taskGoal = "";
                    break;
                }
            }

            switch(socialGoalListBox.SelectedItem.ToString()){
                case "BREAK RAPPORT":{
                    _socialGoal = "BREAK_RAPPORT";
                    break;
                }
                case "MAINTAIN RAPPORT":{
                    _socialGoal = "MAINTAIN_RAPPORT";
                    break;
                }
                case "ENHANCE RAPPORT":{
                    _socialGoal = "ENHANCE_RAPPORT";
                    break;
                }
                case "NONE": {
                    _socialGoal = "NONE";
                    break;
                }
                default: {
                    _socialGoal = "";
                    break;
                }
            }

            switch(speechActListBox.SelectedItem.ToString()){
                case "GREET":{
                    _speechAct = "GREET";
                    break;
                }
                case "INFORM":{
                    _speechAct = "INFORM";
                    break;
                }
                case "REQUEST":{
                    _speechAct = "REQUEST";
                    break;
                }
                case "NONE": {
                    _speechAct = "NONE";
                    break;
                }
                default: {
                    _speechAct = "";
                    break;
                }
            }

            //vrDialogue receiverID vrIntention TaskGoal SocialGoal RapportStrategy VerbalAct
            string msg = "vrDialogue 0 vrIntention " + _taskGoal + " " + _socialGoal + " " + _rapportStrategy + " " + _speechAct;

            _vhmsgClient.SendMessage(msg);

            msg = "vrRapport 0 state " + _rapportState.ToString();

            _vhmsgClient.SendMessage(msg);

            statusLabel.Text = "Message Sent!";
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";

            //Decrease rapport
            if (_rapportState > 0) { _rapportState--; }
        }

        private void sameButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";

            //Increase rapport
            if (_rapportState < 7) { _rapportState++; }
        }

        private void rapportStrategyListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";
        }

        private void taskGoalListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";
        }

        private void socialGoalListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";
        }

        private void speechActListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            statusLabel.Text = "Pending...";
        }
    }
}
