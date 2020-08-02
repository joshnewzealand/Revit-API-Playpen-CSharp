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

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE07_RotatingEntities : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

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

                            XYZ myXYZInstance = ((LocationPoint)myFamilyInstance.Location).Point;

                            Line myLineBasisY = Line.CreateUnbound(myXYZInstance, myFamilyInstance.GetTransform().BasisZ); //  tf_01072_4 = Transform.CreateRotation(myTransform_FakeBasis2.BasisZ, d1 / 10 * myInt_DivideInteger);

                            ElementTransformUtils.RotateElement(doc, myFamilyInstance.Id, myLineBasisY, Math.PI / 4);
                        }

                        tx.Commit();
                    }
                }

                if (myWindow1.myCheckBoxWithWall.IsChecked.Value)
                {
                    if (myElementWall.Category.Name == "Walls")
                    {
                        Wall myWall = myElementWall as Wall;

                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Rotate Wall");

                            Curve myCurve = ((LocationCurve)myWall.Location).Curve;

                            XYZ myXYZ = (myCurve.GetEndPoint(1) + myCurve.GetEndPoint(0)) / 2;

                            Line myLineBasisZ = Line.CreateUnbound(myXYZ, XYZ.BasisZ);

                            ElementTransformUtils.RotateElement(doc, myWall.Id, myLineBasisZ, Math.PI / 20);

                            tx.Commit();
                        }

                    }

                }
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE07_RotatingEntities" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
