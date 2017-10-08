using System;
using System.IO;

namespace FindCommand
{
    public class Find
    {
        public Options Options { get; set; }
        public DirectoryInfo Root { get; set; }

        public Find(Options options)
        {
            Options = options;
            Root = new DirectoryInfo(Directory.GetCurrentDirectory());
        }

        public void Execute()
        {
            string ret = "";
            Search(Root, ref ret);
            Console.WriteLine(ret);
        }

        private void Search(DirectoryInfo root, ref string output)
        {
            foreach (var diPath in Directory.GetDirectories(root.FullName))
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(diPath);
                    output += CheckFile(di);
                    Search(di, ref output);
                }
                catch
                {
                    //No permission
                }
            }

            foreach (var file in root.GetFiles())
            {
                output += CheckFile(file);
            }
        }

        private string CheckFile(FileSystemInfo file)
        {
            if (CheckByConditions(file))
            {
                return GetRightFormatString(file);
            }
            return "";
        }

        private string GetRightFormatString(FileSystemInfo fsi)
        {
            var type = fsi is DirectoryInfo ? "Dir" : "File";
            var length = fsi is FileInfo ? ((FileInfo) fsi).Length +"" : "";
            return $"{type, -4} {fsi.LastWriteTime: dd.MM.yyyy hh:mm:ss}  {length , -8}  {fsi.Name, -30} {GetRightDirectoryPath(fsi)} \n";

        }

        private string GetRightDirectoryPath(FileSystemInfo fsi)
        {
            var path = fsi is DirectoryInfo ? ((DirectoryInfo) fsi).Parent?.FullName : ((FileInfo) fsi).DirectoryName;
            return path?.Replace(Root.FullName, ".");
        }

        private bool CheckByConditions(FileSystemInfo dir)
        {
            bool ret = true;
            foreach (var condition in Options.Condtions)
            {
                ret &= condition(dir);
            }
            return ret;
        }
    }
}