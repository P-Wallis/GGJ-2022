using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    // Animation Parameters
    const string WALK_FORWARD = "WalkForward";
    const string WALK_RIGHT = "WalkRight";
    const string ATTACK = "Attack";

    public static Player _;
    private void Awake()
    {
        if (_ == null)
            _ = this;
    }

    public enum Weapon { FIRE, ICE }

    public float speed = 1;
    public float attackRotationCorrection = 30;
    public float attackIntroSpeed = 10;
    public float walkBlendSpeed = 10;
    public Transform model;

    public int highscoreCount = 10;

    public ParticleSystem fireParticles, iceParticles;
    public Transform raycastPoint;
    public Transform cubeCreatePoint;

    public float maxFireDistance;

    public GameObject gemPickupSFXPrefab;

    public GameObject[] gemPrefabs;

    public GameObject startPanel;
    public TextMeshProUGUI startText;
    public float timerLength = 90f;
    public TextMeshProUGUI timerText;
    public GameObject endPanel;
    public TextMeshProUGUI endScoreText;
    public TextMeshProUGUI endHighScoreListText;
    public Image weaponPanel;
    public TextMeshProUGUI weaponText;

    private Animator m_animator;
    private Camera m_camera;
    private Rigidbody m_rigidbody;
    private Plane m_groundPlane;
    private LayerMask iceLayerMask;
    private Weapon currentWeapon;
    private float timer;
    IEnumerator Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_camera = Camera.main;
        m_groundPlane = new Plane(Vector3.up, 0);
        iceLayerMask = LayerMask.GetMask("Ice");
        m_animator = GetComponentInChildren<Animator>();
        m_animator.SetLayerWeight(1, 0);

        endPanel.SetActive(false);
        startPanel.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            startText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        startText.text = "Go!";
        yield return new WaitForSeconds(1);
        startPanel.SetActive(false);
        SetWeapon(Weapon.FIRE);
        playing = true;

        timer = timerLength;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = (Mathf.FloorToInt(timer / 60) > 0 ? Mathf.FloorToInt(timer / 60) + " minute " : "") + Mathf.FloorToInt(timer % 60) + " seconds";
            yield return null;
        }

        playing = false;
        endPanel.SetActive(true);
        ScoreData scoreData = ScoreSaver.LoadScoreData();
        ScoreDatum currentScore = new ScoreDatum();
        currentScore.gemCount = GemCounter._.Count;
        currentScore.playIndex = ++scoreData.lastPlay;
        currentScore.name = scoreData.lastName;
        scoreData.data.Add(currentScore);
        scoreData.data.Sort((ScoreDatum x, ScoreDatum y) => { return y.gemCount - x.gemCount; });
        string highscores = "";
        ScoreDatum sd;
        for(int i=0; i<scoreData.data.Count && i<highscoreCount; i++)
        {
            sd = scoreData.data[i];
            highscores += "Play " + sd.playIndex + ", Gems: " + sd.gemCount + "\n";
        }
        endHighScoreListText.text = highscores;
        endScoreText.text = ("Current: Play " + currentScore.playIndex + ", Gems: " + currentScore.gemCount);
        ScoreSaver.SaveScoreData(scoreData);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gem")
        {
            GemCounter._.IncrementCount();
            Instantiate(gemPickupSFXPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
    }

    bool playing = false;

    Vector3 movement;
    float attackWeight = 0;
    float attackWeightTarget = 0;
    Vector2 walkAnimBlend = Vector2.zero;
    private void Update()
    {
        if (!playing)
        {
            return;
        }

        // Weapons
        switch (currentWeapon)
        {
            case Weapon.FIRE:
                if (Input.GetMouseButtonDown(0))
                {
                    fireParticles.Play();
                    attackWeightTarget = 1;
                }

                if (Input.GetMouseButton(0))
                {
                    Burn();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    fireParticles.Stop();
                    attackWeightTarget = 0;
                }
                break;
            case Weapon.ICE:
                if (Input.GetMouseButtonDown(0))
                {
                    iceParticles.Play();
                    attackWeightTarget = 1;
                }

                if (Input.GetMouseButton(0))
                {
                    Map._.CreateCubeAtWorldPosition(cubeCreatePoint.position);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    iceParticles.Stop();
                    attackWeightTarget = 0;
                }
                break;
        }

        // Switch Weapon
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetWeapon(currentWeapon == Weapon.FIRE ? Weapon.ICE : Weapon.FIRE);
        }
        attackWeight = Mathf.Lerp(attackWeight, attackWeightTarget, Time.deltaTime * attackIntroSpeed);
        m_animator.SetLayerWeight(1, attackWeight);


        // Rotation
        Vector3 cursorPoint = ScreenToGroundPoint(Input.mousePosition);

        float angle = Mathf.Atan2(cursorPoint.x - transform.position.x, cursorPoint.z - transform.position.z) * Mathf.Rad2Deg;
        float correction = attackRotationCorrection * attackWeight;
        model.transform.rotation = Quaternion.Euler(0, angle + correction, 0);


        // Movement
        movement = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
            ).normalized;

        Vector3 lookDir = cursorPoint - transform.position;
        lookDir.y = 0;
        float walkTowardLook = Vector3.Dot(lookDir, movement);
        float walkOrthoganal = Vector3.Dot(lookDir, new Vector3(movement.z, 0, -movement.x));
        walkAnimBlend = Vector2.Lerp(walkAnimBlend, new Vector2(walkOrthoganal, walkTowardLook), walkBlendSpeed * Time.deltaTime);
        m_animator.SetFloat(WALK_RIGHT, walkAnimBlend.x);
        m_animator.SetFloat(WALK_FORWARD, walkAnimBlend.y);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        weaponPanel.color = weapon == Weapon.FIRE ? new Color(1, 0.7f, 0.7f) : new Color(0.7f, 0.8f, 1);
        weaponText.text = "Current Element: " + weapon.ToString();

        if (fireParticles.isPlaying)
            fireParticles.Stop();
        if (iceParticles.isPlaying)
            iceParticles.Stop();

        if (Input.GetMouseButton(0))
        {
            switch (weapon)
            {
                case Weapon.FIRE:
                    fireParticles.Play();
                    break;
                case Weapon.ICE:
                    iceParticles.Play();
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        if (!exploding)
        {
            m_rigidbody.velocity = movement * speed;
        }
        else
        {
            m_rigidbody.AddForce(explodeDir.normalized * 100);
        }
    }

    private Vector3 ScreenToGroundPoint(Vector2 screenPos)
    {
        Ray ray = m_camera.ScreenPointToRay(screenPos);

        float enter = 0.0f;
        if (m_groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero;
    }

    void Burn()
    {
        RaycastHit hit;
        if(Physics.Raycast(raycastPoint.position, raycastPoint.forward, out hit, maxFireDistance, iceLayerMask))
        {
            GameObject hitObject = hit.collider != null ? hit.collider.gameObject : null;
            IceCube iceCube = hitObject !=null? hitObject.GetComponent<IceCube>() : null;

            if(iceCube !=null)
            {
                iceCube.Burn();
            }
        }
    }

    private bool exploding = false;
    private Vector3 explodeDir;
    public void Explode(Vector3 explosionPos)
    {
        exploding = true;
        explodeDir = (transform.position - explosionPos);
        if (GemCounter._.Count > 0)
        {
            int half = Mathf.CeilToInt(GemCounter._.Count / 2f);
            GemCounter._.SetCount(GemCounter._.Count - half);

            int quarter = Mathf.CeilToInt(GemCounter._.Count / 2f);
            for (int i=0; i<quarter; i++)
            {
                Vector3 vector = Random.insideUnitSphere;
                GameObject go = Instantiate(gemPrefabs[Random.Range(0, gemPrefabs.Length)], transform.position + vector, Quaternion.identity);
                Rigidbody rb = go.AddComponent<Rigidbody>();
                Collider cl = go.GetComponent<Collider>();

                cl.isTrigger = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.AddForce(vector * 10);

                StartCoroutine(CleanUpGemThrow(rb, cl));
            }
        }
        StartCoroutine(CleanUpExlode());
    }

    IEnumerator CleanUpExlode()
    {
        yield return new WaitForSeconds(0.7f);
        exploding = false;
    }

    IEnumerator CleanUpGemThrow(Rigidbody rb, Collider cl)
    {
        yield return new WaitForSeconds(0.7f);

        cl.isTrigger = true;
        Destroy(rb);
    }
}
