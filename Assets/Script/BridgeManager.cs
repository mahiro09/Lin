using UnityEngine;
using System.Collections.Generic;

public class BridgeManager : MonoBehaviour
{
    public static BridgeManager Instance;

    private List<BridgeLine> bridgeLines =
        new List<BridgeLine>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterBridge(BridgeLine bridge)
    {
        if (!bridgeLines.Contains(bridge))
        {
            bridgeLines.Add(bridge);
        }
    }

    public bool IsOnBridge(Vector2 playerPosition)
    {
        foreach (BridgeLine bridge in bridgeLines)
        {
            if (bridge != null &&
                bridge.IsPointOnBridge(playerPosition))
            {
                return true;
            }
        }

        return false;
    }
}