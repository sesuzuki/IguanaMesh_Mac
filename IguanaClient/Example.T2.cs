﻿using Iguana.IguanaMesh.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IguanaClient
{
    // -----------------------------------------------------------------------------
    //
    //  Gmsh C++ tutorial 2
    //
    //  Transformations, extruded geometries, volumes
    //
    // -----------------------------------------------------------------------------

    public static partial class Example
    {
        public static void T2()
        {

            // If argc/argv are passed to gmsh::initialize(), Gmsh will parse the command
            // line in the same way as the standalone Gmsh app:
            Kernel.Initialize();

            Kernel.Option.SetNumber("General.Terminal", 1);

            Kernel.Model.Add("t2");

            // Copied from t1.cpp...
            double lc = 1e-2;
            Kernel.GeometryKernel.AddPoint(0, 0, 0, lc, 1);
            Kernel.GeometryKernel.AddPoint(.1, 0, 0, lc, 2);
            Kernel.GeometryKernel.AddPoint(.1, .3, 0, lc, 3);
            Kernel.GeometryKernel.AddPoint(0, .3, 0, lc, 4);
            Kernel.GeometryKernel.AddLine(1, 2, 1);
            Kernel.GeometryKernel.AddLine(3, 2, 2);
            Kernel.GeometryKernel.AddLine(3, 4, 3);
            Kernel.GeometryKernel.AddLine(4, 1, 4);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 4, 1, -2, 3}, 1);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 1}, 1);

            //IguanaGmsh.Model.AddPhysicalGroup(1, new[]{ 1, 2, 4}, 5);


            // We can then add new points and curves in the same way as we did in
            // `t1.cpp':
            Kernel.GeometryKernel.AddPoint(0, 0.4, 0, lc, 5);
            Kernel.GeometryKernel.AddLine(4, 5, 5);

            // But Gmsh also provides tools to transform (translate, rotate, etc.)
            // elementary entities or copies of elementary entities.  Geometrical
            // transformations take a vector of pairs of integers as first argument, which
            // contains the list of entities, represented by (dimension, tag) pairs.  For
            // example, the point 5 (dimension=0, tag=5) can be moved by 0.02 to the left
            // (dx=-0.02, dy=0, dz=0) with
            Tuple<int, int>[] temp = new Tuple<int, int>[] { Tuple.Create(0, 5) };
            Kernel.GeometryKernel.Translate(temp, -0.02, 0, 0);

            // And it can be further rotated by -Pi/4 around (0, 0.3, 0) (with the
            // rotation along the z axis) with:
            Kernel.GeometryKernel.Rotate(temp, 0, 0.3, 0, 0, 0, 1, -Math.PI / 4);

            // Note that there are no units in Gmsh: coordinates are just numbers - it's
            // up to the user to associate a meaning to them.

            // Point 3 can be duplicated and translated by 0.05 along the y axis by using
            // the `copy()' function, which takes a vector of (dim, tag) pairs as input,
            // and returns another vector of (dim, tag) pairs:
            Tuple<int, int>[] ov;
            temp = new Tuple<int, int>[] { Tuple.Create(0,3) };
            Kernel.GeometryKernel.Copy(temp, out ov);
            Kernel.GeometryKernel.Translate(ov, 0, 0.05, 0);

            // The new point tag is available in ov[0].second, and can be used to create
            // new lines:
            Kernel.GeometryKernel.AddLine(3, ov[0].Item2, 7);
            Kernel.GeometryKernel.AddLine(ov[0].Item2, 5, 8);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 5, -8, -7, 3}, 10);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 10}, 11);

            int ps = Kernel.Model.AddPhysicalGroup(2, new[] { 1, 11 });
            Kernel.Model.SetPhysicalName(2, ps, "My surface");

            // In the same way, we can translate copies of the two surfaces 1 and 11 to
            // the right with the following command:
            temp = new Tuple<int, int>[] { Tuple.Create(2,1), Tuple.Create(2, 11) };
            Kernel.GeometryKernel.Copy(temp, out ov);
            Kernel.GeometryKernel.Translate(ov, 0.12, 0, 0);

            Console.WriteLine("New surfaces '%d' and '%d'\n" + " " + ov[0].Item2 + " " + ov[1].Item2);

            // Volumes are the fourth type of elementary entities in Gmsh. In the same way
            // one defines curve loops to build surfaces, one has to define surface loops
            // (i.e. `shells') to build volumes. The following volume does not have holes
            // and thus consists of a single surface loop:
            Kernel.GeometryKernel.AddPoint(0.0, 0.3, 0.12, lc, 100);
            Kernel.GeometryKernel.AddPoint(0.1, 0.3, 0.12, lc, 101);
            Kernel.GeometryKernel.AddPoint(0.1, 0.35, 0.12, lc, 102);

            // We would like to retrieve the coordinates of point 5 to create point 103,
            // so we synchronize the model, and use `getValue()'
            Kernel.GeometryKernel.Synchronize();
            double[] xyz;
            Kernel.Model.GetValue(0, 5, new double[] { }, out xyz);
            Kernel.GeometryKernel.AddPoint(xyz[0], xyz[1], 0.12, lc, 103);

            Kernel.GeometryKernel.AddLine(4, 100, 110);
            Kernel.GeometryKernel.AddLine(3, 101, 111);
            Kernel.GeometryKernel.AddLine(6, 102, 112);
            Kernel.GeometryKernel.AddLine(5, 103, 113);
            Kernel.GeometryKernel.AddLine(103, 100, 114);
            Kernel.GeometryKernel.AddLine(100, 101, 115);
            Kernel.GeometryKernel.AddLine(101, 102, 116);
            Kernel.GeometryKernel.AddLine(102, 103, 117);

            Kernel.GeometryKernel.AddCurveLoop(new[]{ 115, -111, 3, 110}, 118);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 118}, 119);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 111, 116, -112, -7}, 120);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 120}, 121);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 112, 117, -113, -8}, 122);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 122}, 123);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 114, -110, 5, 113}, 124);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 124}, 125);
            Kernel.GeometryKernel.AddCurveLoop(new[]{ 115, 116, 117, 114}, 126);
            Kernel.GeometryKernel.AddPlaneSurface(new[]{ 126}, 127);

            Kernel.GeometryKernel.AddSurfaceLoop(new[]{ 127, 119, 121, 123, 125, 11}, 128);
            Kernel.GeometryKernel.AddVolume(new[]{ 128 }, 129);

            // When a volume can be extruded from a surface, it is usually easier to use
            // the `extrude()' function directly instead of creating all the points,
            // curves and surfaces by hand. For example, the following command extrudes
            // the surface 11 along the z axis and automatically creates a new volume (as
            // well as all the needed points, curves and surfaces). As expected, the
            // function takes a vector of (dim, tag) pairs as input as well as the
            // translation vector, and returns a vector of (dim, tag) pairs as output:
            Tuple<int, int>[] ov2;
            Kernel.GeometryKernel.Extrude(new Tuple<int,int>[]{ ov[1] }, 0, 0, 0.12, out ov2);

            // Mesh sizes associated to geometrical points can be set by passing a vector
            // of (dim, tag) pairs for the corresponding points:
            temp = new Tuple<int, int>[] { Tuple.Create(0, 103), Tuple.Create(0, 105), Tuple.Create(0, 109), Tuple.Create(0, 102),
                                            Tuple.Create(0, 28), Tuple.Create(0, 24), Tuple.Create(0, 6), Tuple.Create(0, 5) };
            Kernel.MeshingKernel.SetSize( temp, lc * 3);

            // We finish by synchronizing the data from the built-in CAD kernel with the
            // Gmsh model:*/
            Kernel.GeometryKernel.Synchronize();

            // We group volumes 129 and 130 in a single physical group with tag `1' and
            // name "The volume":
            Kernel.Model.AddPhysicalGroup(3, new[]{ 129, 130 }, 1);
            Kernel.Model.SetPhysicalName(3, 1, "The volume");

            // We finally generate and save the mesh:
            //IguanaGmsh.Option.SetNumber("Mesh.RandomFactor3D", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.MeshSizeMin", 1);
            //IguanaGmsh.Option.SetNumber("Mesh.MeshSizeMax", 1);
            //IguanaGmsh.Option.SetString("Mesh.AngleToleranceFacetOverlap", "p/#");
            //IguanaGmsh.Option.SetNumber("Mesh.ElementOrder", 2);
            Kernel.MeshingKernel.Generate(3);

            // Note that, if the transformation tools are handy to create complex
            // geometries, it is also sometimes useful to generate the `flat' geometry,
            // with an explicit representation of all the elementary entities.
            //
            // With the built-in CAD kernel, this can be achieved by saving the model in
            // the `Gmsh Unrolled GEO' format:
            //
            // gmsh::write("t2.geo_unrolled");
            //
            // With the OpenCASCADE CAD kernel, unrolling the geometry can be achieved by
            // exporting in the `OpenCASCADE BRep' format:
            //
            // gmsh::write("t2.brep");
            //
            // (OpenCASCADE geometries can also be exported as STEP files.)

            // It is important to note that Gmsh never translates geometry data into a
            // common representation: all the operations on a geometrical entity are
            // performed natively with the associated CAD kernel. Consequently, one cannot
            // export a geometry constructed with the built-in kernel as an OpenCASCADE
            // BRep file; or export an OpenCASCADE model as an Unrolled GEO file.

            Kernel.FinalizeGmsh();

        }
    }
}
