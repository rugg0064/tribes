
using Sandbox;
using System;

namespace Tribes
{
    public static class Utils
    {
		public static Vector3 randomVector3(int seed)
		{
			Random r = new Random(seed);
			double randomAngle = (r.NextDouble() * Math.PI) * 2;
			double randomDistance = r.NextDouble();
			float xAxis = (float)(Math.Cos(randomAngle) * randomDistance);
			float yAxis = (float)(Math.Sin(randomAngle) * randomDistance);
			float zAxis = (float)(Math.Sqrt(1 - Math.Pow(randomDistance, 2.0)));
			return new Vector3(xAxis, yAxis, zAxis);
		}

		public static Vector2 randomVector2(int seed)
		{
			Random r = new Random(seed);
			double randomAngle = (r.NextDouble() * Math.PI * 2);
			float xAxis = (float)(Math.Cos(randomAngle));
			float yAxis = (float)(Math.Sin(randomAngle));
			return new Vector2(xAxis, yAxis);
		}

		public static bool isPowerOfTwo(int x)
		{
			return (x != 0) && ((x & (x-1)) == 0);
		}

		public static float[,] normalizedMap(float[,] array)
		{

			float min = float.MaxValue;
			float max = float.MinValue;

			for(int i = 0; i < array.GetLength(0); i++)
			{
				for(int j = 0; j < array.GetLength(1); j++)
				{
					min = MathF.Min(min, array[i,j]);
					max = MathF.Max(max, array[i,j]);
				}
			}
			float multiplier = 1 / (max-min);
			float[,] map = new float[array.GetLength(0), array.GetLength(1)];
			for(int i = 0; i < array.GetLength(0); i++)
			{
				for(int j = 0; j < array.GetLength(1); j++)
				{
					map[i,j] = (array[i,j] - min) * multiplier;
				}
			}
			return map;
		}
    }
}
