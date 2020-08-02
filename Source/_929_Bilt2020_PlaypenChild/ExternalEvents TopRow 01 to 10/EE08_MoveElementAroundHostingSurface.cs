using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB.Events;
using System.Runtime.InteropServices;
using _952_PRLoogleClassLibrary;

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE08_MoveElementAroundHostingSurface : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }
        public List<ElementId> myListElementID { get; set; } = new List<ElementId>();

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                if (uidoc.Selection.GetElementIds().Count != 1)
                {
                    MessageBox.Show("Please select ONE Wall.");
                    return;
                }

                Element myElementWall = doc.GetElement(uidoc.Selection.GetElementIds().First());

                if (myElementWall.GetType() == typeof(FamilyInstance))
                {
                    FamilyInstance myFamilyInstance = myElementWall as FamilyInstance;

                    myElementWall = myFamilyInstance.Host;

                    if (myElementWall == null)
                    {
                        MessageBox.Show("Family instance must be hosted to a wall.");
                        return;
                    }
                }

                List<Element> myListOfStuffOnWall = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).Where(x => (((FamilyInstance)x).Host != null)).Where(x => ((FamilyInstance)x).Host.Id == myElementWall.Id).ToList();
                List<Element> myListOfFurniture = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_Furniture).Where(x => (((FamilyInstance)x).Host != null)).Where(x => ((FamilyInstance)x).Host.Id == myElementWall.Id).ToList();
                myListOfStuffOnWall.AddRange(myListOfFurniture);

                if (myListOfStuffOnWall.Count() > 0)
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Rotate People");

                        foreach (FamilyInstance myFI in myListOfStuffOnWall)
                        {
                            FamilyInstance myFamilyInstance = myFI as FamilyInstance;

                            Reference pickedRef = myFI.HostFace;

                            ////doc.Delete(myListElementID);
                            ////myListElementID.Clear();

                            Face myFace = myElementWall.GetGeometryObjectFromReference(pickedRef) as Face;
                            if (myFace == null) return;


                            Transform myXYZ_FamilyTransform = Transform.Identity;

                            if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
                            {
                                myXYZ_FamilyTransform = (myElementWall as FamilyInstance).GetTotalTransform();
                            }

                            Transform myTransform = Transform.Identity;

                            if (myFace.GetType() != typeof(PlanarFace)) return;

                            PlanarFace myPlanarFace = myFace as PlanarFace;

                            UV myUV_Min = myFace.GetBoundingBox().Min;
                            UV myUV_Max = myFace.GetBoundingBox().Max;

                            XYZ myXYZ_CornerOne = myFace.Evaluate(myUV_Min);
                            XYZ myXYZ_CornerTwo = myFace.Evaluate(myUV_Max);

                            XYZ myXYZ_CornerOne_Transformed = myXYZ_FamilyTransform.OfPoint(myXYZ_CornerOne);
                            XYZ myXYZ_CornerTwo_Transformed = myXYZ_FamilyTransform.OfPoint(myXYZ_CornerTwo);

                            myTransform.Origin = myXYZ_CornerOne_Transformed;
                            myTransform.BasisX = myXYZ_FamilyTransform.OfVector(myPlanarFace.XVector);
                            myTransform.BasisY = myXYZ_FamilyTransform.OfVector(myPlanarFace.YVector);
                            myTransform.BasisZ = myXYZ_FamilyTransform.OfVector(myPlanarFace.FaceNormal);

                            XYZ myXYZ_Centre = XYZ.Zero;

                            XYZ myXYZ_Min = new XYZ(myUV_Min.U, myUV_Min.V, 0);
                            XYZ myXYZ_Max = new XYZ(myUV_Max.U, myUV_Max.V, 0);

                            XYZ myXYZ_UV_CornerOne = (((myXYZ_Max - myXYZ_Min) * 0.1));//; + myXYZ_Min;
                            XYZ myXYZ_UV_CornerTwo = (((myXYZ_Max - myXYZ_Min) * 0.9));// + myXYZ_Min;

                            XYZ myXYZ_UV_Corner01 = new XYZ(myXYZ_UV_CornerOne.X, myXYZ_UV_CornerOne.Y, 0);
                            XYZ myXYZ_UV_Corner02 = new XYZ(myXYZ_UV_CornerTwo.X, myXYZ_UV_CornerOne.Y, 0);
                            XYZ myXYZ_UV_Corner03 = new XYZ(myXYZ_UV_CornerTwo.X, myXYZ_UV_CornerTwo.Y, 0);
                            XYZ myXYZ_UV_Corner04 = new XYZ(myXYZ_UV_CornerOne.X, myXYZ_UV_CornerTwo.Y, 0);

                            Line L1 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner01), myTransform.OfPoint(myXYZ_UV_Corner02));
                            Line L2 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner02), myTransform.OfPoint(myXYZ_UV_Corner03));
                            Line L3 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner03), myTransform.OfPoint(myXYZ_UV_Corner04));
                            Line L4 = Line.CreateBound(myTransform.OfPoint(myXYZ_UV_Corner04), myTransform.OfPoint(myXYZ_UV_Corner01));

                            CurveLoop myCurveLoop = new CurveLoop();
                            myCurveLoop.Append(L1);
                            myCurveLoop.Append(L2);
                            myCurveLoop.Append(L3);
                            myCurveLoop.Append(L4);

                            double myDouble_ExactLength = myCurveLoop.GetExactLength();
                            double myDouble_ExactLength_Twenty = myDouble_ExactLength / 40;

                            XYZ myXYZ_Result = myTransform.OfPoint((myXYZ_Max - myXYZ_Min) / 2);

                            int myIntCurrentSpinnerValue = myWindow1.myIntegerUpDown_OneToTwentyCount.Value.Value;

                            int myInt_Positioning = (40 / myListOfStuffOnWall.Count()) * (myListOfStuffOnWall.IndexOf(myFI) + 1);

                            myIntCurrentSpinnerValue = (40 - myInt_Positioning) + myIntCurrentSpinnerValue;

                            if (myIntCurrentSpinnerValue > 40) myIntCurrentSpinnerValue = myIntCurrentSpinnerValue - 40;

                            double myDouble_FutureForeach = myDouble_ExactLength_Twenty * myIntCurrentSpinnerValue;

                            double myDouble_ThisFarAlong;

                            switch (myDouble_FutureForeach)
                            {
                                case double n when n < L1.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach;
                                    myXYZ_Result = L1.GetEndPoint(0) + (L1.GetEndPoint(1) - L1.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                                case double n when n >= L1.Length & n < L1.Length + L2.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach - L1.Length;
                                    myXYZ_Result = L2.GetEndPoint(0) + (L2.GetEndPoint(1) - L2.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                                case double n when n >= L1.Length + L2.Length & n < L1.Length + L2.Length + L3.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach - L1.Length - L2.Length;
                                    myXYZ_Result = L3.GetEndPoint(0) + (L3.GetEndPoint(1) - L3.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                                case double n when n >= L1.Length + L2.Length + L3.Length:

                                    myDouble_ThisFarAlong = myDouble_FutureForeach - L1.Length - L2.Length - L3.Length;
                                    myXYZ_Result = L4.GetEndPoint(0) + (L4.GetEndPoint(1) - L4.GetEndPoint(0)).Normalize().Multiply(myDouble_ThisFarAlong);

                                    break;
                            }

                            XYZ myXYZ_MoveThisMuch = myXYZ_Result - ((LocationPoint)myFamilyInstance.Location).Point;
                            ElementTransformUtils.MoveElement(doc, myFamilyInstance.Id, myXYZ_MoveThisMuch);

                            //SketchPlane mySketchPlane = SketchPlane.Create(doc, pickedRef);
                            //myListElementID.Add(doc.Create.NewModelCurve(L1, mySketchPlane).Id);
                            //myListElementID.Add(doc.Create.NewModelCurve(L2, mySketchPlane).Id);
                            //myListElementID.Add(doc.Create.NewModelCurve(L3, mySketchPlane).Id);
                            //myListElementID.Add(doc.Create.NewModelCurve(L4, mySketchPlane).Id);

                        }
                        tx.Commit();
                    }
                    if (myWindow1.myIntegerUpDown_OneToTwentyCount.Value.Value == 40) myWindow1.myIntegerUpDown_OneToTwentyCount.Value = 0;
                    myWindow1.myIntegerUpDown_OneToTwentyCount.Value++;
                }
            }
            #region catch and finally
            catch (Exception ex)
            {
                DatabaseMethods.writeDebug("EE08_MoveElementAroundHostingSurface" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
