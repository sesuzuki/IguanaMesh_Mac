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
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.ICreators
{
    public class IPrismElementGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IPrismElementGH class.
        /// </summary>
        public IPrismElementGH()
          : base("iPrismElement", "iPrismElement",
              "A three-dimensional prism element.",
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
            pManager.AddIntegerParameter("Fifth", "N5", "Fifth vertex.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Sixth", "N6", "Sixth vertex.", GH_ParamAccess.item);
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
            int N1 = 0, N2 = 0, N3 = 0, N4 = 0, N5 = 0, N6 = 0;
            DA.GetData(0, ref N1);
            DA.GetData(1, ref N2);
            DA.GetData(2, ref N3);
            DA.GetData(3, ref N4);
            DA.GetData(4, ref N5);
            DA.GetData(5, ref N6);

            IPrismElement e = new IPrismElement(N1, N2, N3, N4, N5, N6);

            DA.SetData(0, e);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iPrism;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d5667c19-b47a-4673-8e72-b42d4ef255ff"); }
        }
    }
}