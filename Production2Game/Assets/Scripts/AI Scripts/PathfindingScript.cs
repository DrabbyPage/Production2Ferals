using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingScript : MonoBehaviour
{
    GameObject gridHolder;

    #region NodeRecord
    struct NodeRecord
    {
        public GameObject node;

        public Connection recordConnection;

        public float costSoFar;
        public float estiTotalCost;
    }
    #endregion

    #region StarNodeRecord
    struct StarNodeRecord
    {
        public GameObject node;
        public Connection recordConnection;
        public float costSoFar;
        public float estiTotalCost;
    };
    #endregion

    #region Heuristic
    class Heuristic
    {
        public Heuristic(GameObject endNode)
        {
            heurEndNode = endNode;
        }

        public float EstimateCost(GameObject node)
        {
            Vector3 nodePos = node.transform.position;
            Vector3 endNodePos = heurEndNode.transform.position;

            Vector3 diff = endNodePos - nodePos;
            float distance = diff.magnitude;

            return distance;
        }

        private GameObject heurEndNode;
    }
    #endregion

    [SerializeField]
    public List<GameObject> objectPath;

    //[SerializeField]
    //Vector3 goalPos = new Vector3(0, 0); // uncomment to see where the ai is going (line 77 as well)

    // Use this for initialization
    void Start()
    {
        gridHolder = GameObject.Find("Grid");
        objectPath = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GeneratePath(Vector3 start, Vector3 end)
    {
        GameObject startNode = FindClosestNode(start);
        GameObject endNode = FindClosestNode(end);
        //goalPos = end;

        while (objectPath.Count > 0)
        {
            objectPath.RemoveAt(0);
        }

        if (!ClearedRaycast(startNode, endNode))
        {
            Debug.Log("did not clear the raycast");

            //objectPath = GetDijkstraPath(start, end);
            objectPath = GetAStarPath(start, end);
        }
        else
        {
            //Debug.Log("passed the raycast test");
            objectPath.Add(startNode);
            objectPath.Add(endNode);
        }
    }

    public Vector3 GetNextPos()
    {
        Vector3 outputPos;

        outputPos = gameObject.transform.position;

        if (objectPath.Count > 0)
        {
            outputPos = objectPath[0].transform.position;
        }

        return outputPos;
    }

    public void KnockOutPathNode()
    {
        int frontValue = 0;
        objectPath.RemoveAt(frontValue);
    }

    public void ResetPath()
    {
        objectPath.Clear();
    }

    List<GameObject> GetDijkstraPath(Vector3 start, Vector3 end)
    {
        GameObject startNode = FindClosestNode(start);
        GameObject startEndNode = FindClosestNode(end);

        //Debug.Log(startNode.name);
        //Debug.Log(startEndNode.name);

        // initialize the record for the start nodes
        NodeRecord startRecord = new NodeRecord();
        startRecord.node = startNode;
        startRecord.costSoFar = 0;

        //initialize the open and closed list
        List<NodeRecord> openList = new List<NodeRecord>();
        List<NodeRecord> closedList = new List<NodeRecord>();

        openList.Add(startRecord);

        NodeRecord currentNode = new NodeRecord();

        List<Connection> connections = new List<Connection>();

        // iterate through each process
        while (openList.Count > 0)
        {
            // find the smallest element in the open list
            currentNode = openList[0];

            // if we are at the goal node then terminate
            if (currentNode.node == startEndNode)
            {
                ///Debug.Log("at end of dijkstra");
                break;
            }
            // otherwise get the outgoing connections
            else
            {
                NodeRecord endNodeRecord = new NodeRecord();

                connections = openList[0].node.GetComponent<NodeScript>().GetConnections();

                // loop through each connection in turn
                for (int i = 0; i < connections.Count; i++)
                {
                    // get the cost estimate for teh end node
                    GameObject newEndNode = connections[i].toNode;
                    Connection newEndConnect = connections[i];
                    float endNodeCost = currentNode.costSoFar + connections[i].weight;

                    bool inClosedList = false;
                    bool inOpenList = false;

                    endNodeRecord.node = connections[i].toNode;


                    // skip if the node is closed
                    for (int j = 0; j < closedList.Count; j++)
                    {
                        if (closedList[j].node == newEndNode)
                        {
                            inClosedList = true;
                            break;
                        }
                    }
                    //else if open has the end node
                    if (!inClosedList)
                    {
                        for (int j = 0; j < openList.Count; j++)
                        {
                            if (openList[j].node == newEndNode)
                            {
                                // here we find the record in the oopen list corresponding to the endNode
                                endNodeRecord.node = openList[j].node;
                                inOpenList = true;


                                // if(endNodeRecord.cost <= endNodeCost)
                                if (endNodeRecord.costSoFar <= endNodeCost)
                                {
                                    // continue
                                    continue;
                                }
                                break;
                            }
                        }
                    }
                    if (!inOpenList && !inClosedList)
                    {
                        endNodeRecord.node = newEndNode;

                        // we are here of we need to update the node
                        // update the cost and connection
                        endNodeRecord.recordConnection = newEndConnect;
                    }

                    // add it into the open list
                    if (!inClosedList)
                    {
                        bool inOpen = false;
                        for (int j = 0; j < openList.Count; j++)
                        {
                            if (openList[j].node == endNodeRecord.node)
                            {
                                inOpen = true;
                            }
                        }
                        if (!inOpen && !endNodeRecord.node.GetComponent<NodeScript>().deadNode)
                        {
                            openList.Add(endNodeRecord);
                        }
                    }
                }
            }

            // we have finished looking at the connections for the current node
            // so add it to the closed list and remove it from the open list
            openList.RemoveAt(0);
            closedList.Add(currentNode);
        }

        // we are here if we have either found the goal or if we 
        // have no more nodes to search, find which:
        if (currentNode.node != startEndNode)
        {
            //we have run out of nodes without findign the goal so 
            // there is no solution
            Debug.Log("there is no solution");
            return null;
        }
        else
        {
            List<GameObject> dijkstraPath = new List<GameObject>();

            //work back along the path, accumulating connections
            while (currentNode.node != startNode)
            {
                dijkstraPath.Add(currentNode.node);
                currentNode.node = currentNode.recordConnection.fromNode;

                for (int i = 0; i < closedList.Count; i++)
                {
                    if (closedList[i].node == currentNode.node)
                    {
                        currentNode.recordConnection = closedList[i].recordConnection;
                    }
                }
            }

            // the path is backwards so we must reverse the path
            List<GameObject> reversePath = new List<GameObject>();

            for (int i = 0; i < dijkstraPath.Count; i++)
            {
                GameObject newNode;
                int lastNodeIndex;

                lastNodeIndex = dijkstraPath.Count - (i + 1);

                newNode = dijkstraPath[lastNodeIndex];

                reversePath.Add(newNode);
            }

            //Debug.Log("finished making the path");

            List<GameObject> outputPath = new List<GameObject>();

            //outputPath = reversePath;

            outputPath = SmoothPath(reversePath); // uncomment to smooth the path after making it

            return outputPath;
        }
    }

    List<GameObject> GetAStarPath(Vector3 start, Vector3 end)
    {
        //Debug.Log("starting aStar");
        GameObject startNode = FindClosestNode(start);
        GameObject startEndNode = FindClosestNode(end);

        Heuristic heur = new Heuristic(startEndNode);

        // initialize the record for the start node
        StarNodeRecord startRecord = new StarNodeRecord();
        startRecord.node = startNode;
        //startRecord.recordConnection = null;
        startRecord.costSoFar = 0;
        startRecord.estiTotalCost = heur.EstimateCost(startNode);

        // initialize the open and closed list
        List<StarNodeRecord> openList = new List<StarNodeRecord>();
        List<StarNodeRecord> closedList = new List<StarNodeRecord>();

        openList.Add(startRecord);

        StarNodeRecord current = openList[0];

        //iterate through processing each node
        while (openList.Count > 0)
        {
            //find the smallest element in the open list
            current = openList[0];

            // if it is the goal node then terminate
            if (current.node == startEndNode)
            {
                //Debug.Log("at the end of A Star");
                break;
            }
            // otherwise get its outgoing connections
            else
            {
                // connections= graph.getConnections(current)
                List<Connection> connections = new List<Connection>();
                connections = current.node.GetComponent<NodeScript>().GetConnections();// mpGraph->getConnections(current.node->getId());

                // loop through each connection in turn
                for (int i = 0; i < connections.Count; i++)
                {
                    // get the cost estimate for the end node
                    GameObject endToNode = connections[i].toNode;//connections[i]->getToNode();
                    float endNodeCost = current.costSoFar + connections[i].weight;// connections[i]->getCost();

                    StarNodeRecord endNodeRecord = new StarNodeRecord();

                    float endNodeHeuristic;

                    bool inClosedList = false;
                    bool inOpenList = false;

                    // if the node is closed we may have to skip or remove it from the close list
                    //for (auto nodeRecord = closedList.begin(); nodeRecord != closedList.end(); nodeRecord++)
                    for(int j = 0; j < closedList.Count; j++)
                    {
                        // here we find the record in the closed list corresponding to the end node
                        //if (nodeRecord.node == endNode)
                        if (closedList[j].node == endToNode)
                        {
                            inClosedList = true;
                            endNodeRecord.node = closedList[j].node;// nodeRecord->node;

                            // if we didnt find a shorter route, skip
                            if (endNodeRecord.costSoFar <= endNodeCost)
                            {
                                // continue;
                                continue;
                            }
                            // otherwise remove it from the closed list;
                            else
                            {
                                //closedList.erase(nodeRecord);
                                closedList.RemoveAt(j);

                                // we can use the node's old cost values to calculate its heuristic without
                                // calling the possibly expensive heuristic function
                                endNodeHeuristic = endNodeRecord.estiTotalCost - endNodeRecord.costSoFar;
                            }
                        }

                    }
                    if (!inClosedList)
                    {
                        // skip if the node is open and we've not found a better route
                        //for (auto record = openList.begin(); record != openList.end(); record++)
                        for (int m = 0; m < openList.Count; m++)
                        {
                            //if (record->node == endNode)
                            if(openList[m].node == endToNode)
                            {
                                inOpenList = true;

                                // here we find the record in the open list corresponding to the endNode
                                endNodeRecord.node = openList[m].node;// record->node;

                                // if our route is no better, then skip
                                if (endNodeRecord.costSoFar <= endNodeCost)
                                {
                                    // continue
                                    continue;
                                }
                                else
                                {

                                    // we can use the node's old cost values to calculate its heuristic without
                                    // calling the possibly expensive heuristic function
                                    // endNodeHeuristic = endNodeRecord.recordConnection->getCost() - endNodeRecord.costSoFar;
                                    endNodeHeuristic = endNodeRecord.recordConnection.weight - endNodeRecord.costSoFar;
                                }
                                break;
                            }

                        }
                    }
                    // otherwise we know we've got an unvisited node so make a record for it
                    if (!inClosedList && !inOpenList)
                    {
                        endNodeRecord = new StarNodeRecord();
                        endNodeRecord.node = endToNode;

                        // we'll need to calculate the heuristic value using the function, since
                        // we dont have an existing record to use
                        endNodeHeuristic = heur.EstimateCost(endToNode);

                        // we're here if we need to update the node
                        // update the cost, estimate, and connection
                        endNodeRecord.costSoFar = endNodeCost;
                        endNodeRecord.recordConnection = connections[i];
                        endNodeRecord.estiTotalCost = endNodeCost + endNodeHeuristic;

                    }

                    // and add it to the openList
                    if (!inClosedList)
                    {
                        bool inOpen = false;
                        //for (auto record = openList.begin(); record < openList.end(); record++)
                        for(int k = 0; k < openList.Count; k++)
                        {
                            //if (record->node == endNodeRecord.node)
                            if(openList[k].node == endNodeRecord.node)
                            {
                                inOpen = true;
                                break;
                            }
                        }

                        if (!inOpen)
                        {
                            //openList.push_back(endNodeRecord);
                            openList.Add(endNodeRecord);
                        }
                    }
                }
                // we've finished looking at the connections for the current node so add it to the closed 
                // list and remove it from the open list
                //openList.erase(openList.begin());
                openList.Remove(openList[0]);

                //closedList.push_back(current);
                closedList.Add(current);
            }
        }

        // we're here if we found a goal or if we have no more nodes to search find which one
        if (current.node != startEndNode)
        {
            // we've run out of nodes without finding the goal, so theres no solution
            // return none
            //std::cout << "did not end on goal node" << std::endl;
            Debug.Log("did not end on goal node");
            return null;
        }
        // else:
        else
        {
            // compile the list of connections in the path
            // Path* a_Star_Path = new Path();
            List<GameObject> a_Star_Path = new List<GameObject>();

            // work back along the path, accumulating connections
            //while (current.node != start)
            while (current.node != startNode)
            {
                // path+= current.connection
                // current = current.connection.getFromNode()
                // FYI Update the current's connection as well

                // a_Star_Path->addNode(current.node);
                a_Star_Path.Add(current.node);

                //current.node = current.recordConnection->getFromNode();
                current.node = current.recordConnection.fromNode;

                //for (auto node = closedList.begin(); node < closedList.end(); node++)
                for(int i = 0; i < closedList.Count; i++)
                {
                    //if (node->node == current.node)
                    if(closedList[i].node == current.node)
                    {
                        // current.recordConnection = node->recordConnection;
                        current.recordConnection = closedList[i].recordConnection;
                    }
                }
            }

            // reverse the path, and return it
            // Path* reversePath = new Path();
            List<GameObject> reversePath = new List<GameObject>();

            //for (int i = 0; i < a_Star_Path->getNumNodes(); i++)
            for(int i = 0; i < a_Star_Path.Count; i++)
            {
                //Node* newNode;
                GameObject newNode;
                int lastNodeIndex;

                lastNodeIndex = a_Star_Path.Count - (i+1);//a_Star_Path->getNumNodes() - (i + 1);

                newNode = a_Star_Path[lastNodeIndex];//a_Star_Path->peekNode(lastNodeIndex);

                //reversePath->addNode(newNode);
                reversePath.Add(newNode);
            }
            // return reverse path
            reversePath = SmoothPath(reversePath);
            return reversePath;
        }
    }

    GameObject FindClosestNode(Vector3 point)
    {
        GameObject foundPoint = null;

        float minDist = 1000;

        for (int i = 0; i < gridHolder.GetComponent<GraphScript>().nodes.Count; i++)
        {
            Vector3 diff = point - gridHolder.GetComponent<GraphScript>().nodes[i].transform.position;

            float dist = diff.magnitude;

            if (dist < minDist && !gridHolder.GetComponent<GraphScript>().nodes[i].GetComponent<NodeScript>().deadNode)
            {
                minDist = dist;
                foundPoint = gridHolder.GetComponent<GraphScript>().nodes[i];
            }
        }

        return foundPoint;

    }

    float EstimateCost(GameObject startNode, GameObject endNode)
    {
        //Debug.Log(startNode.name);
        Vector3 fromGridPos = startNode.transform.position;
        Vector3 toGridPos = endNode.transform.position;
        Vector3 dist = toGridPos - fromGridPos;

        return dist.magnitude;
    }

    List<GameObject> SmoothPath(List<GameObject> inputPath)
    {
        // if the path is only two nodes ling then we cant smooth it so return
        if (inputPath.Count <= 2)
        {
            //Debug.Log("Path is less than two units and cannot be smoothed");
            return inputPath;
        }
        else
        {
            //Debug.Log("Smoothing the path");
        }

        // compile an output path
        List<GameObject> outputPath = new List<GameObject>();
        outputPath.Add(inputPath[0]);

        // keep track of where we are in the input path we start at 2, cause
        // we assume two adjacent nodes will pass the ray cast
        int inputIndex = 2;
        int amountOfOutput = 1;

        // loop until we find the last item in the input
        while (inputIndex < inputPath.Count - 1)
        {
            amountOfOutput = outputPath.Count;

            // check if we can "see" the next node (there is no wall/ blocking value in the way)
            bool rayClear = ClearedRaycast(outputPath[amountOfOutput - 1], inputPath[inputIndex]);

            if (!rayClear)
            {
                //Debug.Log("adding to the smoothi list");
                outputPath.Add(inputPath[inputIndex - 1]);
            }

            // consider the next node
            inputIndex++;
        }

        // we reached the end of the input path, add the end node to the output and return it
        outputPath.Add(inputPath[inputPath.Count - 1]);

        // return the output path
        return outputPath;
    }

    bool ClearedRaycast(GameObject outputNode, GameObject inputNode)
    {
        Vector3 fromGridPos = outputNode.transform.position;
        Vector3 toGridPos = inputNode.transform.position;

        Vector3 diff;

        // getting the direction of the input node from the output
        diff = toGridPos - fromGridPos;

        // this is using unity raycasting
        // will check in between the two given nodes for the wall/ or blocking value
        RaycastHit[] pathBlocked = Physics.RaycastAll(fromGridPos, diff.normalized, diff.magnitude);

        // this is for a list of collisions
        for (int i = 0; i < pathBlocked.Length; i++)
        {
            //Debug.Log(pathBlocked[i].collider.gameObject.tag);

            if (pathBlocked[i].collider.gameObject.tag == "Environment")
            {
                //Debug.Log("collision with a non node");
                return false;
            }
        }

        // if all collisions were tagged with node then return true
        return true;
    }

}
