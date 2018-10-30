﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoMapper.Internal;
using DasBlog.Core.Common;
using newtelligence.DasBlog.Util;

namespace DasBlog.Tests.UnitTests
{
	public class UnitTestsConstants
	{
		private static readonly DirectoryInfo root = new DirectoryInfo(DasBlog.Tests.Support.Common.Utils.GetProjectBinaryDirectory());

		public static string TestContentLocation { get { return new DirectoryInfo(Path.Combine(root.Parent.FullName, "netcoreapp2.1/TestContent")).FullName; } }

		public static string TestLoggingLocation { get { return new DirectoryInfo(Path.Combine(root.Parent.FullName, "netcoreapp2.1/logs")).FullName; } }
	}
}
