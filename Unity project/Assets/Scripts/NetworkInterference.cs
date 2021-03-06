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
    [SerializeField] public InterferenceMetrics interferenceMetrics;
    System.Random rand = new System.Random();

    public void interfere(Netcode target, Packet<InputStruct> packet){
        double packetDelay = NextGaussian(rand, interferenceMetrics.ping/2000f, interferenceMetrics.pingDeviation/2000f);
        if (packetDelay >= 1/60f)
            StartCoroutine(sendPacket(target, packet, (float) packetDelay));
        else
            target.remoteInput(packet);
        // TODO chance to disregard the packet (packet loss)
    }

    private IEnumerator sendPacket(Netcode target, Packet<InputStruct> packet, float delayTime){
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
