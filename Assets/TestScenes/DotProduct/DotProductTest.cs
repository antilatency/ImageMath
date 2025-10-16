using UnityEngine;
using ImageMath;
using ImageMath.Views;
using System;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class DotProductTest : MonoBehaviour{
	[Serializable]
	public struct Pair { 
		public Texture Texture;
		public Color Weight;

		public Pair(Texture texture, Color weight) {
			Texture = texture;
			Weight = weight;
		}
	}

	public Pair[] Elements;

	void Update() {

		var firstTexture = Elements.FirstOrDefault(x => x.Texture != null).Texture;
		if (firstTexture == null) {
			return;
		}

		var result = TextureView.GetByName("Result").ResizeRenderTexture(firstTexture.width, firstTexture.height);
		
		new DotProduct(
			Elements.Select(x => x.Texture).ToList(),
			Elements.Select(x => { 
				var linearWeight = x.Weight.linear;
				return new Vector4(linearWeight.r, linearWeight.g, linearWeight.b, linearWeight.a);
			}).ToList()
		).AssignTo(result);

	}
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}