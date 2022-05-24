using UnityEngine;

public class PhysicsUpdater : MonoBehaviour
{
    public int requestsForUpdate = 0;
    private GameState[] gameStates = new GameState[2];
    void Start()
    {
        gameStates = gameObject.GetComponent<NetcodeManager>().gameStates;
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

    public void runPhysics(){
        // assumes both running on same frame
        requestsForUpdate += 1;
        if (requestsForUpdate == 2){
            Physics2D.Simulate(1/60f);
            requestsForUpdate = 0;
            foreach (var game in gameStates){
                game.saveState();
            }
        }
    }

    public void localPhysicsUpdate(int gameEngineID){
        gameStates[(gameEngineID+1)%2].pausePhysics();
        Physics2D.Simulate(1/60f);
        gameStates[(gameEngineID+1)%2].resumePhysics();
    }   
}
