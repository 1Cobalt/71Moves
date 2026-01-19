using System.Collections.Generic;
using UnityEngine;
using MyDice.Helpers;
using MyDice.Players;

namespace MyDice.Board
{
    public class ElementNodeCreator : MonoBehaviour
    {
        #region variable
        #region public
        [HideInInspector] public List<Vector3> points = new List<Vector3>();
        [HideInInspector] public List<GameObject> nodes = new List<GameObject>();
        [HideInInspector] public List<PlayerHome> playerHomes = new List<PlayerHome>();
        [HideInInspector] public DiceManager diceManager;
        #region Shape Struct
        [HideInInspector] public CircleStruct circleStruct;
        [HideInInspector] public SquareStruct squareStruct;
        [HideInInspector] public EllipseStruct ellipseStruct;
        [HideInInspector] public DiamondStruct diamondStruct;
        [HideInInspector] public TriangleStruct triangleStruct;
        [HideInInspector] public PolygonStruct polygonStruct;
        [HideInInspector] public LineStruct lineStruct;
        #endregion
        [Header("Settings")]
        [HideInInspector]
        public KeyCode rollingKeyCode = KeyCode.Space;
        public GameObject defaultPrefab;
        [Header("Editor settings")]
        public float handlePointRadius = .1f;
        public float handleArrowSize = .2f;
        public float handlePlayerHomesRadius = .5f;
        public bool autoRotation = false;
        public bool showRedirectLines = true;
        #endregion
        #region private
        [HideInInspector] public int playerHomeIndex = -1;
        [HideInInspector] public List<GameObject> ghosts;
        [HideInInspector] public bool allowRolling = true;
        [HideInInspector] public ElementNodesManager elementNodesManager;
        #endregion
        #endregion
        #region Functions
        private void OnValidate()
        {
            elementNodesManager = new ElementNodesManager(ref nodes);
            elementNodesManager.chetToFixProblems();
        }
        private void Start()
        {
            diceManager = FindObjectOfType<DiceManager>();
            if (diceManager == null)
            {
                Debug.LogError("Dice manager not found.");
                Extensions.Quit();
                return;
            }
            initPlayers();
        }
        private void FixedUpdate()
        {
            if (diceManager.getDiceState() == DiceState.Ready)
            {
                rolling();
            }
            else if (diceManager.getDiceState() == DiceState.Finish)
            {
                switch (playerHomes[playerHomeIndex].playerMode)
                {
                    case PlayerMode.Human:
                        Player player = playerHomes[playerHomeIndex].getCandidatePlayer();
                        if (player == null)
                        {
                            checkForCandidate_Human(playerHomeIndex);
                            return;
                        }
                        updatePlayerGame_Human(player);
                        break;
                    case PlayerMode.CPU:
                        updatePlayerGame_CPU();
                        break;
                }
            }
        }
        #endregion
        #region functions
        #region updates
        protected void updatePlayerGame_Human(Player player)
        {
            if (player.playerState == PlayerState.Idle)
            {
                //get dice
                player.diceValues = diceManager.getDicesValues();
                //calculate indexes
                player.CalculatePositionIndex(ref nodes);
                //Select Path
                if (player.hasPath())
                {
                    if (player.pathManager.Paths.Count > 1)
                    {
                        showPaths(ref player);
                        player.playerState = PlayerState.SelectPath;
                    }
                    else
                    {
                        player.GoTo_CalculatedIndexes(player.pathManager.Paths[0], ref points);
                    }
                }
                else
                {
                    Debug.Log("No path is exist.");
                    playerAndBoardReset(player);
                }
            }
            else if (player.playerState == PlayerState.MovingComplete)
            {
                PlayerState_MovingComplete(player);
            }
            else if (player.playerState == PlayerState.SelectPath)
            {
                checkForHitGhost(player);
            }
        }
        public void updatePlayerGame_CPU()
        {
            if (playerHomes[playerHomeIndex].getCandidateIndex() < 0)
            {
                playerHomes[playerHomeIndex].chooseCandidateIndexByAI(diceManager.getDicesValues(), ref nodes);
            }
            Player player = playerHomes[playerHomeIndex].getCandidatePlayer();
            if (player == null)
            {
                Debug.Log("No player is exist.");
                playerAndBoardReset(player);
                return;
            }
            if (player.playerState == PlayerState.Idle)
            {
                Path path;
                if (playerHomes[playerHomeIndex].targetIndex > -1)
                {
                    path = player.pathManager.getBestBenefitPath();
                }
                else
                {
                    path = player.pathManager.getRandomPath();
                }
                if (path == null)
                {
                    Debug.Log("No path is exist.");
                    playerAndBoardReset(player);
                    return;
                }
                player.GoTo_CalculatedIndexes(path, ref points);
            }
            else if (player.playerState == PlayerState.MovingComplete)
            {
                PlayerState_MovingComplete(player);
            }
        }
        private void PlayerState_MovingComplete(Player player)
        {
            ElementNode node;
            if ((node = elementNodesManager.getNode(player.currentPositionIndex)) == null) return;

            if (node.redirectIndex != -1
             && node.redirectIndex != player.currentPositionIndex)
            {
                player.GoTo(new List<Vector3>() { points[player.currentPositionIndex], points[node.redirectIndex] });
                player.currentPositionIndex = node.redirectIndex;
                return;
            }
            if (node.elementNodeType == ElementNodeType.ResetPoint
                && playerHomes[playerHomeIndex].startIndex != player.currentPositionIndex)
            {
                player.GoTo(new List<Vector3>() {
                                    points[player.currentPositionIndex],
                                    points[playerHomes[playerHomeIndex].startIndex] });
                player.currentPositionIndex = playerHomes[playerHomeIndex].startIndex;
                return;
            }

            playerAndBoardReset(player);
        }

        private void playerAndBoardReset(Player player)
        {
            if (player != null)
                player.playerState = PlayerState.Idle;
            playerHomes[playerHomeIndex].Reset();
            diceManager.ResetDices();
            nextPlayer();
            allowRolling = true;
        }
        #region checkForCandidate
        protected void checkForCandidate_Human(int playerHomeIndex)
        {
            RaycastHit hit = new RaycastHit();
            if (!mouseHit(ref hit)) return;

            Player player;
            if (hit.collider.gameObject == null
                || (player = hit.collider.gameObject.GetComponent<Player>()) == null) return;
            for (int i = 0; i < playerHomes[playerHomeIndex].players.Count; i++)
            {
                if (player == playerHomes[playerHomeIndex].getPlayer(i))
                {
                    playerHomes[playerHomeIndex].setCandidateIndex(i);
                    playerHomes[playerHomeIndex].getCandidatePlayer().playerState = PlayerState.Idle;
                    return;
                }
            }
        }
        #endregion
        private void checkForHitGhost(Player player)
        {
            RaycastHit hit = new RaycastHit();
            if (!mouseHit(ref hit)) return;

            PlayerGhost pg;
            if (hit.collider.gameObject == null
                || hit.collider.gameObject.transform.parent == null
                || (pg = hit.collider.gameObject.transform.parent.GetComponent<PlayerGhost>()) == null) return;
            if (pg.getPath() == null) return;
            player.GoTo_CalculatedIndexes(pg.getPath(), ref points);
            destroyGosts();
        }
        private bool mouseHit(ref RaycastHit hit)
        {
            if (!Input.GetMouseButtonDown(0)) return false;
            Vector3 mouse = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mouse);
            if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return false;
            return true;
        }
        #endregion
        #region Points
        public int getPointsCount()
        {
            return points == null ? -1 : points.Count;
        }
        public Vector3 getPoint(int index)
        {
            if (index < 0 || index >= points.Count) return Vector3.zero;
            return points[index];
        }
        public Quaternion getPointRotation(int from, int to)
        {
            Vector3 v = getPoint(to) - getPoint(from);
            if (v == Vector3.zero) return Quaternion.identity;
            return Quaternion.LookRotation(v);
        }
        public void setPoint(int index, Vector3 position)
        {
            points[index] = position;
            ElementNode node;
            if (nodes[index] == null)
            {
                initNode(index, position);
            }
            if ((node = elementNodesManager.getNode(index)) == null)
            {
                node = nodes[index].AddComponent<ElementNode>();
            }
            node.setPosition(position);
            if (autoRotation)
            {
                //node.setRotation(getPointRotation(index));
            }
        }
        public void checkPointsForRemovedNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == null)
                {
                    initNode(i, points[i]);
                }
            }
        }

        public void insertPoint(int index, Vector3 position)
        {
            insertNode(index, position);
            points.Insert(index, position);
        }
        private void insertNode(int index, Vector3 position)
        {
            long id = System.DateTime.Now.ToFileTimeUtc();
            GameObject go = new GameObject("_Home " + id.ToString());
            go.transform.SetParent(this.transform);
            ElementNode node = go.AddComponent<ElementNode>();
            node.point = position;
            node.index = index;
            node.ID = id;
            node.UpdatePrefab(defaultPrefab);
            nodes.Insert(index, go);
        }
        public void nodeConnection(int node1Index, int node2Index)
        {
            if (node1Index < 0 || node1Index >= nodes.Count) return;
            if (node2Index < 0 || node2Index >= nodes.Count) return;
            if (node1Index == node2Index) return;
            ElementNode node = elementNodesManager.getNode(node1Index);
            if (node != null)
            {
                node.AddConnection(node2Index);
            }

        }
        public void initNode(int index, Vector3 position)
        {
            long id = System.DateTime.Now.ToFileTimeUtc();
            GameObject go = new GameObject(id.ToString());
            go.transform.SetParent(this.transform);
            ElementNode node = go.AddComponent<ElementNode>();
            node.point = position;
            node.index = index;
            node.ID = id;
            nodes[index] = go;
        }
        public void RemovePoint(int index)
        {
            ElementNode node = elementNodesManager.getNode(index);
            ElementNode nextNode = elementNodesManager.getNode((index - 1) % nodes.Count);
            if (nextNode != null)
            {
                nextNode.AddConnection(node.connections);
            }
            if (node != null)
            {
                Object.DestroyImmediate(nodes[index].gameObject);
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == index || (node = elementNodesManager.getNode(i)) == null) continue;
                node.RemoveConnectionByValue(index);
                if (node.index > index) node.index--;
                node.DecreaseConnectionValues(index);
                if (node.redirectIndex == index)
                {
                    node.setRedirectIndex(-1);
                }
            }
            points.RemoveAt(index);
            nodes.RemoveAt(index);
        }
        public void PointsClear()
        {
            foreach (var node in nodes)
            {
                var f = node.GetComponent<ElementNode>();
                if (f != null) f.OnDestroy();
                Object.DestroyImmediate(node);
            }
            nodes.Clear();
            points.Clear();
        }
        #endregion
        #region player
        private void destroyGosts()
        {
            if (ghosts == null) return;
            foreach (GameObject node in ghosts)
            {
                Object.DestroyImmediate(node);
            }
        }
        private void showPaths(ref Player player)
        {
            ghosts = new List<GameObject>();
            for (int i = 0; i < player.pathManager.Paths.Count; i++)
            {
                int[] hits = player.pathManager.Paths[i].getHitIndex();
                GameObject gost = new GameObject("gost" + i);
                gost.transform.SetParent(this.transform);
                PlayerGhost pg = gost.AddComponent<PlayerGhost>();
                pg.setPath(player.pathManager.Paths[i]);
                if (player.gostPrefab == null)
                {
                    var gp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    pg.setPrefab(gp);
                    Object.DestroyImmediate(gp);
                }
                else
                {
                    pg.setPrefab(player.gostPrefab);
                }
                pg.updatePosition(getPoint(hits[hits.Length - 1]));
                ghosts.Add(gost);
            }
        }
        protected virtual void nextPlayer()
        {
            playerHomeIndex = (playerHomeIndex + 1) % playerHomes.Count;
        }
        public void initPlayers()
        {
            if (playerHomes == null || playerHomes.Count < 1)
            {
                Debug.LogError("No player home found.");
                Extensions.Quit();
                return;
            }
            foreach (PlayerHome node in playerHomes)
            {
                node.Reset();
                node.initPlayers();
            }
            playerHomeIndex = 0;
        }
        #endregion
        #region PlayerHome
        public void addPlayerHome(Vector3 position)
        {
            addPlayerHome(position, 0);
        }
        public void addPlayerHome(Vector3 position, int pointIndex)
        {
            PlayerHome node = new PlayerHome(this.transform);
            node.setCenter(position);
            node.index = playerHomes.Count;
            node.startIndex = pointIndex;
            node.updatePositions(handlePlayerHomesRadius);
            playerHomes.Add(node);
        }
        public void removePlayerHome(PlayerHome item)
        {
            removePlayerHome(playerHomes.IndexOf(item));
        }
        public void removePlayerHome(int index)
        {
            if (index < 0 || index >= playerHomes.Count) return;
            for (int i = index + 1; i < playerHomes.Count; i++)
            {
                playerHomes[i].index--;
            }
            playerHomes[index].onDestroy();
            playerHomes.RemoveAt(index);
        }
        public void playerHomeReset()
        {
            for (int i = 0; i < playerHomes.Count; i++)
            {
                playerHomes[i].onDestroy();
            }
            playerHomes.Clear();
        }
        public void setPlayerHome(int index, Vector3 position)
        {
            if (index < 0 || index >= playerHomes.Count) return;
            if (playerHomes[index] == null)
            {
                return;
            }
            playerHomes[index].center = position;
            playerHomes[index].updatePositions(handlePlayerHomesRadius);
        }
        public PlayerHome getPlayerHome(int index)
        {
            return playerHomes[index];
        }
        #endregion
        #region shape
        public void CircleShape(CircleStruct cs)
        {
            if (points.Count < 1) return;
            cs.center = points.ToArray().ToCenter();
            CircleMaker c = new CircleMaker();
            points = c.CreateCircle(cs.radius, cs.center, points.Count);
            updatePrefabs();
        }
        public void SquareShape(SquareStruct cs)
        {
            if (points.Count < 4) return;
            cs.center = points.ToArray().ToCenter();
            cs.b = cs.a;
            SquareMaker s = new SquareMaker();
            points = s.CreateSquare(cs, points.Count);
            updatePrefabs();
        }
        public void PolygonShape(PolygonStruct ps)
        {
            if (points.Count < 2 || ps.edges < 3 || points.Count < ps.edges) return;
            ps.center = points.ToArray().ToCenter();
            PolygonMaker s = new PolygonMaker();
            points = s.CreatePolygon(ps, points.Count);
            updatePrefabs();
        }
        public void RectangleShape(SquareStruct cs)
        {
            if (points.Count < 4) return;
            cs.center = points.ToArray().ToCenter();
            SquareMaker s = new SquareMaker();
            points = s.CreateSquare(cs, points.Count);
            updatePrefabs();
        }
        public void TriangleShape(TriangleStruct cs)
        {
            if (points.Count < 3) return;
            cs.center = points.ToArray().ToCenter();
            TriangleMaker s = new TriangleMaker();
            points = s.CreateTriangle(cs, points.Count);
            updatePrefabs();
        }
        public void EllipseShape(EllipseStruct es)
        {
            if (points.Count < 1) return;
            es.center = points.ToArray().ToCenter();
            EllipseMaker e = new EllipseMaker();
            points = e.CreateEllipse(es, points.Count);
            updatePrefabs();
        }
        public void DiamondShape(DiamondStruct ds)
        {
            if (points.Count < 4) return;
            ds.center = points.ToArray().ToCenter();
            DiamondMaker d = new DiamondMaker();
            points = d.CreateDiamond(ds, points.Count);
            updatePrefabs();
        }
        public void LineShape(LineStruct ls)
        {
            if (points.Count < 2) return;
            ls.center = points.ToArray().ToCenter();
            LineMaker d = new LineMaker();
            points = d.CreateLine(ls, points.Count);
            updatePrefabs();
        }
        public void SetNewCenter(Vector3 position)
        {
            this.gameObject.transform.position = position;
            Vector3 oldCenter = points.ToArray().ToCenter();
            int i;
            oldCenter.y = 0;
            for (i = 0; i < points.Count; i++)
            {
                Vector3 d = (points[i] - oldCenter);
                setPoint(i, d + position);
            }
            for (i = 0; i < playerHomes.Count; i++)
            {
                Vector3 d = (playerHomes[i].center - oldCenter);
                playerHomes[i].center = d + position;
            }
        }
        public void ChangeNodesScale(float deltaX)
        {
            Vector3 center = points.ToArray().ToCenter();
            int i;
            for (i = 0; i < points.Count; i++)
            {
                Vector3 d = (points[i] - center);
                float distance = Vector3.Distance(points[i], center) * deltaX;
                Vector3 newPosition = d.normalized * (distance) + center;
                setPoint(i, newPosition);
            }
            for (i = 0; i < playerHomes.Count; i++)
            {
                Vector3 d = (playerHomes[i].center - center);
                float distance = Vector3.Distance(playerHomes[i].center, center) * deltaX;
                playerHomes[i].center = d.normalized * (distance) + center;
            }
        }
        public void AssignPrefabForAllNodes(int index)
        {
            ElementNode node;
            if ((node = elementNodesManager.getNode(index)) == null)
            {
                node = nodes[index].AddComponent<ElementNode>();
            }
            var obj = node.prefab;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == index) continue;
                if (nodes[i] == null) initNode(i, points[i]);
                if ((node = elementNodesManager.getNode(i)) == null)
                {
                    node = nodes[i].AddComponent<ElementNode>();
                }
                node.UpdatePrefab(obj);
            }
        }
        public void updatePrefabs()
        {
            ElementNode node;
            for (int i = 0; i < points.Count; i++)
            {
                if ((node = elementNodesManager.getNode(i)) == null) continue;
                node.setPosition(points[i]);
                if (autoRotation)
                {
                    //node.setRotation(getPointRotation(i));
                }
            }
        }
        public void updatePrefabs(Quaternion r)
        {
            ElementNode node;
            for (int i = 0; i < points.Count; i++)
            {
                if ((node = elementNodesManager.getNode(i)) == null) continue;
                node.setPosition(points[i]);
                node.setRotation(r);
            }
        }
        #endregion
        #region controller
        protected virtual void rolling()
        {
            if (!allowRolling) return;
            switch (playerHomes[playerHomeIndex].playerMode)
            {
                case PlayerMode.Human:
                    if (Input.GetKeyDown(rollingKeyCode))
                    {
                        diceManager.setFlag(true);
                        allowRolling = false;
                    }
                    break;
                case PlayerMode.CPU:
                    diceManager.setFlag(true);
                    allowRolling = false;
                    break;
            }
        }
        public void garbageCollector()
        {
            Transform[] ts = this.transform.GetComponentsInChildren<Transform>();
            if (ts == null || ts.Length < 1) return;
            List<GameObject> childs = new List<GameObject>();

            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] == null || ts[i].gameObject == null || ts[i] == this.transform) continue;
                if (ts[i].gameObject.name.IndexOf("_Home ") == 0)
                {
                    childs.Add(ts[i].gameObject);
                }
            }
            ElementNode node;
            foreach (var item in childs)
            {
                if ((node = item.GetComponent<ElementNode>()) == null || nodes.IndexOf(item) < 0)
                {
                    DestroyImmediate(item);
                }
            }
        }
        #endregion
        #endregion
    }
}