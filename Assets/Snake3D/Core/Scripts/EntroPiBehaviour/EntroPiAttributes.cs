using System;

namespace EntroPi
{
    public enum ComponentRelation { Self, Children, Parent }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredComponentAttribute : Attribute
    {
        private ComponentRelation m_Relation;
        private bool m_IncludeInactive = true;

        public ComponentRelation Relation { get { return m_Relation; } }

        public bool IncludeInactive
        {
            get { return m_IncludeInactive; }
            set { m_IncludeInactive = value; }
        }

        public RequiredComponentAttribute(ComponentRelation relation = ComponentRelation.Self)
        {
            m_Relation = relation;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredComponentListAttribute : Attribute
    {
        private ComponentRelation m_Relation;
        private int m_CountMin;
        private int m_CountMax;
        private bool m_IncludeInactive = true;

        public ComponentRelation Relation { get { return m_Relation; } }
        public int CountMin { get { return m_CountMin; } }
        public int CountMax { get { return m_CountMax; } }

        public bool IncludeInactive
        {
            get { return m_IncludeInactive; }
            set { m_IncludeInactive = value; }
        }

        public RequiredComponentListAttribute(uint countMin, ComponentRelation relation = ComponentRelation.Self)
        {
            m_CountMin = (int)countMin;
            m_CountMax = -1;
            m_Relation = relation;
        }

        public RequiredComponentListAttribute(uint countMin, uint countMax, ComponentRelation relation = ComponentRelation.Self)
        {
            m_CountMin = (int)countMin;
            m_CountMax = (int)countMax;
            m_Relation = relation;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredReferenceAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredReferenceListAttribute : Attribute
    {
        private int m_CountMin;
        private int m_CountMax;

        public int CountMin { get { return m_CountMin; } }
        public int CountMax { get { return m_CountMax; } }

        public RequiredReferenceListAttribute(uint countMin)
        {
            m_CountMin = (int)countMin;
            m_CountMax = -1;
        }

        public RequiredReferenceListAttribute(uint countMin, uint countMax)
        {
            m_CountMin = (int)countMin;
            m_CountMax = (int)countMax;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequiredSceneObjectAttribute : Attribute
    {
    }
}