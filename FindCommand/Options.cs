using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;

namespace FindCommand
{
    public class Options
    {
        public int? Mtime { get; set; } = null;
        public string Type { get; set; } = null;
        public string Name { get; set; } = null;
        public DirectoryInfo Root { get; set; } = new DirectoryInfo(Directory.GetCurrentDirectory());
        public List<Condtion> Condtions;

        public delegate bool Condtion(FileSystemInfo fileOrDir);

        public Options(string[] args)
        {
            ParseArguments(args);
            Condtions = CreateConditions();
        }

        private void ParseArguments(string[] args)
        {
            foreach (var arg in args)
            {
                if (!arg.StartsWith("-"))
                {
                    if (!Directory.Exists(arg))
                    {
                        throw new ArgumentException($"{arg} is no valid directory path, or directory do not exist");
                    }
                    Root = new DirectoryInfo(arg);
                    continue;
                }
                var equalIndex = arg.IndexOf('=');
                if (equalIndex < 0)
                    throw new ArgumentException($"argument {arg} does not contains =. Right usage: -ARGUMENT=VALUE");
                var type = arg.Substring(1, equalIndex - 1);
                var value = arg.Substring(equalIndex + 1);

                switch (type)
                {
                    case "name":
                        Name = value;
                        break;
                    case "type":
                        if (value != "f" && value != "d")
                            throw new ArgumentException("argument type must be f for file or d for directories");
                        Type = value;
                        break;
                    case "mtime":
                        int time;
                        if (Int32.TryParse(value, out time) && time >= 0)
                            Mtime = time;
                        else
                            throw new ArgumentException("argument mtime must be set as positive integer");
                        break;
                    default:
                        throw new ArgumentException($"does not know argument {type}");
                }
            }
        }


        private List<Condtion> CreateConditions()
        {
            var conditions = new List<Condtion>();
            if (Type != null)
            {
                conditions.Add((info) =>
                {
                    var type = info is DirectoryInfo ? "d" : "f";
                    return Type == type;
                });
            }

            if (Name != null)
            {
                conditions.Add((info) => info.Name.Contains(Name));
            }

            if (Mtime != null)
            {
                conditions.Add((info) => info.LastWriteTime > DateTime.Now.AddDays(-(double) Mtime));
            }
            return conditions;
        }
    }
}