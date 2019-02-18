using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour
{
    public LayerMask nodeLayer;
    public Vector3 nodePos;
    public bool deadNode;

    [SerializeField]
    int numOfConnections;

    #region Connections of the Node
    public Connection top;
    public Connection bottom;
    public Connection right;
    public Connection left;
    public Connection topRight;
    public Connection bottomRight;
    public Connection topLeft;
    public Connection bottomLeft;
    #endregion

    public void makeConnections()
    {
        #region gameobject find method
        //GameObject.Find Method
        //GameObject otherNode = GameObject.Find("Node_" + (nodePos.x + 0) + "_" + (nodePos.y + 1));

        #region finding nodes
        GameObject topNode = GameObject.Find("Node_" + (nodePos.x) + "_" + (nodePos.y + 1));
        GameObject bottomNode = GameObject.Find("Node_" + (nodePos.x) + "_" + (nodePos.y - 1));
        GameObject leftNode = GameObject.Find("Node_" + (nodePos.x - 1)+ "_" + (nodePos.y));
        GameObject rightNode = GameObject.Find("Node_" + (nodePos.x + 1) + "_" + (nodePos.y));
        GameObject topLeftNode = GameObject.Find("Node_" + (nodePos.x - 1) + "_" + (nodePos.y + 1));
        GameObject topRightNode = GameObject.Find("Node_" + (nodePos.x + 1) + "_" + (nodePos.y + 1));
        GameObject bottomLeftNode = GameObject.Find("Node_" + (nodePos.x - 1) + "_" + (nodePos.y - 1));
        GameObject bottomRightNode = GameObject.Find("Node_" + (nodePos.x + 1) + "_" + (nodePos.y - 1));
        #endregion

        #region applying the nodes
        if (topNode != null)
        {
            numOfConnections++;

            top.fromNode = gameObject;
            top.toNode = topNode;
            Vector3 newNodePos = topNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            top.weight = (newNodePos - objPos).magnitude;
        }
        if (bottomNode != null)
        {
            numOfConnections++;

            bottom.fromNode = gameObject;
            bottom.toNode = bottomNode;
            Vector3 newNodePos = bottomNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            bottom.weight = (newNodePos - objPos).magnitude;
        }
        if (leftNode != null)
        {
            numOfConnections++;

            left.fromNode = gameObject;
            left.toNode = leftNode;
            Vector3 newNodePos = leftNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            left.weight = (newNodePos - objPos).magnitude;
        }
        if (rightNode != null)
        {
            numOfConnections++;

            right.fromNode = gameObject;
            right.toNode = rightNode;
            Vector3 newNodePos = rightNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            right.weight = (newNodePos - objPos).magnitude;
        }
        if (topRightNode != null)
        {
            numOfConnections++;

            topRight.fromNode = gameObject;
            topRight.toNode = topRightNode;
            Vector3 newNodePos = topRightNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            topRight.weight = (newNodePos - objPos).magnitude;
        }
        if (topLeftNode != null)
        {
            numOfConnections++;

            topLeft.fromNode = gameObject;
            topLeft.toNode = topLeftNode;
            Vector3 newNodePos = topLeftNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            topLeft.weight = (newNodePos - objPos).magnitude;
        }
        if (bottomLeftNode != null)
        {
            numOfConnections++;

            bottomLeft.fromNode = gameObject;
            bottomLeft.toNode = bottomLeftNode;
            Vector3 newNodePos = bottomLeftNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            bottomLeft.weight = (newNodePos - objPos).magnitude;
        }
        if (bottomRightNode != null)
        {
            numOfConnections++;

            bottomRight.fromNode = gameObject;
            bottomRight.toNode = bottomRightNode;
            Vector3 newNodePos = bottomRightNode.transform.position;
            Vector3 objPos = gameObject.transform.position;
            bottomRight.weight = (newNodePos - objPos).magnitude;
        }
        #endregion

        #endregion
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Environment")
        {
            deadNode = true;
        }
    }    

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Environment")
        {
            deadNode = true;
        }
    }

    public List<Connection> GetConnections()
    {
        List<Connection> nodeConnections = new List<Connection>();

        if (top.toNode != null && !top.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(top);
        }
        if (bottom.toNode != null && !bottom.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(bottom);
        }
        if (left.toNode != null && !left.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(left);
        }
        if (right.toNode != null && !right.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(right);
        }
        if (topRight.toNode != null && !topRight.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(topRight);
        }
        if (topLeft.toNode != null && !topLeft.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(topLeft);
        }
        if (bottomRight.toNode != null && !bottomRight.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(bottomRight);
        }
        if (bottomLeft.toNode != null && !bottomLeft.toNode.GetComponent<NodeScript>().deadNode)
        {
            nodeConnections.Add(bottomLeft);
        }

        return nodeConnections;
    }
}

//[System.Serializable]
public struct Connection
{
    public GameObject fromNode;
    public GameObject toNode;
    public float weight;

    public Connection(GameObject fN, GameObject tN, float num)
    {
        fromNode = fN;
        toNode = tN;
        weight = num;
    }
}
