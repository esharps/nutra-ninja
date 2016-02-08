using UnityEngine;
using System.Collections;

public class SplatManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine("destroyDelay");
	}

    private IEnumerator destroyDelay()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
