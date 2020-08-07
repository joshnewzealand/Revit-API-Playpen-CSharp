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

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE04_ManualColorOverride : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }
        public bool myBool_RunFromModeless { get; set; } = true;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ///                  TECHNIQUE 4 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ COLOUR OVERRIDE ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     BuiltInParameter.VIEW_DESCRIPTION
                /// 
                /// Demonstrates classes:
                ///     UIDocument
                ///     OverrideGraphicSettings
                ///     FillPatternElement
                /// 
                /// Key methods:
                ///     GetElementOverrides
                ///     ogs.SetSurfaceBackgroundPatternId(
                ///     ogs.SetSurfaceBackgroundPatternColor(
                ///     SetElementOverrides
                ///
                /// 
                /// * class is actually part of the .NET framework (not Revit API)


                Element myElement = null;
                if (uidoc.Selection.GetElementIds().Count == 0)
                {
                    if (myBool_RunFromModeless)
                    {
                        string myString_RememberLast = uidoc.ActiveView.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).AsString();  //stores the last ID in the VIEW_DESCRIPTION, it would be more appropriate to store it in a DataStorage entity.
                        int n;
                        if (!int.TryParse(myString_RememberLast, out n))
                        {
                            MessageBox.Show("Please select just one geometric element (e.g. a Wall).");
                            return;
                        }
                        myElement = doc.GetElement(new ElementId(n));
                    } else
                    {
                        Reference pickedRef = null;
                        try
                        {
                            pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Please select an Element");
                        }

                        #region catch and finally
                        catch (Exception ex)
                        {
                        }
                        finally
                        {
                        }
                        #endregion

                        if (pickedRef == null) return;
                        myElement = doc.GetElement(pickedRef.ElementId);
                    }
                }
                else
                {
                    myElement = doc.GetElement(uidoc.Selection.GetElementIds().First());
                }
                if (myElement == null) return;


                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Manual Color Override");

                    OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                    OverrideGraphicSettings ogsCheeck = doc.ActiveView.GetElementOverrides(myElement.Id);
                    FillPatternElement myFillPattern = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.Name.Contains("Solid fill"));

                    ogs.SetSurfaceBackgroundPatternId(myFillPattern.Id);
                    ogs.SetSurfaceBackgroundPatternColor(new Autodesk.Revit.DB.Color(255, 255, 0));

                    if (ogsCheeck.SurfaceBackgroundPatternId.IntegerValue != -1) 
                    {
                        doc.ActiveView.SetElementOverrides(myElement.Id, new OverrideGraphicSettings()); //if it already has overwrites, this removes it.
                    }
                    else
                    {
                        doc.ActiveView.SetElementOverrides(myElement.Id, ogs);
                    }

                    uidoc.ActiveView.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).Set(myElement.Id.IntegerValue.ToString());
                    tx.Commit();
                }

                uidoc.Selection.SetElementIds(new List<ElementId>() );
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
