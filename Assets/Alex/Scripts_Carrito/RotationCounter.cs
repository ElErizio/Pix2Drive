using UnityEngine;

public class RotationCounter : MonoBehaviour
{
    // Contador de vueltas
    public int vueltas = 0;

    // Rotación previa
    private float rotacionPrevia;

    // Umbral para detectar una vuelta completa
    private const float umbral = 178;

    void Start()
    {
        // Inicializa la rotación previa con la rotación inicial del objeto
        rotacionPrevia = transform.eulerAngles.y;
    }

    void Update()
    {
        // Llama a la función para contar vueltas
        ContarVueltas();
    }

    private void ContarVueltas()
    {
        // Obtener la rotación actual en el eje Y
        float rotacionActual = transform.eulerAngles.y;

        // Calcular la diferencia de rotación desde el último cuadro
        float diferenciaRotacion = rotacionActual - rotacionPrevia;

        // Ajustar la diferencia de rotación si se cruza el límite de 360 grados
        if (diferenciaRotacion > 180f)
        {
            diferenciaRotacion -= 360f;
        }
        else if (diferenciaRotacion < -180f)
        {
            diferenciaRotacion += 360f;
        }

        // Actualizar el contador de vueltas si se ha completado una vuelta
        if (Mathf.Abs(diferenciaRotacion) >= umbral)
        {
            vueltas += (int)(diferenciaRotacion / umbral);

            rotacionPrevia = rotacionActual;

        }
        // Opcional: Mostrar el número de vueltas en la consola
        //Debug.Log("Vueltas: " + vueltas);
    }

    // Función para obtener el número de vueltas
    public int ObtenerVueltas()
    {
        return vueltas/2;
    }

    public bool DioVuelta(int vueltasAnterior)
    {
        if(vueltas > vueltasAnterior)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
