using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListViewItem;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ComboBox = System.Windows.Forms.ComboBox;
using ListView = System.Windows.Forms.ListView;

namespace LightStart_BBS_server
{
    public partial class userManager : Form
    {
        public userManager()
        {
            InitializeComponent();
            foreach(string str in Form1.Constants.USERGROUPS)
            {
                groupBox.Items.Add(str);
            }
            RefreshList();
        }

        void RefreshList()
        {
            int selectedIndex = 0;
            if (List.SelectedIndices.Count>0)
                selectedIndex = List.SelectedIndices[0];
            List.Clear();
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            list = Form1.manager_user.SelectAll();
            List.Columns.Add("ID", 80);
            List.Columns.Add("Name", 100);
            List.Columns.Add("Invitation Key", 100);
            List.Columns.Add("Group", 60);
            foreach (var item in list)
            {
                string id = item["id"];
                string name = item["name"];
                string sharedKey = item["sharedKey"];
                int group = 0;
                int.TryParse(item["usergroup"], out group);
                ListViewItem listViewItem = new ListViewItem(id);
                listViewItem.SubItems.Add(name);
                listViewItem.SubItems.Add(sharedKey);
                try
                {
                    listViewItem.SubItems.Add(Form1.Constants.USERGROUPS[group]);
                }
                catch
                {
                    listViewItem.SubItems.Add(Form1.Constants.USERGROUPS[0]);
                }
                List.Items.Add(listViewItem);
            }
            List.SelectedIndices.Clear();
            if (list.Count > 0)
                List.SelectedIndices.Add(selectedIndex);
        }

        private void List_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ListView.SelectedIndexCollection indexes = this.List.SelectedIndices;
                if (indexes.Count > 0)
                {
                    int index = indexes[0];
                    string id = this.List.Items[index].SubItems[0].Text;
                    string name = this.List.Items[index].SubItems[1].Text;
                    string invitationKey = this.List.Items[index].SubItems[2].Text;
                    string groupStr = this.List.Items[index].SubItems[3].Text;

                    IDInput.Text = id;
                    NameInput.Text = name;
                    InvitationKeyInput.Text = invitationKey;
                    int group = Array.IndexOf(Form1.Constants.USERGROUPS, groupStr);
                    groupBox.SelectedIndex = group;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败: " + ex.Message, "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void confirm_Click(object sender, EventArgs e)
        {
            if(List.SelectedIndices.Count == 1)
            {
                int group = groupBox.SelectedIndex;
                string id = this.List.Items[List.SelectedIndices[0]].SubItems[0].Text;
                Form1.setUserGroup(id, group);
                RefreshList();
            }
        }

        bool VerifyAction(
            byte difficulty, // 难度，0=纯数字 1=纯字母 2=字母+数字 else=null
            ushort length, // 长度，必须>=1
            string msg = "" // 额外提示
            )
        {
            if (difficulty > 2 || length < 1)
            {
                return false;
            }

            string key = Form1.RandomStr(length, difficulty);
            int riskSeverity = (int)(Math.Round(0.056 * Math.Pow(length , 2) + 0.167 * length + 0.778)) + //length计算风险程度:y=0.056x^2+0.167x+0.778
                (int)(Math.Round(0.5 * Math.Pow(difficulty , 2) + 1.5 * difficulty + 1)); //difficulty计算风险程度:y=0.5x^2+1.5x+1
            string message = "";
            if (msg != "")
                message += $"{msg}\n\n";
            message += $"您的本次操作风险程度: {riskSeverity}\n请输入以下字符串以继续您的操作:\n{key}";
            string result = Interaction.InputBox(message, "提示", "", -1, -1);

            if (result.Length > 0 && result != key)// 若输入内容且点击了"确定"按钮且密码错误 (防止直接关闭或点击"取消"后出现窗口)
            {
                MessageBox.Show("已取消", "提示");
                return false;
            }
            return (result == key);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = this.List.SelectedIndices;
            if (indexes.Count > 0)
            {
                int index = indexes[0];
                string id = this.List.Items[index].SubItems[0].Text;
                if (VerifyAction(2, 8, $"是否删除用户 {id}"))
                {
                    Form1.deleteUser(id);
                    RefreshList();
                    MessageBox.Show("已删除", "提示");
                }
            }
        }

        private void cofirmID_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = this.List.SelectedIndices;
            if (indexes.Count > 0)
            {
                int index = indexes[0];
                string id = this.List.Items[index].SubItems[0].Text;
                string id_after = IDInput.Text;
                if (VerifyAction(1, 5, $"是否修改用户 {id} 的 ID 为 {id_after}"))
                {
                    Form1.changeUserID(id, id_after);
                    RefreshList();
                    MessageBox.Show("已修改", "提示");
                }
            }
        }

        private void confirmName_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = this.List.SelectedIndices;
            if (indexes.Count > 0)
            {
                int index = indexes[0];
                string id = this.List.Items[index].SubItems[0].Text;
                string name_after = NameInput.Text;
                if (VerifyAction(0, 6, $"是否修改用户 {id} 的 Name 为 {name_after}"))
                {
                    Form1.changeUserName(id, name_after);
                    RefreshList();
                    MessageBox.Show("已修改", "提示");
                }
            }
        }

        private void cofirmInvitationKey_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = this.List.SelectedIndices;
            if (indexes.Count > 0)
            {
                int index = indexes[0];
                string id = this.List.Items[index].SubItems[0].Text;
                string invitationKey_after = InvitationKeyInput.Text;
                if (VerifyAction(0, 5, $"是否修改用户 {id} 的 InvitationKey 为 {invitationKey_after}"))
                {
                    Form1.changeUserInvitationKey(id, invitationKey_after);
                    RefreshList();
                    MessageBox.Show("已修改", "提示");
                }
            }
        }
    }


}
