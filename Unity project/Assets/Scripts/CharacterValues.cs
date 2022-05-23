using UnityEngine;

public class CharacterValues : MonoBehaviour
{
    [SerializeField] public LayerMask floor;
    [SerializeField] public int rejumpPreventionFrames = 5;
    [SerializeField] public float moveSpeed = 4f;
    [SerializeField] public float jumpSpeed = 10f;
    [SerializeField] public float distanceToGround = 0.1f;
}
