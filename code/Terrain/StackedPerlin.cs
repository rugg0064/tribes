using Sandbox;
using System;
using System.Collections.Generic;


public struct PerlinLayerData
{
	public readonly int   unitsPerGrid;
	public readonly float percentage;
	public PerlinLayerData(int unitsPerGrid, float percentage)
	{
		this.unitsPerGrid = unitsPerGrid;
		this.percentage = percentage;
	}
}
public class StackedPerlin : NoiseGenerator
{
	int baseSeed;
	int layerCount;
	PerlinLayerData[] data;
	List<Perlin> perlinLayers;
	public StackedPerlin(int seed, PerlinLayerData[] data)
	{
		this.data = data;
		this.layerCount = data.Length;
		this.perlinLayers = new List<Perlin>();
		this.baseSeed = seed;
		int iterSeed = baseSeed;
		for(int i = 0; i < layerCount; i++)
		{
			perlinLayers.Add(new Perlin(iterSeed++, data[i].unitsPerGrid));
		}
	}

	public float getHeight(int x, int y)
	{
		float accumulator = 0;
		for(int i = 0; i < layerCount; i++)
		{
			accumulator += perlinLayers[i].getHeight(x,y) * data[i].percentage;
		}
		return accumulator;
	}

	/*
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
	*/

	public Perlin getPerlin(int layer)
	{
		return perlinLayers[layer];
	}
}
