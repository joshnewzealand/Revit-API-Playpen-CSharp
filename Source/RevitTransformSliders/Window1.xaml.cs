/*
 * Created by SharpDevelop.
 * User: Joshua.Lumley
 * Date: 28/04/2019
 * Time: 5:59 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Linq;

using Numerics = System.Numerics;
using Transform = Autodesk.Revit.DB.Transform;
using Binding = Autodesk.Revit.DB.Binding;

namespace RevitTransformSliders
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 
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


    public partial class Window1 : Window
    {
        public Xceed.Wpf.Toolkit.IntegerUpDown myToolKit_IntUpDown { get; set; }
        public Slider mySlider { get; set; }

        // public ReferencePoint myReferencePoint1 { get; set; }
        public List<Transform> myListTransform { get; set; } = new List<Transform>();
        public List<Transform> myListTransform_Interpolate { get; set; } = new List<Transform>();
        public bool mySlideInProgress = false;

        public EE01_Interpolate myEE01_Part1_Interpolate { get; set; }
        public ExternalEvent myExternalEvent_EE01_Part1_Interpolate { get; set; }

        public EE03_RotateAroundBasis myEE03_RotateAroundBasis { get; set; }
        public ExternalEvent myExternalEvent_EE03_RotateAroundBasis { get; set; }

        public EE04_ResetPosition myEE04_ResetPosition { get; set; }
        public ExternalEvent myExternalEvent_EE04_ResetPosition { get; set; }

        public EE05_Move myEE05_Move { get; set; }
        public ExternalEvent myExternalEvent_EE05_Move { get; set; }
        public EE06_LoadFamily myEE06_LoadFamily { get; set; }
        public ExternalEvent myExternalEvent_EE06_LoadFamily { get; set; }

        public EE06_PlaceFamily myEE06_PlaceFamily { get; set; }
        public ExternalEvent myExternalEvent_EE06_PlaceFamily { get; set; }

        public ThisApplication myThisApplication { get; set; }
        public ExternalCommandData commandData { get; set; }

        public Window1(ExternalCommandData cD, ThisApplication tA)
        {
            myThisApplication = tA;
            commandData = cD;

            InitializeComponent();

            // add 'UIDocument uid' as a parameter above, because this is the way it is called form the external event, please see youve 5 Secrets of Revit API Coding for an explaination on this

            this.Top = Properties.Settings.Default.Top;
            this.Left = Properties.Settings.Default.Left;
            this.Height = Properties.Settings.Default.Height;
            this.Width = Properties.Settings.Default.Width;
            myIntUpDown_Middle2.Value = Properties.Settings.Default.LastMiddle;
            myIntUpDown_A.Value = Properties.Settings.Default.LastA;
            myIntUpDown_B.Value = Properties.Settings.Default.LastB;
            //myIntUpDown_Middle2.Value = Properties.Settings.Default.LastRotate;
            //myIntUpDown_Middle2.Value = Properties.Settings.Default.LastMove;

            //setSlider();

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            myEE01_Part1_Interpolate = new EE01_Interpolate();
            myEE01_Part1_Interpolate.myWindow1 = this;
            myExternalEvent_EE01_Part1_Interpolate = ExternalEvent.Create(myEE01_Part1_Interpolate);

            myEE03_RotateAroundBasis = new EE03_RotateAroundBasis();
            myEE03_RotateAroundBasis.myWindow1 = this;
            myExternalEvent_EE03_RotateAroundBasis = ExternalEvent.Create(myEE03_RotateAroundBasis);

            myEE04_ResetPosition = new EE04_ResetPosition();
            myEE04_ResetPosition.myWindow1 = this;
            myExternalEvent_EE04_ResetPosition = ExternalEvent.Create(myEE04_ResetPosition);

            myEE05_Move = new EE05_Move();
            myEE05_Move.myWindow1 = this;
            myExternalEvent_EE05_Move = ExternalEvent.Create(myEE05_Move);

            myEE06_LoadFamily = new EE06_LoadFamily();
            myEE06_LoadFamily.myWindow1 = this;
            myExternalEvent_EE06_LoadFamily = ExternalEvent.Create(myEE06_LoadFamily);

            myEE06_PlaceFamily = new EE06_PlaceFamily();
            myEE06_PlaceFamily.myWindow1 = this;
            myExternalEvent_EE06_PlaceFamily = ExternalEvent.Create(myEE06_PlaceFamily);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.LastMiddle = myIntUpDown_Middle2.Value.Value;
            Properties.Settings.Default.LastA = myIntUpDown_A.Value.Value;
            Properties.Settings.Default.LastB = myIntUpDown_B.Value.Value;
            //Properties.Settings.Default.LastRotate = myIntUpDown_Middle2.Value.Value;
            //Properties.Settings.Default.LastMove = myIntUpDown_Middle2.Value.Value;
            Properties.Settings.Default.Save();
        }

        public bool mySelectMethod(Xceed.Wpf.Toolkit.IntegerUpDown myToolKit_IntUpDown) //myToolKit_IntUpDown
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            while (true)
            {
                if (uidoc.Selection.GetElementIds().Count != 1) break;
                Element myElement = doc.GetElement(uidoc.Selection.GetElementIds().First()) as Element;
                if (myElement.GetType() != typeof(FamilyInstance)) break;
                FamilyInstance myFamilyInstance = myElement as FamilyInstance;
                if (myFamilyInstance.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == "PRL-GM Adaptive Carrier Youtube")
                {
                    myToolKit_IntUpDown.Value = myFamilyInstance.Id.IntegerValue;
                    break;
                }

                if (myFamilyInstance.Host == null) break;
                List<Element> myListElement = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).Where(x => x.Id == myFamilyInstance.Host.Id).ToList();
                if (myListElement.Count != 1) break;
                FamilyInstance myFamilyInstance_2148 = myListElement.First() as FamilyInstance;

                if (myFamilyInstance_2148.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == "PRL-GM Adaptive Carrier Youtube")
                {
                    myToolKit_IntUpDown.Value = myFamilyInstance_2148.Id.IntegerValue;
                    break;
                }
            }
            //i am dead keen to get reference pont closer to the useage of the reference oint
            //
            /*//why was this commented out*/
            if (myToolKit_IntUpDown.Value == null) return false;
            if (myToolKit_IntUpDown.Value.Value == 0) return false;
            if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null) return false;
            return true;
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


        ///             TECHNIQUE 19 OF 19 (Window1.xaml.cs) (scroll down to project 'RevitTransformSliders')
        ///↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ INTERPOLATION ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        ///
        /// Interfaces and ENUM's:
        /// 
        /// 
        /// Demonstrates classes:
        ///     Numerics.Vector3
        /// 
        /// 
        /// Key methods:
        ///     Numerics.Vector3.Lerp(
        ///     
        ///     
        ///     
        /// 


        public void myMethod_whichTook_120Hours_OfCoding()
        {
            myListTransform_Interpolate.Clear();

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FamilyInstance myFamilyInstance_1533 = doc.GetElement(new ElementId(myWindow.myIntUpDown_A.Value.Value)) as FamilyInstance;
            FamilyInstance myFamilyInstance_2252 = doc.GetElement(new ElementId(myWindow.myIntUpDown_B.Value.Value)) as FamilyInstance;

            if (myFamilyInstance_1533 == null) myWindow.myIntUpDown_A.Value = 0;
            if (myFamilyInstance_2252 == null) myWindow.myIntUpDown_B.Value = 0;
            if (myFamilyInstance_2252 == null | myFamilyInstance_1533 == null) return;

            ReferencePoint myReferencePoint2 = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_1533).First()) as ReferencePoint;
            ReferencePoint myReferencePoint3 = doc.GetElement(AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance_2252).First()) as ReferencePoint;

            Transform myTransform_FakeBasis44444 = myReferencePoint2.GetCoordinateSystem();
            Transform myTransform_FakeBasis33333 = myReferencePoint3.GetCoordinateSystem();

            double myDouble_OneDivideTwelve = 1.0 / 24.0;

            List<float> myListfloat = new List<float>();

            myListfloat.Add((float)(myDouble_OneDivideTwelve * 1)); //13
            for (int i = 1; i <= 23; i++)
            {
                myListfloat.Add((float)(myDouble_OneDivideTwelve * i)); //12
            }
            myListfloat.Add((float)(myDouble_OneDivideTwelve * 23)); //13

            myTransform_FakeBasis44444.Origin = XYZ.Zero;
            myTransform_FakeBasis33333.Origin = XYZ.Zero;

            Transform myTransformFromQuatX;
            double aaMega_QuatX = XYZ.BasisZ.AngleTo(myTransform_FakeBasis33333.BasisZ);  //remember this is about vector not basis

            if (IsZero(aaMega_QuatX)) myTransformFromQuatX = Transform.Identity;
            else
            {
                XYZ axis = myTransform_FakeBasis33333.BasisZ.CrossProduct(-XYZ.BasisZ);
                myTransformFromQuatX = Transform.CreateRotationAtPoint(axis, aaMega_QuatX, XYZ.Zero); //step 1, extration
            }

            Transform myTransformFromQuatXX;
            double aaMega_QuatXX = XYZ.BasisZ.AngleTo(myTransform_FakeBasis44444.BasisZ);  //remember this is about vector not basis

            if (IsZero(aaMega_QuatXX)) myTransformFromQuatXX = Transform.Identity;
            else
            {
                XYZ axis = myTransform_FakeBasis44444.BasisZ.CrossProduct(-XYZ.BasisZ);
                myTransformFromQuatXX = Transform.CreateRotationAtPoint(axis, aaMega_QuatXX, XYZ.Zero); //step 1, extration
            }

            Transform myEndPointTransform = Transform.Identity;
            myEndPointTransform.BasisX = myTransformFromQuatXX.Inverse.OfVector(myTransform_FakeBasis44444.BasisX); //step 2, extracting rotation
            myEndPointTransform.BasisY = myTransformFromQuatXX.Inverse.OfVector(myTransform_FakeBasis44444.BasisY); //step 2, extracting rotation
            myEndPointTransform.BasisZ = myTransformFromQuatXX.Inverse.OfVector(myTransform_FakeBasis44444.BasisZ); //step 2, extracting rotation

            Transform myEndPointTransformFor333 = Transform.Identity;
            myEndPointTransformFor333.BasisX = myTransformFromQuatX.Inverse.OfVector(myTransform_FakeBasis33333.BasisX); //step 2, extracting rotation
            myEndPointTransformFor333.BasisY = myTransformFromQuatX.Inverse.OfVector(myTransform_FakeBasis33333.BasisY); //step 2, extracting rotation
            myEndPointTransformFor333.BasisZ = myTransformFromQuatX.Inverse.OfVector(myTransform_FakeBasis33333.BasisZ); //step 2, extracting rotation

            double angle = myEndPointTransform.BasisX.AngleOnPlaneTo(XYZ.BasisX, -XYZ.BasisZ); //step 3, extracting rotation turning into angle
            double d1_Final = myEndPointTransformFor333.BasisX.AngleOnPlaneTo(XYZ.BasisX, -XYZ.BasisZ); //step 3, extracting rotation turning into angle


            Numerics.Vector3 myQuat = new Numerics.Vector3();
            myQuat.X = (float)myTransform_FakeBasis33333.BasisZ.X; //step 3, combining z basis 
            myQuat.Y = (float)myTransform_FakeBasis33333.BasisZ.Y; //step 3, combining z basis 
            myQuat.Z = (float)myTransform_FakeBasis33333.BasisZ.Z; //step 3, combining z basis 

            Numerics.Vector3 myQuat2 = new Numerics.Vector3();
            myQuat2.X = (float)myTransform_FakeBasis44444.BasisZ.X; //step 3, combining z basis 
            myQuat2.Y = (float)myTransform_FakeBasis44444.BasisZ.Y; //step 3, combining z basis 
            myQuat2.Z = (float)myTransform_FakeBasis44444.BasisZ.Z; //step 3, combining z basis 

            foreach (float myFloat in myListfloat)
            {
                double myDoubleAlmostThere = (angle * (1 - myFloat)) + (d1_Final * (myFloat)); //step 2, z normal combining rotation

                Numerics.Vector3 myQuat3 = Numerics.Vector3.Lerp(myQuat2, myQuat, myFloat); //step 3, combining z basis 

                Transform myTransformFromQuat_Final = Transform.Identity;
                if (true)
                {
                    double aaMega_Quat = XYZ.BasisZ.AngleTo(new XYZ(myQuat3.X, myQuat3.Y, myQuat3.Z));  //step 3, turning it into a Transform

                    if (IsZero(aaMega_Quat)) myTransformFromQuat_Final = Transform.Identity;
                    else
                    {
                        XYZ axis = new XYZ(myQuat3.X, myQuat3.Y, myQuat3.Z).CrossProduct(-XYZ.BasisZ);
                        myTransformFromQuat_Final = Transform.CreateRotationAtPoint(axis, aaMega_Quat, XYZ.Zero);
                    }
                }

                Transform myEndPointTransform_JustOnceSide = Transform.CreateRotationAtPoint(myTransformFromQuat_Final.BasisZ, myDoubleAlmostThere, XYZ.Zero); //step 4, bringing it all together 

                Transform myTransformFromQuat_AllNewFinal = Transform.Identity;
                myTransformFromQuat_AllNewFinal.BasisX = myEndPointTransform_JustOnceSide.OfVector(myTransformFromQuat_Final.BasisX); //step 4, bringing it all together 
                myTransformFromQuat_AllNewFinal.BasisY = myEndPointTransform_JustOnceSide.OfVector(myTransformFromQuat_Final.BasisY); //step 4, bringing it all together
                myTransformFromQuat_AllNewFinal.BasisZ = myEndPointTransform_JustOnceSide.OfVector(myTransformFromQuat_Final.BasisZ); //step 4, bringing it all together

                Transform t = myTransformFromQuat_AllNewFinal;

                t.Origin = myReferencePoint3.Position + ((myReferencePoint2.Position - myReferencePoint3.Position) * (1 - myFloat));

                myListTransform_Interpolate.Add(t);
            }
        }


        public void setSlider(Xceed.Wpf.Toolkit.IntegerUpDown myToolKit_IntUpDown_temp, Slider mySlider_temp, bool myBool_UpdateLabels)
        {
            if (myToolKit_IntUpDown_temp.Value.Value == 0) return;

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (doc.GetElement(new ElementId(myToolKit_IntUpDown_temp.Value.Value)) == null) return;

            FamilyInstance myFamilyInstance = doc.GetElement(new ElementId(myToolKit_IntUpDown_temp.Value.Value)) as FamilyInstance;

            IList<ElementId> placePointIds_1338 = AdaptiveComponentInstanceUtils.GetInstancePointElementRefIds(myFamilyInstance);

            ReferencePoint myReferencePoint_Centre = doc.GetElement(placePointIds_1338.First()) as ReferencePoint;

            Transform myTransform = myReferencePoint_Centre.GetCoordinateSystem();

            Transform myTransformFromQuatXX;
            double aaMega_QuatXX = XYZ.BasisZ.AngleTo(myTransform.BasisZ);  //2 places line , 3 places method

            switch (mySlider_temp.Name)
            {
                case "mySlider_Rotate_BasisY":
                    aaMega_QuatXX = XYZ.BasisY.AngleTo(myTransform.BasisY);  //2 places line , 3 places method
                    break;
                case "mySlider_Rotate_BasisX":
                    aaMega_QuatXX = XYZ.BasisX.AngleTo(myTransform.BasisX);  //2 places line , 3 places method
                    break;
            }

            if (IsZero(aaMega_QuatXX)) myTransformFromQuatXX = Transform.Identity;
            else
            {
                XYZ axis = myTransform.BasisZ.CrossProduct(-XYZ.BasisZ);  //2 places line , 3 places method,  normally (z) negative

                switch (mySlider_temp.Name)
                {
                    case "mySlider_Rotate_BasisY":
                        axis = myTransform.BasisY.CrossProduct(-XYZ.BasisY); //2 places line , 3 places method,  normally (z) negative
                        break;
                    case "mySlider_Rotate_BasisX":
                        axis = myTransform.BasisX.CrossProduct(-XYZ.BasisX); //2 places line , 3 places method,  normally (z) negative
                        break;
                }

                if (IsZero(axis.X) & IsZero(axis.Y) & IsZero(axis.Z))
                {
                    myTransformFromQuatXX = myTransform;

                }
                else
                {
                    myTransformFromQuatXX = Transform.CreateRotationAtPoint(axis, aaMega_QuatXX, XYZ.Zero);
                }
            }

            Transform myEndPointTransform = Transform.Identity;
            myEndPointTransform.BasisX = myTransformFromQuatXX.Inverse.OfVector(myTransform.BasisX);
            myEndPointTransform.BasisY = myTransformFromQuatXX.Inverse.OfVector(myTransform.BasisY);
            myEndPointTransform.BasisZ = myTransformFromQuatXX.Inverse.OfVector(myTransform.BasisZ);

            double myDouble_AngleToXBasis = myEndPointTransform.BasisX.AngleOnPlaneTo(-XYZ.BasisX, XYZ.BasisZ); //two places in line, 

            switch (mySlider_temp.Name)
            {
                case "mySlider_Rotate_BasisY":
                    myDouble_AngleToXBasis = myEndPointTransform.BasisZ.AngleOnPlaneTo(-XYZ.BasisZ, XYZ.BasisY); //two places in line, 
                    break;
                case "mySlider_Rotate_BasisX":
                    myDouble_AngleToXBasis = myEndPointTransform.BasisY.AngleOnPlaneTo(-XYZ.BasisY, XYZ.BasisX); //two places in line, 
                    break;
            }

            double myDouble_TurnToVector = myDouble_AngleToXBasis / (Math.PI * 2) * 24;

            mySlider_temp.Value = myDouble_TurnToVector;

            if (myBool_UpdateLabels)
            {
                myLabel_TransformXBasis.Content = myTransform.BasisX.ToString();
                myLabel_TransformYBasis.Content = myTransform.BasisY.ToString();
                myLabel_TransformZBasis.Content = myTransform.BasisZ.ToString();
                myLabel_TransformOrigin.Content = myTransform.Origin.ToString();
            }
        }


        private void mySliderRotate_AGM_B_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {

                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySliderRotate_AGM_B.Value = 12; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_B_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
        private void mySliderRotate_AGM_A_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySliderRotate_AGM_A.Value = 12; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MySlider_DragCompleted_RotateX" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion 
        }

        private void mySliderRotate_AGM_B_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySliderRotate_AGM_B;
                myToolKit_IntUpDown = myIntUpDown_B;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myEE03_RotateAroundBasis.myBool_InterpolateMiddle_WhenEitherA_or_B = true;
                    myExternalEvent_EE03_RotateAroundBasis.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_B_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion   
        }

        private void mySliderRotate_AGM_A_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySliderRotate_AGM_A;
                myToolKit_IntUpDown = myIntUpDown_A;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myEE03_RotateAroundBasis.myBool_InterpolateMiddle_WhenEitherA_or_B = true;
                    myExternalEvent_EE03_RotateAroundBasis.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MySlider_DragStarted_RotateX" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion   
        }

        //private void myCopy_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        //Xceed.Wpf.Toolkit.IntegerUpDown myToolKit_IntUpDown = null;
        //        if (myRadioButton_PlatformMiddle.IsChecked.Value) myToolKit_IntUpDown = myIntUpDown_Middle2;
        //        if (myRadioButton_PlatformA.IsChecked.Value) myToolKit_IntUpDown = myIntUpDown_A;
        //        if (myRadioButton_PlatformB.IsChecked.Value) myToolKit_IntUpDown = myIntUpDown_B;


        //        if (myToolKit_IntUpDown.Value.Value == 0) return;

        //        UIDocument uidoc = commandData.Application.ActiveUIDocument;
        //        Document doc = uidoc.Document;

        //        if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null) return;

        //        myExternalEvent_EE01_Part1_Duplicate.Raise();
        //    }

        //    #region catch and finally
        //    catch (Exception ex)
        //    {
        //        _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myCopy_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
        //    }
        //    finally
        //    {
        //    }
        //    #endregion   
        //}

        //private void myDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    try //delete is to be developed out from what is selected, i'm not sure if mySelectMethod will continue to be used
        //    {
        //        //Xceed.Wpf.Toolkit.IntegerUpDown myToolKit_IntUpDown = null;
        //        if (myRadioButton_PlatformMiddle.IsChecked.Value) myToolKit_IntUpDown = myIntUpDown_Middle2;
        //        if (myRadioButton_PlatformA.IsChecked.Value) myToolKit_IntUpDown = myIntUpDown_A;
        //        if (myRadioButton_PlatformB.IsChecked.Value) myToolKit_IntUpDown = myIntUpDown_B;


        //        if (!mySelectMethod(myToolKit_IntUpDown)) return;
        //        myExternalEvent_EE01_Part1_Delete.Raise();
        //    }

        //    #region catch and finally
        //    catch (Exception ex)
        //    {
        //        _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myDelete_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
        //    }
        //    finally
        //    {
        //    }
        //    #endregion
        //}
        bool myBool_Rezero = false;

        private void mySlider_Rotate_BasisY_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySlider_Rotate_BasisY;
                myToolKit_IntUpDown = myIntUpDown_Middle2;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myEE03_RotateAroundBasis.myBool_InterpolateMiddle_WhenEitherA_or_B = false;
                    myExternalEvent_EE03_RotateAroundBasis.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_BasisY_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Rotate_BasisX_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySlider_Rotate_BasisX;
                myToolKit_IntUpDown = myIntUpDown_Middle2;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myEE03_RotateAroundBasis.myBool_InterpolateMiddle_WhenEitherA_or_B = false;
                    myExternalEvent_EE03_RotateAroundBasis.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_BasisX_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Rotate_BasisZ_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySlider_Rotate_BasisZ;
                myToolKit_IntUpDown = myIntUpDown_Middle2;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myEE03_RotateAroundBasis.myBool_InterpolateMiddle_WhenEitherA_or_B = false;
                    myExternalEvent_EE03_RotateAroundBasis.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_BasisZ_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void mySlider_Interpolate_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                if (true)  //candidate for methodisataion 202006061646
                {

                    if (myIntUpDown_Middle2.Value.Value == 0)
                    {
                        MessageBox.Show("Set or Place AGM first please.");
                        return;
                    }

                    if (myIntUpDown_A.Value.Value == 0)
                    {
                        MessageBox.Show("Set or Place AGM-A first please.");
                        return;
                    }

                    if (myIntUpDown_B.Value.Value == 0)
                    {
                        MessageBox.Show("Set or Place AGM-B first please.");
                        return;
                    }

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myIntUpDown_Middle2.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myMethod_whichTook_120Hours_OfCoding();

                    myLabel_Progress.Content = "In Progress";
                    mySlideInProgress = true;
                    myEE01_Part1_Interpolate.myBool_Cycle = false;
                    myExternalEvent_EE01_Part1_Interpolate.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Interpolate_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Interpolate_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                if (true) //candidate for methodisation 202006061716
                {
                    myLabel_Progress.Content = "Not in Progress";

                    mySlideInProgress = false;

                    if (myBool_Rezero) { mySlider_Interpolate.Value = 12; myBool_Rezero = false; }
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Interpolate_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void myButton_PickMiddle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // uidoc.Selection.SetElementIds(new List<ElementId>() { new ElementId(262203) });  //262203  260988   262427

                if (!mySelectMethod(myIntUpDown_Middle2)) return;
                //setSlider(myIntUpDown_A, mySliderNewRotate_X);
                setSlider(myIntUpDown_Middle2, mySlider_Interpolate, false);
                //setSlider(myIntUpDown_B, mySlider_Rotate_B);
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PickMiddle_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButton_PickA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!mySelectMethod(myIntUpDown_A)) return;
                setSlider(myIntUpDown_A, mySliderRotate_AGM_A, false);
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PickA_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButton_PickB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!mySelectMethod(myIntUpDown_B)) return;
                //setSlider(myIntUpDown_A, mySliderNewRotate_X);
                //setSlider(myIntUpDown_Middle2, mySlider_Interpolate);
                setSlider(myIntUpDown_B, mySliderRotate_AGM_B, false);
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PickB_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void myWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                bool myBool_RunFamilyLoadEvent = false;

                string myString_TempPath = "";


                if (true)
                {
                    string stringAGM_FileName = "PRL-GM Adaptive Carrier Youtube";
                    List<Element> myListFamilySymbol_1738 = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Family))).Where(x => x.Name == stringAGM_FileName).ToList();
                    if (myListFamilySymbol_1738.Count != 1) myBool_RunFamilyLoadEvent = true;
                }

                if (true)
                {
                    string stringChair_FileName = "PRL-GM Chair with always vertical OFF";
                    List<Element> myListFamilySymbol_1738 = new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(Family))).Where(x => x.Name == stringChair_FileName).ToList();
                    if (myListFamilySymbol_1738.Count != 1) myBool_RunFamilyLoadEvent = true;
                }

                if (myBool_RunFamilyLoadEvent) myExternalEvent_EE06_LoadFamily.Raise();




                //myRadioButton_PlatformMiddle.IsChecked = true;
                setSlider(myIntUpDown_A, mySliderRotate_AGM_A, false);
                setSlider(myIntUpDown_Middle2, mySlider_Interpolate, false);
                setSlider(myIntUpDown_B, mySliderRotate_AGM_B, false);


                setSlider(myIntUpDown_Middle2, mySlider_Rotate_BasisZ, false);
                setSlider(myIntUpDown_Middle2, mySlider_Rotate_BasisX, false);
                setSlider(myIntUpDown_Middle2, mySlider_Rotate_BasisY, true);

                myMethod_whichTook_120Hours_OfCoding();




            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myWindow_Loaded" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void mySlider_Rotate_BasisZ_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySlider_Rotate_BasisZ.Value = 12; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_BasisZ_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Rotate_BasisY_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySlider_Rotate_BasisY.Value = 12; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_BasisY_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Rotate_BasisX_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySlider_Rotate_BasisX.Value = 12; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Rotate_BasisX_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void mySlider_Move_X_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySlider_Move_X;
                myToolKit_IntUpDown = myIntUpDown_Middle2;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myExternalEvent_EE05_Move.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Move_X_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Move_Y_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySlider_Move_Y;
                myToolKit_IntUpDown = myIntUpDown_Middle2;

                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myExternalEvent_EE05_Move.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Move_Y_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Move_Z_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            try
            {
                mySlider = mySlider_Move_Z;
                myToolKit_IntUpDown = myIntUpDown_Middle2;


                if (true) //candidate for methodisation 202005232228
                {
                    if (myToolKit_IntUpDown.Value.Value == 0) return;

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myToolKit_IntUpDown.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myLabel_Progress.Content = "In Progress";

                    mySlideInProgress = true;

                    myExternalEvent_EE05_Move.Raise();
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Move_Z_DragStarted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }


        private void mySlider_Move_X_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {

                //if (!myBool_Rezero) myExternalEvent_EE05_Move.Raise();

                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySlider_Move_X.Value = 0; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Move_X_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Move_Y_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySlider_Move_Y.Value = 0; myBool_Rezero = false; }
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Move_Y_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void mySlider_Move_Z_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                myLabel_Progress.Content = "Not in Progress";

                mySlideInProgress = false;

                if (myBool_Rezero) { mySlider_Move_Z.Value = 0; myBool_Rezero = false; }
                //mySlider_Move_Z.Value = 0;
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("mySlider_Move_Z_DragCompleted" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButton_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!mySelectMethod(myIntUpDown_Middle2)) return;

                // MessageBox.Show("not getting this far");

                myToolKit_IntUpDown = myIntUpDown_Middle2;

                myExternalEvent_EE04_ResetPosition.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_Reset_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButton_PlaceCenterAGM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myEE06_PlaceFamily.myString_Family_Platform = "PRL-GM Adaptive Carrier Youtube";
                myEE06_PlaceFamily.myString_Type_Platform = "PRL-GM-2020 Adaptive Carrier Rope White";
                myEE06_PlaceFamily.myString_Family_Chair = "PRL-GM Chair with always vertical OFF";
                myEE06_PlaceFamily.myString_Type_Chair = "565 x 565 mm";
                myEE06_PlaceFamily.myBool_AlsoPlaceAChair = true;
                myEE06_PlaceFamily.myIntUPDown = myIntUpDown_Middle2;

                myExternalEvent_EE06_PlaceFamily.Raise();
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PlaceCenterAGM_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }



        private void myButton_PlaceAGM_A_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    myEE06_PlaceFamily.myString_Family_Platform = "PRL-GM Adaptive Carrier Youtube";
                    myEE06_PlaceFamily.myString_Type_Platform = "PRL-GM-2020 Adaptive Carrier A";
                    myEE06_PlaceFamily.myBool_AlsoPlaceAChair = false;
                    myEE06_PlaceFamily.myIntUPDown = myIntUpDown_A;

                    myExternalEvent_EE06_PlaceFamily.Raise();
                }

                #region catch and finally
                catch (Exception ex)
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PlaceCenterAGM_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                }
                finally
                {
                }
                #endregion
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PlaceAGM_A_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void myButton_PlaceAGM_B_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    myEE06_PlaceFamily.myString_Family_Platform = "PRL-GM Adaptive Carrier Youtube";
                    myEE06_PlaceFamily.myString_Type_Platform = "PRL-GM-2020 Adaptive Carrier B";
                    myEE06_PlaceFamily.myBool_AlsoPlaceAChair = false;
                    myEE06_PlaceFamily.myIntUPDown = myIntUpDown_B;

                    myExternalEvent_EE06_PlaceFamily.Raise();
                }

                #region catch and finally
                catch (Exception ex)
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PlaceCenterAGM_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
                }
                finally
                {
                }
                #endregion
            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("myButton_PlaceAGM_B_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        private void MyButton_Cycle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (true)  //candidate for methodisataion 202006061646
                {

                    if (myIntUpDown_Middle2.Value.Value == 0)
                    {
                        MessageBox.Show("Set or Place AGM first please.");
                        return;
                    }

                    if (myIntUpDown_A.Value.Value == 0)
                    {
                        MessageBox.Show("Set or Place AGM-A first please.");
                        return;
                    }

                    if (myIntUpDown_B.Value.Value == 0)
                    {
                        MessageBox.Show("Set or Place AGM-B first please.");
                        return;
                    }

                    UIDocument uidoc = commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;

                    if (doc.GetElement(new ElementId(myIntUpDown_Middle2.Value.Value)) == null)
                    {
                        myBool_Rezero = true;
                        return;
                    }

                    myMethod_whichTook_120Hours_OfCoding();

                    myLabel_Progress.Content = "In Progress";
                    mySlideInProgress = true;
                    myEE01_Part1_Interpolate.myBool_Cycle = true;
                    myExternalEvent_EE01_Part1_Interpolate.Raise();
                }
            }
            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("MyButton_Cycle_Click" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }
    }
}
