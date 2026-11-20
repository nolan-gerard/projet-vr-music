using UnityEngine;

public class NeckPositionZone : MonoBehaviour
{
    public GuitarManager guitarManager;
    public int positionID = 1;
    public string handTag = "LeftHand";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(handTag) || (other.transform.parent != null && other.transform.parent.CompareTag(handTag)))
        {
            if (guitarManager != null)
                guitarManager.SetNeckPosition(positionID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(handTag) || (other.transform.parent != null && other.transform.parent.CompareTag(handTag)))
        {
            if (guitarManager != null)
                guitarManager.SetNeckPosition(0);
        }
    }
}