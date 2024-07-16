using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GAME_STATE {
    CHOOSE_ACTION,
    ACTIVATE_EFFECT,

}

public class Game : MonoBehaviour
{

    const int INITIAL_SCOUT_AMOUNT  = 8;
    const int INITIAL_VIPER_AMOUNT  = 2;
    public Player playerOne;
    public GameObject cardPrefab;
    public GameObject playerOneGameObject;

    // Start is called before the first frame update
    void Start()
    {
        playerOne = playerOneGameObject.GetComponent<Player>();   
    }

    // Update is called once per frame
    void Update()
    {

    }
}
