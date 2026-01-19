using UnityEditor;
using UnityEngine;

namespace MyDice.Menu
{
    public class MainMenu : MonoBehaviour
    {/*
        #region Dice
        [MenuItem("Dice Board Starter/Dice/Add Dice 4")]
        static void AddDice4()
        {
            GameObject prefab =(GameObject)Resources.Load("prefabs/Dices/Dice_D4",typeof(GameObject));
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.position = Vector3.zero;
            Selection.objects = new Object[] { obj };
        }
        [MenuItem("Dice Board Starter/Dice/Add Dice 6")]
        static void AddDice6()
        {
            GameObject prefab = (GameObject)Resources.Load("prefabs/Dices/Dice_D6", typeof(GameObject));
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.position = Vector3.zero; Selection.objects = new Object[] { obj };
        }
        [MenuItem("Dice Board Starter/Dice/Add Dice 8")]
        static void AddDice8()
        {
            GameObject prefab = (GameObject)Resources.Load("prefabs/Dices/Dice_D8", typeof(GameObject));
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.position = Vector3.zero; Selection.objects = new Object[] { obj };
        }
        [MenuItem("Dice Board Starter/Dice/Add Dice 10")]
        static void AddDice10()
        {
            GameObject prefab = (GameObject)Resources.Load("prefabs/Dices/Dice_D10", typeof(GameObject));
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.position = Vector3.zero; Selection.objects = new Object[] { obj };
        }
        #endregion
        #region Board
        [MenuItem("Dice Board Starter/Board/Add Board 1")]
        static void AddBoard_1()
        {
            GameObject prefab = (GameObject)Resources.Load("prefabs/Boards/Board_1", typeof(GameObject));
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.position = Vector3.zero;
            Selection.objects = new Object[] { obj };
        }
        [MenuItem("Dice Board Starter/Board/Add Board 2")]
        static void AddBoard_2()
        {
            GameObject prefab = (GameObject)Resources.Load("prefabs/Boards/Board_2", typeof(GameObject));
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.position = Vector3.zero;
            Selection.objects = new Object[] { obj };
        }
        #endregion
        #region Player
        /*
        [MenuItem("Dice Board Starter/Player/Add Player 1")]
        static void AddPlayer_1()
        {
            AddPlayer("Player_1");
        }
        [MenuItem("Dice Board Starter/Player/Add Player 2")]
        static void AddPlayer_2()
        {
            AddPlayer("Player_2");
        }
        [MenuItem("Dice Board Starter/Player/Add Player 3")]
        static void AddPlayer_3()
        {
            AddPlayer("Player_3");
        }
        [MenuItem("Dice Board Starter/Player/Add Player 4")]
        static void AddPlayer_4()
        {
            AddPlayer("Player_4");
        }
        private static void AddPlayer(string name)
        {
            GameObject prefab = (GameObject)Resources.Load("prefabs/Players/"+ name, typeof(GameObject));
            GameObject player = new GameObject(name);
            player.transform.position = Vector3.zero;
            GameObject obj = Object.Instantiate(prefab);
            obj.transform.SetParent(player.transform);

            Players.Player f = player.AddComponent<Players.Player>();
            f.prefab = obj;
            Selection.objects = new Object[] { obj };
        }
        #endregion
        #region Board
        [MenuItem("Dice Board Starter/Game manager/Add Game manager")]
        static void AddBoardGameManager()
        {
            GameObject obj = new GameObject("Game manager");
            obj.AddComponent<BoardGameManager>();
            obj.transform.position = Vector3.zero;
            Selection.objects = new Object[] { obj };
        }
        
        #endregion
        */
    }
}