using UnityEngine;
using System.Collections;

public class AIPlayerTargetSelection : MonoBehaviour {

    public LayerMask SensorLevelLayers;

    // The player we are targeting.
    private PlayerTarget targetedPlayer;
    public PlayerTarget TargetedPlayer
    {
        get { return targetedPlayer; }
    }

    protected new Rigidbody rigidbody;
    protected AITargetTracking tracking;
    protected AITargetFollowing following;

    protected void Start () {
        tracking = GetComponent<AITargetTracking>();
        following = GetComponent<AITargetFollowing>();
        updateTargetedPlayerRountine = StartCoroutine(UpdateTargetedPlayerRountine());
    }

    protected void OnDestroy()
    {
        StopCoroutine(updateTargetedPlayerRountine);
    }

    private Coroutine updateTargetedPlayerRountine;
    private IEnumerator UpdateTargetedPlayerRountine()
    {
        float interval = Random.Range(0.7f, 0.8f);
        while (true)
        {
            UpdateTargetedPlayer();
            yield return new WaitForSeconds(interval);
        }
    }

    private void UpdateTargetedPlayer()
    {
        RaycastHit hit;

        // Find nearest visible player.
        targetedPlayer = null;
        float targetedPlayerDistance2 = float.PositiveInfinity;
        foreach (PlayerTarget player in PlayerTarget.Players)
        {
            Vector3 relativePlayerPosition = player.transform.position - transform.position;
            float playerDistance2 = relativePlayerPosition.sqrMagnitude;

            if (playerDistance2 >= targetedPlayerDistance2)
                continue;

            if (Physics.Raycast(transform.position, relativePlayerPosition, out hit, Mathf.Sqrt(playerDistance2), SensorLevelLayers))
                continue;

            targetedPlayer = player;
            targetedPlayerDistance2 = playerDistance2;
            tracking.TrackedTarget = player.GetComponent<Rigidbody>();
        }
    }

}
