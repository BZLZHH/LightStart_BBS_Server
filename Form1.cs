using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using XSystem.Security.Cryptography;
using XAct;
using System.Text.RegularExpressions;

namespace LightStart_BBS_server
{
    public partial class Form1 : Form
    {

        Server server = new Server();
        public static SQLiteManager manager_user;
        public static TextBox LogBox;

        public Form1()
        {

            InitializeComponent();
            LogBox = Log;
            CheckForIllegalCrossThreadCalls = false;
            // �滻Ϊ���SQLite���ݿ��ļ�·���ͱ���
            string filePath = "./server_user.db";
            string tableName = "BBS_server_user";
            manager_user = new SQLiteManager(filePath, tableName);
            // ����SQLiteManagerʵ�����������ݿ�

            bool had = false;
            try
            { manager_user.SelectAll(); had = true; }
            catch
            {
                had = false;
            }
            if (!had)
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
                log("�������\r\n");
            }

            server.Start();
            log("�������ѿ���\r\n");
        }

        public static void changeUserID(string id_before, string id_after)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("id", id_after);
            string condition = "id='" + id_before + "'";
            manager_user.Update(row, condition);
        }
        public static void changeUserName(string id, string name_after)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("name", name_after);
            string condition = "id='" + id + "'";
            manager_user.Update(row, condition);
        }
        public static void changeUserInvitationKey(string id, string invitationKey_after)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("sharedKey", invitationKey_after);
            string condition = "id='" + id + "'";
            manager_user.Update(row, condition);
        }

        public static void deleteUser(string id)
        {
            string condition = "id='" + id + "'";
            manager_user.Delete(condition);
        }

        public static int getUserGroup(string id)
        {
            Dictionary<string, string> valuePairs = getUserByIDPW(id, "");
            return int.Parse(valuePairs["usergroup"]);
        }

        public static void setUserGroup(string id, int group)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row.Add("usergroup", group.ToString());
            string condition = "id='" + id + "'";
            manager_user.Update(row, condition);
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

        public static void log(string text, bool time = true)
        {
            if (time)
                LogBox.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]");
            LogBox.AppendText(text + "\r\n");
        }

        public static string RandomStr(
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

        public static string ChangeUserToken(string id)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            string token = RandomStr(80);
            row.Add("token", token);
            string condition = "id='" + id + "'";
            manager_user.Update(row, condition);
            return token;
        }

        public static string regUser(string id, string name, string password, string salt, string sharedKey)
        {
            Dictionary<string, string> row = new Dictionary<string, string>();
            row["id"] = id;
            row["name"] = name;
            row["password"] = password;
            row["salt"] = salt;
            row["sharedKey"] = sharedKey;
            row["token"] = RandomStr(80);
            row["usergroup"] = Constants.USERGROUP_default.ToString();
            row["ban"] = "";
            row["moreInfoJson"] = "";
            manager_user.Insert(row);
            return row["token"];
        }

        void restartServer()
        {
            /*
            server.Stop();
            server.Start();
            log("������������\r\n");
            */
        }

        private void restart_Click(object sender, EventArgs e)
        {
            restartServer();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            log("���ڹر����ݿ⼰������\r\n");
            manager_user.Close();
            server.Stop();
        }

        public static bool checkIDAvailable(string id)
        {
            bool result = true;
            List<Dictionary<string, string>> list = manager_user.SelectAll();
            foreach (Dictionary<string, string> kvp in list)
            {
                if (kvp["id"] == id)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public static Dictionary<string, string> getUserByIDPW(string id, string pw)
        {
            List<Dictionary<string, string>> list = manager_user.SelectAll();
            Dictionary<string, string> user = new Dictionary<string, string>();
            bool idAvailable = false;
            foreach (Dictionary<string, string> kvp in list)
            {
                if (kvp["id"] == id)
                {
                    user = kvp;
                    idAvailable = true;
                    break;
                }
            }
            if (idAvailable)
            {

                if (SHA256(SHA256(pw) + SHA256(user["salt"]) + SHA1(pw + user["salt"])) == user["password"])
                {
                    user.Add("available", "true");
                }
                else
                {
                    user.Add("available", "false");
                }
            }
            else
            {
                user.Add("available", "false");
            }

            return user;
        }

        public static Dictionary<string, string> getUserByToken(string token)
        {
            List<Dictionary<string, string>> list = manager_user.SelectAll();
            Dictionary<string, string> user = new Dictionary<string, string>();
            foreach (Dictionary<string, string> kvp in list)
            {
                if (kvp["token"] == token)
                {
                    user = kvp;
                    user.Add("available", "true");
                    return user;
                }
            }

            user.Add("available", "false"); // û�л�ȡ��ʱ
            return user;
        }

        public static string SHA256(string str)
        {
            //���str�����ģ���ͬEncoding��sha�ǲ�ͬ�ģ���
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);

            SHA256Managed Sha256 = new SHA256Managed();
            byte[] by = Sha256.ComputeHash(SHA256Data);

            return BitConverter.ToString(by).Replace("-", "").ToLower(); //64
                                                                         //return Convert.ToBase64String(by);                         //44
        }
        public static string SHA1(string str)
        {
            //���str�����ģ���ͬEncoding��sha�ǲ�ͬ�ģ���
            byte[] SHA1Data = Encoding.UTF8.GetBytes(str);

            SHA1Managed Sha1 = new SHA1Managed();
            byte[] by = Sha1.ComputeHash(SHA1Data);

            return BitConverter.ToString(by).Replace("-", "").ToLower(); //64
                                                                         //return Convert.ToBase64String(by);                         //44
        }

        private void GetUsersInfo_Click(object sender, EventArgs e)
        {
            List<Dictionary<string, string>> data = manager_user.SelectAll();
            log("==============================\n", false);
            ;            // ����������
            foreach (var item in data)
            {
                log(string.Join("\r\n", item.Values) + "\r\n\r\n", false);
            }
            log("==============================\n", false);
        }

        private void clientMsgboxChange_Click(object sender, EventArgs e)
        {
            clientMsgboxSetting settingForm = new clientMsgboxSetting(); // ����clientMsgboxSetting��ʵ��
            settingForm.ShowDialog(); // ��ģ̬���ڵķ�ʽ��ʾclientMsgboxSetting����        
        }

        public class Constants
        {
            public const int version = 2;

            public static readonly string[] USERGROUPS = {"default","vip","admin"}; // ˳��Ҫ������һ��
            public const int USERGROUP_default = 0;
            public const int USERGROUP_vip = 1;
            public const int USERGROUP_admin = 2;
        }; 

        public class SQLiteManager
        {
            private SQLiteConnection connection;
            private SQLiteCommand command;
            private string tableName;

            // ���캯�����������ݿ�
            public SQLiteManager(string filePath, string tableName)
            {
                this.tableName = tableName;
                connection = new SQLiteConnection("Data Source=" + filePath);
                connection.Open();
                command = new SQLiteCommand(connection);
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

            // ��ѯ���е�������
            public List<Dictionary<string, string>> SelectAll()
            {
                string query = "SELECT * FROM " + tableName;
                return ExecuteQuery(query);
            }

            // ��ѯ������������
            public List<Dictionary<string, string>> SelectWhere(string condition)
            {
                string query = "SELECT * FROM " + tableName + " WHERE " + condition;
                return ExecuteQuery(query);
            }

            // ����һ������
            public void Insert(Dictionary<string, string> row)
            {
                string columns = "";
                string values = "";
                foreach (string column in row.Keys)
                {
                    columns += column + ",";
                    values += "'" + row[column] + "',";
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
                    setValues += column + "='" + row[column] + "',";
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

                        //("Client connected from " + client.RemoteEndPoint.ToString());

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

            public JObject toJson()
            {
                JObject result = new JObject();
                result.Add("type", this.type);
                result.Add("state", this.state);
                result.Add("data", this.data);
                result.Add("token", this.token);
                if (version != -1) // versionΪ-1ʱ,��ʾ�û�ʹ�õ�����汾����,��֧�ֽ���json�е�version,���Բ����version
                {
                    result.Add("version", this.version);
                }
                return result;
            }

            public string toJsonString()
            {
                string startText = "";
                if (version == -1)
                {
                    startText = "oldVersion";
                }
                return startText + this.toJson().ToString();
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
            public const short MESSAGE_GET_SECTIONS = 4;
            public const short MESSAGE_RETURN_SECTIONS = -4;

            // �ַ����� (ֱ���յ��ַ���)
            public const string MESSAGE_GET_ACTIVE = "active";
            public const string MESSAGE_GET_MSGBOX = "msgbox";
            // end

            private readonly Socket _client;

            public bool CheckIDValidity(string str)
            {
                Regex regex = new Regex("^[a-zA-Z0-9_]*$");
                return !regex.IsMatch(str) && str.Length >= 2 && str.Length <= 40;
            }
            public bool CheckNameValidity(string str)
            {
                Regex regex = new Regex(@"^[^\p{C}]+$");
                return !regex.IsMatch(str) && str.Length >= 2 && str.Length <= 36;
            }

            public Connection(Socket client)
            {
                _client = client;
            }

            public string Action_Reg(Message message)
            {
                string result = "";
                JObject userInfoJson = JObject.Parse(message.data);
                JObject sendedJson = message.toJson();
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
                    err = "The LightStart BBS version you are using is not supported. Please updata to the newest version available."; // ���ṩversion�İ汾��֧��utf-8,ʹ��ascii
                }


                if (err == "")
                {
                    string token = Form1.regUser(userInfoJson["id"].ToString(), userInfoJson["name"].ToString().Replace(" ",""), userInfoJson["password"].ToString(), userInfoJson["salt"].ToString(), userInfoJson["sharedKey"].ToString());
                    result = new Message(MESSAGE_RETURN_TOKEN, true, token, "server").toJsonString();

                    Form1.log(">> �û� " + userInfoJson["id"].ToString() + " ע��: " + userInfoJson.ToString());
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_TOKEN, false, "ERROR: " + err, "server", return_old_verison).toJsonString();
                }
                return result;
            }

            public string Action_Login(Message message)
            {
                string result = "";
                JObject userInfoJson = JObject.Parse(message.data);
                JObject sendedJson = message.toJson();
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
                    err = "The LightStart BBS version you are using is not supported. Please updata to the newest version available."; // ���ṩversion�İ汾��֧��utf-8,ʹ��ascii
                }

                if (err == "")
                {
                    Form1.ChangeUserToken(userInfoJson["id"].ToString());
                    user = Form1.getUserByIDPW(userInfoJson["id"].ToString(), userInfoJson["password"].ToString());
                    result = new Message(MESSAGE_RETURN_TOKEN, true, user["token"], "server").toJsonString();

                    Form1.log(">> �û� " + userInfoJson["id"].ToString() + " ��¼: " + userInfoJson.ToString());
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_TOKEN, false, "ERROR: " + err, "server", return_old_verison).toJsonString();
                }
                return result;
            }

            public string[] Action_GetUserInfo(Message message) //�����践�ص�message��id 
            {
                string result = "";
                Dictionary<string, string> user = Form1.getUserByToken(message.token);
                if (user["available"] == "true")
                {
                    JObject userInfo = new JObject();
                    userInfo.Add("id", user["id"]);
                    userInfo.Add("name", user["name"]);
                    userInfo.Add("sharedKey", user["sharedKey"]);
                    result = new Message(MESSAGE_RETURN_USER_INFO, true, userInfo.ToString(), "server").toJsonString();
                }
                else
                {
                    result = new Message(MESSAGE_RETURN_USER_INFO, false, "ERROR: �û���¼��Ϣ��ʧЧ", "server").toJsonString();
                }
                string[] return_value = new string[2];
                return_value[0] = result;
                if (user["available"] == "true")
                    return_value[1] = user["id"];
                else
                    return_value[1] = "Wrong Token";
                return return_value;
            }

            public void Process()
            {
                try
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesReceived = _client.Receive(buffer);

                        if (bytesReceived == 0)
                        {
                            break;
                        }

                        //("Received data from client: " + System.Text.Encoding.UTF8.GetString(buffer, 0, bytesReceived));

                        // �������ﴦ���ȡ�������ݲ���ͻ���д����Ӧ
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                        System.Diagnostics.Debug.WriteLine(receivedData);
                        try
                        {
                            bool switch_runned = false;
                            switch (receivedData.Trim('\n')) //ֻ��Ҫ�ӷ���˻�ȡ�Ĳ��� ����token
                            {
                                case MESSAGE_GET_ACTIVE:
                                    {
                                        switch_runned = true;
                                        sendData("true");
                                        Form1.log("���û���������Active");
                                        break;
                                    }
                                case MESSAGE_GET_MSGBOX:
                                    {

                                        switch_runned = true;
                                        sendData(Form1.ReadMsgConfigConfig());
                                        Form1.log("���û���ȡ������Ϣ");
                                        break;
                                    }
                            }
                            if(!switch_runned)
                            {
                                Message message = JsonConvert.DeserializeObject<Message>(receivedData); System.Diagnostics.Debug.WriteLine(message.token);
                                string return_msg = "";
                                if (message.token == "null") //��token����
                                {
                                    Form1.log("�յ� ��token ��Ϣ: \n" + receivedData);
                                    switch (message.type)
                                    {
                                        case MESSAGE_ACTION_REG: //ע��
                                            {
                                                return_msg = Action_Reg(message);
                                                sendData(return_msg);
                                                break;
                                            }
                                        case MESSAGE_GET_TOKEN_IDPW: //id+pw��¼,��ȡtoken
                                            {
                                                return_msg = Action_Login(message);
                                                sendData(return_msg);
                                                break;
                                            }
                                    }
                                    Form1.log("���� ��token ��Ϣ: \n" + return_msg);
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
                                                sendData(return_msg);
                                                break;
                                            }
                                    }
                                    if (id != "")
                                    {
                                        Form1.log("�յ� " + id + " ��Ϣ: \n" + receivedData);
                                        Form1.log("���� " + id + " ��Ϣ: \n" + return_msg);
                                    }
                                    else
                                    {
                                        Form1.log("�յ� ʧЧtoken�û� ��Ϣ: \n" + receivedData);
                                        Form1.log("���� ʧЧtoken�û� ��Ϣ: \n" + return_msg);
                                    }
                                }

                                
                            }
                        }
                        catch (Exception ex)
                        {
                            sendData(new Message(MESSAGE_ERROR, false, ex.Message, "server").toJsonString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //(ex.ToString());
                }
                finally
                {
                    _client.Close();
                }
            }

            public void sendData(string data)
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

            public void close()
            {
                _client.Shutdown(SocketShutdown.Both);
                _client.Close();
            }
        }

        private void userGroupChange_Click(object sender, EventArgs e)
        {
            userManager userManager_ = new userManager(); // ����clientMsgboxSetting��ʵ��
            userManager_.ShowDialog(); // ��ģ̬���ڵķ�ʽ��ʾclientMsgboxSetting����        
        }
    }
}