/*
 * <IguanaMesh>
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
using System.Collections.Generic;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Rhino.Geometry;

namespace IguanaMeshGH.ITransform
{
    public class IMoveElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IMoveElementGH class.
        /// </summary>
        public IMoveElementGH()
          : base("iMoveElement", "iMoveElement",
              "Move element",
              "Iguana", "Transform")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Base Iguana mesh.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Element", "e-Key", "Vertex key.", GH_ParamAccess.list);
            pManager.AddVectorParameter("Vector", "T", "Translation vector.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "Iguana mesh.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = new IMesh();
            List<Vector3d> vec = new List<Vector3d>();
            List<int> eKey = new List<int>();

            DA.GetData(0, ref mesh);
            DA.GetDataList(1, eKey);
            DA.GetDataList(2, vec);

            IMesh dM = mesh.DeepCopy();

            ITopologicVertex v;
            IElement e;
            IVector3D T = new IVector3D(vec[0]);
            bool flag = eKey.Count.Equals(vec.Count);

            for (int i = 0; i < eKey.Count; i++)
            {
                e = dM.GetElementWithKey(eKey[i]);
                if (flag) T = new IVector3D(vec[i]);

                foreach (int vK in e.Vertices)
                {
                    v = dM.GetVertexWithKey(vK);
                    dM.SetVertexPosition(vK, v.Position + T);
                }
            }
            dM.UpdateGraphics();

            DA.SetData(0, dM);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.secondary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iMoveElement;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c446ce1e-026b-4e75-a8a8-69b3b051d419"); }
        }
    }
}