using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameBuffer 
{
    private int texWidth;
    private int texHeight;
    private Texture2D depthBufferTexture;

    public FrameBuffer(int width, int height)
    {
        texWidth = width;
        texHeight = height;
        depthBufferTexture = new Texture2D(width, height, TextureFormat.RFloat, false);
        InitBuffer();


    }

    public void SetDepthBuffer(int x, int y, float depth)
    {
        depthBufferTexture.SetPixel(x, y, new Color(depth, 0, 0));
    }

    public float GetDepthBuffer(int x, int y)
    {
        return depthBufferTexture.GetPixel(x, y).r;
    }

    public void Apply()
    {
        depthBufferTexture.Apply();
    }


    public void InitBuffer()
    {
        for (int i = 0; i < texWidth; i++)
        {
            for (int j = 0; j < texHeight; j++)
            {
                SetDepthBuffer(i, j, float.PositiveInfinity);
            }
        }
    }
}
