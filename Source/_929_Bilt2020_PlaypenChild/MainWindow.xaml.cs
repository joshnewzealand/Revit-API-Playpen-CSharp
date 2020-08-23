/*
 * Created by SharpDevelop.
 * User: Joshua
 * Date: 22/02/2020
 * Time: 3:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System.Linq;
using _952_PRLoogleClassLibrary;
using System.Windows.Media.Media3D;

using Numerics = System.Numerics;
using Transform = Autodesk.Revit.DB.Transform;
using Binding = Autodesk.Revit.DB.Binding;
using Autodesk.Revit.DB.Events;
using System.IO;
using System.Runtime.InteropServices;

namespace _929_Bilt2020_PlaypenChild
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        public EE02_OneOfEachWall myEE02_OneOfEachWall { get; set; }
        public ExternalEvent myExternalEvent_EE02_OneOfEachWall { get; set; }

        public EE03_SetDefaultType myEE03_SetDefaultType { get; set; }
        public ExternalEvent myExternalEvent_EE03_SetDefaultType { get; set; }

        public EE04_ManualColorOverride myEE04_ManualColorOverride { get; set; }
        public ExternalEvent myExternalEvent_EE04_ManualColorOverride { get; set; }

        public EE05_LoadAllFamilies myEE05_LoadAllFamilies { get; set; }
        public ExternalEvent myExternalEvent_EE05_LoadAllFamilies { get; set; }

        public EE06_PlaceAFamily_OnDoubleClick myEE06_PlaceAFamily_OnDoubleClick { get; set; }
        public ExternalEvent myExternalEvent_EE06_PlaceAFamily_OnDoubleClick { get; set; }

        public EE07_RotatingEntities myEE07_RotatingEntities { get; set; }
        public ExternalEvent myExternalEvent_EE07_RotatingEntities { get; set; }

        public EE08_MoveElementAroundHostingSurface myEE08_MoveElementAroundHostingSurface { get; set; }
        public ExternalEvent myExternalEvent_EE08_MoveElementAroundHostingSurface { get; set; }

        public EE09_Draw3D_ModelLines myEE09_Draw3D_ModelLines { get; set; }
        public ExternalEvent myExternalEvent_EE09_Draw3D_ModelLines { get; set; }

        public EE10_Draw2D_DetailLines myEE10_Draw2D_DetailLines { get; set; }
        public ExternalEvent myExternalEvent_EE10_Draw2D_DetailLines { get; set; }

        public EE11_GridOutCirclesOnFace myEE11_GridOutCirclesOnFace { get; set; }
        public ExternalEvent myExternalEvent_EE11_GridOutCirclesOnFace { get; set; }

        public EE12_SetupRoom myEE12_SetupRoom { get; set; }
        public ExternalEvent myExternalEvent_EE12_SetupRoom { get; set; }

        public EE13_ExtensibleStorage_DeleteAll myEE13_ExtensibleStorage_DeleteAll { get; set; }
        public ExternalEvent myExternalEvent_EE13_ExtensibleStorage_DeleteAll { get; set; }

        public EE13_ExtensibleStorage_DeleteItem myEE13_ExtensibleStorage_DeleteItem { get; set; }
        public ExternalEvent myExternalEvent_EE13_ExtensibleStorage_DeleteItem { get; set; }

        public EE13_ExtensibleStorage_zRandomise myEE13_ExtensibleStorage_zRandomise { get; set; }
        public ExternalEvent myExternalEvent_EE13_ExtensibleStorage_zRandomise { get; set; }

        public EE13_ExtensibleStorage_NewOrSave myEE13_ExtensibleStorage_NewOrSave { get; set; }
        public ExternalEvent myExternalEvent_EE13_ExtensibleStorage_NewOrSave { get; set; }

        public EE13_ExtensibleStorage_Rearrange myEE13_ExtensibleStorage_Rearrange { get; set; }
        public ExternalEvent myExternalEvent_EE13_ExtensibleStorage_Rearrange { get; set; }


        public EE14_Draw3D_IntersectorLines_Delete myEE14_Draw3D_IntersectorLines_Delete { get; set; }
        public ExternalEvent myExternalEvent_EE14_Draw3D_IntersectorLines_Delete { get; set; }

        public EE14_Draw3D_IntersectorLines myEE14_Draw3D_IntersectorLines { get; set; }
        public ExternalEvent myExternalEvent_EE14_Draw3D_IntersectorLines { get; set; }

        public EE15_BackupSystem myEE15_BackupSystem { get; set; }
        public ExternalEvent myExternalEvent_EE15_BackupSystem { get; set; }


        public EE16_AddSharedParameters_InVariousWays myEE16_AddSharedParameters_InVariousWays { get; set; }
        public ExternalEvent myExternalEvent_EE16_AddSharedParameters_InVariousWays { get; set; }


        public EE17_Edit_StringBasedParameters myEE17_Edit_StringBasedParameters { get; set; }
        public ExternalEvent myExternalEvent_EE17_Edit_StringBasedParameters { get; set; }


        public EE18_UnderStandingTransforms myEE18_UnderStandingTransforms { get; set; }
        public ExternalEvent myExternalEvent_EE18_UnderStandingTransforms { get; set; }

      

        public Schema schema_FurnLocations { get; set; }
        public Schema schema_FurnLocations_Index { get; set; }

        public int myPublicInt { get; set; } = 0;
        public List<ElementId> myListElementID_SketchPlanesToDelete { get; set; } = new List<ElementId>();
        public int myPublicIntUpDown = -1;
        public List<Transform> myListTransform { get; set; } = new List<Transform>();
        public bool myBoolEventInProgress { get; set; } = false;
        bool myBool_ProceedToZeroEverything = false;
        public bool mySlideInProgress = false;
        bool myBool_Checked = false;
        const double _eps = 1.0e-9;
        public List<Window0506_LoadAndPlaceFamilies.ListView_Class> myListClass { get; set; } = new List<Window0506_LoadAndPlaceFamilies.ListView_Class>();


        public MainWindow(ExternalCommandData cD, ThisApplication tA)
        {
            myThisApplication = tA;
            commandData = cD;
            foreach (string myStrrr in Families_ThatMustBeLoaded.ListStringMustHaveFamilies) myListClass.Add(new Window0506_LoadAndPlaceFamilies.ListView_Class() { String_Name = myStrrr, String_FileName = "//Families//" + myStrrr + ".rfa" });

            InitializeComponent();
            // add 'UIDocument uid' as a parameter above, because this is the way it is called form the external event, please see youve 5 Secrets of Revit API Coding for an explaination on this

            if(true)
            {
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
                //this.Height = Properties.Settings.Default.Height;
                //this.Width = Properties.Settings.Default.Width;
            }

            myEE02_OneOfEachWall = new EE02_OneOfEachWall();
            myEE02_OneOfEachWall.myWindow1 = this;
            myExternalEvent_EE02_OneOfEachWall = ExternalEvent.Create(myEE02_OneOfEachWall);

            myEE03_SetDefaultType = new EE03_SetDefaultType();
            myEE03_SetDefaultType.myWindow1 = this;
            myExternalEvent_EE03_SetDefaultType = ExternalEvent.Create(myEE03_SetDefaultType);

            myEE04_ManualColorOverride = new EE04_ManualColorOverride();
            myEE04_ManualColorOverride.myWindow1 = this;
            myExternalEvent_EE04_ManualColorOverride = ExternalEvent.Create(myEE04_ManualColorOverride);

            myEE05_LoadAllFamilies = new EE05_LoadAllFamilies();
            myEE05_LoadAllFamilies.myWindow1 = this;
            myExternalEvent_EE05_LoadAllFamilies = ExternalEvent.Create(myEE05_LoadAllFamilies);

            myEE06_PlaceAFamily_OnDoubleClick = new EE06_PlaceAFamily_OnDoubleClick();
            myEE06_PlaceAFamily_OnDoubleClick.myWindow1 = this;
            myExternalEvent_EE06_PlaceAFamily_OnDoubleClick = ExternalEvent.Create(myEE06_PlaceAFamily_OnDoubleClick);

            myEE07_RotatingEntities = new EE07_RotatingEntities();
            myEE07_RotatingEntities.myWindow1 = this;
            myExternalEvent_EE07_RotatingEntities = ExternalEvent.Create(myEE07_RotatingEntities);

            myEE08_MoveElementAroundHostingSurface = new EE08_MoveElementAroundHostingSurface();
            myEE08_MoveElementAroundHostingSurface.myWindow1 = this;
            myExternalEvent_EE08_MoveElementAroundHostingSurface = ExternalEvent.Create(myEE08_MoveElementAroundHostingSurface);

            myEE09_Draw3D_ModelLines = new EE09_Draw3D_ModelLines();
            myEE09_Draw3D_ModelLines.myWindow1 = this;
            myExternalEvent_EE09_Draw3D_ModelLines = ExternalEvent.Create(myEE09_Draw3D_ModelLines);

            myEE10_Draw2D_DetailLines = new EE10_Draw2D_DetailLines();
            myEE10_Draw2D_DetailLines.myWindow1 = this;
            myExternalEvent_EE10_Draw2D_DetailLines = ExternalEvent.Create(myEE10_Draw2D_DetailLines);

            myEE11_GridOutCirclesOnFace = new EE11_GridOutCirclesOnFace();
            myEE11_GridOutCirclesOnFace.myWindow1 = this;
            myExternalEvent_EE11_GridOutCirclesOnFace = ExternalEvent.Create(myEE11_GridOutCirclesOnFace);

            myEE13_ExtensibleStorage_NewOrSave = new EE13_ExtensibleStorage_NewOrSave();
            myEE13_ExtensibleStorage_NewOrSave.myWindow1 = this;
            myExternalEvent_EE13_ExtensibleStorage_NewOrSave = ExternalEvent.Create(myEE13_ExtensibleStorage_NewOrSave);

            myEE13_ExtensibleStorage_Rearrange = new EE13_ExtensibleStorage_Rearrange();
            myEE13_ExtensibleStorage_Rearrange.myWindow1 = this;
            myExternalEvent_EE13_ExtensibleStorage_Rearrange = ExternalEvent.Create(myEE13_ExtensibleStorage_Rearrange);

            myEE13_ExtensibleStorage_DeleteItem = new EE13_ExtensibleStorage_DeleteItem();
            myEE13_ExtensibleStorage_DeleteItem.myWindow1 = this;
            myExternalEvent_EE13_ExtensibleStorage_DeleteItem = ExternalEvent.Create(myEE13_ExtensibleStorage_DeleteItem);

            myEE13_ExtensibleStorage_DeleteAll = new EE13_ExtensibleStorage_DeleteAll();
            myEE13_ExtensibleStorage_DeleteAll.myWindow1 = this;
            myExternalEvent_EE13_ExtensibleStorage_DeleteAll = ExternalEvent.Create(myEE13_ExtensibleStorage_DeleteAll);


        myEE13_ExtensibleStorage_zRandomise = new EE13_ExtensibleStorage_zRandomise();
        myEE13_ExtensibleStorage_zRandomise.myWindow1 = this;
            myExternalEvent_EE13_ExtensibleStorage_zRandomise = ExternalEvent.Create(myEE13_ExtensibleStorage_zRandomise);

        myEE12_SetupRoom = new EE12_SetupRoom();
            myEE12_SetupRoom.myWindow1 = this;
            myExternalEvent_EE12_SetupRoom = ExternalEvent.Create(myEE12_SetupRoom);

            myEE14_Draw3D_IntersectorLines_Delete = new EE14_Draw3D_IntersectorLines_Delete();
            myEE14_Draw3D_IntersectorLines_Delete.myWindow1 = this;
            myExternalEvent_EE14_Draw3D_IntersectorLines_Delete = ExternalEvent.Create(myEE14_Draw3D_IntersectorLines_Delete);

            myEE14_Draw3D_IntersectorLines = new EE14_Draw3D_IntersectorLines();
            myEE14_Draw3D_IntersectorLines.myWindow1 = this;
            myExternalEvent_EE14_Draw3D_IntersectorLines = ExternalEvent.Create(myEE14_Draw3D_IntersectorLines);

            myEE15_BackupSystem = new EE15_BackupSystem();
            myEE15_BackupSystem.myWindow1 = this;
            myExternalEvent_EE15_BackupSystem = ExternalEvent.Create(myEE15_BackupSystem);

            myEE16_AddSharedParameters_InVariousWays = new EE16_AddSharedParameters_InVariousWays();
            myEE16_AddSharedParameters_InVariousWays.myWindow1 = this;
            myExternalEvent_EE16_AddSharedParameters_InVariousWays = ExternalEvent.Create(myEE16_AddSharedParameters_InVariousWays);

            myEE17_Edit_StringBasedParameters = new EE17_Edit_StringBasedParameters();
            myEE17_Edit_StringBasedParameters.myWindow1 = this;
            myExternalEvent_EE17_Edit_StringBasedParameters = ExternalEvent.Create(myEE17_Edit_StringBasedParameters);

            myEE18_UnderStandingTransforms = new EE18_UnderStandingTransforms();
            myEE18_UnderStandingTransforms.myWindow1 = this;
            myExternalEvent_EE18_UnderStandingTransforms = ExternalEvent.Create(myEE18_UnderStandingTransforms);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Save();
        }
        public static bool IsEqual(double a, double b)
        {
            return IsZero(b - a);
        }
        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }
        public static bool IsZero(double a)
        {
            return IsZero(a, _eps);
        }


        public Window0506_LoadAndPlaceFamilies myWindow3 { get; set; }
        public Window1213_ExtensibleStorage myWindow4 { get; set; }

        public ThisApplication myThisApplication { get; set; }
        public ExternalCommandData commandData { get; set; }


        private void my01ButtonElementID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;


                ///                  TECHNIQUE 1 OF 19 (MainWindow.xaml.cs)
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓SELECT ELEMENT DIRECTLY BY ID ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     BuiltInCategory.OST_ProjectInformation
                /// 
                /// 
                /// Demonstrates classes:
				///		UIDocument
                ///     Element
                /// 
                /// 
                /// Key methods:
				///		doc.GetElement(new ElementId(
                ///     uidoc.Selection.SetElementIds(
                ///     int.TryParse(
                ///
                ///
                /// * class is actually part of the .NET framework (not Revit API)


                Element myElement_ProjectInformation = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation).WhereElementIsNotElementType().First();
                int myInt = myElement_ProjectInformation.Id.IntegerValue;

                if (uidoc.Selection.GetElementIds().Count != 0) //toggling
                {
                    uidoc.Selection.SetElementIds(new List<ElementId>());
                    return;
                }

                if (myTextbox_ManualID.Text != "")
                {
                    int myInt_FromTextBox = -1;
                    if (int.TryParse(myTextbox_ManualID.Text.ToString(), out myInt_FromTextBox))
                    {
                        Element myElement_TestingElementIsThere = doc.GetElement(new ElementId(myInt_FromTextBox));

                        if (myElement_TestingElementIsThere == null)
                        {
                            MessageBox.Show("Entity of id '" + myInt_FromTextBox + "' not found in model.");
                            return;
                        }
                        else
                        {
                            myInt = myInt_FromTextBox;
                        }
                    }
                    else
                    {
                        myTextbox_ManualID.Text = "";
                    }
                }

                Element myElement = doc.GetElement(new ElementId(myInt));
                uidoc.Selection.SetElementIds(new List<ElementId>() { myElement.Id });

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my01ButtonElementID_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my02OneOfEachWall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE02_OneOfEachWall.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my02OneOfEachWall_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my03SetDefaultType_Click(object sender, RoutedEventArgs e)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                myExternalEvent_EE03_SetDefaultType.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my03SetDefaultType_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my04ManualColorOverride_Click(object sender, RoutedEventArgs e)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                myEE04_ManualColorOverride.myBool_RunFromModeless = true;
                myExternalEvent_EE04_ManualColorOverride.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my04ManualColorOverride_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my0506LoadingFamilies_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;
            myWindow3 = new Window0506_LoadAndPlaceFamilies(commandData);

            try
            {
                myWindow3.myWindow1 = this;
                myWindow3.Topmost = true;
                myWindow3.Owner = this;
                myWindow3.Show();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my0506LoadingFamilies_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my07RotateWall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE07_RotatingEntities.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my07RotateWall_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my08MoveFamilyInstances_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE08_MoveElementAroundHostingSurface.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my08MoveFamilyInstances_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my09Draw3DLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myExternalEvent_EE09_Draw3D_ModelLines.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my09Draw3DLines_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my10Draw2DLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myExternalEvent_EE10_Draw2D_DetailLines.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my10Draw2DLines_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my11ClearCirclesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE11_GridOutCirclesOnFace.myBool_DoLoop = false;
                myEE11_GridOutCirclesOnFace.myBool_JustClear = true;
                myExternalEvent_EE11_GridOutCirclesOnFace.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my11ClearCirclesButton_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my11CheckBox_OneTwoOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (myBool_Checked != my11CheckBox_OneTwoOne.IsChecked.Value)
                {
                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    SetForegroundWindow(uidoc.Application.MainWindowHandle);
                    keybd_event(0x1B, 0, 0, 0);
                    keybd_event(0x1B, 0, 2, 0);

                    myEE11_GridOutCirclesOnFace.myBool_DoLoop = false;
                    myEE11_GridOutCirclesOnFace.myBool_JustClear = false;
                    myExternalEvent_EE11_GridOutCirclesOnFace.Raise();
                }

                myBool_Checked = my11CheckBox_OneTwoOne.IsChecked.Value;
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my11CheckBox_OneTwoOne_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my11IntegerUpDown_Rows_Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE11_GridOutCirclesOnFace.myBool_DoLoop = false;
                myEE11_GridOutCirclesOnFace.myBool_JustClear = false;
                myExternalEvent_EE11_GridOutCirclesOnFace.Raise();

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my11IntegerUpDown_Rows_Spinned" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my11IntegerUpDown_Columns_Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                // myCancelledFromSpinner = true;

                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE11_GridOutCirclesOnFace.myBool_DoLoop = false;
                myEE11_GridOutCirclesOnFace.myBool_JustClear = false;
                myExternalEvent_EE11_GridOutCirclesOnFace.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my11IntegerUpDown_Columns_Spinned" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my11GridCircles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE11_GridOutCirclesOnFace.myBool_DoLoop = true;
                myEE11_GridOutCirclesOnFace.myBool_JustClear = false;
                myExternalEvent_EE11_GridOutCirclesOnFace.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my11GridCircles_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my1213ExtensibleStorage_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;
            myWindow4 = new Window1213_ExtensibleStorage(commandData);

            try
            {
                myWindow4.myWindow1 = this;
                myWindow4.Topmost = true;
                myWindow4.Owner = this;
                myWindow4.Show();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my1213ExtensibleStorage_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my14ReferenceIntersector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                List<Element> myListElement = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Where(x => x.Name == "Nerf Gun").ToList();

                if (myListElement.Count() == 0)
                {
                    MessageBox.Show("Please place a nerf gun in the model, (previous button)");
                    return;
                }

                uidoc.Selection.SetElementIds(new List<ElementId>() { myListElement.Last().Id });

                myExternalEvent_EE14_Draw3D_IntersectorLines.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my14ReferenceIntersector_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my14ClearIntersectorLinesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE14_Draw3D_IntersectorLines_Delete.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my14ClearIntersectorLinesButton_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my15BackupSystem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                if (doc.PathName == "")
                {
                    MessageBox.Show("Please save project file.");
                    return;
                }

                myExternalEvent_EE15_BackupSystem.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my15BackupSystem_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my15OpenFolderButton_Click(object sender, RoutedEventArgs e)
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
                    return ;
                }



                if (true)//candidate for methodisation 202004251537
                {

                    if (uidoc.Selection.GetElementIds().Count != 1)
                    {
                        MessageBox.Show("Please select just one element instance.");
                        return;
                    }

                    if (doc.GetElement(uidoc.Selection.GetElementIds().First()).GetType() != typeof(FamilyInstance))
                    {
                        MessageBox.Show("Please select an element of type - 'FamilyInstance'.");
                        return;
                    }

                    myFamilyInstance = doc.GetElement(uidoc.Selection.GetElementIds().First()) as FamilyInstance;

                    myFamily = ((FamilySymbol)doc.GetElement(myFamilyInstance.GetTypeId())).Family;

                    if (!myFamily.IsEditable)
                    {
                        MessageBox.Show("Family is not editable.");
                        return;
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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my15OpenFolderButton_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my1617AddingEditingParameters_Click(object sender, RoutedEventArgs e)
        {
            Window1617_AddEditParameters myWindowWindow1617 = new Window1617_AddEditParameters(commandData);
            try
            {
                myWindowWindow1617.myWindow1 = this;
                myWindowWindow1617.Topmost = true;
                myWindowWindow1617.Owner = this;
                myWindowWindow1617.Show();

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my1617AddingEditingParameters_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void my1819UnderstandingTransforms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE18_UnderStandingTransforms.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("my1819UnderstandingTransforms_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }



        private void myMethod_ShowCodeButtons(string myString_Filename)
        {
            try
            {
                if (myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01")
                {
                    string myString_TempPath = myThisApplication.messageConst.Split('|')[1] + @"\Code Snippets\" + myString_Filename;

                    FileInfo myFileInfo_Start = new FileInfo(myString_TempPath);
                    string destDir = System.IO.Path.GetTempPath();

                    FileInfo myFileInfo_End = new FileInfo(Path.Combine(destDir, myString_Filename));

                    myFileInfo_Start.CopyTo(myFileInfo_End.FullName, true);
                    System.Diagnostics.Process.Start(myFileInfo_End.FullName);

                }
                if (myThisApplication.messageConst.Split('|')[0] == "Button_01_Invoke01Development")
                {
                    string myString_TempPath = myThisApplication.messageConst.Split('|')[1] + @"\_929_Bilt2020_PlaypenChild\Code Snippets\" + myString_Filename;
                    System.Diagnostics.Process.Start(myString_TempPath);
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myMethod_ShowCodeButtons" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion   
        }


        private void myButton_EE00_AlphabeticalListOfClasses_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE00_AlphabeticalListOfClasses.txt");
        }
        private void myButton_EE01_SelectElementWithCode_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE01_SelectElementWithCode.txt");
        }

        private void myButton_EE02_OneOfEachWall_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE02_OneOfEachWall.txt");
        }

        private void myButton_EE03_SetDefaultType_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE03_SetDefaultType.txt");
        }

        private void myButton_EE04_ManualColorOverride_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE04_ManualColorOverride.txt");
        }

        private void myButton_EE05_LoadAllFamilies_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE05_LoadAllFamilies.txt");
        }

        private void myButton_EE06_PlaceAFamily_OnDoubleClick_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE06_PlaceAFamily_OnDoubleClick.txt");
        }

        private void myButton_EE07_RotatingEntities_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE07_RotatingEntities.txt");
        }

        private void myButton_EE08_MoveElementAroundHostingSurface_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE08_MoveElementAroundHostingSurface.txt");
        }

        private void myButton_EE09_Draw3D_ModelLines_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE09_Draw3D_ModelLines.txt");
        }

        private void myButton_EE10_Draw2D_DetailLines_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE10_Draw2D_DetailLines.txt");
        }

        private void myButton_EE11_GridOutCirclesOnFace_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE11_GridOutCirclesOnFace.txt");
        }

        private void myButton_EE12_SetupRoom_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE12_SetupRoom.txt");
        }

        private void myButton_EE13_ExtensibleStorage_NewOrSave_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE13_ExtensibleStorage_NewOrSave.txt");
        }

        private void myButton_EE14_Draw3D_IntersectorLines_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE14_Draw3D_IntersectorLines.txt");
        }

        private void myButton_EE15_BackupSystem_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE15_BackupSystem.txt");
        }

        private void myButton_EE16_AddSharedParameters_InVariousWays_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE16_AddSharedParameters_InVariousWays.txt");
        }

        private void myButton_EE17_Edit_StringBasedParameters_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE17_Edit_StringBasedParameters.txt");
        }

        private void myButton_EE18_UnderStandingTransforms01_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE18_other_project_EE03_RotateAroundBasis.txt");
        }

        private void myButton_EE19_UnderStandingTransforms02_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("EE19_other_project_Window1.xaml.cs_InterpolationMethod.txt");
        }

    }
}