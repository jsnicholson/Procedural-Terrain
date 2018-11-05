using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	void Start() {
        RegionMap regions = new RegionMap();
        regions.AddColourKey(new ColourKey(Color.cyan, 0.0f, 0.3f, "cyan"));
        regions.AddColourKey(new ColourKey(Color.magenta, 0.5f, 1.0f, "magenta"));

        Debug.Log(regions.Evaluate(0.3f));
    }
}
