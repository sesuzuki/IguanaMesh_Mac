using System;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;
using static Iguana.IguanaMesh.IKernel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        public static class MeshingKernel_2_0
        {
            /// <summary>
            /// Create a plane surface from a collection of polylines. 
            /// </summary>
            /// <param name="outerboundary"> Outer boundary. </param>
            /// <param name="holes"> Internal holes. </param>
            /// <param name="solver"> Solver settings. </param>
            /// <param name="logInfo"> Return string with log information. </param>
            /// <param name="entities"> Return underlying entities created. </param>
            /// <param name="constraints"> Meshing constraints to embed. </param>
            /// <param name="transfinites"> Tranfinites meshing constraint to embed. </param>
            /// <param name="field"> Mesh field size to use as a background mesh. </param>
            /// <returns> Iguana mesh </returns>
            public static IMesh CreateShellMeshFromPolylines(Curve outerboundary, List<Curve> holes, ISolver2D solver, out string logInfo, out GH_Structure<IEntityInfo> entities, List<IConstraint> constraints = default, List<ITransfinite> transfinites = default, IField field = null)
            {
                if (outerboundary == null || holes.Contains(null))
                {
                    entities = new GH_Structure<IEntityInfo>();
                    logInfo = "Invalid inputs";
                    return null;
                }

                logInfo = Initialize();
                StartLogger();

                PointCloud cloud = new PointCloud();
                int surfaceTag = IGeometryKernel.CreateUnderlyingPlaneSurfaceDividedByCount(outerboundary, holes, solver.Size, 1, ref cloud);
                IGeometryKernel.IBuilder.Synchronize();

                // Set mesh size
                IMeshingKernel.SetMeshSize(solver.Size);

                // Embed constraints
                IGeometryKernel.EmbedConstraintsNew(constraints, ref cloud, 2, surfaceTag);

                //solver options
                solver.Field = field;

                // Iguana mesh construction
                IMesh mesh = IMeshingKernel.GenerateIMesh2D(solver, transfinites);
                entities = IMeshingKernel.GetUnderlyingEntitiesInformation();

                logInfo += GetLogger();
                StopLogger();

                End();

                return mesh;
            }
        }
    }
}

