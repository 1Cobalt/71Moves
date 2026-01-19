using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyDice.Players
{
    [System.Serializable]
    public class PlayerPositionIndex
    {
        public int[] positionIndex;
        public PlayerPositionIndex(int size)
        {
            if (size < 1) size = 1;
            positionIndex = new int[size];
            Reset();
        }
        public void AddIndex(int index)
        {
            int i = positionIndex.Length;
            while (--i != 0)
            {
                positionIndex[i] = positionIndex[i - 1];
            }
            positionIndex[i] = index;
        }
        public int GetIndex(int index)
        {
            if (index < 0 || index >= positionIndex.Length) return -1;
            return positionIndex[index];
        }
        public int getSize()
        {
            return positionIndex.Length;
        }
        public void Reset()
        {
            for (int i = 0; i < positionIndex.Length; i++)
                positionIndex[i] = -1;
        }
    }
}