using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProtoDerp
{
    public class PrimitiveDrawingElement : IComparable<PrimitiveDrawingElement>
    {
        public readonly int primitiveID;
        public PrimitiveType type;
        public VertexPositionColor[] vertices;
        public int vertexOffset;
        public int primitiveCount;
        public Matrix transform;

        public PrimitiveDrawingElement(PrimitiveType type, VertexPositionColor[] vertices, int vertexOffset, int primitiveCount, Matrix transform)
        {
            this.primitiveID = getNextId();
            this.type = type;
            this.vertices = vertices;
            this.vertexOffset = vertexOffset;
            this.primitiveCount = primitiveCount;
            this.transform = transform;
        }

        int IComparable<PrimitiveDrawingElement>.CompareTo(PrimitiveDrawingElement other)
        {
            return this.primitiveID - other.primitiveID;
        }

        public int CompareTo(PrimitiveDrawingElement other)
        {
            return this.primitiveID - other.primitiveID;
        }

        private static int currentId = 0;
        private static int getNextId()
        {
            return currentId++;
        }
    }
}
