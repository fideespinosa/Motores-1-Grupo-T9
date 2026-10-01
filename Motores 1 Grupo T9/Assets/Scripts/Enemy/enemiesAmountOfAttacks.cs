using UnityEngine;

public class enemiesAmountOfAttacks : MonoBehaviour
{
    [SerializeField] int count = 0;

    public int getAmount()
    {
        return count;
    }
    public void setAmount(int n)
    {
        count = n;
    }

}
