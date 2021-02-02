using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color lineColor;

    private List<Transform> nodes = new List<Transform>();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        Transform[] pathTtransforms = GetComponentsInChildren<Transform>();

        for (int i = 0; i < pathTtransforms.Length; i++)
        {
            if (pathTtransforms[i] != transform)
            {
                nodes.Add(pathTtransforms[i]);
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector2 currentNode = nodes[i].position;
            Vector2 previousNode = nodes[(nodes.Count - 1 + i) % nodes.Count].position;

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.3f);
        }
    }
}
