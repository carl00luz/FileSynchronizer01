using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace FileSync
{
    class MainClass
    {

        private static List<string> names = new List<string>();
        private static char delim;
        private static string[] extensions;
        public static void Main(string[] args)
        {
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                delim = '/';
            } 
            else
            {
                delim = '\\';
            }

            string inputMsg = "Input your source folder path : ";
            string errMsg = "Invalid input, try again";
            string srcPath;
            GetInput(inputMsg, errMsg, out srcPath);
            
            inputMsg = "Input your destination folder path : ";
            string destPath;
            GetInput(inputMsg, errMsg, out destPath);
            

            Console.Write("Input file extensions (various extensions must be seperated by ',')\n");
            Console.Write("ex) exe,obj,jpg : ");
            extensions = Console.ReadLine().Split(',');

            Console.WriteLine("your source path is : " + srcPath);
            Console.WriteLine("your destination path is : " + destPath);
            Console.Write("your extensions are : ");

            for (int i = 0; i < extensions.Length; ++i)
            {
                extensions[i] = extensions[i].Trim();
                Console.Write(extensions[i] + " ");
            }
            Console.WriteLine();
            Console.Write("Is this right? if it does, then enter Y or y ");
            int answer = Console.Read();

            if (answer != 'Y' && answer != 'y')
            {
                return;
            }


            string[] srcDirectory = Directory.GetFiles(srcPath);
            string[] destDirectory = Directory.GetFiles(destPath);

            foreach (string srcFile in srcDirectory)
            {
                if (!IsValidExtension(srcFile))
                {
                    continue;
                }
                string fileName = srcFile.Substring(srcFile.LastIndexOf(delim) + 1);
                foreach (string destFile in destDirectory)
                {
                    if (fileName == destFile.Substring(destFile.LastIndexOf(delim) + 1))
                    {
                        names.Add(fileName);
                        Console.WriteLine("already have " + fileName);
                    }
                }
            }

            while (true)
            {

                

                string[] directory = Directory.GetFiles(srcPath);
                foreach (string file in directory)
                {
                
                    if (IsValidExtension(file) == false)
                    {
                        continue;
                    }
                    if (!names.Contains(file.Substring(file.LastIndexOf(delim) + 1)))
                    {
                        names.Add(file.Substring(file.LastIndexOf(delim) + 1));
                
                        CopyFile(file, destPath);
                    }
                }
                
                Thread.Sleep(10000);
            }
        }

        public static bool IsValidExtension(string path)
        {
            for (int i = 0; i < extensions.Length; ++i)
            {
                if (path.Substring(path.LastIndexOf('.') + 1).Trim() == extensions[i])
                {
                    return true;
                }
            }
            return false;
        }

        public static void GetInput(string msg, string errMsg, out string path)
        {
            while (true)
            {
                Console.Write(msg);
                path = Console.ReadLine();
                if (Directory.Exists(path))
                {
                    if (path[path.Length - 1] == delim)
                    {
                        path = path.Substring(0, path.Length - 1);
                    }
                    break;
                }
                else
                {
                    Console.WriteLine(errMsg);
                }
            }
        }

        public static void CopyFile(string srcPath, string destPath)
        {
            string destFileName = srcPath.Substring(srcPath.LastIndexOf(delim) + 1);
            FileStream src = new FileStream(srcPath,
                FileMode.Open, FileAccess.Read);
            FileStream dest = new FileStream(destPath + delim + destFileName,
                FileMode.OpenOrCreate, FileAccess.Write);



            byte[] buffer = new byte[1024];
            int i = 0;
            int readCount = 0;



            while ((readCount = src.Read(buffer, 0, buffer.Length - 1)) > 0)
            {
                dest.Write(buffer, 0, readCount);
                dest.Flush();
                i += readCount;
            }

            src.Close();
            dest.Close();

        }

    }

}
