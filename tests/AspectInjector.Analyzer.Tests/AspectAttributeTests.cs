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
    public class AspectAttributeTests : Verifiers.CodeFixVerifier
    {
        [Fact]
        public async Task NoCode_NoDiagnostics()
        {
            var test = @"";

            await this.VerifyCSharpDiagnostic(test);
        }


        [Fact]
        public async Task Aspect_Must_Not_Be_Static()
        {
            var test =
@"using AspectInjector.Broker;
    namespace TestNameSpace
    {
            [Aspect(Scope.Global)]
            static class TypeClass
            {
        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
            }
    }";
            var expected = DiagnosticResult.From(AspectRules.AspectMustHaveValidSignature.AsDescriptor(), 4, 14);
            await this.VerifyCSharpDiagnostic(test, expected);

            var fixtest =
@"using AspectInjector.Broker;
    namespace TestNameSpace
    {
            [Aspect(Scope.Global)]
            class TypeClass
            {
        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
            }
    }";
            await this.VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public async Task Aspect_Must_Not_Be_Abstract()
        {
            var test =
@"using AspectInjector.Broker;
    namespace TestNameSpace
    {
            [Aspect(Scope.Global)]
            abstract class TypeClass<T>
            {
        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
            }
    }";

            var fixtest =
@"using AspectInjector.Broker;
    namespace TestNameSpace
    {
            [Aspect(Scope.Global)]
            class TypeClass
            {
        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
            }
    }";
            await this.VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public async Task Aspect_Must_Not_Be_Generic()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass<T>
    {
        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
    }
}";

            var expected = DiagnosticResult.From(AspectRules.AspectMustHaveValidSignature.AsDescriptor(), 4, 6);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Aspect_Must_Have_Parameterless_Ctor()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        public TypeClass(string value){}

        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
    }
}";

            var expected = DiagnosticResult.From(AspectRules.AspectMustHaveContructorOrFactory.AsDescriptor(), 4, 6);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Aspect_Factory_Must_Have_Factory_Method()
        {
            var test =
@"using AspectInjector.Broker;
using System;
namespace TestNameSpace
{    
    [Aspect(Scope.Global, Factory = typeof(FakeFactory))]
    class TypeClass
    {
        public TypeClass(string value){}

        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
    }

    class FakeFactory {
        public object GetInstance(Type type){
            return null;
        }
    }
}";

            var expected = DiagnosticResult.From(AspectRules.AspectFactoryMustContainFactoryMethod.AsDescriptor(), 5, 6);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Aspect_Factory_Must_Have_Factory_Method_Valid()
        {
            var test =
@"using AspectInjector.Broker;
using System;
namespace TestNameSpace
{
    [Aspect(Scope.Global, Factory = typeof(FakeFactory))]
    class TypeClass
    {
        public TypeClass(string value){}

        [Advice(Advice.Type.Before, Target.Method)]
        public void Before(){}
    }

    class FakeFactory {
        public static object GetInstance(Type type){
            return null;
        }
    }
}";
            await this.VerifyCSharpDiagnostic(test);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AspectCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AspectAttributeAnalyzer();
        }
    }
}
