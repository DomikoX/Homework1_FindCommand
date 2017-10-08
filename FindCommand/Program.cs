using System;

namespace FindCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Options opt = new Options(args);
                Find find =  new Find(opt);
                find.Execute();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }

        }
    }
}
