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
    public partial class Interface : Form
    {
        private VHMsg.Client _vhmsgClient;
        private string _intention, _socialGoal, _taskGoal; //TODO: If needed change string to vrConversationMgr enums
        private int num_sent_msgs;

        public Interface()
        {
            InitializeComponent();

            //Populate message type combobox
            var msgTypeDataSource = new List<comboboxItem>();

            msgTypeDataSource.Add(new comboboxItem() { Name = "Intention", Value = "Intention" });
            msgTypeDataSource.Add(new comboboxItem() { Name = "TaskGoal", Value = "TaskGoal" });
            msgTypeDataSource.Add(new comboboxItem() { Name = "SocialGoal", Value = "SocialGoal" });
            msgTypeDataSource.Add(new comboboxItem() { Name = "Spoke", Value = "Spoke" });

            //Setup data binding
            msgTypeComboBox.DataSource = msgTypeDataSource;
            msgTypeComboBox.DisplayMember = "Name";
            msgTypeComboBox.ValueMember = "Value";

            _intention = _socialGoal = _taskGoal = "NONE";

            _vhmsgClient = new VHMsg.Client();
            _vhmsgClient.OpenConnection();

            Console.WriteLine("GLOBAL DATA MODULE");
            Console.WriteLine("VHMSG_SERVER: {0}", _vhmsgClient.Server);
            Console.WriteLine("VHMSG_SCOPE: {0}", _vhmsgClient.Scope);

            //Announce availability
            _vhmsgClient.SendMessage("vrComponent GlobalData all");

            num_sent_msgs = 0;
        }

        ~Interface() {
            _vhmsgClient.CloseConnection();
            _vhmsgClient.Dispose();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            Random rand = new Random();

            string msg = "";

            switch(((comboboxItem)msgTypeComboBox.SelectedItem).Value)
            {
                case "Intention":
                    {
                        //No action if no strategy is selected
                        if (msgContentComboBox.SelectedIndex == -1) { return; }

                        //vrDialogue receiverID vrIntention TaskGoal SocialGoal RapportStrategy VerbalAct
                        msg = "vrDialogue 0 vrIntention " + _taskGoal + " " + _socialGoal + " " + _intention + " CONVENTION_GREETING";

                        break;
                    }

                case "TaskGoal":
                    {
                        //vrGoal, receiverID TaskGoal SocialGoal
                        msg = "vrGoal 0 " + (_taskGoal.Equals("RANDOM") ? (rand.NextDouble() < 0.5 ? " LEARN_GEOMETRY" : " TEACH_GEOMETRY") : _taskGoal) + " " + _socialGoal;

                        break;
                    }

                case "SocialGoal":
                    {
                        //vrGoal, receiverID TaskGoal SocialGoal
                        msg = "vrGoal 0 " + _taskGoal + " " + (_socialGoal.Equals("RANDOM") ? (rand.NextDouble() < 0.5 ? " LEARN_GEOMETRY" : " TEACH_GEOMETRY") : _socialGoal);

                        break;
                    }

                case "Spoke":
                    {
                        msg = "vrSpoke " + ((num_sent_msgs % 2) == 0 ? "Brad" : "Rachel");
                        break;
                    }

                default:
                    {
                        MessageBox.Show("Error! Invalid Message Type!" + msgContentComboBox.SelectedText);
                        return;
                    }
            }

            _vhmsgClient.SendMessage(msg);
            System.Console.WriteLine("Sent '" + msg + "' message - " + (++num_sent_msgs) + " messages so far)");

            sendTextBoxContents();
        }

        private void sendTextBoxContents()
        {
            if (!textBox1.Text.Equals(""))
            {
                //Send fake vrSpeech messages
                _vhmsgClient.SendMessage("vrSpeech start user5 user");
                _vhmsgClient.SendMessage("vrSpeech finished-speaking user5");
                _vhmsgClient.SendMessage("vrSpeech interp user5 1 1.0 normal hello");
                _vhmsgClient.SendMessage("vrSpeech emotion user5 1 1.0 normal neutral");
                _vhmsgClient.SendMessage("vrSpeech tone user5 1 1.0 normal flat");
                _vhmsgClient.SendMessage("vrSpeech asr-complete user5");

                System.Console.WriteLine("Sent 'vrSpeech' message - " + (++num_sent_msgs) + " messages so far)");
                textBox1.Text = "";
            }
        }

        private void messageTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dataSource = new List<comboboxItem>();

            switch(((comboboxItem)msgTypeComboBox.SelectedItem).Value){
                case "Intention": {
                    label2.Text = "Rapport Strategy:";

                    //Create content
                    dataSource.Add(new comboboxItem() { Name = "NONE", Value = "NONE" });
                    dataSource.Add(new comboboxItem() { Name = "INIT_SELF_DISC", Value = "INIT_SELF_DISC" });
                    dataSource.Add(new comboboxItem() { Name = "REFER_SHARED_EXP", Value = "REFER_SHARED_EXP" });
                    dataSource.Add(new comboboxItem() { Name = "INTIMATE_PERSONAL_INFO", Value = "INTIMATE_PERSONAL_INFO" });
                    dataSource.Add(new comboboxItem() { Name = "RECIPROCAL_APPRECIATION", Value = "RECIPROCAL_APPRECIATION" });
                    dataSource.Add(new comboboxItem() { Name = "ADHERE_TO_NORMS", Value = "ADHERE_TO_NORMS" });
                    dataSource.Add(new comboboxItem() { Name = "VIOLATE_NORMS", Value = "VIOLATE_NORMS" });
                    dataSource.Add(new comboboxItem() { Name = "RECIPROCATE_PREV_ACT", Value = "RECIPROCATE_PREV_ACT" });
                    dataSource.Add(new comboboxItem() { Name = "ACKNOWLEDGE", Value = "ACKNOWLEDGE" });
                    dataSource.Add(new comboboxItem() { Name = "PRAISE", Value = "PRAISE" });
                    dataSource.Add(new comboboxItem() { Name = "NEGATIVE_SELF_DISC", Value = "NEGATIVE_SELF_DISC" });
                    dataSource.Add(new comboboxItem() { Name = "EMB_LAUGHTER", Value = "EMB_LAUGHTER" });
                    dataSource.Add(new comboboxItem() { Name = "RANDOM", Value = "RANDOM" });

                    break; 
                }
                case "TaskGoal": {
                    label2.Text = "Task Goal:";

                    //Create content
                    dataSource.Add(new comboboxItem() { Name = "NONE", Value = "NONE" });
                    dataSource.Add(new comboboxItem() { Name = "TEACH_ALGEBRA", Value = "TEACH_ALGEBRA" });
                    dataSource.Add(new comboboxItem() { Name = "LEARN_ALGEBRA", Value = "LEARN_ALGEBRA" });
                    dataSource.Add(new comboboxItem() { Name = "TEACH_GEOMETRY", Value = "TEACH_GEOMETRY" });
                    dataSource.Add(new comboboxItem() { Name = "LEARN_GEOMETRY", Value = "LEARN_GEOMETRY" });
                    dataSource.Add(new comboboxItem() { Name = "RANDOM", Value = "RANDOM" });

                    break; 
                }
                case "SocialGoal": {
                        label2.Text = "Social Goal:";

                        //Create content
                        dataSource.Add(new comboboxItem() { Name = "NONE", Value = "NONE" });
                        dataSource.Add(new comboboxItem() { Name = "ENHANCE_RAPPORT", Value = "ENHANCE_RAPPORT" });
                        dataSource.Add(new comboboxItem() { Name = "MAINTAIN_RAPPORT", Value = "MAINTAIN_RAPPORT" });
                        dataSource.Add(new comboboxItem() { Name = "DESTROY_RAPPORT", Value = "DESTROY_RAPPORT" });
                        dataSource.Add(new comboboxItem() { Name = "RANDOM", Value = "RANDOM" });

                        break;
                 }
                case "Spoke": {
                    label2.Text = "Spoke:";

                    //Create content
                    dataSource.Add(new comboboxItem() { Name = "RANDOM", Value = "RANDOM" });

                    break; 
                }
            }

            //Setup data binding
            msgContentComboBox.DataSource = dataSource;
            msgContentComboBox.DisplayMember = "Name";
            msgContentComboBox.ValueMember = "Value";

            // Make the combobox readonly
            msgContentComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void msgContentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Intention Goal Spoke
            switch (((comboboxItem)msgTypeComboBox.SelectedItem).Value)
            {
                case "Intention":
                    {
                        _intention = ((comboboxItem)msgContentComboBox.SelectedItem).Value;
                        intentionLbl.Text = _intention;

                        break;
                    }
                case "TaskGoal":
                    {
                        _taskGoal = ((comboboxItem)msgContentComboBox.SelectedItem).Value;
                        taskGoalLbl.Text = _taskGoal;

                        break;
                    }
                case "SocialGoal":
                    {
                        _socialGoal = ((comboboxItem)msgContentComboBox.SelectedItem).Value;
                        socialGoalLbl.Text = _socialGoal;

                        break;
                    }
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            //Listen for 'enter' key press
            if (e.KeyChar == 13)
            {
                sendTextBoxContents();
            }
        }
    }

    public class comboboxItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
