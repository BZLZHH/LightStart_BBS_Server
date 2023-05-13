using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightStart_BBS_server
{
    public partial class clientMsgboxSetting : Form
    {
        public clientMsgboxSetting()
        {
            InitializeComponent();
            ReadConfig();
        }

        MsgBoxConfig ReadConfig()
        {
            const string jsonFilePath = "msgboxConfig.json"; // 文件路径
            MsgBoxConfig config = new MsgBoxConfig(); // 创建一个新的MsgBoxConfig实例

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath); // 读取json文件内容
                config = JsonConvert.DeserializeObject<MsgBoxConfig>(jsonContent); // 解析json文件内容，存入MsgBoxConfig实例中
                                                                                   // 将MsgBoxConfig实例中保存的值显示在WinForm控件中
            }
            else
            {
                config.enable = false;
                config.shutable = true;
                config.text_en = "";
                config.text_zh = "";
            }

            msgboxEnable.Checked = config.enable;
            msgboxTextCN.Text = config.text_zh;
            msgboxTextEN.Text = config.text_en;
            shutable.Checked = config.shutable;
            return config;
        }

        void SaveConfig()
        {
            // 将WinForm控件中的值赋到MsgBoxConfig实例中
            MsgBoxConfig config = new MsgBoxConfig();
            config.enable = msgboxEnable.Checked;
            config.text_zh = msgboxTextCN.Text;
            config.text_en = msgboxTextEN.Text;
            config.shutable = shutable.Checked;
            // 将MsgBoxConfig实例转换成json格式，并保存到文件中
            File.WriteAllText("msgboxConfig.json", JsonConvert.SerializeObject(config));
        }

        private void save_Click(object sender, EventArgs e)
        {
            SaveConfig();
            MessageBox.Show("已保存");
        }
    }

    public class MsgBoxConfig
    {
        public string text_zh;
        public string text_en;
        public bool enable;
        public bool shutable;
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
