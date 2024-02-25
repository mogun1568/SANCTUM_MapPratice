using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeInfo
{
    public int R { get; set; }
    public int C { get; set; }
    public int Direction { get; set; }

    public NodeInfo(int r = 0, int c = 0, int direction = 0)
    {
        R = r;
        C = c;
        Direction = direction;
    }

    public void SetValues(int newR, int newC, int newDirection = 0)
    {
        R = newR;
        C = newC;
        Direction = newDirection;
    }
}

public class Map : MonoBehaviour
{
    [SerializeField] GameObject roadPrefab;
    [SerializeField] GameObject groundPrefab;
    [SerializeField] GameObject EnvironmentPrefab;
    [SerializeField] GameObject parentPrefab;
    GameObject parent;
    [SerializeField] GameObject EnvironmentParentPrefab;
    GameObject EnvironmentParent;

    [SerializeField] GameObject startPrefab;
    [SerializeField] GameObject endPrefab;
    GameObject startObj;
    GameObject endObj;

    [SerializeField] GameObject enemyPrefab;

    const int mapDefaultLength = 101;

    int[,] map = new int[mapDefaultLength, mapDefaultLength];
    bool[,] visit = new bool[mapDefaultLength, mapDefaultLength];
    public LinkedList<NodeInfo> roads = new LinkedList<NodeInfo>();

    // 우, 하, 좌, 상
    int[] dr = new int[4] { 0, 1, 0, -1 };
    int[] dc = new int[4] { 1, 0, -1, 0 };

    NodeInfo startPoint = new NodeInfo();
    NodeInfo endPoint = new NodeInfo();

    int mapLength = 3;
    int NodeSize = 1;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mapLength > mapDefaultLength / 2 - 1)
            {
                return;
            }

            ExpendMap();

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Instantiate(enemyPrefab);
        }
    }

    void Print2DArray()
    {
        string s = "(" + startPoint.R + ", " + startPoint.C + "), (" + endPoint.R + ", " + endPoint.C + ")";
        Debug.Log(s);

        string patternString = "";

        for (int i = 0; i < mapDefaultLength; i++)
        {
            for (int j = 0; j < mapDefaultLength; j++)
            {
                patternString += map[i, j].ToString();
            }
            patternString += "\n"; // 한 행 출력 후 줄 바꿈
        }

        Debug.Log(patternString);

        string boolString = "";

        for (int i = 0; i < mapDefaultLength; i++)
        {
            for (int j = 0; j < mapDefaultLength; j++)
            {
                if (visit[i, j])
                {
                    boolString += 1;
                } else
                {
                    boolString += 0;
                }
            }
            boolString += "\n"; // 한 행 출력 후 줄 바꿈
        }

        Debug.Log(boolString);
    }

    void Init()
    {
        parent = Instantiate(parentPrefab);
        EnvironmentParent = Instantiate(EnvironmentParentPrefab);
        startObj = Instantiate(startPrefab);
        endObj = Instantiate(endPrefab);

        CreateDefaultMap();
        //CreateDefaultMap1();
        //CreateDefaultMap2();

        //for (int i = mapLength - 2; i < mapDefaultLength / 4; i++)
        //{
        //    ExpendMap();
        //}
    }

    // 1x1 맵 (이걸로 할 듯)
    void CreateDefaultMap()
    {
        int mid = mapDefaultLength / 2;

        map[mid, mid] = 1;
        CreateNode("RoadE", mid * NodeSize, mid * NodeSize);
        visit[mid, mid] = true;
        roads.AddLast(new NodeInfo(mid, mid));

        startPoint.SetValues(mid, mid);
        endPoint.SetValues(mid, mid);

        mapLength = 3;
    }

    /*
    // 3x3 맵
    void CreateDefaultMap1()
    {
        int mid = mapDefaultLength / 2;

        startPoint[0] = mid - 1;
        startPoint[1] = mid;
        endPoint[0] = mid + 1;
        endPoint[1] = mid;

        for (int i = mid - 1; i < mid + 2; i++)
        {
            for (int j = mid - 1; j < mid + 2; j++)
            {
                if (j == mid)
                {
                    map[i, j] = 1;
                    Instantiate(roadPrefab, new Vector3(i, 0, j), Quaternion.identity, parent.transform);
                } else
                {
                    map[i, j] = 2;
                    Instantiate(groundPrefab, new Vector3(i, 0, j), Quaternion.identity, parent.transform);
                }
                visit[i, j] = true;
            }
        }

        mapLength = 5;
    }

    // 2x2 맵 (맵 확장에서 문제가 있는 듯 안됨)
    void CreateDefaultMap2()
    {
        int mid = mapDefaultLength / 2;
        int[] nodeR = new int[4] { mid - 1, mid, mid - 1, mid };
        int[] nodeC = new int[4] { mid - 1, mid - 1, mid, mid };

        int startNode = Random.Range(0, 4);
        int endNode = Random.Range(0, 4);
        while (Mathf.Abs(nodeR[startNode] - nodeR[endNode]) + Mathf.Abs(nodeC[startNode] - nodeC[endNode]) != 1)
        {
            endNode = Random.Range(0, 4);
        }

        startPoint[0] = nodeR[startNode];
        startPoint[1] = nodeC[startNode];
        endPoint[0] = nodeR[endNode];
        endPoint[1] = nodeC[endNode];

        for (int i = 0; i < 4; i++)
        {
            if (i == startNode || i == endNode)
            {
                map[nodeR[i], nodeC[i]] = 1;
                Instantiate(roadPrefab, new Vector3(nodeR[i], 0, nodeC[i]), Quaternion.identity, parent.transform);
            } else
            {
                map[nodeR[i], nodeC[i]] = 2;
                Instantiate(groundPrefab, new Vector3(nodeR[i], 0, nodeC[i]), Quaternion.identity, parent.transform);
            }

            visit[nodeR[i], nodeC[i]] = true;
        }
    }
    */

    void ExpendMap()
    {
        CreateStartPath(startPoint);
        CreateEndPath(endPoint);
        // 이거 렉 오짐
        //Print2DArray();

        //foreach (var node in roads)
        //{
        //    Debug.Log($"Node Values: R={node.R}, C={node.C}");
        //}
    }


    // 길이 만들어지는 경우가 너무 적으면
    // 길을 만들 개수를 랜덤으로 정하고 출발지와 도착지의 거리를 구해서 해야 할듯


    // 맵의 마지막 길 노드 다음 방향 구하기 
    int FirstPathDirection(NodeInfo node)
    {
        bool[] dir = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            if (!visit[node.R + dr[i], node.C + dc[i]])
            {
                if (map[node.R + dr[i], node.C + dc[i]] == 1)
                {
                    continue;
                }

                int nextDirection = (i + 1) % 4;
                if (map[node.R + dr[i] + dr[nextDirection], node.C + dc[i] + dc[nextDirection]] == 1)
                {
                    continue;
                }
                nextDirection = (i + 3) % 4;
                if (map[node.R + dr[i] + dr[nextDirection], node.C + dc[i] + dc[nextDirection]] == 1)
                {
                    continue;
                }

                dir[i] = true;
            }
        }

        int result = Random.Range(0, 4);
        while (!dir[result])
        {
            result = Random.Range(0, 4);
        }

        return result;
    }
            
    // 첫 노드 다음 노드들 방향 정하기
    int NextPathDirection(NodeInfo node, int firstNodeDirection)
    {
        bool[] dir = new bool[4];

        int newPathDirection = (firstNodeDirection + 1) % 4;
        if (map[node.R + dr[newPathDirection], node.C + dc[newPathDirection]] != 1)
        {
            dir[newPathDirection] = true;
        }
        newPathDirection = (firstNodeDirection + 3) % 4;
        if (map[node.R + dr[newPathDirection], node.C + dc[newPathDirection]] != 1)
        {
            dir[newPathDirection] = true;
        }

        int result = Random.Range(0, 4);
        while (!dir[result])
        {
            result = Random.Range(0, 4);
        }

        return result;
    }

    void UpdatePosition()
    {
        startObj.transform.position = new Vector3(startPoint.R * NodeSize, 1, startPoint.C * NodeSize);
        startObj.transform.rotation = Quaternion.Euler(0, startPoint.Direction * 90, 0);
        endObj.transform.position = new Vector3(endPoint.R * NodeSize, 1, endPoint.C * NodeSize);
        endObj.transform.rotation = Quaternion.Euler(0, endPoint.Direction * 90, 0);
    }

    void CreateNode(string type, int r, int c)
    {
        if (type == "RoadS")
        {
            Instantiate(roadPrefab, new Vector3(r, 0, c), Quaternion.identity, parent.transform);
            roads.AddFirst(new NodeInfo(r, c));
        } else if (type == "RoadE") 
        {
            Instantiate(roadPrefab, new Vector3(r, 0, c), Quaternion.identity, parent.transform);
            roads.AddLast(new NodeInfo(r, c));
        } else
        {
            float y = Random.Range(-0.25f, 0.25f);
            Instantiate(groundPrefab, new Vector3(r, y, c), Quaternion.identity, parent.transform);
            if (Random.Range(0, 10) == 1)
            {
                CreateEnvironment(r, y, c);
            }
        }
    }

    void CreateEnvironment(int r, float y, int c)
    {
        Instantiate(EnvironmentPrefab, new Vector3(r, y + NodeSize - 0.5f, c), Quaternion.identity, EnvironmentParent.transform);

        // 해당 노드에 환경 요소가 있다고 알려줄 변수 필요
    }


    void CreateStartPath(NodeInfo node)
    {
        // 첫 노드 방향, 현재 길 방향, 다음 길 방향, 회전 방향
        int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1;
        int nextR, nextC;
        bool isCorner;  // 코너 확인

        firstNodeDirection = FirstPathDirection(node);
        nextR = node.R + dr[firstNodeDirection];
        nextC = node.C + dc[firstNodeDirection];

        // map[nextR, nextC]에 길 노드 생성
        map[nextR, nextC] = 1;
        CreateNode("RoadS", nextR * NodeSize, nextC * NodeSize);
        startPoint.SetValues(nextR, nextC, (firstNodeDirection + 2) % 4);

        // 길을 더 만들지 말지 결정
        if (Random.Range(0, 2) == 0)
        {
            return;
        }

        currentPathDirection = NextPathDirection(node, firstNodeDirection);
        // 반시계 방향인 경우
        if (firstNodeDirection - currentPathDirection == -3 || firstNodeDirection - currentPathDirection == 1)
        {
            clockwise = 3;
        }
        // 다음 방향
        nextPathDirection = (currentPathDirection + clockwise) % 4;

        int[] currentCoor = new int[2] { nextR + dr[currentPathDirection], nextC + dc[currentPathDirection] };
        int MaxNodeCount = mapLength * 4 - 4 - 1;

        while (MaxNodeCount > 0)
        {
            int currentR = currentCoor[0];
            int currentC = currentCoor[1];
            isCorner = false;

            // 코너인 경우
            if (!visit[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]])
            {
                currentPathDirection = nextPathDirection;
                nextPathDirection = (currentPathDirection + clockwise) % 4;

                isCorner = true;
            }

            if (!isCorner)
            {
                // 옆에 있는 경우
                if (map[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]] == 1)
                {
                    return;
                }
                // 대각선에 있는 경우
                else if (map[currentR + dr[currentPathDirection] + dr[nextPathDirection], currentC + dc[currentPathDirection] + dc[nextPathDirection]] == 1)
                {
                    return;
                }
            }

            // map[currentR, currentC]에 길 노드 생성
            map[currentR, currentC] = 1;
            CreateNode("RoadS", currentR * NodeSize, currentC * NodeSize);
            if (!isCorner)
            {
                startPoint.SetValues(currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
            }
            else
            {
                startPoint.SetValues(currentR, currentC, (currentPathDirection + clockwise) % 4);
            }

            if (Random.Range(0, 2) == 0)
            {
                return;
            }

            currentCoor[0] = currentR + dr[currentPathDirection];
            currentCoor[1] = currentC + dc[currentPathDirection];

            MaxNodeCount--;
        }
    }

    void CreateEndPath(NodeInfo node)
    {
        // 첫 노드 방향, 현재 길 방향, 다음 길 방향, 회전 방향, 길인지 땅인지 확인 변수
        int firstNodeDirection, currentPathDirection, nextPathDirection, clockwise = 1, createNode = 1;
        int nextR, nextC;
        bool isCorner; // 코너 확인

        firstNodeDirection = FirstPathDirection(node);
        nextR = node.R + dr[firstNodeDirection];
        nextC = node.C + dc[firstNodeDirection];

        // map[currentR, currentC]에 길 노드 생성
        map[nextR, nextC] = createNode;
        CreateNode("RoadE", nextR * NodeSize, nextC * NodeSize);
        visit[nextR, nextC] = true;
        endPoint.SetValues(nextR, nextC, (firstNodeDirection + 2) % 4);

        createNode = Random.Range(1, 3);

        currentPathDirection = NextPathDirection(node, firstNodeDirection);
        // 반시계 방향인 경우
        if (firstNodeDirection - currentPathDirection == -3 || firstNodeDirection - currentPathDirection == 1)
        {
            clockwise = 3;
        }
        // 다음 방향
        nextPathDirection = (currentPathDirection + clockwise) % 4;

        int[] currentCoor = new int[2] { nextR + dr[currentPathDirection], nextC + dc[currentPathDirection] };
        int MaxNodeCount = mapLength * 4 - 4 - 1;

        while (MaxNodeCount > 0)
        {
            int currentR = currentCoor[0];
            int currentC = currentCoor[1];
            isCorner = false;

            // 코너인 경우
            if (!visit[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]])
            {
                currentPathDirection = nextPathDirection;
                nextPathDirection = (currentPathDirection + clockwise) % 4;

                isCorner = true;
            }

            if (createNode == 1 && !isCorner)
            {
                // 앞에 있는 경우
                if (map[currentR + dr[currentPathDirection], currentC + dc[currentPathDirection]] == 1)
                {
                    createNode = 2;
                }
                // 옆에 있는 경우
                else if (map[currentR + dr[nextPathDirection], currentC + dc[nextPathDirection]] == 1)
                {
                    createNode = 2;
                }
                // 대각선에 있는 경우
                else if (map[currentR + dr[currentPathDirection] + dr[nextPathDirection], currentC + dc[currentPathDirection] + dc[nextPathDirection]] == 1)
                {
                    createNode = 2;
                }
            }

            // map[currentR, currentC]에 길 노드 생성
            if (map[currentR, currentC] == 0)
            {
                map[currentR, currentC] = createNode;
                if (createNode == 1)
                {
                    CreateNode("RoadE", currentR * NodeSize, currentC * NodeSize);
                    if (!isCorner)
                    {
                        endPoint.SetValues(currentR, currentC, (currentPathDirection + 2 * clockwise) % 4);
                    } else
                    {
                        endPoint.SetValues(currentR, currentC, (currentPathDirection + clockwise) % 4);
                    }
                }
                else
                {
                    CreateNode("Ground", currentR * NodeSize, currentC * NodeSize);
                }
            }
            visit[currentR, currentC] = true;

            if (createNode == 1)
            {
                createNode = Random.Range(1, 3);
            }

            currentCoor[0] = currentR + dr[currentPathDirection];
            currentCoor[1] = currentC + dc[currentPathDirection];

            MaxNodeCount--;
        }

        UpdatePosition();

        mapLength += 2;
    }
}
