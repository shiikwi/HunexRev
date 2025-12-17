using System;
using System.IO;
using System.Text;

namespace VmDsm
{
    class Program
    {
        static void Main(string[] args)
        {

            var parse = new unas_EXEC();
            foreach(var inpath in args)
            {
                var outpath = Path.Combine(Path.GetDirectoryName(inpath)!, Path.GetFileNameWithoutExtension(inpath) + ".txt");
                parse.Parse(inpath, outpath);
                Console.WriteLine($"Disassemble {Path.GetFileName(inpath)} Success" );
            }

            Console.WriteLine();
            Console.WriteLine("Vm disassemble Finish......");
            Console.ReadKey();
        }
    }

}
