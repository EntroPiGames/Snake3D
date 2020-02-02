using System;
using System.Reflection;
using UnityEngine;

namespace EntroPi
{
    public class EntroPiBehaviour : MonoBehaviour
    {
        private bool m_AreAllRequiredReferencesAssigned = false;

        public bool AreReferencesAssigned { get { return m_AreAllRequiredReferencesAssigned; } }

        public bool CheckAndAssertReferencesAreAssigned()
        {
            Debug.Assert(m_AreAllRequiredReferencesAssigned == true, "[EntroPiBehaviour] Not all reference are assigned.", this);

            return m_AreAllRequiredReferencesAssigned;
        }

        protected virtual void Awake()
        {
            m_AreAllRequiredReferencesAssigned = AssignReferences();

            if (m_AreAllRequiredReferencesAssigned)
            {
                m_AreAllRequiredReferencesAssigned &= OnAssignAdditionalReferences();
            }

            Debug.AssertFormat(m_AreAllRequiredReferencesAssigned == true, this, "[EntroPiBehaviour] Failed to assign all required references.\nDisabling script component of type {0}", this.GetType());

            enabled &= m_AreAllRequiredReferencesAssigned;

            if (m_AreAllRequiredReferencesAssigned)
            {
                OnInitialize();
            }
        }

        protected virtual void OnDestroy()
        {
            if (m_AreAllRequiredReferencesAssigned)
            {
                OnTerminate();
            }

            m_AreAllRequiredReferencesAssigned = false;
        }

        protected virtual bool OnAssignAdditionalReferences()
        {
            return true;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnTerminate()
        { }

        #region Private Functions

        private bool AssignReferences()
        {
            bool areAllReferencesAssigned = true;

            Type behaviourType = this.GetType();
            FieldInfo[] fieldInfo = behaviourType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (FieldInfo field in fieldInfo)
            {
                object[] attributes = field.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    if (attribute is RequiredComponentAttribute)
                    {
                        areAllReferencesAssigned &= EntroPiAttributesUtil.AssignRequiredComponentReference(this, field, attribute as RequiredComponentAttribute);
                    }
                    else if (attribute is RequiredComponentListAttribute)
                    {
                        areAllReferencesAssigned &= EntroPiAttributesUtil.AssignRequiredComponentListReference(this, field, attribute as RequiredComponentListAttribute);
                    }
                    else if (attribute is RequiredReferenceAttribute)
                    {
                        areAllReferencesAssigned &= EntroPiAttributesUtil.CheckRequiredReference(this, field, attribute as RequiredReferenceAttribute);
                    }
                    else if (attribute is RequiredReferenceListAttribute)
                    {
                        areAllReferencesAssigned &= EntroPiAttributesUtil.CheckRequiredReferenceList(this, field, attribute as RequiredReferenceListAttribute);
                    }
                    else if (attribute is RequiredSceneObjectAttribute)
                    {
                        areAllReferencesAssigned &= EntroPiAttributesUtil.AssignRequiredSceneObjectReference(this, field, attribute as RequiredSceneObjectAttribute);
                    }
                }
            }

            return areAllReferencesAssigned;
        }

        #endregion Private Functions
    }
}