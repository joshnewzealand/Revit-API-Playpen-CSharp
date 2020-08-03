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
    public class EE02_OneOfEachWall : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public MainWindow myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                ///                  TECHNIQUE 2 OF 19 (EE02_OneOfEachWall.cs)
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ ONE OF EACH WALL ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
                ///
                /// Interfaces and ENUM's:
                ///     BuiltInCategory.OST_Walls
                ///     BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS
                /// 
                /// Demonstrates classes:
                ///     FilteredElementCollector
                ///     Line
                ///     Wall
                /// 
                /// Key methods:
                ///     doc.Delete(
                ///     new XYZ(
                ///     Line.CreateBound(
                ///     Wall.Create(
                ///     myWall.get_Parameter(
                ///
                ///


                using (Transaction y = new Transaction(doc, "Foreach on each wall type."))
                {
                    y.Start();
                    List<ElementId> myFEC_Walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() == "Example 2 Walls").Select(x => x.Id).ToList();
                    if (myFEC_Walls.Count != 0)
                    {
                        doc.Delete(myFEC_Walls);
                    }
                    else
                    {
                        FilteredElementCollector myFEC_WallTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();

                        double myX = 0;
                        Element myLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First();

                        foreach (ElementId myElementID in myFEC_WallTypes.ToElementIds())
                        {
                            // Creates a geometry line in Revit application
                            XYZ startPoint = new XYZ(myX, 10, 0);
                            XYZ endPoint = new XYZ(myX, 0, 0);
                            Line geomLine = Line.CreateBound(startPoint, endPoint);

                            Wall myWall = Wall.Create(doc, geomLine, myElementID, myLevel.Id, 10, 0, false, false);
                            myWall.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).Set("Example 2 Walls");

                            myX = myX + 3;
                        }
                    }

                    y.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE02_OneOfEachWall" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
