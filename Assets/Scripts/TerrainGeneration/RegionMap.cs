using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a colour gradient to be used in colouring a terrain mesh
/// </summary>
public class RegionMap {

    private List<ColourKey> keys;

    public RegionMap() {
        keys = new List<ColourKey>();
        // Initialise the list with default values
        //keys.Add(new ColourKey(Color.white, 0.0f, 0.0f, "white"));
        //keys.Add(new ColourKey(Color.black, 1.0f, 1.0f, "black"));
    }

    public void AddColourKey(ColourKey newKey) {
        if (keys.Count == 0) {
            keys.Add(newKey);
            return;
        }

        for (int i = 0; i < keys.Count; i++) {
            ColourKey currentKey = keys[i];
            if (newKey.endRange < currentKey.startRange) {
                keys.Insert(i, newKey);
                return;
            }
        }

        keys.Add(newKey);
    }

    public ColourKey GetKey(int index) {
        return keys[index];
    }

    public int NumKeys() {
        return keys.Count;
    }

    /// <summary>
    /// Evaluates a float and returns a colour from the region keys depedant on that.
    /// If within range (start, end) of a key, will return the defined colour for that key.
    /// If the value falls outside a (start/end) for a specific key, it will interpolate
    /// a colour between its two adjacent keys
    /// </summary>
    /// <returns></returns>
    public Color32 Evaluate(float value) {
        if (value < 0 || value > 1) {
            Debug.LogError("Value must be in range (0,1)");
        }

        Color32 newCol = new Color32();

        for(int i = 0; i < keys.Count; i++) {
            if (value >= keys[i].startRange && value <= keys[i].endRange) {
                return keys[i].colour;
            } else if (value > keys[i].endRange && value < keys[i+1].startRange) {
                float percentBetween = (value - keys[i].endRange) / (keys[i+1].startRange - keys[i].endRange);
                return Color32.Lerp(keys[i].colour, keys[i+1].colour, percentBetween);
            } else {
                continue;
            }
        }
        return newCol;
    }
}

/// <summary>
/// Represents a block of discrete colour, using values between 0 and 1 to define its range
/// </summary>
public class ColourKey {

    // The colour of the key
    public Color32 colour;
    // Where the discrete colour block will start
    public float startRange;
    // End value of the discrete colour block
    public float endRange;

    public string name;

    public ColourKey(Color32 _colour, float _startRange, float _endRange, string _name) {
        // Restrict the values of start and end to range (0,1)
        if (_startRange < 0 || _startRange > 1 || _endRange < 0 || _endRange > 1) {
            Debug.LogError("ColourKey: start and end values must be between 0 and 1");
        }
        if (_startRange > _endRange) {
            Debug.LogError("ColourKey: startValue must be less than endValue");
        }

        // Assign values
        colour = _colour;
        startRange = _startRange;
        endRange = _endRange;
        name = _name;
    }
}