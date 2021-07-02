using Sandbox;
using System.Runtime.InteropServices;

namespace Tribes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HeightMapVertex
    {
        public Vector3 Position;
        public Vector3 Tangent;
        public Vector3 Normal;
        public Color color;
        public Vector2 TexCoords;
    
        public HeightMapVertex(Vector3 position, Vector3 tangent, Vector3 normal, Color color, Vector2 texCoords)
        {
            this.Position =  position;
            this.Tangent =   tangent;
            this.Normal =    normal;
            this.color =     color;
            this.TexCoords = texCoords;
        }
    }
}
