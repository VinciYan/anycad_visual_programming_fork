using Autodesk.DesignScript.Geometry;
using DynamoServices;

using AnyCAD.Foundation;

using AnyCAD.Rapid.Dynamo.Services.Persistence;
using AnyCAD.CoreNodes.GeometryInterop;
using AnyCAD.CoreNodes.Geometry;

using ADGeometry = Autodesk.DesignScript.Geometry.Geometry;
using System.Reflection.Metadata;
using AnyCADWpfCommon;

namespace AnyCAD.CoreNodes.Elements
{
    /// <summary>
    /// ShapeElementNode，AnyCAD.Foundation.ShapeElement在Dynamo环境下的适配器
    /// 扩展用于创建ShapeElement的节点时从该类继承并增加自定义的By...静态方法
    /// </summary>
    [RegisterForTrace]
    public class ShapeElementNode : ElementNode
    {
        internal ShapeElement InternalShape { get; private set; }
        public override Element InternalElement => InternalShape;

        protected ShapeElementNode(TopoShape topoShape)
        {
            SafeInit(() => Init(topoShape));
        }

        public static ShapeElementNode ByGeometry(ADGeometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            var topoShape = geometry.To();
            if (topoShape == null)
            {
                throw new ArgumentNullException(nameof(topoShape));
            }

            return new ShapeElementNode(topoShape);
        }

        public static ShapeElementNode ByTopoShape(TopoShapeNode topo)
        {
            if (topo == null)
            {
                throw new ArgumentNullException(nameof(topo));
            }

            return new ShapeElementNode(topo.Value);
        }

        protected void Init(TopoShape topoShape)
        {            
            List<ElementId> elementIds = new List<ElementId>();
            // try to reuse from trace
            var shapeElement = ShapeElement.Cast(ElementBinder.GetElementFromTrace(Document));
            UndoTransaction undo = new(Document);
            undo.Start("shape");
            if (shapeElement == null)
            {
                shapeElement = new ShapeElement();
                shapeElement.SetShape(topoShape);
                Guid guid = Guid.NewGuid();
                byte[] byteArray = guid.ToByteArray();
                ulong numericId = BitConverter.ToUInt64(byteArray, 0);
                shapeElement.SetId(new ElementId(numericId));
            }
            shapeElement.SetShape(topoShape);
 
            undo.Commit();

            InternalInit(shapeElement);

            // TODO: clear and set?
            ElementBinder.SetElementForTrace(shapeElement.GetId());
            WorkManager.Instance.Render(topoShape, shapeElement.GetId().GetInteger());
        }

        private void InternalInit(ShapeElement shapeElement)
        {
            InternalShape = shapeElement;
            InternalId = shapeElement.GetId();
        }
    }
}
