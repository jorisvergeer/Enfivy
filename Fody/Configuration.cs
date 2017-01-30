using System;
using System.Collections.Generic;
using System.Xml.Linq;

public class Configuration
{
    public Dictionary<string, string> Defaults = new Dictionary<string, string>();
    public List<string> AttributeNames = new List<string>();

    public string Preamble = "%%";
    public string Postamble = "%%";
    public string Regex = "[a-zA-Z0-9_-]*";

    public Configuration(XElement config)
    {
        if (config == null)
        {
            return;
        }

        foreach (var attribute in config.Attributes())
        {
            if (attribute.Name == "Preamble")
                Preamble = attribute.Value;
            if (attribute.Name == "Postamble")
                Postamble = attribute.Value;
            if (attribute.Name == "Regex")
                Regex = attribute.Value;
        }

        foreach (var element in config.Elements())
        {
            if (element.Name == "Attribute")
            {
                var name = (string)null;

                foreach (var attribute in element.Attributes())
                {
                    if (attribute.Name == "Name")
                    {
                        name = attribute.Value;
                    }
                }

                if (name != null)
                {
                    AttributeNames.Add(name);
                }
            }

            if (element.Name == "Default")
            {
                var key = (string)null;
                var value = (string)null;

                foreach (var attribute in element.Attributes())
                {
                    if (attribute.Name == "Key")
                    {
                        key = attribute.Value;
                    }
                    else if (attribute.Name == "Value")
                    {
                        value = attribute.Value;
                    }
                }

                if (key != null && value != null)
                {
                    Defaults.Add(key, value);
                }
            }
        }
    }
}
