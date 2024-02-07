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
using GH_IO.Serialization;
using Grasshopper.Kernel.Types;

namespace Iguana.IguanaMesh.ITypes
{
    public enum IConstraintType { Point, Line, Curve, Surface }

    public struct IConstraint : IGH_Goo
    {
        public IConstraintType ConstraintType { get; private set; }
        public int Dim { get; private set; }
        public Object RhinoGeometry { get; private set; }
        public double Size { get; private set; }
        public int EntityDim { get; private set; }
        public int EntityID { get; private set; }
        public int NumberOfNodes { get; private set; }
        public double CurveDivisionLength { get; private set; }

        public IConstraint(IConstraintType constraintType, int dimension, Object geometry, double size, int entityDimension=-1, int entityTag=-1, int numberOfNodes=1, double divisionLength=1)
        {
            ConstraintType = constraintType;
            Dim = dimension;
            RhinoGeometry = geometry;
            Size = size;
            EntityDim = entityDimension;
            EntityID = entityTag;
            CurveDivisionLength = divisionLength;
            NumberOfNodes = numberOfNodes;
        }

        #region GH_methods
        public bool IsValid
        {
            get => !this.Equals(null);
        }

        public string IsValidWhyNot
        {
            get
            {
                string msg;
                if (this.IsValid) msg = string.Empty;
                else msg = string.Format("Invalid type.", this.TypeName);
                return msg;
            }
        }

        public override string ToString()
        {
            return "IguanaGmshConstraint";
        }

        public string TypeName
        {
            get => "IguanaGmshConstraint";
        }

        public string TypeDescription
        {
            get => ToString();
        }

        public IGH_Goo Duplicate()
        {
            return (IGH_Goo) this.MemberwiseClone();
        }

        public IGH_GooProxy EmitProxy()
        {
            return null;
        }

        public bool CastFrom(object source)
        {
            return false;
        }

        public object ScriptVariable()
        {
            return this;
        }

        public bool Write(GH_IWriter writer)
        {
            return true;
        }

        public bool Read(GH_IReader reader)
        {
            return true;
        }

        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(IConstraint)))
            {
                target = (T)(object)this;
                return true;
            }

            target = default(T);
            return false;
        }
        #endregion
    }
}
