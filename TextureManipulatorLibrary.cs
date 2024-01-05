using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// TIPS:
/// 1. Ensure the "Read/Write Enabled" option is ticked in the texture's advanced settings.
/// 2. Change the Normal Map texture type to default, as the normal map type stores information about the direction of normals      rather than RGBA values.
/// 3. In the end of the process, remember to set the Normal Map texture type back to normal map type.
/// </summary>

/// <summary>
/// TextureManipulatorLibrary class description:
/// - This is a utility library containing various texture manipulation functions.
/// - Used by the MapGenerator class to perform specific texture manipulations during map generation.
/// </summary>

public static class TextureManipulatorLibrary
{
    // Enum to represent color channels
    public enum ColorChannel
    {
        Red,
        Green,
        Blue,
        Alpha
    }

    /// <summary>
    /// Desaturate the input texture.
    /// </summary>
    /// <param name="originalTexture">The texture to desaturate.</param>
    /// <returns>The desaturated texture.</returns>
    public static Texture2D Desaturate(Texture2D originalTexture)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;

        Color[] pixels = originalTexture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            // Apply the luminosity formula to convert to grayscale
            float luminosity = pixels[i].r * 0.3f + pixels[i].g * 0.59f + pixels[i].b * 0.11f;

            pixels[i] = new Color(luminosity, luminosity, luminosity, pixels[i].a);
        }

        Texture2D desaturatedTexture = new Texture2D(width, height);
        desaturatedTexture.SetPixels(pixels);
        desaturatedTexture.Apply();

        return desaturatedTexture;
    }

    /// <summary>
    /// Get a specific color channel from the input texture.
    /// </summary>
    /// <param name="originalTexture">The texture to extract the channel from.</param>
    /// <param name="channel">The color channel to extract.</param>
    /// <returns>The extracted channel as a new texture.</returns>
    public static Texture2D GetChannel(Texture2D originalTexture, ColorChannel channel)
    {
        int width = originalTexture.width;
        int height = originalTexture.height;

        // Get the pixels from the original texture
        Color[] pixels = originalTexture.GetPixels();

        // Create an array to store the selected channel
        float[] channelValues = new float[pixels.Length];

        // Extract the specified channel values
        for (int i = 0; i < pixels.Length; i++)
        {
            switch (channel)
            {
                case ColorChannel.Red:
                    channelValues[i] = pixels[i].r;
                    break;
                case ColorChannel.Green:
                    channelValues[i] = pixels[i].g;
                    break;
                case ColorChannel.Blue:
                    channelValues[i] = pixels[i].b;
                    break;
                case ColorChannel.Alpha:
                    channelValues[i] = pixels[i].a;
                    break;
            }
        }

        // Create a new Texture2D for the channel
        Texture2D channelTexture = new Texture2D(width, height);

        // Set the grayscale values to the new texture
        for (int i = 0; i < channelValues.Length; i++)
        {
            channelTexture.SetPixel(i % width, i / width, new Color(channelValues[i], channelValues[i], channelValues[i], 1.0f));
        }

        channelTexture.Apply();

        return channelTexture;
    }

    /// <summary>
    /// Convert roughness values to smoothness in the input texture.
    /// </summary>
    /// <param name="originalTexture">The texture containing roughness values.</param>
    /// <returns>The converted smoothness texture.</returns>
    public static Texture2D ConvertRoughnessToSmoothness(Texture2D originalTexture)
    {

        int width = originalTexture.width;
        int height = originalTexture.height;

        Color[] roughnessPixels = originalTexture.GetPixels();
        Color[] smoothnessPixels = new Color[roughnessPixels.Length];

        for (int i = 0; i < roughnessPixels.Length; i++)
        {
            // Convert roughness to smoothness using the formula: Smoothness = 1.0 - Roughness
            float smoothnessValue = 1.0f - roughnessPixels[i].r;

            // Set the smoothness value to the new pixel
            smoothnessPixels[i] = new Color(smoothnessValue, smoothnessValue, smoothnessValue, 1.0f);
        }

        // Create a new texture for smoothness
        Texture2D smoothnessTexture = new Texture2D(width, height);
        smoothnessTexture.SetPixels(smoothnessPixels);
        smoothnessTexture.Apply();

        return smoothnessTexture;

    }

    /// <summary>
    /// Generate a detail map using multiple input textures.
    /// </summary>
    /// <param name="desaturatedDiffuse">Desaturated albedo texture.</param>
    /// <param name="normalMapGreen">Green channel of the normal map.</param>
    /// <param name="smoothnessMap">Smoothness map texture.</param>
    /// <param name="normalMapRed">Red channel of the normal map.</param>
    /// <returns>The generated detail map texture.</returns>
    public static Texture2D GenerateDetailMap(Texture2D desaturatedDiffuse, Texture2D normalMapGreen, Texture2D smoothnessMap, Texture2D normalMapRed)
    {
        int width = desaturatedDiffuse.width;
        int height = desaturatedDiffuse.height;

        Color[] desaturatedAlbedoPixels = desaturatedDiffuse.GetPixels();
        Color[] normalMapGreenPixels = normalMapGreen.GetPixels();
        Color[] smoothnessMapPixels = smoothnessMap.GetPixels();
        Color[] normalMapRedPixels = normalMapRed.GetPixels();

        Color[] detailPixels = new Color[desaturatedAlbedoPixels.Length];

        for (int i = 0; i < desaturatedAlbedoPixels.Length; i++)
        {
            // Extract components from individual textures
            float red = desaturatedAlbedoPixels[i].r;
            float green = normalMapGreenPixels[i].r;
            float blue = smoothnessMapPixels[i].r;
            float alpha = normalMapRedPixels[i].r;

            // Combine components into the detail map
            detailPixels[i] = new Color(red, green, blue, alpha);
        }

        // Create a new texture for the detail map
        Texture2D detailMapTexture = new Texture2D(width, height);
        detailMapTexture.SetPixels(detailPixels);
        detailMapTexture.Apply();

        return detailMapTexture;
    }

    /// <summary>
    /// Generate a mask map using multiple input textures.
    /// </summary>
    /// <param name="metallicMap">Metallic map texture.</param>
    /// <param name="aoMap">Ambient occlusion map texture.</param>
    /// <param name="detailMaskMap">Detail mask texture.</param>
    /// <param name="roughnessMap">Roughness map texture.</param>
    /// <returns>The generated mask map texture.</returns>
    public static Texture2D GenerateMaskMap(Texture2D metallicMap, Texture2D aoMap, Texture2D detailMaskMap, Texture2D roughnessMap)
    {
        int width = metallicMap.width;
        int height = metallicMap.height;

        Color[] metallicPixels = metallicMap.GetPixels();
        Color[] aoPixels = aoMap.GetPixels();
        Color[] detailMaskPixels = detailMaskMap.GetPixels();
        Color[] roughnessPixels = roughnessMap.GetPixels();

        Color[] maskMapPixels = new Color[metallicPixels.Length];

        for (int i = 0; i < metallicPixels.Length; i++)
        {
            // Extract components from individual textures
            float metallic = metallicPixels[i].r;
            float ao = aoPixels[i].r;
            float detailMask = detailMaskPixels[i].r;
            float roughness = roughnessPixels[i].r;

            // Combine components into the mask map
            // You may customize this logic based on your specific requirements
            Color maskPixel = new Color(metallic, ao, detailMask, roughness);
            maskMapPixels[i] = maskPixel;
        }

        // Create a new texture for the mask map
        Texture2D maskMapTexture = new Texture2D(width, height);
        maskMapTexture.SetPixels(maskMapPixels);
        maskMapTexture.Apply();

        return maskMapTexture;
    }

    /// <summary>
    /// Save a texture to a file with the specified folder path and file name.
    /// </summary>
    /// <param name="folderPath">The folder path to save the texture.</param>
    /// <param name="fileName">The name of the file (without extension).</param>
    /// <param name="texture">The texture to save.</param>
    public static void SaveTextureToFile(string folderPath, string fileName, Texture2D texture)
    {
        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Convert the texture to bytes
        byte[] bytes = texture.EncodeToPNG();

        // Combine the folder path and file name
        string filePath = Path.Combine(folderPath, fileName + ".png");

        // Write the bytes to the file
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Texture saved to: " + filePath);

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Create a black texture with the specified width and height.
    /// </summary>
    /// <param name="width">The width of the black texture.</param>
    /// <param name="height">The height of the black texture.</param>
    /// <returns>The created black texture.</returns>
    public static Texture2D CreateBlackTexture(int width, int height)
    {
        // Create a new Texture2D
        Texture2D blackTexture = new Texture2D(width, height);

        // Set all pixel values to black
        Color[] blackPixels = new Color[width * height];
        for (int i = 0; i < blackPixels.Length; i++)
        {
            blackPixels[i] = Color.black;
        }

        // Apply the black pixels to the texture
        blackTexture.SetPixels(blackPixels);
        blackTexture.Apply();

        return blackTexture;
    }

    /// <summary>
    /// Generate a metallic texture based on a numeric value.
    /// </summary>
    /// <param name="metallicValue">The metallic value (between 0 and 1).</param>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <returns>The generated metallic texture.</returns>
    public static Texture2D GenerateMetallicTexture(float metallicValue, int width, int height)
    {
        Color[] metallicPixels = new Color[width * height];

        // Fill the texture with the same metallic value
        for (int i = 0; i < metallicPixels.Length; i++)
        {
            metallicPixels[i] = new Color(metallicValue, metallicValue, metallicValue, 1.0f);
        }

        // Create a new texture for metallic
        Texture2D metallicTexture = new Texture2D(width, height);
        metallicTexture.SetPixels(metallicPixels);
        metallicTexture.Apply();

        return metallicTexture;
    }

    /// <summary>
    /// Generate a roughness texture based on a numeric value.
    /// </summary>
    /// <param name="roughnessValue">The roughness value (between 0 and 1).</param>
    /// <param name="width">The width of the texture.</param>
    /// <param name="height">The height of the texture.</param>
    /// <returns>The generated roughness texture.</returns>
    public static Texture2D GenerateRoughnessTexture(float roughnessValue, int width, int height)
    {
        Color[] roughnessPixels = new Color[width * height];

        // Fill the texture with the same roughness value
        for (int i = 0; i < roughnessPixels.Length; i++)
        {
            roughnessPixels[i] = new Color(roughnessValue, roughnessValue, roughnessValue, 1.0f);
        }

        // Create a new texture for roughness
        Texture2D roughnessTexture = new Texture2D(width, height);
        roughnessTexture.SetPixels(roughnessPixels);
        roughnessTexture.Apply();

        return roughnessTexture;
    }

}

