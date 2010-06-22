using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Bramble.Core
{
    /// <summary>
    /// Hierarchical property bag structure. Each PropertyBag is a dictionary of name/value pairs where each
    /// value can either be a string or another child PropertyBag. In addition, each PropertyBag may have a
    /// base PropertyBag that it will inherit (and can override) values from.
    /// </summary>
    public class PropertyBag : IEnumerable<PropertyBag>
    {
        public static PropertyBag FromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath, Encoding.ASCII);

            IEnumerable<string> included = PropertyBagParser.ParseIncludes(Path.GetDirectoryName(filePath), lines);
            IEnumerable<string> noComments = PropertyBagParser.StripComments(included);
            IEnumerable<string> noWhitespace = PropertyBagParser.StripEmptyLines(noComments);

            IndentationTree tree = IndentationTree.Parse(noWhitespace);

            return PropertyBagParser.Parse(tree);
        }

        public PropertyBag(string name, string value, IEnumerable<PropertyBag> baseProps)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            mName = name;
            mValue = value;

            if (baseProps != null)
            {
                mBases.AddRange(baseProps);
            }
        }

        public PropertyBag(string name, string value) : this(name, value, null) { }

        public PropertyBag(string name, IEnumerable<PropertyBag> baseProps) : this(name, String.Empty, baseProps) { }

        public PropertyBag(string name) : this(name, String.Empty) { }

        public bool Contains(string child)
        {
            return FlattenProperties.Contains(child);
        }

        #region IEnumerable<PropertyBag> Members

        public IEnumerator<PropertyBag> GetEnumerator()
        {
            return FlattenProperties.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public PropertyBag this[string name]
        {
            get
            {
                // try this level
                if (mChildren.Contains(name)) return mChildren[name];

                // recurse up the bases (in reverse order so that later items override previous ones)
                for (int i = mBases.Count - 1; i >= 0; i--)
                {
                    PropertyBag baseProp = mBases[i];
                    if (baseProp.Contains(name)) return baseProp[name];
                }

                // not found
                return null;
            }
        }

        public string Name { get { return mName; } }
        public string Value { get { return mValue; } }

        public ReadOnlyCollection<PropertyBag> Bases { get { return new ReadOnlyCollection<PropertyBag>(mBases); } }

        public int Count { get { return FlattenProperties.Count; } }

        public int ToInt32()
        {
            return Int32.Parse(mValue);
        }

        public void Add(PropertyBag prop)
        {
            mChildren.Add(prop);
        }

        public bool TryGetValue(string name, out PropertyBag value)
        {
            value = this[name];
            return value != null;
        }

        public T GetOrDefault<T>(string name, Func<string, T> converter, T defaultValue)
        {
            if (Contains(name)) return converter(this[name].Value);

            return defaultValue;
        }

        public string GetOrDefault(string name, string defaultValue)
        {
            if (Contains(name)) return this[name].Value;

            return defaultValue;
        }

        public int GetOrDefault(string name, int defaultValue)
        {
            if (Contains(name)) return this[name].ToInt32();

            return defaultValue;
        }

        private PropSetCollection FlattenProperties
        {
            get
            {
                PropSetCollection properties = new PropSetCollection();

                // start with the parent properties
                foreach (PropertyBag baseProp in mBases)
                {
                    foreach (PropertyBag child in baseProp.FlattenProperties)
                    {
                        if (properties.Contains(child.Name)) properties.Remove(child.Name);
                        properties.Add(child);
                    }
                }

                // override with the child ones
                foreach (PropertyBag child in mChildren)
                {
                    if (properties.Contains(child.Name)) properties.Remove(child.Name);
                    properties.Add(child);
                }

                return properties;
            }
        }

        private class PropSetCollection : KeyedCollection<string, PropertyBag>
        {
            protected override string GetKeyForItem(PropertyBag item) { return item.Name; }
        }

        private readonly List<PropertyBag> mBases = new List<PropertyBag>();

        private string mName;
        private string mValue;

        private PropSetCollection mChildren = new PropSetCollection();
    }
}
