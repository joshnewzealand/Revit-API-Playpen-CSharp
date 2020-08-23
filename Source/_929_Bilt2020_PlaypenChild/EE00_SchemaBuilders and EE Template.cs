using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

using Autodesk.Revit.DB.ExtensibleStorage;


namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE01_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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


    class Schema_FurnLocations
    {

        public const string myConstantStringSchema_FurnLocations_Index = "3e2b5963-de35-4d50-9284-cd3154f202fa";
        public const string myConstantStringSchema_FurnLocations = "330a1ede-d77b-4350-963d-3505f7ae5e23";

        public static Schema createSchema_FurnLocations_Index()
        {
            SchemaBuilder mySchemaBuilder = new SchemaBuilder(new Guid(myConstantStringSchema_FurnLocations_Index));
            mySchemaBuilder.SetSchemaName("FurnLocations_Index");
            FieldBuilder mapField_Parent = mySchemaBuilder.AddMapField("FurnLocations_Index", typeof(string), typeof(Entity));
            mapField_Parent.SetSubSchemaGUID(new Guid(myConstantStringSchema_FurnLocations));

            return mySchemaBuilder.Finish();
        }


        public static Schema createSchema_FurnLocations()
        {
            Guid myGUID = new Guid(myConstantStringSchema_FurnLocations);
            SchemaBuilder mySchemaBuilder = new SchemaBuilder(myGUID);
            mySchemaBuilder.SetSchemaName("FurnLocations");

            FieldBuilder mapField_Child = mySchemaBuilder.AddMapField("FurnLocations", typeof(ElementId), typeof(XYZ));
            mapField_Child.SetUnitType(UnitType.UT_Length);

            FieldBuilder mapField_Child_Angle = mySchemaBuilder.AddMapField("FurnLocations_Angle", typeof(ElementId), typeof(double));
            mapField_Child_Angle.SetUnitType(UnitType.UT_Length);
            //IList<int> list = new List<int>() { 111, 222, 333 };


            FieldBuilder mapField_Child_Pattern = mySchemaBuilder.AddMapField("FurnLocations_Pattern", typeof(ElementId), typeof(ElementId));
            FieldBuilder mapField_Child_Red = mySchemaBuilder.AddMapField("FurnLocations_ColorRed", typeof(ElementId), typeof(int));
            FieldBuilder mapField_Child_Green = mySchemaBuilder.AddMapField("FurnLocations_ColorGreen", typeof(ElementId), typeof(int));
            FieldBuilder mapField_Child_Blue = mySchemaBuilder.AddMapField("FurnLocations_ColorBlue", typeof(ElementId), typeof(int));


            return mySchemaBuilder.Finish();
        }

    }
}
