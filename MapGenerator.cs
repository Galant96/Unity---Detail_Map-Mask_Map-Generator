using UnityEditor;
using UnityEngine;

/// <summary>
/// TIPS:
/// 1. Ensure the "Read/Write Enabled" option is ticked in the texture's advanced settings.
/// 2. Change the Normal Map texture type to default, as the normal map type stores information about the direction of normals      rather than RGBA values.
/// 3. In the end of the process, remember to set the Normal Map texture type back to normal map type.
/// </summary>

/// <summary>
/// MapGenerator class description:
/// - This class represents the main window for the Map Generator tool in the Unity Editor.
/// - It provides functionality to generate both Detail Maps and Mask Maps based on specified settings.
/// - Contains fields for various textures, file paths, and file names used in the generation process.
/// - Displays buttons and input fields for user interaction in the Unity Editor.
/// - Utilizes the TextureManipulatorLibrary to perform texture manipulations and saving.
/// </summary>
public class MapGenerator : EditorWindow
{
    // Generated File Details
    private string filePath = "Assets/Materials/Brick";
    private string detailMapName = "detail_map";
    private string maskMapName = "mask_map";

    // Detail Map Fields    
    private Texture2D diffuseTexture;
    private Texture2D normalTexture;
    private Texture2D roughnessTextureDM;

    private Texture2D desaturatedDiffuseTexture;
    private Texture2D smoothnessTextureDM;
    private Texture2D redChannelTexture;
    private Texture2D greenChannelTexture;
    private Texture2D blueChannelTexture;
    private Texture2D alphaChannelTexture;
    private Texture2D detailMapTexture;


    // Mask Map Fields    
    private Texture2D metallicTexture;
    private Texture2D aoTexture;
    private Texture2D detailMapTextureMM;
    private Texture2D roughnessTextureMM;
    private Texture2D maskMapTexture;
    private Texture2D smoothnessTextureMM;

    // Other Options
    private bool useMetallicTexture;
    private float metallicValue;
    private bool useRoughnessTextureDM;
    private bool useRoughnessTextureMM;
    private float roughnessValueDM;
    private float roughnessValueMM;



    [MenuItem("Tools/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapGenerator>("Map Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Map Generator", EditorStyles.boldLabel);

        if (GUILayout.Button(new GUIContent("Show Instructions", "Display instructions on how to use the Map Generator")))
        {
            ShowInstructionsPopup();
        }

        DisplayDetailMapSettings();

        if (GUILayout.Button(new GUIContent("Generate Detail Map", "Generate a detail map using the specified settings")))
        {
            GenerateDetailMap();
        }

        DisplayMaskMapSettings();

        if (GUILayout.Button(new GUIContent("Generate Mask Map", "Generate a mask map using the specified settings")))
        {
            GenerateMaskMap();
        }
    }

    private void ShowInstructionsPopup()
    {
        InstructionsPopup window = InstructionsPopup.Create();
        window.ShowUtility();
    }

    private void DisplayDetailMapSettings()
    {
        EditorGUILayout.LabelField("Detail Map Settings", EditorStyles.boldLabel);

        // Add tooltips to GUI elements
        diffuseTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Diffuse", "The diffuse texture used for detail map generation"), diffuseTexture, typeof(Texture2D), false);
        normalTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Normal Map", "The normal map texture used for detail map generation"), normalTexture, typeof(Texture2D), false);

        // Option to choose whether to use roughness map or value
        useRoughnessTextureDM = EditorGUILayout.Toggle("Use Roughness Map", useRoughnessTextureDM);

        // If using roughness map, allow user to assign the texture
        if (useRoughnessTextureDM)
        {
            roughnessTextureDM = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Roughness", "The roughness texture used for detail map generation"), roughnessTextureDM, typeof(Texture2D), false);
        }
        else
        {
            // If not using roughness map, allow user to input numeric value
            roughnessValueDM = EditorGUILayout.Slider("Roughness Value", roughnessValueDM, 0f, 1f);

        }

        detailMapTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Detail Map", "The generated detail map texture"), detailMapTexture, typeof(Texture2D), false);

        // Additional input fields for file name and path
        EditorGUILayout.LabelField("File Saving Details", EditorStyles.boldLabel);
        detailMapName = EditorGUILayout.TextField("File Name", detailMapName);
        filePath = EditorGUILayout.TextField("File Path", filePath);
    }

    private void DisplayMaskMapSettings()
    {
        EditorGUILayout.LabelField("Mask Map Settings", EditorStyles.boldLabel);

        // Add tooltips to GUI elements

        // Option to choose whether to use metallic map or value
        useMetallicTexture = EditorGUILayout.Toggle("Use Metallic Map", useMetallicTexture);

        // If using metallic map, allow user to assign the texture
        if (useMetallicTexture)
        {
            metallicTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Metallic", "The metallic texture used for mask map generation"), metallicTexture, typeof(Texture2D), false);
        }
        else
        {
            // If not using metallic map, allow user to input numeric value
            metallicValue = EditorGUILayout.Slider("Metallic Value", metallicValue, 0f, 1f);
        }

        aoTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Ambient Occlusion", "The ambient occlusion texture used for mask map generation"), aoTexture, typeof(Texture2D), false);

        detailMapTextureMM = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Detail Mask", "The detail mask texture used for mask map generation"), detailMapTextureMM, typeof(Texture2D), false);

        useRoughnessTextureMM = EditorGUILayout.Toggle("Use Roughness Map", useRoughnessTextureMM);

        // If using roughness map, allow user to assign the texture
        if (useRoughnessTextureMM)
        {
            roughnessTextureMM = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Roughness", "The roughness texture used for detail map generation"), roughnessTextureMM, typeof(Texture2D), false);
        }
        else
        {
            // If not using roughness map, allow user to input numeric value
            roughnessValueMM = EditorGUILayout.Slider("Roughness Value", roughnessValueMM, 0f, 1f);
        }

        maskMapTexture = (Texture2D)EditorGUILayout.ObjectField(new GUIContent("Mask Map", "The generated mask map texture"), maskMapTexture, typeof(Texture2D), false);

        // Additional input fields for file name and path
        EditorGUILayout.LabelField("File Saving Details", EditorStyles.boldLabel);
        maskMapName = EditorGUILayout.TextField("File Name", maskMapName);
        filePath = EditorGUILayout.TextField("File Path", filePath);
    }

    private void GenerateDetailMap()
    {
        desaturatedDiffuseTexture = TextureManipulatorLibrary.Desaturate(diffuseTexture);

        if (!useRoughnessTextureDM)
        {
            // Check if roughnessTextureDM is null and instantiate it if necessary
            if (roughnessTextureDM == null)
            {
                roughnessTextureDM = new Texture2D(diffuseTexture.width, diffuseTexture.height);
            }

            roughnessTextureDM = TextureManipulatorLibrary.GenerateRoughnessTexture(roughnessValueDM, roughnessTextureDM.width, roughnessTextureDM.height);
        }
        smoothnessTextureDM = TextureManipulatorLibrary.ConvertRoughnessToSmoothness(roughnessTextureDM);

        redChannelTexture = TextureManipulatorLibrary.GetChannel(normalTexture, TextureManipulatorLibrary.ColorChannel.Red);
        greenChannelTexture = TextureManipulatorLibrary.GetChannel(normalTexture, TextureManipulatorLibrary.ColorChannel.Green);
        blueChannelTexture = TextureManipulatorLibrary.GetChannel(normalTexture, TextureManipulatorLibrary.ColorChannel.Blue);

        // Generate Detail Map Texture
        detailMapTexture = TextureManipulatorLibrary.GenerateDetailMap(desaturatedDiffuseTexture, greenChannelTexture, smoothnessTextureDM, redChannelTexture);
        detailMapTexture = (Texture2D)EditorGUILayout.ObjectField("Detail Map", detailMapTexture, typeof(Texture2D), false);

        Debug.Log("Generating Detail Map...");

        TextureManipulatorLibrary.SaveTextureToFile(filePath, detailMapName, detailMapTexture);
    }

    private void GenerateMaskMap()
    {
        if (!useMetallicTexture)
        {
            // Check if roughnessTextureDM is null and instantiate it if necessary
            if (metallicTexture == null)
            {
                metallicTexture = new Texture2D(diffuseTexture.width, diffuseTexture.height);
            }
            metallicTexture = TextureManipulatorLibrary.GenerateMetallicTexture(metallicValue, detailMapTextureMM.width, detailMapTextureMM.height);
        }

        if (!useRoughnessTextureMM)
        {
            // Check if roughnessTextureDM is null and instantiate it if necessary
            if (roughnessTextureMM == null)
            {
                roughnessTextureMM = new Texture2D(diffuseTexture.width, diffuseTexture.height);
            }

            roughnessTextureMM = TextureManipulatorLibrary.GenerateRoughnessTexture(roughnessValueMM, roughnessTextureMM.width, roughnessTextureMM.height);
        }
        smoothnessTextureMM = TextureManipulatorLibrary.ConvertRoughnessToSmoothness(roughnessTextureMM);

        maskMapTexture = TextureManipulatorLibrary.GenerateMaskMap(metallicTexture, aoTexture, detailMapTextureMM, roughnessTextureMM);
        maskMapTexture = (Texture2D)EditorGUILayout.ObjectField("Mask Map", maskMapTexture, typeof(Texture2D), false);

        Debug.Log("Generating Mask Map...");

        TextureManipulatorLibrary.SaveTextureToFile(filePath, maskMapName, maskMapTexture);
    }

}

/// <summary>
/// InstructionsPopup class description:
/// - This class represents a popup window that displays instructions on how to use the Map Generator tool.
/// - It is a separate window created by the MapGenerator class to provide additional information to users.
/// - Displays a label, an information box with usage instructions, and a close button.
/// </summary>

public class InstructionsPopup : EditorWindow
{
    public static InstructionsPopup Create()
    {
        InstructionsPopup window = GetWindow<InstructionsPopup>("Instructions");
        window.minSize = new Vector2(300, 150);
        return window;
    }

    void OnGUI()
    {
        GUILayout.Label("Tips", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("1. Ensure the 'Read/Write Enabled' option is ticked in the texture's advanced settings.\n2. Change the normal map texture type to default.\n3. In the end of the process, remember to set the normal map texture type back to normal map.", MessageType.Info);

        GUILayout.Label("MapGenerator class description:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("The MapGenerator class represents the main window for the Map Generator tool in the Unity Editor. It provides functionality to generate both Detail Maps and Mask Maps based on specified settings. The class contains fields for various textures, file paths, and file names used in the generation process. It displays buttons and input fields for user interaction in the Unity Editor and utilizes the TextureManipulatorLibrary to perform texture manipulations and saving.", MessageType.Info);

        GUILayout.Label("InstructionsPopup class description:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("The InstructionsPopup class represents a popup window that displays instructions on how to use the Map Generator tool. It is a separate window created by the MapGenerator class to provide additional information to users. The class displays a label, an information box with usage instructions, and a close button.", MessageType.Info);

        GUILayout.Label("TextureManipulatorLibrary class description:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("The TextureManipulatorLibrary class provides various static methods for manipulating textures. It includes functions for desaturation, extracting color channels, converting roughness to smoothness, generating detail maps, creating black textures, and saving textures to files. This library is essential for texture-related operations within the Map Generator tool.", MessageType.Info);

        if (GUILayout.Button("Close"))
        {
            Close();
        }
    }
}
