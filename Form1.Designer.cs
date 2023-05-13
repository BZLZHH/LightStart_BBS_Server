namespace LightStart_BBS_server
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.restart = new System.Windows.Forms.Button();
            this.Log = new System.Windows.Forms.TextBox();
            this.GetUsersInfo = new System.Windows.Forms.Button();
            this.clientMsgboxChange = new System.Windows.Forms.Button();
            this.userGroupChange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // restart
            // 
            this.restart.Enabled = false;
            this.restart.Location = new System.Drawing.Point(685, 388);
            this.restart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.restart.Name = "restart";
            this.restart.Size = new System.Drawing.Size(103, 42);
            this.restart.TabIndex = 0;
            this.restart.Text = "Restart Server";
            this.restart.UseVisualStyleBackColor = true;
            this.restart.Visible = false;
            this.restart.Click += new System.EventHandler(this.restart_Click);
            // 
            // Log
            // 
            this.Log.Location = new System.Drawing.Point(12, 12);
            this.Log.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Log.Multiline = true;
            this.Log.Name = "Log";
            this.Log.ReadOnly = true;
            this.Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Log.Size = new System.Drawing.Size(667, 424);
            this.Log.TabIndex = 1;
            // 
            // GetUsersInfo
            // 
            this.GetUsersInfo.Location = new System.Drawing.Point(685, 12);
            this.GetUsersInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.GetUsersInfo.Name = "GetUsersInfo";
            this.GetUsersInfo.Size = new System.Drawing.Size(103, 42);
            this.GetUsersInfo.TabIndex = 2;
            this.GetUsersInfo.Text = "Get Users Info";
            this.GetUsersInfo.UseVisualStyleBackColor = true;
            this.GetUsersInfo.Click += new System.EventHandler(this.GetUsersInfo_Click);
            // 
            // clientMsgboxChange
            // 
            this.clientMsgboxChange.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.clientMsgboxChange.Location = new System.Drawing.Point(685, 60);
            this.clientMsgboxChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.clientMsgboxChange.Name = "clientMsgboxChange";
            this.clientMsgboxChange.Size = new System.Drawing.Size(103, 42);
            this.clientMsgboxChange.TabIndex = 3;
            this.clientMsgboxChange.Text = "Client Msgbox Setting";
            this.clientMsgboxChange.UseVisualStyleBackColor = true;
            this.clientMsgboxChange.Click += new System.EventHandler(this.clientMsgboxChange_Click);
            // 
            // userGroupChange
            // 
            this.userGroupChange.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.userGroupChange.Location = new System.Drawing.Point(684, 108);
            this.userGroupChange.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.userGroupChange.Name = "userGroupChange";
            this.userGroupChange.Size = new System.Drawing.Size(103, 42);
            this.userGroupChange.TabIndex = 4;
            this.userGroupChange.Text = "User Group Manager";
            this.userGroupChange.UseVisualStyleBackColor = true;
            this.userGroupChange.Click += new System.EventHandler(this.userGroupChange_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 442);
            this.Controls.Add(this.userGroupChange);
            this.Controls.Add(this.clientMsgboxChange);
            this.Controls.Add(this.GetUsersInfo);
            this.Controls.Add(this.Log);
            this.Controls.Add(this.restart);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "LightStart BBS Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button restart;
        public TextBox Log;
        private Button GetUsersInfo;
        private Button clientMsgboxChange;
        private Button userGroupChange;
    }
}