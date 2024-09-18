using AnyCAD.Foundation;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.CoreNodes.Geometry
{
    public class GLineNode : IGraphicItem
    {
        private readonly double _startX;
        private readonly double _startY;
        private readonly double _startZ;

        private readonly double _endX;
        private readonly double _endY;
        private readonly double _endZ;
        private GLineNode(GPntNode startGPntNode, GPntNode endGPntNode)
        {
            _startX = startGPntNode.X;
            _startY = startGPntNode.Y;
            _startZ = startGPntNode.Z;

            _endX = endGPntNode.X;
            _endY = endGPntNode.Y;
            _endZ = endGPntNode.Z;
        }
        [IsVisibleInDynamoLibrary(false)]
        //public TopoShape TopoShape => CurveBuilder.MakeCircle(mCirc);
        public TopoShape TopoShape => SketchBuilder.MakeLine(new GPnt(_startX, _startY, _startZ), new GPnt(_endX, _endY, _endZ));
        /// <summary>
        /// 通过两个点GPntNode创建直线GLineNode
        /// </summary>
        /// <param name="startPosition">起点GPntNode</param>
        /// <param name="endPosition">终点GPntNode</param>
        /// <returns></returns>
        public static GLineNode ByTwoGPntNode([DefaultArgument("GPntNode.ByCoordinates(0, 0, 0)")] GPntNode startPosition, [DefaultArgument("GPntNode.ByCoordinates(10, 10, 10)")] GPntNode endPosition)
        {
            return new GLineNode(startPosition, endPosition);
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {

        }
    }
}
