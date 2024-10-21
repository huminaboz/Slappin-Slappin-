using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Pickup : MonoBehaviour, IObjectPool<Pickup>
{
    [SerializeField] private int currency1 = 1;

    // [SerializeField] private int currency2 = 0;
    [FormerlySerializedAs("hp")] [SerializeField] private float hpPercentageRefill = .1f;

    private Collider _collider;
    [HideInInspector] public Hover hover;

    private float speed;
    private bool playerIsAbsorbing = false;
    private Vector3 goalPosition;
    [SerializeField] private float pickupZoneZOffset = 1f;
    [SerializeField] private float pickupZoneYOffset = 1f;

    public void SetupObjectFirstTime()
    {
        _collider = GetComponent<Collider>();
        hover = GetComponent<Hover>();
        gameObject.SetActive(false);
    }

    public void InitializeObjectFromPool()
    {
        _collider.enabled = false;
        gameObject.SetActive(true);
        //StartCoroutine(WaitToEnableCollider());
        //NOTE: Have to call "SetNewPosition" separate because hover takes over the position
        StateAbsorbState.OnAbsorbPressed += PlayerStartedAbsorbing;
        StateAbsorbState.OnAbsorbReleased += PlayerStoppedAbsorbing;
    }

    public void SetNewHoverPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        hover.SetOriginPosition();
        hover.enabled = true; //This is dangerous because it takes over the position.
    }

    private IEnumerator WaitToEnableCollider()
    {
        yield return new WaitForSeconds(.5f);
        _collider.enabled = true;
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.GetComponent<IPickerUpper>() is not null)
    //     {
    //         PlayerStartedAbsorbing();
    //     }
    // }

    public void SetupCurrency(int value)
    {
        currency1 = value;
        //TODO:: Make provisions for currency2
        //TODO:: Make it fancier with higher values
    }

    private void PlayerStartedAbsorbing()
    {
        _collider.enabled = false;
        hover.enabled = false;

        speed = 0.6f * StatLiason.I.Get(Stat.AbsorbSpeed);
        goalPosition = Camera.main.transform.position;
        playerIsAbsorbing = true;
    }

    private void PlayerStoppedAbsorbing()
    {
        _collider.enabled = true;
        hover.enabled = true;
        playerIsAbsorbing = false;
        SetNewHoverPosition(transform.position);
    }

    private void Update()
    {
        if (!playerIsAbsorbing) return;


        // Get the direction from the current position to the target position
        Vector3 direction = goalPosition - transform.position;
        direction.y = 0f + pickupZoneYOffset; //Manually set how high up it goes

        // Move the object towards the target
        Vector3 newPosition = transform.position + direction.normalized * speed * Time.deltaTime;
        transform.position = newPosition;

        if (transform.localPosition.z <= goalPosition.z + pickupZoneZOffset)
        {
            if (hpPercentageRefill > 0)
            {
                PlayerInfo.I.health.AdjustHp(
                    (int)(PlayerInfo.I.health.maxHp * hpPercentageRefill),
                    gameObject);
                SFXPlayer.I.Play(AudioEventsStorage.I.healthPickup);
            }
            else
            {
                SFXPlayer.I.Play(AudioEventsStorage.I.pickedUpCurrency1);
            }

            PlayerStats.I.AddCurency(currency1);
            ReturnObjectToPool();
        }
    }


    //Might still use this if I decide to let the collider of an attack pick up stuff
    // private IEnumerator MoveTowardsCamera()
    // {
    //     float speed = 2f;
    //     Vector3 goalPosition = Camera.main.transform.position;
    //
    //     while (transform.localPosition.z > goalPosition.z)
    //     {
    //         // Get the direction from the current position to the target position
    //         Vector3 direction = goalPosition - transform.position;
    //         direction.y = 0f; // Ignore the Y axis for movement
    //
    //         // Move the object towards the target
    //         Vector3 newPosition = transform.position + direction.normalized * speed * Time.deltaTime;
    //         transform.position = newPosition;
    //         yield return null;
    //     }
    //
    //     PlayerStats.I.AddCurency(currency1);
    //     ReturnObjectToPool();
    // }


    public void ReturnObjectToPool()
    {
        StateAbsorbState.OnAbsorbPressed -= PlayerStartedAbsorbing;
        StateAbsorbState.OnAbsorbReleased -= PlayerStoppedAbsorbing;
        // StopAllCoroutines();
        ObjectPoolManager<Pickup>.ReturnObject(this);
    }
}