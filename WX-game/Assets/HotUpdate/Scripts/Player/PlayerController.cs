using UnityEngine;
using UnityEngine.Scripting;
[Preserve]
public class PlayerController : MonoBehaviour
{
    public static GameObject Player;
    public static PlayerAttack Attack;
    void Start()
    {
        Player = this.gameObject;
        Attack = GetComponent<PlayerAttack>();
    }
}
