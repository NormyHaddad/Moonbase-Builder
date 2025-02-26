using UnityEngine;
using UnityEditor;

public class CopyComponents : MonoBehaviour
{
    [MenuItem("Tools/Copy Components from Selected")]
    private static void CopySelectedComponents()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length < 2)
        {
            Debug.LogWarning("Please select a source object first, then a target object.");
            return;
        }

        GameObject source = selectedObjects[0];
        GameObject target = selectedObjects[1];

        // Loop through all components in the source object
        foreach (Component component in source.GetComponents<Component>())
        {
            // Skip the Transform component
            if (component is Transform) continue;

            // Add the same component type to the target object
            Component newComponent = target.AddComponent(component.GetType());

            // Copy the serialized values from the source component to the new one
            EditorUtility.CopySerialized(component, newComponent);
        }

        Debug.Log("Components copied from " + source.name + " to " + target.name);
    }
}