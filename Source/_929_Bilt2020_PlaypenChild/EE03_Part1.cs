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
    public class EE03_Part1_NewOrSave : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }
        public bool myBool_New { get; set; }


        //saving now saving now, saving now saving now , saving now 
        public void Execute(UIApplication uiapp)
        {
            try
            {
                //if it is new then this value gets stored
                KeyValuePair<string, Entity> myKeyValuePair = myBool_New ? new KeyValuePair<string, Entity>() : (KeyValuePair<string, Entity>)myWindow1.myWindow4.myListView.SelectedItem;

                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                List<ElementId> myFEC_DataStorage = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).WhereElementIsNotElementType().Where(x => x.Name == "Room Setup Entities").Select(x => x.Id).ToList();

                if (myFEC_DataStorage.Count == 0)
                {
                    MessageBox.Show("Please click the 'Place room' button before Saving");
                    return;
                }

                DataStorage myDatastorage = doc.GetElement(myFEC_DataStorage.First()) as DataStorage;
                List<Element> myFEC_RoomSetupEntities = new FilteredElementCollector(doc).WhereElementIsNotElementType().Where(x => x.LookupParameter("Comments") != null).Where(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsString() == "Room Setup Entities").ToList();

                Schema schema_FurnLocations = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations));
                if (schema_FurnLocations == null) schema_FurnLocations = Schema_FurnLocations.createSchema_FurnLocations();

                //not used
                Entity ent_Child = myBool_New ? new Entity(schema_FurnLocations) : myKeyValuePair.Value;
                IDictionary<ElementId, XYZ> dict_Child = new Dictionary<ElementId, XYZ>();
                IDictionary<ElementId, double> dict_Child_Angle = new Dictionary<ElementId, double>();

                foreach (Element myEle in myFEC_RoomSetupEntities)
                {
                    if (myEle.Location.GetType() == typeof(LocationPoint))
                    {
                        dict_Child.Add(myEle.Id, ((LocationPoint)myEle.Location).Point);

                        double myDouble = ((FamilyInstance)myEle).GetTransform().BasisX.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ);
                        dict_Child_Angle.Add(myEle.Id, myDouble);
                    }
                }

                ent_Child.Set<IDictionary<ElementId, XYZ>>("FurnLocations", dict_Child, DisplayUnitType.DUT_MILLIMETERS);
                ent_Child.Set<IDictionary<ElementId, double>>("FurnLocations_Angle", dict_Child_Angle, DisplayUnitType.DUT_MILLIMETERS);

                Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                if (schema_FurnLocations_Index == null) schema_FurnLocations_Index = Schema_FurnLocations.createSchema_FurnLocations_Index();

                Entity ent_Parent = myDatastorage.GetEntity(schema_FurnLocations_Index);
                IDictionary<string, Entity> dict_Parent = null; // new IDictionary<int, Entity>();

                if (ent_Parent.IsValid())
                {
                    dict_Parent = ent_Parent.Get<IDictionary<string, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);
                }
                else
                {
                    ent_Parent = new Entity(schema_FurnLocations_Index);
                    dict_Parent = new Dictionary<string, Entity>();
                }

                if(myBool_New)
                {
                    dict_Parent.Add(DateTime.Now.ToString("yyyyMMdd HHmm ss"), ent_Child);
                } else
                {
                    dict_Parent[myKeyValuePair.Key] = ent_Child;
                }

                ent_Parent.Set<IDictionary<string, Entity>>("FurnLocations_Index", dict_Parent, DisplayUnitType.DUT_MILLIMETERS);
                using (Transaction y = new Transaction(doc, "New Furniture Arrangement"))
                {
                    y.Start();

                    myDatastorage.SetEntity(ent_Parent);

                    y.Commit();
                }

                myWindow1.myWindow4.myListView.ItemsSource = dict_Parent;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE03_Part1_New" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE03_Part2_ShowArrangement : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                    tx.Start("Show Arrangement");

                    KeyValuePair<string, Entity> myKeyValuePair = (KeyValuePair<string, Entity>)myWindow1.myWindow4.myListView.SelectedItem;

                    IDictionary<ElementId, XYZ> dict_Child = myKeyValuePair.Value.Get<IDictionary<ElementId, XYZ>>("FurnLocations", DisplayUnitType.DUT_MILLIMETERS);

                    foreach (KeyValuePair<ElementId, XYZ> myKP in dict_Child)
                    {
                        Element Searchelem = doc.GetElement(myKP.Key);
                        if (Searchelem == null) continue;

                        ElementTransformUtils.MoveElement(doc, myKP.Key, myKP.Value - ((LocationPoint)Searchelem.Location).Point);
                    }

                    IDictionary<ElementId, double> dict_Child_Angle = myKeyValuePair.Value.Get<IDictionary<ElementId, double>>("FurnLocations_Angle", DisplayUnitType.DUT_MILLIMETERS);

                    foreach (KeyValuePair<ElementId, double> myKP in dict_Child_Angle)
                    {
                        FamilyInstance Searchelem = doc.GetElement(myKP.Key) as FamilyInstance;
                        if (Searchelem == null) continue;

                        Line line = Line.CreateBound(((LocationPoint)Searchelem.Location).Point, ((LocationPoint)Searchelem.Location).Point + XYZ.BasisZ);

                        double myDouble = Searchelem.GetTransform().BasisX.AngleOnPlaneTo(XYZ.BasisY, XYZ.BasisZ) - myKP.Value;
                        //MessageBox.Show(myDouble.ToString());

                        ElementTransformUtils.RotateElement(doc, myKP.Key, line, myDouble);
                    }

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


    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE03_Part2_DeleteOne : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                List<ElementId> myFEC_DataStorage = new FilteredElementCollector(doc).OfClass(typeof(DataStorage)).WhereElementIsNotElementType().Where(x => x.Name == "Room Setup Entities").Select(x => x.Id).ToList();

                if (myFEC_DataStorage.Count == 0)
                {
                    MessageBox.Show("Data storage entity is not present.");
                    return;
                }

                DataStorage myDatastorage = doc.GetElement(myFEC_DataStorage.First()) as DataStorage;

                Schema schema_FurnLocations_Index = Schema.Lookup(new Guid(Schema_FurnLocations.myConstantStringSchema_FurnLocations_Index));
                if (schema_FurnLocations_Index == null) schema_FurnLocations_Index = Schema_FurnLocations.createSchema_FurnLocations_Index();

                Entity ent_Parent = myDatastorage.GetEntity(schema_FurnLocations_Index);
                IDictionary<string, Entity> dict_Parent = ent_Parent.Get<IDictionary<string, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                KeyValuePair<string, Entity> myKeyValuePair = (KeyValuePair<string, Entity>)myWindow1.myWindow4.myListView.SelectedItem;
                dict_Parent.Remove(myKeyValuePair.Key);
                ent_Parent.Set<IDictionary<string, Entity>>("FurnLocations_Index", dict_Parent, DisplayUnitType.DUT_MILLIMETERS);

                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Delete Arrangement");

                    myDatastorage.SetEntity(ent_Parent);

                    tx.Commit();
                }

                myWindow1.myWindow4.myListView.ItemsSource = dict_Parent;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE03_Part2_DeleteOne" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE03_Part3_ClearValues : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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

                    IDictionary<string, Entity> dict_Parent = ent_Parent.Get<IDictionary<string, Entity>>("FurnLocations_Index", DisplayUnitType.DUT_MILLIMETERS);

                    dict_Parent.Clear();

                    ent_Parent.Set<IDictionary<string, Entity>>("FurnLocations_Index", dict_Parent, DisplayUnitType.DUT_MILLIMETERS);

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
