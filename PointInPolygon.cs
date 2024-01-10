using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class PointInPolygon
{
    // Calculates the winding sum of the polygon; if the winding sum is 360 degrees, the point is considered inside.
    // This method is more robust than using "insideRay" for determining point inclusion.
    public static bool insideAngle(Vector2 a, List<Vertex> polygon, float tolerance)
    {
        double sum = 0;

        for (int i = 0; i < polygon.Count; i++){
            Vector2 b = polygon[i].pos;
            Vector2 c = polygon[(i + 1) % polygon.Count].pos;

            if (Vector2.Distance(b, a) + Vector2.Distance(c, a) <= Vector2.Distance(b, c) + tolerance){
                return true;
            }

            sum += Vector2.SignedAngle(b - a, c - a);
        }

        sum = Mathf.Abs((float)sum);

        if (sum > 359)
            return true;

        return false;
    }

    // Performs ray casting in the positive x-direction, counting the number of intersections. 
    // If the count is even, the point is considered outside; otherwise, it is inside.
    // Note: This method may fail if the ray passes through multiple polygon vertices.
    public static bool insideRay(Vector2 a, List<Vertex> polygon)
    {
        int intersections = 0;
        Vector2 b = a + new Vector2(10000, 0);

        for (int i = 0; i < polygon.Count; i++){
            Vector2 c = polygon[i].pos;
            Vector2 d = polygon[(i + 1) % polygon.Count].pos;

            if (Vector2.Distance(c, a) + Vector2.Distance(d, a) == Vector2.Distance(c, d))
                return true;

            if (LineIntersector.doIntersect(a, b, c, d))
                intersections++;
        }

        if ((intersections % 2) == 0)
            return false;
        
        return true;
    }

    // Adjust the tolerance field to mitigate numerical robustness issues when the point lies on the circumference of the polygon.
    public static bool insideRay(Vector2 a, List<Vertex> polygon, float tolerance)
    {
        int intersections = 0;
        Vector2 b = a + new Vector2(10000, 0);

        for (int i = 0; i < polygon.Count; i++){
            Vector2 c = polygon[i].pos;
            Vector2 d = polygon[(i + 1) % polygon.Count].pos;

            if (Vector2.Distance(c, a) + Vector2.Distance(d, a) <= Vector2.Distance(c, d) + tolerance)
                return true;
            
            if (LineIntersector.doIntersect(a, b, c, d))
                intersections++;
        }

        if ((intersections % 2) == 0)
            return false;
        
        return true;
    }
}
