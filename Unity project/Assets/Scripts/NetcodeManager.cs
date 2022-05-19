using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetcodeManager : MonoBehaviour
{
    // Player 0 has rollback and player 1 has delay based
    [SerializeField] public InterferenceMetrics interferenceMetrics;
    [SerializeField] public const int rollbackFrames = 8;
    [SerializeField] public int[] delayFrames = {0, 0};

    private Rollback rollbackNetcode = new Rollback();
    private DelayBased delayBasedNetcode = new DelayBased();
    private NetworkInterference interference = new NetworkInterference();
    public void update(InputStruct input, int delayBased)
    {
        interference.interfere(delayBased == 0 ? rollbackNetcode : delayBasedNetcode, input, interferenceMetrics);
    }
    public InputStruct getRemoteInput(int delayBased){
        if (delayBased == 1)
            return delayBasedNetcode.getRemoteInput();
        else
            return rollbackNetcode.getRemoteInput();
    }
}
