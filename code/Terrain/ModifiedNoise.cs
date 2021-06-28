using System;
using System.Collections;
using System.Collections.Generic;

public class ModifiedNoise<T> : NoiseGenerator where T : NoiseGenerator
{
	public List<Func<float, float>> modifierList { get; set; }
	private T noise;

	public ModifiedNoise(T noise)
	{
		this.noise = noise;
		modifierList = null;
	}

	public float getHeight(int x, int y)
	{
		float height = noise.getHeight( x, y );
		if(modifierList != null)
		{
			for ( int i = 0; i < modifierList.Count; i++ )
			{
				height = modifierList[i].Invoke( height );
			}
		}
		return height;
	}
}
