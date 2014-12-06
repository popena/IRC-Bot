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

            //discard first four messages send by the server
            for (int i = 0; i < 3; i++) { ircbot.readMessage(); }

            //save chatlog in the same folder with .sln file
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

                message = formatMsg(message, 1);
                if (message == "!quit")
                    break;
                else
                    ircbot.Functions(message);
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
                formatted += message.Split(new char[] {':', '!'})[1] + "> ";
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