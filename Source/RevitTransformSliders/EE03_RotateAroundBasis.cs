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
    public class EE03_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
    public class EE03_RotateAroundBasis : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {

        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }
        const double _eps = 1.0e-9;
        public static bool IsZero(double a)
        {
            return IsZero(a, _eps);
        }

        public Window1 myWindow1 { get; set; }
        public bool myBool_InterpolateMiddle_WhenEitherA_or_B { get; set; }

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

                FamilyInstance myFamilyInstance_Middle2 = doc.GetElement(new ElementId(myWindow1.myIntUpDown_Middle2.Value.Value)) as FamilyInstance;
                IList<ElementId> placePointIds_1339 = AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_Middle2);
                ReferencePoint myReferencePoint_Middle = doc.GetElement(placePointIds_1339.First()) as ReferencePoint;


                FamilyInstance myFamilyInstance_Departure = doc.GetElement(new ElementId(myWindow1.myToolKit_IntUpDown.Value.Value)) as FamilyInstance;
                IList<ElementId> placePointIds_1338 = AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_Departure);
                ReferencePoint myReferencePoint_Centre = doc.GetElement(placePointIds_1338.First()) as ReferencePoint;

                Transform myTransform = myReferencePoint_Centre.GetCoordinateSystem();

                Transform myTransform_ToMakeTheRotationRelative;
                if (true)
                {
                    double myDouble_AngleToBasis = XYZ.BasisZ.AngleTo(myTransform.BasisZ);  //2 places line , 3 places method

                    switch (myWindow1.mySlider.Name)
                    {
                        case "mySlider_Rotate_BasisY":
                            myDouble_AngleToBasis = XYZ.BasisY.AngleTo(myTransform.BasisY);  //2 places line , 3 places method
                            break;
                        case "mySlider_Rotate_BasisX":
                            myDouble_AngleToBasis = XYZ.BasisX.AngleTo(myTransform.BasisX);  //2 places line , 3 places method
                            break;
                    }

                    if (IsZero(myDouble_AngleToBasis)) myTransform_ToMakeTheRotationRelative = Transform.Identity;
                    else
                    {
                        XYZ axis = myTransform.BasisZ.CrossProduct(-XYZ.BasisZ);  //2 places line , 3 places method,  normally (z) negative

                        switch (myWindow1.mySlider.Name)
                        {
                            case "mySlider_Rotate_BasisY":
                                axis = myTransform.BasisY.CrossProduct(-XYZ.BasisY); //2 places line , 3 places method,  normally (z) negative
                                break;
                            case "mySlider_Rotate_BasisX":
                                axis = myTransform.BasisX.CrossProduct(-XYZ.BasisX); //2 places line , 3 places method,  normally (z) negative
                                break;
                        }

                        myTransform_ToMakeTheRotationRelative = Transform.CreateRotationAtPoint(axis, myDouble_AngleToBasis, XYZ.Zero);
                    }
                }

                double myDouble = -2.4;
                for (int i = 0; i < 25; i++)
                {
                    myWindow1.myListTransform.Add(new Transform(myTransform) { Origin = myTransform.Origin /*+ new XYZ(myDouble, 0, 0)*/ });
                    myDouble = myDouble + 0.2;
                }

                int myIntTimeOut = 0;
                int myInt_ChangeCount = 0;
                double myDouble_ChangePosition = -1;

                using (TransactionGroup transGroup = new TransactionGroup(doc))
                {
                    transGroup.Start("Transform animation 2");

                    bool myBool_Perform_Unhost_And_Rehost = true;
                    if (myReferencePoint_Centre.GetPointElementReference() == null) myBool_Perform_Unhost_And_Rehost = false;

                    PointOnFace myPointOnFace = (PointOnFace)myReferencePoint_Centre.GetPointElementReference();

                    if (myBool_Perform_Unhost_And_Rehost)
                    {
                        using (Transaction y = new Transaction(doc, "Remove Hosting"))
                        {
                            y.Start();
                            myReferencePoint_Centre.get_Parameter(BuiltInParameter.POINT_ELEMENT_DRIVEN).Set(0);
                            y.Commit();
                        }
                    }

                    while (myWindow1.mySlideInProgress)
                    {
                        wait(100); myIntTimeOut++;

                        if (myDouble_ChangePosition != myWindow1.mySlider.Value)
                        {
                            myDouble_ChangePosition = myWindow1.mySlider.Value;
                            myWindow1.myLabel_ChangeCount.Content = myInt_ChangeCount++.ToString();
                            using (Transaction y = new Transaction(doc, "a Transform"))
                            {
                                y.Start();

                                double myDoubleRotateAngle = ((Math.PI * 2) / 24) * myDouble_ChangePosition;

                                Transform myTransform_Rotate = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI + -myDoubleRotateAngle, XYZ.Zero); //1 places line and DOUBLE normally (z) negative , 3 places method

                                switch (myWindow1.mySlider.Name)
                                {
                                    case "mySlider_Rotate_BasisY":
                                        myTransform_Rotate = Transform.CreateRotationAtPoint(XYZ.BasisY, Math.PI + -myDoubleRotateAngle, XYZ.Zero); //1 places line and DOUBLE normally (z) negative , 3 places method
                                        myBool_Perform_Unhost_And_Rehost = false;
                                        break;
                                    case "mySlider_Rotate_BasisX":
                                        myTransform_Rotate = Transform.CreateRotationAtPoint(XYZ.BasisX, Math.PI + -myDoubleRotateAngle, XYZ.Zero); //1 places line and DOUBLE normally (z) negative , 3 places method
                                        myBool_Perform_Unhost_And_Rehost = false;
                                        break;
                                }

                                Transform myTransform_Temp = Transform.Identity;
                                myTransform_Temp.Origin = myReferencePoint_Centre.GetCoordinateSystem().Origin;

                                myTransform_Temp.BasisX = myTransform_ToMakeTheRotationRelative.OfVector(myTransform_Rotate.BasisX);
                                myTransform_Temp.BasisY = myTransform_ToMakeTheRotationRelative.OfVector(myTransform_Rotate.BasisY);
                                myTransform_Temp.BasisZ = myTransform_ToMakeTheRotationRelative.OfVector(myTransform_Rotate.BasisZ);

                                myReferencePoint_Centre.SetCoordinateSystem(myTransform_Temp);

                                if(myBool_InterpolateMiddle_WhenEitherA_or_B)
                                {
                                    myWindow1.myMethod_whichTook_120Hours_OfCoding();

                                    myReferencePoint_Middle.SetCoordinateSystem(myWindow1.myListTransform_Interpolate[(int)myWindow1.mySlider_Interpolate.Value]);
                                }
                                y.Commit();

                                myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisZ, false);
                                myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisX, false);
                                myWindow1.setSlider(myWindow1.myIntUpDown_Middle2, myWindow1.mySlider_Rotate_BasisY, true);
                            }
                        }

                        myWindow1.myLabel_Setting.Content = myWindow1.mySlider.Value.ToString();

                        if (myIntTimeOut == 400)
                        {
                            MessageBox.Show("Timeout");
                            break;
                        }
                    }

                    transGroup.Assimilate();
                }

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE03_RotateAroundBasis" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
