using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static Transform FindChildRecursively(this Transform parent, string childName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
            var child = queue.Dequeue();
            if (child.name == childName)
                return child;
            foreach(Transform t in child)
                queue.Enqueue(t);
        }
        return null;
    }    
}
