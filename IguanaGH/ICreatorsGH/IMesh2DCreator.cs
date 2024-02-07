using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Iguana.IguanaMesh;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.ICreatorsGH
{
    public class IMesh2DCreator : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public IMesh2DCreator()
          : base("iMesher2D", "iMesher2D",
              "Create a two-dimensional mesh.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Outer boundary", "Outer", "External polygonal boundary", GH_ParamAccess.item);
            pManager.AddCurveParameter("Inner boundaries", "Inner", "Internal holes", GH_ParamAccess.list);
            pManager.AddGenericParameter("iMeshField", "iField", "Field to specify the size of the mesh elements.", GH_ParamAccess.item);
            pManager.AddGenericParameter("iConstraints", "iConstraints", "Geometric constraints for mesh generation.", GH_ParamAccess.list);
            pManager.AddGenericParameter("iTransfinites", "iTransfinites", "Transfinite constraints for mesh generation", GH_ParamAccess.list);
            pManager.AddGenericParameter("iSettings", "iSettings", "Shell mesh settings.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana surface mesh.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Entities", "Entities", "Information about the underlying entities used for meshing.", GH_ParamAccess.tree);
            pManager.AddTextParameter("Info", "Info", "Log information about the meshing process.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve _outer = null;
            List<Curve> _inner = new List<Curve>();
            List<IConstraint> constraints = new List<IConstraint>();
            List<ITransfinite> transfinites = new List<ITransfinite>();
            ISolver2D solver = new ISolver2D();
            IField field = null;

            //Retrieve vertices and elements
            if (!DA.GetData(0, ref _outer)) return;
            DA.GetDataList(1, _inner);
            DA.GetData(2, ref field);
            DA.GetDataList(3, constraints);
            DA.GetDataList(4, transfinites);
            DA.GetData(5, ref solver);

            if (_outer == null || !_outer.IsPolyline())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The outer boundary should be planar closed polyline.");
                return;
            }

            if (_inner.Contains(null))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A null curve was found.");
                return;
            }

            if (_inner.Count > 0 && !_inner.TrueForAll(crv => crv.IsPolyline()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Inner boundaries should be planar closed polylines.");
                return;
            }

            if (constraints == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A null constraint was found.");
                return;
            }

            if (transfinites == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "A null transfinite was found.");
                return;
            }


            string logInfo;
            GH_Structure<IEntityInfo> entities;
            IMesh mesh = IKernel.MeshingKernel_2_0.CreateShellMeshFromPolylines(_outer, _inner, solver, out logInfo, out entities, constraints, transfinites, field);


            DA.SetData(0, mesh);
            DA.SetDataTree(1, entities);
            DA.SetData(2, logInfo);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("624e21f0-65d4-4fca-98b2-ae3ea58ae915"); }
        }
    }
}