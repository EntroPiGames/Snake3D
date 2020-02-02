using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace EntroPi
{
    public static class EntroPiAttributesUtil
    {
        public static bool CheckRequiredReference(UnityEngine.Object target, FieldInfo field, RequiredReferenceAttribute attribute)
        {
            bool isReferenceAssigned = false;

            Type fieldType = field.FieldType;
            bool isObjectReference = IsSameOrSubclassOfType<UnityEngine.Object>(fieldType);

            Debug.AssertFormat(isObjectReference, target, "[RequiredReference] Attribute is assigned to variable with incorrect type.\nType {0} is not a Unity Object reference.", fieldType);

            if (isObjectReference)
            {
                object fieldValue = field.GetValue(target);
                isReferenceAssigned = CheckUnityObjectReference(fieldValue);

                Debug.AssertFormat(isReferenceAssigned, target, "[RequiredReference]\nThe required reference for {0} is not assigned in script {1}", field.Name, target.GetType());
            }

            return isReferenceAssigned;
        }

        public static bool CheckRequiredReferenceList(UnityEngine.Object target, FieldInfo field, RequiredReferenceListAttribute attribute)
        {
            bool isReferenceAssigned = false;

            Type fieldType = field.FieldType;
            bool isFieldGenericList = EntroPiAttributesUtil.IsTypeofGenericList(fieldType);

            Debug.AssertFormat(isFieldGenericList, target, "[RequiredReferenceList] Attribute is assigned to variable with incorrect type.\nVariable {0} in {1} is not a Generic List.", field.Name, target.GetType());

            if (isFieldGenericList)
            {
                Type itemType = fieldType.GetGenericArguments()[0];
                bool isItemTypeObject = EntroPiAttributesUtil.IsSameOrSubclassOfType<UnityEngine.Object>(itemType);

                Debug.AssertFormat(isItemTypeObject, target, "[RequiredReferenceList] Attribute is assigned to List<> with incorrect type.\nType {0} is not a Unity Object.", itemType);

                if (isItemTypeObject)
                {
                    IList fieldList = field.GetValue(target) as IList;

                    bool isListInitialized = fieldList != null;

                    Debug.AssertFormat(isListInitialized, target, "[RequiredReferenceList]\nList<> variable {0} in {1} is not initialized.", field.Name, target.GetType());

                    if (isListInitialized)
                    {
                        bool areAllListReferencesAssigned = true;

                        for (int i = 0; i < fieldList.Count; ++i)
                        {
                            areAllListReferencesAssigned &= EntroPiAttributesUtil.CheckUnityObjectReference(fieldList[i]);
                        }

                        Debug.AssertFormat(areAllListReferencesAssigned, target, "[RequiredReferenceList]\nList<> variable {0} in {1} contains unassigned references.", field.Name, target.GetType());

                        if (areAllListReferencesAssigned)
                        {
                            bool isListCountInRange = EntroPiAttributesUtil.CheckListCount(fieldList.Count, attribute.CountMin, attribute.CountMax);

                            Debug.AssertFormat(isListCountInRange, target, "[RequiredReferenceList]\nInvalid list item count for variable {0} in {1} ", field.Name, target.GetType());

                            isReferenceAssigned = isListCountInRange;
                        }
                    }
                }
            }

            return isReferenceAssigned;
        }

        public static bool AssignRequiredComponentReference(EntroPiBehaviour target, FieldInfo field, RequiredComponentAttribute attribute)
        {
            bool isReferenceAssigned = false;

            Type fieldType = field.FieldType;

            Component requiredComponent = null;

            switch (attribute.Relation)
            {
                case ComponentRelation.Self:
                    requiredComponent = target.GetComponent(fieldType);
                    break;

                case ComponentRelation.Children:
                    requiredComponent = target.GetComponentInChildren(fieldType, attribute.IncludeInactive);
                    break;

                case ComponentRelation.Parent:
                    requiredComponent = target.GetComponentInParent(fieldType);
                    break;

                default:
                    Debug.LogError("[RequiredComponent]\nInvalid component relation.");
                    break;
            }

            isReferenceAssigned = requiredComponent != null;

            Debug.AssertFormat(isReferenceAssigned, target, "[RequiredComponent]\nFailed to get component for variable {1} (Type: {0}) in script {2}", field.FieldType, field.Name, target.GetType());

            field.SetValue(target, requiredComponent);

            return isReferenceAssigned;
        }

        public static bool AssignRequiredComponentListReference(EntroPiBehaviour target, FieldInfo field, RequiredComponentListAttribute attribute)
        {
            bool isReferenceAssigned = false;

            Type fieldType = field.FieldType;
            bool isFieldGenericList = EntroPiAttributesUtil.IsTypeofGenericList(fieldType);

            Debug.AssertFormat(isFieldGenericList, target, "[RequiredComponentList] Attribute is assigned to variable with incorrect type.\nVariable {0} in {1} is not a Generic List.", field.Name, target.GetType());

            if (isFieldGenericList)
            {
                Type itemType = fieldType.GetGenericArguments()[0];

                IList fieldList = field.GetValue(target) as IList;

                if (fieldList == null)
                {
                    Type listType = typeof(List<>).MakeGenericType(itemType);
                    fieldList = Activator.CreateInstance(listType) as IList;
                    field.SetValue(target, fieldList);
                }

                Component[] requiredComponents = null;

                switch (attribute.Relation)
                {
                    case ComponentRelation.Self:
                        requiredComponents = target.GetComponents(itemType);
                        break;

                    case ComponentRelation.Children:
                        requiredComponents = target.GetComponentsInChildren(itemType, attribute.IncludeInactive);
                        break;

                    case ComponentRelation.Parent:
                        requiredComponents = target.GetComponentsInParent(itemType);
                        break;

                    default:
                        Debug.LogError("[RequiredComponentList]\nInvalid component relation.");
                        break;
                }

                foreach (Component component in requiredComponents)
                {
                    fieldList.Add(component);
                }

                bool isListCountInRange = EntroPiAttributesUtil.CheckListCount(fieldList.Count, attribute.CountMin, attribute.CountMax);

                Debug.AssertFormat(isListCountInRange, target, "[RequiredComponentList]\nInvalid list item count for variable {0} in {1} ", field.Name, target.GetType());

                isReferenceAssigned = isListCountInRange;
            }

            return isReferenceAssigned;
        }

        public static bool AssignRequiredSceneObjectReference(EntroPiBehaviour target, FieldInfo field, RequiredSceneObjectAttribute attribute)
        {
            bool isReferenceAssigned = false;

            Type fieldType = field.FieldType;
            bool isObjectReference = EntroPiAttributesUtil.IsSameOrSubclassOfType<UnityEngine.Object>(fieldType);

            Debug.AssertFormat(isObjectReference, target, "[RequiredSceneObject] Attribute is assigned to variable with incorrect type.\nType {0} is not a Unity Object reference.", fieldType);

            if (isObjectReference)
            {
                UnityEngine.Object fieldObject = UnityEngine.Object.FindObjectOfType(field.FieldType);

                isReferenceAssigned = fieldObject != null;

                Debug.AssertFormat(isReferenceAssigned, target, "[RequiredSceneObject]\nFailed to find object of type {0} for variable {1} in script {2}", field.FieldType, field.Name, target.GetType());

                field.SetValue(target, fieldObject);
            }

            return isReferenceAssigned;
        }

        public static bool CheckUnityObjectReference(object value)
        {
            bool isReferenceAssigned = false;

            if (value != null && value is UnityEngine.Object)
            {
                UnityEngine.Object fieldObject = value as UnityEngine.Object;

                if (fieldObject != null)
                {
                    int instanceID = fieldObject.GetInstanceID();
                    isReferenceAssigned = instanceID != 0;
                }
            }

            return isReferenceAssigned;
        }

        public static bool CheckListCount(int count, int listCountMin, int listCountMax)
        {
            bool isListCountInRange = count >= listCountMin;

            if (listCountMax >= 0)
            {
                isListCountInRange = count <= listCountMax;
            }

            return isListCountInRange;
        }

        public static bool IsTypeofGenericList(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>));
        }

        public static bool IsSameOrSubclassOfType<T>(Type type)
        {
            Type genericType = typeof(T);
            return type.IsSubclassOf(genericType) || type == genericType;
        }
    }
}