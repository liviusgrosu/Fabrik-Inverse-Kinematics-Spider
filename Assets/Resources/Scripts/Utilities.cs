using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Set of useful utility functions to call into
/// </summary>
public static class Utilities {
    /// <summary>
    /// Find the child with a specificed name attached to the parent regardless of level/dimension
    /// </summary>
    /// <params name="parent">The parent object</returns>
    /// <params name="childName">The child object to find</returns>
    /// <returns>The child with the specified name or null</returns>
    public static Transform FindChildRecursively(Transform parent, string childName) {
        // Store each child of the current level into a queue
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0) {
            var child = queue.Dequeue();
            if (child.name == childName) {
                // Return the child if one of the current layer children shares the same name
                return child;
            }
            foreach(Transform t in child) {
                // Add the children of the current children into the queue
                queue.Enqueue(t);
            }
        }
        return null;
    }    
}
