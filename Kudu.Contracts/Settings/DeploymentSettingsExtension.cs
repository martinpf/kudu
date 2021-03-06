﻿using System;
using System.Diagnostics;
using Kudu.Contracts.SourceControl;

namespace Kudu.Contracts.Settings
{
    public static class DeploymentSettingsExtension
    {
        public static readonly TimeSpan DefaultCommandIdleTimeout = TimeSpan.FromSeconds(180);
        public static readonly TimeSpan DefaultLogStreamTimeout = TimeSpan.FromSeconds(1800);
        public const TraceLevel DefaultTraceLevel = TraceLevel.Error;

        public static string GetValue(this IDeploymentSettingsManager settings, string value)
        {
            return settings.GetValue(value, onlyPerSite: false);
        }

        public static TraceLevel GetTraceLevel(this IDeploymentSettingsManager settings)
        {
            string value = settings.GetValue(SettingsKeys.TraceLevel);
            int level;
            if (Int32.TryParse(value, out level))
            {
                if (level <= (int)TraceLevel.Off)
                {
                    return TraceLevel.Off;
                }
                else if (level >= (int)TraceLevel.Verbose)
                {
                    return TraceLevel.Verbose;
                }
                else
                {
                    return (TraceLevel)level;
                }
            }

            return DeploymentSettingsExtension.DefaultTraceLevel;
        }

        public static TimeSpan GetCommandIdleTimeout(this IDeploymentSettingsManager settings)
        {
            string value = settings.GetValue(SettingsKeys.CommandIdleTimeout);
            int seconds;
            if (Int32.TryParse(value, out seconds))
            {
                return TimeSpan.FromSeconds(seconds >= 0 ? seconds : 0);
            }

            return DeploymentSettingsExtension.DefaultCommandIdleTimeout;
        }

        public static TimeSpan GetLogStreamTimeout(this IDeploymentSettingsManager settings)
        {
            string value = settings.GetValue(SettingsKeys.LogStreamTimeout);
            int seconds;
            if (Int32.TryParse(value, out seconds))
            {
                return TimeSpan.FromSeconds(seconds >= 0 ? seconds : 0);
            }

            return DeploymentSettingsExtension.DefaultLogStreamTimeout;
        }

        public static string GetGitUsername(this IDeploymentSettingsManager settings)
        {
            string value = settings.GetValue(SettingsKeys.GitUsername);
            return !String.IsNullOrEmpty(value) ? value : "unknown";
        }

        public static string GetGitEmail(this IDeploymentSettingsManager settings)
        {
            string value = settings.GetValue(SettingsKeys.GitEmail);
            return !String.IsNullOrEmpty(value) ? value : "unknown";
        }

        public static string GetBranch(this IDeploymentSettingsManager settings)
        {
            string value = settings.GetValue(SettingsKeys.Branch, onlyPerSite: true);
            if (!String.IsNullOrEmpty(value))
            {
                return value;
            }

            return settings.GetValue(SettingsKeys.DeploymentBranch);
        }

        public static void SetBranch(this IDeploymentSettingsManager settings, string branchName)
        {
            // If we're updating branch, clear out the legacy value first
            settings.DeleteValue(SettingsKeys.Branch);
            settings.SetValue(SettingsKeys.DeploymentBranch, branchName);
        }

        // allow /deploy endpoint
        public static bool IsScmEnabled(this IDeploymentSettingsManager settings)
        {
            string scmType = settings.GetValue(SettingsKeys.ScmType);
            return scmType != ScmType.None && scmType != ScmType.Tfs;
        }
    }
}