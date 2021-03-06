/*
 * <Iguana>
    Copyright (C) < 2020 >  < Seiichi Suzuki >

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 2 or later of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace Iguana.IguanaMesh.ICreators
{
    public static class IMeshCreator
    {
        public static IMesh CreatePlane(int U, int V, double sizeU, double sizeV, Plane plane)
        {
            IPlaneGrid data = new IPlaneGrid(U, V, sizeU, sizeV, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateTube(int U, int V, double innerRadius, double outerRadius, double height, double shiftX, double shiftY)
        {
            ITube data = new ITube(U, V, innerRadius, outerRadius, height, shiftX, shiftY);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateTube(int U, int V, double innerRadius, double outerRadius, Curve path)
        {
            ITube data = new ITube(U, V, innerRadius, outerRadius, path);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateCylinder(int U, int V, double lowerRadius, double upperRadius, double height, Plane plane)
        {
            ICylinder data = new ICylinder(U, V, lowerRadius, upperRadius, height, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateCube(Box box, int u, int v, int w, Boolean weld, double tolerance)
        {
            ICube data = new ICube(box, u, v, w, weld, tolerance);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateCube(Plane pl, Interval x, Interval y, Interval z, int u, int v, int w, Boolean weld, double tolerance)
        {
            Box box = new Box(pl, x, y, z);
            ICube data = new ICube(box, u, v, w, weld, tolerance);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateFromRhinoMesh(Mesh m)
        {
            IFromRhinoMesh data = new IFromRhinoMesh(m);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateMoebiusStrip(int U, int V, double r1, double r2, double h, Plane plane)
        {
            IMoebius data = new IMoebius(U, V, r1, r2, h, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateEllipticDupinCyclide(int U, int V, double a, double b, double c, double d, Interval D1, Interval D2, Plane plane)
        {
            IEllipticDupinCyclide data = new IEllipticDupinCyclide(U, V, a, b, c, d, D1, D2, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateTorus(int U, int V, double r1, double r2, Interval D1, Interval D2, Plane plane)
        {
            ITorus data = new ITorus(U, V, r1, r2, D1, D2, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateHyperboloidOneSheet(int U, int V, double a, double b, double c, Interval D1, Interval D2, Plane plane)
        {
            IHyperboloidOneSheet data = new IHyperboloidOneSheet(U, V, a, b, c, D1, D2, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateParabolicCylinder(int U, int V, double a, Interval D1, Interval D2, Plane plane)
        {
            IParabolicCylinder data = new IParabolicCylinder(U, V, a, D1, D2, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }

        public static IMesh CreateEllipsoid(int U, int V, double a, double b, double c, Interval D1, Interval D2, Plane plane)
        {
            IEllipsoid data = new IEllipsoid(U, V, a, b, c, D1, D2, plane);
            IMesh mesh = data.BuildMesh();
            return mesh;
        }
    }
}
