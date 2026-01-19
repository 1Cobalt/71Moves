using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyDice.Board
{
    [System.Serializable]
    public class PathManager
    {
        #region variable
        public List<Path> Paths { get { return paths; } }
        protected List<Path> paths;
        protected int pathSize;
        protected int currentIndex;
        protected int[] diceValues;
        #endregion
        #region constructor
        public PathManager(int currentIndex, int[] diceValues)
        {
            paths = new List<Path>();
            this.diceValues = diceValues;
            this.currentIndex = currentIndex;
            this.pathSize = 1;
            for (int i = 0; i < diceValues.Length; i++)
                this.pathSize += diceValues[i];
        }
        #endregion
        #region functions
        #region paths
        public void FindPaths(ref List<GameObject> nodes)
        {
            ElementNodesManager elementNodesManager = new ElementNodesManager(ref nodes);
            paths = new List<Path>();
            ElementNode node = elementNodesManager.getNode(currentIndex);
            if (node == null) return;
            Path p = new Path(pathSize, diceValues);
            p.setIndex(0, currentIndex);
            findPath(p, ref elementNodesManager);
        }
        private void findPath(Path p, ref ElementNodesManager elementNodesManager)
        {
            if (p.isCompleted())
            {
                if (!pathIsExist(p)) paths.Add(p);
                return;
            }
            int lastIndexValue = p.getLastValue();
            if (lastIndexValue == -1) return;
            ElementNode node = elementNodesManager.getNode(lastIndexValue);
            if (node == null) return;
            for (int i = 0; i < node.getConnectionsSize(); i++)
            {
                Path newPath = new Path(p);
                if (newPath.addIndex(node.connections[i]))
                {
                    findPath(newPath, ref elementNodesManager);
                }
            }
            p = null;
        }
        public bool pathIsExist(Path p)
        {
            foreach (Path path in paths)
                if (p.EqualsTo(path)) return true;
            return false;
        }
        public Path getBestBenefitPath()
        {
            if (paths.Count < 1) return null;
            List<Path> result = new List<Path>();
            result.Add(paths[0]);
            int bestBenefit = paths[0].Benefit;
            for (int i = 1; i < paths.Count; i++)
            {
                if (paths[i].Benefit == bestBenefit)
                {
                    result.Add(paths[i]);
                }
                else if (paths[i].Benefit < bestBenefit)
                {
                    result.Clear();
                    result.Add(paths[i]);
                    bestBenefit = paths[i].Benefit;
                }
            }
            if (result.Count == 1)
            {
                return result[0];
            }
            return result[Random.Range(0, result.Count)];
        }
        public Path getRandomPath()
        {
            if (paths.Count < 1) return null;
            return paths[Random.Range(0, paths.Count)];
        }
        #endregion
        #region AI
        public bool UpdateBenefits(int startIndex, int targetIndex, ref List<GameObject> nodes)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                if (!UpdateBenefits(paths[i], startIndex, targetIndex, ref nodes))
                {
                    return false;
                }
            }
            return true;
        }
        public bool UpdateBenefits(Path path, int startHomeIndex, int targetIndex, ref List<GameObject> nodes)
        {
            if (!path.isCompleted()) return false;

            ElementNodesManager elementNodesManager = new ElementNodesManager(ref nodes);
            path.Benefit = elementNodesManager.getNodesSize();

            ElementNode startNode = elementNodesManager.getNode(path.getLastValue());
            ElementNode targetNode = elementNodesManager.getNode(targetIndex);

            if (startNode == null) return false;
            if (targetNode == null)
            {
                return true;
            }
            startNode = elementNodesManager.getFinalDestination(startNode);
            if (startNode.getElementNodeType() == ElementNodeType.ResetPoint)
            {
                startNode = elementNodesManager.getNode(startHomeIndex);
            }
            bool condition = true;

            findBenefit(ref path, startNode, targetNode, new PathData(), ref condition, ref elementNodesManager);
            return true;
        }
        public void findBenefit(ref Path path, ElementNode startNode, ElementNode targetNode, PathData pathData, ref bool condition, ref ElementNodesManager elementNodesManager)
        {
            if (!condition)
            {
                return;
            }
            if (startNode == targetNode)
            {
                path.Benefit = pathData.getNodesCount();
                condition = false;
                return;
            }
            for (int i = 0; i < startNode.connections.Count; i++)
            {
                if (!pathData.nodeIsExist(startNode.connections[i]))
                {
                    PathData newPathData = new PathData(pathData);
                    findBenefit(ref path,
                        elementNodesManager.getNode(startNode.connections[i]),
                        targetNode,
                        newPathData.addNode(startNode.connections[i]),
                        ref condition, ref elementNodesManager);
                }
            }
        }
        #endregion
        #endregion
    }
}