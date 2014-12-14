using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace IRCBot
{
    class Bot
    {
        private TcpClient irc;
        private Stream data;
        private StreamReader reader;
        private StreamWriter writer;
        private string channel = "";
        private string nick = "";

        public Bot(string server, int port)
        {
            try
            {
                irc = new TcpClient(server, port);
                data = irc.GetStream();
                reader = new StreamReader(data);
                writer = new StreamWriter(data) { NewLine = "\r\n", AutoFlush = true };

            }
            catch (Exception e)
                Console.WriteLine(e.ToString());
        }

        public void closeConnection()
        {
            irc.Close();
            data.Close();
            writer.Close();
            reader.Close();
        }

        public string readMessage()
        {
            return reader.ReadLine().ToString();
        }

        public void sendMessage(string message)
        {
            writer.WriteLine("PRIVMSG " + channel + " :" + message);
            message = "[" + DateTime.Now.ToString("HH:mm") + "]<" + nick + "> " + message;
            Program.sr.WriteLine(message);
        }

        public void connectToChannel(string channel, string nick)
        {
            this.channel = channel;
            this.nick = nick;
            Console.WriteLine("\nConnecting, this may take a while...");

            writer.WriteLine("USER ircbot irc bot :irc_bot");
            writer.WriteLine("NICK " + nick);

            Console.WriteLine("Waiting for ping request...");

            string message = "";
            while (true)
            {
                message = reader.ReadLine();
                if (message.StartsWith("PING"))
                {
                    sendPONG(message);
                    Console.WriteLine("Pong sent!");
                    break;
                }
            }

            writer.WriteLine("JOIN " + channel);

            Console.WriteLine("Successfully connected!");
        }

        public void sendPONG(string msg)
        {
            msg = msg.Split(' ')[1];
            writer.WriteLine("PONG " + msg);
        }

        public void Functions(string msg)
        {
            if (msg.StartsWith("!greet"))
                sendMessage("Hello " + msg.Split(' ')[1]);
            else if (msg == "!date")
                sendMessage(DateTime.Now.ToString());
            else
                Analyze(msg);
        }

        private void Analyze(string msg)
        {
            string linkPattern = @"((http(s)?://)?(www.)?(.*)?.+\.(.){1,})(.+)?";

            if (Regex.IsMatch(msg, linkPattern, RegexOptions.Singleline))
            {
                string url = Regex.Match(msg, linkPattern, RegexOptions.Singleline).ToString();
                if (url.StartsWith("//"))
                    url = "http:" + url;
                else if (!url.StartsWith("//"))
                    url = "http://" + url;

                string sourceCode = "";
                string titleContent = "";

                try
                {
                    WebClient wb = new WebClient();
                    wb.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    Stream data = wb.OpenRead(url);
                    StreamReader sr = new StreamReader(data);
                    sourceCode = sr.ReadToEnd();

                    string titlePattern = @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>";
                    titleContent = Regex.Match(sourceCode, titlePattern, RegexOptions.Multiline).Groups["Title"].Value;

                    titleContent = "[URL] " + WebUtility.HtmlDecode(titleContent);
                }
                catch (Exception ex)
                {
                    //error handling needs to be enhanced
                }
                sendMessage(titleContent);
            }
            else if (msg == msg.ToUpper())
                sendMessage("Warning! Stop typing in caps!");
        }
    }//class
}//namespace
