using UnityEngine;

public class CharacterValues : MonoBehaviour
{
    [SerializeField] public LayerMask floor;
    [SerializeField] public float MoveSpeed = 4f;
    [SerializeField] public float JumpSpeed = 10f;
    [SerializeField] public float DeadZone = 0.1f;
    [SerializeField] public float DistanceToGround = 0.1f;
}
