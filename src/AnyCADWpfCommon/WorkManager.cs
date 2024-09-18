
using AnyCAD.Foundation;
using System.Numerics;
using System.Windows;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using Application = AnyCAD.Foundation.Application;

namespace AnyCADWpfCommon
{
    public class WorkManager
    {
        private static readonly Lazy<WorkManager> _instance = new Lazy<WorkManager>(() => new WorkManager());
        private string _workId;
        private AnyCAD.WPF.RenderControl _renderControl;
        private List<ElementId> _allEleIds;
        private List<ElementId> _curEleIds;
        private List<ShapeElement> _allShape;
        private List<ShapeElement> _curShape;
        private bool _rebuildFlag;
        private WorkManager()
        {
            _allEleIds = new List<ElementId>();
        }
        public static void Initialize()
        {
            if (_instance.IsValueCreated)
            {
                throw new InvalidOperationException("WorkManager is already initialized.");
            }
            _instance.Value._workId = Guid.NewGuid().ToString();
        }
        public static WorkManager Instance => _instance.Value;
        public string GetWorkId()
        {
            return _workId;
        }
        public void SetRebuildFlag(bool flag)
        {
            _rebuildFlag = flag;
        }
        public bool GetRebuildFlag()
        {
            return _rebuildFlag;
        }
        public AnyCAD.WPF.RenderControl GetRenderControl => _renderControl;
        public bool SetRenderControl(AnyCAD.WPF.RenderControl renderControl)
        {
            _renderControl = renderControl;
            return true;
        }
        public void Render(TopoShape topoShape, ulong userId)
        {
            AddAllEleIds(new ElementId(userId));
            var node = BrepSceneNode.Create(topoShape, null, null);
            //_renderControl.ShowShape(topoShape, new AnyCAD.Foundation.Vector3());
            //_renderControl.ShowShape
            //_renderControl.ShowSceneNode(node);
            var scene = _renderControl.ViewContext.GetScene();
            if(node == null)
            {
                _renderControl.ShowShape(topoShape, new AnyCAD.Foundation.Vector3());
                return;
            }
            node.SetUserId(userId);
            scene.AddNode(node);
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
        }
        public void RemoveNode(ElementId elementId)
        {
            RemoveAllEleIds(elementId);
            Application.Instance().GetActiveDocument().RemoveElement(elementId);
            ulong inte = elementId.GetInteger();
            var scene = _renderControl.ViewContext.GetScene();
            var sceneNode = scene.FindNodeByUserId(inte);
            if(sceneNode != null)
            {
                scene.RemoveNode(sceneNode.GetUuid());
                _renderControl.ViewContext.RequestUpdate(EnumUpdateFlags.Scene);
            }
            _renderControl.RequestDraw(EnumUpdateFlags.Scene);
            //_renderControl.RemoveSceneNode((uint)elementId.GetInteger());
        }
        public void AddAllEleIds(ElementId elementId)
        {
            _allEleIds.Add(elementId);
        }
        public void RemoveAllEleIds(ElementId elementId)
        {
            _allEleIds.Remove(elementId);
        }
        public void ClearAllEleIds()
        {
            _allEleIds.Clear();
        }
        public void AddCurEleIds(ElementId elementId)
        {
            _curEleIds.Clear();
            _curEleIds.Add(elementId);
        }
        public List<ElementId> AllEleIds => _allEleIds ?? new List<ElementId>();
        public List<ElementId> CurEleIds => _curEleIds ?? new List<ElementId>();
        public void SetAllEleIds(List<ElementId> elementIds)
        {
            _allEleIds = elementIds;
        }
        public void AddCurShape(ShapeElement shapeElement)
        {
            _curShape.Add(shapeElement);
        }     
        public void InitialViewer()
        {
            var viewer = _renderControl.Viewer;
            viewer.SetBackgroundColor(new AnyCAD.Foundation.Vector4(203.0f / 255, 216.0f / 255, 234.0f / 255, 0));
            var viewContext = _renderControl.ViewContext;
            Application.Instance().SetActiveViewer(viewer);
            viewContext.ClearAll();
            Scene scene = viewContext.GetScene();
            var _AxisWidget = AxisWidget.Create(1f, new AnyCAD.Foundation.Vector3(50f));
            scene.AddNode(_AxisWidget);
            viewer.ZoomToExtend();
        }
        public void ShowAllEleIds()
        {
            var res = string.Join("\n", _allEleIds);
            MessageBox.Show($"模型数量为{_allEleIds.Count}，分别为：\n" + res);
        }
        public void ClearViewer()
        {
            _renderControl.ClearScene();
            InitialViewer();
            ClearAllEleIds();
        }
    }
}
