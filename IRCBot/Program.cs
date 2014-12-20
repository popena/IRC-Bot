using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace IRCBot
{
    class Program
    {
        public static StreamWriter sr;

        static void Main(string[] args)
        {
            string[] botInfo = new string[4];
            botInfo = getInfo();

            Bot ircbot = new Bot(botInfo[0], Int32.Parse(botInfo[1]));
            ircbot.connectToChannel(botInfo[2], botInfo[3]);

            //discard first three messages send by the server
            for (int i = 0; i < 3; i++) { ircbot.readMessage(); }

            //save chatlog in the same folder with .sln file
            sr = new StreamWriter(@"../../../chatlog.txt");
			string message = "";
			string[] messageInfo;
            while (true)
            {
                message = ircbot.readMessage();
				Console.WriteLine (message);
                if (message.StartsWith("PING"))
                    ircbot.sendPONG(message);

				messageInfo = getMessageInfo (message);
				if (messageInfo [0] != null) {
					sr.WriteLine (messageInfo [0]+": "+messageInfo [1], 0);
					if (message == "!quit")
						break;
					else
						ircbot.Functions (messageInfo [1],messageInfo[0]);
				}
            }
            sr.Close();
            ircbot.closeConnection();
        }

        private static string formatMsg(string message, int flag)
        {
            string formatted = "";
            string[] words = message.Split(':');

            if (flag == 0)
            {
                formatted +=  "[" + DateTime.Now.ToString("HH:mm") + "]<";//24 hour clock
                formatted += message.Split(new char[] {':', '!'})[1] + "> ";//<usrname>
                formatted += words[words.Length - 1];
            }
            else
                formatted = words[words.Length - 1];

            return formatted;
        }

		private static string[] getMessageInfo(string message){
			const string pattern = @":(.+?)!(.+?)PRIVMSG(.+?):(.+)";
			Match match = Regex.Match (message, pattern);
			string []retVal = new string[2];
			if (match.Groups.Count == 5) {
				retVal [0] = match.Groups [1].Value;
				retVal [1] = match.Groups [4].Value;
			}
			return retVal;
		}

        private static string[] getInfo()
        {
            string[] info = new string[4];
            string userValue = "";

            Console.Write("Server (default: chat.freenode.net): ");
            userValue = Console.ReadLine();
            info[0] = (userValue != "") ? userValue : "chat.freenode.net";

            Console.Write("Port (default: 6667): ");
            userValue = Console.ReadLine();
            info[1] = (userValue != "") ? userValue : "6667";
            
            Console.Write("Channel (default: #irc): ");
            userValue = Console.ReadLine();
            info[2] = (userValue != "") ? userValue : "#irc";

            Console.Write("Bot's nickname (default: irc_bot): ");
            userValue = Console.ReadLine();
            info[3] = (userValue != "") ? userValue : "irc_bot";

            return info;
        }
    }
}
