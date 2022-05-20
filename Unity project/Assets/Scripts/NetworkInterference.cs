using System;
using System.Collections;
using UnityEngine;
[System.Serializable] public struct InterferenceMetrics{
    public int ping;
    //public float packetLoss; add packetloss later
    public float pingDeviation;
}

public class NetworkInterference : MonoBehaviour
{
    // TODO getting rollbacks at 0 ping/0variation, dir>jump not consistent behaviour, mash jump, jumps multiple times, desync issues
    [SerializeField] public InterferenceMetrics interferenceMetrics;
    System.Random rand = new System.Random();

    public void interfere(Netcode target, TimedData<InputStruct> packet){
        double packetDelay = NextGaussian(rand, interferenceMetrics.ping/2, interferenceMetrics.pingDeviation);
        StartCoroutine(sendPacket(target, packet, (float) packetDelay));
        // TODO chance to disregard the packet (packet loss)
    }

    private IEnumerator sendPacket(Netcode target, TimedData<InputStruct> packet, float delayTime){
        yield return new WaitForSeconds(delayTime);
        target.remoteInput(packet);
    }
    public static double NextGaussian(System.Random r, double mean = 0, double stdDev = 1)
    {
        // credit https://bitbucket.org/Superbest/superbest-random/src/master/
        var u1 = r.NextDouble();
        var u2 = r.NextDouble();

        var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);

        var rand_normal = mean + stdDev * rand_std_normal;

        return rand_normal;
    }
}
