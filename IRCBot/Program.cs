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
        static void Main(string[] args)
        {
            Bot ircbot = new Bot("chat.freenode.net", 6667);
            ircbot.connectToChannel("#elgisbottest", "superPotti");

            StreamWriter sr = new StreamWriter(@"../../../chatlog.txt");
            string message = "";

            while (true)
            {
                message = ircbot.readMessage();

                if (message.StartsWith("PING"))
                {
                    ircbot.sendPONG(message);
                    Console.WriteLine(message);
                }

                sr.WriteLine(formatMsg(message, 0));

                if (formatMsg(message, 1) == "!quit")
                    break;
                else
                    ircbot.Functions(formatMsg(message, 1));
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
                var date = DateTime.Now;
                formatted = "[" + date.Hour + ":" + date.Minute + "]<";
                formatted += words[1] + "> ";//username,
                formatted += words[words.Length - 1];
            }
            else
            {
                formatted = words[words.Length - 1];
            }
            return formatted;
        }
    }
}