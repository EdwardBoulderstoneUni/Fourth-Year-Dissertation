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
            output += "Rollback Frames +(X) / -(x): " + rollbackFrames + "\n";
        output += "Delay Frames +(C) / -(c): " + (rollback ? rollbackDelayFrames : delayDelayFrames) + "\n\n";

        output += "Ping +(V) / -(v): " + ping + " ms\n";
        output += "Ping Deviation +(B) / -(b): " + pingDeviation.ToString(".0#");
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
        if (Input.GetKey(netcodeTypeKey)){
            netcodeManager.swapNetcode();
        }
        if (Input.GetKey(rollbackFramesKey))
            changed = netcodeManager.changeRollbackFrames(modifier);

        if (Input.GetKey(delayFramesKey))
            changed = netcodeManager.changeDelayFrames(modifier);

        if (Input.GetKey(pingKey))
            netcodeManager.changePing(modifier);

        if (Input.GetKey(pingDeviationsKey))
            netcodeManager.changePingDeviation(modifier);
        
        if(changed)
            updateText();
    }

    public void updateText(){
        Debug.Log("????????");
        textMesh.text = netcodeManager.getStatus().ToString();
    }
}
