using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RasterizeUtils
{
    public static List<Vector4> ClipWithPlane(List<Vector4> vertices, ClipPlane clipPlane)
    {
        List<Vector4> clippedVertices = new List<Vector4>();

        for (int i = 0; i < vertices.Count; ++i)
        {
            int startIndex = i;
            int endIndex = (i + 1 + vertices.Count) % vertices.Count;

            // 边的起点
            Vector4 startVertex = vertices[startIndex];
            // 边的终点
            Vector4 endVertex = vertices[endIndex];

            bool startVertexIn = false;
            bool endVertexIn = false;

            switch (clipPlane)
            {
                case ClipPlane.Near:
                    if (startVertex.z > -startVertex.w)
                        startVertexIn = true;
                    if (endVertex.z > -endVertex.w)
                        endVertexIn = true;
                    break;
                case ClipPlane.Far:
                    if (startVertex.z < startVertex.w)
                        startVertexIn = true;
                    if (endVertex.z < endVertex.w)
                        endVertexIn = true;
                    break;
                case ClipPlane.Left:
                    if (startVertex.x > -startVertex.w)
                        startVertexIn = true;
                    if (endVertex.x > -endVertex.w)
                        endVertexIn = true;
                    break;
                case ClipPlane.Right:
                    if (startVertex.x < startVertex.w)
                        startVertexIn = true;
                    if (endVertex.x < endVertex.w)
                        endVertexIn = true;
                    break;
                case ClipPlane.Top:
                    if (startVertex.y < startVertex.w)
                        startVertexIn = true;
                    if (endVertex.y < endVertex.w)
                        endVertexIn = true;
                    break;
                case ClipPlane.Bottom:
                    if (startVertex.y > -startVertex.w)
                        startVertexIn = true;
                    if (endVertex.y > -endVertex.w)
                        endVertexIn = true;
                    break;
            }

        
            if (startVertexIn != endVertexIn)
            {
                float t = 0;
                switch (clipPlane)
                {
                    case ClipPlane.Near:
                        t = (startVertex.w + startVertex.z) / (-(endVertex.z - startVertex.z) - (endVertex.w - startVertex.w));
                        break;
                    case ClipPlane.Far:
                        t = (startVertex.w - startVertex.z) / ((endVertex.z - startVertex.z) - (endVertex.w - startVertex.w));
                        break;
                    case ClipPlane.Left:
                        t = (startVertex.w + startVertex.x) / (-(endVertex.x - startVertex.x) - (endVertex.w - startVertex.w));
                        break;
                    case ClipPlane.Right:
                        t = (startVertex.w - startVertex.x) / ((endVertex.x - startVertex.x) - (endVertex.w - startVertex.w));
                        break;
                    case ClipPlane.Top:
                        t = (startVertex.w - startVertex.y) / ((endVertex.y - startVertex.y) - (endVertex.w - startVertex.w));
                        break;
                    case ClipPlane.Bottom:
                        t = (startVertex.w + startVertex.y) / (-(endVertex.y - startVertex.y) - (endVertex.w - startVertex.w));
                        break;
                }

                Vector4 intersection = Vector4.Lerp(startVertex, endVertex, t);
                clippedVertices.Add(intersection);
            }

            if (endVertexIn)
            {
                clippedVertices.Add(endVertex);
            }
        }

        return clippedVertices;
    }

    public static bool IsInsideTriangle(float x, float y, Vertex vert1, Vertex vert2, Vertex vert3, bool back = false)
    {

        Vector3 v0v1 = new Vector3(vert2.vert_pixel.x - vert1.vert_pixel.x, vert2.vert_pixel.y - vert1.vert_pixel.y, 0);
        Vector3 v1v2 = new Vector3(vert3.vert_pixel.x - vert2.vert_pixel.x, vert3.vert_pixel.y - vert2.vert_pixel.y, 0);
        Vector3 v2v0 = new Vector3(vert1.vert_pixel.x - vert3.vert_pixel.x, vert1.vert_pixel.y - vert3.vert_pixel.y, 0);


        Vector3 v0p = new Vector3(x - vert1.vert_pixel.x, y - vert1.vert_pixel.y, 0);
        Vector3 v1p = new Vector3(x - vert2.vert_pixel.x, y - vert2.vert_pixel.y, 0);
        Vector3 v2p = new Vector3(x - vert3.vert_pixel.x, y - vert3.vert_pixel.y, 0);

        if (back)
        {
            if (Vector3.Cross(v0v1, v0p).z > 0
            && Vector3.Cross(v1v2, v1p).z > 0
            && Vector3.Cross(v2v0, v2p).z > 0)
                return true;
            else
                return false;
        }
        else
        {
            if (Vector3.Cross(v0v1, v0p).z < 0
            && Vector3.Cross(v1v2, v1p).z < 0
            && Vector3.Cross(v2v0, v2p).z < 0)
                return true;
            else
                return false;
        }

    }

    public static Vector3 BarycentricCoordinate(float x, float y, Vertex vert1, Vertex vert2, Vertex vert3)
    {
        Vector3 v1v2 = new Vector3(vert2.vert_pixel.x - vert1.vert_pixel.x, vert2.vert_pixel.y - vert1.vert_pixel.y, 0);
        Vector3 v2v3 = new Vector3(vert3.vert_pixel.x - vert2.vert_pixel.x, vert3.vert_pixel.y - vert2.vert_pixel.y, 0);
        Vector3 v3v1 = new Vector3(vert1.vert_pixel.x - vert3.vert_pixel.x, vert1.vert_pixel.y - vert3.vert_pixel.y, 0);


        Vector3 v1p = new Vector3(x - vert1.vert_pixel.x, y - vert1.vert_pixel.y, 0);
        Vector3 v2p = new Vector3(x - vert2.vert_pixel.x, y - vert2.vert_pixel.y, 0);
        Vector3 v3p = new Vector3(x - vert3.vert_pixel.x, y - vert3.vert_pixel.y, 0);

        // 因为v0v1v2,p的z坐标都是0，叉乘向量的x、y是0，直接取z当作模长
        float area_v2v3p = Mathf.Abs(Vector3.Cross(v2v3, v2p).z) / 2;
        float area_v3v1p = Mathf.Abs(Vector3.Cross(v3v1, v3p).z) / 2;
        float area_v1v2p = Mathf.Abs(Vector3.Cross(v1v2, v1p).z) / 2;
        float area_v1v2v3 = Mathf.Abs(Vector3.Cross(v1v2, v3v1).z) / 2;

        return new Vector3(area_v2v3p / area_v1v2v3, area_v3v1p / area_v1v2v3, area_v1v2p / area_v1v2v3);
    }

    public static double ComputeMipMapLevel(float x, float y, Vertex v1, Vertex v2, Vertex v3,Texture2D sampleTex2D)
    {

        float[] s0 = SampleTexel(x, y, v1, v2, v3, sampleTex2D);
        float[] s1 = SampleTexel(x + 1, y, v1, v2, v3, sampleTex2D);
        float[] s2 = SampleTexel(x, y + 1, v1, v2, v3, sampleTex2D);

        double dx = s1[0] - s0[0]; // 0 width 1 height
        double dy = s1[1] - s0[1];
        float L1 = (float)Math.Sqrt((dx * dx + dy * dy));

        dx = s2[0] - s0[0];
        dy = s2[1] - s0[1];
        float L2 = (float)Math.Sqrt((dx * dx + dy * dy));


        float Lmax = Mathf.Max(L1, L2);
        double level;
        if (Lmax < 1)
            level = 0;
        else
            level = Math.Log(Lmax, 2);

        return level;
    }


    public static float[] SampleTexel(float x, float y, Vertex v1, Vertex v2, Vertex v3, Texture2D sampleTex2D)
    {
        float[] f = new float[2];
        // 计算重心坐标
        Vector3 barycentricCoordinate = BarycentricCoordinate(x,y, v1, v2, v3);
        // 计算该像素在观察空间的深度值:观察空间中的z的倒数在屏幕空间是线性的，所以用重心坐标可以插值z的倒数，再进行转换求出该像素的观察空间中的深度
        float z_view = 1.0f / (barycentricCoordinate.x / v1.vert_proj.w + barycentricCoordinate.y / v2.vert_proj.w + barycentricCoordinate.z / v3.vert_proj.w);

        Vector2 uv = z_view * (v1.uv / v1.vert_view.z * barycentricCoordinate.x +
                                v2.uv / v2.vert_view.z * barycentricCoordinate.y +
                                v3.uv / v3.vert_view.z * barycentricCoordinate.z);
        float width = uv.x * sampleTex2D.width;
        float height = uv.y * sampleTex2D.height;
        f[0] = width;
        f[1] = height;
        return f;
    }

    public static Color GetColorByBilinear(float[] texel, Texture2D sampleTex2D ,int level = 0)
    {
        
        int texWidth = (int)texel[0];
        int texHeight = (int)texel[1];

        float fw = GetFloat(texel[0]);
        float fh = GetFloat(texel[1]);

        texWidth = texWidth / (int)Math.Pow(2, level);
        texHeight = texHeight / (int)Math.Pow(2, level);

        int sampleWidthMipMap = sampleTex2D.width / (int)Math.Pow(2, level);
        int sampleHeighthMipMap = sampleTex2D.height / (int)Math.Pow(2, level);

        int dx = texWidth + 1 > sampleWidthMipMap ? sampleWidthMipMap - 1 : texWidth;
        int dxx = texWidth + 2 > sampleWidthMipMap ? sampleWidthMipMap - 2 : texWidth;
        int dy = texHeight + 1 > sampleHeighthMipMap ? sampleHeighthMipMap - 1 : texHeight;
        int dyy = texHeight + 2 > sampleHeighthMipMap ? sampleHeighthMipMap - 2 : texHeight;
        Color c1 = sampleTex2D.GetPixels(dx, dy, 1, 1, level)[0];
        Color c2 = sampleTex2D.GetPixels(dxx + 1, dy, 1, 1, level)[0];
        Color c3 = sampleTex2D.GetPixels(dx, dyy + 1, 1, 1, level)[0];
        Color c4 = sampleTex2D.GetPixels(dxx + 1, dyy + 1, 1, 1, level)[0];

        Color c1c2 = Color.Lerp(c1, c2, fw);
        Color c3c4 = Color.Lerp(c3, c4, fw);
        Color c1c2c3c4 = Color.Lerp(c1c2, c3c4, fh);
        return c1c2c3c4;
        //
    }


    public static Vertex[] GetVertexArray(MeshFilter meshFilter)
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        int[] indices = meshFilter.mesh.triangles;//三角形索引数组的顺序来绘制
        Vector2[] uvs = meshFilter.mesh.uv;
        Vector3[] normals = meshFilter.mesh.normals;
        Vector4[] tangents = meshFilter.mesh.tangents;
        Color[] colors = meshFilter.mesh.colors;


        Vertex[] vertexArray = new Vertex[vertices.Length];
        if (colors.Length > 0)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertexArray[i] = new Vertex(vertices[i], uvs[i], normals[i], tangents[i], colors[i]);
        }
        else
        {
            for (int i = 0; i < vertexArray.Length; i++)
                vertexArray[i] = new Vertex(vertices[i], uvs[i], normals[i], tangents[i], Color.white);
        }

        return vertexArray;
    }

    public static float GetFloat(float a)
    {
        int b = (int)a;
        float c = a - b;
        return c;
    }

    public static float GetFloat(double a)
    {
        int b = (int)a;
        float c = (float)(a - b);
        return c;
    }

}



