namespace vrGlobalData
{
    partial class Interface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interface));
            this.sendButton = new System.Windows.Forms.Button();
            this.msgContentComboBox = new System.Windows.Forms.ComboBox();
            this.msgTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.intentionLbl = new System.Windows.Forms.Label();
            this.socialGoalLbl = new System.Windows.Forms.Label();
            this.taskGoalLbl = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(209, 177);
            this.sendButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(56, 19);
            this.sendButton.TabIndex = 0;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // msgContentComboBox
            // 
            this.msgContentComboBox.FormattingEnabled = true;
            this.msgContentComboBox.Items.AddRange(new object[] {
            "INIT_SELF_DISC",
            "REFER_SHARED_EXP",
            "INTIMATE_PERSONAL_INFO",
            "RECIPROCAL_APPRECIATION",
            "ADHERE_TO_NORMS",
            "VIOLATE_NORMS",
            "RECIPROCATE_PREV_ACT",
            "ACKNOWLEDGE",
            "PRAISE",
            "NEGATIVE_SELF_DISC",
            "EMB_LAUGHTER"});
            this.msgContentComboBox.Location = new System.Drawing.Point(9, 69);
            this.msgContentComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.msgContentComboBox.Name = "msgContentComboBox";
            this.msgContentComboBox.Size = new System.Drawing.Size(194, 21);
            this.msgContentComboBox.TabIndex = 1;
            this.msgContentComboBox.SelectedIndexChanged += new System.EventHandler(this.msgContentComboBox_SelectedIndexChanged);
            // 
            // msgTypeComboBox
            // 
            this.msgTypeComboBox.FormattingEnabled = true;
            this.msgTypeComboBox.Items.AddRange(new object[] {
            "Intention",
            "TaskGoal",
            "SocialGoal",
            "Spoke"});
            this.msgTypeComboBox.Location = new System.Drawing.Point(9, 21);
            this.msgTypeComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.msgTypeComboBox.Name = "msgTypeComboBox";
            this.msgTypeComboBox.Size = new System.Drawing.Size(92, 21);
            this.msgTypeComboBox.TabIndex = 2;
            this.msgTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.messageTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Message Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Message Content:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label3.Location = new System.Drawing.Point(6, 103);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Intention:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Location = new System.Drawing.Point(6, 134);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Social Goal:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.Location = new System.Drawing.Point(5, 118);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Task Goal:";
            // 
            // intentionLbl
            // 
            this.intentionLbl.AutoSize = true;
            this.intentionLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.intentionLbl.Location = new System.Drawing.Point(81, 103);
            this.intentionLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.intentionLbl.Name = "intentionLbl";
            this.intentionLbl.Size = new System.Drawing.Size(38, 13);
            this.intentionLbl.TabIndex = 8;
            this.intentionLbl.Text = "NONE";
            // 
            // socialGoalLbl
            // 
            this.socialGoalLbl.AutoSize = true;
            this.socialGoalLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.socialGoalLbl.Location = new System.Drawing.Point(81, 134);
            this.socialGoalLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.socialGoalLbl.Name = "socialGoalLbl";
            this.socialGoalLbl.Size = new System.Drawing.Size(38, 13);
            this.socialGoalLbl.TabIndex = 9;
            this.socialGoalLbl.Text = "NONE";
            // 
            // taskGoalLbl
            // 
            this.taskGoalLbl.AutoSize = true;
            this.taskGoalLbl.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.taskGoalLbl.Location = new System.Drawing.Point(81, 118);
            this.taskGoalLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.taskGoalLbl.Name = "taskGoalLbl";
            this.taskGoalLbl.Size = new System.Drawing.Size(38, 13);
            this.taskGoalLbl.TabIndex = 10;
            this.taskGoalLbl.Text = "NONE";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 178);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(194, 20);
            this.textBox1.TabIndex = 11;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(textBox1_KeyPress);
            // 
            // Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(276, 207);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.taskGoalLbl);
            this.Controls.Add(this.socialGoalLbl);
            this.Controls.Add(this.intentionLbl);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.msgTypeComboBox);
            this.Controls.Add(this.msgContentComboBox);
            this.Controls.Add(this.sendButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Interface";
            this.Text = "Intention Understanding";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ComboBox msgContentComboBox;
        private System.Windows.Forms.ComboBox msgTypeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label intentionLbl;
        private System.Windows.Forms.Label socialGoalLbl;
        private System.Windows.Forms.Label taskGoalLbl;
        private System.Windows.Forms.TextBox textBox1;
    }
}