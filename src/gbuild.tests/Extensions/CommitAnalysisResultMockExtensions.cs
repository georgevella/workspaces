﻿using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using GBuild.Context;
using GBuild.Models;
using Moq;

namespace gbuild.tests.Extensions
{
	public static class CommitAnalysisResultMockExtensions
	{
		public static void Setup(
			this Mock<IContextData<CommitHistoryAnalysis>> mock,
			IFixture fixture,
			int commits,
			int changedFiles,
			IDictionary<Project, int> projectChanges = null
		)
		{
			var changedProjects =
				projectChanges?.ToDictionary(x => x.Key, x => new List<Commit>(fixture.CreateMany<Commit>(x.Value))) ??
				new Dictionary<Project, List<Commit>>();


			mock.SetupGet(x => x.Data)
				.Returns(
					new CommitHistoryAnalysis(
						changedProjects,
						fixture.CreateMany<Commit>(commits),
						fixture.CreateMany<ChangedFile>(changedFiles),
						false,
						false
					)
				);
		}
	}
}