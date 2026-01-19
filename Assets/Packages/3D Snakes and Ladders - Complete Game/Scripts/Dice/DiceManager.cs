using System.Collections.Generic;
using UnityEngine;

namespace MyDice
{
    public class DiceManager : MonoBehaviour
    {
        #region variables
        #region public
        public int[] dicesValues { get; protected set; }
        public List<string> collisionObjectTags = new List<string>() { "Dice Ground Wall" };
        public float rollingThreshold = 10;
        #endregion
        #region protected
        [HideInInspector] public List<Dice> dices;
        [HideInInspector] public DiceState dicesState = DiceState.Null;
        #endregion
        #region private
        [HideInInspector] public bool flag = false;
        [HideInInspector] public bool isFresh_dicesValues;
        [HideInInspector] public float rollingThresholdCounter = 0;
        #endregion
        #endregion
        #region getter setter
        public DiceState getDiceState()
        {
            return dicesState;
        }
        public void setFlag(bool val)
        {
            flag = val;
        }
        public bool getFlag() { return flag; }
        private bool isFlaged()
        {
            if (flag)
            {
                flag = false;
                return true;
            }
            return false;
        }
        #endregion
        #region Functions
        private void Start()
        {
            init();
        }
        private void Update()
        {
            checkDicesState();
            if (dicesState == DiceState.Rolling)
            {
                manageRollingThreshold();
            }
            else if (dicesState == DiceState.Ready)
            {
                if (isFlaged())
                    RoolDices();
            }
            else if (dicesState == DiceState.Finish)
            {
                if (!isFresh_dicesValues)
                {
                    getDicesValues();
                }
            }
        }
        #endregion
        #region functions
        #region init
        private void init()
        {
            findDices();
            initDices();
        }
        #endregion
        #region dices
        private void findDices()
        {
            var temp = FindObjectsOfType<Dice>();
            if (temp == null || temp.Length < 1)
            {
                Debug.LogError("No dice found.");
                Extensions.Quit();
            }
            dices = new List<Dice>(temp);
        }
        private void initDices()
        {
            string diceGroundTagName = this.gameObject.tag;
            if (collisionObjectTags == null) collisionObjectTags = new List<string>();
            for (int i = 0; i < dices.Count; i++)
            {
                if (dices[i] == null)
                {
                    dices.RemoveAt(i--);
                    continue;
                }
                string tagName = dices[i].tag;
                if (string.IsNullOrEmpty(tagName) || collisionObjectTags.Exists(e => e.Equals(tagName))) continue;
                collisionObjectTags.Add(tagName);
            }
            for (int i = 0; i < dices.Count; i++)
            {
                if (dices[i] == null)
                {
                    dices.RemoveAt(i--);
                    continue;
                }
                dices[i].diceGroundTagName = diceGroundTagName;
                dices[i].collisionObjectTags = this.collisionObjectTags;
            }
        }
        private void checkDicesState()
        {
            if (dices.Count < 1) return;
            for (int i = 1; i < dices.Count; i++)
            {
                if (dices[i] == null)
                {
                    dices.RemoveAt(i--);
                    continue;
                }
                if (dices[i].diceState != dices[0].diceState)
                {
                    dicesState = DiceState.Null;
                    return;
                }
            }
            if (dicesState != dices[0].diceState) isFresh_dicesValues = false;
            dicesState = dices[0].diceState;
        }
        public void RoolDices()
        {
            for (int i = 0; i < dices.Count; i++)
            {
                if (dices[i] == null)
                {
                    dices.RemoveAt(i--);
                    continue;
                }
                dices[i].RollDice();
            }
        }
        public void ResetDices()
        {
            for (int i = 0; i < dices.Count; i++)
            {
                if (dices[i] == null)
                {
                    dices.RemoveAt(i--);
                    continue;
                }
                dices[i].ResetDice();
            }
        }
        public int[] getDicesValues()
        {
            if (dicesState != DiceState.Finish) return null;
            isFresh_dicesValues = true;
            int[] result = new int[dices.Count];
            for (int i = 0; i < dices.Count; i++)
            {
                if (dices[i] == null) continue;
                result[i] = dices[i].Value;
            }
            return dicesValues = result;
        }
        #region rolling over flow
        private void manageRollingThreshold()
        {
            rollingThresholdCounter += Time.fixedDeltaTime;
            if (rollingThresholdCounter > rollingThreshold)
            {
                rollingThresholdCounter = 0;
                Debug.Log("Warning: Rolling is over flow.");
                onRollingOverFlow();
            }
        }
        public virtual void onRollingOverFlow()
        {

        }
        #endregion
        #endregion
        #endregion
    }
}