using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.UnitTests
{
	public class Tester
	{
		[Fact]
		public void Test_DoStuff()
		{
/*
			IDoer doer = new Doer();
			var stuff = doer.DoStuff<Int32>();
			Assert.NotNull(stuff);
*/
		}

		[Fact]
		public void TestWithFake_DoStuff()
		{
			Mock<ISub> subMocker = new Mock<ISub>();
			subMocker.Setup(s => s.SubStuff()).Returns<CategoryCacheEntryCollection>(null);
			ISub fakeSub = subMocker.Object;
			Mock<IDoer> doerMocker = new Mock<IDoer>();
			doerMocker.Setup(d => d.DoStuff<List<string>>(fakeSub.SubStuff())).Returns(new List<string>());
			IDoer fakeDoer = doerMocker.Object;
			var stuff = fakeDoer.DoStuff<Int32>(null);
			Assert.NotNull(stuff);
		}

		[Fact]
		public void Test_User()
		{
			Mock<ISub> subMocker = new Mock<ISub>();
			subMocker.Setup(s => s.SubStuff()).Returns(new CategoryCacheEntryCollection());
			ISub fakeSub = subMocker.Object;
			Mock<IDoer> doerMocker = new Mock<IDoer>();
			doerMocker.Setup(d => d.DoStuff<List<string>>(fakeSub.SubStuff())).Returns(new List<string>());
			IDoer fakeDoer = doerMocker.Object;
			var user = new UserX(fakeSub, fakeDoer);
			var stuff = user.UseStuff();
			Assert.NotNull(stuff);
		}
		[Fact]
		public void TestWithFake_SubStuff()
		{
			Mock<ISub> subMocker = new Mock<ISub>();
			ISub fakeSub = subMocker.Object;
			Assert.NotNull(fakeSub);
		}
	}

	public interface IDoer
	{
		T DoStuff<T>(CategoryCacheEntryCollection str) where T : new();
	}
	internal class Doer : IDoer
	{
		public T DoStuff<T>(CategoryCacheEntryCollection str) where T : new()
		{
			return new T();
		}
	}

	public interface ISub
	{
		CategoryCacheEntryCollection SubStuff();
	}

	internal class Sub
	{
		public CategoryCacheEntryCollection SubStuff()
		{
			return null;
		}
	}

	public class UserX
	{
		private ISub sub;
		private IDoer doer;

		public UserX(ISub sub, IDoer doer)
		{
			this.sub = sub;
			this.doer = doer;
		}
		public object UseStuff()
		{
			return doer.DoStuff<List<string>>(sub.SubStuff());
		}
	}
}
