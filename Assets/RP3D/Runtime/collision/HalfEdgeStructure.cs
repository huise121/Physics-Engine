using System.Collections.Generic;

using VerticesPair = RP3D.Pair<int,int>;

namespace RP3D
{
      
    
    public class HalfEdgeStructure
    {
        public record Edge
        {
            public int vertexIndex; // Index of the vertex at the beginning of the edge
            public int twinEdgeIndex; // Index of the twin edge
            public int faceIndex; // Adjacent face index of the edge
            public int nextEdgeIndex; // Index of the next edge
        }

        public record Face
        {
            public int edgeIndex; // Index of an half-edge of the face
            public List<int> faceVertices; // Index of the vertices of the face
        }

        public record Vertex
        {
            public int vertexPointIndex; // Index of the vertex point in the origin vertex array
            public int edgeIndex; // Index of one edge emanting from this vertex
        }

        /// All the faces
        public List<Face> mFaces;
        

        /// All the vertices
        public List<Vertex> mVertices;

        /// All the half-edges
        public List<Edge> mEdges;


        public HalfEdgeStructure(int facesCapacity, int verticesCapacity, int edgesCapacity)
        {
            mFaces = new List<Face>(facesCapacity);
            mVertices = new List<Vertex>(verticesCapacity);
            mEdges = new List<Edge>(edgesCapacity);
        }


        /// Initialize the structure (when all vertices and faces have been added)
        public void init()
        {
            Dictionary<VerticesPair, Edge> edges = new Dictionary<VerticesPair, Edge>();
            Dictionary<VerticesPair, VerticesPair> nextEdges = new Dictionary<VerticesPair, VerticesPair>();
            Dictionary<VerticesPair, int> mapEdgeToStartVertex = new Dictionary<VerticesPair, int>();
            Dictionary<VerticesPair, int> mapEdgeToIndex = new Dictionary<VerticesPair, int>();
            Dictionary<int, VerticesPair> mapEdgeIndexToKey = new Dictionary<int, VerticesPair>();
            Dictionary<int, VerticesPair> mapFaceIndexToEdgeKey = new Dictionary<int, VerticesPair>();


            var currentFaceEdges = new List<VerticesPair>(mFaces[0].faceVertices.Count);

            // For each face
            var nbFaces = mFaces.Count;
            for (var f = 0; f < nbFaces; f++)
            {
                var face = mFaces[f];

                var firstEdgeKey = new VerticesPair(0, 0);

                // For each vertex of the face
                var nbFaceVertices = face.faceVertices.Count;
                for (var v = 0; v < nbFaceVertices; v++)
                {
                    var v1Index = face.faceVertices[v];
                    var v2Index = face.faceVertices[v == face.faceVertices.Count - 1 ? 0 : v + 1];

                    var pairV1V2 = new VerticesPair(v1Index, v2Index);

                    // Create a new half-edge
                    var edge = new Edge
                    {
                        faceIndex = f,
                        vertexIndex = v1Index
                    };
                    if (v == 0)
                        firstEdgeKey = pairV1V2;
                    else if (v >= 1) nextEdges.Add(currentFaceEdges[currentFaceEdges.Count - 1], pairV1V2);
                    if (v == face.faceVertices.Count - 1) nextEdges.Add(pairV1V2, firstEdgeKey);
                    edges.Add(pairV1V2, edge);

                    var pairV2V1 = new Pair<int, int>(v2Index, v1Index);

                    mapEdgeToStartVertex[pairV1V2] = v1Index;
                    mapEdgeToStartVertex[pairV2V1] =  v2Index;

                    mapFaceIndexToEdgeKey[f] = pairV1V2;

                    if (edges.ContainsKey(pairV2V1))
                    {
                        var itEdge = edges[pairV2V1];
                        var edgeIndex = mEdges.Count;

                        itEdge.twinEdgeIndex = edgeIndex + 1;
                        edge.twinEdgeIndex = edgeIndex;

                        mapEdgeIndexToKey.Add(edgeIndex, pairV2V1);
                        mapEdgeIndexToKey.Add(edgeIndex + 1, pairV1V2);

                        mVertices[v1Index].edgeIndex = edgeIndex + 1;
                        mVertices[v2Index].edgeIndex = edgeIndex;

                        mapEdgeToIndex.Add(pairV1V2, edgeIndex + 1);
                        mapEdgeToIndex.Add(pairV2V1, edgeIndex);

                        mEdges.Add(itEdge);
                        mEdges.Add(edge);
                    }

                    currentFaceEdges.Add(pairV1V2);
                }

                currentFaceEdges.Clear();
            }

            // Set next edges
            var nbEdges = mEdges.Count;
            for (var i = 0; i < nbEdges; i++) mEdges[i].nextEdgeIndex = mapEdgeToIndex[nextEdges[mapEdgeIndexToKey[i]]];

            // Set face edge
            for (var f = 0; f < nbFaces; f++) mFaces[f].edgeIndex = mapEdgeToIndex[mapFaceIndexToEdgeKey[f]];
        }

        /// Add a vertex
        public int addVertex(int vertexPointIndex)
        {
            var vertex = new Vertex
            {
                vertexPointIndex = vertexPointIndex,
                edgeIndex = 0
            };
            mVertices.Add(vertex);
            return mVertices.Count - 1;
        }

        /// Add a face
        public void addFace(List<int> faceVertices)
        {
            var face = new Face
            {
                faceVertices = faceVertices,
                edgeIndex = 0
            };
            mFaces.Add(face);
        }

        /// Return the number of faces
        public int getNbFaces()
        {
            return mFaces.Count;
        }

        /// Return the number of half-edges
        public int getNbHalfEdges()
        {
            return mEdges.Count;
        }

        /// Return the number of vertices
        public int getNbVertices()
        {
            return mVertices.Count;
        }

        /// Return a given face
        public Face getFace(int index)
        {
            return mFaces[index];
        }

        /// Return a given edge
        public Edge getHalfEdge(int index)
        {
            return mEdges[index];
        }

        /// Return a given vertex
        public Vertex getVertex(int index)
        {
            return mVertices[index];
        }
    }
}