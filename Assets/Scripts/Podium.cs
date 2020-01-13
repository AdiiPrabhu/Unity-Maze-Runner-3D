using UnityEngine;
using System.Collections;

public class Podium : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
        if (other.tag == "runner")
        {
            Game.GameState = GameStates.Win;
        }
	}
}
