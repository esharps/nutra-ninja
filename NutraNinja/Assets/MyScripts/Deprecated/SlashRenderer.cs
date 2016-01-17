using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;

public class SlashRenderer : MonoBehaviour {

	public Color c1 = Color.yellow;
	public Color c2 = Color.red;

	private GameObject slashGameObj;
	private LineRenderer lineRenderer;
	private int i = 0;

	// Use this for initialization
	void Start () {

		slashGameObj = new GameObject("Slash");
		slashGameObj.AddComponent<LineRenderer>();
		lineRenderer = slashGameObj.GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetWidth(0.3f, 0);
		lineRenderer.SetVertexCount(0);
	}

	// Update is called once per frame
	void Update () {


		lineRenderer.SetVertexCount(i + 1);

	}
}
