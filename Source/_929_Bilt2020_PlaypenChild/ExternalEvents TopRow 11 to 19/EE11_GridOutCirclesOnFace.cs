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
    public class EE11_GridOutCirclesOnFace : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public bool myBool_DoLoop { get; set; } = true;
        public bool myBool_JustClear { get; set; } = false;

        public List<ElementId> myListElementID { get; set; } = new List<ElementId>();

        UIApplication uiapp;

        public Reference myReference { get; set; } = null;

        public void Execute(UIApplication uiappp)
        {
            uiapp = uiappp;

            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                if (!myBool_JustClear)
                {
                    if (myBool_DoLoop)
                    {
                        myListElementID.Clear();
                        do
                        {
                            Reference pickedRef = null;

                            try
                            {
                                pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face, "Please select a Face");
                            }

                            #region catch and finally
                            catch (Exception ex)
                            {
                            }
                            finally
                            {
                            }
                            #endregion

                            if (pickedRef == null) break;
                            myReference = pickedRef;

                            myWindow1.my11IntegerUpDown_Columns.IsEnabled = true;
                            myWindow1.my11IntegerUpDown_Rows.IsEnabled = true;
                            myWindow1.my11CheckBox_OneTwoOne.IsEnabled = true;


                            CreatingCircles_ReturnsBreak(myReference);

                        } while (true);
                    }
                    else
                    {
                        CreatingCircles_ReturnsBreak(myReference);
                    }
                }
                else
                {
                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Delete the previous circles.");
                        doc.Delete(myListElementID);
                        myListElementID.Clear();
                        tx.Commit();
                    }

                    myReference = null;

                    myWindow1.my11IntegerUpDown_Columns.IsEnabled = false;
                    myWindow1.my11IntegerUpDown_Rows.IsEnabled = false;
                    myWindow1.my11CheckBox_OneTwoOne.IsEnabled = false;

                }

            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_GridOutCirclesOnFace" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void CreatingCircles_ReturnsBreak(Reference pickedRef)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            ///           TECHNIQUE 11 OF 19 (EE11_GridOutCirclesOnFace.cs)
            ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ GRID OUT CIRCLES ON FACE ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
            ///
            /// Interfaces and ENUM's:
            ///     
            /// 
            /// Demonstrates classes:
            ///     Xceed.Wpf.Toolkit.IntegerUpDown*
            /// 
            /// 
            /// Key methods:
            ///     doc.Delete(
            ///
            ///   A 2 DIMENSION MATRIX IS A LOOP WITIN A LOOP
            ///        for (int int_Outer = 1; int_Outer <= myWindow1.my11IntegerUpDown_Columns.Value.Value * (myBool_OneTwoOneLayout ? 2 : 1); int_Outer++)
            ///        {
            ///                for (int int_Inner = 1; int_Inner <= myWindow1.my11IntegerUpDown_Rows.Value.Value * (myBool_OneTwoOneLayout ? 2 : 1); int_Inner++)
            ///                {
            ///                    XYZ myXYZ_GridPoint = new XYZ(myXYZ_DifferernceColumn.X * int_Outer, myXYZ_DifferernceRow.Y * int_Inner, 0);
            ///                        Arc geomPlane3 = Arc.Create(myTransform.OfPoint(myXYZ_GridPoint), radius, startAngle, endAngle, myTransform.BasisX, myTransform.BasisY);
            ///                        doc.Create.NewModelCurve(geomPlane3, mySketchPlane).Id;
            ///                }
            ///        }
            /// 
            ///
            ///
            /// * class is part xceed.wpf.toolkit (not Revit API)
			///	https://github.com/joshnewzealand/Revit-API-Playpen-CSharp


            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Delete the previous circles.");
                doc.Delete(myListElementID);
                myListElementID.Clear();
                tx.Commit();
            }

            Element myElement = doc.GetElement(pickedRef.ElementId);

            Face myFace = myElement.GetGeometryObjectFromReference(pickedRef) as Face;
            if (myFace == null) return;

            Transform myXYZ_FamilyTransform = Transform.Identity;

            if (pickedRef.ConvertToStableRepresentation(doc).Contains("INSTANCE"))
            {
                myXYZ_FamilyTransform = (myElement as FamilyInstance).GetTotalTransform();
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


            bool myBool_OneTwoOneLayout = myWindow1.my11CheckBox_OneTwoOne.IsChecked.Value;

            int myInt_Columns = myWindow1.my11IntegerUpDown_Columns.Value.Value;
            int myInt_Rows = myWindow1.my11IntegerUpDown_Rows.Value.Value;

            int myIntDivideColumn = myBool_OneTwoOneLayout ? myInt_Columns * 2 : myInt_Columns + 1;
            int myIntDivideRow = myBool_OneTwoOneLayout ? myInt_Rows * 2 : myInt_Rows + 1;

            XYZ myXYZ_DifferernceColumn = (myXYZ_Max - myXYZ_Min) / myIntDivideColumn;
            double myDouble_WidthColumn = myXYZ_DifferernceColumn.X;

            XYZ myXYZ_DifferernceRow = (myXYZ_Max - myXYZ_Min) / myIntDivideRow;
            double myDouble_WidthRow = myXYZ_DifferernceRow.Y;

            double myDouble_SmallestWins = myDouble_WidthColumn < myDouble_WidthRow ? myDouble_WidthColumn : myDouble_WidthRow;

            double startAngle = 0;
            double endAngle = 2 * Math.PI;
            double radius = myDouble_SmallestWins / (myBool_OneTwoOneLayout ? 1 : 2);


            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Draw Grid of Circles On Face");
                SketchPlane mySketchPlane = SketchPlane.Create(doc, pickedRef);

                for (int int_Outer = 1; int_Outer <= myWindow1.my11IntegerUpDown_Columns.Value.Value * (myBool_OneTwoOneLayout ? 2 : 1); int_Outer++)
                {
                    if (int_Outer % 2 != 0 | !myBool_OneTwoOneLayout) //here
                    {
                        for (int int_Inner = 1; int_Inner <= myWindow1.my11IntegerUpDown_Rows.Value.Value * (myBool_OneTwoOneLayout ? 2 : 1); int_Inner++)
                        {
                            XYZ myXYZ_GridPoint = new XYZ(myXYZ_DifferernceColumn.X * int_Outer, myXYZ_DifferernceRow.Y * int_Inner, 0);

                            if (int_Inner % 2 != 0 | !myBool_OneTwoOneLayout) //here
                            {
                                Arc geomPlane3 = Arc.Create(myTransform.OfPoint(myXYZ_GridPoint), radius, startAngle, endAngle, myTransform.BasisX, myTransform.BasisY);
                                myListElementID.Add(doc.Create.NewModelCurve(geomPlane3, mySketchPlane).Id);
                            }
                        }
                    }
                }
                
                tx.Commit();
            }
            return;
        }

        public string GetName()
        {
            return "External Event Example";
        }
    }
}
