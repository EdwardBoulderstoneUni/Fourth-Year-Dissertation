using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Netcode : MonoBehaviour
{
    // Player 0 has rollback and player 1 has delay based
    [SerializeField] public int ping = 30;
    [SerializeField] public const int rollbackFrames = 8;
    [SerializeField] public int[] delayFrames = {0, 0};
    [Range(0, 100)] [SerializeField] public float packetLoss = 0f;
    // Start is called before the first frame update
    void Start()
    {
        // TODO this needs work
    }
    public void update(InputStruct input, int delayBased)
    {
        // TODO this needs work
    }
    public InputStruct getRemoteInput(int delayBased){
        // TODO this needs work
        return new InputStruct();
    }
}
