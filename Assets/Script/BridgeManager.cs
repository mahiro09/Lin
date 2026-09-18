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

    // ‹´‚ğ“o˜^
    public void RegisterBridge(BridgeLine bridge)
    {
        if (bridge == null)
        {
            return;
        }

        if (!bridgeLines.Contains(bridge))
        {
            bridgeLines.Add(bridge);

            Debug.Log(
                "BridgeManagerF‹´‚ğ“o˜^‚µ‚Ü‚µ‚½"
            );
        }
    }

    // ‹´‚ğ“o˜^‰ğœ
    public void UnregisterBridge(BridgeLine bridge)
    {
        if (bridge == null)
        {
            return;
        }

        if (bridgeLines.Contains(bridge))
        {
            bridgeLines.Remove(bridge);

            Debug.Log(
                "BridgeManagerF‹´‚ğ“o˜^‰ğœ‚µ‚Ü‚µ‚½"
            );
        }
    }

    // ƒvƒŒƒCƒ„[‚ª‹´‚Ìã‚É‚¢‚é‚©
    public bool IsOnBridge(Vector2 playerPosition)
    {
        foreach (BridgeLine bridge in bridgeLines)
        {
            if (bridge == null)
            {
                continue;
            }

            if (bridge.IsPointOnBridge(playerPosition))
            {
                return true;
            }
        }

        return false;
    }
}