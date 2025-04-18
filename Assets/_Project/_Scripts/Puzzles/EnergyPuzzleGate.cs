using UnityEngine;

public class EnergyPuzzleGate : MonoBehaviour
{
    public EnergyType requiredEnergy;
    public GameObject gateObject;

    public void TryActivate(CompanionController companion)
    {
        if (companion.GetEnergyType() == requiredEnergy)
        {
            Debug.Log("Gate unlocked!");
            gateObject.SetActive(false);
        }
        else
        {
            Debug.Log("Incorrect energy. Gate remains locked.");
        }
    }
}