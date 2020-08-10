using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Networking
{
    public static class CommandManager
    {
        public static bool TryExecute(byte[] input)
        {
            if (input.SequenceEqual(new byte[64]))
            {
                return false;
            }

            Command command = Deserialize(input, out var arg);
            switch (command)
            {
                case Command.Print:
                    Debug.Log(arg);
                    return true;
                case Command.SendMessage:
                    return SendMessage(arg);
                case Command.Broadcast:
                    return BroadcastMessage(arg);
                case Command.SetId:
                    return SetId(arg);
                default:
                    return false;
            }
        }

        public static Command Deserialize(byte[] input, out string arg)
        {
            Command command = (Command)BitConverter.ToInt32(input, 0);
            arg = Encoding.Unicode.GetString(input, 4, input.Length - 4);
            return command;
        }
        public static byte[] Serialize(Command command, string arg)
        {
            byte[] result = new byte[arg.Length * sizeof(char) + sizeof(Command)];
            BitConverter.GetBytes((int)command).CopyTo(result, 0);
            for (int i = 0; i < arg.Length; i++)
            {
                BitConverter.GetBytes(arg[i]).CopyTo(result, sizeof(Command) + i * sizeof(char));
            }

            return result;
        }

        public static bool SetId(string arg)
        {
            NetworkManager.Id = Convert.ToInt32(arg);
            return true;
        }
        private static bool SendMessage(string arg)
        {
            return false;
        }

        private static bool BroadcastMessage(string arg)
        {
            return false;
        }
    }
}
