using System.Windows.Forms;

namespace LightStart_BBS_server
{
    partial class userManager
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
            this.List = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.confirmGroup = new System.Windows.Forms.Button();
            this.groupBox = new System.Windows.Forms.ComboBox();
            this.delete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.IDInput = new System.Windows.Forms.TextBox();
            this.cofirmID = new System.Windows.Forms.Button();
            this.confirmName = new System.Windows.Forms.Button();
            this.NameInput = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cofirmInvitationKey = new System.Windows.Forms.Button();
            this.InvitationKeyInput = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.FullRowSelect = true;
            this.List.GridLines = true;
            this.List.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.List.Location = new System.Drawing.Point(4, 2);
            this.List.MultiSelect = false;
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(433, 466);
            this.List.TabIndex = 1;
            this.List.UseCompatibleStateImageBehavior = false;
            this.List.View = System.Windows.Forms.View.Details;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(443, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Group:";
            // 
            // confirmGroup
            // 
            this.confirmGroup.Location = new System.Drawing.Point(679, 150);
            this.confirmGroup.Name = "confirmGroup";
            this.confirmGroup.Size = new System.Drawing.Size(48, 25);
            this.confirmGroup.TabIndex = 6;
            this.confirmGroup.Text = "确定";
            this.confirmGroup.UseMnemonic = false;
            this.confirmGroup.UseVisualStyleBackColor = true;
            this.confirmGroup.Click += new System.EventHandler(this.confirm_Click);
            // 
            // groupBox
            // 
            this.groupBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupBox.FormattingEnabled = true;
            this.groupBox.Location = new System.Drawing.Point(443, 150);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(230, 25);
            this.groupBox.TabIndex = 7;
            // 
            // delete
            // 
            this.delete.Location = new System.Drawing.Point(648, 423);
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(79, 45);
            this.delete.TabIndex = 8;
            this.delete.Text = "删除用户";
            this.delete.UseVisualStyleBackColor = true;
            this.delete.Click += new System.EventHandler(this.delete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(443, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "ID:";
            // 
            // IDInput
            // 
            this.IDInput.Location = new System.Drawing.Point(473, 6);
            this.IDInput.Name = "IDInput";
            this.IDInput.Size = new System.Drawing.Size(200, 23);
            this.IDInput.TabIndex = 10;
            // 
            // cofirmID
            // 
            this.cofirmID.Location = new System.Drawing.Point(679, 5);
            this.cofirmID.Name = "cofirmID";
            this.cofirmID.Size = new System.Drawing.Size(48, 25);
            this.cofirmID.TabIndex = 11;
            this.cofirmID.Text = "确定";
            this.cofirmID.UseMnemonic = false;
            this.cofirmID.UseVisualStyleBackColor = true;
            this.cofirmID.Click += new System.EventHandler(this.cofirmID_Click);
            // 
            // confirmName
            // 
            this.confirmName.Location = new System.Drawing.Point(679, 50);
            this.confirmName.Name = "confirmName";
            this.confirmName.Size = new System.Drawing.Size(48, 25);
            this.confirmName.TabIndex = 14;
            this.confirmName.Text = "确定";
            this.confirmName.UseMnemonic = false;
            this.confirmName.UseVisualStyleBackColor = true;
            this.confirmName.Click += new System.EventHandler(this.confirmName_Click);
            // 
            // NameInput
            // 
            this.NameInput.Location = new System.Drawing.Point(443, 52);
            this.NameInput.Name = "NameInput";
            this.NameInput.Size = new System.Drawing.Size(230, 23);
            this.NameInput.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(443, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Name:";
            // 
            // cofirmInvitationKey
            // 
            this.cofirmInvitationKey.Location = new System.Drawing.Point(679, 96);
            this.cofirmInvitationKey.Name = "cofirmInvitationKey";
            this.cofirmInvitationKey.Size = new System.Drawing.Size(48, 25);
            this.cofirmInvitationKey.TabIndex = 17;
            this.cofirmInvitationKey.Text = "确定";
            this.cofirmInvitationKey.UseMnemonic = false;
            this.cofirmInvitationKey.UseVisualStyleBackColor = true;
            this.cofirmInvitationKey.Click += new System.EventHandler(this.cofirmInvitationKey_Click);
            // 
            // InvitationKeyInput
            // 
            this.InvitationKeyInput.Location = new System.Drawing.Point(443, 98);
            this.InvitationKeyInput.Name = "InvitationKeyInput";
            this.InvitationKeyInput.Size = new System.Drawing.Size(230, 23);
            this.InvitationKeyInput.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(443, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 17);
            this.label4.TabIndex = 15;
            this.label4.Text = "InvitationKey:";
            // 
            // userManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 470);
            this.Controls.Add(this.cofirmInvitationKey);
            this.Controls.Add(this.InvitationKeyInput);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.confirmName);
            this.Controls.Add(this.NameInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cofirmID);
            this.Controls.Add(this.IDInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.delete);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.confirmGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.List);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "userManager";
            this.Text = "User Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListView List;
        private Label label2;
        private Button confirmGroup;
        private ComboBox groupBox;
        private Button delete;
        private Label label1;
        private TextBox IDInput;
        private Button cofirmID;
        private Button confirmName;
        private TextBox NameInput;
        private Label label3;
        private Button cofirmInvitationKey;
        private TextBox InvitationKeyInput;
        private Label label4;
    }
}