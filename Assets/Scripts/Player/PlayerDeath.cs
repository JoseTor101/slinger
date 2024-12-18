using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public Material dissolveMaterial; 
    [SerializeField] public float dissolveSpeed = 1.8f;  
    private bool isDissolving = false; 
    public float dissolveAmount = 0.1f;

    void Start()
    {
        dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
    }
    
    void Update()
    {
        if (isDissolving)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);

            if (dissolveAmount >= 1f)
            {

                //Debug.Log("Dissolve Ammount: " + dissolveAmount);
                // Aquí puedes hacer que el jugador desaparezca o sea destruido
                gameObject.SetActive(false);
            }
        }
    }

    public void restartDeath()
    {
        dissolveAmount = 0.1f;
        isDissolving = false;
        gameObject.SetActive(true);
        dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
    }

    // Método para activar la disolución cuando el jugador muere
    public void OnPlayerDeath()
    {
        isDissolving = true;
    }
}