using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



//This Class is based on:
//Wilson J.E. 2013. Polygon Subtraction in 2 or 3 Dimensions Richland, WA: Pacific Northwest National Laboratory.
//https://www.pnnl.gov/main/publications/external/technical_reports/PNNL-SA-97135.pdf


public static class PolygonBooleans 
{
    static List<Vertex> verticesA;
    static List<Vertex> verticesB;

    static int indexA;
    static int indexB;

    static List<List<Vector2>> outputPolygons;
    static Bool booleantype;
    public static List<List<Vector2>> boolean(List<Vector2> polygonA, List<Vector2> polygonB, Bool boolean)
    {
        // Set up the lists of points at the beginning before processing them.
        indexA = 0;
        indexB = 0;
        verticesA = initializeVertices(polygonA);
        verticesB = initializeVertices(polygonB);
        booleantype = boolean;

        intersectPolygons();
        insertPoints(verticesA, verticesB);
        insertPoints(verticesB, verticesA);

        crossoverPolygons();
        poinsInside(verticesA, verticesB);
        poinsInside(verticesB, verticesA);

        for(int i = 0; i < verticesA.Count; i++)
        {
            Debug.Log(verticesA[i].pos + " " + verticesA[i].outside);
        }

        // Process the Polygons
        outputPolygons = new List<List<Vector2>>();

        step1();

        return outputPolygons;
    }

    //Pre-Processing
    static List<Vertex> initializeVertices(List<Vector2> polygon)
    {
        List<Vertex> vertices = new List<Vertex>();
        foreach(Vector2 v2 in polygon)
        {
            vertices.Add(new Vertex(v2));
        }

        return vertices;
    }
    static void intersectPolygons()
    {
        
        for(int i = 0; i < verticesA.Count; i++)
        {
            for (int j = 0; j < verticesB.Count; j++)
            {
                bool inter = LineIntersector.intersectionPoint(verticesA[i].pos, verticesA[(i + 1) % verticesA.Count].pos, verticesB[j].pos, verticesB[(j + 1) % verticesB.Count].pos, out Vector2 intersection);

                if (inter == true && !verticesA.Any(v2 => v2.pos == intersection))
                {
                    verticesA.Insert(i + 1, new Vertex(intersection, true));
                }               
            }   
        }
        

        for (int i = 0; i < verticesB.Count; i++)
        {
            for (int j = 0; j < verticesA.Count; j++)
            {
                bool inter = LineIntersector.intersectionPoint(verticesB[i].pos, verticesB[(i + 1) % verticesB.Count].pos, verticesA[j].pos, verticesA[(j + 1) % verticesA.Count].pos, out Vector2 intersection);

                if (inter == true && !verticesB.Any(v2 => v2.pos == intersection))
                {
                    verticesB.Insert(i + 1, new Vertex(intersection, true));
                }
            }
        }
    }
    static void crossoverPolygons()
    {
        for(int i = 0; i < verticesA.Count;i++)
        {
            for(int j = 0;  j < verticesB.Count; j++)
            {
                if (verticesA[i].pos == verticesB[j].pos)
                {
                    verticesA[i].cross = j;
                    verticesB[j].cross = i;

                    break;
                }
            }
        }
    }
    static void insertPoints(List<Vertex> verticesA, List<Vertex> verticesB)
    {
        for (int i = 0; i < verticesA.Count; i++)
        {
            Vertex a = verticesA[i];
            Vertex b = verticesA[(i+1) % verticesA.Count];

            if (a.intersection && b.intersection)
            {
                Vector2 midPoint = a.pos + (b.pos - a.pos) / 2;

                if (PointInPolygon.insideAngle(midPoint, verticesB, 0.0001f) == false)
                {
                    verticesA.Insert(i + 1, new Vertex(midPoint));
                }
            }
        }
    }
    static void poinsInside(List<Vertex> verticesA, List<Vertex> verticesB)
    {
        for (int i = 0; i < verticesA.Count; i++)
        {
            if (!PointInPolygon.insideAngle(verticesA[i].pos, verticesB, 0.0001f))
            {
                verticesA[i].outside = true;
            }
        }
    }
    static int mod(int x, int m)
    {
        return ((x % m) + m) % m;
    }
    
    //Process
    static void step1()
    {
        for(int i = 0; i < verticesA.Count; i++)
        {
            if (verticesA[i].processed == false && verticesA[i].outside == true)
            {
                indexA = i;
                outputPolygons.Add(new List<Vector2>());
                step2();
            }
        }

        indexA = -1;
    }
    static void step2()
    {
        if (outputPolygons.Last().Count > 1 && outputPolygons.Last()[0] == verticesA[indexA].pos)
        {
            step1();
        }

        else{
            outputPolygons.Last().Add(verticesA[indexA].pos);
            verticesA[indexA].processed = true;
            step3();
        }
    }
    static void step3()
    {
        if (verticesA[indexA].cross == -1 || specialCaseThisToOther(indexA, booleantype))
        {
            step9();
        }

        else
        {
            step4();
        }
    }
    static void step4()
    {
        indexB = verticesA[indexA].cross;
        step5();
    }
    static void step5()
    {
        if(booleantype == Bool.Subtract)
            indexB = mod(indexB - 1, verticesB.Count);

        else if(booleantype == Bool.Add)
            indexB = mod(indexB + 1, verticesB.Count);

        step6();
    }
    static void step6()
    {
        outputPolygons.Last().Add(verticesB[indexB].pos);
        step7();
    }
    static void step7()
    {
        if (verticesB[indexB].cross == -1 || specialCaseOtherToThis(indexB))
        {
            step5();
        }

        else
        {
            step8();
        }
    }
    static void step8()
    {
        indexA = verticesB[indexB].cross;
        indexA = (indexA + 1) % verticesA.Count;
        step2();
    }
    static void step9()
    {
        indexA = (indexA + 1) % verticesA.Count;
        step2();
    }

    static bool specialCaseThisToOther(int iA, Bool type)
    {
        int vertexAfterCrossing = verticesA[iA].cross;
        int thisNext = mod(iA + 1, verticesA.Count);

        if (type == Bool.Subtract){
            vertexAfterCrossing = mod(vertexAfterCrossing - 1, verticesB.Count);
            if (verticesB[vertexAfterCrossing].outside == true)
                return true;
        }

        else{
            vertexAfterCrossing = mod(vertexAfterCrossing + 1, verticesB.Count);
            if (verticesB[vertexAfterCrossing].outside == false)
                return true;
        }

        if (verticesB[vertexAfterCrossing].cross == -1)
            return false;

        if (verticesB[vertexAfterCrossing].cross == thisNext)
            return true;


        return false;
    }
    static bool specialCaseOtherToThis(int iB) 
    {
        int vertexAfterCrossing = verticesB[iB].cross;
        vertexAfterCrossing = mod(vertexAfterCrossing + 1, verticesA.Count);
        int thisNext = mod(iB - 1, verticesB.Count);

        if (verticesA[vertexAfterCrossing].outside == false)
            return true;

        if (verticesA[vertexAfterCrossing].cross == -1)
            return false;

        if (verticesA[vertexAfterCrossing].cross == thisNext)
            return true;


        return false;
    }

}

public class Vertex
{
    public Vector2 pos;
    public bool outside;
    public int cross;
    public bool processed;

    public bool intersection;

    public Vertex(Vector2 pos){
        this.pos = pos;
        this.cross = -1;
    }
    public Vertex(Vector2 pos, bool inserted)
    {
        this.pos = pos;
        this.intersection = inserted;
        this.cross = -1;
    }
}
public enum Bool
{
    Subtract,
    Add
}
