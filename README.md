# 2025-02-06 - Project forked and patched

Patched the generator to build classes without a static CultureInfo object. We needed to access various resource text with different translations for each incoming http request. The original implementation uses CurrentUIThread.CultureInfo which is not thread safe at all when we need to change the CultureInfo according to the incoming http request parameters.

**NOTE:** this project must be included as a nuget package. We tried to include it directly in the solution, but failed to do this successfully. Unknown what caused issues, but the generator did not create correct files. It works great when included as a nuget package.

--
Henning Jensen, henning_AT_arkitektum.no


# ResXFileCodeGenerator
ResX Designer Source Generator. Generates strongly-typed resource classes for looking up localized strings.

## Usage

Install the `VocaDb.ResXFileCodeGenerator` package:

```psl
dotnet add package VocaDb.ResXFileCodeGenerator
```

Generated source from [ActivityEntrySortRuleNames.resx](https://github.com/VocaDB/vocadb/blob/6ac18dd2981f82100c8c99566537e4916920219e/VocaDbWeb.Resources/App_GlobalResources/ActivityEntrySortRuleNames.resx):

```cs
// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace Resources
{
    using System.Globalization;
    using System.Resources;

    public static class ActivityEntrySortRuleNames
    {
        private static ResourceManager? s_resourceManager;
        public static ResourceManager ResourceManager => s_resourceManager ??= new ResourceManager("VocaDb.Web.App_GlobalResources.ActivityEntrySortRuleNames", typeof(ActivityEntrySortRuleNames).Assembly);
        public static CultureInfo? CultureInfo { get; set; }

        /// <summary>
        /// Looks up a localized string similar to Oldest.
        /// </summary>
        public static string? CreateDate => ResourceManager.GetString(nameof(CreateDate), CultureInfo);

        /// <summary>
        /// Looks up a localized string similar to Newest.
        /// </summary>
        public static string? CreateDateDescending => ResourceManager.GetString(nameof(CreateDateDescending), CultureInfo);
    }
}
```

## New in version 3

* The generator now utilizes the IIncrementalGenerator API to instantly update the generated code, thus giving you instant intellisense.

* Added error handling for multiple members of same name, and members that have same name as class. These are clickable in visual studio to lead you to the source of the error, unlike before where they resulted in broken builds and you had to figure out why.

* Namespace naming fixed for resx files in the top level folder.

* Resx files can now be named with multiple extensions, e.g. myresources.cshtml.resx and will result in class being called myresources.

* Added the ability to generate inner classes, partial outer classes and non-static members. Very useful if you want to ensure that only a particular class can use those resources instead of being spread around the codebase.

* Use same 'Link' setting as msbuild uses to determine embedded file name.

* Can set a class postfix name

## New in version 3.1

* The generator can now generate code to lookup translations instead of using the 20 year old System.Resources.ResourceManager

## Options

### PublicClass (per file or globally)

Use cases: https://github.com/VocaDB/ResXFileCodeGenerator/issues/2.

Since version 2.0.0, VocaDB.ResXFileCodeGenerator generates internal classes by default. You can change this behavior by setting `PublicClass` to `true`.

```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\ArtistCategoriesNames.resx">
    <PublicClass>true</PublicClass>
  </EmbeddedResource>
</ItemGroup>
```
or
```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\ArtistCategoriesNames.resx" PublicClass="true" />
</ItemGroup>
```

If you want to apply this globally, use
```xml
<PropertyGroup>
  <ResXFileCodeGenerator_PublicClass>true</ResXFileCodeGenerator_PublicClass>
</PropertyGroup>
```

### NullForgivingOperators (globally)

Use cases: https://github.com/VocaDB/ResXFileCodeGenerator/issues/1.

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_NullForgivingOperators>true</ResXFileCodeGenerator_NullForgivingOperators>
</PropertyGroup>
```

By setting `ResXFileCodeGenerator_NullForgivingOperators` to `true`, VocaDB.ResXFileCodeGenerator generates
```cs
public static string CreateDate => ResourceManager.GetString(nameof(CreateDate), CultureInfo)!;
```
instead of
```cs
public static string? CreateDate => ResourceManager.GetString(nameof(CreateDate), CultureInfo);
```

### Non-static classes (per file or globally)

To use generated resources with [Microsoft.Extensions.Localization](https://docs.microsoft.com/en-us/dotnet/core/extensions/localization) `IStringLocalizer<T>` and resource manager, the resolved type cannot be a static class. You can disable default behaviour per file by setting the value to `false`.

```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\ArtistCategoriesNames.resx">
    <StaticClass>false</StaticClass>
  </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_StaticClass>false</ResXFileCodeGenerator_StaticClass>
</PropertyGroup>
```

With global non-static class you can also reset `StaticClass` per file by setting the value to anything but `false`.

### Partial classes (per file or globally)

To extend an existing class, you can make your classes partial.

```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\ArtistCategoriesNames.resx">
    <PartialClass>true</PartialClass>
  </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_PartialClass>true</ResXFileCodeGenerator_PartialClass>
</PropertyGroup>
```

### Static Members (per file or globally)

In some rare cases it might be useful for the members to be non-static.

```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\ArtistCategoriesNames.resx">
    <StaticMembers>false</StaticMembers>
  </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_StaticMembers>false</ResXFileCodeGenerator_StaticMembers>
</PropertyGroup>
```

### Postfix class name (per file or globally)

In some cases the it is useful if the name of the generated class doesn't follow the filename.

A clear example is Razor pages that always generates a class for the code-behind named "-Model".
This example configuration allows you to use Resources.MyResource in your model, or @Model.Resources.MyResource in your cshtml file.

```xml
<ItemGroup>
  <EmbeddedResource Update="**/Pages/*.resx">
    <ClassNamePostfix>Model</ClassNamePostfix>
    <StaticMembers>false</StaticMembers>
    <StaticClass>false</StaticClass>
    <PartialClass>true</PartialClass>
    <PublicClass>true</PublicClass>
    <InnerClassVisibility>public</InnerClassVisibility>
    <PartialClass>false</PartialClass>
    <InnerClassInstanceName>Resources</InnerClassInstanceName>
    <InnerClassName>_Resources</InnerClassName>
  </EmbeddedResource>
</ItemGroup>
```


or just the postfix globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_ClassNamePostfix>Model</ResXFileCodeGenerator_ClassNamePostfix>
</PropertyGroup>
```

## Inner classes (per file or globally)

If your resx files are organized along with code files, it can be quite useful to ensure that the resources are not accessible outside the specific class the resx file belong to.

```xml
<ItemGroup>
    <EmbeddedResource Update="**/*.resx">
        <DependentUpon>$([System.String]::Copy('%(FileName).cs'))</DependentUpon>
        <InnerClassName>MyResources</InnerClassName>
        <InnerClassVisibility>private</InnerClassVisibility>
        <InnerClassInstanceName>EveryoneLikeMyNaming</InnerClassInstanceName>
        <StaticMembers>false</StaticMembers>
        <StaticClass>false</StaticClass>
        <PartialClass>true</PartialClass>
    </EmbeddedResource>
    <EmbeddedResource Update="**/*.??.resx;**/*.??-??.resx">
        <DependentUpon>$([System.IO.Path]::GetFileNameWithoutExtension('%(FileName)')).resx</DependentUpon>
    </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_InnerClassName>MyResources</ResXFileCodeGenerator_InnerClassName>
  <ResXFileCodeGenerator_InnerClassVisibility>private</ResXFileCodeGenerator_InnerClassVisibility>
  <ResXFileCodeGenerator_InnerClassInstanceName>EveryoneLikeMyNaming</InnerClassInstanceName>
  <ResXFileCodeGenerator_StaticMembers>false</ResXFileCodeGenerator_StaticMembers>
  <ResXFileCodeGenerator_StaticClass>false</ResXFileCodeGenerator_StaticClass>
  <ResXFileCodeGenerator_PartialClass>true</ResXFileCodeGenerator_PartialClass>
</PropertyGroup>
```

This example would generate files like this:

```cs
// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace Resources
{
    using System.Globalization;
    using System.Resources;

    public partial class ActivityEntryModel
    {
        public MyResources EveryoneLikeMyNaming { get; } = new();

        private class MyResources
        {
            private static ResourceManager? s_resourceManager;
            public static ResourceManager ResourceManager => s_resourceManager ??= new ResourceManager("VocaDb.Web.App_GlobalResources.ActivityEntryModel", typeof(ActivityEntryModel).Assembly);
            public CultureInfo? CultureInfo { get; set; }

            /// <summary>
            /// Looks up a localized string similar to Oldest.
            /// </summary>
            public string? CreateDate => ResourceManager.GetString(nameof(CreateDate), CultureInfo);

            /// <summary>
            /// Looks up a localized string similar to Newest.
            /// </summary>
            public string? CreateDateDescending => ResourceManager.GetString(nameof(CreateDateDescending), CultureInfo);
        }
    }
}
```

### Inner Class Visibility (per file or globally)

By default inner classes are not generated, unless this setting is one of the following:

 * Public
 * Internal
 * Private
 * Protected
 * SameAsOuter

Case is ignored, so you could use "private".

It is also possible to use "NotGenerated" to override on a file if the global setting is to generate inner classes.

```xml
<ItemGroup>
    <EmbeddedResource Update="**/*.resx">
        <InnerClassVisibility>private</InnerClassVisibility>
    </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_InnerClassVisibility>private</ResXFileCodeGenerator_InnerClassVisibility>
</PropertyGroup>
```

### Inner Class name (per file or globally)

By default the inner class is named "Resources", which can be overriden with this setting:

```xml
<ItemGroup>
    <EmbeddedResource Update="**/*.resx">
        <InnerClassName>MyResources</InnerClassName>
    </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_InnerClassName>MyResources</ResXFileCodeGenerator_InnerClassName>
</PropertyGroup>
```


### Inner Class instance name (per file or globally)

By default no instance is available of the class, but that can be made available if this setting is given.

```xml
<ItemGroup>
    <EmbeddedResource Update="**/*.resx">
        <InnerClassInstanceName>EveryoneLikeMyNaming</InnerClassInstanceName>
    </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_InnerClassInstanceName>EveryoneLikeMyNaming</ResXFileCodeGenerator_InnerClassInstanceName>
</PropertyGroup>
```

For brevity, settings to make everything non-static is obmitted.

### Generate Code (per file or globally)

By default the ancient `System.Resources.ResourceManager` is used.

Benefits of using `System.Resources.ResourceManager`:

* Supports custom `CultureInfo`
* Languages are only loaded the first time a language is referenced
* Only use memory for the languages used
* Can ship satellite dlls seperately

Disadvantages of using `System.Resources.ResourceManager`

* The satellite dlls are always lazy loaded, so cold start penalty is high
* Satellite dlls requires that you can scan the dir for which files are available, which can cause issues in some project types
* Loading a satellite dll takes way more memory than just loading the respective strings
* Build time for .resources -> satellite dll can be quite slow (~150msec per file)
* Linker optimization doesn't work, since it cannot know which resources are referenced

Benefits of using `VocaDB` code generation:

* All languages are placed in the main dll, no more satellite dlls
* Lookup speed is ~600% faster (5ns vs 33ns)
* Zero allocations
* Very small code footprint (about 10 bytes per language, instead of including the entire `System.Resources`)
* Very fast build time
* Because all code is referencing the strings directly, the linker can see which strings are actually used and which are not.
* No cold start penalty
* Smaller combined size of dll (up to 50%, since it doesn't need to store the keys for every single language)

Disadvantages of using `VocaDB` code generation

* Since `CultureInfo` are pre-computed, custom `CultureInfo` are not supported (or rather, they always return the default language)
* Cannot lookup "all" keys (unless using reflection)
* Main dll size increased since it contains all language strings (sometimes, the compiler can pack code strings much better than resource strings and it doesn't need to store the keys)

Notice, it is required to set `GenerateResource` to false for all resx files to prevent the built-in resgen.exe from running.

```xml
<ItemGroup>
    <EmbeddedResource Update="**/*.resx">
        <UseVocaDbResManager>true</UseVocaDbResManager>
        <GenerateResource>false</GenerateResource>
    </EmbeddedResource>
</ItemGroup>
```

or globally

```xml
<PropertyGroup>
  <ResXFileCodeGenerator_UseVocaDbResManager>true</ResXFileCodeGenerator_UseVocaDbResManager>
</PropertyGroup>
<ItemGroup>
    <EmbeddedResource Update="@(EmbeddedResource)">
        <GenerateResource>false</GenerateResource>
    </EmbeddedResource>
</ItemGroup>
```

If you get build error MSB3030, add this to your csproj to prevent it from trying to copy satellite dlls that no longer exists

```xml
<Target Name="PreventMSB3030" DependsOnTargets="ComputeIntermediateSatelliteAssemblies" BeforeTargets="GenerateSatelliteAssemblies" >
  <ItemGroup>
    <IntermediateSatelliteAssembliesWithTargetPath Remove="@(IntermediateSatelliteAssembliesWithTargetPath)"></IntermediateSatelliteAssembliesWithTargetPath>
  </ItemGroup>   
</Target>
```


## Resource file namespaces

Linked resources namespace follow `Link` if it is set. The `Link` setting is also used by msbuild built-in 'resgen.exe' to determine the embedded filename.

Use-case: Linking `.resx` files from outside source (e.g. generated in a localization sub-module by translators) and expose them as "Resources" namespace.

```xml
<ItemGroup>
  <EmbeddedResource Include="..\..\Another.Project\Translations\*.resx">
    <Link>Resources\%(FileName)%(Extension)</Link>
    <PublicClass>true</PublicClass>
    <StaticClass>false</StaticClass>
  </EmbeddedResource>
  <EmbeddedResource Update="..\..\Another.Project\Translations\*.*.resx">
    <DependentUpon>$([System.IO.Path]::GetFilenameWithoutExtension([System.String]::Copy('%(FileName)'))).resx</DependentUpon>
  </EmbeddedResource>
</ItemGroup>
```

You can also use the `TargetPath` to just overwrite the namespace

```xml
<ItemGroup>
  <EmbeddedResource Include="..\..\Another.Project\Translations\*.resx">
    <TargetPath>Resources\%(FileName)%(Extension)</TargetPath>
    <PublicClass>true</PublicClass>
    <StaticClass>false</StaticClass>
  </EmbeddedResource>
  <EmbeddedResource Update="..\..\Another.Project\Translations\*.*.resx">
    <DependentUpon>$([System.IO.Path]::GetFilenameWithoutExtension([System.String]::Copy('%(FileName)'))).resx</DependentUpon>
  </EmbeddedResource>
</ItemGroup>
```

It is also possible to set the namespace using the `CustomToolNamespace` setting. Unlike the `Link` and `TargetPath`, which will prepend the assemblys namespace and includes the filename, the `CustomToolNamespace` is taken verbatim.

```xml
<ItemGroup>
  <EmbeddedResource Update="**\*.resx">
    <CustomToolNamespace>MyNamespace.AllMyResourcesAreBelongToYouNamespace</CustomToolNamespace>
  </EmbeddedResource>
</ItemGroup>
```

## References
- [Introducing C# Source Generators | .NET Blog](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/)
- [microsoft/CsWin32: A source generator to add a user-defined set of Win32 P/Invoke methods and supporting types to a C# project.](https://github.com/microsoft/cswin32)
- [kenkendk/mdresxfilecodegenerator: Resx Designer Generator](https://github.com/kenkendk/mdresxfilecodegenerator)
- [dotnet/ResXResourceManager: Manage localization of all ResX-Based resources in one central place.](https://github.com/dotnet/ResXResourceManager)
- [roslyn/source-generators.cookbook.md at master · dotnet/roslyn](https://github.com/dotnet/roslyn/blob/master/docs/features/source-generators.cookbook.md)
- [roslyn/Using Additional Files.md at master · dotnet/roslyn](https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Using%20Additional%20Files.md)
- [ufcpp - YouTube](https://www.youtube.com/channel/UCY-z_9mau6X-Vr4gk2aWtMQ)
