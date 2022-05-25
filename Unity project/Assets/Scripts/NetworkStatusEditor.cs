using UnityEngine;
using TMPro;
public struct NetworkStatus{
    public bool rollback;
    public int rollbackFrames;
    public int rollbackDelayFrames;
    public int delayDelayFrames;
    public int ping;
    public float pingDeviation;
    public override string ToString(){
        string output = "";
        output += "(Z/z) " + (rollback ? "Rollback" : "Delay Based") + "\n";
        if (rollback)
            output += "Rollback Frames +(x) / -(X): " + rollbackFrames + "\n";
        output += "Delay Frames +(c) / -(C): " + (rollback ? rollbackDelayFrames : delayDelayFrames) + "\n\n";

        output += "Ping +(v) / -(V): " + ping + " ms\n";
        output += "Ping Deviation +(b) / -(B): " + pingDeviation.ToString(".0#");
        return output;
    }
}

public class NetworkStatusEditor : MonoBehaviour
{
    private NetcodeManager netcodeManager;
    [SerializeField] private TextMeshProUGUI textMesh;
    private const KeyCode modifierKey = KeyCode.LeftShift;
    private const KeyCode netcodeTypeKey = KeyCode.Z;
    private const KeyCode rollbackFramesKey = KeyCode.X;
    private const KeyCode delayFramesKey = KeyCode.C;
    private const KeyCode pingKey = KeyCode.V;
    private const KeyCode pingDeviationsKey = KeyCode.B;
    void Start(){
        netcodeManager = GetComponent<NetcodeManager>();
        updateText();
    }
    void Update()
    {
        bool changed = false;
        bool modifier = Input.GetKey(modifierKey);
        if (Input.GetKeyDown(netcodeTypeKey)){
            netcodeManager.swapNetcode();
            changed = true;
        }
        if (Input.GetKeyDown(rollbackFramesKey))
            changed = netcodeManager.changeRollbackFrames(modifier);

        if (Input.GetKeyDown(delayFramesKey))
            changed = netcodeManager.changeDelayFrames(modifier);

        if (Input.GetKeyDown(pingKey))
            netcodeManager.changePing(modifier);

        if (Input.GetKeyDown(pingDeviationsKey))
            netcodeManager.changePingDeviation(modifier);
        
        if(changed)
            updateText();
    }

    public void updateText(){
        textMesh.text = netcodeManager.getStatus().ToString();
    }
}
