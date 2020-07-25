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

namespace _929_Bilt2020_PlaypenChild
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE03_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
    public class EE03_Part1 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);


                Wall myWall = doc.GetElement(new ElementId(165358)) as Wall;

                List<Element> myListElementId_Furniture = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Furniture)
                    .WhereElementIsNotElementType().Where(x => x.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString() == "03 - Floor").Where(x => ((LocationPoint)x.Location).Point.X > ((LocationCurve)myWall.Location).Curve.GetEndPoint(0).X).Where(x => ((LocationPoint)x.Location).Point.Y < ((LocationCurve)myWall.Location).Curve.GetEndPoint(0).Y).ToList();

                //MessageBox.Show(myListElementId_WallTypes.Count().ToString());

                using (Transaction y = new Transaction(doc, "Save Furniture Arrangement"))
                {
                    y.Start();

                    Schema schema_FurnLocations = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations));
                    Entity ent_Child = new Entity(schema_FurnLocations);
                    IDictionary<ElementId, XYZ> dict_Child = new Dictionary<ElementId, XYZ>();
                    Entity ent_Child_Angle = new Entity(schema_FurnLocations);
                    IDictionary<ElementId, double> dict_Child_Angle = new Dictionary<ElementId, double>();

                    foreach (Element myEle in myListElementId_Furniture)
                    {
                        dict_Child.Add(myEle.Id, ((LocationPoint)myEle.Location).Point);

                        double myDouble = ((FamilyInstance)myEle).GetTransform().BasisX.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ);
                        //MessageBox.Show(myDouble.ToString());

                        dict_Child_Angle.Add(myEle.Id, myDouble);
                    }

                    ent_Child.Set<IDictionary<ElementId, XYZ>>("FurnLocations", dict_Child, DisplayUnitType.DUT_MILLIMETERS);
                    ent_Child.Set<IDictionary<ElementId, double>>("FurnLocations_Angle", dict_Child_Angle, DisplayUnitType.DUT_MILLIMETERS);

                    Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                    Entity ent_Parent = doc.ProjectInformation.GetEntity(schema_FurnLocations_Index);
                    IDictionary<int, Entity> dict_Parent = ent_Parent.Get<IDictionary<int, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                    int myIntZero = 0; 
                    if(dict_Parent.Count != 0)
                    {
                        myIntZero = dict_Parent.Keys.Max() + 1;
                    }

                    dict_Parent.Add(myIntZero, ent_Child);
                    ent_Parent.Set<IDictionary<int, Entity>>("FurnLocations_Index", dict_Parent, DisplayUnitType.DUT_MILLIMETERS);

                    doc.ProjectInformation.SetEntity(ent_Parent);

                    //MessageBox.Show(dict_Parent.Keys.Max().ToString());

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
    public class EE03_Part2 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                    tx.Start("SequenceThrough");

                    Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                    Entity ent_Parent = doc.ProjectInformation.GetEntity(schema_FurnLocations_Index);

                    IDictionary<int, Entity> dict_Parent = ent_Parent.Get<IDictionary<int, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                    if(dict_Parent.Count != 0)
                    {
                        if (dict_Parent.Count == myWindow1.myPublicInt) myWindow1.myPublicInt = 0;

                        Entity myEntity_Child = dict_Parent.ElementAt(myWindow1.myPublicInt).Value;

                        IDictionary<ElementId, XYZ> dict_Child = myEntity_Child.Get<IDictionary<ElementId, XYZ>>("FurnLocations", DisplayUnitType.DUT_MILLIMETERS);

                        foreach (KeyValuePair<ElementId, XYZ> myKP in dict_Child)
                        {
                            Element Searchelem = doc.GetElement(myKP.Key);
                            if (Searchelem == null) continue;

                            ElementTransformUtils.MoveElement(doc, myKP.Key, myKP.Value - ((LocationPoint)Searchelem.Location).Point);
                        }


                        IDictionary<ElementId, double> dict_Child_Angle = myEntity_Child.Get<IDictionary<ElementId, double>>("FurnLocations_Angle", DisplayUnitType.DUT_MILLIMETERS);

                        foreach (KeyValuePair<ElementId, double> myKP in dict_Child_Angle)
                        {
                            FamilyInstance Searchelem = doc.GetElement(myKP.Key) as FamilyInstance;
                            if (Searchelem == null) continue;

                            Line line = Line.CreateBound(((LocationPoint)Searchelem.Location).Point, ((LocationPoint)Searchelem.Location).Point + XYZ.BasisZ);

                            double myDouble = Searchelem.GetTransform().BasisX.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ) - myKP.Value;
                            //MessageBox.Show(myDouble.ToString());

                            ElementTransformUtils.RotateElement(doc, myKP.Key, line, myDouble);
                        }
                    }

                    tx.Commit();
                }

                myWindow1.myPublicInt++;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE03_Part2" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE03_Part3 : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                    tx.Start("ClearValues");

                    Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                    Entity ent_Parent = doc.ProjectInformation.GetEntity(schema_FurnLocations_Index);

                    IDictionary<int, Entity> dict_Parent = ent_Parent.Get<IDictionary<int, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                    dict_Parent.Clear();

                    ent_Parent.Set<IDictionary<int, Entity>>("FurnLocations_Index", dict_Parent, DisplayUnitType.DUT_MILLIMETERS);

                    doc.ProjectInformation.SetEntity(ent_Parent);

                    tx.Commit();
                }

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE03_Part2" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
