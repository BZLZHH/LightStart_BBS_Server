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
            LogT("��־�ļ������ᱣ���� " + logFileName + "\r\n");
            CheckForIllegalCrossThreadCalls = false;
            // �滻Ϊ���SQLite���ݿ��ļ�·���ͱ���
            string filePath = ".\\server_user.db";
            string tableName = "BBS_server_user";
            manager_user = new SQLiteManager(filePath, tableName);
            // ����SQLiteManagerʵ�����������ݿ�
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
                // ����һ����Ϊ"users"�ı�
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
                LogT("���� BBS_server_user ���");
            }

            if (!forum_db_availabe)
            {
                Dictionary<string, string> columns = new Dictionary<string, string>();
                columns["type"] = "TEXT"; //����,1-����,2-����
                //��Ϊ����
                columns["id"] = "TEXT"; //����or����ID,����,���ӵ���,��������
                //��Ϊ����
                columns["boardName"] = "TEXT"; //���ƶ�����,json����(zh��en)
                //��Ϊ����
                columns["forumgroup"] = "TEXT"; //ʹ��ID��Ӧ����
                columns["poster"] = "TEXT"; //ʹ��userID��Ӧ�û�
                columns["title"] = "TEXT";
                columns["likes"] = "TEXT"; //����
                columns["topics"] = "TEXT"; //ʹ��json����,[{"":__,"poster":"_userid_","text":"_text(markdown)_","likes":[_list_],"comments":["id"+"poster"+"text"+"likes"+"reply"��]},...] //��:0-root, others-_id_
                columns["text"] = "TEXT"; //markdown
                columns["moreInfoJson"] = "TEXT";
                manager_forum.CreateTable(tableName_forum, columns);
                LogT("���� BBS_server_forum ���");
            }

            Extract7zDll();
            server.Start();
            LogT("�������ѿ���");
        }

        static void Extract7zDll()
        {
            string dllName = "7z.dll";
            string currentDirectory = Directory.GetCurrentDirectory();
            string dllPath = Path.Combine(currentDirectory, dllName);

            if (!File.Exists(dllPath))
            {
                // ��Ƕ����Դ����ȡ7z.dll����ǰĿ¼
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
            // �ڳ�������Ŀ¼�´���һ�� Windows �ļ�
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            // ���ļ���д��ָ�����ı����ݣ�������
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine(textToWrite);
            }
        }

        public static void AppendToFile(string fileName, string textToAppend)
        {
            // �ڳ�������Ŀ¼���ҵ�ָ�����ı��ļ�
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            // ���ļ���׷��ָ�����ı����ݣ�������
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
                    // ��"id"ֵת��Ϊ��������
                    int id1 = int.Parse(dict1["id"]);
                    int id2 = int.Parse(dict2["id"]);

                    // ʹ������ֵ���бȽ�
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
            //�л�board
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
            //�л�post
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
            row.Add("type", "1"); //�����̶�
            row.Add("id", id.ToString());
            row.Add("boardName", title_json);
            manager_forum.Insert(row);
        }

        public static string ReadMsgConfigConfig()
        {
            const string jsonFilePath = "msgboxConfig.json"; // �ļ�·��
            MsgBoxConfig config = new MsgBoxConfig(); // ����һ���µ�MsgBoxConfigʵ��

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath); // ��ȡjson�ļ�����
                config = JsonConvert.DeserializeObject<MsgBoxConfig>(jsonContent); // ����json�ļ����ݣ�����MsgBoxConfigʵ����
                                                                                   // ��MsgBoxConfigʵ���б����ֵ��ʾ��WinForm�ؼ���
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
            int length, //����
            int difficulty = 2 //�Ѷ�, 0=������ 1=����ĸ 2=��ĸ+���� else=null
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
                        type = 1; // 1:���� 2:Сд��ĸ 3:��д��ĸ
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

        public static void AddPost(JObject postInfo) //json�����forumgroup poster title text
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row["type"] = "2"; //�̶�type=2
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
            log("������������\r\n");
            */
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            RestartServer();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            LogT("���ڹر����ݿ⼰������\r\n");
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
                compressor.TempFolderPath = Path.GetTempPath(); // ������ʱ�ļ���·��

                if (!File.Exists(zipFilePath))
                {
                    SevenZipBase.SetLibraryPath(".\\7z.dll");
                    string emptyFilePath = Path.Combine(Path.GetTempPath(), "empty.txt");
                    File.WriteAllText(emptyFilePath, string.Empty); // ����һ���յ��ı��ļ�
                    compressor.CompressFiles(zipFilePath, emptyFilePath); // ѹ�����ļ���ָ��·��
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

        public static void CheckErrorLogT() //ɾ������24h�Ĵ�����־ѹ���ļ�
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
            // ����������
            foreach (var item in data)
            {
                LogT(string.Join("\r\n", item.Values) + "\r\n\r\n", false, false);
            }
            LogT("==============================\n", false, false);
        }

        private void ClientMsgboxChange_Click(object sender, EventArgs e)
        {
            clientMsgboxSetting settingForm = new clientMsgboxSetting(); // ����clientMsgboxSetting��ʵ��
            settingForm.ShowDialog(); // ��ģ̬���ڵķ�ʽ��ʾclientMsgboxSetting����        
        }

        public class Constants
        {
            public const int version = 3;

            public static readonly string[] USERGROUPS = { "default", "vip", "admin", "vip-" }; // ˳��Ҫ������һ��
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
                            // ��ȡÿ���е�ֵ
                            string id = reader.GetString(0);
                            string name = reader.GetString(1);
                            string password = reader.GetString(2);
                            string salt = reader.GetString(3);
                            string sharedKey = reader.GetString(4);
                            string token = reader.GetString(5);
                            string usergroup = reader.GetString(6);
                            string ban = reader.GetString(7);
                            string moreInfoJson = reader.GetString(8);

                            // ʹ�ü��ܺ�������ֵ
                            string encryptedId = Encrypt(id);
                            string encryptedName = Encrypt(name);
                            string encryptedPassword = Encrypt(password);
                            string encryptedSalt = Encrypt(salt);
                            string encryptedSharedKey = Encrypt(sharedKey);
                            string encryptedToken = Encrypt(token);
                            string encryptedUsergroup = Encrypt(usergroup);
                            string encryptedBan = Encrypt(ban);
                            string encryptedMoreInfoJson = Encrypt(moreInfoJson);

                            // ���»������ܺ��ֵ�� SQLite
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

            // ����һ����
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

            // ��ѯ���е�������
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

            // ��ѯ������������
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

            // ����һ������
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

            // �޸ķ�����������
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

            // ɾ��������������
            public void Delete(string condition)
            {
                string query = "DELETE FROM " + tableName + " WHERE " + condition;
                ExecuteNonQuery(query);
            }

            // ִ��SELECT��ѯ������List<Dictionary<string, string>>���
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

            // ִ��INSERT��UPDATE��DELETE�ȷ�SELECT������������Ӱ�������
            private int ExecuteNonQuery(string query)
            {
                command.CommandText = query;
                int result = command.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine(result);
                return result;
            }

            // �ر�����
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
            public bool state; // true���� false����
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
                if (version != -1) // versionΪ-1ʱ,��ʾ�û�ʹ�õ�����汾����,��֧�ֽ���json�е�version,���Բ����version
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
            // json��
            public const short MESSAGE_ERROR = 0;
            // ������״̬
            public const short MESSAGE_GET_SERVER_STATE = 4;
            public const short MESSAGE_RETURN_SERVER_STATE = -4;
            // ע�ἰ��½
            public const short MESSAGE_ACTION_REG = 1; // user  ע��
            public const short MESSAGE_RETURN_TOKEN = -1; // server  ����token
            public const short MESSAGE_GET_TOKEN_IDPW = 2; // user  id+pw��ȡtoken,��½ʱ��
            public const short MESSAGE_GET_TOKEN_AVAILABLE = 3; // user  ���token������
            // ��̳��Ϣ
            public const short MESSAGE_GET_USER_INFO = 3; // user  ��ȡ�û���Ϣ
            public const short MESSAGE_RETURN_USER_INFO = -3;// server  �����û���Ϣ
            public const short MESSAGE_GET_BOARDS = 4; // user ��ȡ��̳����
            public const short MESSAGE_RETURN_BOARDS = -4; // server ������̳���� (����dataΪ{"zh":[],"en":[]})
            public const short MESSAGE_ADD_BOARD = 5; // user �����̳����(��admin)
            public const short MESSAGE_RETURN_ADD_BOARD = -5; // server ���������̳����״̬
            public const short MESSAGE_GET_USER_INFO_PUBLIC = 6; // user ��ȡ�����û���Ϣ(ʹ��ID)
            public const short MESSAGE_RETURN_USER_INFO_PUBLIC = -6; // server ���ع����û���Ϣ
            public const short MESSAGE_GET_ALL_POSTS = 7; // user ��ȡ��������(���ݷ���)
            public const short MESSAGE_RETURN_ALL_POSTS = -7; // user ������������(ֻ����id title poster likes)
            public const short MESSAGE_ADD_POST = 8; // user ��������
            public const short MESSAGE_RETURN_ADD_POST = -8; // server ���ط�������״̬
            public const short MESSAGE_GET_POST = 9; // user ��ȡ����(ʹ��ID)
            public const short MESSAGE_RETURN_POST = -9; // server ��������
            public const short MESSAGE_DELETE_POST = 10; // user ɾ������
            public const short MESSAGE_RETURN_DELETE_POST = -10; // server ����ɾ������
            public const short MESSAGE_DELETE_BOARD = 11; // user ɾ������
            public const short MESSAGE_RETURN_DELETE_BOARD = -11; // server ����ɾ������

            // �ַ����� (ֱ���յ��ַ���)
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
                            switch (receivedData.Trim('\n')) //ֻ��Ҫ�ӷ���˻�ȡ�Ĳ��� ����token
                            {
                                case MESSAGE_GET_ACTIVE:
                                    {
                                        switch_run = true;
                                        SendData("true");
                                        Form1.LogT("���û���������Active", ip: ip);
                                        break;
                                    }
                                case MESSAGE_GET_MSGBOX:
                                    {

                                        switch_run = true;
                                        SendData(Form1.ReadMsgConfigConfig());
                                        Form1.LogT("���û���ȡ������Ϣ", ip: ip);
                                        break;
                                    }
                            }
                            if (!switch_run)
                            {
                                Message message = JsonConvert.DeserializeObject<Message>(receivedData); 
                                string return_msg = "";
                                if (message.token == "null") //��token����
                                {
                                    if (message.type != MESSAGE_GET_TOKEN_IDPW)
                                        Form1.LogT("�յ� ��token ��Ϣ: \n" + receivedData, ip: ip);
                                    else //ȥ������
                                    {
                                        JObject data = JObject.Parse(message.data);
                                        string password = data["password"].ToString();
                                        data["password"] = "***";
                                        message.data = data.ToString(Formatting.None);
                                        Form1.LogT("�յ� ��token ��Ϣ: \n" + message.ToJsonString(), ip: ip);
                                        data["password"] = password;
                                        message.data = data.ToString(Formatting.None);
                                    }
                                    switch (message.type)
                                    {
                                        case MESSAGE_ACTION_REG: //ע��
                                            {
                                                return_msg = Action_Reg(message);
                                                SendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_TOKEN_IDPW: //id+pw��¼,��ȡtoken
                                            {
                                                return_msg = Action_Login(message);
                                                SendData(return_msg);
                                                break;
                                            }
                                    }
                                    try
                                    {
                                        if (bool.Parse(JObject.Parse(return_msg)["state"].ToString())) //�ɹ�ʱ�᷵��token,��ֹй¶
                                        {
                                            Form1.LogT("���� ��token ��Ϣ: ***", ip: ip);
                                        }
                                        else
                                        {
                                            Form1.LogT("���� ��token ��Ϣ: \r\n" + return_msg, ip: ip);
                                        }
                                    }
                                    catch { }
                                }
                                else //��token����
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
                                            Form1.LogT($"�յ� {id} ��Ϣ: \r\n{receivedData_json.ToString(Formatting.None)} \r\n+������: \r\n{return_msg}", ip: ip);
                                        }
                                        catch (Exception e) { }
                                    }
                                    else
                                    {
                                        Form1.LogT("�յ� ʧЧtoken ��Ϣ: \r\n" + receivedData + "\r\n+������: \r\n" + return_msg, ip: ip);
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
                            LogT($"�������ʱ���ִ��� (code: {error_code}):{ex.Message}", ip: ip);

                            SendData(new Message(MESSAGE_ERROR, false, $"������ڴ������Ĳ���ʱ���ִ���,������\n\n����������Գ���,����24Сʱ�ڽ�ͼ��ҳ��(���ʹ������)�����ǵ�����: lsbbs@bzlzhh.top\n���δ������: {error_code}", "server").ToJsonString());
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
                        result = new Message(MESSAGE_RETURN_DELETE_BOARD, false, "��û��Ȩ��ִ�д˲���", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_DELETE_BOARD, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                        result = new Message(MESSAGE_RETURN_DELETE_POST, false, "��û��Ȩ��ɾ������", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_DELETE_POST, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                    result = new Message(MESSAGE_RETURN_POST, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                    //���forumgroup poster title text
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
                        result = new Message(MESSAGE_RETURN_ADD_POST, false, "�ṩ�Ĳ���������Ҫ��", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_ADD_POST, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                    list.Reverse(); //��ת
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
                    result = new Message(MESSAGE_RETURN_ALL_POSTS, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                        name = "�û���ע��";
                    }
                    try
                    {
                        sharedKey = foundUser["sharedKey"];
                    }
                    catch
                    {
                        sharedKey = "δ֪";
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
                    result = new Message(MESSAGE_RETURN_USER_INFO_PUBLIC, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                        result = new Message(MESSAGE_RETURN_ADD_BOARD, false, "��û��Ȩ��ִ�д˲���", "server").ToJsonString();
                    }
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_ADD_BOARD, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                    if (message.version < 3) //�ɰ汾����
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
                    result = new Message(MESSAGE_RETURN_BOARDS, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                    err = "����һ�����õ�ID";
                }
                if (userInfoJson["sharedKey"].ToString() != "LightStart_Sssss")
                //if (!true)
                {
                    err = "����һ�����õ�������";
                }
                if (CheckIDValidity(userInfoJson["id"].ToString()))
                {
                    err = "����һ���淶��Ψһ��ʶ��\nΨһ��ʶ��Ҫ������:\n1.Ψһ��ʶ���������2���ַ���С��40���ַ�\n2.Ψһ��ʶ��ֻ���������ĸ�����ּ��»���";
                }

                if (CheckNameValidity(userInfoJson["name"].ToString()))
                {
                    err = "����һ���淶������\n����Ҫ������:\n1.���Ʊ������2���ַ���С��36���ַ�\n2.���Ʋ�������������ַ�(�绻�е�)";
                }

                try
                {
                    if (int.Parse(sendedJson["version"].ToString()) < 1)
                    {
                        err = "����������̸�汾����,����������°汾";
                    }
                }
                catch
                {
                    return_old_verison = false;
                    err = "The LightStart BBS version you are using is not supported. Please update to the newest version available."; // ���ṩversion�İ汾��֧��utf-8,ʹ��ascii
                }


                if (err == "")
                {
                    string token = Form1.RegUser(userInfoJson["id"].ToString(), userInfoJson["name"].ToString().Replace(" ", ""), userInfoJson["password"].ToString(), userInfoJson["salt"].ToString(), userInfoJson["sharedKey"].ToString());
                    result = new Message(MESSAGE_RETURN_TOKEN, true, token, "server").ToJsonString();

                    userInfoJson["password"] = "***";
                    userInfoJson["token"] = "***";

                    Form1.LogT(">> �û� " + userInfoJson["id"].ToString() + " ע��: " + userInfoJson.ToString(Formatting.None));
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
                    err = "�����Ψһ��ʶ��������";
                }

                try
                {
                    if (int.Parse(sendedJson["version"].ToString()) < 1)
                    {
                        err = "����������̸�汾����,����������°汾";
                    }
                }
                catch
                {
                    return_old_verison = false;
                    err = "The LightStart BBS version you are using is not supported. Please update to the newest version available."; // ���ṩversion�İ汾��֧��utf-8,ʹ��ascii
                }

                if (err == "")
                {
                    string token = Form1.ChangeUserToken(userInfoJson["id"].ToString());
                    result = new Message(MESSAGE_RETURN_TOKEN, true, token, "server").ToJsonString();

                    userInfoJson["password"] = "***";
                    userInfoJson["token"] = "***";
                    Form1.LogT(">> �û� " + userInfoJson["id"].ToString() + " ��¼: " + userInfoJson.ToString(Formatting.None));
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_TOKEN, false, err, "server", return_old_verison).ToJsonString();
                }
                return result;
            }

            public static string[] Action_GetUserInfo(Message message) //�����践�ص�message([0])��id([1]) 
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
                    result = new Message(MESSAGE_RETURN_USER_INFO, false, "�û���¼��Ϣ��ʧЧ", "server").ToJsonString();
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
                if (data_.StartsWith("oldVersion")) //�ɰ汾����
                {
                    data_ = data_.Substring(10);
                    buffer = Encoding.ASCII.GetBytes(data_ + " "); //�ɰ汾��ɾ��ĩβһ���ַ�
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
            manager manager_ = new manager(); // ����clientMsgboxSetting��ʵ��
            manager_.ShowDialog(); // ��ģ̬���ڵķ�ʽ��ʾclientMsgboxSetting����        
        }
    }
}