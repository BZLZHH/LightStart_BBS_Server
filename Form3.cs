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
    public partial class userGroupManager : Form
    {
        public userGroupManager()
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
            List.Columns.Add("ID", 100);
            List.Columns.Add("Group", 100);
            foreach (var item in list)
            {
                string id = item["id"];
                int group = 0;
                int.TryParse(item["usergroup"], out group);
                ListViewItem listViewItem = new ListViewItem(id);
                listViewItem.SubItems.Add(Form1.Constants.USERGROUPS[group]);
                List.Items.Add(listViewItem);
            }
            List.SelectedIndices.Clear();
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
                    string groupStr = this.List.Items[index].SubItems[1].Text;
                    int group = Array.IndexOf(Form1.Constants.USERGROUPS, groupStr);
                    groupBox.SelectedIndex = group;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败！\n" + ex.Message, "提示", MessageBoxButtons.OK,
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
    }


}
