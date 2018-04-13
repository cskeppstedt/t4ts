using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using T4TS.Builders;
using T4TS.Example.Models;
using T4TS.Tests.Models;
using T4TS.Tests.Utils;
using T4TS.Tests.Fixtures.Inheritance;
using T4TS.Tests.Fixtures.Basic;
using System;

namespace T4TS.Tests
{
    [TestClass]
    public class CodeTraverserTests
    {
        [TestMethod]
        public void ShouldBuildInterfacesFromMarkedClassesOnly()
        {
            var solution = DTETransformer.BuildDteSolution(
                typeof(LocalModel),                 // has the TypeScriptInterface attribute
                typeof(ModelFromDifferentProject),  // has the TypeScriptInterface attribute
                typeof(string)                      // has no TypeScriptInterface attribute
            );

            var attributeBuilder = new AttributeInterfaceBuilder(new Settings());
            var codeTraverser = new CodeTraverser(
                solution,
                new TypeContext())
            {
                Settings = new CodeTraverser.TraverserSettings()
                {
                    ClassToInterfaceBuilder = attributeBuilder
                }
            };
            Assert.AreEqual(2, codeTraverser.GetAllInterfaces().Count());
        }

        [TestMethod]
        public void ShouldWorkIfSolutionContainsPartialClasses() {
            //this may not make much sense, but this is my best guess at mimicking partial classes...
            //Actually traceing out all the TypeScriptInterfaces in the T4TS.Example solution contains these:
            //  ...
            //T4TS.Example.Models.PartialModelByEF 
            //T4TS.Example.Models.PartialModelByEF 
            //T4TS.Example.Models.InheritsPartialModelByEF 
            //T4TS.Example.Models.Partial 
            //T4TS.Example.Models.Partial 
            //  ...

            var solution = DTETransformer.BuildDteSolution(
                typeof(T4TS.Tests.Fixtures.Partial.PartialModel),
                typeof(T4TS.Tests.Fixtures.Partial.PartialModel),
                typeof(T4TS.Tests.Fixtures.Partial.InheritsFromPartialModel)
            );

            var attributeBuilder = new AttributeInterfaceBuilder(new Settings());
            var codeTraverser = new CodeTraverser(
                solution,
                new TypeContext())
            {
                Settings = new CodeTraverser.TraverserSettings()
                {
                    ClassToInterfaceBuilder = attributeBuilder
                }
            };
            var allModules = codeTraverser.GetAllInterfaces();
            Assert.AreEqual(1, allModules.Count());

            // TODO: verify that the interface contains members from both partial classes
            Assert.AreEqual(2, allModules.First().Interfaces.Count());
        }

        [TestMethod]
        public void ShouldHandleReservedPropNames()
        {
            var solution = DTETransformer.BuildDteSolution(typeof(ReservedPropModel));
            var attributeBuilder = new AttributeInterfaceBuilder(new Settings());
            var codeTraverser = new CodeTraverser(
                solution,
                new TypeContext())
            {
                Settings = new CodeTraverser.TraverserSettings()
                {
                    ClassToInterfaceBuilder = attributeBuilder
                }
            };
            
            var modules = codeTraverser.GetAllInterfaces();
            var interfaces = modules.Single().Interfaces;
            var modelInterface = interfaces.Single();
            
            var classProp = modelInterface.Fields.SingleOrDefault(m => m.Name == "class");
            var readonlyProp = modelInterface.Fields.SingleOrDefault(m => m.Name == "readonly");
            var publicProp = modelInterface.Fields.SingleOrDefault(m => m.Name == "public");

            Assert.AreEqual(3, modelInterface.Fields.Count);

            Assert.IsNotNull(classProp);
            Assert.IsNotNull(readonlyProp);
            Assert.IsNotNull(publicProp);
            
            Assert.IsTrue(publicProp.Optional);
        }

        [TestMethod]
        public void CodeTraverserClassFilterFail()
        {
            try
            {
            // Expect
            new OutputForDirectBuilder(
                typeof(InheritanceModel),
                typeof(OtherInheritanceModel),
                typeof(ModelFromDifferentProject),
                typeof(BasicModel))
                    .WithSettings((settings) =>
                    {
                        settings.NamespaceToModuleMap.Add(
                            typeof(ModelFromDifferentProject).Namespace,
                            "External");
                        settings.NamespaceToModuleMap.Add(
                            typeof(InheritanceModel).Namespace,
                            "T4TS");
                        settings.NamespaceToModuleMap.Add(
                            typeof(BasicModel).Namespace,
                            "T4TS");
                    })
                    .WithTraverserSettings((settings) =>
                        settings.ClassFilter = (codeClass) =>
                            codeClass.FullName == typeof(InheritanceModel).FullName
                                || codeClass.FullName == typeof(BasicModel).FullName)
                    .ToEqual(ExpectedOutputSingle);

                Assert.Fail("Expected exception for unresolved type");
            }
            catch (Exception)
            {
            }
        }

        [TestMethod]
        public void CodeTraverserClassFilterWithResolve()
        {
            // Expect
            new OutputForDirectBuilder(
                typeof(InheritanceModel),
                typeof(OtherInheritanceModel),
                typeof(ModelFromDifferentProject),
                typeof(BasicModel))
                    .WithSettings((settings) =>
                    {
                        settings.NamespaceToModuleMap.Add(
                            typeof(ModelFromDifferentProject).Namespace,
                            "External");
                        settings.NamespaceToModuleMap.Add(
                            typeof(InheritanceModel).Namespace,
                            "T4TS");
                        settings.NamespaceToModuleMap.Add(
                            typeof(BasicModel).Namespace,
                            "T4TS");
                    })
                    .WithTraverserSettings((settings) =>
                    {
                        settings.ClassFilter =
                            (codeClass) => codeClass.FullName == typeof(InheritanceModel).FullName;
                        settings.ResolveReferences = true;
                    })
                    .ToEqual(ExpectedOutputResolve);
        }

        [TestMethod]
        public void CodeTraverserClassFilterWithResolveMultipleProjects()
        {
            // Expect
            new OutputForDirectBuilder(
                typeof(InheritanceModel),
                typeof(OtherInheritanceModel),
                typeof(ModelFromDifferentProject),
                typeof(BasicModel),
                typeof(DifferentProjectSameNamespace))
                    .WithSettings((settings) =>
                    {
                        settings.NamespaceToModuleMap.Add(
                            typeof(ModelFromDifferentProject).Namespace,
                            "External");
                        settings.NamespaceToModuleMap.Add(
                            typeof(InheritanceModel).Namespace,
                            "T4TS");
                        settings.NamespaceToModuleMap.Add(
                            typeof(BasicModel).Namespace,
                            "T4TS");
                    })
                    .WithTraverserSettings((settings) =>
                    {
                        settings.ClassFilter = (codeClass) => 
                            codeClass.FullName == typeof(InheritanceModel).FullName
                                || codeClass.FullName == typeof(DifferentProjectSameNamespace).FullName;
                        settings.ResolveReferences = true;
                    })
                    .ToEqual(ExpectedOutputResolveMultipleProjects);
        }

        const string ExpectedOutputSingle =
@"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.InheritanceModel */
    export interface InheritanceModel {
        OnInheritanceModel: T4TS.BasicModel;
    }
}
";

        const string ExpectedOutputResolve =
@"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module External {
    /** Generated from T4TS.Example.Models.ModelFromDifferentProject */
    export interface ModelFromDifferentProject {
        Id: number;
    }
}

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.InheritanceModel */
    export interface InheritanceModel extends T4TS.OtherInheritanceModel {
        OnInheritanceModel: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel */
    export interface OtherInheritanceModel extends External.ModelFromDifferentProject {
        OnOtherInheritanceModel: T4TS.BasicModel;
    }
}
";
        const string ExpectedOutputResolveMultipleProjects =
@"/****************************************************************************
  Generated by T4TS.tt - don't make any changes in this file
****************************************************************************/

declare module External {
    /** Generated from T4TS.Example.Models.ModelFromDifferentProject */
    export interface ModelFromDifferentProject {
        Id: number;
    }
}

declare module T4TS {
    /** Generated from T4TS.Tests.Fixtures.Basic.BasicModel */
    export interface BasicModel {
        MyProperty: number;
        SomeDateTime: string;
    }
    /** Generated from T4TS.Tests.Fixtures.Basic.DifferentProjectSameNamespace */
    export interface DifferentProjectSameNamespace {
        Id: number;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.InheritanceModel */
    export interface InheritanceModel extends T4TS.OtherInheritanceModel {
        OnInheritanceModel: T4TS.BasicModel;
    }
    /** Generated from T4TS.Tests.Fixtures.Inheritance.OtherInheritanceModel */
    export interface OtherInheritanceModel extends External.ModelFromDifferentProject {
        OnOtherInheritanceModel: T4TS.BasicModel;
    }
}
";
    }
}
