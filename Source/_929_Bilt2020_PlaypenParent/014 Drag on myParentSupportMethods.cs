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


        public PushButtonData myPushButton_01(string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(ChecklistsNumber, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke01");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\012 Button Image Properties Grid.png"), UriKind.Absolute));
            return myPushButtonData;
        }

        public PushButtonData myPushButton_02(string ChecklistsNumber, string path)
        {
            PushButtonData myPushButtonData = new PushButtonData(ChecklistsNumber, ChecklistsNumber, exeConfigPath(path), myTA.dllName + ".Invoke02");
            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\013 Button Image Uninstall.png"), UriKind.Absolute));
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
