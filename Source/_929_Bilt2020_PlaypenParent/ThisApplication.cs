/*
 * Created by SharpDevelop.
 * User: Joshua
 * Date: 22/02/2020
 * Time: 3:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using System.IO;
//using System.Windows.Media.Imaging;
using adWin = Autodesk.Windows;

using Ookii.Dialogs.Wpf;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace _929_Bilt2020_PlaypenParent
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("A633825E-1E3D-4EB3-BED1-84D017A5B2F4")]
	public partial class ThisApplication : IExternalApplication
	{
        public string dllName { get; set; } = "_929_Bilt2020_PlaypenParent";
        public string TabName { get; set; } = "C# Playpen";
        public string PanelName { get; set; } = "cSharpPlaypen joshnewzealand";
        public string PanelTransferring { get; set; } = "Modeless Properties";
        public string Button_01 { get; set; } = "Properties Grid";
        public string Button_02_Uninstall { get; set; } = "Uninstall";

        public string Button01_Start { get; set; } = "Full Window";
        public string Button02_DrawWallTypes { get; set; } = "Draw Wall Types";
        public string Button03_SetDefault { get; set; } = "Set Default Type";
        public string Button04_ManualOverrideColor { get; set; } = "Manual Color Override";
        public string Button07_ExtensibleStorage { get; set; } = "Extensible Storage Example";
        public string Button15_SingleClickFamilyBackup { get; set; } = "Single Click Family Backup";
        public string Button1617_AddEditParameters { get; set; } = "Add Edit Parameters";
        public string Button1819_UnderstandingTransforms { get; set; } = "Understanding Transforms";
        public string path { get; set; } = Assembly.GetExecutingAssembly().Location;

        RibbonPanel RibbonPanelCurrent { get; set; }
        RibbonPanel RibbonPanelWithSingleButtonForLater { get; set; }

        private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

        public Result OnShutdown(UIControlledApplication a)
        { return Result.Succeeded; }

        public Result OnStartup(UIControlledApplication a)
        {
            ParentSupportMethods myParentSupportMethods = new ParentSupportMethods();
            myParentSupportMethods.myTA = this;

            string stringCommand01Button = "Set Development Path Root";

            Properties.Settings.Default.AssemblyNeedLoading = true;
            Properties.Settings.Default.Save();

            String exeConfigPath = Path.GetDirectoryName(path) + "\\" + dllName + ".dll";
            a.CreateRibbonTab(TabName);
            RibbonPanelCurrent = a.CreateRibbonPanel(TabName, PanelName);

            PushButtonData myPushButtonData01 = new PushButtonData(stringCommand01Button, stringCommand01Button, exeConfigPath, dllName + ".InvokeSetDevelopmentPath");

            ComboBoxData cbData = new ComboBoxData("DeveloperSwitch") { ToolTip = "Select an Option", LongDescription = "Select a number or letter" };
            ComboBox ComboBox01 = RibbonPanelCurrent.AddStackedItems(cbData, myPushButtonData01)[0] as ComboBox;

            string stringProductVersion = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("ProductVersion").ToString();
            //Bug fix here by Max Sun (01/05/19)

            ComboBox01.AddItem(new ComboBoxMemberData("Release", "Release: " + stringProductVersion));
            ComboBox01.AddItem(new ComboBoxMemberData("Development", "C# Developer Mode"));
            ComboBox01.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(SwitchBetweenDeveloperAndRelease);

            //RibbonPanelCurrent.AddItem(myParentSupportMethods.myPushButton_01(Button_01, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button01_Start("Button01_Start", Button01_Start, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button02_DrawWallTypes("Button02_DrawWallTypes", Button02_DrawWallTypes, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button03_SetDefault("Button03_SetDefault", Button03_SetDefault, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button04_ManualOverrideColor("Button04_ManualOverrideColor", Button04_ManualOverrideColor, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button07_ExtensibleStorage("Button07_ExtensibleStorage", Button07_ExtensibleStorage, path));

            PushButtonData myPushButtonData_OneClickBackup = myParentSupportMethods.Button15_SingleClickFamilyBackup("Button15_SingleClickFamilyBackup", Button15_SingleClickFamilyBackup, path);
            PushButtonData myPushButtonData_OpenBackupFolder = new PushButtonData("Button15_OpenBackupFolder", "Open Backup Folder", exeConfigPath, dllName + ".InvokeOpenBackupFolder");

            SplitButtonData sb1 = new SplitButtonData("OneClickBackupSystem", "One Click Backup System");
            SplitButton sb = RibbonPanelCurrent.AddItem(sb1) as SplitButton;
            sb.AddPushButton(myPushButtonData_OneClickBackup);
            sb.AddPushButton(myPushButtonData_OpenBackupFolder);
            sb.IsSynchronizedWithCurrentItem = false;



            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button1617_AddEditParameters("Button1617_AddEditParameters", Button1617_AddEditParameters, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button1819_UnderstandingTransforms("Button1819_UnderstandingTransforms", Button1819_UnderstandingTransforms, path));
            RibbonPanelCurrent.AddItem(myParentSupportMethods.Button02_Uninstall("Button_02_Uninstall", Button_02_Uninstall, path));

            //PRLChecklistsPanel2.Visible = false;
            //RibbonPanel PRLChecklistsPanel2 = a.CreateRibbonPanel(TabName, PanelTransferring);
            //myParentSupportMethods.PlaceButtonOnModifyRibbon();
            return Result.Succeeded;
        }
        public void SwitchBetweenDeveloperAndRelease(object sender, Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs e)
        {
            try
            {
                ComboBox cBox = sender as ComboBox;

                //PushButton myPushButton00 = RibbonPanelCurrent.GetItems()[1] as PushButton;
                PushButton myPushButton01_Start = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button01_Start").First() as PushButton;
                PushButton myPushButton02_DrawWallTypes = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button02_DrawWallTypes").First() as PushButton;
                PushButton myPushButton03_SetDefault = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button03_SetDefault").First() as PushButton;
                PushButton myPushButton04_ManualOverrideColor = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button04_ManualOverrideColor").First() as PushButton;
                PushButton myPushButton07_ExtensibleStorage = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button07_ExtensibleStorage").First() as PushButton;
                SplitButton mSplitButton_OneClickBackupSystem = RibbonPanelCurrent.GetItems().Where(x => x.Name == "OneClickBackupSystem").First() as SplitButton;
                PushButton myPushButton15_SingleClickFamilyBackup = mSplitButton_OneClickBackupSystem.GetItems()[0] as PushButton;
                PushButton myPushButton1617_AddEditParameters = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button1617_AddEditParameters").First() as PushButton;
                PushButton myPushButton1819_UnderstandingTransforms = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button1819_UnderstandingTransforms").First() as PushButton;
                PushButton myPushButton_02_Uninstall = RibbonPanelCurrent.GetItems().Where(x => x.Name == "Button_02_Uninstall").First() as PushButton;


                if (cBox.Current.Name == "Development") myPushButton_02_Uninstall.ClassName = dllName + ".Invoke02_Uninstall";
                if (cBox.Current.Name == "Development") myPushButton01_Start.ClassName = dllName + ".DevInvoke01_Start";
                if (cBox.Current.Name == "Development") myPushButton02_DrawWallTypes.ClassName = dllName + ".DevInvoke02_DrawWallTypes";
                if (cBox.Current.Name == "Development") myPushButton03_SetDefault.ClassName = dllName + ".DevInvoke03_SetDefault";
                if (cBox.Current.Name == "Development") myPushButton04_ManualOverrideColor.ClassName = dllName + ".DevInvoke04_ManualOverrideColor";
                if (cBox.Current.Name == "Development") myPushButton07_ExtensibleStorage.ClassName = dllName + ".DevInvoke07_ExtensibleStorage";
                if (cBox.Current.Name == "Development") myPushButton15_SingleClickFamilyBackup.ClassName = dllName + ".DevInvoke15_SingleClickFamilyBackup";
                if (cBox.Current.Name == "Development") myPushButton1617_AddEditParameters.ClassName = dllName + ".DevInvoke1617_AddEditParameters";
                if (cBox.Current.Name == "Development") myPushButton1819_UnderstandingTransforms.ClassName = dllName + ".DevInvoke1819_UnderstandingTransforms";

                if (cBox.Current.Name == "Release") myPushButton_02_Uninstall.ClassName = dllName + ".Invoke02_Uninstall";
                if (cBox.Current.Name == "Release") myPushButton01_Start.ClassName = dllName + ".Invoke01_Start";
                if (cBox.Current.Name == "Release") myPushButton02_DrawWallTypes.ClassName = dllName + ".Invoke02_DrawWallTypes";
                if (cBox.Current.Name == "Release") myPushButton03_SetDefault.ClassName = dllName + ".Invoke03_SetDefault";
                if (cBox.Current.Name == "Release") myPushButton04_ManualOverrideColor.ClassName = dllName + ".Invoke04_ManualOverrideColor";
                if (cBox.Current.Name == "Release") myPushButton07_ExtensibleStorage.ClassName = dllName + ".Invoke07_ExtensibleStorage";
                if (cBox.Current.Name == "Release") myPushButton15_SingleClickFamilyBackup.ClassName = dllName + ".Invoke15_SingleClickFamilyBackup";
                if (cBox.Current.Name == "Release") myPushButton1617_AddEditParameters.ClassName = dllName + ".Invoke1617_AddEditParameters";
                if (cBox.Current.Name == "Release") myPushButton1819_UnderstandingTransforms.ClassName = dllName + ".Invoke1819_UnderstandingTransforms";


                string FILE_NAME = System.Environment.GetEnvironmentVariable("ProgramData") + "\\Pedersen Read Limited"; // cSharpPlaypen joshnewzealand

                if (true) //grouping for clarity will alwasy be true
                {
                    if (!System.IO.Directory.Exists(FILE_NAME)) System.IO.Directory.CreateDirectory(FILE_NAME);
                    FILE_NAME = FILE_NAME + "\\cSharpPlaypen joshnewzealand"; // 
                    if (!System.IO.Directory.Exists(FILE_NAME)) System.IO.Directory.CreateDirectory(FILE_NAME);
                    FILE_NAME = (FILE_NAME + "\\Location Of Shared Parameters File.txt");
                }

                if (true) //write line
                {
                    string path = "";
                    if (cBox.Current.Name == "Release")
                    {
                        path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Pedersen Read Limited\\cSharpPlaypen joshnewzealand").GetValue("TARGETDIR").ToString();
                    }
                    if (cBox.Current.Name == "Development")
                    {
                        string dllModuleName = "_929_Bilt2020_PlaypenChild";
                        path = Properties.Settings.Default.DevelopmentPathRoot + "\\" + dllModuleName + "\\AddIn";
                    }

                    System.IO.File.Create(FILE_NAME).Dispose();
                    System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true);
                    objWriter.WriteLine(path);
                    objWriter.Close();
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
        }

        #region Revit Macros generated code
        private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
	}


}