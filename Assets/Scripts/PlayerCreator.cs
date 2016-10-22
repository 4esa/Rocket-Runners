using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreator : MonoBehaviour
{

    public GameObject PlayerPrefab;
    public PlayerInformation[] Players;

    void Start()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            Vector3 pos = new Vector3(gameObject.transform.position.x - i, gameObject.transform.position.y, gameObject.transform.position.z);
            Quaternion quaternion = new Quaternion();
            GameObject player = (GameObject)Instantiate(PlayerPrefab, pos, quaternion);
            player.GetComponent<MovementController>().JumpName = Players[i].JumpName;
            player.GetComponent<MovementController>().FallName = Players[i].FallName;
            player.GetComponent<ScoreController>().ScoreText = Players[i].ScoreText;
            player.GetComponent<ScoreController>().PlayerName = Players[i].PlayerName;
            GameObject character = Instantiate(Players[i].Character);
            character.gameObject.transform.SetParent(player.transform);
            character.gameObject.transform.SetSiblingIndex(0);
        }
        StartCoroutine(NextFrame());
    }

    IEnumerator NextFrame()
    {
        yield return null;
        GameObject[] PlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < PlayerObjects.Length; i++)
        {
            PlayerObjects[i].transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0); //Realign
        }
    }
}

[Serializable]
public class PlayerInformation
{
    public String JumpName;
    public String FallName;
    public Text ScoreText;
    public string PlayerName;
    public GameObject Character;
}

