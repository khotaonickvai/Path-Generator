using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static void SetXPosition(this Transform targetTransform, float newX)
    {
        var currentPos = targetTransform.position;
        currentPos.x = newX;
        targetTransform.position = currentPos;
    }
    
    public static void SetYPosition(this Transform targetTransform, float newY)
    {
        var currentPos = targetTransform.position;
        currentPos.y = newY;
        targetTransform.position = currentPos;
    }

    public static void SetSizeDeltaX(this RectTransform rectTransform, float newX)
    {
        var sizeDelta = rectTransform.sizeDelta;
        sizeDelta.x = newX;
        rectTransform.sizeDelta = sizeDelta;
    }
    
    public static void SetSizeDeltaY(this RectTransform rectTransform, float newY)
    {
        var sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = newY;
        rectTransform.sizeDelta = sizeDelta;
    }

    public static float ConvertDirectionToZAngle(this Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Convert the angle to be within the range of 0 to 360 degrees
        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }
    
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        // Check if the file exists
        if (System.IO.File.Exists(filePath))
        {
            // Read the bytes from the file
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);

            // Create a new texture
            Texture2D texture = new Texture2D(2, 2);

            // Load the image data into the texture
            if (texture.LoadImage(fileData))
            {
                Debug.Log("Texture loaded successfully.");
                return texture;
            }
            else
            {
                Debug.LogError("Failed to load texture from file.");
                return null;
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }
    }
}
