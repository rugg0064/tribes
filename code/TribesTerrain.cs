using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Tribes
{
    public class TribesTerrain
    {
        public Prop prop;
		public int vertSize;
		public float mapSize;
		public float horizontalMultiplier;
		public float height;

		private int seed;
		private StackedPerlin noise;
		public TribesTerrain(int seed, Vector3 pos)
        {
            this.seed = seed;
            Material material = Material.Load( "materials/4_grass.vmat" );
			//Material material = Material.Load( "materials/dev/black_grid_8.vmat" );
			//Material material = Material.Load( "materials/dev/dev_measuregeneric01.vmat" );
			
			Mesh mesh = new Mesh(material);
			int size = 128; // N * N
			this.vertSize = size;
			float squareSize = 8192;
			this.mapSize = squareSize;
			this.height = 512;
			//int seed = 271;
			float textureMultiplier = 0.25f;
			
			//Perlin perlin = new Perlin(seed, 8);
			StackedPerlin perlin = new StackedPerlin(seed, new PerlinLayerData[]
			{
				new PerlinLayerData(128, 0.30f),
				new PerlinLayerData(128, 0.30f),
				new PerlinLayerData(128, 0.30f),
				new PerlinLayerData(8, 0.10f),
			});
			this.noise = perlin;
			
			float[,] hm = Utils.normalizedMap(perlin.getMap(0, 0, size-1, size-1));

			Vector3[]  vectors =   new Vector3[size*size];
			HeightMapVertex[] verticies = new HeightMapVertex[size*size];
			this.horizontalMultiplier = ((float)squareSize) / ((float)(size-1)); // AKA horizontal distance per index

			for(int i = 0; i < size; i++)
			{
				for(int j = 0; j < size; j++)
				{
					Vector3 vPos = vectors[(i*size) + j] = new Vector3( i * horizontalMultiplier, j * horizontalMultiplier, hm[i,j] * height);

					float U = hm[i, j+1 >= size ? 0 : j+1] * height;
					float D = hm[i, j - 1 < 0 ? size-1 : j-1] * height;
					float R = hm[i+1 >= size ? 0 : i+1, j] * height;
					float L = hm[i+1 >= size ? 0 : i+1, j] * height;

					Vector3 vNormal = new Vector3( (R-L) / (2*horizontalMultiplier), (D-U) / (2*horizontalMultiplier), 1 ).Normal;
					
					Vector3 vTangent =  Vector3.Cross(vNormal, Vector3.Up);

					Vector2 vTexCoord = new Vector2( i * textureMultiplier, j * textureMultiplier);
					verticies[(i*size) + j] = new HeightMapVertex(vPos, vTangent, vNormal, Color.Green, vTexCoord);
				}
			}

			mesh.CreateVertexBuffer<HeightMapVertex>(
				size*size*6,
				new VertexAttribute[]{
					new VertexAttribute( VertexAttributeType.Position, VertexAttributeFormat.Float32, 3),
					new VertexAttribute( VertexAttributeType.Tangent,  VertexAttributeFormat.Float32, 3),
					new VertexAttribute( VertexAttributeType.Normal,   VertexAttributeFormat.Float32, 3),
					new VertexAttribute( VertexAttributeType.Color,    VertexAttributeFormat.Float32, 4),
					new VertexAttribute( VertexAttributeType.TexCoord, VertexAttributeFormat.Float32, 2)
				},
				verticies
			);
			int[] triangles = new int[ (size-1) * (size-1) * 6 ];
			int tri = 0;
			for(int i = 0; i < size - 1; i++) // vertical
			{
				for(int j = 0; j < size - 1; j++) //horizontal
				{
					triangles[tri++] = ts(i,   j,   size);
					triangles[tri++] = ts(i+1, j,   size);
					triangles[tri++] = ts(i,   j+1, size);
					
					triangles[tri++] = ts(i+1, j,   size);
					triangles[tri++] = ts(i+1, j+1, size);
					triangles[tri++] = ts(i,   j+1, size);
				}
			}
			mesh.CreateIndexBuffer( triangles.Length, triangles);
			mesh.SetIndexRange(0, triangles.Length);

			Model model = new ModelBuilder()
				.AddMesh( mesh )
				.AddCollisionMesh(vectors, triangles)
				.WithMass( 10 )
				.Create();

			Prop p = new Prop();
			p.SetModel( model );
			p.Position = pos;
			p.Spawn();
			p.PhysicsBody.BodyType = PhysicsBodyType.Static;
            
			DebugOverlay.Box(9999f, p.Position, p.Position + new Vector3(squareSize, squareSize, height), Color.Green, true);
			this.prop = p;
        }

        private static int ts(int i, int j, int size)
		{
			return (i * size) + j;
		}

		public Vector3 getPos(int x, int y)
		{
			return prop.Position + new Vector3( x * horizontalMultiplier, y * horizontalMultiplier, noise.getHeight(x,y) * height);
		}
	}
}
