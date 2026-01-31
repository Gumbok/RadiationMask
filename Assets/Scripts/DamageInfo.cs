using System;
using UnityEngine;

    /// <summary>
    /// Container that describes the context for a damage instance.
    /// </summary>
    [Serializable]
    public struct DamageInfo
    {
        [SerializeField] private int amount;
        [SerializeField] private GameObject source;
        [SerializeField] private bool hasSourceTeam;
        [SerializeField] private int sourceTeam;
        [SerializeField] private bool hasImpulse;
        [SerializeField] private Vector3 impulse;
        [SerializeField] private ForceMode impulseMode;
        [SerializeField] private bool hasImpulsePoint;
        [SerializeField] private Vector3 impulsePoint;

        /// <summary>
        /// The amount of damage to apply.
        /// </summary>
        public int Amount => amount;

        /// <summary>
        /// Optional reference to the source GameObject.
        /// </summary>
        public GameObject Source => source;

        /// <summary>
        /// Optional team the damage originates from.
        /// </summary>
        public int? SourceTeam => hasSourceTeam ? sourceTeam : (int?)null;

        /// <summary>
        /// Returns true if this damage carries an impulse.
        /// </summary>
        public bool HasImpulse => hasImpulse;

        /// <summary>
        /// World-space impulse to apply when <see cref="HasImpulse"/> is true.
        /// </summary>
        public Vector3 Impulse => impulse;

        /// <summary>
        /// Force mode to use when applying <see cref="Impulse"/>.
        /// </summary>
        public ForceMode ImpulseMode => impulseMode;

        /// <summary>
        /// Optional world-space position to apply the impulse at.
        /// </summary>
        public bool HasImpulsePoint => hasImpulsePoint;

        public Vector3 ImpulsePoint => impulsePoint;

        public DamageInfo(int amount, GameObject source = null, int? sourceTeam = null)
        {
            this.amount = amount;
            this.source = source;
            if (sourceTeam.HasValue)
            {
                hasSourceTeam = true;
                this.sourceTeam = sourceTeam.Value;
            }
            else
            {
                hasSourceTeam = false;
                this.sourceTeam = default;
            }
            hasImpulse = false;
            impulse = Vector3.zero;
            impulseMode = ForceMode.Impulse;
            hasImpulsePoint = false;
            impulsePoint = Vector3.zero;
        }

        public DamageInfo(int amount, Component source, int? sourceTeam = null)
            : this(amount, source != null ? source.gameObject : null, sourceTeam)
        {
        }

        public DamageInfo WithAmount(int newAmount)
        {
            DamageInfo copy = this;
            copy.amount = newAmount;
            return copy;
        }

        public DamageInfo WithSource(GameObject newSource)
        {
            DamageInfo copy = this;
            copy.source = newSource;
            return copy;
        }

        public DamageInfo WithSource(Component newSource)
        {
            return WithSource(newSource != null ? newSource.gameObject : null);
        }

        public DamageInfo WithTeam(int? newTeam)
        {
            DamageInfo copy = this;
            if (newTeam.HasValue)
            {
                copy.hasSourceTeam = true;
                copy.sourceTeam = newTeam.Value;
            }
            else
            {
                copy.hasSourceTeam = false;
                copy.sourceTeam = default;
            }

            return copy;
        }

        public DamageInfo WithImpulse(Vector3 newImpulse, ForceMode mode = ForceMode.Impulse, Vector3? atPoint = null)
        {
            DamageInfo copy = this;
            copy.hasImpulse = true;
            copy.impulse = newImpulse;
            copy.impulseMode = mode;
            copy.hasImpulsePoint = atPoint.HasValue;
            copy.impulsePoint = atPoint ?? Vector3.zero;
            return copy;
        }

        public DamageInfo WithoutImpulse()
        {
            DamageInfo copy = this;
            copy.hasImpulse = false;
            copy.impulse = Vector3.zero;
            copy.impulseMode = ForceMode.Impulse;
            copy.hasImpulsePoint = false;
            copy.impulsePoint = Vector3.zero;
            return copy;
        }

        public bool TryGetImpulse(out Vector3 impulseValue, out Vector3? atPoint, out ForceMode mode)
        {
            if (hasImpulse)
            {
                impulseValue = impulse;
                mode = impulseMode;
                atPoint = hasImpulsePoint ? impulsePoint : (Vector3?)null;
                return true;
            }

            impulseValue = Vector3.zero;
            mode = ForceMode.Impulse;
            atPoint = null;
            return false;
        }
    }
