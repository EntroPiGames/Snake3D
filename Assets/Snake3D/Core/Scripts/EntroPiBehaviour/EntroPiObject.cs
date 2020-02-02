using System;
using System.Reflection;
using UnityEngine;

namespace EntroPi
{
    public class EntroPiObject : ScriptableObject
    {
        private bool m_AreAllRequiredReferencesAssigned = false;

        public bool IsAwake { get { return m_AreAllRequiredReferencesAssigned; } }

        // Public so that a warning is thrown when implementing the same function in a derived classes.
        public void OnEnable()
        {
            m_AreAllRequiredReferencesAssigned = AssignReferences();

            if (m_AreAllRequiredReferencesAssigned)
            {
                m_AreAllRequiredReferencesAssigned &= OnAssignAdditionalReferences();
            }

            Debug.AssertFormat(m_AreAllRequiredReferencesAssigned == true, this, "[EntroPiObject] Failed to assign all required references on object of type {0}", this.GetType());

            if (m_AreAllRequiredReferencesAssigned)
            {
                OnInitialize();
            }
        }

        // Public so that a warning is thrown when implementing the same function in a derived classes.
        public void OnDisable()
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
        {
        }

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
                        areAllReferencesAssigned &= false;
                        Debug.LogError("[EntroPiObject] RequiredComponentAttribute attribute is not supported");
                    }
                    else if (attribute is RequiredComponentListAttribute)
                    {
                        areAllReferencesAssigned &= false;
                        Debug.LogError("[EntroPiObject] RequiredComponentListAttribute attribute is not supported");
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
                        areAllReferencesAssigned &= false;
                        Debug.LogError("[EntroPiObject] RequiredSceneObjectAttribute attribute is not supported");
                    }
                }
            }

            return areAllReferencesAssigned;
        }

        #endregion Private Functions
    }
}