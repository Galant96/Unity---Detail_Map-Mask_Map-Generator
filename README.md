# Unity - Detail_Map-Mask_Map-Generator - Map Generator

## Overview
The Map Generator is a Unity Editor tool designed for effortless creation of Detail Maps and Mask Maps. This tool simplifies texture generation through an intuitive interface, backed by the powerful `TextureManipulatorLibrary`, enhancing workflow efficiency.

## Features
- **Detail Map Generation:** Easily create detailed maps with customizable options for diffuse, normal maps, and roughness settings.
- **Mask Map Generation:** Generate mask maps with flexibility in defining metallic, ambient occlusion, and roughness parameters.
- **User-Friendly Interface:** The `MapGenerator` class offers a straightforward UI for a seamless user experience.
- **Instructions Popup:** Access helpful tips and usage instructions through the "Show Instructions" button.
- **File Saving Options:** Specify file names and paths for saving generated maps.

## Instructions
1. Open the tool from the Unity Editor menu under "Tools/Map Generator."
2. Follow on-screen instructions to set up Detail and Mask Map parameters.
3. Generate maps with the respective buttons.
4. Adjust file names and paths as needed.
5. Check the "Show Instructions" button for additional tips.

## Tips
- Ensure "Read/Write Enabled" in texture settings.
- Temporarily set Normal Map texture type to default.
- Reset Normal Map texture type after completion.

## Class Descriptions
- **MapGenerator:** Main window for the Map Generator tool, handling UI and map generation.
- **InstructionsPopup:** Popup window displaying usage instructions.
- **TextureManipulatorLibrary:** Library providing essential methods for texture manipulation and saving.

## How to Install
1. Clone the repository.
2. Copy the "MapGenerator" folder into your Unity project's "Editor" folder.

## Acknowledgments
This project utilizes the `TextureManipulatorLibrary` for efficient texture operations.

