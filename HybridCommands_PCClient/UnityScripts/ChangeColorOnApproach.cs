using UnityEngine;
using UnityEngine.UI;

public class ChangeColorOnApproach : MonoBehaviour
{
    public Transform playerTransform;
    public float distanceThreshold;
    public float distanceThreshold2;
    public Color normalColor;
    public Color approachColor;
    public Color approachColor2;
    public int scene;

    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.color = normalColor;
    }

    void Update()
    {
        float distanceToPlayer = 8-(Vector3.Distance(transform.position, new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z * (scene == 1 ? -1 : 1))));

        if (distanceToPlayer < distanceThreshold && distanceToPlayer>=distanceThreshold2){
            buttonImage.color = approachColor;}
        else if(distanceToPlayer < distanceThreshold2){
            buttonImage.color = approachColor2;
            float scaleFactor = Mathf.Lerp(1.0f, 1.5f, distanceToPlayer / distanceThreshold2);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else{
            buttonImage.color = normalColor;
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);}
    }
}