using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TmDice25D
{
    [SerializeField]
    public class DiceRoll : MonoBehaviour
    {
        enum RollStarte{
            None=0,
            Roll,
            Stop,
        };
        [SerializeField] int m_pip=-1;
        [SerializeField] Vector2 rollVec = Vector2.one * 200f;
        public int pip { get { return m_pip; } }
        RollStarte state;
        Animator anm;
        Rigidbody2D rb;
        AudioSource ac;
        Vector3 defPos;
        bool toRoll;
        
        public enum sides
        {
            right,
            left
        }
        public sides side;
        private int roll;

        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _circleCollider2D;

        // Use this for initialization
        void Start()
        {
            anm = GetComponent<Animator>();
            ac = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _circleCollider2D = GetComponent<CircleCollider2D>();
            rb.isKinematic = true;
            defPos = transform.position;
            toRoll = false;
            _spriteRenderer.enabled = false;
            StartCoroutine(diceRollCo());
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ac.Play();
            if (rb.velocity.sqrMagnitude < 10f)
            {
                state = RollStarte.Stop;
            }
        }

        private void Update()
        {
            _circleCollider2D.isTrigger = !_spriteRenderer.enabled;
        }

        IEnumerator diceRollCo()
        {
            while (true)
            {
                rb.isKinematic = true;
                transform.position = defPos;
                state = RollStarte.Stop;
                m_pip = -1;
                Roll();
                while (!toRoll)
                {
                    yield return null;
                }
                state = RollStarte.Roll;
                rb.isKinematic = false;
                rb.velocity = Vector2.zero;
                float multiple = 1f;
                if (side == sides.right) multiple = -1;
                rb.AddForce(new Vector2(rollVec.x  * multiple, rollVec.y));

                Roll();

                while (state == RollStarte.Roll)
                {
                    yield return null;
                }

                SetPip(roll);
                
                toRoll = false;
                while (!toRoll)
                {
                    yield return null;
                }
            }
        }

        public void RollDice(int _roll)
        {
            roll = _roll;
            toRoll = true;
            _spriteRenderer.enabled = true;
        }

        /// <summary>
        /// Start roll animation
        /// </summary>
        public void Roll()
        {
            anm.Play("roll");
        }

        /// <summary>
        /// Set pip and start pip animation
        /// </summary>
        /// <param name="_pip">pip of dice</param>
        public void SetPip(int _pip)
        {
            m_pip = _pip;
            anm.Play("to" + m_pip.ToString());
        }

        public void EndShow()
        {
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;
        }
    }
}
