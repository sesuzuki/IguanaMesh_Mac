﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes.IElements;
using Rhino.Geometry;

namespace IguanaGH.IguanaMeshGH.ICreatorsGH
{
    public class IQuadrangularFaceGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IQuadrangularFaceGH class.
        /// </summary>
        public IQuadrangularFaceGH()
          : base("iQuadrangleElement", "iQuadrangleElement",
              "A two-dimensional quadrangular element.",
              "Iguana", "Creators")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("First", "N1", "First vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Second", "N2", "Second vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Third", "N3", "Third vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Fourth", "N4", "Fourth vertex.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iElement", "iE", "Iguana element", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int A = 0, B = 0, C = 0, D=0;
            DA.GetData(0, ref A);
            DA.GetData(1, ref B);
            DA.GetData(2, ref C);
            DA.GetData(3, ref D);

            ISurfaceElement e = new ISurfaceElement(A, B, C, D);

            DA.SetData(0, e);
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
            get { return new Guid("57f3cd1e-b000-4b5e-9e1c-4a233f94fac1"); }
        }
    }
}