using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphScript : MonoBehaviour {

    public int columns;
    public int rows;

    public List<GameObject> nodes = new List<GameObject>();

	void Start () {
        BuildGrid();
	}

    void BuildGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject node = Instantiate(Resources.Load<GameObject>("Prefabs/Node"), gameObject.transform);

                node.transform.position = new Vector3(gameObject.transform.position.x + i, 0, gameObject.transform.position.z + j);
                node.GetComponent<NodeScript>().nodePos = new Vector2(i, j);
                node.name = "Node_" + i + "_" + j;

                nodes.Add(node);
            }
        }

        NodeConnections();
    }

    void NodeConnections()
    {
        foreach (GameObject node in nodes)
        {
            node.GetComponent<NodeScript>().makeConnections();
        }
    }
}
