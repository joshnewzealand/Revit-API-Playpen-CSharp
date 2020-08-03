using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Reflection;
using System.IO;
using Ookii.Dialogs.Wpf;
using Xceed.Wpf.Toolkit;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace _929_Bilt2020_PlaypenParent
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class InvokeSetDevelopmentPath : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                string myStringYear = commandData.Application.Application.VersionName.ToString().Substring(commandData.Application.Application.VersionName.ToString().Length - 4);

                if (!Directory.Exists(Properties.Settings.Default.DevelopmentPathRoot)) //This needs to be 'one level up' from your current project (to allow for future projects)."
                {
                    string stringProgramDataPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    string stringAppDataCompanyProductYearPath = stringProgramDataPath + "\\Autodesk\\Revit\\Macros\\" + myStringYear + "\\Revit\\AppHookup";

                    Properties.Settings.Default.DevelopmentPathRoot = stringAppDataCompanyProductYearPath;
                    Properties.Settings.Default.Save();
                }

                VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                dlg.SelectedPath = (Directory.Exists(Properties.Settings.Default.DevelopmentPathRoot) ? Properties.Settings.Default.DevelopmentPathRoot : "");
                dlg.ShowNewFolderButton = true;
                dlg.Description = "Navigate to local respository folder 'Revit-API-Playpen-CSharp', choose directory called 'Source', click 'Select Folder' button.";

                if (dlg.ShowDialog() == true)
                {
                    if (System.IO.Directory.Exists(dlg.SelectedPath))
                    {
                        Properties.Settings.Default.DevelopmentPathRoot = dlg.SelectedPath;
                        Properties.Settings.Default.Save();

                      //  Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand", true).SetValue("DEVELOPDIR", dlg.SelectedPath);

                        TaskDialog.Show("Done", "Path has been set to: " + Environment.NewLine + Properties.Settings.Default.DevelopmentPathRoot);
                    }
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                TaskDialog.Show("Catch", "Failed due to: " + ex.Message);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Invoke02 : IExternalCommand
    {
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Me", "Please use Add Remove Programs List from Control Panel." + Environment.NewLine + Environment.NewLine + "The name of the application is: 'Revit API NuGet Example 2019'." + Environment.NewLine + Environment.NewLine + "Tip: Sort by 'Installed On' date.");
            ////Manually Remove Programs from the Add Remove Programs List

            return Result.Succeeded;
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Invoke01 : IExternalCommand
    {
        public string dllModuleName { get; set; } = "_929_Bilt2020_PlaypenChild";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (Properties.Settings.Default.AssemblyNeedLoading)
                {
                    //2 August 2019: Start, The the following lines were added in Take 10 in order prevent double loading of packages.
                    Microsoft.Win32.RegistryKey rkbase = null;
                    rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
                    string stringTargetOokiiVersion = rkbase.OpenSubKey("SOFTWARE\\Wow6432Node\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("OokiiVersion").ToString();
                    string stringTargetXceedVersion = rkbase.OpenSubKey("SOFTWARE\\Wow6432Node\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("XceedVersion").ToString();
                    if (AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName == stringTargetOokiiVersion).Count() == 0)
                    {
                        string stringTargetDirectory = rkbase.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString();
                        Assembly.Load(File.ReadAllBytes(stringTargetDirectory + "\\Ookii.Dialogs.Wpf.dll"));
                    }
                    if (AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName == stringTargetXceedVersion).Count() == 0)
                    {
                        string stringTargetDirectory = rkbase.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString();
                        Assembly.Load(File.ReadAllBytes(stringTargetDirectory + "\\Xceed.Wpf.Toolkit.dll"));
                    }
                    //2 August 2019: End.


                    Properties.Settings.Default.AssemblyNeedLoading = false;
                    Properties.Settings.Default.Save();
                }

                string path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString(); ;

                Assembly objAssembly01 = Assembly.Load(File.ReadAllBytes(path + "\\" + dllModuleName + ".dll"));
                string strCommandName = "ThisApplication";

                IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                foreach (Type objType in myIEnumerableType)
                {
                    if (objType.IsClass)
                    {
                        if (objType.Name.ToLower() == strCommandName.ToLower())
                        {
                            object ibaseObject = Activator.CreateInstance(objType);
                            object[] arguments = new object[] { commandData, "Button_01_Invoke01|" + path, elements };
                            object result = null;

                            result = objType.InvokeMember("OpenWindowForm", BindingFlags.Default | BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                            break;
                        }
                    }
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                ParentSupportMethods.writeDebug("Invoke01" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);

            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;

        }
        private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Invoke01Development : IExternalCommand
    {
        public string dllModuleName { get; set; } = "_929_Bilt2020_PlaypenChild";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (Properties.Settings.Default.AssemblyNeedLoading)
                {
                    //2 August 2019: Start, The the following lines were added in Take 10 in order prevent double loading of packages.
                    Microsoft.Win32.RegistryKey rkbase = null;
                    rkbase = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64);
                    string stringTargetOokiiVersion = rkbase.OpenSubKey("SOFTWARE\\Wow6432Node\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("OokiiVersion").ToString();
                    string stringTargetXceedVersion = rkbase.OpenSubKey("SOFTWARE\\Wow6432Node\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("XceedVersion").ToString();
                    if (AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName == stringTargetOokiiVersion).Count() == 0)
                    {
                        string stringTargetDirectory = rkbase.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString();
                        Assembly.Load(File.ReadAllBytes(stringTargetDirectory + "\\Ookii.Dialogs.Wpf.dll"));
                    }
                    if (AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName == stringTargetXceedVersion).Count() == 0)
                    {
                        string stringTargetDirectory = rkbase.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString();
                        Assembly.Load(File.ReadAllBytes(stringTargetDirectory + "\\Xceed.Wpf.Toolkit.dll"));
                    }
                    //2 August 2019: End.
                }

                string path = Properties.Settings.Default.DevelopmentPathRoot + "";

                Assembly objAssembly01 = Assembly.Load(File.ReadAllBytes(path + "\\" + dllModuleName + "\\AddIn\\" + dllModuleName + ".dll"));

                string strCommandName = "ThisApplication";

                IEnumerable<Type> myIEnumerableType = GetTypesSafely(objAssembly01);
                foreach (Type objType in myIEnumerableType)
                {
                    if (objType.IsClass)
                    {
                        if (objType.Name.ToLower() == strCommandName.ToLower())
                        {
                            object ibaseObject = Activator.CreateInstance(objType);
                            object[] arguments = new object[] { commandData, "Button_01_Invoke01Development|" + path, elements };
                            object result = null;

                            result = objType.InvokeMember("OpenWindowForm", BindingFlags.Default | BindingFlags.InvokeMethod, null, ibaseObject, arguments);

                            break;
                        }
                    }
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                TaskDialog.Show("Catch", "Failed due to: " + ex.Message);

                string pathHeader = "Please check this file (and directory) exist: " + Environment.NewLine;
                string path = Properties.Settings.Default.DevelopmentPathRoot + "";
                ParentSupportMethods.writeDebug(pathHeader + path + "\\" + dllModuleName + "\\AddIn\\" + dllModuleName + ".dll", true);
            }
            finally
            {
            }
            #endregion
            return Result.Succeeded;
        }
        private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(x => x != null);
            }
        }
    }
}
