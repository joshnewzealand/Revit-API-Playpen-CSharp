using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;

using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using adWin = Autodesk.Windows;

using Ookii.Dialogs.Wpf;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace _929_Bilt2020_PlaypenParent
{
    public class ParentSupportMethods
    {

        public ThisApplication myTA { get; set; }

        public string exeConfigPath(string path)
        {
            return Path.GetDirectoryName(path) + "\\" + myTA.dllName + ".dll";
        }


        public PushButtonData Button02_Uninstall(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke02_Uninstall");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\013 Button Image Uninstall.png"), UriKind.Absolute));
            return myPushButtonData;
        }

        public PushButtonData Button01_Start(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke01_Start");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button01_Start.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button02_DrawWallTypes(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke02_DrawWallTypes");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button02_DrawWallTypes.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button03_SetDefault(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke03_SetDefault");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button03_SetDefault.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button04_ManualOverrideColor(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke04_ManualOverrideColor");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button04_ManualOverrideColor.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button07_ExtensibleStorage(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke07_ExtensibleStorage");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button07_ExtensibleStorage.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button15_SingleClickFamilyBackup(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke15_SingleClickFamilyBackup");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button15_SingleClickFamilyBackup.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button1617_AddEditParameters(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke1617_AddEditParameters");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button1617_AddEditParameters.png"), UriKind.Absolute));
            return myPushButtonData;
        }
        public PushButtonData Button1819_UnderstandingTransforms(string Name, string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(Name, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke1819_UnderstandingTransforms");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\Images\\Button1819_UnderstandingTransforms.png"), UriKind.Absolute));
            return myPushButtonData;
        }

        public void PlaceButtonOnModifyRibbon()
        {
            try
            {
                String SystemTabId = "Modify";
                String SystemPanelId = "modify_shr";

                adWin.RibbonControl adWinRibbon = adWin.ComponentManager.Ribbon;

                adWin.RibbonTab adWinSysTab = null;
                adWin.RibbonPanel adWinSysPanel = null;

                adWin.RibbonTab adWinApiTab = null;
                adWin.RibbonPanel adWinApiPanel = null;
                adWin.RibbonItem adWinApiItem = null;

                foreach (adWin.RibbonTab ribbonTab in adWinRibbon.Tabs)
                {
                    // Look for the specified system tab

                    if (ribbonTab.Id == SystemTabId)
                    {
                        adWinSysTab = ribbonTab;

                        foreach (adWin.RibbonPanel ribbonPanel
                          in ribbonTab.Panels)
                        {
                            // Look for the specified panel 
                            // within the system tab

                            if (ribbonPanel.Source.Id == SystemPanelId)
                            {
                                adWinSysPanel = ribbonPanel;
                            }
                        }
                    }
                    else
                    {
                        // Look for our API tab

                        if (ribbonTab.Id == myTA.TabName)
                        {
                            adWinApiTab = ribbonTab;

                            foreach (adWin.RibbonPanel ribbonPanel in ribbonTab.Panels)
                            {
                                if (ribbonPanel.Source.Id == "CustomCtrl_%" + myTA.TabName + "%" + myTA.PanelName)
                                {
                                    foreach (adWin.RibbonItem ribbonItem in ribbonPanel.Source.Items)
                                    {
                                        if (ribbonItem.Id == "CustomCtrl_%CustomCtrl_%" + myTA.TabName + "%" + myTA.PanelName + "%" + myTA.Button_01)
                                        {
                                            adWinApiItem = ribbonItem;
                                        }
                                    }
                                }

                                if (ribbonPanel.Source.Id == "CustomCtrl_%" + myTA.TabName + "%" + myTA.PanelTransferring)
                                {
                                    adWinApiPanel = ribbonPanel;
                                }
                            }
                        }
                    }
                }


                if (adWinSysTab != null
                  && adWinSysPanel != null
                  && adWinApiTab != null
                  && adWinApiPanel != null
                   && adWinApiItem != null)
                {
                    adWinSysTab.Panels.Add(adWinApiPanel);
                    adWinApiPanel.Source.Items.Add(adWinApiItem);
                    adWinApiTab.Panels.Remove(adWinApiPanel);
                }


            }

            #region catch and finally
            catch (Exception ex)
            {
                TaskDialog.Show("me", ex.Message + Environment.NewLine + ex.InnerException);
            }
            finally
            {
            }
            #endregion
        }


        public static void writeDebug(string x, bool AndShow)
        {

            string path = (System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\` aa Joshua Lumley Secrets Debug Strings");
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            //string subdirectory_reversedatetothesecond = (path + ("\\" + (DateTime.Now.ToString("yyyyMMddHHmmss"))));
            //if (!System.IO.Directory.Exists(subdirectory_reversedatetothesecond)) System.IO.Directory.CreateDirectory(subdirectory_reversedatetothesecond);

            string FILE_NAME = (path + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");

            System.IO.File.Create(FILE_NAME).Dispose();

            System.IO.StreamWriter objWriter = new System.IO.StreamWriter(FILE_NAME, true);
            objWriter.WriteLine(x);
            objWriter.Close();

            if (AndShow) System.Diagnostics.Process.Start(FILE_NAME);
        }
    }
}
