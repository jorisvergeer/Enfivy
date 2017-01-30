### This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Envify replace string tokens with environment variables during build

Shamelessly based on Stamp.Fody

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/Envify.Fody.svg?style=flat)](https://www.nuget.org/packages/Envify.Fody/)

https://nuget.org/packages/Envify.Fody/

    PM> Install-Package Envify.Fody

## What it does 

Replaces token in assemblies with values of environment valiables. (For example set by various build scripts or CI environments)

For example

~~~~
[assembly: AssemblyInformationalVersionAttribute("%%TEST%%_%%OS%%")]
~~~~

With the example configuration below becomes:

~~~~
[assembly: AssemblyInformationalVersionAttribute("This works_Windows_NT")]
~~~~

## Configuration

All config options are attributes en children of Envify element in FodyWeavers.xml

~~~~
<?xml version="1.0" encoding="utf-8"?>
<Weavers>
  <Envify Preamble="%%" Postamble="%%" Regex="[a-zA-Z0-9_-]*">
    <Attribute Name="AssemblyInformationalVersionAttribute"/>
    <Default Key="TEST_VAR" Value="This works" />
  </Envify>
</Weavers>
~~~~

### Attributes of Envify

`Preamble` defines the start of a replacement token. It defaults to "%%"

`Postamble` defines the end of a replacement token. It defaults to "%%"

`Regex` defined the valid characters between the `Preamble` and `Postamble` . It defaults to "[a-zA-Z0-9_-]*"

### Elements in Envify

`Attribute` defines which attributes should be checked for replacement tokens

`Default` defines default values for a replacement token defined by the `Key` and `Value` attributes