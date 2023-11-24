internal static class Form1Helpers
{

    public static void Log(string text, bool time = true, bool saveToFile = true, string ip = "")
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
}