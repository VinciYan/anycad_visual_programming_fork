using AnyCAD.Foundation;
using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Interfaces;
using Autodesk.DesignScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.CoreNodes.Geometry
{
    public class GBoxNode : IGraphicItem
    {
        private readonly GAx2 _gAx2;
        private readonly double _dx;
        private readonly double _dy;
        private readonly double _dz;
        private GBoxNode(GAx2 position, double dx, double dy, double dz)
        {
            _gAx2 = position;
            _dx = dx;
            _dy = dy;
            _dz = dz;
        }
        [IsVisibleInDynamoLibrary(false)]
        //public TopoShape TopoShape => CurveBuilder.MakeCircle(mCirc);
        public TopoShape TopoShape => ShapeBuilder.MakeBox(_gAx2, _dx, _dy, _dz);
        /// <summary>
        /// 通过中心位置和长度创建GBoxNode
        /// </summary>
        /// <param name="position">GPntNode类型的中心位置</param>
        /// <param name="dx">X长度</param>
        /// <param name="dy">Y长度</param>
        /// <param name="dz">Z长度</param>
        /// <returns></returns>
        public static GBoxNode ByCenterPosition([DefaultArgument("GPntNode.ByCoordinates(0, 0, 0)")] GPntNode position, double dx = 1.0, double dy = 1.0, double dz = 1.0)
        {
            return new GBoxNode(new GAx2(new GPnt(position.X, position.Y, position.Z), new GDir(0, 0, 1)),dx, dy, dz);
        }

        [IsVisibleInDynamoLibrary(false)]
        public void Tessellate(IRenderPackage package, TessellationParameters parameters)
        {

        }
    }
}
