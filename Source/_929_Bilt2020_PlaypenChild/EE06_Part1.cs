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
using System.Windows.Media.Media3D;
using Numerics = System.Numerics;
using std = System.Math;
using _952_PRLoogleClassLibrary;
using System.Text.RegularExpressions;



namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE06_Part1_Template : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
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
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_Part1_Template" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE06_Part1_FamilyManager : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }
        public FamilyInstance myFamilyInstance;

        class FamilyOption : IFamilyLoadOptions
        {
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }

            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
                return true;
            }
        }

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                string myString_FamilyBackups_FamilyName = "";
                Family myFamily = null;
                string myString_FamilyName = "";

                if (true) //candidate for methodisation 202004251537
                {
                    if (uidoc.Selection.GetElementIds().Count != 1)
                    {
                        MessageBox.Show("Please select just one element instance.");
                        return;
                    }

                    if (doc.GetElement(uidoc.Selection.GetElementIds().First()).GetType() != typeof(FamilyInstance))
                    {
                        MessageBox.Show("Please select an element of type - 'FamilyInstance'.");
                        return;
                    }

                    myFamilyInstance = doc.GetElement(uidoc.Selection.GetElementIds().First()) as FamilyInstance;

                    myFamily = ((FamilySymbol)doc.GetElement(myFamilyInstance.GetTypeId())).Family;

                    if (!myFamily.IsEditable)
                    {
                        MessageBox.Show("Family is not editable.");
                        return;
                    }

                    string myString_FamilyBackups = System.IO.Path.GetDirectoryName(doc.PathName) + "\\Family Backups";

                    if (!System.IO.Directory.Exists(myString_FamilyBackups))
                    {
                        System.IO.Directory.CreateDirectory(myString_FamilyBackups);
                    }

                    myString_FamilyName = myFamily.Name;
                    string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());

                    foreach (char c in invalid)
                    {
                        myString_FamilyName = myString_FamilyName.Replace(c.ToString(), "");
                    }

                    myString_FamilyBackups_FamilyName = myString_FamilyBackups + "\\" + myString_FamilyName;

                    if (!System.IO.Directory.Exists(myString_FamilyBackups_FamilyName))
                    {
                        System.IO.Directory.CreateDirectory(myString_FamilyBackups_FamilyName);
                    }
                }

                string myString_FamilyBackups_FamilyName_CurrentRevision = "";
                int myInt_StreamReader = 0;

                if (true) //candidate for methodisation 202004251614
                {
                    myString_FamilyBackups_FamilyName_CurrentRevision = myString_FamilyBackups_FamilyName + "\\Current Revision - do not edit manually.txt";

                    if (!System.IO.File.Exists(myString_FamilyBackups_FamilyName_CurrentRevision))
                    {
                        System.IO.StreamWriter myStreamWriter_FirstZero = new System.IO.StreamWriter(myString_FamilyBackups_FamilyName_CurrentRevision);
                        myStreamWriter_FirstZero.Write(((int)0).ToString());
                        myStreamWriter_FirstZero.Close();
                    }
                    System.IO.File.SetAttributes(myString_FamilyBackups_FamilyName_CurrentRevision, System.IO.File.GetAttributes(myString_FamilyBackups_FamilyName_CurrentRevision) & ~System.IO.FileAttributes.ReadOnly);

                    System.IO.StreamReader myStreamReader = new System.IO.StreamReader(myString_FamilyBackups_FamilyName_CurrentRevision);
                    string myString_StreamReader = myStreamReader.ReadLine();
                    myStreamReader.Close();

                    myInt_StreamReader = int.Parse(myString_StreamReader); myInt_StreamReader++;
                }


                System.IO.StreamWriter myStreamWriter = new System.IO.StreamWriter(myString_FamilyBackups_FamilyName_CurrentRevision);
                myStreamWriter.Write((myInt_StreamReader).ToString());
                myStreamWriter.Close();

                System.IO.StreamReader myStreamReader_CheckPass = new System.IO.StreamReader(myString_FamilyBackups_FamilyName_CurrentRevision);

                //double.Parse(fields[0], CultureInfo.InvariantCulture)
                string myString_TheRevision = myStreamReader_CheckPass.ReadLine();

                myStreamReader_CheckPass.Close();
                System.IO.File.SetAttributes(myString_FamilyBackups_FamilyName_CurrentRevision, System.IO.FileAttributes.ReadOnly);

                //string myString_Directory01 = myString_FamilyFileDirectory + "\\" + doc.Application.VersionNumber;
                //string myString_Directory02 = myString_Directory01 + "\\Previous Versions";
                string myString_Directory03 = myString_TheRevision.PadLeft(4, '0');
                //MessageBox.Show(myString_Directory03);

                string myString_FamilyBackups_FamilyName_TheRevision = myString_FamilyBackups_FamilyName + "\\" + myString_Directory03;

                if (!System.IO.Directory.Exists(myString_FamilyBackups_FamilyName_TheRevision))
                {
                    System.IO.Directory.CreateDirectory(myString_FamilyBackups_FamilyName_TheRevision);
                }

                System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(myString_FamilyBackups_FamilyName_TheRevision + "\\"));
                System.IO.File.Create(myString_FamilyBackups_FamilyName_TheRevision + "\\Restore instructions.txt").Dispose();
                //make the folders, make the folders, make the folders with the zeros, s

                System.IO.StreamWriter objWriter = new System.IO.StreamWriter(myString_FamilyBackups_FamilyName_TheRevision + "\\Restore instructions.txt", true);
                objWriter.WriteLine("To Restore: " + Environment.NewLine + "1. Open this *.rfa file in Revit." + Environment.NewLine + "2. Check it is the revision you wish to restore." + Environment.NewLine + "3. Load into Project and Close." + Environment.NewLine + Environment.NewLine + "Note: This system only increments the revision - it never 'rolls back' and it never deletes." + Environment.NewLine + Environment.NewLine + "4. THEREFORE -> Click SAVE 'FOREVER' BACKUP again, after step 3, to make sure it becomes the new revision.");
                objWriter.Close();

                //copy file in root, then in subdirectory

                Document famDoc = null;
                famDoc = doc.EditFamily(myFamily);

                if (true)
                {
                    if (famDoc.FamilyManager.Parameters.Cast<FamilyParameter>().Where(x => x.Definition.Name == "Family Revision").Count() == 0)
                    {
                        using (Transaction tx = new Transaction(famDoc))
                        {
                            tx.Start("Add Family Revision parameters");

                            famDoc.FamilyManager.AddParameter("Family Revision", BuiltInParameterGroup.INVALID, ParameterType.Integer, false); // true = instance parameter
                            myFamily = famDoc.LoadFamily(doc, new FamilyOption());

                            tx.Commit();
                        }
                    }

                    //famDoc.Close(false);

                    FamilySymbol myFamilySymbol = doc.GetElement(myFamily.GetFamilySymbolIds().First()) as FamilySymbol;

                    using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Save revision");
                        foreach (ElementId myElementID in myFamily.GetFamilySymbolIds())
                        {
                            myFamilySymbol.GetParameters("Family Revision")[0].Set(myInt_StreamReader); //we're gonna replace myFamilyType after after checking we can't remove it altogether
                        }
                        tx.Commit();

                        //MessageBox.Show("Revsion " + myInt_StreamReader.ToString() + " saved.");
                    }
                }

                //famDoc.SaveAs(myString_FamilyBackups_FamilyName + "\\" + myString_FamilyName + ".rfa", new SaveAsOptions() { OverwriteExistingFile = true });
                famDoc.SaveAs(myString_FamilyBackups_FamilyName_TheRevision + "\\" + myString_FamilyName + ".rfa", new SaveAsOptions() { OverwriteExistingFile = true });
                famDoc.Close(false);
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_Part1_FamilyManager" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE06_Part1_FrameInAnimation : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }

        private async Task myPrivateAsync_SetFamilyOnCarrier(Document doc)
        {
            await System.Threading.Tasks.Task.Delay(200);
        }

        public static void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }


        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document; // myListView_ALL_Fam_Master.Items.Add(doc.GetElement(uidoc.Selection.GetElementIds().First()).Name);

                ReferencePoint myReferencePoint = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myWindow1.myCarrierCarrier).First()) as ReferencePoint;

                Transform trans_Immediate = myReferencePoint.GetCoordinateSystem();

                Transform tf_0105 = Transform.Identity;   //top   //candidate for methodisation  202001062020
                tf_0105.Origin = trans_Immediate.Origin;
                tf_0105.BasisX = new XYZ(1.0, 0.0, 0.0);
                tf_0105.BasisY = new XYZ(0.0, 1.0, 0.0);
                tf_0105.BasisZ = new XYZ(0.0, 0.0, 1.0);

                Transform tf_2153 = Transform.Identity;  //front
                tf_2153.Origin = trans_Immediate.Origin;
                tf_2153.BasisX = new XYZ(0.0, 1.0, 0.0);
                tf_2153.BasisY = new XYZ(0.0, 0.0, 1.0);
                tf_2153.BasisZ = new XYZ(1.0, 0.0, 0.0);

                Transform tf_2154 = myReferencePoint.GetCoordinateSystem();  //front
                tf_2154.BasisX = new XYZ(0.0, 1.0, 0.0);
                tf_2154.BasisY = new XYZ(0.0, 0.0, 1.0);
                tf_2154.BasisZ = new XYZ(1.0, 0.0, 0.0);


                int myIntTimeOut = 0;
                int myInt_ChangeCount = 0;
                double myDouble_ChangePosition = -1;
               // bool myBool_Toggle = true;

                using (TransactionGroup transGroup = new TransactionGroup(doc))
                {
                    transGroup.Start("Transform animation");

                    while (myWindow1.mySlideInProgress)
                    {
                        wait(100); myIntTimeOut++;

                        if (myDouble_ChangePosition != myWindow1.mySliderNewRotate_X.Value)
                        {
                            myDouble_ChangePosition = myWindow1.mySliderNewRotate_X.Value;
                            myWindow1.myLabel_ChangeCount.Content = myInt_ChangeCount++.ToString();
                            using (Transaction y = new Transaction(doc, "a Transform"))
                            {
                                y.Start();

                                myReferencePoint.SetCoordinateSystem(myWindow1.myListTransform[(int)myDouble_ChangePosition]);

                                y.Commit();
                            }
                        }

                        myWindow1.myLabel_Setting.Content = myWindow1.mySliderNewRotate_X.Value.ToString();

                        if (myIntTimeOut == 1000)
                        {
                            MessageBox.Show("Timeout");
                            break;
                        }
                    }

                    transGroup.Assimilate();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE06_Part1_FrameInAnimation" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE06_Part1_Zero : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        public Window1 myWindow1 { get; set; }
        //public ReferencePoint myReferencePoint { get; set; }

        public bool myClockwise { get; set; } = true;
        public bool myBool_X_Proceed { get; set; } = true;
        public bool myBool_Y_Proceed { get; set; } = true;
        public bool myBool_Z_Proceed { get; set; } = true;
        public bool myBool_UseNinety { get; set; } = false;
        public bool myBool_PerformTheZero { get; set; } = false;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                using (Transaction y = new Transaction(doc, "Reset to Zero"))
                {
                    y.Start();

                    ReferencePoint myReferencePoint = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myWindow1.myCarrierCarrier).First()) as ReferencePoint;

                    Transform trans_Immediate = myReferencePoint.GetCoordinateSystem();

                    Transform tf_0105 = Transform.Identity;   //top   //candidate for methodisation  202001062020
                    tf_0105.Origin = trans_Immediate.Origin;
                    tf_0105.BasisX = new XYZ(1.0, 0.0, 0.0);
                    tf_0105.BasisY = new XYZ(0.0, 1.0, 0.0);
                    tf_0105.BasisZ = new XYZ(0.0, 0.0, 1.0);

                    Transform tf_2153 = Transform.Identity;  //front
                    tf_2153.Origin = trans_Immediate.Origin;
                    tf_2153.BasisX = new XYZ(0.0, 1.0, 0.0);
                    tf_2153.BasisY = new XYZ(0.0, 0.0, 1.0);
                    tf_2153.BasisZ = new XYZ(1.0, 0.0, 0.0);

                    Transform tf_0106 = Transform.Identity;  //right
                    tf_0106.Origin = trans_Immediate.Origin;
                    tf_0106.BasisX = new XYZ(0.0, 0.0, 1.0);
                    tf_0106.BasisY = new XYZ(1.0, 0.0, 0.0);
                    tf_0106.BasisZ = new XYZ(0.0, 1.0, 0.0);

                    if (myBool_X_Proceed)
                    {
                        trans_Immediate = myReferencePoint.GetCoordinateSystem();
                        Line myLine1205 = Line.CreateUnbound(trans_Immediate.Origin, trans_Immediate.BasisZ);
                        if (!myBool_UseNinety) myReferencePoint.SetCoordinateSystem(tf_0105);
                        if (myBool_UseNinety) myReferencePoint.Location.Rotate(myLine1205, (myClockwise ? 1 : -1) * Math.PI / 4);
                    }

                    if (myBool_Y_Proceed)
                    {
                        trans_Immediate = myReferencePoint.GetCoordinateSystem();
                        Line myLine2058 = Line.CreateUnbound(trans_Immediate.Origin, trans_Immediate.BasisX);
                        if (!myBool_UseNinety) myReferencePoint.SetCoordinateSystem(tf_2153);
                        if (myBool_UseNinety) myReferencePoint.Location.Rotate(myLine2058, (myClockwise ? 1 : -1) * Math.PI / 4);
                    }

                    if (myBool_Z_Proceed)
                    {
                        trans_Immediate = myReferencePoint.GetCoordinateSystem();
                        Line myLine2059 = Line.CreateUnbound(trans_Immediate.Origin, trans_Immediate.BasisY);
                        if (!myBool_UseNinety) myReferencePoint.SetCoordinateSystem(tf_0106);
                        if (myBool_UseNinety) myReferencePoint.Location.Rotate(myLine2059, (myClockwise ? 1 : -1) * Math.PI / 4);
                    }

                    trans_Immediate = myReferencePoint.GetCoordinateSystem();

                    doc.Regenerate();

                    /////////////////////////////////////////////Window2.thePublicStatic(trans_Immediate, myWindow2);

                    y.Commit();
                }
                myBool_X_Proceed = true;
                myBool_Y_Proceed = true;
                myBool_Z_Proceed = true;
                myBool_UseNinety = false;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("_934_PRLoogle_Command06_EE01_Zero" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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
    public class EE06_Part1_Zero_Special : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        internal static void ToMatrix(Quaternion quaternion, out Transform matrix)
        {
            matrix = Transform.Identity;

            // source -> http://content.gpwiki.org/index.php/OpenGL:Tutorials:Using_Quaternions_to_represent_rotation#Quaternion_to_Matrix
            double x2 = quaternion.X * quaternion.X;
            double y2 = quaternion.Y * quaternion.Y;
            double z2 = quaternion.Z * quaternion.Z;
            double xy = quaternion.X * quaternion.Y;
            double xz = quaternion.X * quaternion.Z;
            double yz = quaternion.Y * quaternion.Z;
            double wx = quaternion.W * quaternion.X;
            double wy = quaternion.W * quaternion.Y;
            double wz = quaternion.W * quaternion.Z;

            // This calculation would be a lot more complicated for non-unit length quaternions
            // Note: The constructor of Matrix4 expects the Matrix in column-major format like 
            // expected by OpenGL
            // Revit has a 3x3 matrix and vector objects for its columns
            // so Matrix.M1? would be Transform.BasisX in Revit
            // matrix.M11 = 1.0f - 2.0f * (y2 + z2);
            // matrix.M12 = 2.0f * (xy - wz);
            // matrix.M13 = 2.0f * (xz + wy);
            // matrix.M14 = 0.0f;
            XYZ xvec = new XYZ(1.0f - 2.0f * (y2 + z2), 2.0f * (xy - wz), 2.0f * (xz + wy));

            // matrix.M21 = 2.0f * (xy + wz);
            // matrix.M22 = 1.0f - 2.0f * (x2 + z2);
            // matrix.M23 = 2.0f * (yz - wx);
            // matrix.M24 = 0.0f;
            XYZ yvec = new XYZ(2.0f * (xy + wz), 1.0f - 2.0f * (x2 + z2), 2.0f * (yz - wx));

            // matrix.M31 = 2.0f * (xz - wy);
            // matrix.M32 = 2.0f * (yz + wx);
            // matrix.M33 = 1.0f - 2.0f * (x2 + y2);
            // matrix.M34 = 0.0f;
            XYZ zvec = new XYZ(2.0f * (xz - wy), 2.0f * (yz + wx), 1.0f - 2.0f * (x2 + y2));

            // the big question now is if we can safely omit this or if this
            // will break down the calculations. This column would be the translation
            // part of the 4x4 matrix. Note also that the next single return solution
            // has this row as the standard (0,0,0,1) that you would expect from a 4x4
            // matrix.M41 = 2.0f * (xz - wy);
            // matrix.M42 = 2.0f * (yz + wx);
            // matrix.M43 = 1.0f - 2.0f * (x2 + y2);
            // matrix.M44 = 0.0f;

            // return Matrix4( 1.0f - 2.0f * (y2 + z2), 2.0f * (xy - wz), 2.0f * (xz + wy), 0.0f,
            // 2.0f * (xy + wz), 1.0f - 2.0f * (x2 + z2), 2.0f * (yz - wx), 0.0f,
            // 2.0f * (xz - wy), 2.0f * (yz + wx), 1.0f - 2.0f * (x2 + y2), 0.0f,
            // 0.0f, 0.0f, 0.0f, 1.0f)

            matrix.BasisX = xvec;
            matrix.BasisY = yvec;
            matrix.BasisZ = zvec;
        }



        public partial class aIntegerGreatest
        {
            public double myDoubleAngle;
            public int myIntCountToThree;

        }

        public static bool IsEqual(double a, double b)
        {
            return IsZero(b - a);
        }

        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }
        const double _eps = 1.0e-9;

        public static bool IsZero(double a)
        {
            return IsZero(a, _eps);
        }

        public partial class EulerAngles
        {
            public double roll, pitch, yaw;
        };

        public Window1 myWindow1 { get; set; }
        //public ReferencePoint myReferencePoint { get; set; }

        public bool myClockwise { get; set; } = true;
        public bool myBool_X_Proceed { get; set; } = true;
        public bool myBool_Y_Proceed { get; set; } = true;
        public bool myBool_Z_Proceed { get; set; } = true;
        public bool myBool_UseNinety { get; set; } = false;
        public bool myBool_PerformTheZero { get; set; } = false;

        public void Execute(UIApplication uiapp)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                using (Transaction y = new Transaction(doc, "Reset to Zero"))
                {
                    y.Start();

                    ReferencePoint myReferencePoint = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myWindow1.myCarrierCarrier).First()) as ReferencePoint;

                    Transform trans_Immediate = myReferencePoint.GetCoordinateSystem();

                    Transform tf_0105 = Transform.Identity;   //top   //candidate for methodisation  202001062020
                    tf_0105.Origin = trans_Immediate.Origin;
                    tf_0105.BasisX = new XYZ(1.0, 0.0, 0.0);
                    tf_0105.BasisY = new XYZ(0.0, 1.0, 0.0);
                    tf_0105.BasisZ = new XYZ(0.0, 0.0, 1.0);

                    Transform tf_2153 = Transform.Identity;  //front
                    tf_2153.Origin = trans_Immediate.Origin;
                    tf_2153.BasisX = new XYZ(0.0, 1.0, 0.0);
                    tf_2153.BasisY = new XYZ(0.0, 0.0, 1.0);
                    tf_2153.BasisZ = new XYZ(1.0, 0.0, 0.0);

                    FamilyInstance myFamilyInstance_1533 = doc.GetElement(new ElementId(575579)) as FamilyInstance;
                    ReferencePoint myReferencePoint2 = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_1533).First()) as ReferencePoint;
                    Transform myTransform_FakeBasis = myReferencePoint2.GetCoordinateSystem();

                    FamilyInstance myFamilyInstance_2252 = doc.GetElement(new ElementId(577125)) as FamilyInstance;
                    ReferencePoint myReferencePoint3 = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_2252).First()) as ReferencePoint;
                    Transform myTransform_FakeBasis2 = myReferencePoint3.GetCoordinateSystem();

                    Quaternion myQuaternion_FakeBasis_X = new Quaternion(myTransform_FakeBasis.BasisX.X, myTransform_FakeBasis.BasisX.Y, myTransform_FakeBasis.BasisX.Z, 0.0);
                    Quaternion myQuaternion_FakeBasis_Y = new Quaternion(myTransform_FakeBasis.BasisY.X, myTransform_FakeBasis.BasisY.Y, myTransform_FakeBasis.BasisY.Z, 0.0);
                    Quaternion myQuaternion_FakeBasis_Z = new Quaternion(myTransform_FakeBasis.BasisZ.X, myTransform_FakeBasis.BasisZ.Y, myTransform_FakeBasis.BasisZ.Z, 0.0);

                    Quaternion myQuaternion_Immediate_X = new Quaternion(myTransform_FakeBasis2.BasisX.X, myTransform_FakeBasis2.BasisX.Y, myTransform_FakeBasis2.BasisX.Z, 0.0);
                    Quaternion myQuaternion_Immediate_Y = new Quaternion(myTransform_FakeBasis2.BasisY.X, myTransform_FakeBasis2.BasisY.Y, myTransform_FakeBasis2.BasisY.Z, 0.0);
                    Quaternion myQuaternion_Immediate_Z = new Quaternion(myTransform_FakeBasis2.BasisZ.X, myTransform_FakeBasis2.BasisZ.Y, myTransform_FakeBasis2.BasisZ.Z, 0.0);
                 
                    XYZ myXYZ_Vector2 = myTransform_FakeBasis.OfVector(new XYZ(0, 0, 1));

                    Quaternion myQuaternion = Quaternion.Slerp(myQuaternion_FakeBasis_Z, myQuaternion_Immediate_Z, 0.5);

                    QuaternionRotation3D myQuaternionRotation3D = new QuaternionRotation3D(myQuaternion);

                    RotateTransform3D myRotateTransform3D = new RotateTransform3D();

                    myRotateTransform3D.Rotation = myQuaternionRotation3D;

                    Matrix3D myMatrix3D_2 = new Matrix3D();

                    myMatrix3D_2.M11 = myTransform_FakeBasis2.BasisX.X;
                    myMatrix3D_2.M12 = myTransform_FakeBasis2.BasisX.Y;
                    myMatrix3D_2.M13 = myTransform_FakeBasis2.BasisX.Z;

                    myMatrix3D_2.M21 = myTransform_FakeBasis2.BasisY.X;
                    myMatrix3D_2.M22 = myTransform_FakeBasis2.BasisY.Y;
                    myMatrix3D_2.M23 = myTransform_FakeBasis2.BasisY.Z;

                    myMatrix3D_2.M31 = myTransform_FakeBasis2.BasisZ.X;
                    myMatrix3D_2.M32 = myTransform_FakeBasis2.BasisZ.Y;
                    myMatrix3D_2.M33 = myTransform_FakeBasis2.BasisZ.Z;

                    // Vector3D myVector3D_2 = new Vector3D(myFamilyInstance_1533.FacingOrientation.X, myFamilyInstance_1533.FacingOrientation.Y, myFamilyInstance_1533.FacingOrientation.Z);
                    Vector3D myVector3D_2 = new Vector3D();
                    myVector3D_2.X = myMatrix3D_2.M11 + myMatrix3D_2.M12 + myMatrix3D_2.M13;
                    myVector3D_2.Y = myMatrix3D_2.M21 + myMatrix3D_2.M22 + myMatrix3D_2.M23;
                    myVector3D_2.Z = myMatrix3D_2.M31 + myMatrix3D_2.M32 + myMatrix3D_2.M33;

                    Numerics.Matrix4x4 myMatrix4x4_2 = new Numerics.Matrix4x4();
                    myMatrix4x4_2.M11 = (float)myTransform_FakeBasis2.BasisX.X;
                    myMatrix4x4_2.M12 = (float)myTransform_FakeBasis2.BasisX.Y;
                    myMatrix4x4_2.M13 = (float)myTransform_FakeBasis2.BasisX.Z;

                    myMatrix4x4_2.M21 = (float)myTransform_FakeBasis2.BasisY.X;
                    myMatrix4x4_2.M22 = (float)myTransform_FakeBasis2.BasisY.Y;
                    myMatrix4x4_2.M23 = (float)myTransform_FakeBasis2.BasisY.Z;

                    myMatrix4x4_2.M31 = (float)myTransform_FakeBasis2.BasisZ.X;
                    myMatrix4x4_2.M32 = (float)myTransform_FakeBasis2.BasisZ.Y;
                    myMatrix4x4_2.M33 = (float)myTransform_FakeBasis2.BasisZ.Z;

                    Matrix3D myMatrix3D_3 = new Matrix3D();

                    myMatrix3D_3.M11 = myTransform_FakeBasis.BasisX.X;
                    myMatrix3D_3.M12 = myTransform_FakeBasis.BasisX.Y;
                    myMatrix3D_3.M13 = myTransform_FakeBasis.BasisX.Z;

                    myMatrix3D_3.M21 = myTransform_FakeBasis.BasisY.X;
                    myMatrix3D_3.M22 = myTransform_FakeBasis.BasisY.Y;
                    myMatrix3D_3.M23 = myTransform_FakeBasis.BasisY.Z;

                    myMatrix3D_3.M31 = myTransform_FakeBasis.BasisZ.X;
                    myMatrix3D_3.M32 = myTransform_FakeBasis.BasisZ.Y;
                    myMatrix3D_3.M33 = myTransform_FakeBasis.BasisZ.Z;

                    //Vector3D myVector3D_3 = new Vector3D(myFamilyInstance_2252.FacingOrientation.X, myFamilyInstance_2252.FacingOrientation.Y, myFamilyInstance_2252.FacingOrientation.Z);
                    Vector3D myVector3D_3 = new Vector3D();
                    myVector3D_3.X = myMatrix3D_3.M11 + myMatrix3D_3.M12 + myMatrix3D_3.M13;
                    myVector3D_3.Y = myMatrix3D_3.M21 + myMatrix3D_3.M22 + myMatrix3D_3.M23;
                    myVector3D_3.Z = myMatrix3D_3.M31 + myMatrix3D_3.M32 + myMatrix3D_3.M33;

                    Numerics.Matrix4x4 myMatrix4x4_3 = new Numerics.Matrix4x4();
                    myMatrix4x4_3.M11 = (float)myTransform_FakeBasis.BasisX.X;
                    myMatrix4x4_3.M12 = (float)myTransform_FakeBasis.BasisX.Y;
                    myMatrix4x4_3.M13 = (float)myTransform_FakeBasis.BasisX.Z;

                    myMatrix4x4_3.M21 = (float)myTransform_FakeBasis.BasisY.X;
                    myMatrix4x4_3.M22 = (float)myTransform_FakeBasis.BasisY.Y;
                    myMatrix4x4_3.M23 = (float)myTransform_FakeBasis.BasisY.Z;

                    myMatrix4x4_3.M31 = (float)myTransform_FakeBasis.BasisZ.X;
                    myMatrix4x4_3.M32 = (float)myTransform_FakeBasis.BasisZ.Y;
                    myMatrix4x4_3.M33 = (float)myTransform_FakeBasis.BasisZ.Z;

                    Numerics.Matrix4x4 myMatrix4x4_4 = new Numerics.Matrix4x4();
                    myMatrix4x4_4 = Numerics.Matrix4x4.Lerp(myMatrix4x4_2, myMatrix4x4_3, (float)0.5);

                    Quaternion myQuaternion_2 = new Quaternion(myVector3D_2, 0);
                    Quaternion myQuaternion_3 = new Quaternion(myVector3D_3, 0);

                    Quaternion myQuaternion_4 = Quaternion.Slerp(myQuaternion_2, myQuaternion_3, 0.5);
                    //myRotateTransform3D.Transform()
          
                    Numerics.Quaternion myNumericsQuaternionRotation_2 = Numerics.Quaternion.CreateFromRotationMatrix(myMatrix4x4_2);
                    Numerics.Quaternion myNumericsQuaternionRotation_3 = Numerics.Quaternion.CreateFromRotationMatrix(myMatrix4x4_3);

                    Numerics.Vector3 mymymyNumericsBasisX = new Numerics.Vector3();
                    Numerics.Vector3 mymymyNumericsBasisY = new Numerics.Vector3();
                    Numerics.Vector3 mymymyNumericsBasisZ = new Numerics.Vector3();
                    Numerics.Vector3 mymymyNumericsBasisZ_Baseline = new Numerics.Vector3();

                    List<float> myListfloat  = new List<float>() { (float)0.1,  (float)0.2, (float)0.3, (float)0.4, (float)0.5, (float)0.6, (float)0.7, (float)0.8, (float)0.9};

                    foreach (float myFloat in myListfloat)
                    {
                        if (true)
                        {
                            Numerics.Vector3 NumericsVector3_2 = new Numerics.Vector3();
                            NumericsVector3_2.X = (float)myTransform_FakeBasis.BasisX.X;
                            NumericsVector3_2.Y = (float)myTransform_FakeBasis.BasisX.Y;
                            NumericsVector3_2.Z = (float)myTransform_FakeBasis.BasisX.Z;
                            NumericsVector3_2 = Numerics.Vector3.Normalize(NumericsVector3_2);

                            Numerics.Vector3 NumericsVector3_3 = new Numerics.Vector3();
                            NumericsVector3_3.X = (float)myTransform_FakeBasis2.BasisX.X;
                            NumericsVector3_3.Y = (float)myTransform_FakeBasis2.BasisX.Y;
                            NumericsVector3_3.Z = (float)myTransform_FakeBasis2.BasisX.Z;
                            NumericsVector3_3 = Numerics.Vector3.Normalize(NumericsVector3_3);

                            mymymyNumericsBasisX = Numerics.Vector3.Lerp(NumericsVector3_2, NumericsVector3_3, myFloat);
                            mymymyNumericsBasisX = Numerics.Vector3.Normalize(mymymyNumericsBasisX);
                        }

                        if (true)
                        {
                            Numerics.Vector3 NumericsVector3_2 = new Numerics.Vector3();
                            NumericsVector3_2.X = (float)myTransform_FakeBasis.BasisY.X;
                            NumericsVector3_2.Y = (float)myTransform_FakeBasis.BasisY.Y;
                            NumericsVector3_2.Z = (float)myTransform_FakeBasis.BasisY.Z;
                            NumericsVector3_2 = Numerics.Vector3.Normalize(NumericsVector3_2);

                            Numerics.Vector3 NumericsVector3_3 = new Numerics.Vector3();
                            NumericsVector3_3.X = (float)myTransform_FakeBasis2.BasisY.X;
                            NumericsVector3_3.Y = (float)myTransform_FakeBasis2.BasisY.Y;
                            NumericsVector3_3.Z = (float)myTransform_FakeBasis2.BasisY.Z;
                            NumericsVector3_3 = Numerics.Vector3.Normalize(NumericsVector3_3);

                            mymymyNumericsBasisY = Numerics.Vector3.Lerp(NumericsVector3_2, NumericsVector3_3, myFloat);
                            mymymyNumericsBasisY = Numerics.Vector3.Normalize(mymymyNumericsBasisY);
                        }

                        if (true)
                        {
                            Numerics.Vector3 NumericsVector3_2 = new Numerics.Vector3();
                            NumericsVector3_2.X = (float)myTransform_FakeBasis.BasisZ.X;
                            NumericsVector3_2.Y = (float)myTransform_FakeBasis.BasisZ.Y;
                            NumericsVector3_2.Z = (float)myTransform_FakeBasis.BasisZ.Z;
                            NumericsVector3_2 = Numerics.Vector3.Normalize(NumericsVector3_2);

                            Numerics.Vector3 NumericsVector3_3 = new Numerics.Vector3();
                            NumericsVector3_3.X = (float)myTransform_FakeBasis2.BasisZ.X;
                            NumericsVector3_3.Y = (float)myTransform_FakeBasis2.BasisZ.Y;
                            NumericsVector3_3.Z = (float)myTransform_FakeBasis2.BasisZ.Z;
                            NumericsVector3_3 = Numerics.Vector3.Normalize(NumericsVector3_3);

                            mymymyNumericsBasisZ = Numerics.Vector3.Lerp(NumericsVector3_2, NumericsVector3_3, myFloat);
                            mymymyNumericsBasisZ = Numerics.Vector3.Normalize(mymymyNumericsBasisZ);

                            mymymyNumericsBasisZ_Baseline = Numerics.Vector3.Lerp(NumericsVector3_2, NumericsVector3_3, 0);
                            mymymyNumericsBasisZ_Baseline = Numerics.Vector3.Normalize(mymymyNumericsBasisZ);
                        }

                        Transform tf_0107_4 = Transform.Identity;  //front

                        tf_0107_4.BasisZ = new XYZ(mymymyNumericsBasisZ.X, mymymyNumericsBasisZ.Y, mymymyNumericsBasisZ.Z);

                        Transform t;
                        if (true)
                        {
                            double a = XYZ.BasisZ.AngleTo(tf_0107_4.BasisZ);

                            if (IsZero(a))
                            {
                                t = Transform.Identity;
                            }
                            else
                            {
                                XYZ axis = IsEqual(a, Math.PI)
                                  ? XYZ.BasisX
                                  : tf_0107_4.BasisZ.CrossProduct(-XYZ.BasisZ);

                                t = Transform.CreateRotationAtPoint(axis, a, XYZ.Zero);
                            }

                            t.Origin = myReferencePoint.Position/* + new XYZ(0.1,0.1,0.1) */;
                        }

                        Transform tbaseline; 
                        if (true)
                        {
                            double a = XYZ.BasisZ.AngleTo(new XYZ(mymymyNumericsBasisZ_Baseline.X, mymymyNumericsBasisZ_Baseline.Y, mymymyNumericsBasisZ_Baseline.Z));

                            if (IsZero(a))
                            {
                                tbaseline = Transform.Identity;
                            }
                            else
                            {
                                XYZ axis = IsEqual(a, Math.PI)
                                  ? XYZ.BasisX
                                  : new XYZ(mymymyNumericsBasisZ_Baseline.X, mymymyNumericsBasisZ_Baseline.Y, mymymyNumericsBasisZ_Baseline.Z).CrossProduct(-XYZ.BasisZ);

                                tbaseline = Transform.CreateRotationAtPoint(axis, a, XYZ.Zero);
                            }

                            tbaseline.Origin = myTransform_FakeBasis.Origin/* + new XYZ(0.1,0.1,0.1) */;

                            //     myReferencePoint = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myWindow1.myCarrierCarrier).First()) as ReferencePoint;

                            double aaa2000 = -myTransform_FakeBasis.BasisX.AngleTo(tbaseline.BasisX);

                            Transform myT_2000 = Transform.CreateRotation(tbaseline.OfVector(tbaseline.BasisZ), aaa2000 * (1 - myFloat)/*, myReferencePoint.Position*/);

                            t = t.Multiply(myT_2000);
                            t.Origin = myReferencePoint3.Position + ((myReferencePoint2.Position - myReferencePoint3.Position) * (1 - myFloat))/* + new XYZ(0.1,0.1,0.1) */;
                            myReferencePoint.SetCoordinateSystem(t);

                        }

                        doc.Regenerate();
                        uidoc.RefreshActiveView();

                        myTransform_FakeBasis.Origin = t.Origin;
                        myTransform_FakeBasis2.Origin = t.Origin;

                        doc.Regenerate();
                        uidoc.RefreshActiveView();
                        System.Threading.Thread.Sleep(200);
                    }


                    /////////////////////////////////////////////Window2.thePublicStatic(trans_Immediate, myWindow2);

                    y.Commit();
                }
                myBool_X_Proceed = true;
                myBool_Y_Proceed = true;
                myBool_Z_Proceed = true;
                myBool_UseNinety = false;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("_934_PRLoogle_Command06_EE01_Zero" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
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

