using System.Text.RegularExpressions;
using System.Xml;
using Verse;

namespace TR;

public class TypeFloat<T>
{
    public T type;
    public float value = 1;

    public TypeFloat(){}
    public TypeFloat(T type, float value)
    {
        this.type = type;
        this.value = value;
    }

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        string s = Regex.Replace(xmlRoot.FirstChild.Value, @"\s+", "");
        string[] array = s.Split(',');
        type = (T) ParseHelper.FromString(array[0], typeof(T));
        if (array.Length > 1)
            this.value = (float)ParseHelper.FromString(array[1], typeof(float));
    }
}