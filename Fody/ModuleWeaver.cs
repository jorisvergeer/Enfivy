using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Mono.Cecil;

public class ModuleWeaver
{
    public XElement Config { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public string SolutionDirectoryPath { get; set; }
    public string ProjectDirectoryPath { get; set; }
    public string AddinDirectoryPath { get; set; }
    public string AssemblyFilePath { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        var config = new Configuration(Config);

        var customAttributes = ModuleDefinition.Assembly.CustomAttributes;

        foreach (var customAttribute in customAttributes)
        {
            if (config.AttributeNames.Contains(customAttribute.AttributeType.Name))
            {
                LogInfo.Invoke("Found attribute: " + customAttribute.AttributeType.Name);
                for (int i = 0; i < customAttribute.ConstructorArguments.Count; i++)
                {
                    LogInfo.Invoke($"Attribute has {customAttribute.ConstructorArguments.Count} constructor arguments");
                    var arg = customAttribute.ConstructorArguments[i];
                    if (arg.Value is string)
                    {
                        LogInfo.Invoke($"Constructor argument index {i} is a string, replacing tokens");
                        var result = arg.Value as string;

                        foreach (Match match in Regex.Matches((string) arg.Value, $"{config.Preamble}({config.Regex}){config.Postamble}"))
                        {
                            LogInfo.Invoke($"Found {match.Value}");
                            var replaceWith = string.Empty;
                            var signature = match.Value;

                            if (match.Groups.Count > 1)
                            {
                                var key = match.Groups[1].Value;

                                LogInfo.Invoke($"Using key: {key}");
                                LogInfo.Invoke("Looking for environment var");
                                replaceWith = Environment.GetEnvironmentVariable(key);
                                if (replaceWith == null)
                                {
                                    LogInfo.Invoke("Looking for default ");
                                    replaceWith = config.Defaults.Where(pair => pair.Key == key).Select(pair => pair.Value).FirstOrDefault();
                                }
                                if (replaceWith == null)
                                {
                                    LogInfo.Invoke("No vlaue found");
                                    replaceWith = string.Empty;
                                }
                            }

                            LogInfo.Invoke($"Replacing {signature} with {replaceWith}");
                            result = result.Replace(signature, replaceWith);
                        }

                        customAttribute.ConstructorArguments[i] = new CustomAttributeArgument(ModuleDefinition.TypeSystem.String, result);
                    }
                    else
                    {
                        LogInfo.Invoke($"Constructor argument index {i} is no string");
                    }
                }
            }
        }
    }

    public void AfterWeaving()
    {

    }
}