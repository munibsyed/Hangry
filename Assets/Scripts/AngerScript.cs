using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AngerScript : MonoBehaviour {

	// anger is a percantage between 0 and 1
	public float m_anger = 0.5f;
	// cooldown is a number of seconds that will pass before the anger begins to increase
	public float m_cooldown = 1f;
	private float m_cooldownTimer = 0f;
	// anger increase rate is the amount the anger meter will change per second
	public float m_angerIncreaseRate = 0.01f;
	// point at which the player will recieve the debuff for having 'max' anger
	public float m_angerMax = 0.9f;
	// point at which the player will recieve the buff for having 'min' anger
	public float m_angerMin = 0.1f;
	// if this is checked when anger is added, the anger cooldown will be reset
	public bool m_addAngerCooldown = false;
	// if this is checked when anger is subbed, the anger cooldown will be reset
	public bool m_subAngerCooldown = true;

    public int happyPointBuff;

	public Slider m_UISlider = null;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (GameManager.Instance.hasWon == false)
        {

            float deltaTime = Time.deltaTime;
            if (m_cooldownTimer > 0)
            {
                m_cooldownTimer -= deltaTime;
            }

            if (m_cooldownTimer <= 0)
            {
                if (m_cooldownTimer < 0)
                {
                    m_anger -= (m_cooldownTimer * m_angerIncreaseRate);
                    m_cooldownTimer = 0;
                }
                else
                {
                    m_anger += (deltaTime * m_angerIncreaseRate);
                }
            }

            if (m_anger > 1)
            {
                m_anger = 1;
            }

            if (m_anger < 0)
            {
                m_anger = 0;
            }

            if (m_anger > m_angerMax)
            {
                maxAngerUpdate();
            }
            else if (m_anger < m_angerMin)
            {
                minAngerUpdate();
            }

            if (m_UISlider != null)
            {
                m_UISlider.value = m_anger;
            }
        }
	}

	public void testGoodFoodPickup()
	{
		addAnger(-0.1f);
	}

	public void testBadFoodPickup()
	{
		addAnger(0.1f);
	}

	void minAngerUpdate()
	{
        GameManager.Instance.pointBuff += happyPointBuff;
	}

	void maxAngerUpdate()
	{

	}

	public void addAnger(float amount)
	{
		if ((m_subAngerCooldown && amount < 0) || (m_addAngerCooldown && amount > 0))
		{
			m_cooldownTimer = m_cooldown;
		}
		m_anger += amount;
	}
}
