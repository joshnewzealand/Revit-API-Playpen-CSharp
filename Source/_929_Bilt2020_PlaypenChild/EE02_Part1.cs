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
    public class EE02_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                    tx.Start("Appropriate descriptor");

                    tx.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE02_Part1 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                using (Transaction y = new Transaction(doc, "Foreach on each wall type."))
                {
                    y.Start();
                    List<ElementId> myFEC_Walls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() == "Example 2 Walls").Select(x => x.Id).ToList();
                    if (myFEC_Walls.Count != 0)
                    {
                        doc.Delete(myFEC_Walls);
                    } else
                    {
                        ///                  TECHNIQUE 2 OF 19
                        ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ ONE OF EACH WAL L↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

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
                        ///↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑
                    }

                    y.Commit();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE02_Part1_SetDefault : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; 

                if (uidoc.Selection.GetElementIds().Count != 1)
                {
                    MessageBox.Show("Please select ONE Element.");
                    return;
                }

                Element myElement = doc.GetElement(uidoc.Selection.GetElementIds().First());


                ///                  TECHNIQUE 3 OF 19
                ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ SET DEFAULT TYPE ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

                if (myElement.GetTypeId() == null)
                {
                    MessageBox.Show("Selected entity does not have types.");
                    return;
                }


                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Set Default Type");

                    if (myElement.GetType() == typeof(FamilyInstance))
                    {
                        doc.SetDefaultFamilyTypeId(myElement.Category.Id, ((FamilyInstance)myElement).Symbol.Id);
                        
                    } else
                    {
                        ElementType myElementType = doc.GetElement(myElement.GetTypeId()) as ElementType;
                        foreach (ElementTypeGroup myElementTypeGroup in Enum.GetValues(typeof(ElementTypeGroup)))
                        {
                            if (myElementTypeGroup.ToString() == myElementType.GetType().Name)
                            {
                                doc.SetDefaultElementTypeId(myElementTypeGroup, myElement.GetTypeId());
                            }
                        }
                    }
                    tx.Commit();
                }

                MessageBox.Show("'" + doc.GetElement(myElement.GetTypeId()).Name + "'" + Environment.NewLine + Environment.NewLine + "Is now the defaulf of category." + Environment.NewLine + Environment.NewLine + "'" + myElement.Category.Name + "'"  );

                ///↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE02_Part1_SetDefault" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
