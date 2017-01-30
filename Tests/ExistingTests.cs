using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class ExistingTests
{
    Assembly assembly;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public ExistingTests()
    {
        beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcessExistingAttribute\bin\Debug\AssemblyToProcessExistingAttribute.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath);
        var currentDirectory = AssemblyLocation.CurrentDirectory();

        var config = new XElement("Envify");
        config.Add(new XElement("Attribute", new XAttribute("Name", "AssemblyInformationalVersionAttribute")));
        config.Add(new XElement("Default", new XAttribute("Key", "TEST"), new XAttribute("Value", "SUCCEED")));

        var moduleWeaver = new ModuleWeaver
                           {
                               Config = config,
                               ModuleDefinition = moduleDefinition,
                               AddinDirectoryPath = currentDirectory,
                               SolutionDirectoryPath = currentDirectory,
                               AssemblyFilePath = afterAssemblyPath,
                           };

        moduleWeaver.Execute();
        moduleDefinition.Write(afterAssemblyPath);
        moduleWeaver.AfterWeaving();

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }


    [Test]
    public void EnsureAttributeExists()
    {
        var customAttributes = (AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof (AssemblyInformationalVersionAttribute), false).First();
        Assert.IsNotNull(customAttributes.InformationalVersion);
        Assert.IsNotEmpty(customAttributes.InformationalVersion);
        Debug.WriteLine(customAttributes.InformationalVersion);
    }

    [Test]
    public void TemplateIsReplaced()
    {
        var os = Environment.GetEnvironmentVariable("OS");
        Assert.NotNull(os);

        var customAttributes = (AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false).First();
        Assert.True(customAttributes.InformationalVersion.StartsWith("SUCCEED"));
        Assert.True(customAttributes.InformationalVersion.Contains(os));
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }
#endif

}