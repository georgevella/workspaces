﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GBuild.Configuration.Models;

namespace GBuild.Models
{
	public class CommitHistoryAnalysis
	{
		public CommitHistoryAnalysis(
			IDictionary<Project, List<Commit>> changedProjects,
			IEnumerable<Commit> commits,
			IEnumerable<ChangedFile> changedFiles,
			bool hasBreakingChanges,
			bool hasNewFeatures
			)
		{
			ChangedProjects = new ReadOnlyDictionary<Project, List<Commit>>(changedProjects);
			Commits = commits.ToList();
			ChangedFiles = changedFiles.ToList();
			HasBreakingChanges = hasBreakingChanges;
			HasNewFeatures = hasNewFeatures;
		}

		/// <summary>
		///     Projects changed in this branch.
		/// </summary>
		public IReadOnlyDictionary<Project, List<Commit>> ChangedProjects { get; }

		public IReadOnlyCollection<ChangedFile> ChangedFiles { get; }

		public IReadOnlyCollection<Commit> Commits { get; }

		public bool HasBreakingChanges { get; }

		public bool HasNewFeatures { get; }
	}
}