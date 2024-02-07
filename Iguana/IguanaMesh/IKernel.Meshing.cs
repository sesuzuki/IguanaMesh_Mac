﻿using System;
using Iguana.IguanaMesh.IUtils;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Iguana.IguanaMesh
{
    public static partial class IKernel
    {
        public static class Meshing
        {
            #region Gmsh Methods 

            /// <summary>
            /// Generate a mesh of the current model, up to dimension `dim' (0, 1, 2 or 3).
            /// </summary>
            /// <param name="dim"></param>
            public static void Generate(int dim)
            {
                IWrap.GmshModelMeshGenerate(dim, ref _ierr);
            }

            /// <summary>
            /// Get the nodes classified on the entity of dimension `dim'.  
            /// </summary>
            /// <param name="nodeTags_out"> `nodeTags_out' contains the node tags (their unique, strictly positive identification numbers).  </param>
            /// <param name="coord_out"> `coord' is a two-dimensional array that contains the x, y, z coordinates of the nodes. </param>
            /// <param name="parametricCoord_out"> If `dim' >= 0, `parametricCoord' contains the parametric coordinates([u1, u2, ...] or [u1, v1, u2, ...]) of the nodes, if available. </param>
            /// <param name="dim"/> If `dim' is negative (default), get all the nodes in the mesh. </param>
            public static void GetNodes(out long[] nodeTags_out, out double[][] coord_out, out double[][] parametricCoord_out, int dim = -1, int tag = -1)
            {
                IntPtr nodeTags, coord, parametricCoord;
                long nodeTags_Number, coord_Number, parametricCoord_Number;
                IWrap.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);

                nodeTags_out = new long[nodeTags_Number];
                coord_out = new double[nodeTags_Number][];
                parametricCoord_out = new double[0][];

                // Tags
                if (nodeTags_Number > 0)
                {
                    // Coordinates
                    var xyz = new double[coord_Number];
                    Marshal.Copy(coord, xyz, 0, (int)coord_Number);
                    // Keys
                    var keys = new long[nodeTags_Number];
                    Marshal.Copy(nodeTags, keys, 0, (int)nodeTags_Number);

                    for (int i = 0; i < nodeTags_Number; i++)
                    {
                        coord_out[i] = new double[] { xyz[i * 3], xyz[i * 3 + 1], xyz[i * 3 + 2] };
                        nodeTags_out[i] = (int)keys[i];
                    }

                    int paramCount = (int)(parametricCoord_Number / dim);
                    var uvw = new double[parametricCoord_Number];
                    if (paramCount > 0)
                    {
                        parametricCoord_out = new double[paramCount][];
                        Marshal.Copy(parametricCoord, uvw, 0, (int)parametricCoord_Number);
                        for (int i = 0; i < paramCount; i++)
                        {
                            if (dim == 1) parametricCoord_out[i] = new double[] { uvw[i * dim] };
                            else if (dim == 2) parametricCoord_out[i] = new double[] { uvw[i * dim], uvw[i * dim + 1] };
                            else if (dim == 3) parametricCoord_out[i] = new double[] { uvw[i * dim], uvw[i * dim + 1], uvw[i * dim + 2] };
                        }
                    }
                }

                // Delete unmanaged allocated memory
                IWrap.GmshFree(nodeTags);
                IWrap.GmshFree(coord);
                IWrap.GmshFree(parametricCoord);
            }

            public static void GetCenter(int dim, int tag, out double[] center)
            {
                IntPtr nodeTags, coord, parametricCoord;
                long nodeTags_Number, coord_Number, parametricCoord_Number;
                IWrap.GmshModelMeshGetNodes(out nodeTags, out nodeTags_Number, out coord, out coord_Number, out parametricCoord, out parametricCoord_Number, dim, tag, Convert.ToInt32(true), Convert.ToInt32(true), ref _ierr);

                center = new double[3];
                // Tags
                if (nodeTags_Number > 0)
                {
                    // Coordinates
                    var xyz = new double[coord_Number];
                    Marshal.Copy(coord, xyz, 0, (int)coord_Number);

                    for (int i = 0; i < nodeTags_Number; i++)
                    {
                        center[0] += xyz[i * 3];
                        center[1] += xyz[i * 3 + 1];
                        center[2] += xyz[i * 3 + 2];
                    }

                    center[0] /= nodeTags_Number;
                    center[1] /= nodeTags_Number;
                    center[2] /= nodeTags_Number;
                }

                // Delete unmanaged allocated memory
                IWrap.GmshFree(nodeTags);
                IWrap.GmshFree(coord);
                IWrap.GmshFree(parametricCoord);
            }

            /// <summary>
            /// Get the elements classified on the entity of dimension `dim'.
            /// `elementTypes' contains the MSH types of the elements (e.g. `2' for 3-node triangles: see `getElementProperties' to obtain the properties for a given element type). 
            /// `elementTags' is a vector of the same length as `elementTypes'; each entry is a vector containing the tags (unique, strictly positive identifiers) of the elements of the corresponding type.
            /// `nodeTags' is also a vector of the same length as `elementTypes'; each entry is a vector of length equal to the number of elements of the given type times the number N of nodes for this type of element, 
            /// that contains the node tags of all the elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1, ...]. 
            /// </summary>
            /// <param name="elementTypes"></param>
            /// <param name="elementTypes_n"></param>
            /// <param name="elementTags"></param>
            /// <param name="elementTags_n"></param>
            /// <param name="elementTags_nn"></param>
            /// <param name="nodeTags"></param>
            /// <param name="nodeTags_n"></param>
            /// <param name="nodeTags_nn"></param>
            /// <param name="dim"> If `dim' is negative (default), get all the elements in the mesh.  </param>
            public static void GetElements(out int[] elementTypes_out, out long[][] elementTags_out, out long[][] nodeTags_out, int dim = -1, int tag = -1)
            {
                IntPtr elementTypes, elementTags, nodeTags, elementTags_n, nodeTags_n;
                long elementTypes_Number, elementTags_NNumber, nodeTags_NNumber;

                IWrap.GmshModelMeshGetElements(out elementTypes, out elementTypes_Number, out elementTags, out elementTags_n, out elementTags_NNumber, out nodeTags, out nodeTags_n, out nodeTags_NNumber, dim, tag, ref _ierr);

                elementTypes_out = new int[elementTypes_Number];
                var eTags_n = new long[elementTags_NNumber];
                var nTags_n = new long[nodeTags_NNumber];

                Marshal.Copy(elementTypes, elementTypes_out, 0, (int)elementTypes_Number);
                Marshal.Copy(elementTags_n, eTags_n, 0, (int)elementTags_NNumber);
                Marshal.Copy(nodeTags_n, nTags_n, 0, (int)nodeTags_NNumber);

                var nTags_ptr = new IntPtr[nodeTags_NNumber];
                var eTags_ptr = new IntPtr[elementTags_NNumber];

                Marshal.Copy(nodeTags, nTags_ptr, 0, (int)nodeTags_NNumber);
                Marshal.Copy(elementTags, eTags_ptr, 0, (int)elementTags_NNumber);

                elementTags_out = new long[elementTags_NNumber][];
                nodeTags_out = new long[nodeTags_NNumber][];

                for (int i = 0; i < elementTags_NNumber; i++)
                {
                    // Initializing containers
                    elementTags_out[i] = new long[eTags_n[i]];
                    nodeTags_out[i] = new long[nTags_n[i]];

                    // Marshalling
                    Marshal.Copy(eTags_ptr[i], elementTags_out[i], 0, (int)eTags_n[i]);
                    Marshal.Copy(nTags_ptr[i], nodeTags_out[i], 0, (int)nTags_n[i]);
                }

                // Delete unmanaged allocated memory
                IWrap.GmshFree(elementTypes);
                IWrap.GmshFree(elementTags);
                IWrap.GmshFree(nodeTags);
                IWrap.GmshFree(elementTags_n);
                IWrap.GmshFree(nodeTags_n);

                for (int i = 0; i < nTags_ptr.Length; i++)
                {
                    IWrap.GmshFree(nTags_ptr[i]);
                }
                for (int i = 0; i < eTags_ptr.Length; i++)
                {
                    IWrap.GmshFree(eTags_ptr[i]);
                }
            }

            /// <summary>
            /// Optimize the mesh of the current model using.
            /// <param name="method"> `method' (empty for default tetrahedral mesh optimizer, "Netgen" for Netgen optimizer, "HighOrder" for
            /// direct high-order mesh optimizer, "HighOrderElastic" for high-order elastic smoother, "HighOrderFastCurving" for fast curving algorithm,
            /// "Laplace2D" for Laplace smoothing, "Relocate2D" and "Relocate3D" for node relocation)</param>
            /// <param name="niter"> Number of Iterations. Default is 5. </param>
            public static void Optimize(string method = default, int niter = 5)
            {
                if (method == default) method = "";
                IKernel.IWrap.GmshModelMeshOptimize(method, -1, niter, null, IntPtr.Zero, ref _ierr);
            }

            /// <summary>
            /// Remove duplicate nodes in the mesh of the current model.
            /// </summary>
            public static void RemoveDuplicateNodes()
            {
                IKernel.IWrap.GmshModelMeshRemoveDuplicateNodes(ref _ierr);
            }

            /// <summary>
            /// Split (into two triangles) all quadrangles in surface `tag' whose quality is lower than `quality'. 
            /// </summary>
            /// <param name="quality"> Quality of the surface. </param>
            /// <param name="tag"> If `tag' < 0, split quadrangles in all surfaces. </param>
            public static void SplitQuadrangles(double quality, int tag = -1)
            {
                IKernel.IWrap.GmshModelMeshSplitQuadrangles(quality, tag, ref _ierr);
            }

            /// <summary>
            /// Set a mesh size constraint on the model entities `dimTags'. Currently only entities of dimension 0 (points) are handled.
            /// </summary>
            /// <param name="dimTags"></param>
            /// <param name="size"></param>
            public static void SetSize(Tuple<int, int>[] dimTags, double size)
            {
                int[] dimTags_flatten = IHelpers.FlattenIntTupleArray(dimTags);
                IWrap.GmshModelMeshSetSize(dimTags_flatten, dimTags_flatten.LongLength, size, ref _ierr);
            }

            /// <summary>
            /// Set mesh size constraints at the given parametric points `parametricCoord'
            /// on the model entity of dimension `dim' and tag `tag'. Currently only
            /// entities of dimension 1 (lines) are handled.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="parametricCoord"></param>
            /// <param name="sizes"></param>
            public static void SetSizeAtParametricPoints(int dim, int tag, double[] parametricCoord, double[] sizes)
            {
                IWrap.GmshModelMeshSetSizeAtParametricPoints(dim, tag, parametricCoord, parametricCoord.LongLength, sizes, sizes.LongLength, ref _ierr);
            }

            /// <summary>
            /// Set a transfinite meshing constraint on the curve `tag', with `numNodes'
            /// nodes distributed according to `meshType' and `coef'. Currently supported
            /// types are "Progression" (geometrical progression with power `coef') and
            /// "Bump" (refinement toward both extremities of the curve).
            /// </summary>
            public static void SetTransfiniteCurve(int tag, int numNodes, string meshType = "Progression", double coef = 1)
            {
                IWrap.GmshModelMeshSetTransfiniteCurve(tag, numNodes, meshType, coef, ref _ierr);
            }

            /// <summary>
            /// Set a transfinite meshing constraint on the surface `tag'. `arrangement'
            /// describes the arrangement of the triangles when the surface is not flagged
            /// as recombined: currently supported values are "Left", "Right",
            /// "AlternateLeft" and "AlternateRight". `cornerTags' can be used to specify
            /// the(3 or 4) corners of the transfinite interpolation explicitly;
            /// specifying the corners explicitly is mandatory if the surface has more than
            /// 3 or 4 points on its boundary.
            /// </summary>
            public static void SetTransfiniteSurface(int tag, string arrangement = "Left", int[] cornerTags = default)
            {
                if (cornerTags == default) cornerTags = new int[0];
                IWrap.GmshModelMeshSetTransfiniteSurface(tag, arrangement, cornerTags, cornerTags.LongLength, ref _ierr);
            }

            /// <summary>
            /// Set a transfinite meshing constraint on the surface `tag'. `cornerTags' can be used to specify the(6 or 8) corners of the transfinite interpolation explicitly.
            /// </summary>
            /// <param name="tag"></param>
            /// <param name="cornerTags"></param>
            public static void SetTransfiniteVolume(int tag, int[] cornerTags = default)
            {
                if (cornerTags == default) cornerTags = new int[] { };
                IWrap.GmshModelMeshSetTransfiniteVolume(tag, cornerTags, cornerTags.LongLength, ref _ierr);
            }

            /// <summary>
            /// (Only available with gmsh_4.7) Set transfinite meshing constraints on the model entities in `dimTag'.
            /// Transfinite meshing constraints are added to the curves of the quadrangular
            /// surfaces and to the faces of 6-sided volumes.Quadragular faces with a
            /// corner angle superior to `cornerAngle' (in radians) are ignored. The number
            /// of points is automatically determined from the sizing constraints. If
            /// `dimTag' is empty, the constraints are applied to all entities in the
            /// model.If `recombine' is true, the recombine flag is automatically set on
            /// the transfinite surfaces.
            /// </summary>
            public static void SetAutomaticTransfinite(Tuple<int, int>[] dimTags, double cornerAngle = 90, bool recombine = false)
            {
                var data = IHelpers.FlattenIntTupleArray(dimTags);
                IWrap.GmshModelMeshSetTransfiniteAutomatic(data, data.LongLength, cornerAngle * Math.PI / 180, Convert.ToInt32(recombine), ref _ierr);
            }

            /// <summary>
            /// Set a recombination meshing constraint on the model entity of dimension
            /// `dim' and tag `tag'. Currently only entities of dimension 2 (to recombine
            /// triangles into quadrangles) are supported.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="angle"> angle' specifies the threshold angle for the simple recombination algorithm. </param>
            public static void SetRecombine(int dim, int tag, double angle = 0.01)
            {
                IWrap.GmshModelMeshSetRecombine(dim, tag, angle, ref _ierr);
            }

            /// <summary>
            /// Set a smoothing meshing constraint on the model entity of dimension `dim' and tag `tag'. `val' iterations of a Laplace smoother are applied.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="val"></param>
            public static void SetSmoothing(int dim, int tag, int val)
            {
                IWrap.GmshModelMeshSetSmoothing(dim, tag, val, ref _ierr);
            }

            /// <summary>
            /// Set a reverse meshing constraint on the model entity of dimension `dim' and
            /// tag `tag'. If `val' is true, the mesh orientation will be reversed with
            /// respect to the natural mesh orientation(i.e.the orientation consistent with the orientation of the geometry). If `val' is false, the mesh is left as-is.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="val"></param>
            public static void SetReverse(int dim, int tag, bool val)
            {
                IWrap.GmshModelMeshSetReverse(dim, tag, Convert.ToInt32(val), ref _ierr);
            }

            /// <summary>
            /// Set a reverse meshing constraint on the model entity of dimension `dim' and
            /// tag `tag'. If `val' is true, the mesh orientation will be reversed with
            /// respect to the natural mesh orientation(i.e.the orientation consistent with the orientation of the geometry). If `val' is false, the mesh is left as-is.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="val"></param>
            public static void Reverse(int dim, int tag)
            {
                IWrap.GmshModelMeshReverse(dim, tag, ref _ierr);
            }

            /// <summary>
            /// Set the meshing algorithm on the model entity of dimension `dim' and tag
            /// `tag'. Currently only supported for `dim' == 2.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="val"></param>
            public static void SetAlgorithm(int dim, int tag, int val)
            {
                IWrap.GmshModelMeshSetAlgorithm(dim, tag, val, ref _ierr);
            }

            /// <summary>
            /// Force the mesh size to be extended from the boundary, or not, for the model
            /// entity of dimension `dim' and tag `tag'. 
            /// Currently only supported for `dim' == 2.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tag"></param>
            /// <param name="val"></param>
            public static void SetSizeFromBoundary(int dim, int tag, bool val)
            {
                IWrap.GmshModelMeshSetSizeFromBoundary(dim, tag, Convert.ToInt32(val), ref _ierr);
            }

            /// <summary>
            /// Set a compound meshing constraint on the model entities of dimension `dim'
            /// and tags `tags'. During meshing, compound entities are treated as a single
            /// discrete entity, which is automatically reparametrized.
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tags"></param>
            public static void SetCompound(int dim, int[] tags)
            {
                IWrap.GmshModelMeshSetCompound(dim, tags, tags.LongLength, ref _ierr);
            }

            /// <summary>
            /// Set meshing constraints on the bounding surfaces of the volume of tag `tag'
            /// so that all surfaces are oriented with outward pointing normals.Currently
            /// only available with the OpenCASCADE kernel, as it relies on the STL triangulation.
            /// </summary>
            /// <param name="tag"></param>
            public static void SetOutwardOrientation(int tag)
            {
                IWrap.GmshModelMeshSetOutwardOrientation(tag, ref _ierr);
            }

            /// <summary>
            /// Renumber the node tags in a continuous sequence.
            /// </summary>
            public static void RenumberNodes()
            {
                IWrap.GmshModelMeshRenumberNodes(ref _ierr);
            }

            /// <summary>
            /// Renumber the element tags in a continuous sequence.
            /// </summary>
            public static void RenumberElements()
            {
                IWrap.GmshModelMeshRenumberElements(ref _ierr);
            }

            /// <summary>
            /// Set the meshes of the entities of dimension `dim' and tag `tags' as
            /// periodic copies of the meshes of entities `tagsMaster', using the affine
            /// transformation specified in `affineTransformation' (16 entries of a 4x4
            /// matrix, by row). If used after meshing, generate the periodic node
            /// correspondence information assuming the meshes of entities `tags'
            /// effectively match the meshes of entities `tagsMaster' (useful for
            /// structured and extruded meshes). Currently only available for @code{dim} == 1 and @code { dim } == 2. 
            /// </summary>
            /// <param name="dim"></param>
            /// <param name="tags"></param>
            /// <param name="tagsMaster"></param>
            /// <param name="affineTransform"></param>
            public static void SetPeriodic(int dim, int[] tags, int[] tagsMaster, double[] affineTransform)
            {
                IWrap.GmshModelMeshSetPeriodic(dim, tags, tags.LongLength, tagsMaster, tagsMaster.LongLength, affineTransform, affineTransform.LongLength, ref _ierr);
            }

            /// <summary>
            /// Classify ("color") the surface mesh based on the angle threshold `angle'
            /// (in radians), and create new discrete surfaces, curves and points
            /// accordingly.If `boundary' is set, also create discrete curves on the 
            /// boundary if the surface is open.If `forReparametrization' is set, create
            /// edges and surfaces that can be reparametrized using a single map.If
            /// `curveAngle' is less than Pi, also force curves to be split according to
            /// `curveAngle'.
            /// </summary>
            /// <param name="angle"></param>
            /// <param name="boundary"></param>
            /// <param name="forReparametrization"></param>
            /// <param name="curveAngle"></param>
            /// <param name="exportDiscrete">If `exportDiscrete' is set, clear any built-in CAD kernel entities and export the discrete entities in the built-in CAD kernel.</param>
            public static void ClassifySurfaces(double angle, bool boundary, bool forReparametrization, double curveAngle, bool exportDiscrete = true)
            {
                IWrap.GmshModelMeshClassifySurfaces(angle, Convert.ToInt32(boundary), Convert.ToInt32(forReparametrization), curveAngle, exportDiscrete ? 1 : 0, ref _ierr);
            }

            /// <summary>
            /// Create a geometry for the discrete entities `dimTags' (represented solely
            /// by a mesh, without an underlying CAD description), i.e.create a
            /// parametrization for discrete curves and surfaces, assuming that each can be
            /// parametrized with a single map.If `dimTags' is empty, create a geometry
            /// for all the discrete entities.
            /// </summary>
            /// <param name="dimTags"></param>
            public static void CreateGeometry(long[] dimTags = default)
            {
                if (dimTags == default) dimTags = new long[0];
                IWrap.GmshModelMeshCreateGeometry(dimTags, dimTags.LongLength, ref _ierr);
            }

            /// <summary>
            /// Create a boundary representation from the mesh if the model does not have
            /// one(e.g.when imported from mesh file formats with no BRep representation
            /// of the underlying model). If `makeSimplyConnected' is set, enforce simply 
            /// connected discrete surfaces and volumes.If `exportDiscrete' is set, clear
            /// any built-in CAD kernel entities and export the discrete entities in the
            /// built-in CAD kernel.
            /// </summary>
            /// <param name="makeSimplyConnected"></param>
            /// <param name="exportDiscrete"></param>
            public static void CreateTopology(bool makeSimplyConnected, bool exportDiscrete)
            {
                IWrap.GmshModelMeshCreateTopology(Convert.ToInt32(makeSimplyConnected), Convert.ToInt32(exportDiscrete), ref _ierr);
            }

            /// <summary>
            /// Compute a basis representation for homology spaces after a mesh has been
            /// generated.The computation domain is given in a list of physical group tags
            /// `domainTags'; if empty, the whole mesh is the domain. The computation
            /// subdomain for relative homology computation is given in a list of physical
            /// group tags `subdomainTags'; if empty, absolute homology is computed. The
            /// dimensions homology bases to be computed are given in the list `dim'; if
            /// empty, all bases are computed.Resulting basis representation chains are
            /// stored as physical groups in the mesh.
            /// </summary>
            /// <param name="domainTags"></param>
            /// <param name="subdomainTags"></param>
            /// <param name="dims"></param>
            public static void ComputeHomology(int[] domainTags, int[] subdomainTags, int[] dims)
            {
                IWrap.GmshModelMeshAddHomologyRequest(domainTags, domainTags.LongLength, subdomainTags, subdomainTags.LongLength, dims, dims.LongLength, ref _ierr);

                IntPtr dimTagsPtr;
                int dimTags_n;
                IWrap.GmshModelMeshComputeHomology(out dimTagsPtr, out dimTags_n, ref _ierr);

                Tuple<int, int>[] dimTags = null;

                // Tags
                if (dimTags_n > 0)
                {
                    var temp = new int[dimTags_n];
                    Marshal.Copy(dimTagsPtr, temp, 0, (int)dimTags_n);

                    dimTags = IHelpers.GraftIntTupleArray(temp);
                }

                // Delete unmanaged allocated memory
                IWrap.GmshFree(dimTagsPtr);
            }

            /// <summary>
            /// Compute a cross field for the current mesh. The function creates 3 views: the H function, the Theta function and cross directions.
            /// Return the tags of the views
            /// </summary>
            /// <param name="viewTags_out"></param>
            public static void MeshComputeCrossField(out int[] viewTags_out)
            {
                IntPtr viewTags;
                long viewTags_n;
                IWrap.GmshModelMeshComputeCrossField(out viewTags, out viewTags_n, ref _ierr);

                viewTags_out = null;
                if (viewTags_n > 0)
                {
                    viewTags_out = new int[viewTags_n];
                    Marshal.Copy(viewTags, viewTags_out, 0, (int)viewTags_n);
                }
                IWrap.GmshFree(viewTags);
            }

            /// <summary>
            /// Refine the mesh of the current model by uniformly splitting the elements.
            /// </summary>
            public static void Refine()
            {
                IWrap.GmshModelMeshRefine(ref _ierr);
            }

            /// <summary>
            /// Recombine the mesh of the current model.
            /// </summary>
            public static void Recombine()
            {
                IWrap.GmshModelMeshRecombine(ref _ierr);
            }

            /// <summary>
            /// Embed the model entities of dimension `dim' and tags `tags' in the (`inDim', `inTag') model entity.
            /// The embedded entities should not be part of the boundary of the entity `inTag', whose mesh will conform to the mesh of the embedded entities.
            /// </summary>
            /// <param name="dim"> The dimension `dim' can 0, 1 or 2 and must be strictly smaller than `inDim', which must be either 2 or 3. </param>
            /// <param name="tags"></param>
            /// <param name="inDim"></param>
            /// <param name="inTag"></param>
            public static void Embed(int dim, int[] tags, int inDim, int inTag)
            {
                IWrap.GmshModelMeshEmbed(dim, tags, tags.LongLength, inDim, inTag, ref _ierr);
            }

            /// <summary>
            /// Remove embedded entities from the model entities `dimTags'. if `dim' is >= 0, only remove embedded entities of the given dimension (e.g.embedded points if `dim' == 0).
            /// </summary>
            public static void RemoveEmbedded(Tuple<int, int>[] dimTags, int dim)
            {
                var arr = IHelpers.FlattenIntTupleArray(dimTags);
                IWrap.GmshModelMeshRemoveEmbedded(arr, arr.LongLength, dim, ref _ierr);
            }

            /// <summary>
            /// Reclassify all nodes on their associated model entity, based on the
            /// elements.Can be used when importing nodes in bulk(e.g.by associating
            /// them all to a single volume), to reclassify them correctly on model
            /// surfaces, curves, etc.after the elements have been set.
            /// </summary>
            public static void ReclassifyNodes()
            {
                IWrap.GmshModelMeshReclassifyNodes(ref _ierr);
            }

            /// <summary>
            /// Add nodes classified on the model entity of dimension `dim' and tag `tag'.
            /// `nodeTags' contains the node tags (their unique, strictly positive
            /// identification numbers). `coord' is a vector of length 3 times the length
            /// of `nodeTags' that contains the x, y, z coordinates of the nodes,
            /// concatenated: [n1x, n1y, n1z, n2x, ...]. The optional `parametricCoord'
            /// vector contains the parametric coordinates of the nodes, if any.The length
            /// of `parametricCoord' can be 0 or `dim' times the length of `nodeTags'. If
            /// the `nodeTags' vector is empty, new tags are automatically assigned to the
            /// nodes.
            /// </summary>
            public static void AddNodes(int dim, int tag, long[] nodeTags, double[] coord, double[] parametricCoord = default)
            {
                if (parametricCoord == default) parametricCoord = new double[0];
                IWrap.GmshModelMeshAddNodes(dim, tag, nodeTags, nodeTags.LongLength, coord, coord.LongLength, parametricCoord, parametricCoord.LongLength, ref _ierr);
            }

            /// <summary>
            /// Add nodes classified on the model entity of dimension `dim' and tag `tag'.
            /// `nodeTags' contains the node tags (their unique, strictly positive
            /// identification numbers). `coord' is a vector of length 3 times the length
            /// of `nodeTags' that contains the x, y, z coordinates of the nodes,
            /// concatenated: [n1x, n1y, n1z, n2x, ...]. The optional `parametricCoord'
            /// vector contains the parametric coordinates of the nodes, if any.The length
            /// of `parametricCoord' can be 0 or `dim' times the length of `nodeTags'. If
            /// the `nodeTags' vector is empty, new tags are automatically assigned to the
            /// nodes.
            /// </summary>
            public static void AddNodes(int tag, MeshVertexList vertices, MeshTextureCoordinateList textures)
            {
                long[] nodeTags;
                double[] coord;
                IHelpers.ParseRhinoVertices(vertices, out coord, out nodeTags);
                double[] parametricCoord;
                IHelpers.ParseRhinoTextures(textures, out parametricCoord);

                AddNodes(2, tag, nodeTags, coord, parametricCoord);
            }

            /// <summary>
            /// Add elements classified on the entity of dimension `dim' and tag `tag'.
            /// `types' contains the MSH types of the elements (e.g. `2' for 3-node
            /// triangles: see the Gmsh reference manual). `elementTags' is a vector of the
            /// same length as `types'; each entry is a vector containing the tags (unique,
            /// strictly positive identifiers) of the elements of the corresponding type.
            /// `nodeTags' is also a vector of the same length as `types'; each entry is a
            /// vector of length equal to the number of elements of the given type times
            /// the number N of nodes per element, that contains the node tags of all the
            /// elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1,...].
            /// </summary>
            public static void AddElements(int dim, int tag, int[] elementTypes, long[][] elementTags, long[][] nodeTags)
            {
                long[] elementTags_flatten = IHelpers.FlattenLongArray(elementTags);
                long[] nodesTags_flatten = IHelpers.FlattenLongArray(nodeTags);

                IWrap.GmshModelMeshAddElements(dim, tag, elementTypes, elementTypes.LongLength, elementTags_flatten, elementTypes.LongLength, elementTags_flatten.LongLength, nodesTags_flatten, elementTypes.LongLength, nodesTags_flatten.LongLength, ref _ierr);
            }

            /// <summary>
            /// Add elements classified on the entity of dimension `dim' and tag `tag'.
            /// `types' contains the MSH types of the elements (e.g. `2' for 3-node
            /// triangles: see the Gmsh reference manual). `elementTags' is a vector of the
            /// same length as `types'; each entry is a vector containing the tags (unique,
            /// strictly positive identifiers) of the elements of the corresponding type.
            /// `nodeTags' is also a vector of the same length as `types'; each entry is a
            /// vector of length equal to the number of elements of the given type times
            /// the number N of nodes per element, that contains the node tags of all the
            /// elements of the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1,...].
            /// </summary>
            //public static void AddElements(int tag, MeshFaceList faces)
            //{
            //    MeshFace f;
            //    List<long> tagsT = new List<long>();
            //    List<long> nodesT = new List<long>();
            //    List<long> tagsQ = new List<long>();
            //    List<long> nodesQ = new List<long>();

            //    for (int i = 0; i < faces.Count; i++)
            //    {
            //        f = faces[i];
            //        if (f.IsTriangle)
            //        {
            //            tagsT.Add(i);
            //            nodesT.AddRange(new long[] { f.A, f.B, f.C });
            //        }
            //        else if (f.IsQuad)
            //        {
            //            tagsQ.Add(i);
            //            nodesQ.AddRange(new long[] { f.A, f.B, f.C, f.D });
            //        }
            //    }

            //    int[] elementTypes = new int[0];
            //    if (tagsT.Count > 0 && tagsQ.Count > 0) elementTypes = new int[] { 2, 3 };
            //    else if (tagsT.Count > 0 && tagsQ.Count == 0) elementTypes = new int[] { 2 };
            //    else if (tagsT.Count == 0 && tagsQ.Count > 0) elementTypes = new int[] { 3 };

            //    long[] elementTags_flatten = tagsT.Concat(tagsQ).ToArray();
            //    long[] nodesTags_flatten = nodesT.Concat(nodesQ).ToArray();

            //    IWrap.GmshModelMeshAddElements(2, tag, elementTypes, elementTypes.LongLength, elementTags_flatten, elementTypes.LongLength, elementTags_flatten.LongLength, nodesTags_flatten, elementTypes.LongLength, nodesTags_flatten.LongLength, ref _ierr);
        }

        /// <summary>
        /// Add elements of type `elementType' classified on the entity of tag `tag'.
        /// `elementTags' contains the tags (unique, strictly positive identifiers) of
        /// the elements of the corresponding type. `nodeTags' is a vector of length
        /// equal to the number of elements times the number N of nodes per element,
        /// that contains the node tags of all the elements, concatenated: [e1n1, e1n2,
        /// ..., e1nN, e2n1, ...]. If the `elementTag' vector is empty, new tags are
        /// automatically assigned to the elements.
        /// </summary>
        public static void AddElementsByType(int tag, int elementType, long[] elementTags, long[] nodeTags)
        {
            IWrap.GmshModelMeshAddElementsByType(tag, elementType, elementTags, elementTags.LongLength, nodeTags, nodeTags.LongLength, ref _ierr);
        }

        /// <summary>
        /// Partition the mesh of the current model into `numPart' partitions.
        /// </summary>
        public static void Partition(int numPart)
        {
            IWrap.GmshModelMeshPartition(numPart, ref _ierr);
        }

        /// <summary>
        /// Unpartition the mesh of the current model.
        /// </summary>
        public static void Unpartition()
        {
            IWrap.GmshModelMeshUnpartition(ref _ierr);
        }

        /// <summary>
        /// Set the order of the elements in the mesh of the current model to `order'.
        /// </summary>
        public static void SetOrder(int order)
        {
            IWrap.GmshModelMeshSetOrder(order, ref _ierr);
        }

        /// <summary>
        /// Clear the mesh, i.e. delete all the nodes and elements, for the entities
        /// `dimTags'. if `dimTags' is empty, clear the whole mesh.Note that the mesh
        /// of an entity can only be cleared if this entity is not on the boundary of
        /// another entity with a non-empty mesh.
        /// </summary>
        public static void Clear(Tuple<int, int>[] dimTags)
        {
            var arr = IHelpers.FlattenIntTupleArray(dimTags);
            IWrap.GmshModelMeshClear(arr, arr.LongLength, ref _ierr);
        }

        /// <summary>
        /// Get the nodes classified on the entity of tag `tag', for all the elements
        /// of type `elementType'. The other arguments are treated as in `getNodes'.
        /// </summary>
        public static void GetNodesByElementType(int elementType, out long[] nodeTags, out double[] coord, out double[] parametricCoord, int tag, bool returnParametricCoord)
        {
            IntPtr ntP, cP, pcP;
            long nodeTags_n, coord_n, parametricCoord_n;
            IWrap.GmshModelMeshGetNodesByElementType(elementType, out ntP, out nodeTags_n, out cP, out coord_n, out pcP, out parametricCoord_n, tag, Convert.ToInt32(returnParametricCoord), ref _ierr);

            nodeTags = new long[nodeTags_n];
            coord = new double[coord_n];
            parametricCoord = new double[parametricCoord_n];
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);
            Marshal.Copy(cP, coord, 0, (int)coord_n);
            Marshal.Copy(pcP, parametricCoord, 0, (int)parametricCoord_n);

            Free(ntP);
            Free(cP);
            Free(pcP);
        }

        /// <summary>
        /// Get the coordinates and the parametric coordinates (if any) of the node
        /// with tag `tag'. This function relies on an internal cache (a vector in case
        /// of dense node numbering, a map otherwise); for large meshes accessing nodes
        /// in bulk is often preferable.
        /// </summary>
        public static void GetNode(long nodeTag, out double[] coord, out double[] parametricCoord)
        {
            IntPtr cP, pcP;
            long coord_n, parametricCoord_n;

            IWrap.GmshModelMeshGetNode(nodeTag, out cP, out coord_n, out pcP, out parametricCoord_n, ref _ierr);

            coord = new double[coord_n];
            parametricCoord = new double[parametricCoord_n];
            Marshal.Copy(cP, coord, 0, (int)coord_n);
            Marshal.Copy(pcP, parametricCoord, 0, (int)parametricCoord_n);

            Free(cP);
            Free(pcP);
        }

        /// <summary>
        /// Set the coordinates and the parametric coordinates (if any) of the node
        /// with tag `tag'. This function relies on an internal cache (a vector in case
        /// of dense node numbering, a map otherwise); for large meshes accessing nodes
        /// in bulk is often preferable.
        /// </summary>
        public static void SetNode(long nodeTag, double[] coord, double[] parametricCoord)
        {
            IWrap.GmshModelMeshSetNode(nodeTag, coord, coord.LongLength, parametricCoord, parametricCoord.LongLength, ref _ierr);
        }

        /// <summary>
        /// Rebuild the node cache.
        /// </summary>
        public static void RebuildNodeCache(bool onlyIfNecessary)
        {
            IWrap.GmshModelMeshRebuildNodeCache(Convert.ToInt32(onlyIfNecessary), ref _ierr);
        }

        /// <summary>
        /// Rebuild the element cache.
        /// </summary>
        public static void RebuildElementCache(bool onlyIfNecessary)
        {
            IWrap.GmshModelMeshRebuildElementCache(Convert.ToInt32(onlyIfNecessary), ref _ierr);
        }

        /// <summary>
        /// Get the nodes from all the elements belonging to the physical group of
        /// dimension `dim' and tag `tag'. `nodeTags' contains the node tags; `coord'
        /// is a vector of length 3 times the length of `nodeTags' that contains the x,
        /// y, z coordinates of the nodes, concatenated: [n1x, n1y, n1z, n2x, ...]. 
        /// </summary>
        public static void GetNodesForPhysicalGroup(int dim, int tag, out long[] nodeTags, out double[] coord)
        {
            IntPtr ntP, cP;
            long nodeTags_n, coord_n;

            IWrap.GmshModelMeshGetNodesForPhysicalGroup(dim, tag, out ntP, out nodeTags_n, out cP, out coord_n, ref _ierr);

            nodeTags = new long[nodeTags_n];
            coord = new double[coord_n];
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);
            Marshal.Copy(cP, coord, 0, (int)coord_n);

            Free(ntP);
            Free(cP);
        }

        /// <summary>
        /// Relocate the nodes classified on the entity of dimension `dim' and tag
        /// `tag' using their parametric coordinates. If `tag' < 0, relocate the nodes
        /// for all entities of dimension `dim'. If `dim' and `tag' are negative,
        /// relocate all the nodes in the mesh.
        /// </summary>
        public static void RelocateNodes(int dim, int tag)
        {
            IWrap.GmshModelMeshRelocateNodes(dim, tag, ref _ierr);
        }

        /// <summary>
        /// Get the type and node tags of the element with tag `tag', as well as the dimension `dim' and tag `tag' of the entity on which the element is classified. This function
        /// relies on an internal cache(a vector in case of dense element numbering, a
        /// map otherwise); for large meshes accessing elements in bulk is often preferable.
        /// </summary>
        public static void GetElement(long elementTag, int dim, int tag, out int elementType, out long[] nodeTags)
        {
            IntPtr ntP;
            long nodeTags_n;
            IWrap.GmshModelMeshGetElement(elementTag, out elementType, out ntP, out nodeTags_n, dim, tag, ref _ierr);

            nodeTags = new long[nodeTags_n];
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

            Free(ntP);
        }

        /// <summary>
        /// Search the mesh for an element located at coordinates (`x', `y', `z'). This
        /// function performs a search in a spatial octree.If an element is found,
        /// return its tag, type and node tags, as well as the local coordinates(`u',
        /// `v', `w') within the reference element corresponding to search location.If
        /// `dim' is >= 0, only search for elements of the given dimension. If `strict'
        /// is not set, use a tolerance to find elements near the search location.
        /// </summary>
        public static void GetElementByCoordinates(double x, double y, double z, out long elementTag, out int elementType, out long[] nodeTags, out double u, out double v, out double w, int dim, bool strict = false)
        {
            IntPtr ntP;
            long nodeTags_n;
            IWrap.GmshModelMeshGetElementByCoordinates(x, y, z, out elementTag, out elementType, out ntP, out nodeTags_n, out u, out v, out w, dim, Convert.ToInt32(strict), ref _ierr);

            nodeTags = new long[nodeTags_n];
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

            Free(ntP);
        }

        /// <summary>
        /// Search the mesh for element(s) located at coordinates (`x', `y', `z'). This
        /// function performs a search in a spatial octree.Return the tags of all
        /// found elements in `elementTags'. Additional information about the elements
        /// can be accessed through `getElement' and `getLocalCoordinatesInElement'. If
        /// `dim' is >= 0, only search for elements of the given dimension. If `strict'
        /// is not set, use a tolerance to find elements near the search location.
        /// </summary>
        public static void GetElementsByCoordinates(double x, double y, double z, out long[] elementTags, int dim, bool strict = false)
        {
            IntPtr etP;
            long elementTags_n;
            IWrap.GmshModelMeshGetElementsByCoordinates(x, y, z, out etP, out elementTags_n, dim, Convert.ToInt32(strict), ref _ierr);

            elementTags = new long[elementTags_n];
            Marshal.Copy(etP, elementTags, 0, (int)elementTags_n);

            Free(etP);
        }

        /// <summary>
        /// Return the local coordinates (`u', `v', `w') within the element
        /// `elementTag' corresponding to the model coordinates (`x', `y', `z'). This
        /// function relies on an internal cache(a vector in case of dense element
        /// numbering, a map otherwise); for large meshes accessing elements in bulk is
        /// often preferable.
        /// </summary>
        public static void GetLocalCoordinatesInElement(long elementTag, double x, double y, double z, out double u, out double v, out double w)
        {
            IWrap.GmshModelMeshGetLocalCoordinatesInElement(elementTag, x, y, z, out u, out v, out w, ref _ierr);
        }

        /// <summary>
        /// Get the types of elements in the entity of dimension `dim' and tag `tag'.
        /// If `tag' < 0, get the types for all entities of dimension `dim'. If `dim'
        /// and `tag' are negative, get all the types in the mesh.
        /// </summary>
        public static void GetElementTypes(out int[] elementTypes, int dim, int tag)
        {
            IntPtr etP;
            long elementTypes_n;
            IWrap.GmshModelMeshGetElementTypes(out etP, out elementTypes_n, dim, tag, ref _ierr);

            elementTypes = new int[elementTypes_n];
            Marshal.Copy(etP, elementTypes, 0, (int)elementTypes_n);

            Free(etP);
        }

        /// <summary>
        /// Return an element type given its family name `familyName' ("point", "line",
        /// "triangle", "quadrangle", "tetrahedron", "pyramid", "prism", "hexahedron")
        /// and polynomial order `order'. If `serendip' is true, return the
        /// corresponding serendip element type(element without interior nodes).
        /// </summary>
        public static void GetElementType(string familyName, int order, bool serendip = false)
        {
            IWrap.GmshModelMeshGetElementType(familyName, order, Convert.ToInt32(serendip), ref _ierr);
        }

        /// <summary>
        /// Get the properties of an element of type `elementType': its name
        /// (`elementName'), dimension (`dim'), order(`order'), number of nodes
        /// (`numNodes'), local coordinates of the nodes in the reference element
        /// (`localNodeCoord' vector, of length `dim' times `numNodes') and number of
        /// primary (first order) nodes (`numPrimaryNodes').
        /// </summary>
        public static void GetElementProperties(int elementType, string elementName, out int dim, out int order, out int numNodes, out double[] localNodeCoord, out int numPrimaryNodes)
        {
            IntPtr lcdP;
            long localNodeCoord_n;
            IWrap.GmshModelMeshGetElementProperties(elementType, elementName, out dim, out order, out numNodes, out lcdP, out localNodeCoord_n, out numPrimaryNodes, ref _ierr);

            localNodeCoord = new double[localNodeCoord_n];
            Marshal.Copy(lcdP, localNodeCoord, 0, (int)localNodeCoord_n);

            Free(lcdP);
        }

        /// <summary>
        /// Get the elements of type `elementType' classified on the entity of tag
        /// `tag'. If `tag' < 0, get the elements for all entities. `elementTags' is a
        /// vector containing the tags (unique, strictly positive identifiers) of the
        /// elements of the corresponding type. `nodeTags' is a vector of length equal
        /// to the number of elements of the given type times the number N of nodes for
        /// this type of element, that contains the node tags of all the elements of
        /// the given type, concatenated: [e1n1, e1n2, ..., e1nN, e2n1, ...]. If
        /// `numTasks' > 1, only compute and return the part of the data indexed by `task'.
        /// </summary>
        public static void GetElementsByType(int elementType, out long[] elementTags, out long[] nodeTags, int tag, long task, long numTasks)
        {
            IntPtr etP, ntP;
            long elementTags_n, nodeTags_n;
            IWrap.GmshModelMeshGetElementsByType(elementType, out etP, out elementTags_n, out ntP, out nodeTags_n, tag, task, numTasks, ref _ierr);

            elementTags = new long[elementTags_n];
            nodeTags = new long[nodeTags_n];
            Marshal.Copy(etP, elementTags, 0, (int)elementTags_n);
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

            Free(ntP);
            Free(etP);
        }

        /// <summary>
        /// Get the Jacobians of all the elements of type `elementType' classified on
        /// the entity of tag `tag', at the G evaluation points `localCoord' given as
        /// concatenated triplets of coordinates in the reference element[g1u, g1v,
        /// g1w, ..., gGu, gGv, gGw]. Data is returned by element, with elements in the
        /// same order as in `getElements' and `getElementsByType'. `jacobians'
        /// contains for each element the 9 entries of the 3x3 Jacobian matrix at each
        /// evaluation point.The matrix is returned by column: [e1g1Jxu, e1g1Jyu,
        /// e1g1Jzu, e1g1Jxv, ..., e1g1Jzw, e1g2Jxu, ..., e1gGJzw, e2g1Jxu, ...], with
        /// Jxu = dx / du, Jyu = dy / du, etc. `determinants' contains for each element the
        /// determinant of the Jacobian matrix at each evaluation point: [e1g1, e1g2,
        /// ... e1gG, e2g1, ...]. `coord' contains for each element the x, y, z
        /// coordinates of the evaluation points.If `tag' < 0, get the Jacobian data
        /// for all entities.If `numTasks' > 1, only compute and return the part of
        /// the data indexed by `task'.
        /// </summary>
        public static void GetJacobians(int elementType, double[] localCoord, out double[] jacobians, out double[] determinants, out double[] coord, int tag, long task, long numTasks)
        {
            IntPtr jP, dP, cP;
            long jacobians_n, determinants_n, coord_n;
            IWrap.GmshModelMeshGetJacobians(elementType, localCoord, localCoord.LongLength, out jP, out jacobians_n, out dP, out determinants_n, out cP, out coord_n, tag, task, numTasks, ref _ierr);

            jacobians = new double[jacobians_n];
            determinants = new double[determinants_n];
            coord = new double[coord_n];
            Marshal.Copy(jP, jacobians, 0, (int)jacobians_n);
            Marshal.Copy(dP, determinants, 0, (int)determinants_n);
            Marshal.Copy(cP, coord, 0, (int)coord_n);

            Free(jP);
            Free(dP);
            Free(cP);
        }

        /// <summary>
        ///  Get the Jacobian for a single element `elementTag', at the G evaluation
        ///  points `localCoord' given as concatenated triplets of coordinates in the
        ///  reference element[g1u, g1v, g1w, ..., gGu, gGv, gGw]. `jacobians' contains
        ///  the 9 entries of the 3x3 Jacobian matrix at each evaluation point.The
        ///  matrix is returned by column: [e1g1Jxu, e1g1Jyu, e1g1Jzu, e1g1Jxv, ...,
        ///  e1g1Jzw, e1g2Jxu, ..., e1gGJzw, e2g1Jxu, ...], with Jxu = dx / du, Jyu = dy / du,
        ///  etc. `determinants' contains the determinant of the Jacobian matrix at each
        ///  evaluation point. `coord' contains the x, y, z coordinates of the
        ///  evaluation points. This function relies on an internal cache(a vector in
        ///  case of dense element numbering, a map otherwise); for large meshes
        ///  accessing Jacobians in bulk is often preferable.
        /// </summary>
        public static void GetJacobian(long elementTag, double[] localCoord, out double[] jacobians, out double[] determinants, out double[] coord)
        {
            IntPtr jP, dP, cP;
            long jacobians_n, determinants_n, coord_n;

            IWrap.GmshModelMeshGetJacobian(elementTag, localCoord, localCoord.LongLength, out jP, out jacobians_n, out dP, out determinants_n, out cP, out coord_n, ref _ierr);

            jacobians = new double[jacobians_n];
            determinants = new double[determinants_n];
            coord = new double[coord_n];
            Marshal.Copy(jP, jacobians, 0, (int)jacobians_n);
            Marshal.Copy(dP, determinants, 0, (int)determinants_n);
            Marshal.Copy(cP, coord, 0, (int)coord_n);

            Free(jP);
            Free(dP);
            Free(cP);
        }

        /// <summary>
        /// Get the barycenters of all elements of type `elementType' classified on the
        /// entity of tag `tag'. If `primary' is set, only the primary nodes of the
        /// elements are taken into account for the barycenter calculation.If `fast'
        /// is set, the function returns the sum of the primary node coordinates
        /// (without normalizing by the number of nodes). If `tag' < 0, get the
        /// barycenters for all entities.If `numTasks' > 1, only compute and return
        /// the part of the data indexed by `task'.
        /// </summary>
        public static void GetBarycenters(int elementType, int tag, bool fast, bool primary, out double[] barycenters, long task, long numTasks)
        {
            IntPtr bP;
            long barycenters_n;
            IWrap.GmshModelMeshGetBarycenters(elementType, tag, Convert.ToInt32(fast), Convert.ToInt32(primary), out bP, out barycenters_n, task, numTasks, ref _ierr);

            barycenters = new double[barycenters_n];
            Marshal.Copy(bP, barycenters, 0, (int)barycenters_n);

            Free(bP);
        }

        /// <summary>
        /// Get the nodes on the edges of all elements of type `elementType' classified
        /// on the entity of tag `tag'. `nodeTags' contains the node tags of the edges
        /// for all the elements: [e1a1n1, e1a1n2, e1a2n1, ...]. Data is returned by
        /// element, with elements in the same order as in `getElements' and
        /// `getElementsByType'. If `primary' is set, only the primary(begin/end)
        /// nodes of the edges are returned.If `tag' < 0, get the edge nodes for all
        /// entities.If `numTasks' > 1, only compute and return the part of the data
        /// indexed by `task'. 
        /// </summary>
        public static void GetElementEdgeNodes(int elementType, out long[] nodeTags, int tag, bool primary, long task, long numTasks)
        {
            IntPtr ntP;
            long nodeTags_n;
            IWrap.GmshModelMeshGetElementEdgeNodes(elementType, out ntP, out nodeTags_n, tag, Convert.ToInt32(primary), task, numTasks, ref _ierr);

            nodeTags = new long[nodeTags_n];
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

            Free(ntP);
        }

        /// <summary>
        /// Get the nodes on the faces of type `faceType' (3 for triangular faces, 4
        /// for quadrangular faces) of all elements of type `elementType' classified on
        /// the entity of tag `tag'. `nodeTags' contains the node tags of the faces for
        /// all elements: [e1f1n1, ..., e1f1nFaceType, e1f2n1, ...]. Data is returne
        /// by element, with elements in the same order as in `getElements' and
        /// `getElementsByType'. If `primary' is set, only the primary(corner) nodes
        /// of the faces are returned.If `tag' < 0, get the face nodes for all
        /// entities.If `numTasks' > 1, only compute and return the part of the data
        /// indexed by `task'. 
        /// </summary>
        public static void GetElementFaceNodes(int elementType, int faceType, out long[] nodeTags, int tag, bool primary, long task, long numTasks)
        {
            IntPtr ntP;
            long nodeTags_n;
            IWrap.GmshModelMeshGetElementFaceNodes(elementType, faceType, out ntP, out nodeTags_n, tag, Convert.ToInt32(primary), task, numTasks, ref _ierr);

            nodeTags = new long[nodeTags_n];
            Marshal.Copy(ntP, nodeTags, 0, (int)nodeTags_n);

            Free(ntP);
        }

        /// <summary>
        /// Reorder the elements of type `elementType' classified on the entity of tag `tag' according to `ordering'.
        /// </summary>
        public static void ReorderElements(int elementType, int tag, long[] ordering)
        {
            IWrap.GmshModelMeshReorderElements(elementType, tag, ordering, ordering.LongLength, ref _ierr);
        }

        /// <summary>
        /// Get the basis functions of the element of type `elementType' at the
        /// evaluation points `localCoord' (given as concatenated triplets of
        /// coordinates in the reference element[g1u, g1v, g1w, ..., gGu, gGv, gGw]),
        /// for the function space `functionSpaceType' (e.g. "Lagrange" or
        /// "GradLagrange" for Lagrange basis functions or their gradient, in the u, v,
        /// w coordinates of the reference element; or "H1Legendre3" or
        /// "GradH1Legendre3" for 3rd order hierarchical H1 Legendre functions).
        /// `numComponents' returns the number C of components of a basis function.
        /// `basisFunctions' returns the value of the N basis functions at the
        /// evaluation points, i.e. [g1f1, g1f2, ..., g1fN, g2f1, ...] when C == 1 or
        /// [g1f1u, g1f1v, g1f1w, g1f2u, ..., g1fNw, g2f1u, ...] when C == 3. For basis
        /// functions that depend on the orientation of the elements, all values for
        /// the first orientation are returned first, followed by values for the
        /// second, etc. `numOrientations' returns the overall number of orientations.
        /// If `wantedOrientations' is not empty, only return the values for the desired orientation indices.
        /// </summary>
        public static void GetBasisFunctions(int elementType, double[] localCoord, string functionSpaceType, out int numComponents, out double[] basisFunctions, out int numOrientations, int[] wantedOrientations)
        {
            IntPtr bfP;
            long basisFunctions_n;
            IWrap.GmshModelMeshGetBasisFunctions(elementType, localCoord, localCoord.LongLength, functionSpaceType, out numComponents, out bfP, out basisFunctions_n, out numOrientations, wantedOrientations, wantedOrientations.LongLength, ref _ierr);

            basisFunctions = new double[0];
            if (basisFunctions_n > 0)
            {
                basisFunctions = new double[basisFunctions_n];
                Marshal.Copy(bfP, basisFunctions, 0, (int)basisFunctions_n);
            }

            Free(bfP);
        }

        /// <summary>
        /// Get the orientation index of the elements of type `elementType' in the
        /// entity of tag `tag'. The arguments have the same meaning as in
        /// `getBasisFunctions'. `basisFunctionsOrientation' is a vector giving for
        /// each element the orientation index in the values returned by
        /// `getBasisFunctions'. For Lagrange basis functions the call is superfluous
        /// as it will return a vector of zeros.
        /// </summary>
        public static void GetBasisFunctionsOrientationForElements(int elementType, string functionSpaceType, out int[] basisFunctionsOrientation, int tag, long task, long numTasks)
        {
            IntPtr bP;
            long basisFunctionsOrientation_n;
            IWrap.GmshModelMeshGetBasisFunctionsOrientationForElements(elementType, functionSpaceType, out bP, out basisFunctionsOrientation_n, tag, task, numTasks, ref _ierr);

            basisFunctionsOrientation = new int[basisFunctionsOrientation_n];
            Marshal.Copy(bP, basisFunctionsOrientation, 0, (int)basisFunctionsOrientation_n);

            Free(bP);
        }

        /// <summary>
        /// Get the orientation of a single element `elementTag'.
        /// </summary>
        public static void GetBasisFunctionsOrientationForElement(long elementTag, string functionSpaceType, out int basisFunctionsOrientation)
        {
            IWrap.GmshModelMeshGetBasisFunctionsOrientationForElement(elementTag, functionSpaceType, out basisFunctionsOrientation, ref _ierr);
        }

        /// <summary>
        /// Get the number of possible orientations for elements of type `elementType'
        /// and function space named `functionSpaceType'.
        /// </summary>
        public static void GetNumberOfOrientations(int elementType, string functionSpaceType)
        {
            IWrap.GmshModelMeshGetNumberOfOrientations(elementType, functionSpaceType, ref _ierr);
        }

        /// <summary>
        /// Add a new mesh size field of type `fieldType'. 
        /// If `tag' is positive, assign the tag explicitly; otherwise a new tag is assigned automatically.Return the field tag.
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static int AddMeshField(string fieldType, int tag = -1)
        {
            return IWrap.GmshModelMeshFieldAdd(fieldType, tag, ref _ierr);
        }

        /// <summary>
        /// Remove the field with tag `tag'.
        /// </summary>
        /// <param name="tag"></param>
        public static void RemoveMeshField(int tag)
        {
            IWrap.GmshModelMeshFieldRemove(tag, ref _ierr);
        }

        /// <summary>
        /// Set the numerical option `option' to value `value' for field `tag'.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public static void SetMeshFieldOptionNumber(int tag, string option, double value)
        {
            IWrap.GmshModelMeshFieldSetNumber(tag, option, value, ref _ierr);
        }

        /// <summary>
        /// Set the string option `option' to value `value' for field `tag'.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public static void SetMeshFieldOptionString(int tag, string option, string value)
        {
            IWrap.GmshModelMeshFieldSetString(tag, option, value, ref _ierr);
        }

        /// <summary>
        /// Set the numerical list option `option' to value `value' for field `tag'.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public static void SetMeshFieldOptionNumbers(int tag, string option, double[] value)
        {
            IWrap.GmshModelMeshFieldSetNumbers(tag, option, value, value.LongLength, ref _ierr);
        }

        /// <summary>
        /// Set the field `tag' as the background mesh size field. 
        /// </summary>
        /// <param name="tag"></param>
        public static void SetMeshFieldAsBackgroundMesh(int tag)
        {
            IWrap.GmshModelMeshFieldSetAsBackgroundMesh(tag, ref _ierr);
        }

        /// <summary>
        /// Set the field `tag' as a boundary layer size field.
        /// </summary>
        /// <param name="tag"></param>
        public static void SetMeshFieldAsBoundaryLayer(int tag)
        {
            IWrap.GmshModelMeshFieldSetAsBoundaryLayer(tag, ref _ierr);
        }
        #endregion
    }
}

