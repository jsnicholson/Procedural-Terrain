using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

    public static float[,] GenerateFalloffMap(int size) {
        float[,] map = new float[size, size];

        Vector2 centre = new Vector2(Mathf.Floor(size/2), Mathf.Floor(size / 2));
        Debug.Log("centre: x:" + centre.x + " y:" + centre.y);

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    public static float[,] GenerateCircularFalloffMap(int size) {
        float[,] map = new float[size, size];
        Vector2 centre = new Vector2(size / 2, size / 2);
        float circleRadius = size / 4.0f;
        float minimumDistance = (size - (circleRadius * 2)) / 2;

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                if (DistBetween(centre, new Vector2(i,j)) <= circleRadius) {
                    map[i, j] = 0.0f;
                } else {
                    float distToCircleEdge = DistBetween(centre, new Vector2(i, j)) - circleRadius;
                    float value = distToCircleEdge / minimumDistance;
                    map[i, j] = value;
                }
                
            }
        }

        return map;
    }

    static float Evaluate(float value) {
        float a = 2;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }

    static float DistBetween(Vector2 a, Vector2 b) {
        float dx = Mathf.Abs(a.x - b.x);
        float dy = Mathf.Abs(a.y - b.y);

        return Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
    }
}