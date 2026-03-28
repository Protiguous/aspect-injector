using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspectInjector.Tests.Assets;
using Xunit;
using static AspectInjector.Tests.Assets.IAssetIface1Wrapper<AspectInjector.Tests.Assets.Asset1>;

namespace AspectInjector.Tests.Runtime
{

	public class TestRunner
	{

		private static readonly List<string> _staticCtorEvents;

		static TestRunner()
		{
			TestLog.Reset();

			var type = typeof(TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>);
			RuntimeHelpers.RunClassConstructor(type.TypeHandle);

			_staticCtorEvents = TestLog.Log.ToList();
			TestLog.Reset();
		}

		public TestRunner()
		{
			var a = typeof(TestRunner);
			TestLog.Reset();
		}

		public List<string> GetConstructorArgsSequence(string prefix)
		{
			return new List<string>
			{
				$"{prefix}:{this.GetArgEvent(TestAssets.asset1)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset2)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset3)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset4)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset1)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset2)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset3)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset4)}",
				$"{prefix}:{this.GetArgEvent(default(int))}",
				$"{prefix}:{this.GetArgEvent(null)}",
				$"{prefix}:{this.GetArgEvent(default(short))}",
				$"{prefix}:{this.GetArgEvent(null)}"
			};
		}

		public List<string> GetMethodArgsSequence(string prefix)
		{
			return new List<string>
			{
				$"{prefix}:{this.GetArgEvent(TestAssets.asset1)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset2)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset3)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset4)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset5)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset1)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset2)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset3)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset4)}",
				$"{prefix}:{this.GetArgEvent(TestAssets.asset5)}",
				$"{prefix}:{this.GetArgEvent(default(int))}",
				$"{prefix}:{this.GetArgEvent(null)}",
				$"{prefix}:{this.GetArgEvent(default(short))}",
				$"{prefix}:{this.GetArgEvent(null)}",
				$"{prefix}:{this.GetArgEvent(null)}"
			};
		}

		private string GetArgEvent(object o)
		{
			if (o == null)
			{
				return "Arguments:null";
			}

			return $"Arguments:{o.GetType().Name}:{o}";
		}

		public void ExecConstructor()
		{
			int ao1;
			Asset1 ao2;
			short ao3;
			IAssetIface1<Asset1> ao4;

			var test = new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>(TestAssets.asset1, TestAssets.asset2, TestAssets.asset3,
				TestAssets.asset4, ref TestAssets.asset1, ref TestAssets.asset2, ref TestAssets.asset3, ref TestAssets.asset4, out ao1, out ao2,
				out ao3, out ao4);

			Assert.Equal(TestAssets.asset1, ao1);
			Assert.Equal(TestAssets.asset2, ao2);
			Assert.Equal(TestAssets.asset3, ao3);
			Assert.Equal(TestAssets.asset4, ao4);
		}

		public void ExecStaticConstructor()
		{
			_staticCtorEvents.ForEach(TestLog.Write);
		}

		public void ExecMethod()
		{
			int ao1;
			Asset1 ao2;
			short ao3;
			IAssetIface1<Asset1> ao4;
			Asset2 ao5;

			var result = new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().Fact(TestAssets.asset1, TestAssets.asset2, TestAssets.asset3,
				TestAssets.asset4, TestAssets.asset5, ref TestAssets.asset1, ref TestAssets.asset2, ref TestAssets.asset3, ref TestAssets.asset4,
				ref TestAssets.asset5, out ao1, out ao2, out ao3, out ao4, out ao5);

			Assert.Equal(TestAssets.asset1, ao1);
			Assert.Equal(TestAssets.asset2, ao2);
			Assert.Equal(TestAssets.asset3, ao3);
			Assert.Equal(TestAssets.asset4, ao4);
			Assert.Equal(TestAssets.asset5, ao5);

			Assert.Equal(TestAssets.asset1, result.Item1);
			Assert.Equal(TestAssets.asset2, result.Item2);
			Assert.Equal(TestAssets.asset3, result.Item3);
			Assert.Equal(TestAssets.asset4, result.Item4);
			Assert.Equal(TestAssets.asset5, result.Item5);
		}

		public void ExecStaticMethod()
		{
			int ao1;
			Asset1 ao2;
			short ao3;
			IAssetIface1<Asset1> ao4;
			Asset2 ao5;

			var result = TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticMethod(TestAssets.asset1, TestAssets.asset2,
				TestAssets.asset3, TestAssets.asset4, TestAssets.asset5, ref TestAssets.asset1, ref TestAssets.asset2, ref TestAssets.asset3,
				ref TestAssets.asset4, ref TestAssets.asset5, out ao1, out ao2, out ao3, out ao4, out ao5);

			Assert.Equal(TestAssets.asset1, ao1);
			Assert.Equal(TestAssets.asset2, ao2);
			Assert.Equal(TestAssets.asset3, ao3);
			Assert.Equal(TestAssets.asset4, ao4);
			Assert.Equal(TestAssets.asset5, ao5);

			Assert.Equal(TestAssets.asset1, result.Item1);
			Assert.Equal(TestAssets.asset2, result.Item2);
			Assert.Equal(TestAssets.asset3, result.Item3);
			Assert.Equal(TestAssets.asset4, result.Item4);
			Assert.Equal(TestAssets.asset5, result.Item5);
		}

		public void ExecSetter()
		{
			new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestProperty =
				new Tuple<short, IAssetIface1<Asset1>>(TestAssets.asset3, TestAssets.asset4);
		}

		public void ExecStaticSetter()
		{
			TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticProperty =
				new Tuple<short, IAssetIface1<Asset1>>(TestAssets.asset3, TestAssets.asset4);
		}

		public void ExecGetter()
		{
			var result = new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestProperty;
		}

		public void ExecStaticGetter()
		{
			var result = TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticProperty;
		}

		public void ExecAdd()
		{
			new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestEvent += (s, e) => { };
		}

		public void ExecStaticAdd()
		{
			TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticEvent += (s, e) => { };
		}

		public void ExecRemove()
		{
			new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestEvent -= (s, e) => { };
		}

		public void ExecStaticRemove()
		{
			TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticEvent -= (s, e) => { };
		}

		public void ExecIteratorMethod()
		{
			var result = new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestIteratorMethod(TestAssets.asset1, TestAssets.asset2,
				TestAssets.asset3, TestAssets.asset4, TestAssets.asset5).Last();

			Assert.Equal(TestAssets.asset1, result.Item1);
			Assert.Equal(TestAssets.asset2, result.Item2);
			Assert.Equal(TestAssets.asset3, result.Item3);
			Assert.Equal(TestAssets.asset4, result.Item4);
			Assert.Equal(TestAssets.asset5, result.Item5);
		}

		public void ExecStaticIteratorMethod()
		{
			var result = TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticIteratorMethod(TestAssets.asset1, TestAssets.asset2,
				TestAssets.asset3, TestAssets.asset4, TestAssets.asset5).Last();

			Assert.Equal(TestAssets.asset1, result.Item1);
			Assert.Equal(TestAssets.asset2, result.Item2);
			Assert.Equal(TestAssets.asset3, result.Item3);
			Assert.Equal(TestAssets.asset4, result.Item4);
			Assert.Equal(TestAssets.asset5, result.Item5);
		}

		public void ExecAsyncTypedTaskMethod()
		{
			var result = new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestAsyncMethod1(TestAssets.asset1, TestAssets.asset2,
				TestAssets.asset3, TestAssets.asset4, TestAssets.asset5).Result;

			Assert.Equal(TestAssets.asset1, result.Item1);
			Assert.Equal(TestAssets.asset2, result.Item2);
			Assert.Equal(TestAssets.asset3, result.Item3);
			Assert.Equal(TestAssets.asset4, result.Item4);
			Assert.Equal(TestAssets.asset5, result.Item5);
		}

		public void ExecStaticAsyncTypedTaskMethod()
		{
			var result = TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticAsyncMethod1(TestAssets.asset1, TestAssets.asset2,
				TestAssets.asset3, TestAssets.asset4, TestAssets.asset5).Result;

			Assert.Equal(TestAssets.asset1, result.Item1);
			Assert.Equal(TestAssets.asset2, result.Item2);
			Assert.Equal(TestAssets.asset3, result.Item3);
			Assert.Equal(TestAssets.asset4, result.Item4);
			Assert.Equal(TestAssets.asset5, result.Item5);
		}

		public void ExecAsyncTaskMethod()
		{
			new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestAsyncMethod2(TestAssets.asset1, TestAssets.asset2, TestAssets.asset3,
				TestAssets.asset4, TestAssets.asset5).Wait();
		}

		public void ExecStaticAsyncTaskMethod()
		{
			TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticAsyncMethod2(TestAssets.asset1, TestAssets.asset2, TestAssets.asset3,
				TestAssets.asset4, TestAssets.asset5).Wait();
		}

		public void ExecAsyncVoidMethod()
		{
			new TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>().TestAsyncMethod3(TestAssets.asset1, TestAssets.asset2, TestAssets.asset3,
				TestAssets.asset4, TestAssets.asset5);

			Task.Delay(300).Wait();
		}

		public void ExecStaticAsyncVoidMethod()
		{
			TestClassWrapper<Int16>.TestClass<IAssetIface1<Asset1>>.TestStaticAsyncMethod3(TestAssets.asset1, TestAssets.asset2, TestAssets.asset3,
				TestAssets.asset4, TestAssets.asset5);

			Task.Delay(300).Wait();
		}

		public void CheckSequence(IReadOnlyList<string> orderedEvents)
		{
			var logEvents = TestLog.Log.Where(e => orderedEvents.Contains(e));

			if (!logEvents.SequenceEqual(orderedEvents))
			{
				//Assert.Fail(  string.Join(Environment.NewLine, logEvents));
				Assert.Fail( string.Join(Environment.NewLine, logEvents));
			}
		}

	}

}