using System.Threading.Tasks;
using AspectInjector.Analyzer.Analyzers;
using AspectInjector.Analyzer.Tests.Helpers;
using AspectInjector.Rules;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AspectInjector.Analyzer.Tests.Analyzers
{
    public class MixinAnalyzerTests : CorrectDefinitionsTests
    {
        [Fact]
        public async Task Can_Mixin_Only_Interfaces()
        {
            var test = 
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Mixin(typeof(DummyClass))]
    [Aspect(Scope.Global)]
    class TypeClass
    {   
    }

    class DummyClass{}
}";

            var expected = DiagnosticResult.From(EffectRules.MixinSupportsOnlyInterfaces.AsDescriptor(), 4, 6);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Mixin_Should_Implement_Interface()
        {
            var test = 
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Mixin(typeof(IDummyInterface))]
    [Aspect(Scope.Global)]
    class TypeClass
    {   
    }

    interface IDummyInterface{}
}";

            var expected = DiagnosticResult.From(EffectRules.MixinSupportsOnlyAspectInterfaces.AsDescriptor(), 4, 6);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Mixin_Should_Implement_Interface_With_BaseClass()
        {
            var test = 
@"using AspectInjector.Broker;
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
}";
            var expected = new[] {
                DiagnosticResult.From(EffectRules.MixinSupportsOnlyAspectInterfaces.AsDescriptor(), 4, 6),
                DiagnosticResult.From(EffectRules.MixinSupportsOnlyAspectInterfaces.AsDescriptor(), 5, 6),
            };

            await this.VerifyCSharpDiagnostic(test, expected);
        }


        [Fact]
        public async Task Mixin_Should_Be_Part_Of_Aspect()
        {
            var test = 
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Mixin(typeof(IDummyInterface))]
    class TypeClass : IDummyInterface
    {   
    }

    interface IDummyInterface{}
}";
            var expected = DiagnosticResult.From(EffectRules.EffectMustBePartOfAspect.AsDescriptor(), 4, 6);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MixinAttributeAnalyzer();
        }
    }
}
