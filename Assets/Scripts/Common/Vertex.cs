using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public Vector3 vert_pos;
    public Vector2 uv;
    public Vector3 normal;
    public Vector4 tangent;
    public Color color;

    public Vector3 vert_world;
    public Vector4 vert_view;
    public Vector4 vert_proj;

    public Vector3 vert_ndc;
    public Vector3 vert_viewport;
    public Vector2 vert_pixel;
    public Vertex(Vector3 _vert_pos, Vector2 _uv, Vector3 _normal, Vector4 _tangent, Color _color)
    {
        vert_pos = _vert_pos;
        uv = _uv;
        normal = _normal;
        tangent = _tangent;
        color = _color;
    }


    public void UNITY_MATRIX_MVP(Matrix4x4 model,Matrix4x4 view, Matrix4x4 projection)
    {
        vert_world = model.MultiplyPoint(vert_pos);
        vert_view = view.MultiplyPoint(vert_world);
        vert_view.w = 1;
        vert_proj = projection * vert_view;
    }


    public void UNITY_NDC()
    {
        vert_ndc = new Vector3(vert_proj.x / vert_proj.w, vert_proj.y / vert_proj.w, vert_proj.z / vert_proj.w);
    }


    public void UNITY_TRANSFER_VIEWPORT()
    {
        vert_viewport = new Vector3((vert_ndc.x + 1) / 2, (vert_ndc.y + 1) / 2, 0);
    }


    public void UNITY_TRANSFER_PIXEL()
    {
        vert_pixel = new Vector3(vert_viewport.x * Screen.width, vert_viewport.y * Screen.height);
    }


    public void UnityObjectToViewPort(Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
    {
        UNITY_MATRIX_MVP(model, view, projection);
        UNITY_NDC();
        UNITY_TRANSFER_VIEWPORT();
    }
}
