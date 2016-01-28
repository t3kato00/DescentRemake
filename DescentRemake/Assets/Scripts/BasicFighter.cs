using UnityEngine;
using System.Collections;

public class BasicFighter : MonoBehaviour {
    AIPlayerTargetSelection playerTracking;

    bool Fighting
    {
        get { return playerTracking.TargetedPlayer != null; }
    }

    // Use this for initialization
    void Start () {
        playerTracking = GetComponent<AIPlayerTargetSelection>();
	}
	
	// Update is called once per frame
	void Update () {
	    if( Fighting )
        {

        }
	}
}
