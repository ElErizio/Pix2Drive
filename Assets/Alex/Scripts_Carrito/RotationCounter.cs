using UnityEngine;

public class RotationCounter : MonoBehaviour
{
    // Contador de vueltas
    public int vueltas = 0;

    // Rotaci�n previa
    private float rotacionPrevia;

    // Umbral para detectar una vuelta completa
    private const float umbral = 178;

    void Start()
    {
        // Inicializa la rotaci�n previa con la rotaci�n inicial del objeto
        rotacionPrevia = transform.eulerAngles.y;
    }

    void Update()
    {
        // Llama a la funci�n para contar vueltas
        ContarVueltas();
    }

    private void ContarVueltas()
    {
        // Obtener la rotaci�n actual en el eje Y
        float rotacionActual = transform.eulerAngles.y;

        // Calcular la diferencia de rotaci�n desde el �ltimo cuadro
        float diferenciaRotacion = rotacionActual - rotacionPrevia;

        // Ajustar la diferencia de rotaci�n si se cruza el l�mite de 360 grados
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
        // Opcional: Mostrar el n�mero de vueltas en la consola
        //Debug.Log("Vueltas: " + vueltas);
    }

    // Funci�n para obtener el n�mero de vueltas
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
