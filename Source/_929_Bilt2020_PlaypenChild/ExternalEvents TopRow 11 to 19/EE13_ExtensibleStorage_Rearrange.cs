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
using Autodesk.Revit.DB.ExtensibleStorage;


namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE13_ExtensibleStorage_Rearrange : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);


                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Show Arrangement");

                    KeyValuePair<string, Entity> myKeyValuePair = (KeyValuePair<string, Entity>)myWindow1.myWindow4.myListViewEE.SelectedItem;

                    IDictionary<ElementId, XYZ> dict_Child = myKeyValuePair.Value.Get<IDictionary<ElementId, XYZ>>("FurnLocations", DisplayUnitType.DUT_MILLIMETERS);

                    string myStringAggregate_Location = "";

                    foreach (KeyValuePair<ElementId, XYZ> myKP in dict_Child)
                    {
                        Element Searchelem = doc.GetElement(myKP.Key);
                        if (Searchelem == null) continue;

                        ElementTransformUtils.MoveElement(doc, myKP.Key, myKP.Value - ((LocationPoint)Searchelem.Location).Point);

                        myStringAggregate_Location = myStringAggregate_Location + Environment.NewLine + ((FamilyInstance)doc.GetElement(myKP.Key)).get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString() + "," + Math.Round(myKP.Value.X, 2) + "," + Math.Round(myKP.Value.Y, 2) + "," + Math.Round(myKP.Value.Z, 2);
                    }


                    IDictionary<ElementId, double> dict_Child_Angle = myKeyValuePair.Value.Get<IDictionary<ElementId, double>>("FurnLocations_Angle", DisplayUnitType.DUT_MILLIMETERS);

                    string myStringAggregate_Angle = "";

                    foreach (KeyValuePair<ElementId, double> myKP in dict_Child_Angle)
                    {
                        FamilyInstance Searchelem = doc.GetElement(myKP.Key) as FamilyInstance;
                        if (Searchelem == null) continue;

                        Line line = Line.CreateBound(((LocationPoint)Searchelem.Location).Point, ((LocationPoint)Searchelem.Location).Point + XYZ.BasisZ);

                        double myDouble = Searchelem.GetTransform().BasisX.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ) - myKP.Value;

                        ElementTransformUtils.RotateElement(doc, myKP.Key, line, myDouble);

                        myStringAggregate_Angle = myStringAggregate_Angle + Environment.NewLine + (IsZero(myKP.Value, _eps) ? 0 : Math.Round(myKP.Value, 2));
                    }

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE13_ExtensibleStorage_Rearrange" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }
        const double _eps = 1.0e-9;
    }
}
