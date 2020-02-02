using UnityEngine;
using UnityEngine.SceneManagement;

public static class ExtensionMethodsComponent
{
    #region GetOrAddComponent

    static public T GetOrAddComponent<T>(this Component currentComponent) where T : Component
    {
        T component = currentComponent.GetComponent<T>();

        if (component == null)
        {
            component = currentComponent.gameObject.AddComponent<T>();
        }

        Debug.Assert(component != null, "Something went horribly wrong!");

        return component;
    }

    static public T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
    {
        T component = behaviour.GetComponent<T>();

        if (component == null)
        {
            component = behaviour.gameObject.AddComponent<T>();
        }

        Debug.Assert(component != null, "Something went horribly wrong!");

        return component;
    }

    static public T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        Debug.Assert(component != null, "Something went horribly wrong!");

        return component;
    }

    #endregion GetOrAddComponent

    #region GetAndAssertComponent

    static public T GetAndAssertComponent<T>(this Component component)
    {
        T result = component.GetComponent<T>();

        Debug.AssertFormat(result != null, "Failed to get component: {0}", typeof(T).ToString(), component);

        return result;
    }

    static public T GetAndAssertComponent<T>(this MonoBehaviour behaviour)
    {
        T result = behaviour.GetComponent<T>();

        Debug.AssertFormat(result != null, "Failed to get component: {0}", typeof(T).ToString(), behaviour);

        return result;
    }

    static public T GetAndAssertComponent<T>(this GameObject gameObject)
    {
        T result = gameObject.GetComponent<T>();

        Debug.AssertFormat(result != null, "Failed to get component: {0}", typeof(T).ToString(), gameObject);

        return result;
    }

    #endregion GetAndAssertComponent

    #region GetAndAssertComponentInChildren

    static public T GetAndAssertComponentInChildren<T>(this Component component)
    {
        T result = component.GetComponentInChildren<T>();

        Debug.AssertFormat(result != null, "Failed to get component in children: {0}", typeof(T).ToString(), component);

        return result;
    }

    static public T GetAndAssertComponentInChildren<T>(this MonoBehaviour behaviour)
    {
        T result = behaviour.GetComponentInChildren<T>();

        Debug.AssertFormat(result != null, "Failed to get component in children: {0}", typeof(T).ToString(), behaviour);

        return result;
    }

    static public T GetAndAssertComponentInChildren<T>(this GameObject gameObject)
    {
        T result = gameObject.GetComponentInChildren<T>();

        Debug.AssertFormat(result != null, "Failed to get component in children: {0}", typeof(T).ToString(), gameObject);

        return result;
    }

    #endregion GetAndAssertComponentInChildren

    #region GetAndAssertComponentInParent

    static public T GetAndAssertComponentInParent<T>(this Component component)
    {
        T result = component.GetComponentInParent<T>();

        Debug.AssertFormat(result != null, "Failed to get component in parent: {0}", typeof(T).ToString(), component);

        return result;
    }

    static public T GetAndAssertComponentInParent<T>(this MonoBehaviour behaviour)
    {
        T result = behaviour.GetComponentInParent<T>();

        Debug.AssertFormat(result != null, "Failed to get component in parent: {0}", typeof(T).ToString(), behaviour);

        return result;
    }

    static public T GetAndAssertComponentInParent<T>(this GameObject gameObject)
    {
        T result = gameObject.GetComponentInParent<T>();

        Debug.AssertFormat(result != null, "Failed to get component in parent: {0}", typeof(T).ToString(), gameObject);

        return result;
    }

    #endregion GetAndAssertComponentInParent

    #region RequiredComponents

    /// <summary>
    /// Disables the Monobehaviour if the required component is not found.
    /// Should only be used in OnEnable functions!
    /// </summary>
    static public T GetRequiredComponent<T>(this MonoBehaviour behaviour)
    {
        T requiredComponent = behaviour.GetComponent<T>();
        bool componentFound = requiredComponent != null;

        Debug.AssertFormat(componentFound, "Failed to get required component of type \"{0}\"!\nDisabling \"{1}\" component in GameObject \"{2}\"", typeof(T).ToString(), behaviour.GetType(), behaviour.name, behaviour);

        behaviour.enabled &= componentFound;

        return requiredComponent;
    }

    /// <summary>
    /// Disables the Monobehaviour if the required component is not found in any of its children.
    /// Should only be used in OnEnable functions!
    /// </summary>
    static public T GetRequiredComponentInChildren<T>(this MonoBehaviour behaviour)
    {
        T requiredComponent = behaviour.GetComponentInChildren<T>();
        bool componentFound = requiredComponent != null;

        Debug.AssertFormat(componentFound, "Failed to get required component in children of type \"{0}\"!\nDisabling \"{1}\" component in GameObject \"{2}\"", typeof(T).ToString(), behaviour.GetType(), behaviour.name, behaviour);

        behaviour.enabled &= componentFound;

        return requiredComponent;
    }

    /// <summary>
    /// Disables the Monobehaviour if the required component is not found in the scene.
    /// Should only be used in OnEnable functions!
    /// </summary>
    static public T FindRequiredObjectOfType<T>(this MonoBehaviour behaviour) where T : Object
    {
        T requiredComponent = UnityEngine.Object.FindObjectOfType<T>();
        bool componentFound = requiredComponent != null;

        Debug.AssertFormat(componentFound, "Failed to find required object of type \"{0}\"!\nDisabling \"{1}\" component in GameObject \"{2}\"", typeof(T).ToString(), behaviour.GetType(), behaviour.name, behaviour);

        behaviour.enabled &= componentFound;

        return requiredComponent;
    }

    /// <summary>
    /// Disables the Monobehaviour if the object is not valid.
    /// Should only be used in OnEnable functions!
    /// </summary>
    static public void VerifyRequiredObject(this MonoBehaviour behaviour, Object currentObject)
    {
        bool isObjectValid = currentObject != null;

        Debug.AssertFormat(isObjectValid, "Invalid object!\nDisabling component \"{0}\" in GameObject \"{1}\"", behaviour.GetType(), behaviour.name, behaviour);

        behaviour.enabled &= isObjectValid;
    }

    #endregion RequiredComponents

    #region Scene

    static public T GetComponent<T>(this Scene scene)
    {
        T component = default(T);

        GameObject[] sceneRootObjects = scene.GetRootGameObjects();

        for (int i = 0; i < sceneRootObjects.Length; ++i)
        {
            component = sceneRootObjects[i].GetComponentInChildren<T>();

            if (component != null)
            {
                break;
            }
        }

        return component;
    }

    static public T GetAndAssertComponent<T>(this Scene scene)
    {
        T component = scene.GetComponent<T>();

        Debug.AssertFormat(component != null, "Failed to get component of type <{0}> in scene {1}", typeof(T).ToString(), scene.name);

        return component;
    }

    #endregion Scene
}