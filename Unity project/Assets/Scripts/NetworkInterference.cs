public struct InterferenceMetrics{
    public int ping;
    public float packetLoss;
}

public class NetworkInterference
{
    public void interfere(Netcode netcode, InputStruct packet, InterferenceMetrics interferenceMetrics){
        // TODO chance to disregard the packet, then send it after random delay around ping (need to work out sd)
    }

    private void sendPacket(Netcode netcode, InputStruct packet){
        netcode.update(packet);
    }
}
