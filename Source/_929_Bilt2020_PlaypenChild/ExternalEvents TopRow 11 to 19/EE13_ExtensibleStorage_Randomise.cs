
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
using Autodesk.Revit.DB.ExtensibleStorage;

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE13_ExtensibleStorage_Randomise : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }
        public bool myBool_RunFromModeless { get; set; } = true;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

               

                if (myWindow1.myWindow4.myListViewEE.Items.Count == 0)
                {
                    MessageBox.Show("Please save an an arrangement");
                    return;
                }

                if (myWindow1.myWindow4.myListViewEE.SelectedIndex == -1) myWindow1.myWindow4.myListViewEE.SelectedIndex = 0;

                Random rnd = new Random();

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Show Arrangement");

                    KeyValuePair<string, Autodesk.Revit.DB.ExtensibleStorage.Entity> myKeyValuePair = (KeyValuePair<string, Entity>)myWindow1.myWindow4.myListViewEE.SelectedItem;

                    IDictionary<ElementId, XYZ> dict_Child = myKeyValuePair.Value.Get<IDictionary<ElementId, XYZ>>("FurnLocations", DisplayUnitType.DUT_MILLIMETERS);

                    string myStringAggregate_Location = "";

                    foreach (KeyValuePair<ElementId, XYZ> myKP in dict_Child)
                    {
                        Element Searchelem = doc.GetElement(myKP.Key);
                        if (Searchelem == null) continue;

                        if (Searchelem.Category.Name != "Furniture") continue;

                        OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                        OverrideGraphicSettings ogsCheeck = doc.ActiveView.GetElementOverrides(myKP.Key);
                        //FillPatternElement myFillPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.Name.Contains("Solid fill"));
                        List<FillPatternElement> myListFillPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().Where(x => x.GetFillPattern().Target == FillPatternTarget.Drafting).ToList();

                        FillPatternElement myFillPattern = myListFillPattern[(byte)rnd.Next(0, myListFillPattern.Count - 1)];

                        ogs.SetSurfaceBackgroundPatternId(myFillPattern.Id);

                        Color myColor_Random01 = new Autodesk.Revit.DB.Color((byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256), (byte)rnd.Next(0, 256));


                        ogs.SetProjectionLineWeight(5);
                        ogs.SetProjectionLineColor(myColor_Random01);
                        ogs.SetSurfaceBackgroundPatternColor(myColor_Random01);

                        doc.ActiveView.SetElementOverrides(myKP.Key, ogs);

                        myStringAggregate_Location = myStringAggregate_Location + Environment.NewLine + ((FamilyInstance)doc.GetElement(myKP.Key)).get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM).AsValueString() + "," + Math.Round(myKP.Value.X, 2) + "," + Math.Round(myKP.Value.Y, 2) + "," + Math.Round(myKP.Value.Z, 2);
                    }
                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_ManualColorOverride" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
