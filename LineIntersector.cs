using System;
using UnityEngine;

public static class LineIntersector
{
    static public bool doIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2){
        // See https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        // for a full code explanation.

        int o1 = orientation(p1, q1, p2);
        int o2 = orientation(p1, q1, q2);
        int o3 = orientation(p2, q2, p1);
        int o4 = orientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4) return true;

        // Special Cases 
        // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
        if (o1 == 0 && onSegment(p1, p2, q1)) return true;

        // p1, q1 and q2 are collinear and q2 lies on segment p1q1 
        if (o2 == 0 && onSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
        if (o3 == 0 && onSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
        if (o4 == 0 && onSegment(p2, q1, q2)) return true;

        // Doesn't fall in any of the above cases 
        return false;
    }
    static int orientation(Vector2 p, Vector2 q, Vector2 r){
        float val = (q.y - p.y) * (r.x - q.x) -
                    (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0; // collinear

        return (val > 0) ? 1 : 2; // clock or counterclock wise
    }
    static bool onSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;

        return false;
    }

    static public bool intersectionPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection){
        //Based on https://encyclopedia.pub/entry/32060#:~:text=Two%20Lines&text=one%20gets%2C%20from%20Cramer's%20rule,2%20%E2%88%92%20a%202%20b%201%20.
        //https://handwiki.org/wiki/Intersection_(Euclidean_geometry)
        
        intersection = new Vector2();

        float a1 = p2.x - p1.x;
        float b1 = p4.x - p3.x;
        float c1 = p3.x - p1.x;
        float a2 = p2.y - p1.y;
        float b2 = p4.y - p3.y;
        float c2 = p3.y - p1.y;

        float det = a1 * b2 - a2 * b1;
        if (det == 0) return false;

        float xS = (c1 * b2 - c2 * b1) / det;
        float yS = -(a1 * c2 - a2 * c1) / det;

        if(xS >= 0 && xS <= 1 && yS >= 0 && yS <= 1){
            intersection = p1 + xS * (p2 - p1);
            return true;
        }
            
        return false;
    }
    static public bool intersectionPointNoTouch(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        //Based on https://encyclopedia.pub/entry/32060#:~:text=Two%20Lines&text=one%20gets%2C%20from%20Cramer's%20rule,2%20%E2%88%92%20a%202%20b%201%20.
        //https://handwiki.org/wiki/Intersection_(Euclidean_geometry)

        intersection = new Vector2();

        float a1 = p2.x - p1.x;
        float b1 = p4.x - p3.x;
        float c1 = p3.x - p1.x;
        float a2 = p2.y - p1.y;
        float b2 = p4.y - p3.y;
        float c2 = p3.y - p1.y;

        float det = a1 * b2 - a2 * b1;
        if (det == 0) return false;

        float xS = (c1 * b2 - c2 * b1) / det;
        float yS = -(a1 * c2 - a2 * c1) / det;

        if (xS > 0 && xS < 1 && yS > 0 && yS < 1)
        {
            intersection = p1 + xS * (p2 - p1);
            return true;
        }

        return false;
    }
    static public bool intersectionPoint(Vector3 p1, Vector3 q1, Vector3 p2, Vector3 q2)
    {

        return true;
    }

}
