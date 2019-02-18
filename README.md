# Production2Ferals
The most important things I learned in this project was implementing A* 
pathfinding into the AI and by creating a universal 2D Raycast Lighting...
Both of these scripts can be applied to any project in the future

Created by Cameron Belcher (DrabbyPage) (Programmer), Brian Harney (Producer),
            Kaylee Sharp (Artist), Rose Gruner (Designer), Jae Ettinger (Designer)
            at Champlain College 2019

The two scripts that I want to show off are the Pathfinding script and the 2D Raycasting Script

--------------------------------------------------------------------------------------------------------------------------------------

*** Pathfinding Script ***
I use A* pathfinding, which can easily be converted to Dijkstra if you go 
to line 88 and uncomment then comment out line 89.

The heuristic i use for A* is the distance of the node to the goal node
and I used a struct called StarNodeRecord to hold data for each node.
Along with this I made a struct in NodeScript called Connection that kept
track of all the data of conneciton between nodes


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

    GameObject gridHolder;

    struct StarNodeRecord
    {
        public GameObject node;
        public Connection recordConnection;
        public float costSoFar;
        public float estiTotalCost;
    };
    
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

    public List<GameObject> objectPath;

    //[SerializeField]
    //Vector3 goalPos = new Vector3(0, 0); // uncomment to see where the ai is going (line 77 as well)

    // Use this for initialization
    void Start()
    {
        gridHolder = GameObject.Find("Grid");
        objectPath = new List<GameObject>();
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
    
    List<GameObject> GetAStarPath(Vector3 start, Vector3 end)
    {
        // initialize the record for the start node

        // initialize the open and closed list
        List<StarNodeRecord> openList = new List<StarNodeRecord>();
        List<StarNodeRecord> closedList = new List<StarNodeRecord>();

        // add it to the open list
        openList.Add(startRecord);

        // make the current node record the beginning of the openlist
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
                // get the connections of the current node
                List<Connection> connections = new List<Connection>();
                connections = current.node.GetComponent<NodeScript>().GetConnections();

                // loop through each connection in turn
                for (int i = 0; i < connections.Count; i++)
                {
                    // get the cost estimate for the end node
                    GameObject endToNode = connections[i].toNode;
                    float endNodeCost = current.costSoFar + connections[i].weight;// connections[i]->getCost();

                    StarNodeRecord endNodeRecord = new StarNodeRecord();

                    float endNodeHeuristic;

                    bool inClosedList = false;
                    bool inOpenList = false;

                    // if the node is closed we may have to skip or remove it from the close list
                    for(int j = 0; j < closedList.Count; j++)
                    {
                        // here we find the record in the closed list corresponding to the end node
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
                        for (int m = 0; m < openList.Count; m++)
                        {
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
                            if(openList[k].node == endNodeRecord.node)
                            {
                                inOpen = true;
                                break;
                            }
                        }

                        if (!inOpen)
                        {
                            openList.Add(endNodeRecord);
                        }
                    }
                }
                // we've finished looking at the connections for the current node so add it to the closed 
                // list and remove it from the open list
                openList.Remove(openList[0]);

                closedList.Add(current);
            }
        }

        // we're here if we found a goal or if we have no more nodes to search find which one
        if (current.node != startEndNode)
        {
            // we've run out of nodes without finding the goal, so theres no solution
            // return none
            Debug.Log("did not end on goal node");
            return null;
        }
        // else:
        else
        {
            // compile the list of connections in the path
            List<GameObject> a_Star_Path = new List<GameObject>();

            // work back along the path, accumulating connections
            while (current.node != startNode)
            {
                // FYI Update the current's connection as well

                a_Star_Path.Add(current.node);

                current.node = current.recordConnection.fromNode;

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
            List<GameObject> reversePath = new List<GameObject>();

            for(int i = 0; i < a_Star_Path.Count; i++)
            {
                GameObject newNode;
                int lastNodeIndex;

                lastNodeIndex = a_Star_Path.Count - (i+1);

                newNode = a_Star_Path[lastNodeIndex];

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
----------------------------------------------------------------------------------------------------------------------------------
    
*** 2D Raycast Lighting Script***
Uses Unity Sprite Masks and Raycasting to produce an area for other
either be seen or not seen
this is to make multiple raycasting easy without needing to use
a 2d array for raycasts
    
    struct RaycastHolder
    {
        public RaycastHit[] rayHit;
        public Vector3 rayDirection;
        public float rayDistance;
    }

    // one singular way to test raycasting
    RaycastHit[] testRay;

    // these are objects that hold the sprite masks not actual sprite masks
    GameObject[] masksArray;

    [SerializeField]
    Sprite spriteMaskSprite;

    [SerializeField]
    float angleOfLight;

    [SerializeField]
    float amountOfRaycasts = 16;

    [SerializeField]
    public float maxLightDist = 10;

    [SerializeField]
    float angleOffset = 45;

    [SerializeField]
    float maskYValue = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        masksArray = new GameObject[(int)(amountOfRaycasts)];
        //Debug.Log(spriteMaskSprite);
    }

    // Update is called once per frame
    void Update()
    {
        //ShowMultipleRaycasts();
        MultRaysWithStruct();
    }

    void MultRaysWithStruct()
    {
        RaycastHolder[] raycastHolderArray = new RaycastHolder[(int)(amountOfRaycasts)];

        for(int i = 0; i < amountOfRaycasts; i++)
        {
            float angle = (angleOfLight * (i / amountOfRaycasts));

            RaycastHolder holder = raycastHolderArray[i];

            holder.rayDirection = GetRayAngledDirection(angle);
            holder.rayHit = Physics.RaycastAll(gameObject.transform.position, holder.rayDirection);
            holder.rayDistance = maxLightDist;
            float minDist = maxLightDist;
            for(int m = 0; m < holder.rayHit.Length; m++)
            {
                GameObject objHit = holder.rayHit[m].collider.gameObject;
                if (objHit.tag == "Environment")
                {
                    if (holder.rayHit[m].distance < minDist)
                    {
                        minDist = holder.rayHit[m].distance;
                        holder.rayDistance = holder.rayHit[m].distance;
                    }
                }
                else if(objHit.tag == "Enemy")
                {
                    if (holder.rayHit[m].distance < minDist)
                    {
                        // check to see if there is an object between the light and the enemy
                        Vector3 enemyDir = holder.rayHit[m].collider.gameObject.transform.position - gameObject.transform.position;
                        RaycastHit[] fleeCheck = Physics.RaycastAll(gameObject.transform.position, enemyDir, maxLightDist);

                        bool somethingInWay = false;
                        for(int n = 0; n < fleeCheck.Length; n++)
                        {
                            if(fleeCheck[n].distance < enemyDir.magnitude && 
                                fleeCheck[n].collider.gameObject.tag == "Environment")
                            {
                                somethingInWay = true;
                                break;
                            }
                        }

                        if(!somethingInWay)
                        {
                            // objHit.GetComponent<AIFleeScript>().SetFleeState(true);
                        }
                    }
                }
            }

            holder.rayDirection = holder.rayDirection.normalized * holder.rayDistance;

            Debug.DrawRay(gameObject.transform.position, holder.rayDirection, Color.green);

            CreateLight(holder.rayDirection, holder.rayDistance, angle, i);
        }
    }

    // creates a sprite mask and sets the size according to the size of the ray
    void CreateLight(Vector3 rayDirection, float distance, float angle, int maskID)
    {
        GameObject currentMask = masksArray[maskID];

        if(currentMask == null)
        {
            currentMask = Instantiate(Resources.Load("Prefabs/SpriteMaskObj")) as GameObject;
        }

        currentMask.name = "SpriteMask" + maskID;
        currentMask.transform.position = gameObject.transform.position + ((rayDirection)/2);

        
        float objEulerX = gameObject.transform.eulerAngles.x;
        float maskAnglePt2 = Mathf.Atan2(rayDirection.z, rayDirection.x) * Mathf.Rad2Deg;
        currentMask.transform.rotation = Quaternion.Euler(objEulerX, 0, -maskAnglePt2);

        currentMask.GetComponent<SpriteMask>().sprite = spriteMaskSprite;
        currentMask.GetComponent<Transform>().localScale = new Vector3(distance, maskYValue, 1);

        masksArray[maskID] = currentMask;
    }

    // this duplicates the Raycasts (next function)
    void ShowMultipleRaycasts()
    {
        for(int i = 0; i < amountOfRaycasts; i++)
        {
            float angle = angleOfLight * (i / amountOfRaycasts);

            ShowRaycast(angle);
        }

    }

    // this is used for scene to show how the raycasts will work
    void ShowRaycast(float angle)
    {
        // shows only one ray
        Vector3 rayDirection = new Vector3(0,0,0);
        
        rayDirection = GetRayAngledDirection(angle);
        testRay = Physics.RaycastAll(gameObject.transform.position, rayDirection);
        float dist = maxLightDist;
        for(int i = 0; i < testRay.Length; i++)
        {
            if (testRay[i].collider.gameObject.tag == "Environment")
            {
                if (testRay[i].distance < maxLightDist)
                {
                    //Debug.Log("collision with a non node");
                    dist = testRay[i].distance;
                }
            }
        }

        rayDirection = rayDirection.normalized * dist;
        Debug.DrawRay(gameObject.transform.position, rayDirection, Color.green);

        //CreateLight(rayDirection, dist);
    }

    // this is for the direction compared to the mouse
    Vector3 GetRayDirection()
    {
        Vector3 direction = new Vector3(0,0,0);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 objPos = gameObject.transform.position;
        
        direction = mousePos - objPos;
        direction.y = 0;
        return direction.normalized;
    }

    // pass in a divisor of 360 and get the direction
    Vector3 GetRayAngledDirection(float angle)
    {
        Vector3 direction = Vector3.right;

        float angleToRad = (gameObject.transform.eulerAngles.y + (angle + angleOffset)) * Mathf.Deg2Rad;

        Vector3 objPos = gameObject.transform.position;

        direction = new Vector3(Mathf.Cos(angleToRad), 0, Mathf.Sin(angleToRad));//angleToRad;

        direction.y = 0;

        return direction.normalized;
    }

    // this allows for aspects to be done in scene time
    void OnDrawGizmosSelected()
    {
        // Draws a 5 unit long red line in front of the object
        ShowMultipleRaycasts(); // works with scene
        //MultRaysWithStruct(); // doesnt work with scene
    }
    
