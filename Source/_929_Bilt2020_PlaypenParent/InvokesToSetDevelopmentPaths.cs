using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Reflection;
using System.IO;
using Ookii.Dialogs.Wpf;
using Xceed.Wpf.Toolkit;

using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using MessageBox = System.Windows.Forms.MessageBox;

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
    public class Invoke02_Uninstall : IExternalCommand
    {
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Me", "Please use Add Remove Programs List from Control Panel." + Environment.NewLine + Environment.NewLine + "The name of the application is: 'cSharpPlaypen joshnewzealand'." + Environment.NewLine + Environment.NewLine + "Tip: Sort by 'Installed On' date.");
            ////Manually Remove Programs from the Add Remove Programs List

            return Result.Succeeded;
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class InvokeOpenBackupFolder : IExternalCommand
    {
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                FamilyInstance myFamilyInstance; //candidate for methodisation 202004251537

                string myString_FamilyBackups_FamilyName = "";
                Family myFamily = null;
                string myString_FamilyName = "";


                if (doc.PathName == "")
                {
                    MessageBox.Show("Please save project file.");
                    return Result.Succeeded;
                }


                if (true)//candidate for methodisation 202004251537
                {

                    if (uidoc.Selection.GetElementIds().Count != 1)
                    {
                        MessageBox.Show("Please select just one element instance.");
                        return Result.Succeeded;
                    }

                    if (doc.GetElement(uidoc.Selection.GetElementIds().First()).GetType() != typeof(FamilyInstance))
                    {
                        MessageBox.Show("Please select an element of type - 'FamilyInstance'.");
                        return Result.Succeeded;
                    }

                    myFamilyInstance = doc.GetElement(uidoc.Selection.GetElementIds().First()) as FamilyInstance;

                    myFamily = ((FamilySymbol)doc.GetElement(myFamilyInstance.GetTypeId())).Family;

                    if (!myFamily.IsEditable)
                    {
                        MessageBox.Show("Family is not editable.");
                        return Result.Succeeded;
                    }

                    string myString_FamilyBackups = System.IO.Path.GetDirectoryName(doc.PathName) + "\\Family Backups";

                    if (!System.IO.Directory.Exists(myString_FamilyBackups))
                    {
                        System.IO.Directory.CreateDirectory(myString_FamilyBackups);
                    }

                    myString_FamilyName = myFamily.Name;
                    string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());

                    foreach (char c in invalid)
                    {
                        myString_FamilyName = myString_FamilyName.Replace(c.ToString(), "");
                    }

                    myString_FamilyBackups_FamilyName = myString_FamilyBackups + "\\" + myString_FamilyName;

                    if (!System.IO.Directory.Exists(myString_FamilyBackups_FamilyName))
                    {
                        System.IO.Directory.CreateDirectory(myString_FamilyBackups_FamilyName);
                    }

                }
                System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(myString_FamilyBackups_FamilyName + "\\"));
            }

            #region catch and finally
            catch (Exception ex)
            {
                MessageBox.Show("my15OpenFolderButton_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException);
            }
            finally
            {
            }
            #endregion

            return Result.Succeeded;
        }
    }

}
