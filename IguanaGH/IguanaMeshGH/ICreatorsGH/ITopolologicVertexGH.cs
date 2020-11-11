﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class ITopolologicVertexGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ITopolologicVertexGH class.
        /// </summary>
        public ITopolologicVertexGH()
          : base("iTopolologicVertex", "iTopologicVertex",
              "Constructa topologic vertex.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Position", "Position", "Vertex position.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Key", "Key", "Unique key identifier. The key should be unique to avoid topological inconsistencies during meshing.", GH_ParamAccess.item);
            pManager.AddNumberParameter("u", "u", "Parametric coordinate in the X axis.", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("v", "v", "Parametric coordinate in the Y axis.", GH_ParamAccess.item,0);
            pManager.AddNumberParameter("w", "w", "Parametric coordinate in the Z axis.", GH_ParamAccess.item,0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iTopologicVertex", "iV", "Iguana vertex.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d pt = new Point3d();
            double u = 0, v = 0, w = 0;
            int key = -1;
            DA.GetData(0, ref pt);
            DA.GetData(1, ref key);
            DA.GetData(2, ref u);
            DA.GetData(2, ref v);
            DA.GetData(2, ref w);

            ITopologicVertex vertex = new ITopologicVertex(pt.X,pt.Y,pt.Z,u,v,w,key);

            DA.SetData(0, vertex);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("6d5a7b66-3389-4e42-8f5b-51b82c7dc085"); }
        }
    }
}