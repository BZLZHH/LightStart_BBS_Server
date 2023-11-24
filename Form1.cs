using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using XSystem.Security.Cryptography;
using XAct;
using SevenZip;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using System.Collections.Generic;
using System.Data;
using System.Text.Json.Nodes;
using System.IO.Compression;
using CompressionLevel = SevenZip.CompressionLevel;
using System;
using CompressionMode = SevenZip.CompressionMode;

namespace LightStart_BBS_server
{
    public partial class Form1 : Form
    {
        readonly Server server = new Server();
        public static SQLiteManager manager_user;
        public static SQLiteManager manager_forum;
        public static TextBox LogBox;

        public static string logFileName = "";
        public static string logDirectory = ".\\lsbbs_log";
        public static string errorDirectory = ".\\error";

        public Form1()
        {

            InitializeComponent();
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyy.MM.dd_HH_mm");
            logFileName = logDirectory + "\\LSBBS_LOG_" + formattedDate + ".log";
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);
            if (!Directory.Exists(errorDirectory))
                Directory.CreateDirectory(errorDirectory);
            LogBox = Log;
            LogT("日志文件将被会保存至 " + logFileName + "\r\n");
            CheckForIllegalCrossThreadCalls = false;
            // 替换为你的SQLite数据库文件路径和表名
            string filePath = ".\\server_user.db";
            string tableName = "BBS_server_user";
            manager_user = new SQLiteManager(filePath, tableName);
            // 创建SQLiteManager实例并连接数据库
            string filePath_forum = ".\\server_forum.db";
            string tableName_forum = "BBS_server_forum";
            manager_forum = new SQLiteManager(filePath_forum, tableName_forum);

            bool user_db_availabe = false;
            try
            {
                manager_user.SelectAll();
                user_db_availabe = true;
            }
            catch
            {
                user_db_availabe = false;
            }
            bool forum_db_availabe = false;
            try
            {
                manager_forum.SelectAll();
                forum_db_availabe = true;
            }
            catch
            {
                forum_db_availabe = false;
            }
            if (!user_db_availabe)
            {
                // 创建一个名为"users"的表
                Dictionary<string, string> columns = new Dictionary<string, string>();
                columns["id"] = "TEXT";
                columns["name"] = "TEXT";
                columns["password"] = "TEXT";
                columns["salt"] = "TEXT";
                columns["sharedKey"] = "TEXT";
                columns["token"] = "TEXT";
                columns["usergroup"] = "TEXT";
                columns["ban"] = "TEXT";
                columns["moreInfoJson"] = "TEXT";
                manager_user.CreateTable(tableName, columns);
                LogT("创建 BBS_server_user 表格");
            }

            if (!forum_db_availabe)
            {
                Dictionary<string, string> columns = new Dictionary<string, string>();
                columns["type"] = "TEXT"; //类型,1-分区,2-帖子
                //下为公共
                columns["id"] = "TEXT"; //帖子or分区ID,整数,帖子倒序,分区正序
                //下为分区
                columns["boardName"] = "TEXT"; //名称多语言,json保存(zh与en)
                //下为帖子
                columns["forumgroup"] = "TEXT"; //使用ID对应分类
                columns["poster"] = "TEXT"; //使用userID对应用户
                columns["title"] = "TEXT";
                columns["likes"] = "TEXT"; //数组
                columns["topics"] = "TEXT"; //使用json保存,[{"":__,"poster":"_userid_","text":"_text(markdown)_","likes":[_list_],"comments":["id"+"poster"+"text"+"likes"+"reply"①]},...] //①:0-root, others-_id_
                columns["text"] = "TEXT"; //markdown
                columns["moreInfoJson"] = "TEXT";
                manager_forum.CreateTable(tableName_forum, columns);
                LogT("创建 BBS_server_forum 表格");
            }

            Extract7zDll();
            server.Start();
            LogT("服务器已开启");
        }

        static void Extract7zDll()
        {
            string dllName = "7z.dll";
            string currentDirectory = Directory.GetCurrentDirectory();
            string dllPath = Path.Combine(currentDirectory, dllName);

            if (!File.Exists(dllPath))
            {
                // 从嵌入资源中提取7z.dll到当前目录
                using (Stream resourceStream = typeof(Form1).Assembly.GetManifestResourceStream("LightStart_BBS_server.7z.dll"))
                {
                    using (FileStream fileStream = new FileStream(dllPath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
            }
        }

        public static void CreateFile(string fileName, string textToWrite)
        {
            // 在程序运行目录下创建一个 Windows 文件
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            // 向文件中写入指定的文本内容，并保存
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(textToWrite);
            }
        }

        public static void AppendToFile(string fileName, string textToAppend)
        {
            // 在程序运行目录下找到指定的文本文件
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            // 向文件中追加指定的文本内容，并保存
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(textToAppend);
            }
        }

        public static void ChangeUserID(string id_before, string id_after)
        {
            Dictionary<string, string> row = new Dictionary<string, string>
            {
                { "id", id_after }
            };
            string condition = "id='" + SQLiteManager.Encrypt(id_before) + "'";
            manager_user.Update(row, condition);
            Dictionary<string, string> row2 = new Dictionary<string, string>
            {
                { "poster", id_after }
            };
            string condition2 = $"type='" + SQLiteManager.Encrypt("2") + "' AND poster='" + SQLiteManager.Encrypt(id_before) + "'";
            manager_forum.Update(row2, condition2);
        }
        public static void ChangeUserName(string id, string name_after)
        {
            Dictionary<string, string> row = new Dictionary<string, string>
            {
                { "name", name_after }
            };
            string condition = "id='" + SQLiteManager.Encrypt(id) + "'";
            manager_user.Update(row, condition);
        }
        public static void ChangeUserInvitationKey(string id, string invitationKey_after)
        {
            Dictionary<string, string> row = new Dictionary<string, string>
            {
                { "sharedKey", invitationKey_after }
            };
            string condition = "id='" + SQLiteManager.Encrypt(id) + "'";
            manager_user.Update(row, condition);
        }

        public static void DeleteUser(string id)
        {
            string condition = "id='" + SQLiteManager.Encrypt(id) + "'";
            manager_user.Delete(condition);
        }

        public static void DeleteForumBoard(int id)
        {
            string condition = $"type='{SQLiteManager.Encrypt("1")}' AND id='{SQLiteManager.Encrypt(id.ToString())}'";
            manager_forum.Delete(condition);
            string condition2 = $"type='{SQLiteManager.Encrypt("2")}' AND forumgroup='{SQLiteManager.Encrypt(id.ToString())}'";
            manager_forum.Delete(condition2);
        }

        public static int GetUserGroup(string id)
        {
            Dictionary<string, string> valuePairs = getUserByIDPW(id, "");
            return int.Parse(valuePairs["usergroup"]);
        }

        public static void SetUserGroup(string id, int group)
        {
            Dictionary<string, string> row = new Dictionary<string, string>
            {
                { "usergroup", group.ToString() }
            };
            string condition = "id='" + SQLiteManager.Encrypt(id) + "'";
            manager_user.Update(row, condition);
        }

        public static List<Dictionary<string, string>> GetForumBoards(bool autoSort=true)
        {
            string condition = "type='" + SQLiteManager.Encrypt("1") + "'";
            List<Dictionary<string, string>> boards = manager_forum.SelectWhere(condition);
            if (autoSort)
            {
                boards.Sort((dict1, dict2) =>
                {
                    // 将"id"值转换为整数类型
                    int id1 = int.Parse(dict1["id"]);
                    int id2 = int.Parse(dict2["id"]);

                    // 使用整数值进行比较
                    return id1.CompareTo(id2);
                });
            }

            return boards;
        }

        public static void SetForumBoardsTitle(int id,string zhText="",string enText="")
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            JObject titleJson = new JObject();
            if (zhText != "")
                titleJson.Add("zh", zhText);
            if (enText != "")
                titleJson.Add("en", enText);
            row.Add("boardName", titleJson.ToString(Formatting.None));
            string condition = $"type='{SQLiteManager.Encrypt("1")}' AND id='{SQLiteManager.Encrypt(id.ToString())}'";
            manager_forum.Update(row, condition);
        }

        public static void SwapForumBoards(int id1, int id2)
        {
            //切换board
            string condition1 = $"type='{SQLiteManager.Encrypt("1")}' AND id='{SQLiteManager.Encrypt(id1.ToString())}'";
            string condition2 = $"type='{SQLiteManager.Encrypt("1")}' AND id='{SQLiteManager.Encrypt(id2.ToString())}'";
            Dictionary<string, string> row1 = new Dictionary<string, string>
            {
                { "id", manager_forum.SelectWhere(condition1)[0]["id"] }
            };
            Dictionary<string, string> row2 = new Dictionary<string, string>
            {
                { "id", manager_forum.SelectWhere(condition2)[0]["id"] }
            };
            Dictionary<string, string> row2_tmp = new Dictionary<string, string>
            {
                { "id", "row2before" }
            };
            string condition2_tmp = $"type='{SQLiteManager.Encrypt("1")}' AND id='{SQLiteManager.Encrypt("row2before")}'";
            manager_forum.Update(row2_tmp, condition2);
            manager_forum.Update(row2, condition1);
            manager_forum.Update(row1, condition2_tmp);
            //切换post
            condition1 = $"type='{SQLiteManager.Encrypt("2")}' AND forumgroup='{SQLiteManager.Encrypt(id1.ToString())}'";
            condition2 = $"type='{SQLiteManager.Encrypt("2")}' AND forumgroup='{SQLiteManager.Encrypt(id2.ToString())}'";
            row1 = new Dictionary<string, string>
            {
                { "forumgroup", id1.ToString() }
            };
            row2 = new Dictionary<string, string>
            {
                { "forumgroup", id2.ToString() }
            };
            row2_tmp = new Dictionary<string, string>
            {
                { "forumgroup", "group2before" }
            };
            condition2_tmp = $"type='{SQLiteManager.Encrypt("2")}' AND forumgroup='{SQLiteManager.Encrypt("group2before")}'";
            manager_forum.Update(row2_tmp, condition2);
            manager_forum.Update(row2, condition1);
            manager_forum.Update(row1, condition2_tmp);
        }

        public static void AddForumBoard(string title_json)
        {
            List<Dictionary<string, string>> boards = GetForumBoards();
            int id = 0;
            try
            {
                foreach (var i in boards)
                {
                    if (int.Parse(i["id"]) >= id)
                    {
                        id = int.Parse(i["id"]) + 1;
                    }
                }
            }
            catch { }
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("type", "1"); //分区固定
            row.Add("id", id.ToString());
            row.Add("boardName", title_json);
            manager_forum.Insert(row);
        }

        public static string ReadMsgConfigConfig()
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
            return config.ToJsonString();
        }

        public static void LogT(string text, bool time = true, bool saveToFile = true, string ip = "")
        {
            string appendText = "";
            if (time)
                appendText += "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
            if (ip != "")
                appendText += $"[{ip}]\r\n";
            appendText += text + "\r\n";
            if (saveToFile)
                AppendToFile(logFileName, appendText);
            LogBox.AppendText(appendText);
        }

        public static string randomStr(
            int length, //长度
            int difficulty = 2 //难度, 0=纯数字 1=纯字母 2=字母+数字 else=null
            )
        {
            Random random = new Random();
            string result = "";
            for (int i = 0; i < length; i++)
            {
                int type = 0;
                switch (difficulty)
                {
                    case 0:
                        type = 1; // 1:数字 2:小写字母 3:大写字母
                        break;
                    case 1:
                        type = random.Next(2, 4);
                        break;
                    case 2:
                        type = random.Next(1, 4);
                        break;
                }
                switch (type)
                {
                    case 1:
                        int rand1 = random.Next(0, 10);
                        result += rand1.ToString();
                        break;
                    case 2:
                        int rand2 = random.Next(97, 123);
                        char char2 = (char)rand2;
                        result += char2.ToString();
                        break;
                    case 3:
                        int rand3 = random.Next(65, 91);
                        char char3 = (char)rand3;
                        result += char3.ToString();
                        break;
                }
            }
            return result;
        }

        public static string HashPw(string str, string userid)
        {
            Dictionary<string, string> user = manager_user.SelectWhere("id='" + SQLiteManager.Encrypt(userid) + "'")[0];
            return SHA256(SHA256(str) + SHA256(user["salt"]) + SHA1(str + user["salt"]));
        }

        public static string HashToken(string str)
        {
            return SHA256(SHA256(str + SHA256(str)) + SHA256(str) + SHA1(str));
        }

        public static string ChangeUserToken(string id)
        {
            string token = randomStr(60);
            while (getUserByToken(token)["available"] == "true")
            {
                token = randomStr(80);
            }
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("token", HashToken(token));
            string condition = "id='" + SQLiteManager.Encrypt(id) + "'";
            manager_user.Update(row, condition);
            return token;
        }

        public static string RegUser(string id, string name, string password, string salt, string sharedKey)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row["id"] = id;
            row["name"] = name;
            row["password"] = password;
            row["salt"] = salt;
            row["sharedKey"] = sharedKey;
            string token = randomStr(60);
            while (getUserByToken(token)["available"] == "true")
            {
                token = randomStr(80);
            }
            row["token"] = HashToken(token);
            row["usergroup"] = Constants.USERGROUP_default.ToString();
            row["ban"] = "";
            row["moreInfoJson"] = "";
            manager_user.Insert(row);
            return row["token"];
        }

        public static void AddComment(string postID, string posterID, string commentText)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();

        }

        public static void DeletePost(string postID)
        {
            string condition = $"type='" + SQLiteManager.Encrypt("2") + "' AND id='" + SQLiteManager.Encrypt(postID) + "'";
            manager_forum.Delete(condition);
        }

        public static void AddPost(JObject postInfo) //json需包含forumgroup poster title text
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row["type"] = "2"; //固定type=2
            List<Dictionary<string, string>> posts = GetEveryPost();
            int id = 0;
            try
            {
                foreach(var i in posts) 
                {
                    if (int.Parse(i["id"]) >= id)
                    {
                        id = int.Parse(i["id"]) + 1;
                    }
                }
            }
            catch { }
            row["id"] = id.ToString();
            row["forumgroup"] = postInfo["forumgroup"].ToString();
            row["poster"] = postInfo["poster"].ToString();
            row["title"] = postInfo["title"].ToString();
            row["text"] = postInfo["text"].ToString();
            row["likes"] = "[]";
            row["topics"] = "[]";
            row["moreInfoJson"] = "";
            manager_forum.Insert(row);
        }

        public static Dictionary<string, string> GetPost(string id)
        {
            return manager_forum.SelectWhere($"type='" + SQLiteManager.Encrypt("2") + "' AND id='" + SQLiteManager.Encrypt(id) + "'")[0];
        }

        public static List<Dictionary<string, string>> GetEveryPost()
        {
            return manager_forum.SelectWhere("type='" + SQLiteManager.Encrypt("2") + "'");
        }

        static void RestartServer()
        {
            /*
            server.Stop();
            server.Start();
            log("服务器已重启\r\n");
            */
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            RestartServer();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            LogT("正在关闭数据库及服务器\r\n");
            manager_user.Close();
            server.Stop();
        }

        public static void UpdateErrorDirectory()
        {
            DateTime now = DateTime.Now;
            string formattedDate = now.ToString("yyyy_MM_dd");
            string zipFilePath = Path.Combine(errorDirectory, $"error_{formattedDate}_.7z");
            try
            {

                string[] files = Directory.GetFiles(errorDirectory);
                SevenZipCompressor compressor = new SevenZipCompressor();
                compressor.ArchiveFormat = OutArchiveFormat.SevenZip;
                compressor.CompressionMode = CompressionMode.Create;
                compressor.CompressionLevel = CompressionLevel.Fast;
                compressor.CompressionMethod = CompressionMethod.Default;
                compressor.TempFolderPath = Path.GetTempPath(); // 设置临时文件夹路径

                if (!File.Exists(zipFilePath))
                {
                    SevenZipBase.SetLibraryPath(".\\7z.dll");
                    string emptyFilePath = Path.Combine(Path.GetTempPath(), "empty.txt");
                    File.WriteAllText(emptyFilePath, string.Empty); // 创建一个空的文本文件
                    compressor.CompressFiles(zipFilePath, emptyFilePath); // 压缩空文件到指定路径
                    File.Delete(emptyFilePath);
                }

                compressor.CompressionMode = CompressionMode.Append;
                files = Directory.GetFiles(errorDirectory, "*.txt", SearchOption.TopDirectoryOnly);

                foreach (var file in files)
                {
                    compressor.CompressFiles(zipFilePath, file);
                }

                files = Directory.GetFiles(errorDirectory);
                foreach (string file in files)
                {
                    if (!file.Equals(zipFilePath))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch(Exception e)
            {
                LogT(e.Message, false, false);
            }
        }

        public static void CheckErrorLogT() //删除超过24h的错误日志压缩文件
        {
            string[] files = Directory.GetFiles(errorDirectory);
            foreach (string file in files)
            {
                DateTime creationTime = File.GetCreationTime(file);
                if (DateTime.Now - creationTime > TimeSpan.FromHours(24))
                {
                    File.Delete(file);
                }
            }
        }

        public static bool checkIDAvailable(string id)
        {
            List<Dictionary<string, string>> list = manager_user.SelectWhere("id='" + SQLiteManager.Encrypt(id) + "'");
            Dictionary<string, string> user = new Dictionary<string, string>();
            bool idAvailable = false;
            if (list.Count == 1)
            {
                idAvailable = false;
            }
            else
            {
                idAvailable = true;
            }
            return idAvailable;
        }

        public static List<Dictionary<string, string>> getPostsByGroup(string group)
        {
            string condition = $"type='" + SQLiteManager.Encrypt("2") + "' AND forumgroup='" + SQLiteManager.Encrypt(group) + "'";
            return manager_forum.SelectWhere(condition);
        }

        public static Dictionary<string, string> getUserByIDPW(string id, string pw)
        {
            List<Dictionary<string, string>> list = manager_user.SelectWhere("id='" + SQLiteManager.Encrypt(id) + "'");
            Dictionary<string, string> user = new Dictionary<string, string>();
            bool idAvailable = false;
            if (list.Count == 1)
            {
                user = list[0];
                idAvailable = true;
            }
            else
            {
                idAvailable = false;
            }
            user.Add("available", "false");
            if (idAvailable && HashPw(pw, id) == user["password"])
            {
                user["available"] = "true";
            }
            return user;
        }

        public static Dictionary<string, string> getUserByToken(string token)
        {
            List<Dictionary<string, string>> list = manager_user.SelectWhere("token='" + SQLiteManager.Encrypt(HashToken(token)) + "'");
            Dictionary<string, string> user = new Dictionary<string, string>();
            if (list.Count == 1)
            {
                user = list[0];
                user.Add("available", "true");
            }
            else
            {
                user.Add("available", "false");
            }
            return user;
        }

        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] by = Sha256.ComputeHash(SHA256Data);
            return BitConverter.ToString(by).Replace("-", "").ToLower(); 
        }
        public static string SHA1(string str)
        {
            byte[] SHA1Data = Encoding.UTF8.GetBytes(str);
            SHA1Managed Sha1 = new SHA1Managed();
            byte[] by = Sha1.ComputeHash(SHA1Data);
            return BitConverter.ToString(by).Replace("-", "").ToLower(); 
        }

        private void GetUsersInfo_Click(object sender, EventArgs e)
        {
            List<Dictionary<string, string>> data = manager_user.SelectAll();
            LogT("==============================\n", false, false);
            // 输出表格数据
            foreach (var item in data)
            {
                LogT(string.Join("\r\n", item.Values) + "\r\n\r\n", false, false);
            }
            LogT("==============================\n", false, false);
        }

        private void ClientMsgboxChange_Click(object sender, EventArgs e)
        {
            clientMsgboxSetting settingForm = new clientMsgboxSetting(); // 创建clientMsgboxSetting的实例
            settingForm.ShowDialog(); // 以模态窗口的方式显示clientMsgboxSetting窗体        
        }

        public class Constants
        {
            public const int version = 3;

            public static readonly string[] USERGROUPS = { "default", "vip", "admin", "vip-" }; // 顺序要与下面一致
            public const int USERGROUP_default = 0;
            public const int USERGROUP_vip = 1;
            public const int USERGROUP_admin = 2;
            public const int USERGROUP_vip_low = 3;
        };

        public class SQLiteManager
        {
            private SQLiteConnection connection;
            private SQLiteCommand command;
            private string tableName;
            private const string replaced_slash = "_";

            public void UpdateFromv2()
            {
                string selectQuery = "SELECT * FROM BBS_server_user";
                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // 获取每个列的值
                            string id = reader.GetString(0);
                            string name = reader.GetString(1);
                            string password = reader.GetString(2);
                            string salt = reader.GetString(3);
                            string sharedKey = reader.GetString(4);
                            string token = reader.GetString(5);
                            string usergroup = reader.GetString(6);
                            string ban = reader.GetString(7);
                            string moreInfoJson = reader.GetString(8);

                            // 使用加密函数处理值
                            string encryptedId = Encrypt(id);
                            string encryptedName = Encrypt(name);
                            string encryptedPassword = Encrypt(password);
                            string encryptedSalt = Encrypt(salt);
                            string encryptedSharedKey = Encrypt(sharedKey);
                            string encryptedToken = Encrypt(token);
                            string encryptedUsergroup = Encrypt(usergroup);
                            string encryptedBan = Encrypt(ban);
                            string encryptedMoreInfoJson = Encrypt(moreInfoJson);

                            // 更新或插入加密后的值到 SQLite
                            using (SQLiteCommand updateCommand = new SQLiteCommand(connection))
                            {
                                updateCommand.CommandText = "UPDATE BBS_server_user SET " +
                                    "id = @encryptedId, " +
                                    "name = @encryptedName, " +
                                    "password = @encryptedPassword, " +
                                    "salt = @encryptedSalt, " +
                                    "sharedKey = @encryptedSharedKey, " +
                                    "token = @encryptedToken, " +
                                    "usergroup = @encryptedUsergroup, " +
                                    "ban = @encryptedBan, " +
                                    "moreInfoJson = @encryptedMoreInfoJson " +
                                    "WHERE id = @id";
                                updateCommand.Parameters.AddWithValue("@encryptedId", encryptedId);
                                updateCommand.Parameters.AddWithValue("@encryptedName", encryptedName);
                                updateCommand.Parameters.AddWithValue("@encryptedPassword", encryptedPassword);
                                updateCommand.Parameters.AddWithValue("@encryptedSalt", encryptedSalt);
                                updateCommand.Parameters.AddWithValue("@encryptedSharedKey", encryptedSharedKey);
                                updateCommand.Parameters.AddWithValue("@encryptedToken", encryptedToken);
                                updateCommand.Parameters.AddWithValue("@encryptedUsergroup", encryptedUsergroup);
                                updateCommand.Parameters.AddWithValue("@encryptedBan", encryptedBan);
                                updateCommand.Parameters.AddWithValue("@encryptedMoreInfoJson", encryptedMoreInfoJson);
                                updateCommand.Parameters.AddWithValue("@id", id);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }



            }

            public SQLiteManager(string filePath, string tableName)
            {
                if(filePath!="")
                {
                    this.tableName = tableName;
                    connection = new SQLiteConnection("Data Source=" + filePath);
                    connection.Open();
                    command = new SQLiteCommand(connection);
                }
            }

            // 创建一个表
            public void CreateTable(string tableName, Dictionary<string, string> columns)
            {
                string query = "CREATE TABLE IF NOT EXISTS " + tableName + " (";
                foreach (string column in columns.Keys)
                {
                    string dataType = columns[column];
                    query += column + " " + dataType + ",";
                }
                query = query.TrimEnd(',') + ")";
                ExecuteNonQuery(query);
            }

            public static string Encrypt(string str)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                string encryptedData = Convert.ToBase64String(bytes);
                encryptedData = encryptedData.Replace("/", replaced_slash);
                return encryptedData;
            }

            public static string Decrypt(string str)
            {
                try
                {
                    byte[] bytes = Convert.FromBase64String(str.Replace(replaced_slash, "/"));
                    string decryptedData = Encoding.UTF8.GetString(bytes);
                    return decryptedData;
                }
                catch
                {
                    return str;
                }
            }

            // 查询表中的所有行
            public List<Dictionary<string, string>> SelectAll()
            {
                string query = "SELECT * FROM " + tableName;
                List<Dictionary<string, string>> list = ExecuteQuery(query);
                foreach (Dictionary<string, string> item in list)
                {
                    foreach (string key in item.Keys)
                    {
                        string value = item[key];
                        value = Decrypt(value);
                        item[key] = value;
                    }

                }
                return list;
            }

            // 查询符合条件的行
            public List<Dictionary<string, string>> SelectWhere(string condition)
            {
                string query = "SELECT * FROM " + tableName + " WHERE " + condition;
                List<Dictionary<string, string>> list = ExecuteQuery(query);
                foreach (Dictionary<string, string> item in list)
                {
                    foreach (string key in item.Keys)
                    {
                        string value = item[key];
                        value = Decrypt(value);
                        item[key] = value;
                    }

                }
                return list;
            }

            // 插入一行数据
            public void Insert(Dictionary<string, string> row)
            {
                string columns = "";
                string values = "";
                foreach (string column in row.Keys)
                {
                    columns += column + ",";
                    values += "'" + Encrypt(row[column]) + "',";
                }
                columns = columns.TrimEnd(',');
                values = values.TrimEnd(',');
                string query = "INSERT INTO " + tableName + " (" + columns + ") VALUES (" + values + ")";
                ExecuteNonQuery(query);
            }

            // 修改符合条件的行
            public void Update(Dictionary<string, string> row, string condition)
            {
                string setValues = "";
                foreach (string column in row.Keys)
                {
                    setValues += column + "='" + Encrypt(row[column]) + "',";
                }
                setValues = setValues.TrimEnd(',');
                string query = "UPDATE " + tableName + " SET " + setValues + " WHERE " + condition;

                System.Diagnostics.Debug.WriteLine(query);
                ExecuteNonQuery(query);
            }

            // 删除符合条件的行
            public void Delete(string condition)
            {
                string query = "DELETE FROM " + tableName + " WHERE " + condition;
                ExecuteNonQuery(query);
            }

            // 执行SELECT查询，返回List<Dictionary<string, string>>结果
            private List<Dictionary<string, string>> ExecuteQuery(string query)
            {
                command.CommandText = query;
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> row = new Dictionary<string, string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string fieldName = reader.GetName(i);
                            string fieldValue = reader.GetValue(i).ToString();
                            row[fieldName] = fieldValue;
                        }
                        result.Add(row);
                    }
                }
                return result;
            }

            // 执行INSERT、UPDATE、DELETE等非SELECT操作，返回受影响的行数
            private int ExecuteNonQuery(string query)
            {
                command.CommandText = query;
                int result = command.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine(result);
                return result;
            }

            // 关闭连接
            public void Close()
            {
                command.Dispose();
                connection.Close();
            }
        }

        class Server
        {
            private Socket _listener;
            private readonly int _port = 33567;
            private bool _isRunning = true;

            public void Start()
            {
                //("Starting server at " + _port);
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, _port));
                _listener.Listen(100);

                Thread thread1 = new Thread(this.run);
                thread1.Start();
            }

            private void run()
            {
                while (_isRunning)
                {
                    try
                    {
                        Socket client = _listener.Accept();

                        //("Client connected from " + client.RemoteEndPoint.ToString(Formatting.None));

                        Connection connection = new Connection(client);
                        Thread thread = new Thread(connection.Process);
                        thread.Start();
                    }
                    catch
                    {

                    }
                }
            }

            public void Stop()
            {
                _isRunning = false;

                _listener.Close();
            }
        }

        class Message
        {
            public int type;
            public bool state; // true正常 false错误
            public string data;
            public string token;
            public int version;

            public Message(int type_, bool state_, string data_, string token_, bool return_old_verison = true)
            {
                this.type = type_;
                this.state = state_;
                this.data = data_;
                this.token = token_;
                if (return_old_verison)
                    this.version = Constants.version;
                else
                    this.version = -1;
            }

            public JObject ToJson()
            {
                JObject result = new JObject
                {
                    { "type", this.type },
                    { "state", this.state },
                    { "data", this.data },
                    { "token", this.token }
                };
                if (version != -1) // version为-1时,表示用户使用的软件版本过低,不支持接受json中的version,所以不添加version
                {
                    result.Add("version", this.version);
                }
                return result;
            }

            public string ToJsonString()
            {
                string startText = "";
                if (version == -1)
                {
                    startText = "oldVersion";
                }
                return startText + this.ToJson().ToString(Formatting.None);
            }
        }

        class Connection
        {
            // const
            // json型
            public const short MESSAGE_ERROR = 0;
            // 服务器状态
            public const short MESSAGE_GET_SERVER_STATE = 4;
            public const short MESSAGE_RETURN_SERVER_STATE = -4;
            // 注册及登陆
            public const short MESSAGE_ACTION_REG = 1; // user  注册
            public const short MESSAGE_RETURN_TOKEN = -1; // server  返回token
            public const short MESSAGE_GET_TOKEN_IDPW = 2; // user  id+pw获取token,登陆时用
            public const short MESSAGE_GET_TOKEN_AVAILABLE = 3; // user  检查token可用性
            // 论坛信息
            public const short MESSAGE_GET_USER_INFO = 3; // user  获取用户信息
            public const short MESSAGE_RETURN_USER_INFO = -3;// server  返回用户信息
            public const short MESSAGE_GET_BOARDS = 4; // user 获取论坛分区
            public const short MESSAGE_RETURN_BOARDS = -4; // server 返回论坛分区 (返回data为{"zh":[],"en":[]})
            public const short MESSAGE_ADD_BOARD = 5; // user 添加论坛分区(仅admin)
            public const short MESSAGE_RETURN_ADD_BOARD = -5; // server 返回添加论坛分区状态
            public const short MESSAGE_GET_USER_INFO_PUBLIC = 6; // user 获取公开用户信息(使用ID)
            public const short MESSAGE_RETURN_USER_INFO_PUBLIC = -6; // server 返回公开用户信息
            public const short MESSAGE_GET_ALL_POSTS = 7; // user 获取所有帖子(根据分区)
            public const short MESSAGE_RETURN_ALL_POSTS = -7; // user 返回所有帖子(只返回id title poster likes)
            public const short MESSAGE_ADD_POST = 8; // user 发送帖子
            public const short MESSAGE_RETURN_ADD_POST = -8; // server 返回发送帖子状态
            public const short MESSAGE_GET_POST = 9; // user 获取帖子(使用ID)
            public const short MESSAGE_RETURN_POST = -9; // server 返回帖子
            public const short MESSAGE_DELETE_POST = 10; // user 删除帖子
            public const short MESSAGE_RETURN_DELETE_POST = -10; // server 返回删除帖子
            public const short MESSAGE_DELETE_BOARD = 11; // user 删除分区
            public const short MESSAGE_RETURN_DELETE_BOARD = -11; // server 返回删除分区

            // 字符串型 (直接收到字符串)
            public const string MESSAGE_GET_ACTIVE = "active";
            public const string MESSAGE_GET_MSGBOX = "msgbox";
            // end

            private readonly Socket _client;

            public void Process()
            {
                try
                {
                    while (true)
                    {
                        byte[] buffer = new byte[262144];
                        int bytesReceived = _client.Receive(buffer);

                        if (bytesReceived == 0)
                        {
                            break;
                        }

                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                        string ip = ((IPEndPoint)_client.RemoteEndPoint).Address.ToString();
                        System.Diagnostics.Debug.WriteLine(receivedData);
                        try
                        {
                            bool switch_run = false;
                            switch (receivedData.Trim('\n')) //只需要从服务端获取的操作 不分token
                            {
                                case MESSAGE_GET_ACTIVE:
                                    {
                                        switch_run = true;
                                        SendData("true");
                                        Form1.LogT("有用户检测服务器Active", ip: ip);
                                        break;
                                    }
                                case MESSAGE_GET_MSGBOX:
                                    {

                                        switch_run = true;
                                        SendData(Form1.ReadMsgConfigConfig());
                                        Form1.LogT("有用户获取弹窗信息", ip: ip);
                                        break;
                                    }
                            }
                            if (!switch_run)
                            {
                                Message message = JsonConvert.DeserializeObject<Message>(receivedData); 
                                string return_msg = "";
                                if (message.token == "null") //无token操作
                                {
                                    if (message.type != MESSAGE_GET_TOKEN_IDPW)
                                        Form1.LogT("收到 无token 信息: \n" + receivedData, ip: ip);
                                    else //去除密码
                                    {
                                        JObject data = JObject.Parse(message.data);
                                        string password = data["password"].ToString();
                                        data["password"] = "***";
                                        message.data = data.ToString(Formatting.None);
                                        Form1.LogT("收到 无token 信息: \n" + message.ToJsonString(), ip: ip);
                                        data["password"] = password;
                                        message.data = data.ToString(Formatting.None);
                                    }
                                    switch (message.type)
                                    {
                                        case MESSAGE_ACTION_REG: //注册
                                            {
                                                return_msg = Action_Reg(message);
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_TOKEN_IDPW: //id+pw登录,获取token
                                            {
                                                return_msg = Action_Login(message);
                                                SendData(return_msg);
                                                break;
                                            }
                                    }
                                    try
                                    {
                                        if (bool.Parse(JObject.Parse(return_msg)["state"].ToString())) //成功时会返回token,防止泄露
                                        {
                                            Form1.LogT("返回 无token 信息: ***", ip: ip);
                                        }
                                        else
                                        {
                                            Form1.LogT("返回 无token 信息: \r\n" + return_msg, ip: ip);
                                        }
                                    }
                                    catch { }
                                }
                                else //有token操作
                                {
                                    string id = "";
                                    switch (message.type)
                                    {
                                        case MESSAGE_GET_USER_INFO:
                                            {
                                                string[] strings = Action_GetUserInfo(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_BOARDS:
                                            {
                                                string[] strings = Action_GetSortedBoards(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_ADD_BOARD:
                                            {
                                                string[] strings = Action_AddBoard(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_USER_INFO_PUBLIC:
                                            {
                                                string[] strings = Action_GetPublicUserInfo(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_ALL_POSTS:
                                            {
                                                string[] strings = Action_GetPostsByGroup(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_ADD_POST:
                                            {
                                                string[] strings = Action_AddPost(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_POST:
                                            {
                                                string[] strings = Action_GetPost(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_DELETE_POST:
                                            {
                                                string[] strings = Action_DeletePost(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_DELETE_BOARD:
                                            {
                                                string[] strings = Action_DeleteBoard(message);
                                                return_msg = strings[0];
                                                id = strings[1];
                                                SendData(return_msg);
                                                break;
                                            }
                                    }
                                    if (id != "")
                                    {
                                        try
                                        {
                                            JObject receivedData_json = JObject.Parse(receivedData);
                                            receivedData_json["token"] = "***";
                                            Form1.LogT($"收到 {id} 信息: \r\n{receivedData_json.ToString(Formatting.None)} \r\n+并返回: \r\n{return_msg}", ip: ip);
                                        }
                                        catch (Exception e) { }
                                    }
                                    else
                                    {
                                        Form1.LogT("收到 失效token 信息: \r\n" + receivedData + "\r\n+并返回: \r\n" + return_msg, ip: ip);
                                    }
                                }


                            }
                        }
                        catch (Exception ex)
                        {
                            DateTime now = DateTime.Now;
                            string formattedDate = now.ToString("yyyyMMdd");
                            string error_code = formattedDate + randomStr(4, 2);
                            error_code = SQLiteManager.Encrypt(error_code);
                            Form1.CheckErrorLogT();
                            string filePath = Path.Combine(Environment.CurrentDirectory, $"{errorDirectory}\\{error_code}.txt");
                            using (StreamWriter sw = File.AppendText(filePath))
                            {
                                sw.WriteLine($"IP Address: {ip}\nReceived Message:\n{receivedData}\n\nerr:\n{ex.Message}");
                            }
                            Form1.UpdateErrorDirectory();
                            LogT($"处理操作时出现错误 (code: {error_code}):{ex.Message}", ip: ip);

                            SendData(new Message(MESSAGE_ERROR, false, $"服务端在处理您的操作时出现错误,请重试\n\n若多次重试仍出错,请在24小时内截图此页面(或发送错误代号)至我们的邮箱: lsbbs@bzlzhh.top\n本次错误代号: {error_code}", "server").ToJsonString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //(ex.ToString(Formatting.None));
                }
                finally
                {
                    _client.Close();
                }
            }

            public static string[] Action_DeleteBoard(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    JObject data = JObject.Parse(message.data);
                    if (user["usergroup"] == Constants.USERGROUP_admin.ToString())
                    {
                        Form1.DeleteForumBoard(int.Parse(data["board"].ToString()));
                        result = new Message(MESSAGE_RETURN_DELETE_BOARD, true, "", "server").ToJsonString();
                    }
                    else
                    {
                        result = new Message(MESSAGE_RETURN_DELETE_BOARD, false, "你没有权限执行此操作", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_DELETE_BOARD, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_DeletePost(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    JObject data = JObject.Parse(message.data);
                    if (user["usergroup"] == Constants.USERGROUP_admin.ToString() || Form1.GetPost(data["post"].ToString())["poster"] == user["id"])
                    {
                        Form1.DeletePost(data["post"].ToString());
                        result = new Message(MESSAGE_RETURN_DELETE_POST, true, "", "server").ToJsonString();
                    }
                    else
                    {
                        result = new Message(MESSAGE_RETURN_DELETE_POST, false, "你没有权限删除此帖", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_DELETE_POST, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_GetPost(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    Dictionary<string, string> post = Form1.GetPost(message.data);
                    JObject postInfo = new JObject
                    {
                        { "id", post["id"] },
                        { "forumgroup", post["forumgroup"] },
                        { "title", post["title"] },
                        { "likes", post["likes"] },
                        { "topics", post["topics"] },
                        { "poster", post["poster"] },
                        { "text", post["text"] }
                    };
                    result = new Message(MESSAGE_RETURN_POST, true, postInfo.ToString(Formatting.None), "server").ToJsonString();
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_POST, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_AddPost(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    //检查forumgroup poster title text
                    JObject data = JObject.Parse(message.data);
                    bool right = true;
                    try
                    {
                        data["forumgroup"].ToString();
                        if (data["title"].ToString().Trim() == "" || data["text"].ToString().Trim() == "" || !CheckPostTitleValidity(data["title"].ToString()))
                        {
                            right = false;
                        }
                    }
                    catch { }
                    if (right)
                    {
                        data.Add("poster", user["id"]);
                        Form1.AddPost(data);
                        result = new Message(MESSAGE_RETURN_ADD_POST, true, "", "server").ToJsonString();
                    }
                    else
                    {
                        result = new Message(MESSAGE_RETURN_ADD_POST, false, "提供的参数不符合要求", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_ADD_POST, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_GetPostsByGroup(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    List<Dictionary<string, string>> list = Form1.getPostsByGroup(message.data);
                    list.Reverse(); //反转
                    JArray array = new JArray();
                    foreach (Dictionary<string, string> item in list)
                    {
                        JObject jObject = new JObject
                        {
                            {"id", item["id"]},
                            {"title", item["title"]},
                            {"poster", item["poster"]},
                            {"likes", item["likes"]}
                        };
                        array.Add(jObject);
                    }
                    result = new Message(MESSAGE_RETURN_ALL_POSTS, true, array.ToString(Formatting.None), "server").ToJsonString();
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_ALL_POSTS, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_GetPublicUserInfo(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    LogT(message.data);
                    Dictionary<string, string> foundUser = Form1.getUserByIDPW(message.data, "");
                    string id = "";
                    string name = "";
                    string sharedKey = "";
                    string usergroup = "";

                    try
                    {
                        id = foundUser["id"];
                    }
                    catch
                    {
                        id = message.data;
                    }
                    try
                    {
                        name = foundUser["name"];
                    }
                    catch
                    {
                        name = "用户已注销";
                    }
                    try
                    {
                        sharedKey = foundUser["sharedKey"];
                    }
                    catch
                    {
                        sharedKey = "未知";
                    }
                    try
                    {
                        usergroup = foundUser["usergroup"];
                    }
                    catch
                    {
                        usergroup = "0";
                    }

                    JObject userInfo = new JObject
                    {
                        { "id", id },
                        { "name", name },
                        { "sharedKey", sharedKey },
                        { "usergroup", usergroup }
                    };
                    result = new Message(MESSAGE_RETURN_USER_INFO_PUBLIC, true, userInfo.ToString(Formatting.None), "server").ToJsonString();
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_USER_INFO_PUBLIC, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_AddBoard(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    if (user["usergroup"] == Constants.USERGROUP_admin.ToString())
                    {
                        Form1.AddForumBoard(message.data);
                        result = new Message(MESSAGE_RETURN_ADD_BOARD, true, "", "server").ToJsonString();
                    }
                    else
                    {
                        result = new Message(MESSAGE_RETURN_ADD_BOARD, false, "你没有权限执行此操作", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_ADD_BOARD, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string[] Action_GetSortedBoards(Message message)
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    if (message.version < 3) //旧版本兼容
                    {
                        JObject json = new JObject();
                        JArray textZH = new JArray();
                        JArray textEN = new JArray();
                        List<Dictionary<string, string>> list = Form1.GetForumBoards();
                        List<Dictionary<string, string>> sortedList = list.OrderBy(dict => int.Parse(dict["id"])).ToList();
                        foreach (Dictionary<string, string> key in sortedList)
                        {
                            textZH.Add(JObject.Parse(key["boardName"])["zh"].ToString());
                            textEN.Add(JObject.Parse(key["boardName"])["en"].ToString());
                        }
                        json.Add("zh", textZH);
                        json.Add("en", textEN);
                        result = new Message(MESSAGE_RETURN_BOARDS, true, json.ToString(), "server").ToJsonString();
                    }
                    else
                    {
                        List<Dictionary<string, string>> list = Form1.GetForumBoards();
                        JArray json = new JArray();
                        foreach (var i in list)
                        {
                            JObject titles = JObject.Parse(i["boardName"]);
                            JObject tmp = new JObject
                            {
                                { "id", i["id"] },
                                { "title_zh", titles["zh"].ToString() },
                                { "title_en", titles["en"].ToString() },
                            };
                            json.Add(tmp);
                        }
                        result = new Message(MESSAGE_RETURN_BOARDS, true, json.ToString(Formatting.None), "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_BOARDS, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static string Action_Reg(Message message)
            {
                string result = "";
                JObject userInfoJson = JObject.Parse(message.data);
                JObject sendedJson = message.ToJson();
                string err = "";
                bool return_old_verison = true;
                if (!Form1.checkIDAvailable(userInfoJson["id"].ToString()))
                {
                    err = "不是一个可用的ID";
                }
                if (userInfoJson["sharedKey"].ToString() != "LightStart_Sssss")
                //if (!true)
                {
                    err = "不是一个可用的邀请码";
                }
                if (CheckIDValidity(userInfoJson["id"].ToString()))
                {
                    err = "不是一个规范的唯一标识符\n唯一标识符要求如下:\n1.唯一标识符必须大于2个字符且小于40个字符\n2.唯一标识符只允许包含字母、数字及下划线";
                }

                if (CheckNameValidity(userInfoJson["name"].ToString()))
                {
                    err = "不是一个规范的名称\n名称要求如下:\n1.名称必须大于2个字符且小于36个字符\n2.名称不允许包含控制字符(如换行等)";
                }

                try
                {
                    if (int.Parse(sendedJson["version"].ToString()) < 1)
                    {
                        err = "您的轻启嫩谈版本过低,请更新至最新版本";
                    }
                }
                catch
                {
                    return_old_verison = false;
                    err = "The LightStart BBS version you are using is not supported. Please update to the newest version available."; // 不提供version的版本不支持utf-8,使用ascii
                }


                if (err == "")
                {
                    string token = Form1.RegUser(userInfoJson["id"].ToString(), userInfoJson["name"].ToString().Replace(" ", ""), userInfoJson["password"].ToString(), userInfoJson["salt"].ToString(), userInfoJson["sharedKey"].ToString());
                    result = new Message(MESSAGE_RETURN_TOKEN, true, token, "server").ToJsonString();

                    userInfoJson["password"] = "***";
                    userInfoJson["token"] = "***";

                    Form1.LogT(">> 用户 " + userInfoJson["id"].ToString() + " 注册: " + userInfoJson.ToString(Formatting.None));
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_TOKEN, false, err, "server", return_old_verison).ToJsonString();
                }
                return result;
            }

            public static string Action_Login(Message message)
            {
                string result = "";
                JObject userInfoJson = JObject.Parse(message.data);
                JObject sendedJson = message.ToJson();
                Dictionary<string, string> user = Form1.getUserByIDPW(userInfoJson["id"].ToString(), userInfoJson["password"].ToString());
                string err = "";
                bool return_old_verison = true;

                if (user["available"].ToString() != "true")
                {
                    err = "错误的唯一标识符或密码";
                }

                try
                {
                    if (int.Parse(sendedJson["version"].ToString()) < 1)
                    {
                        err = "您的轻启嫩谈版本过低,请更新至最新版本";
                    }
                }
                catch
                {
                    return_old_verison = false;
                    err = "The LightStart BBS version you are using is not supported. Please update to the newest version available."; // 不提供version的版本不支持utf-8,使用ascii
                }

                if (err == "")
                {
                    string token = Form1.ChangeUserToken(userInfoJson["id"].ToString());
                    result = new Message(MESSAGE_RETURN_TOKEN, true, token, "server").ToJsonString();

                    userInfoJson["password"] = "***";
                    userInfoJson["token"] = "***";
                    Form1.LogT(">> 用户 " + userInfoJson["id"].ToString() + " 登录: " + userInfoJson.ToString(Formatting.None));
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_TOKEN, false, err, "server", return_old_verison).ToJsonString();
                }
                return result;
            }

            public static string[] Action_GetUserInfo(Message message) //返回需返回的message([0])及id([1]) 
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    JObject userInfo = new JObject
                    {
                        { "id", user["id"] },
                        { "name", user["name"] },
                        { "sharedKey", user["sharedKey"] },
                        { "usergroup", user["usergroup"] }
                    };
                    result = new Message(MESSAGE_RETURN_USER_INFO, true, userInfo.ToString(Formatting.None), "server").ToJsonString();
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_USER_INFO, false, "用户登录信息已失效", "server").ToJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public static bool CheckIDValidity(string str)
            {
                Regex regex = new Regex("^[a-zA-Z0-9_]*$");
                return !regex.IsMatch(str) && str.Length >= 2 && str.Length <= 40;
            }

            public static bool CheckNameValidity(string str)
            {
                Regex regex = new Regex(@"^[^\p{C}]+$");
                return !regex.IsMatch(str) && str.Length >= 2 && str.Length <= 36;
            }

            public static bool CheckPostTitleValidity(string str)
            {
                Regex regex = new Regex(@"\r|\n");
                return !regex.IsMatch(str);
            }

            public Connection(Socket client)
            {
                _client = client;
            }

            public static void SendData(string data)
            {
                System.Diagnostics.Debug.WriteLine("sending");
                string data_ = data;
                //data_ = data.Replace("\\r\\n","");
                System.Diagnostics.Debug.WriteLine(data_);

                byte[] buffer;
                if (data_.StartsWith("oldVersion")) //旧版本兼容
                {
                    data_ = data_.Substring(10);
                    buffer = Encoding.ASCII.GetBytes(data_ + " "); //旧版本会删除末尾一个字符
                }
                else
                {
                    buffer = Encoding.UTF8.GetBytes(data_);
                }
                _client.Send(buffer);
            }

            public static void Close()
            {
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
        }

        private void UserGroupChange_Click(object sender, EventArgs e)
        {
            manager manager_ = new manager(); // 创建clientMsgboxSetting的实例
            manager_.ShowDialog(); // 以模态窗口的方式显示clientMsgboxSetting窗体        
        }
    }
}