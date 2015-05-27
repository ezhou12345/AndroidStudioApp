namespace vrGlobalData
{
    partial class WozInterface
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
            this.rapportStrategyListBox = new System.Windows.Forms.ListBox();
            this.taskGoalListBox = new System.Windows.Forms.ListBox();
            this.speechActListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.downButton = new System.Windows.Forms.Button();
            this.sameButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.socialGoalListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // rapportStrategyListBox
            // 
            this.rapportStrategyListBox.FormattingEnabled = true;
            this.rapportStrategyListBox.Items.AddRange(new object[] {
            "ELICIT SELF DISCLOSURE",
            "POSITIVE SELF DISCLOSURE",
            "NEGATIVE SELF DISCLOSURE",
            "REFER TO SHARED EXPERIENCE",
            "ADHERE TO SOCIAL NORMS",
            "ACKNOWLEDGE",
            "PRAISE",
            "NONE"});
            this.rapportStrategyListBox.Location = new System.Drawing.Point(13, 76);
            this.rapportStrategyListBox.Name = "rapportStrategyListBox";
            this.rapportStrategyListBox.Size = new System.Drawing.Size(207, 524);
            this.rapportStrategyListBox.TabIndex = 0;
            this.rapportStrategyListBox.SelectedIndexChanged += new System.EventHandler(this.rapportStrategyListBox_SelectedIndexChanged);
            // 
            // taskGoalListBox
            // 
            this.taskGoalListBox.FormattingEnabled = true;
            this.taskGoalListBox.Items.AddRange(new object[] {
            "GET NEWS",
            "NEXT NEWS ARTICLE",
            "STOP",
            "QUIT",
            "NONE"});
            this.taskGoalListBox.Location = new System.Drawing.Point(238, 76);
            this.taskGoalListBox.Name = "taskGoalListBox";
            this.taskGoalListBox.Size = new System.Drawing.Size(207, 238);
            this.taskGoalListBox.TabIndex = 1;
            this.taskGoalListBox.SelectedIndexChanged += new System.EventHandler(this.taskGoalListBox_SelectedIndexChanged);
            // 
            // speechActListBox
            // 
            this.speechActListBox.FormattingEnabled = true;
            this.speechActListBox.Items.AddRange(new object[] {
            "GREET",
            "INFORM",
            "REQUEST",
            "NONE"});
            this.speechActListBox.Location = new System.Drawing.Point(464, 76);
            this.speechActListBox.Name = "speechActListBox";
            this.speechActListBox.Size = new System.Drawing.Size(207, 524);
            this.speechActListBox.TabIndex = 2;
            this.speechActListBox.SelectedIndexChanged += new System.EventHandler(this.speechActListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Rapport Strategy:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(234, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Task Goal:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(460, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Speech Act:";
            // 
            // sendButton
            // 
            this.sendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.Location = new System.Drawing.Point(464, 607);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(207, 54);
            this.sendButton.TabIndex = 9;
            this.sendButton.Text = "SEND";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(464, 668);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(67, 20);
            this.statusLabel.TabIndex = 10;
            this.statusLabel.Text = "Pending";
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(13, 631);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(61, 37);
            this.downButton.TabIndex = 11;
            this.downButton.Text = "DOWN";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // sameButton
            // 
            this.sameButton.Location = new System.Drawing.Point(84, 631);
            this.sameButton.Name = "sameButton";
            this.sameButton.Size = new System.Drawing.Size(66, 37);
            this.sameButton.TabIndex = 12;
            this.sameButton.Text = "SAME";
            this.sameButton.UseVisualStyleBackColor = true;
            this.sameButton.Click += new System.EventHandler(this.sameButton_Click);
            // 
            // upButton
            // 
            this.upButton.Location = new System.Drawing.Point(159, 631);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(61, 37);
            this.upButton.TabIndex = 13;
            this.upButton.Text = "UP";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 608);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(131, 20);
            this.label5.TabIndex = 14;
            this.label5.Text = "Rapport Change:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 676);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(117, 20);
            this.label6.TabIndex = 15;
            this.label6.Text = "Rapport: Same";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(234, 339);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Social Goal:";
            // 
            // socialGoalListBox
            // 
            this.socialGoalListBox.FormattingEnabled = true;
            this.socialGoalListBox.Items.AddRange(new object[] {
            "BREAK RAPPORT",
            "MAINTAIN RAPPORT",
            "ENHANCE RAPPORT",
            "NONE"});
            this.socialGoalListBox.Location = new System.Drawing.Point(238, 362);
            this.socialGoalListBox.Name = "socialGoalListBox";
            this.socialGoalListBox.Size = new System.Drawing.Size(207, 238);
            this.socialGoalListBox.TabIndex = 3;
            this.socialGoalListBox.SelectedIndexChanged += new System.EventHandler(this.socialGoalListBox_SelectedIndexChanged);
            // 
            // WozInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 705);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.sameButton);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.socialGoalListBox);
            this.Controls.Add(this.speechActListBox);
            this.Controls.Add(this.taskGoalListBox);
            this.Controls.Add(this.rapportStrategyListBox);
            this.Name = "WozInterface";
            this.Text = "Understanding";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox rapportStrategyListBox;
        private System.Windows.Forms.ListBox taskGoalListBox;
        private System.Windows.Forms.ListBox speechActListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button sameButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox socialGoalListBox;
    }
}