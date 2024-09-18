using AnyCAD.CoreNodes.Geometry;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.CoreNodes.BooleanTool
{
    public class GCutFeatureNode : IGraphicItem
    {
        private readonly TopoShape _baseShape;
        private readonly TopoShape _toolShape;
        private GCutFeatureNode(TopoShapeNode baseShape, TopoShapeNode toolShape)
        {
            _baseShape = baseShape.Value;
            _toolShape = toolShape.Value;
        }
        [IsVisibleInDynamoLibrary(false)]
        //public TopoShape TopoShape => CurveBuilder.MakeCircle(mCirc);
        public TopoShape TopoShape => AnyCAD.Foundation.BooleanTool.Cut(_baseShape, _toolShape);

        public static GCutFeatureNode ByTwoShape(TopoShapeNode baseShape, TopoShapeNode toolShape)
        {
            return new GCutFeatureNode(baseShape, toolShape);
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {

        }
    }
}
