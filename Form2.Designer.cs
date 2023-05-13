namespace LightStart_BBS_server
{
    partial class clientMsgboxSetting
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
            this.msgboxEnable = new System.Windows.Forms.CheckBox();
            this.msgboxTextCN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.msgboxTextEN = new System.Windows.Forms.TextBox();
            this.shutable = new System.Windows.Forms.CheckBox();
            this.save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // msgboxEnable
            // 
            this.msgboxEnable.AutoSize = true;
            this.msgboxEnable.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.msgboxEnable.Location = new System.Drawing.Point(12, 12);
            this.msgboxEnable.Name = "msgboxEnable";
            this.msgboxEnable.Size = new System.Drawing.Size(125, 25);
            this.msgboxEnable.TabIndex = 0;
            this.msgboxEnable.Text = "启用公告窗口";
            this.msgboxEnable.UseVisualStyleBackColor = true;
            // 
            // msgboxTextCN
            // 
            this.msgboxTextCN.Location = new System.Drawing.Point(60, 43);
            this.msgboxTextCN.Multiline = true;
            this.msgboxTextCN.Name = "msgboxTextCN";
            this.msgboxTextCN.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.msgboxTextCN.Size = new System.Drawing.Size(261, 260);
            this.msgboxTextCN.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "zh_cn:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(333, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "en_us:";
            // 
            // msgboxTextEN
            // 
            this.msgboxTextEN.Location = new System.Drawing.Point(381, 43);
            this.msgboxTextEN.Multiline = true;
            this.msgboxTextEN.Name = "msgboxTextEN";
            this.msgboxTextEN.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.msgboxTextEN.Size = new System.Drawing.Size(261, 260);
            this.msgboxTextEN.TabIndex = 3;
            // 
            // shutable
            // 
            this.shutable.AutoSize = true;
            this.shutable.Location = new System.Drawing.Point(12, 309);
            this.shutable.Name = "shutable";
            this.shutable.Size = new System.Drawing.Size(87, 21);
            this.shutable.TabIndex = 5;
            this.shutable.Text = "窗口可关闭";
            this.shutable.UseVisualStyleBackColor = true;
            // 
            // save
            // 
            this.save.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.save.Location = new System.Drawing.Point(536, 309);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(106, 50);
            this.save.TabIndex = 6;
            this.save.Text = "保存";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // clientMsgboxSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 362);
            this.Controls.Add(this.save);
            this.Controls.Add(this.shutable);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.msgboxTextEN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.msgboxTextCN);
            this.Controls.Add(this.msgboxEnable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "clientMsgboxSetting";
            this.Text = "Client Msgbox Setting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox msgboxEnable;
        private TextBox msgboxTextCN;
        private Label label1;
        private Label label2;
        private TextBox msgboxTextEN;
        private CheckBox shutable;
        private Button save;
    }
}