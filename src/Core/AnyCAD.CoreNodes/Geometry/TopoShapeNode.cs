using AnyCAD.CoreNodes.BooleanTool;
using AnyCAD.Foundation;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.CoreNodes.Geometry
{
    public class TopoShapeNode
    {
        private TopoShape mTopoShape;
        private TopoShapeNode(TopoShape shape)
        {
            mTopoShape = shape;
        }

        [IsVisibleInDynamoLibrary(false)]
        public TopoShape Value => mTopoShape;

        public static TopoShapeNode ByGCicle(GCircleNode circle)
        {
            if (circle == null)
            {
                throw new ArgumentNullException(nameof(circle));
            }
            return new TopoShapeNode(circle.TopoShape);
        }
        public static TopoShapeNode ByGBox(GBoxNode box)
        {
            if (box == null)
            {
                throw new ArgumentNullException(nameof(box));
            }
            return new TopoShapeNode(box.TopoShape);
        }
        public static TopoShapeNode ByGLine(GLineNode line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }
            return new TopoShapeNode(line.TopoShape);
        }
        public static TopoShapeNode ByGCutFeature(GCutFeatureNode gCutFeature)
        {
            if (gCutFeature == null)
            {
                throw new ArgumentNullException(nameof(gCutFeature));
            }
            return new TopoShapeNode(gCutFeature.TopoShape);
        }
    }
}
