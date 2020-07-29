using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace _952_PRLoogleClassLibrary
{
    public class _20181201_Loaded
    {
        public static Process GetRunningInstance(bool NormalOrPopup, string ProcessName)
        {
            // Get the current process and all processes 
            // with the same name
            Process current = Process.GetCurrentProcess();
            //SearchOption so = (recurseFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            Process[] processes = Process.GetProcessesByName(NormalOrPopup ? current.ProcessName : ProcessName);

            //MessageBox.Show(processes.Count().ToString());

            //Loop through the running processes with the same name
            foreach (Process process in processes)
            {
                // Looking for a process with a different ID and
                // the same username
                if ((process.Id != current.Id)/* &&
                    UserInstance.IsSameUser(process.MainWindowHandle)*/)
                {
                    //Return the other process instance.
                    return process;
                }
            }
            return null;
        }

        public static void UseAtLoadedAndIfDeletedWhenOpen(string pathServerDirectory, string stringDirectoryOfDatabases)
        {
            if (!System.IO.Directory.Exists(pathServerDirectory)) System.IO.Directory.CreateDirectory(pathServerDirectory);

            if (!System.IO.Directory.Exists(pathParent(pathServerDirectory, stringDirectoryOfDatabases))) System.IO.Directory.CreateDirectory(pathParent(pathServerDirectory, stringDirectoryOfDatabases));

            string pathDirectoryDBsss = pathFullDB(pathParent(pathServerDirectory, stringDirectoryOfDatabases), stringDirectoryOfDatabases);
            //string path000000 = pathParent + DateTime.Now.ToString("yyyyMMddHHmmss") + ".accdb";
            if (!System.IO.File.Exists(pathDirectoryDBsss)) File.Copy(Directory.GetCurrentDirectory() + "\\New Microsoft Access Database.accdb", pathDirectoryDBsss, true);
        }

        public static string pathParent(string pathServerDirectory, string stringDirectoryOfDatabases)
        {
            return (pathServerDirectory + "~" + stringDirectoryOfDatabases + "\\");
        }

        public static string pathFullDB(string pathParentSSS, string stringDirectoryOfDatabases)
        {
            return (pathParentSSS + stringDirectoryOfDatabases + ".accdb");
        }

        public static string RegexNoLines(string subject)
        {
            return Regex.Replace(Regex.Replace(subject, @"\t|\r\n", ", "), @"\n", ", ");
        }

        // string stringSearchString = myElementType.Name + ": " + RegexNoLines(stringgLamps) + " (" + RegexNoLines(stringgSupplier) + ") " + Environment.NewLine + " (" + RegexNoLines(stringgMake) + ") " + Environment.NewLine + stringgDescription;

        public static string RegexSafety(string myElementTypeName, string Hazard, string Risk, string Mitigation, string Severity)
        {
            return myElementTypeName + ": " + RegexNoLines(Hazard) + Environment.NewLine + "Severity = " + RegexNoLines(Severity) + "." + Environment.NewLine + "(" + RegexNoLines(Risk) + ") " + Environment.NewLine + Mitigation; ;
        }

        public static string RegexLighting(string myElementTypeName, string stringgLamps, string stringgSupplier, string stringgMake, string stringgDescription )
        {
            return myElementTypeName + ": " + RegexNoLines(stringgLamps) + " (" + RegexNoLines(stringgSupplier) + ") " + Environment.NewLine + "(" + RegexNoLines(stringgMake) + ") " + Environment.NewLine + stringgDescription; ;
        }

        public static string RegexFWC(string myElementTypeName, string stringgDescription)
        {
            return myElementTypeName + ": " + stringgDescription; ;
        }

        public static string RegexLegend(string stringgDescription)
        {
            return stringgDescription; 
        }

    }
}
