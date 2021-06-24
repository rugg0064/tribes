using Sandbox;
using System;
using System.Collections.Generic;


namespace MinimalExample
{    public class Perlin
    {
		public readonly int seed;
		public readonly int unitsPerGrid;
		private readonly float maxLengthOfGrid;

		private Dictionary<(int, int), float> heightCache; // Measured in primaryResolution
		private Dictionary<(int, int), Vector2> vectorCache; // Measured in noiseResolution

		public Perlin(int seed, int unitsPerGrid)
		{
			this.seed = seed;
			this.unitsPerGrid = unitsPerGrid;

			this.maxLengthOfGrid = MathF.Sqrt( 2 * MathF.Pow(this.unitsPerGrid, 2) );

			this.vectorCache = new Dictionary<(int, int), Vector2>();
			this.heightCache = new Dictionary<(int, int), float>();
		}

		public float getHeight(int x, int y)
		{
			if(heightCache.ContainsKey( (x,y) ))
			{
				return heightCache[(x,y)];
			}
			else
			{
				int gridCoordX1 = x / unitsPerGrid;
				int gridCoordY1 = y / unitsPerGrid;
				int gridCoordX2 = gridCoordX1 + 1;
				int gridCoordY2 = gridCoordY1 + 1;

				float n0, n1, ix0, ix1;
				float xInterp = sCurve( ((float)(x - (gridCoordX1 * unitsPerGrid))) / ((float) (unitsPerGrid-1)) );
				float yInterp = sCurve( ((float)(y - (gridCoordY1 * unitsPerGrid))) / ((float) (unitsPerGrid-1)) );

				n0 = dotGradient(x, y, gridCoordX1, gridCoordY1);
				n1 = dotGradient(x, y, gridCoordX2, gridCoordY1);
				ix0 = MathX.LerpTo(n0, n1, xInterp);

				n0 = dotGradient(x, y, gridCoordX1, gridCoordY2);
				n1 = dotGradient(x, y, gridCoordX2, gridCoordY2);
				ix1 = MathX.LerpTo(n0, n1, xInterp);

				float result = MathX.LerpTo(ix0, ix1, yInterp) + 0.5f;
				heightCache[ (x,y) ] = result;
				return result;
			}
		}

		// POSITIONS ARE INCLUSIVE [x0,x1] and [y0,y1]
		public float[,] getMap(int x0, int y0, int x1, int y1)
		{
			float[,] map = new float[1 + x1-x0, 1 + y1-y0];
			for(int i = 0; i <= x1-x0; i++) //x index
			{
				for(int j = 0; j <= y1-y0; j++) //y index
				{
					map[i,j] = this.getHeight(x0 + i, y0 + j);
				}
			}
			return map;
		}

		private float dotGradient(int x, int y, int vectorCoordX, int vectorCoordY)
		{
			Vector2 vector;
			if(vectorCache.ContainsKey( (vectorCoordX, vectorCoordY) ))
			{
				vector = vectorCache[ (vectorCoordX, vectorCoordY) ];
			}
			else
			{
				vector = Utils.randomVector2(getCoordinateSeed(vectorCoordX, vectorCoordY));
				vectorCache[ (vectorCoordX, vectorCoordY) ] = vector;
			}
			float xOffset = ( (float) ((vectorCoordX * unitsPerGrid) - x) ) / maxLengthOfGrid;
			float yOffset = ( (float) ((vectorCoordY * unitsPerGrid) - y) ) / maxLengthOfGrid;

			Vector2 offset = new Vector2(xOffset, yOffset);
			float toReturn = (float) Vector2.GetDot(vector, offset);
			return toReturn;
		}

		//Code from https://stackoverflow.com/questions/37128451/random-number-generator-with-x-y-coordinates-as-seed
		private int getCoordinateSeed(int x, int y)
		{   
			int h = seed + x*374761393 + y*668265263; //all constants are prime
			h = (h^(h >> 13))*1274126177;
			return h^(h >> 16);
		}

		private static float sCurve(float w)
		{
			return (float) (1 * ((w * ( w * 6.0 - 15.0) + 10.0) * w * w * w));
		}
    }
}
