using UnityEngine;

public static class ParticleSystemUtils
{
    public static void SetParticleParameter(GameObject gameObject, string childObjectName, string grandchildObjectName, string parameterName, object value)
    {
        Transform childTransform = gameObject.transform.Find(childObjectName);
        if (childTransform == null)
        {
            Debug.LogWarning("Child object '" + childObjectName + "' not found!");
            return;
        }

        Transform grandchildTransform = childTransform.Find(grandchildObjectName);
        if (grandchildTransform == null)
        {
            Debug.LogWarning("Grandchild object '" + grandchildObjectName + "' not found!");
            return;
        }

        ParticleSystem particleSystem = grandchildTransform.GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogWarning("Particle system component not found in grandchild object '" + grandchildObjectName + "'!");
            return;
        }

        particleSystem.Pause();

        var mainModule = particleSystem.main;

        // Set the parameter value based on its type
        if (parameterName == "startColor")
        {
            mainModule.startColor = (Color)value;
        }
        else if (parameterName == "startSize")
        {
            mainModule.startSize = (float)value;
        }
        else if (parameterName == "startLifetime")
        {
            mainModule.startLifetime = (float)value;
        }
        // Add more conditions to handle other particle system parameters

        // Print a debug message with the updated parameter value
        Debug.Log("Set particle parameter '" + parameterName + "' to " + value.ToString());

        // Apply the changes to the particle system
        particleSystem.Play();
    }
}
