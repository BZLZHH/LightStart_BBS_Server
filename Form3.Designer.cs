using System.Windows.Forms;

namespace LightStart_BBS_server
{
    partial class manager
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
            this.List_user = new System.Windows.Forms.ListView();
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
            this.List_forumBoards = new System.Windows.Forms.ListView();
            this.fb_ChangeTitle = new System.Windows.Forms.Button();
            this.fb_EN_input = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fb_ZH_input = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.swapItem1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.swapItem2 = new System.Windows.Forms.TextBox();
            this.fb_swap = new System.Windows.Forms.Button();
            this.fb_delete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // List_user
            // 
            this.List_user.FullRowSelect = true;
            this.List_user.GridLines = true;
            this.List_user.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.List_user.Location = new System.Drawing.Point(4, 2);
            this.List_user.MultiSelect = false;
            this.List_user.Name = "List_user";
            this.List_user.Size = new System.Drawing.Size(379, 387);
            this.List_user.TabIndex = 1;
            this.List_user.UseCompatibleStateImageBehavior = false;
            this.List_user.View = System.Windows.Forms.View.Details;
            this.List_user.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 481);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Group:";
            // 
            // confirmGroup
            // 
            this.confirmGroup.Location = new System.Drawing.Point(267, 478);
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
            this.groupBox.Location = new System.Drawing.Point(61, 478);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(200, 25);
            this.groupBox.TabIndex = 7;
            // 
            // delete
            // 
            this.delete.Location = new System.Drawing.Point(321, 478);
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(64, 25);
            this.delete.TabIndex = 8;
            this.delete.Text = "删除用户";
            this.delete.UseVisualStyleBackColor = true;
            this.delete.Click += new System.EventHandler(this.delete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 399);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "ID:";
            // 
            // IDInput
            // 
            this.IDInput.Location = new System.Drawing.Point(61, 396);
            this.IDInput.Name = "IDInput";
            this.IDInput.Size = new System.Drawing.Size(200, 23);
            this.IDInput.TabIndex = 10;
            // 
            // cofirmID
            // 
            this.cofirmID.Location = new System.Drawing.Point(267, 395);
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
            this.confirmName.Location = new System.Drawing.Point(267, 422);
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
            this.NameInput.Location = new System.Drawing.Point(61, 423);
            this.NameInput.Name = "NameInput";
            this.NameInput.Size = new System.Drawing.Size(200, 23);
            this.NameInput.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 425);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Name:";
            // 
            // cofirmInvitationKey
            // 
            this.cofirmInvitationKey.Location = new System.Drawing.Point(267, 450);
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
            this.InvitationKeyInput.Location = new System.Drawing.Point(61, 450);
            this.InvitationKeyInput.Name = "InvitationKeyInput";
            this.InvitationKeyInput.Size = new System.Drawing.Size(200, 23);
            this.InvitationKeyInput.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 454);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 17);
            this.label4.TabIndex = 15;
            this.label4.Text = "InvKey:";
            // 
            // List_forumBoards
            // 
            this.List_forumBoards.FullRowSelect = true;
            this.List_forumBoards.GridLines = true;
            this.List_forumBoards.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.List_forumBoards.Location = new System.Drawing.Point(403, 2);
            this.List_forumBoards.MultiSelect = false;
            this.List_forumBoards.Name = "List_forumBoards";
            this.List_forumBoards.Size = new System.Drawing.Size(302, 387);
            this.List_forumBoards.TabIndex = 18;
            this.List_forumBoards.UseCompatibleStateImageBehavior = false;
            this.List_forumBoards.View = System.Windows.Forms.View.Details;
            this.List_forumBoards.SelectedIndexChanged += new System.EventHandler(this.List_forumBoards_SelectedIndexChanged);
            this.List_forumBoards.MouseDown += new System.Windows.Forms.MouseEventHandler(this.List_forumBoards_MouseDown);
            // 
            // fb_ChangeTitle
            // 
            this.fb_ChangeTitle.Location = new System.Drawing.Point(657, 410);
            this.fb_ChangeTitle.Name = "fb_ChangeTitle";
            this.fb_ChangeTitle.Size = new System.Drawing.Size(48, 25);
            this.fb_ChangeTitle.TabIndex = 24;
            this.fb_ChangeTitle.Text = "确定";
            this.fb_ChangeTitle.UseMnemonic = false;
            this.fb_ChangeTitle.UseVisualStyleBackColor = true;
            this.fb_ChangeTitle.Click += new System.EventHandler(this.fb_ChangeTitle_Click);
            // 
            // fb_EN_input
            // 
            this.fb_EN_input.Location = new System.Drawing.Point(436, 424);
            this.fb_EN_input.Name = "fb_EN_input";
            this.fb_EN_input.Size = new System.Drawing.Size(214, 23);
            this.fb_EN_input.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(403, 427);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 17);
            this.label5.TabIndex = 22;
            this.label5.Text = "EN:";
            // 
            // fb_ZH_input
            // 
            this.fb_ZH_input.Location = new System.Drawing.Point(436, 396);
            this.fb_ZH_input.Name = "fb_ZH_input";
            this.fb_ZH_input.Size = new System.Drawing.Size(214, 23);
            this.fb_ZH_input.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(403, 399);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "ZH:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(403, 454);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 25;
            this.label7.Text = "Swap ID";
            // 
            // swapItem1
            // 
            this.swapItem1.Location = new System.Drawing.Point(465, 452);
            this.swapItem1.Name = "swapItem1";
            this.swapItem1.Size = new System.Drawing.Size(70, 23);
            this.swapItem1.TabIndex = 26;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(542, 455);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 17);
            this.label8.TabIndex = 27;
            this.label8.Text = "with";
            // 
            // swapItem2
            // 
            this.swapItem2.Location = new System.Drawing.Point(580, 452);
            this.swapItem2.Name = "swapItem2";
            this.swapItem2.Size = new System.Drawing.Size(70, 23);
            this.swapItem2.TabIndex = 28;
            // 
            // fb_swap
            // 
            this.fb_swap.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.fb_swap.Location = new System.Drawing.Point(657, 451);
            this.fb_swap.Name = "fb_swap";
            this.fb_swap.Size = new System.Drawing.Size(48, 25);
            this.fb_swap.TabIndex = 29;
            this.fb_swap.Text = "确定";
            this.fb_swap.UseMnemonic = false;
            this.fb_swap.UseVisualStyleBackColor = true;
            this.fb_swap.Click += new System.EventHandler(this.fb_swap_Click);
            // 
            // fb_delete
            // 
            this.fb_delete.Location = new System.Drawing.Point(641, 478);
            this.fb_delete.Name = "fb_delete";
            this.fb_delete.Size = new System.Drawing.Size(64, 25);
            this.fb_delete.TabIndex = 30;
            this.fb_delete.Text = "删除板块";
            this.fb_delete.UseVisualStyleBackColor = true;
            this.fb_delete.Click += new System.EventHandler(this.fb_delete_Click);
            // 
            // manager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 506);
            this.Controls.Add(this.fb_delete);
            this.Controls.Add(this.fb_swap);
            this.Controls.Add(this.swapItem2);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.swapItem1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.fb_ChangeTitle);
            this.Controls.Add(this.fb_EN_input);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.fb_ZH_input);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.List_forumBoards);
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
            this.Controls.Add(this.List_user);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "manager";
            this.Text = "Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListView List_user;
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
        private ListView List_forumBoards;
        private Button fb_ChangeTitle;
        private TextBox fb_EN_input;
        private Label label5;
        private TextBox fb_ZH_input;
        private Label label6;
        private Label label7;
        private TextBox swapItem1;
        private Label label8;
        private TextBox swapItem2;
        private Button fb_swap;
        private Button fb_delete;
    }
}