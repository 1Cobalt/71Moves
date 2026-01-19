using System.Collections.Generic;
using UnityEngine;
using MyDice.Board;

namespace MyDice.Players
{
    public class Player : MonoBehaviour
    {
        #region variable
        #region public
        public float movementSpeed = 1;
        public float targetDistanceHit = .05f;
        public GameObject gostPrefab;
        public PlayerMovementType playerMovementType = PlayerMovementType.Direct;
        //public int positionIndexSize = 2;
        [HideInInspector] public int playerHomeIndex;
        #region PlayerPositionIndex
        [HideInInspector] public int[] hitIndex;
        public int currentPositionIndex { get { return positionIndex.GetIndex(0); } set { positionIndex.AddIndex(value); } }
        #endregion
        [HideInInspector] public float deltaTime;
        [HideInInspector] public int[] diceValues;
        [HideInInspector] public PathManager pathManager;
        public PlayerState playerState { get; set; }
        #endregion
        #region protected
        [HideInInspector] public PlayerPositionIndex positionIndex = new PlayerPositionIndex(1);
        #endregion
        #region private
        [HideInInspector] public List<Vector3> targets;
        #endregion
        #endregion
        #region Functions
        void Start()
        {
            if (deltaTime == 0)
                deltaTime = Time.fixedDeltaTime;
            //if (positionIndexSize < 1) positionIndexSize = 1;
            //positionIndex = new PlayerPositionIndex(positionIndexSize);
            //positionIndex.AddIndex(startIndex);
            targets = new List<Vector3>();
        }
        private void LateUpdate()
        {
            updateMovementPosition();
        }
        #endregion
        #region functions
        #region prefab

        #endregion
        #region values
        public int getTotalValues()
        {
            int sum = 0;
            if (diceValues == null) return sum;
            for (int i = 0; i < diceValues.Length; i++)
                sum += diceValues[i];
            return sum;
        }
        #endregion
        #region movement
        #region GOTO
        public void GoTo_CalculatedIndexes(Path p,ref List<Vector3> nodes)
        {
            currentPositionIndex = p.getIndex(p.getIndexSize() - 1);
            List<Vector3> positions = new List<Vector3>();
            for (int i = 0; i < p.getIndexSize(); i++)
            {
                positions.Add(nodes[p.getIndex(i)]);
            }
            GoTo(positions);
        }
        public void GoTo(List<Vector3> positions)
        {
            switch (playerMovementType)
            {
                /* case PlayerMovementType.Circle:
                     CircleMaker c = new CircleMaker();
                     for (int i = 0; i < positions.Count - 1; i++)
                     {
                         var list = c.CreateHalfCircle(positions[i], positions[i + 1], 5);
                         if (list == null) continue;
                         for (int j = 0; j < list.Count; j++)
                             targets.Add(list[j]);
                     }
                     break;*/
                default:
                    for (int i = 0; i < positions.Count; i++)
                        targets.Add(positions[i]);
                    break;
            }
            GoTo_Immediately(positions[0]);
            playerState = PlayerState.Moving;
        }
        public void GoTo_Immediately(Vector3 position)
        {
            this.transform.position = position;
        }
        #endregion
        public bool hasPath()
        {
            return pathManager != null && pathManager.Paths.Count > 0;
        }
        public void CalculatePositionIndex(ref List<GameObject> nodes)
        {
            if (nodes==null || diceValues == null && diceValues.Length < 1) return;
            pathManager = new PathManager(currentPositionIndex, diceValues);
            pathManager.FindPaths(ref nodes);
        }
        public void CalculatePositionIndex(int size)
        {
            hitIndex = new int[diceValues.Length + 1];
            hitIndex[0] = currentPositionIndex;
            for (int i = 1; i < hitIndex.Length; i++)
            {
                hitIndex[i] = (hitIndex[i - 1] + diceValues[i - 1]) % size;
            }
            positionIndex.AddIndex((currentPositionIndex + getTotalValues()) % (size));
        }
        private void updateMovementPosition()
        {
            if (targets == null || targets.Count < 1) return;
            if (Vector3.Distance(targets[0], this.transform.position) < targetDistanceHit)
            {
                targets.RemoveAt(0);
                if (targets.Count < 1)
                    playerState = PlayerState.MovingComplete;
                return;
            }
            Vector3 dir = (targets[0] - this.transform.position);
            this.transform.rotation = Quaternion.LookRotation(dir);
            this.transform.position += dir.normalized * (deltaTime * movementSpeed);
        }
        #endregion
        #endregion
    }
}