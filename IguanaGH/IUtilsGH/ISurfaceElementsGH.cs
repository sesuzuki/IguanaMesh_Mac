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
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;

namespace IguanaMeshGH.IUtils
{
    public class I2DElementsAsSurfacesGH : GH_Component
    {
        bool _massivePreview = false;

        /// <summary>
        /// Initializes a new instance of the I2DElementsAsSurfacesGH class.
        /// </summary>
        public I2DElementsAsSurfacesGH()
          : base("iSurfaceElements", "iSrfElements",
              "Retrieve two-dimensional elements as surfaces.",
              "Iguana", "Utils")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("iMesh", "iM", "The Iguana mesh to extract two-dimesional elements.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Surfaces.", "S", "Two-dimensional elements as surfaces", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IMesh mesh = null;
            DA.GetData(0, ref mesh);

            int eCount = mesh.ElementsCount;
            if (eCount > 1e4 && _massivePreview == false) AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "A large number of elements was detected. Enable 'Massive Display' for continuing but this might slow down the entire preview process.");
            else
            {
                List<Surface> surfaces = IRhinoGeometry.Get2DElementsAsSurfaces(mesh);

                DA.SetDataList(0, surfaces);
            }
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.tertiary; }
        }

        public bool MassivePreview
        {
            get { return _massivePreview; }
            set
            {
                _massivePreview = value;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean("Massive Preview", MassivePreview);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            bool refFlag = false;
            if (reader.TryGetBoolean("Massive Preview", ref refFlag))
            {
                MassivePreview = refFlag;
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            ToolStripMenuItem item = Menu_AppendItem(menu, "Massive Preview", Menu_MassivePreviewClicked, true, MassivePreview);
            item.ToolTipText = "CAUTION: When checked, disable the imposed limit of volume elements that can be represented as Brep.\nThis might slow down the entire preview process.";
        }

        private void Menu_MassivePreviewClicked(object sender, EventArgs e)
        {
            RecordUndoEvent("Massive Preview");
            MassivePreview = !MassivePreview;
            ExpireSolution(true);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.iSurfaceElement;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d78979e7-df48-48ce-bef0-45b38da43d8a"); }
        }
    }
}