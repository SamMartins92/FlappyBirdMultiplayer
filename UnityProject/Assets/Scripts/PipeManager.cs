using UnityEngine;

public class PipeManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private float speed;
    [SerializeField] private float height;
    #endregion

    void Update()
    {
        movePipe();
    }
    private void movePipe()
    {
        foreach (GameObject pipeGroup in GameObject.FindGameObjectsWithTag("pipeGroup"))
        {
            //make the pipe move left
            pipeGroup.transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }
}
