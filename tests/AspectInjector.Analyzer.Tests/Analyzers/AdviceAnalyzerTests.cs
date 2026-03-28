using System.Threading.Tasks;
using AspectInjector.Analyzer.Analyzers;
using AspectInjector.Analyzer.Tests.Helpers;
using AspectInjector.Rules;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace AspectInjector.Analyzer.Tests.Analyzers
{
    public class AdviceAnalyzerTests : CorrectDefinitionsTests
    {
        [Fact]
        public async Task Advice_Must_Not_Be_Static()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        [Advice(Kind.Before)]
        public static void Before(){}
    }
}";
            var expected = DiagnosticResult.From(EffectRules.AdviceMustHaveValidSingnature.AsDescriptor(), 7, 10);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Advice_Must_Be_Public()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        [Advice(Kind.Before)]
        private void Before(){}
    }
}";

            var expected = DiagnosticResult.From(EffectRules.AdviceMustHaveValidSingnature.AsDescriptor(), 7, 10);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Advice_Must_Not_Be_Generic()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        [Advice(Kind.Before)]
        public void Before<T>(){}
    }
}";

            var expected = DiagnosticResult.From(EffectRules.AdviceMustHaveValidSingnature.AsDescriptor(), 7, 10);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Advice_Inline_Must_Be_Void()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        [Advice(Kind.Before)]
        public int Before(){ return 1; }
    }
}";

            var expected = DiagnosticResult.From(EffectRules.AdviceMustHaveValidSingnature.AsDescriptor(), 7, 10);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Advice_Around_Must_Be_Object()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        [Advice(Kind.Around)]
        public void Around(){ }
    }
}";

            var expected = DiagnosticResult.From(EffectRules.AdviceMustHaveValidSingnature.AsDescriptor(), 7, 10);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Advice_Argument_Must_Be_Bound()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    [Aspect(Scope.Global)]
    class TypeClass
    {
        [Advice(Kind.Before)]
        public void Before(string data, int val, [Argument(Source.Instance)] object i){ }
    }
}";

            var expected = new[]{
                DiagnosticResult.From(EffectRules.AdviceArgumentMustBeBound.AsDescriptor(), 8, 35),
                DiagnosticResult.From(EffectRules.AdviceArgumentMustBeBound.AsDescriptor(), 8, 45),
            };
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public async Task Advice_Should_Be_Part_Of_Aspect()
        {
            var test =
@"using AspectInjector.Broker;
namespace TestNameSpace
{
    class TypeClass
    {   
        [Advice(Kind.Before)]
        public void Before(){}
    }
}";
            var expected = DiagnosticResult.From(EffectRules.EffectMustBePartOfAspect.AsDescriptor(), 6, 10);
            await this.VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AdviceAttributeAnalyzer();
        }
    }
}
