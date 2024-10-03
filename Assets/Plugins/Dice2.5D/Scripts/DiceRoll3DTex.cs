using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TmDice25D
{
    [SerializeField]
    public class DiceRoll3DTex : MonoBehaviour
    {
        public enum RollStarte{
            None=0,
            Roll,
            Stop,
        };
        [SerializeField] RollStarte m_state = RollStarte.None;
        [SerializeField,Range(1,6)] int m_pip=1;
        [SerializeField,Range(0.5f,5f)] float m_speedGain = 2f; 
        [SerializeField] bool m_isStep = false;
        [SerializeField] MeshRenderer m_mr = null;
        [SerializeField] AudioSource m_ac = null;
        [SerializeField] Vector2Int m_div = new Vector2Int(16, 8);
        RollStarte m_previousState = RollStarte.None;

        public int pip { get { return m_pip; } }
        public RollStarte state { get { return m_state; } }

        public bool used;
        public int roll;
        public SpriteRenderer sprite; 
        public Sprite[] sprites;

        void Start()
        {
            if (m_mr == null) m_mr = GetComponent<MeshRenderer>();
            m_state = m_previousState = RollStarte.None;
            m_mr.enabled = false;
            sprite.enabled = false;
        }

        void Update(){
            if(m_state!= m_previousState)
            {
                if(m_state== RollStarte.Roll)
                {
                    Roll();
                }
                else if (m_state == RollStarte.Stop)
                {
                    SetPip(m_pip); // random
                }

                m_previousState = m_state;
            }

            SetUsed();
        }

        void SetUsed()
        {
            Color color = sprite.color;

            if (used)
                color.a = 0.5f;
            else
                color.a = 1f;

            sprite.color = color;
        }

        /// <summary>
        /// Start roll animation
        /// </summary>
        public void Roll()
        {
            StopAllCoroutines();
            StartCoroutine(diceRollCo(0));
        }

        /// <summary>
        /// Set pip and start pip animation
        /// </summary>
        /// <param name="_pip">pip of dice</param>
        public void SetPip(int _pip)
        {
            m_pip = _pip;
            StopAllCoroutines();
            StartCoroutine(diceRollCo(m_pip));
        }

        public void RollDice(int _roll)
        {
            used = false;
            m_state = RollStarte.Roll;
            roll = _roll;
            m_mr.enabled = true;
            sprite.enabled = false;
        }

        public void StopDice()
        {
            m_state = RollStarte.Stop;
            m_pip = roll;
        }

        public void EndShow()
        {
            if (m_mr != null)
                m_mr.enabled = false;

            sprite.enabled = false;
        }
        
        public void SetRoll(int _roll)
        {
            roll = _roll;
            m_state = RollStarte.Stop;
            m_pip = _roll;
            m_mr.enabled = true;
        }

        IEnumerator diceRollCo(int _pip=0)
        {
            float animTime = 2f;
            float timer = animTime;
            int sttFrame = _pip * m_div.x;
            Vector4 vec = new Vector4(0, 0, 0, m_div.x* m_div.y);
            m_state = (_pip > 0) ? RollStarte.Stop : RollStarte.Roll;
            if (m_ac != null)
            {
                m_ac.PlayOneShot(m_ac.clip);
            }

            while (timer > 0)
            {
                timer = Mathf.Max(0f, timer - Time.deltaTime * m_speedGain);
                float rate = 1f - (timer/animTime);
                vec.z = (float)sttFrame + rate * (float)(m_div.x - 1);
                if (m_isStep)
                {
                    vec.z = Mathf.Floor(vec.z);
                }
                m_mr.material.SetVector("_Offset", vec);
                if (timer<=0)
                {
                    if (_pip > 0)
                    {
                        m_state = RollStarte.None;
                        if (m_ac != null)
                        {
                            m_ac.PlayOneShot(m_ac.clip);
                        }
                        //Debug.Log($"pip={this.pip}");
                        sprite.enabled = true;
                        m_mr.enabled = false;
                        sprite.sprite = sprites[roll - 1];
                    }
                    else
                    {
                        timer = animTime;
                    }
                }
                yield return null;
            }
        }
    }
}
