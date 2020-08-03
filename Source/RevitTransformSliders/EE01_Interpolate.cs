using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Numerics = System.Numerics;
using std = System.Math;
using _952_PRLoogleClassLibrary;
using System.Text.RegularExpressions;


namespace RevitTransformSliders
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE01_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("EE01_Part1_Template");

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_Template" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        public string GetName()
        {
            return "EE01_Part1_Template";
        }
    }


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE01_Interpolate : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }
        public bool myBool_RunOnce { get; set; }
        public bool myBool_Cycle { get; set; }
        public static void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                FamilyInstance myFamilyInstance_Departure = doc.GetElement(new ElementId(myWindow1.myIntUpDown_Middle2.Value.Value)) as FamilyInstance;
                FamilySymbol myFamilySymbol = doc.GetElement(myFamilyInstance_Departure.GetTypeId()) as FamilySymbol;

                IList<ElementId> placePointIds_1338 = AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_Departure);
                ReferencePoint myReferencePoint_Departure = doc.GetElement(placePointIds_1338.First()) as ReferencePoint;

                Transform myTransform = myReferencePoint_Departure.GetCoordinateSystem();

                int myIntTimeOut = 0;
                int myInt_ChangeCount = 0;
                double myDouble_ChangePosition = -1;


                ///                  TECHNIQUE 6 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ SET DEFAULT TYPE ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///Demonstrates: 
                ///SketchPlane
                ///PromptForFamilyInstancePlacement with the PromptForFamilyInstancePlacementOptions class
                ///DocumentChanged event that cancels the command and focuses the window


                using (TransactionGroup transGroup = new TransactionGroup(doc))
                {
                    transGroup.Start("Transform animation");

                    if (myBool_Cycle)
                    {
                        for (int i = 1; i <= myWindow1.myUpDown_CycleNumber.Value; i++)
                        {
                            for (int ii = 0; ii <= 24; ii++)
                            {
                                wait(100);
                                if (true) //candidate for methodisation 202006141254
                                {
                                    if (myDouble_ChangePosition != ii)
                                    {
                                        myDouble_ChangePosition = ii;
                                        myWindow1.myLabel_ChangeCount.Content = myInt_ChangeCount++.ToString();
                                        using (Transaction y = new Transaction(doc, "a Transform"))
                                        {
                                            y.Start();

                                            myReferencePoint_Departure.SetCoordinateSystem(myWindow1.myListTransform_Interpolate[(int)myDouble_ChangePosition]);

                                            y.Commit();
                                        }

                                        myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisZ, false);
                                        myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisX, false);
                                        myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisY, true);
                                    }
                                }
                                myWindow1.myLabel_Setting.Content = ii.ToString();
                            }
                        }
                    }

                    if (!myBool_Cycle)
                    {
                        while (myWindow1.mySlideInProgress | myBool_RunOnce == true)
                        {
                            myBool_RunOnce = false;
                            wait(100); myIntTimeOut++;

                            if (true) //candidate for methodisation 202006141254
                            {
                                if (myDouble_ChangePosition != myWindow1.mySlider_Interpolate.Value)
                                {
                                    myDouble_ChangePosition = myWindow1.mySlider_Interpolate.Value;
                                    myWindow1.myLabel_ChangeCount.Content = myInt_ChangeCount++.ToString();
                                    using (Transaction y = new Transaction(doc, "a Transform"))
                                    {
                                        y.Start();

                                        myReferencePoint_Departure.SetCoordinateSystem(myWindow1.myListTransform_Interpolate[(int)myDouble_ChangePosition]);

                                        y.Commit();
                                    }

                                    myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisZ, false);
                                    myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisX, false);
                                    myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisY, true);
                                }
                            }

                            myWindow1.myLabel_Setting.Content = myWindow1.mySlider_Interpolate.Value.ToString();

                            if (myIntTimeOut == 400)
                            {
                                MessageBox.Show("Timeout");
                                break;
                            }
                        }
                    }

                    transGroup.Assimilate();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_Interpolate" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        public string GetName()
        {
            return "External Event Example";
        }
    }



}
