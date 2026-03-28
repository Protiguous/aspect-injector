using System.Threading.Tasks;
using AspectInjector.Analyzer.Analyzers;
using AspectInjector.Analyzer.CodeFixes;
using AspectInjector.Analyzer.Tests.Helpers;
using AspectInjector.Rules;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AspectInjector.Analyzer.Tests
{
    public class MixinAnalyzerTests : Verifiers.CodeFixVerifier
    {
        [Fact]
        public async Task NoCode_NoDiagnostics()
        {
            var test = @"";

            await this.VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public async Task Can_Mixin_Only_Interfaces()
        {
            var test = @"  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AspectInjector.Broker;

    namespace TestNameSpace
    {
        [Mixin(typeof(DummyClass))]
        [Aspect(Scope.Global)]
        class TypeClass
        {   
        }

        interface IDummyInterface{}
        class DummyClass{}
    }";
            var expected = DiagnosticResult.From(EffectRules.MixinSupportsOnlyInterfaces.AsDescriptor(), 12, 10);
            await this.VerifyCSharpDiagnostic(test, expected);

            //        var fixtest = @"
            //using System;
            //using System.Collections.Generic;
            //using System.Linq;
            //using System.Text;
            //using System.Threading.Tasks;
            //using System.Diagnostics;

            //namespace ConsoleApplication1
            //{
            //    class TYPENAME
            //    {   
            //    }
            //}";
            //        VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public async Task Mixin_Should_Implement_Interface()
        {
            var test = @"  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AspectInjector.Broker;

    namespace TestNameSpace
    {
        [Mixin(typeof(IDummyInterface))]
        [Aspect(Scope.Global)]
        class TypeClass
        {   
        }

        interface IDummyInterface{}
        class DummyClass{}
    }";
            var expected = DiagnosticResult.From(EffectRules.MixinSupportsOnlyAspectInterfaces.AsDescriptor(), 12, 10);
            await this.VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AspectInjector.Broker;

    namespace TestNameSpace
    {
        [Mixin(typeof(IDummyInterface))]
        [Aspect(Scope.Global)]
        class TypeClass : IDummyInterface
        {   
        }

        interface IDummyInterface{}
        class DummyClass{}
    }";
            await this.VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public async Task Mixin_Should_Implement_Interface_With_BaseClass()
        {
            var test = @"  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AspectInjector.Broker;

    namespace TestNameSpace
    {
        [Mixin(typeof(IDummyInterface))]
        [Mixin(typeof(IDummyInterface2))]
        [Aspect(Scope.Global)]
        class TypeClass : object
        {   
        }

        interface IDummyInterface{}
        interface IDummyInterface2{}
        class DummyClass{}
    }";
            var expected1 = DiagnosticResult.From(EffectRules.MixinSupportsOnlyAspectInterfaces.AsDescriptor(), 12, 10);
            var expected2 = DiagnosticResult.From(EffectRules.MixinSupportsOnlyAspectInterfaces.AsDescriptor(), 13, 10);

            await this.VerifyCSharpDiagnostic(test, expected1, expected2);

            var fixtest = @"  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AspectInjector.Broker;

    namespace TestNameSpace
    {
        [Mixin(typeof(IDummyInterface))]
        [Mixin(typeof(IDummyInterface2))]
        [Aspect(Scope.Global)]
        class TypeClass : object, IDummyInterface, IDummyInterface2
        {   
        }

        interface IDummyInterface{}
        interface IDummyInterface2{}
        class DummyClass{}
    }";
            await this.VerifyCSharpFix(test, fixtest);
        }


        [Fact]
        public async Task Mixin_Should_Be_Part_Of_Aspect()
        {
            var test = @"  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AspectInjector.Broker;

    namespace TestNameSpace
    {
        [Mixin(typeof(IDummyInterface))]
        class TypeClass : IDummyInterface
        {   
        }

        interface IDummyInterface{}
        class DummyClass{}
    }";
            var expected = DiagnosticResult.From(EffectRules.EffectMustBePartOfAspect.AsDescriptor(), 12, 10);
            await this.VerifyCSharpDiagnostic(test, expected);

//            var fixtest = @"
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using AspectInjector.Broker;

//    namespace TestNameSpace
//    {
//        [Mixin(typeof(IDummyInterface))]
//        [Aspect(Scope.Global)]
//        class TypeClass : IDummyInterface
//        {   
//        }

//        interface IDummyInterface{}
//        class DummyClass{}
//    }";
//            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MixinCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MixinAttributeAnalyzer();
        }
    }
}
