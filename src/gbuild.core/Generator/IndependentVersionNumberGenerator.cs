﻿using System.Linq;
using GBuild.Configuration;
using GBuild.Context;
using GBuild.Models;
using GBuild.Variables;

namespace GBuild.Generator
{
	/// <summary>
	///     Version number generator for a branch that is the 'development' counter part of an other branch designated as the
	///     'production' branch.
	/// </summary>
	public class IndependentVersionNumberGenerator : IVersionNumberGenerator
	{
		private readonly IWorkspaceConfiguration _workspaceConfiguration;
		private readonly Releases _releases;
		private readonly IVariableRenderer _variableRenderer;
		private readonly CommitHistoryAnalysis _commitAnalysis;
		private readonly Workspace _workspace;

		public IndependentVersionNumberGenerator(
			IWorkspaceConfiguration workspaceConfiguration,
			IContextData<CommitHistoryAnalysis> commitAnalysis,
			IContextData<Workspace> workspace,
			IContextData<Releases> workspaceReleaseInformation,
			IVariableRenderer variableRenderer
			)
		{
			_workspaceConfiguration = workspaceConfiguration;
			_releases = workspaceReleaseInformation.Data;
			_variableRenderer = variableRenderer;
			_commitAnalysis = commitAnalysis.Data;
			_workspace = workspace.Data;
		}

		public SemanticVersion GetVersion(Project project)
		{
			var branchVersioningStrategyModel = _workspace.BranchVersioningStrategy;

			var baseVersion = _workspaceConfiguration.StartingVersion;

			// TODO: check for any release branches, that are in progress, and run a commit analysis on latest release branch and version generation logic
			
			if (_releases.PastReleases.Any())
			{
				var release = _releases.PastReleases.First();

				if (_commitAnalysis.ChangedProjects.TryGetValue(project, out var changedProject))
				{
					if (changedProject.HasBreakingChanges)
					{
						// TODO: make this configurable from branching strategy
						baseVersion = release.VersionNumbers[project].IncrementMajor();
					}
					else
					{
						baseVersion = release.VersionNumbers[project].IncrementMinor();
					}
				}
				
				// we don't touch the version if there are no changes for this project, we simply point to the latest release.
			}

			return branchVersioningStrategyModel.Generate(baseVersion, project);
		}
	}
}