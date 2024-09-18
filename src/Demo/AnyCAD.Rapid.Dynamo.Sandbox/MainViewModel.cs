using System.Reflection;
using CommunityToolkit.Mvvm.Input;
using AnyCAD.WPF;
using AnyCAD.Foundation;
using AnyCAD.Rapid.Dynamo.Startup;
using AnyCADWpfCommon;
using System.Windows;
using Application = AnyCAD.Foundation.Application;

namespace AnyCAD.Rapid.Dynamo.Sandbox
{
    public partial class MainViewModel
    {
        private RenderControl mRenderControl;
        private MainDocumentListener mDocumentListener;

        public MainViewModel(RenderControl renderControl)
        {
            mRenderControl = renderControl;
            WorkManager.Instance.SetRenderControl(renderControl);
        }

        public Document Document => Application.Instance().GetActiveDocument();

        public bool Initialize()
        {
            if (mRenderControl == null)
                return false;

            

            var viewer = mRenderControl.Viewer;
            viewer.SetBackgroundColor(new Vector4(203.0f / 255, 216.0f / 255, 234.0f / 255, 0));
            var viewContext = mRenderControl.ViewContext;
            Application.Instance().SetActiveViewer(viewer);

            var smgr = viewContext.GetSelection();
            smgr.SetDepthTest(true);
            smgr.SetListener(new PickListener());

            mDocumentListener = new MainDocumentListener(this);
            DocumentEvent.Instance().AddListener(mDocumentListener);
            Document document = Application.Instance().CreateDocument(".acad");
            document.SetModified(false);
            Application.Instance().SetActiveDocument(document);
            var mRootNode = new DocumentSceneNode(document);
            mRootNode.SetCulling(val: false);
            viewer.GetContext().GetScene().AddNode(mRootNode);
            viewer.GetContext().RequestUpdate(EnumUpdateFlags.Scene);




            mRootNode?.SetDocument(document);
            mRootNode?.RequstUpdate();
            //var mViewContext = mView3d.ViewContext;
            viewContext.ClearAll();
            Scene scene = viewContext.GetScene();
            var _AxisWidget = AxisWidget.Create(1f, new Vector3(50f));
            scene.AddNode(_AxisWidget);
            scene.AddNode(mRootNode);

            viewer.ZoomToExtend();
            return true;
        }

        public void OnDocumentChanged(DocumentEventArgs args)
        {
            ViewContext viewContext = mRenderControl.ViewContext;
            if (viewContext == null || args.GetPreviewing())
            {
                return;
            }

            Scene scene = viewContext.GetScene();
            foreach (ElementId addedId in args.GetAddedIds())
            {
                ulong integer = addedId.GetInteger();
                if (scene.FindNodeByUserId(integer) == null)
                {   
                    
                    
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }

            foreach (ElementId removedId in args.GetRemovedIds())
            {
                ulong integer2 = removedId.GetInteger();
                SceneNode sceneNode = scene.FindNodeByUserId(integer2);
                if (sceneNode != null)
                {
                    scene.RemoveNode(sceneNode.GetUuid());
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }

            Document document = Document;
            foreach (var changedId in args.GetChangedIds())
            {
                ElementId objectId = changedId;
                ulong value = changedId.Value;
                ShapeElement component = ShapeElement.Cast(document.FindElement(changedId));
                if (component != null)
                {
                    objectId = component.GetId();

                    value = objectId.Value;
                }

                if (scene.FindNodeByUserId(value) != null)
                {
                    viewContext.RequestUpdate(EnumUpdateFlags.Scene);
                }
            }
        }

        [RelayCommand]
        void OnOpenDynamo()
        {
            var userNodesDll = new List<string>()
            {
                "AnyCAD.UserNodes.dll"
            };
            var layoutSpecs = GetLayoutSpecsPath();
            RapidDynamoManager.Instance.ConfigureUserNodes(userNodesDll, layoutSpecs);
            RapidDynamoManager.Instance.StartDynamo();
        }
        [RelayCommand]
        void OnTestFunc()
        {
            WorkManager.Instance.ShowAllEleIds();
        }

        private string? GetLayoutSpecsPath()
        {
            var asmLocation = Assembly.GetExecutingAssembly().Location;
            var asmDirectory = System.IO.Path.GetDirectoryName(asmLocation);
            return System.IO.Path.Combine(asmDirectory, "Resources", "LayoutSpecs.json");
        }
    }
}
