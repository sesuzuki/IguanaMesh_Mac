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
using Grasshopper.Kernel.Types;
using Iguana.IguanaMesh.ITypes;

namespace IguanaMeshGH.IFields
{
    public class IAnisoIntersectFieldGH : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the IAnisoIntersectFieldGH class.
        /// </summary>
        public IAnisoIntersectFieldGH()
          : base("iAnisotropicIntersectField", "iAnisotropicIntersectField",
              "Compute the intersection of different anisotropic fields to specify the size of mesh elements.",
              "Iguana", "Fields")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iFieldList", "iFieldList", "Fields to evaluate.", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("iField", "iField", "Field for mesh generation.", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IField> fieldList = new List<IField>();
            foreach (IGH_Goo data in Params.Input[0].VolatileData.AllData(true))
            {
                IField f;
                if (data.CastTo(out f))
                {
                    fieldList.Add(f);
                }
            }

            IField.IntersectAniso field = new IField.IntersectAniso();
            field.FieldsList = fieldList;

            DA.SetData(0, field);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iAnisoField;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("35ba84f1-8726-4a99-889d-e177e6131a55"); }
        }
    }
}