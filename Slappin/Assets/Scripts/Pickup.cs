using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour, IObjectPool<Pickup>
{
    [SerializeField] public int currency1 = 1;
    [SerializeField] public int currency2 = 0;
    [SerializeField] public int hp = 0;

    private Collider _collider;
    private ArcToCamera _arcToCamera;
    private Hover hover;

    public void SetupObjectFirstTime()
    {
        _collider = GetComponent<Collider>();
        _arcToCamera = GetComponent<ArcToCamera>();
        hover = GetComponent<Hover>();
        gameObject.SetActive(false);
    }

    public void InitializeObjectFromPool()
    {
        _collider.enabled = false;
        hover.enabled = true;
        gameObject.SetActive(true);
        StartCoroutine(WaitToEnableCollider());
    }

    private IEnumerator WaitToEnableCollider()
    {
        yield return new WaitForSeconds(.5f);
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IPickerUpper>() is not null)
        {
            // IPickerUpper pickerUpper = other.GetComponent<IPickerUpper>();
            GetPickedUp();
        }
    }

    private void GetPickedUp()
    {
        //Todo:: Play a sfx
        _collider.enabled = false;
        hover.enabled = false;

        //TODO:: Fix this - or some other animation for feedback
        // _arcToCamera.FlyTowardsCamera();

        PlayerStats.I.AddCurency(currency1);
        PlayerInfo.I.health.AdjustHp(hp, gameObject);
        StartCoroutine(MoveTowardsCamera());
    }


    private IEnumerator MoveTowardsCamera()
    {
        float speed = 2f;
        Vector3 goalPosition = Camera.main.transform.position;

        while (transform.localPosition.z > goalPosition.z)
        {
            // Get the direction from the current position to the target position
            Vector3 direction = goalPosition - transform.position;
            direction.y = 0f; // Ignore the Y axis for movement

            // Move the object towards the target
            Vector3 newPosition = transform.position + direction.normalized * speed * Time.deltaTime;
            transform.position = newPosition;
            yield return null;
        }

        ReturnObjectToPool();
    }


    public void ReturnObjectToPool()
    {
        StopAllCoroutines();
        ObjectPoolManager<Pickup>.ReturnObject(this);
    }
}