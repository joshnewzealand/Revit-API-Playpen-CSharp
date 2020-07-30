using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

using Autodesk.Revit.DB.ExtensibleStorage;


namespace _929_Bilt2020_PlaypenChild
{
    class Schema_FurnLocations
    {

        public const string myConstantStringSchema_FurnLocations_Index = "d4afae09-95b2-47d1-a966-468995d06cf9";
        public const string myConstantStringSchema_FurnLocations = "d001de57-02c8-465b-8cff-6c6af5d86ebf";

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

            return mySchemaBuilder.Finish();
        }

    }
}
