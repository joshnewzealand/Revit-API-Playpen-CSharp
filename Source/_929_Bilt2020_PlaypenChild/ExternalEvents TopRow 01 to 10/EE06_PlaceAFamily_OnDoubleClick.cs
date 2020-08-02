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
using Autodesk.Revit.DB.Events;
using System.Runtime.InteropServices;

namespace _929_Bilt2020_PlaypenChild
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class EE06_PlaceAFamily_OnDoubleClick : IExternalEventHandler  //this is the last when one making a checklist change, EE4 must be just for when an element is new
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public MainWindow myWindow1 { get; set; }

        public FamilySymbol myFamilySymbol { get; set; }

        void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            try
            {
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                if (e.GetAddedElementIds().Count == 0 & e.GetModifiedElementIds().Count == 0) return;

                if (e.GetAddedElementIds().Count > 0)
                {
                    Element myElement = doc.GetElement(e.GetAddedElementIds().First());

                    if (myElement.GetType().Name != "FamilyInstance") return;
                }

                if (e.GetModifiedElementIds().Count > 0)
                {
                    Element myElement = doc.GetElement(e.GetModifiedElementIds().First());

                    if (myElement.GetType().Name != "FamilyInstance") return;
                }

                uidoc.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

                SetForegroundWindow(uidoc.Application.MainWindowHandle); //this is an excape event
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);
                keybd_event(0x1B, 0, 0, 0);
                keybd_event(0x1B, 0, 2, 0);

            }

            #region catch and finally
            catch (Exception ex)
            {
                _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("OnDocumentChanged" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);
            }
            finally
            {
            }
            #endregion
        }

        UIApplication uiapp;
        bool myBool_PassThrough = true;

        public void Execute(UIApplication uiappp)
        {
            uiapp = uiappp;

            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            uidoc.Application.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);

            PromptForFamilyInstancePlacementOptions myPromptForFamilyInstancePlacementOptions = new PromptForFamilyInstancePlacementOptions();

            if (uidoc.ActiveView.SketchPlane.Name != "Level 1")
            {
                Level myLevel = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().First() as Level;
                using (Transaction y = new Transaction(doc, "SetDefaultPlane"))
                {
                    y.Start();
                    uidoc.ActiveView.SketchPlane = SketchPlane.Create(doc, myLevel.GetPlaneReference());
                    y.Commit();

                }
            }

            myPromptForFamilyInstancePlacementOptions.FaceBasedPlacementType = FaceBasedPlacementType.PlaceOnWorkPlane;
            SetForegroundWindow(uidoc.Application.MainWindowHandle); //this is an excape event



            try
            {
                if(myFamilySymbol.Family.get_Parameter(BuiltInParameter.FAMILY_WORK_PLANE_BASED).AsInteger() == 0)
                {
                    uidoc.PromptForFamilyInstancePlacement(myFamilySymbol);
                } else
                {
                    uidoc.PromptForFamilyInstancePlacement(myFamilySymbol, myPromptForFamilyInstancePlacementOptions);
                }
            }

            #region catch and finally
            catch (Exception ex)
            {
                if (ex.Message != "The user aborted the pick operation.")
                {
                    _952_PRLoogleClassLibrary.DatabaseMethods.writeDebug("EE01_Part1_PlaceAFamily" + Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException, true);

                }
                else
                {
                    uidoc.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(OnDocumentChanged);
                }
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
