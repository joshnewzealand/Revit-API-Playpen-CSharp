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
    public partial class Window1 : Window
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public ExternalCommandData commandData { get; set; }

        public EE01_Part1 myEE01_Part1 { get; set; }
        public ExternalEvent myExternalEvent_EE01 { get; set; }

        public EE01_Part1_ManualColorOverride myEE01_Part1_ManualColorOverride { get; set; }
        public ExternalEvent myExternalEvent_EE01_Part1_ManualColorOverride { get; set; }

        //
        //
        //  transformable rotate, to let 
        //

        public EE01_Part1_PlaceAFamily myEE01_Part1_PlaceAFamily { get; set; }
        public ExternalEvent myExternalEvent_EE01_Part1_PlaceAFamily { get; set; }

        public EE01_Part1_GridOutCirclesOnFace myEE01_Part1_GridOutCirclesOnFace { get; set; }
        public ExternalEvent myExternalEvent_EE01_GridOutCirclesOnFace { get; set; }

        public EE02_Part1 myEE02_Part1 { get; set; }
        public ExternalEvent myExternalEvent_EE02 { get; set; }

        public EE02_Part1_SetDefault myEE02_Part1_SetDefault { get; set; } 
        public ExternalEvent myExternalEvent_EE02_Part1_SetDefault { get; set; }
        public EE03_Part1_NewOrSave myEE03_Part1_NewOrSave { get; set; }
        public ExternalEvent myExternalEvent_EE03_Part1_NewOrSave { get; set; }
        public EE03_Part2_ShowArrangement myEE03_Part2_ShowArrangement { get; set; }
        public ExternalEvent myExternalEvent_EE03_Part2_ShowArrangement { get; set; }
        
        public EE03_Part2_DeleteOne myEE03_Part2_DeleteOne { get; set; }
        public ExternalEvent myExternalEvent_EE03_Part2_DeleteOne { get; set; }
        
        public EE03_Part3_ClearValues myEE03_Part3_ClearValues { get; set; }
        public ExternalEvent myExternalEvent_EE03_Part3_ClearValues { get; set; }

        public EE04_Part1 myEE04_Part1 { get; set; }
        public ExternalEvent myExternalEvent_EE04 { get; set; }

        public EE04_Part1_MoveElementOnWall myEE04_Part1_MoveElementOnWall { get; set; }
        public ExternalEvent myExternalEvent_EE04_Part1_MoveElementOnWall { get; set; }

        public EE04_UnderStandingTransforms myEE04_UnderStandingTransforms { get; set; }
        public ExternalEvent myExternalEvent_EE04_UnderStandingTransforms { get; set; }

        

        public EE04_Part2_2DShapes myEE04_Part2_2DShapes { get; set; }
        public ExternalEvent myExternalEvent_EE04_Part2_2DShapes { get; set; }


        public EE05_Part1 myEE05_Part1 { get; set; }
        public ExternalEvent myExternalEvent_EE05 { get; set; }

        public EE05_Part1_LoadAllFamilies myEE05_Part1_LoadAllFamilies { get; set; }
        public ExternalEvent myExternalEvent_EE05_Part1_LoadAllFamilies { get; set; }

        public EE05_Part1_DeleteSketchPlanes myEE05_Part1_DeleteSketchPlanes { get; set; }
        public ExternalEvent myExternalEvent_EE05_Part1_DeleteSketchPlanes { get; set; }


        public EE06_Part1_FamilyManager myEE06_Part1_FamilyManager { get; set; }
        public ExternalEvent myExternalEvent_EE06_FamilyManager { get; set; }

        public EE07_Part1 myEE07_Part1 { get; set; }
        public ExternalEvent myExternalEvent_EE07 { get; set; }

        public EE07_Part1_Binding myEE07_Part1_Binding { get; set; }
        public ExternalEvent myExternalEvent_EE07_Binding { get; set; }

        public EE07_SetupRoom myEE07_SetupRoom { get; set; }
        public ExternalEvent myExternalEvent_EE07_SetupRoom { get; set; }


        public Schema schema_FurnLocations { get; set; }
        public Schema schema_FurnLocations_Index { get; set; }

        public ThisApplication myThisApplication { get; set; }
        public Window3 myWindow3 { get; set; }
        public Window4 myWindow4 { get; set; }


        //MessageBox.Show(myString_FamilyName + " Family has not been loaded into project.");

        public Window1(ExternalCommandData cD, ThisApplication tA)
        {
            myThisApplication = tA;
            commandData = cD;

            InitializeComponent();
            // add 'UIDocument uid' as a parameter above, because this is the way it is called form the external event, please see youve 5 Secrets of Revit API Coding for an explaination on this

            if(true)
            {
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
                this.Height = Properties.Settings.Default.Height;
                this.Width = Properties.Settings.Default.Width;
            }

            myEE01_Part1 = new EE01_Part1();
            myEE01_Part1.myWindow1 = this;
            myExternalEvent_EE01 = ExternalEvent.Create(myEE01_Part1);

            myEE01_Part1_ManualColorOverride = new EE01_Part1_ManualColorOverride();
            myEE01_Part1_ManualColorOverride.myWindow1 = this;
            myExternalEvent_EE01_Part1_ManualColorOverride = ExternalEvent.Create(myEE01_Part1_ManualColorOverride);

            myEE01_Part1_GridOutCirclesOnFace = new EE01_Part1_GridOutCirclesOnFace();
            myEE01_Part1_GridOutCirclesOnFace.myWindow1 = this;
            myExternalEvent_EE01_GridOutCirclesOnFace = ExternalEvent.Create(myEE01_Part1_GridOutCirclesOnFace);

            myEE02_Part1 = new EE02_Part1();
            myEE02_Part1.myWindow1 = this;
            myExternalEvent_EE02 = ExternalEvent.Create(myEE02_Part1);

            myEE02_Part1_SetDefault = new EE02_Part1_SetDefault();
            myEE02_Part1_SetDefault.myWindow1 = this;
            myExternalEvent_EE02_Part1_SetDefault = ExternalEvent.Create(myEE02_Part1_SetDefault);


            myEE03_Part1_NewOrSave = new EE03_Part1_NewOrSave();
            myEE03_Part1_NewOrSave.myWindow1 = this;
            myExternalEvent_EE03_Part1_NewOrSave = ExternalEvent.Create(myEE03_Part1_NewOrSave);

            myEE03_Part2_ShowArrangement = new EE03_Part2_ShowArrangement();
            myEE03_Part2_ShowArrangement.myWindow1 = this;
            myExternalEvent_EE03_Part2_ShowArrangement = ExternalEvent.Create(myEE03_Part2_ShowArrangement);


            myEE03_Part2_DeleteOne = new EE03_Part2_DeleteOne();
            myEE03_Part2_DeleteOne.myWindow1 = this;
            myExternalEvent_EE03_Part2_DeleteOne = ExternalEvent.Create(myEE03_Part2_DeleteOne);

            myEE03_Part3_ClearValues = new EE03_Part3_ClearValues();
            myEE03_Part3_ClearValues.myWindow1 = this;
            myExternalEvent_EE03_Part3_ClearValues = ExternalEvent.Create(myEE03_Part3_ClearValues);

            myEE04_Part1 = new EE04_Part1();
            myEE04_Part1.myWindow1 = this;
            myExternalEvent_EE04 = ExternalEvent.Create(myEE04_Part1);


            myEE04_Part1_MoveElementOnWall = new EE04_Part1_MoveElementOnWall();
            myEE04_Part1_MoveElementOnWall.myWindow1 = this;
            myExternalEvent_EE04_Part1_MoveElementOnWall = ExternalEvent.Create(myEE04_Part1_MoveElementOnWall);
        ////    /        public EE04_Part1_MoveElementOnWall myEE04_Part1_MoveElementOnWall { get; set; }
        ////public ExternalEvent myExternalEvent_EE04_Part1_MoveElementOnWall { get; set; }


            myEE04_UnderStandingTransforms = new EE04_UnderStandingTransforms();
            myEE04_UnderStandingTransforms.myWindow1 = this;
            myExternalEvent_EE04_UnderStandingTransforms = ExternalEvent.Create(myEE04_UnderStandingTransforms);


            myEE04_Part2_2DShapes = new EE04_Part2_2DShapes();
            myEE04_Part2_2DShapes.myWindow1 = this;
            myExternalEvent_EE04_Part2_2DShapes = ExternalEvent.Create(myEE04_Part2_2DShapes);
            //

            myEE05_Part1 = new EE05_Part1();
            myEE05_Part1.myWindow1 = this;
            myExternalEvent_EE05 = ExternalEvent.Create(myEE05_Part1);

            myEE05_Part1_LoadAllFamilies = new EE05_Part1_LoadAllFamilies();
            myEE05_Part1_LoadAllFamilies.myWindow1 = this;
            myExternalEvent_EE05_Part1_LoadAllFamilies = ExternalEvent.Create(myEE05_Part1_LoadAllFamilies);

            myEE05_Part1_DeleteSketchPlanes = new EE05_Part1_DeleteSketchPlanes();
            myEE05_Part1_DeleteSketchPlanes.myWindow1 = this;
            myExternalEvent_EE05_Part1_DeleteSketchPlanes = ExternalEvent.Create(myEE05_Part1_DeleteSketchPlanes);


            myEE01_Part1_PlaceAFamily = new EE01_Part1_PlaceAFamily();
            myEE01_Part1_PlaceAFamily.myWindow1 = this;
            myExternalEvent_EE01_Part1_PlaceAFamily = ExternalEvent.Create(myEE01_Part1_PlaceAFamily);

            myEE06_Part1_FamilyManager = new EE06_Part1_FamilyManager();
            myEE06_Part1_FamilyManager.myWindow1 = this;
            myExternalEvent_EE06_FamilyManager = ExternalEvent.Create(myEE06_Part1_FamilyManager);

            myEE07_Part1 = new EE07_Part1();
            myEE07_Part1.myWindow1 = this;
            myExternalEvent_EE07 = ExternalEvent.Create(myEE07_Part1);

            myEE07_Part1_Binding = new EE07_Part1_Binding();
            myEE07_Part1_Binding.myWindow1 = this;
            myExternalEvent_EE07_Binding = ExternalEvent.Create(myEE07_Part1_Binding);

            myEE07_SetupRoom = new EE07_SetupRoom();
            myEE07_SetupRoom.myWindow1 = this;
            myExternalEvent_EE07_SetupRoom = ExternalEvent.Create(myEE07_SetupRoom);

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            //myMakeTheSchemas(doc);
        }

        public int myPublicInt { get; set; } = 0;

        public List<ElementId> myListElementID_SketchPlanesToDelete { get; set; } = new List<ElementId>();

        private void MyButtonRotateWall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE04.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonRotateWall_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        public int myPublicIntUpDown = -1;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            int eL = -1;

            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //uidoc.Selection.SetElementIds(new List<ElementId>() { new ElementId(262427) });
                //528229

                /////////////////////if (!myHostId_To_Selection()) { acquring(true); } else { return; };

                //myFormerWindow3();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Window_Loaded, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                ///_952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Window_Loaded" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Save();
        }
        private void myButton_Draw3DLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myExternalEvent_EE01.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonWeCan_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void MyButtonFilteredElementCollector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE02.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonFilteredElementCollector_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void myButtonExtensible_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;
            myWindow4 = new Window4(commandData);

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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonExtensible_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion

        }
        private void MyButtonElementID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                Element myElement_ProjectInformation = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_ProjectInformation).WhereElementIsNotElementType().First();
                int myInt = myElement_ProjectInformation.Id.IntegerValue;

                if(uidoc.Selection.GetElementIds().Count != 0) //toggling
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
                            //myTextbox_ManualID.Text = "";
                            return;
                        }
                        else
                        {
                            myInt = myInt_FromTextBox;
                        }
                    } else
                    {
                        myTextbox_ManualID.Text = "";
                    }
                }

                ///the above lines are important for this example, but is not the 'technique'.
                ///

                ///                  TECHNIQUE 1 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓SELECT ELEMENT DIRECTLY BY ID ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

                Element myElement = doc.GetElement(new ElementId(myInt));
                uidoc.Selection.SetElementIds(new List<ElementId>() { myElement.Id });

                ///↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonElementID_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void myReferenceIntersector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                List<Element> myListElement = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Where(x => x.Name == "Nerf Gun").ToList();

                if(myListElement.Count() == 0)
                {
                    MessageBox.Show("Please place a nerf gun in the model, (previous button)");
                    return;
                }

                uidoc.Selection.SetElementIds(new List<ElementId>() { myListElement.Last().Id });

                myExternalEvent_EE05.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myReferenceIntersector_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void myButtonPickPlace_Click(object sender, RoutedEventArgs e)
        {
            int eL = -1;
            myWindow3 = new Window3(commandData);

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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonPickPlace_Click, error line:" + eL + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void MyButtonFamilyManager_Click(object sender, RoutedEventArgs e)
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

                myExternalEvent_EE06_FamilyManager.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyFamilyManager_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void MyButtonSetDefaultWall_Click(object sender, RoutedEventArgs e)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                myExternalEvent_EE02_Part1_SetDefault.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonSetDefaultWall_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButtonLookAtThisTool_Click(object sender, RoutedEventArgs e)
        {
            Window2 myWindow2 = new Window2(commandData);
            try
            {
                myWindow2.myWindow1 = this;
                myWindow2.Topmost = true;
                myWindow2.Owner = this;
                myWindow2.Show();

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonOpenParameterEdit_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                myExternalEvent_EE01_Part1_ManualColorOverride.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("Button_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void myButtonOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                FamilyInstance myFamilyInstance; //candidate for methodisation 202004251537

                string myString_FamilyBackups_FamilyName = "";
                Family myFamily = null;
                string myString_FamilyName = "";

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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonOpenFolder_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void myButtonPlonk2DLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myExternalEvent_EE04_Part2_2DShapes.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonPlonk2DLines_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        public List<Transform> myListTransform { get; set; } = new List<Transform>();

        public bool myBoolEventInProgress { get; set; } = false;
        bool myBool_ProceedToZeroEverything = false;
        public bool mySlideInProgress = false;





        public void myFormerWindow3()
        {

            myListTransform.Clear();

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FamilyInstance myFamilyInstance_1533 = doc.GetElement(new ElementId(575579)) as FamilyInstance;  //this is platform A, this code will probably be removed later
            ReferencePoint myReferencePoint2 = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_1533).First()) as ReferencePoint;
            Transform myTransform_FakeBasis = myReferencePoint2.GetCoordinateSystem();

            FamilyInstance myFamilyInstance_2252 = doc.GetElement(new ElementId(577125)) as FamilyInstance;  //this is platform A, this code will probably be removed later
            ReferencePoint myReferencePoint3 = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_2252).First()) as ReferencePoint;
            Transform myTransform_FakeBasis2 = myReferencePoint3.GetCoordinateSystem();

            double myDouble_OneDivideTwelve = 1.0 / 24.0;

            List<float> myListfloat = new List<float>();

            myListfloat.Add((float)(myDouble_OneDivideTwelve * 1)); //13
            for (int i = 1; i <= 23; i++)
            {
                myListfloat.Add((float)(myDouble_OneDivideTwelve * i)); //12
            }
            myListfloat.Add((float)(myDouble_OneDivideTwelve * 23)); //13

            foreach (float myFloat in myListfloat)
            {
                FamilyInstance myFamilyInstance_1533333 = doc.GetElement(new ElementId(575579)) as FamilyInstance;
                Transform myTransform_FakeBasis44444 = myFamilyInstance_1533333.GetTransform();

                FamilyInstance myFamilyInstance_1544444 = doc.GetElement(new ElementId(577125)) as FamilyInstance;
                Transform myTransform_FakeBasis33333 = myFamilyInstance_1544444.GetTransform();

                myTransform_FakeBasis44444.Origin = XYZ.Zero;
                myTransform_FakeBasis33333.Origin = XYZ.Zero;

                Transform myTransformFromQuatX;
                double aaMega_QuatX = XYZ.BasisZ.AngleTo(myTransform_FakeBasis33333.BasisZ);  //remember this is about vector not basis

                if (IsZero(aaMega_QuatX)) myTransformFromQuatX = Transform.Identity;
                else
                {
                    XYZ axis = myTransform_FakeBasis33333.BasisZ.CrossProduct(-XYZ.BasisZ);
                    myTransformFromQuatX = Transform.CreateRotationAtPoint(axis, aaMega_QuatX, XYZ.Zero);
                }

                Transform myTransformFromQuatXX;
                double aaMega_QuatXX = XYZ.BasisZ.AngleTo(myTransform_FakeBasis44444.BasisZ);  //remember this is about vector not basis

                if (IsZero(aaMega_QuatXX)) myTransformFromQuatXX = Transform.Identity;
                else
                {
                    XYZ axis = myTransform_FakeBasis44444.BasisZ.CrossProduct(-XYZ.BasisZ);
                    myTransformFromQuatXX = Transform.CreateRotationAtPoint(axis, aaMega_QuatXX, XYZ.Zero);
                }

                Transform myEndPointTransform = Transform.Identity;
                myEndPointTransform.BasisX = myTransformFromQuatXX.Inverse.OfVector(myTransform_FakeBasis44444.BasisX);
                myEndPointTransform.BasisY = myTransformFromQuatXX.Inverse.OfVector(myTransform_FakeBasis44444.BasisY);
                myEndPointTransform.BasisZ = myTransformFromQuatXX.Inverse.OfVector(myTransform_FakeBasis44444.BasisZ);

                Transform myEndPointTransformFor333 = Transform.Identity;
                myEndPointTransformFor333.BasisX = myTransformFromQuatX.Inverse.OfVector(myTransform_FakeBasis33333.BasisX);
                myEndPointTransformFor333.BasisY = myTransformFromQuatX.Inverse.OfVector(myTransform_FakeBasis33333.BasisY);
                myEndPointTransformFor333.BasisZ = myTransformFromQuatX.Inverse.OfVector(myTransform_FakeBasis33333.BasisZ);

                double angle = myEndPointTransform.BasisX.AngleOnPlaneTo(XYZ.BasisX, -XYZ.BasisZ);
                double d1_Final = myEndPointTransformFor333.BasisX.AngleOnPlaneTo(XYZ.BasisX, -XYZ.BasisZ);

                if (d1_Final > Math.PI) d1_Final = -((Math.PI * 2) - d1_Final);
                if (angle > Math.PI) angle = -((Math.PI * 2) - angle);

                Numerics.Vector3 myQuat = new Numerics.Vector3();
                myQuat.X = (float)myTransform_FakeBasis33333.BasisZ.X;
                myQuat.Y = (float)myTransform_FakeBasis33333.BasisZ.Y;
                myQuat.Z = (float)myTransform_FakeBasis33333.BasisZ.Z;

                Numerics.Vector3 myQuat2 = new Numerics.Vector3();
                myQuat2.X = (float)myTransform_FakeBasis44444.BasisZ.X;
                myQuat2.Y = (float)myTransform_FakeBasis44444.BasisZ.Y;
                myQuat2.Z = (float)myTransform_FakeBasis44444.BasisZ.Z;

                Numerics.Vector3 myQuat3 = Numerics.Vector3.Lerp(myQuat2, myQuat, myFloat);

                Transform myTransformFromQuat_Final = Transform.Identity;
                if (true)
                {
                    double aaMega_Quat = XYZ.BasisZ.AngleTo(new XYZ(myQuat3.X, myQuat3.Y, myQuat3.Z));  //remember this is about vector not basis

                    if (IsZero(aaMega_Quat)) myTransformFromQuat_Final = Transform.Identity;
                    else
                    {
                        XYZ axis = new XYZ(myQuat3.X, myQuat3.Y, myQuat3.Z).CrossProduct(-XYZ.BasisZ);
                        myTransformFromQuat_Final = Transform.CreateRotationAtPoint(axis, aaMega_Quat, XYZ.Zero);
                    }
                }

                double myDoubleAlmostThere = (angle * (1 - myFloat)) + (d1_Final * (myFloat));

                Transform myEndPointTransform_JustOnceSide = Transform.CreateRotationAtPoint(myTransformFromQuat_Final.BasisZ, myDoubleAlmostThere, XYZ.Zero);

                Transform myTransformFromQuat_AllNewFinal = Transform.Identity;
                myTransformFromQuat_AllNewFinal.BasisX = myEndPointTransform_JustOnceSide.OfVector(myTransformFromQuat_Final.BasisX);
                myTransformFromQuat_AllNewFinal.BasisY = myEndPointTransform_JustOnceSide.OfVector(myTransformFromQuat_Final.BasisY);
                myTransformFromQuat_AllNewFinal.BasisZ = myEndPointTransform_JustOnceSide.OfVector(myTransformFromQuat_Final.BasisZ);

                Transform t = myTransformFromQuat_AllNewFinal;

                t.Origin = myReferencePoint3.Position + ((myReferencePoint2.Position - myReferencePoint3.Position) * (1 - myFloat));

                myListTransform.Add(t);
            }
        }


        public static bool IsEqual(double a, double b)
        {
            return IsZero(b - a);
        }

        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }
        const double _eps = 1.0e-9;

        public static bool IsZero(double a)
        {
            return IsZero(a, _eps);
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
                    string myString_TempPath = myThisApplication.messageConst.Split('|')[1] + @"\_929_Bilt2020_PlaypenChild\Code Snippets\01 of 19 Select element with code.txt";
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

        private void myButtonShowCode_SelectElementWithCode_Click(object sender, RoutedEventArgs e)
        {
            myMethod_ShowCodeButtons("01 of 19 Select element with code.txt");
        }

        private void myButton_UnderStandingTransforms_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE04_UnderStandingTransforms.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_UnderStandingTransforms_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void MyButtonGridCircles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE01_Part1_GridOutCirclesOnFace.myBool_DoLoop = true;
                myEE01_Part1_GridOutCirclesOnFace.myBool_JustClear = false;
                myExternalEvent_EE01_GridOutCirclesOnFace.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButtonGridCircles_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myIntegerUpDown_Rows_Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE01_Part1_GridOutCirclesOnFace.myBool_DoLoop = false;
                myEE01_Part1_GridOutCirclesOnFace.myBool_JustClear = false;
                myExternalEvent_EE01_GridOutCirclesOnFace.Raise();

                //there are differences here and there, here and there, here and there
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myIntegerUpDown_Rows_Spinned" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

     //  public bool myCancelledFromSpinner { get; set; } = false;

        private void myIntegerUpDown_Columns_Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

               // myCancelledFromSpinner = true;

                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE01_Part1_GridOutCirclesOnFace.myBool_DoLoop = false;
                myEE01_Part1_GridOutCirclesOnFace.myBool_JustClear = false;
                myExternalEvent_EE01_GridOutCirclesOnFace.Raise();

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myIntegerUpDown_Columns_Spinned" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void myClearCircles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                //clearing existing command
                SetForegroundWindow(uidoc.Application.MainWindowHandle);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

                myEE01_Part1_GridOutCirclesOnFace.myBool_DoLoop = false;
                myEE01_Part1_GridOutCirclesOnFace.myBool_JustClear = true;
                myExternalEvent_EE01_GridOutCirclesOnFace.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myClearCircles_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        bool myBool_Checked = false;
        private void myCheckBox_OneTwoOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (myBool_Checked != myCheckBox_OneTwoOne.IsChecked.Value)
                {
                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    // myCancelledFromSpinner = true;

                    SetForegroundWindow(uidoc.Application.MainWindowHandle);
                    keybd_event(0x1B, 0, 0, 0);
                    keybd_event(0x1B, 0, 2, 0);

                    myEE01_Part1_GridOutCirclesOnFace.myBool_DoLoop = false;
                    myEE01_Part1_GridOutCirclesOnFace.myBool_JustClear = false;
                    myExternalEvent_EE01_GridOutCirclesOnFace.Raise();
                }

                myBool_Checked = myCheckBox_OneTwoOne.IsChecked.Value;
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myCheckBox_OneTwoOne_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion

        }

        private void myButton_ClearIntersectorLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myExternalEvent_EE05_Part1_DeleteSketchPlanes.Raise();
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_ClearIntersectorLines_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
      
        }

        private void myButtonMoveElementsOnAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                myExternalEvent_EE04_Part1_MoveElementOnWall.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButtonMoveElementsOnAll_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
    }
}