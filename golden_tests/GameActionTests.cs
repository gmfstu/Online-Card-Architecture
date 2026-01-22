using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoldenTests
{
    /// <summary>
    /// Tests for GameAction architecture validation
    /// Validates that GA.cs files follow the correct patterns for instantiation and ActionSystem usage
    /// </summary>
    public static class GameActionTests
    {
        private static readonly string SourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        
        /// <summary>
        /// Test 1: Verify all *GA.cs files extend GameAction
        /// </summary>
        public static TestResult TestAllGAFilesExtendGameAction()
        {
            var gaFiles = Directory.GetFiles(SourceDir, "*GA.cs", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith(".meta"))
                .ToList();
            
            if (gaFiles.Count == 0)
            {
                return new TestResult(false, "No GA.cs files found in source directory");
            }

            var errors = new List<string>();
            foreach (var file in gaFiles)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                
                // Check if class extends GameAction
                var classPattern = $@"public\s+class\s+{Regex.Escape(fileName)}\s*:\s*GameAction";
                if (!Regex.IsMatch(content, classPattern))
                {
                    errors.Add($"{fileName} does not extend GameAction");
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, $"All {gaFiles.Count} GA files extend GameAction");
        }

        /// <summary>
        /// Test 2: Verify GA files have public constructors (can be instantiated)
        /// </summary>
        public static TestResult TestGAFilesHaveConstructors()
        {
            var gaFiles = Directory.GetFiles(SourceDir, "*GA.cs", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith(".meta"))
                .ToList();

            var errors = new List<string>();
            foreach (var file in gaFiles)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                
                // Check if class has a public constructor
                var constructorPattern = $@"public\s+{Regex.Escape(fileName)}\s*\(";
                if (!Regex.IsMatch(content, constructorPattern))
                {
                    errors.Add($"{fileName} does not have a public constructor");
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, $"All GA files have public constructors");
        }

        /// <summary>
        /// Test 3: Verify ActionSystem has Perform method that accepts GameAction
        /// </summary>
        public static TestResult TestActionSystemHasPerformMethod()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            
            // Check for Perform method that accepts GameAction
            var performPattern = @"public\s+void\s+Perform\s*\(\s*GameAction\s+\w+";
            if (!Regex.IsMatch(content, performPattern))
            {
                return new TestResult(false, "ActionSystem does not have Perform(GameAction) method");
            }

            return new TestResult(true, "ActionSystem has Perform(GameAction) method");
        }

        /// <summary>
        /// Test 4: Verify ActionSystem has reaction subscription methods
        /// </summary>
        public static TestResult TestActionSystemHasSubscriptionMethods()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            var errors = new List<string>();

            // Check for SubscribeReaction method
            if (!Regex.IsMatch(content, @"public\s+static\s+void\s+SubscribeReaction"))
            {
                errors.Add("Missing SubscribeReaction method");
            }

            // Check for AttachPerformer method
            if (!Regex.IsMatch(content, @"public\s+static\s+void\s+AttachPerformer"))
            {
                errors.Add("Missing AttachPerformer method");
            }

            // Check for AddReaction method (for direct reaction attachment)
            if (!Regex.IsMatch(content, @"public\s+void\s+AddReaction"))
            {
                errors.Add("Missing AddReaction method");
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "ActionSystem has all subscription methods");
        }

        /// <summary>
        /// Test 5: Verify GameAction base class has reaction lists
        /// </summary>
        public static TestResult TestGameActionHasReactionLists()
        {
            var gameActionPath = Path.Combine(SourceDir, "GameAction.cs");
            if (!File.Exists(gameActionPath))
            {
                return new TestResult(false, "GameAction.cs not found");
            }

            var content = File.ReadAllText(gameActionPath);
            var errors = new List<string>();

            // Check for PreReactions list (property declaration with List<GameAction> type)
            if (!Regex.IsMatch(content, @"List<GameAction>\s*>\s*PreReactions|PreReactions.*=\s*new"))
            {
                errors.Add("Missing PreReactions list");
            }

            // Check for PostReactions list (property declaration with List<GameAction> type)
            if (!Regex.IsMatch(content, @"List<GameAction>\s*>\s*PostReactions|PostReactions.*=\s*new"))
            {
                errors.Add("Missing PostReactions list");
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "GameAction has reaction lists");
        }

        /// <summary>
        /// Test 6: Verify GA files can be instantiated with maximum 3 lines of meaningful code
        /// (Constructor call should be simple - check parameters are reasonable)
        /// </summary>
        public static TestResult TestGAInstantiationSimplicity()
        {
            var gaFiles = Directory.GetFiles(SourceDir, "*GA.cs", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith(".meta"))
                .ToList();

            var errors = new List<string>();
            foreach (var file in gaFiles)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                
                // Count constructor parameters (should be reasonable for 3-line usage)
                var constructorMatch = Regex.Match(content, $@"public\s+{Regex.Escape(fileName)}\s*\(([^)]*)\)");
                if (constructorMatch.Success)
                {
                    var parameters = constructorMatch.Groups[1].Value;
                    if (!string.IsNullOrWhiteSpace(parameters))
                    {
                        var paramCount = parameters.Split(',').Length;
                        // Allow up to 5 parameters for flexibility
                        if (paramCount > 5)
                        {
                            errors.Add($"{fileName} has too many constructor parameters ({paramCount}) for simple instantiation");
                        }
                    }
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "All GA files have simple constructors suitable for 3-line usage");
        }

        /// <summary>
        /// Test 7: Verify ActionSystem Flow method exists and handles the reaction chain
        /// </summary>
        public static TestResult TestActionSystemHasFlowMethod()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            
            // Check for Flow method (coroutine)
            if (!Regex.IsMatch(content, @"(private|protected)\s+IEnumerator\s+Flow"))
            {
                return new TestResult(false, "ActionSystem missing Flow method for reaction chain");
            }

            // Check that Flow handles pre, perform, and post reactions
            var flowSection = ExtractMethod(content, "Flow");
            if (flowSection == null)
            {
                return new TestResult(false, "Could not analyze Flow method");
            }

            var hasPreReactions = flowSection.Contains("PreReaction") || flowSection.Contains("preSubs");
            var hasPostReactions = flowSection.Contains("PostReaction") || flowSection.Contains("postSubs");
            var hasPerform = flowSection.Contains("Perform") || flowSection.Contains("performer");

            if (!hasPreReactions || !hasPostReactions || !hasPerform)
            {
                return new TestResult(false, "Flow method does not handle all reaction phases (pre, perform, post)");
            }

            return new TestResult(true, "ActionSystem has proper Flow method with reaction chain");
        }

        /// <summary>
        /// Test 8: Verify ActionSystem uses isPerforming flag to prevent concurrent actions
        /// </summary>
        public static TestResult TestActionSystemPreventsConcurrentActions()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            
            // Check for isPerforming or similar lock mechanism
            var hasLock = Regex.IsMatch(content, @"is[Pp]reforming|is[Pp]erforming|_performing|isRunning", RegexOptions.IgnoreCase);
            
            if (!hasLock)
            {
                return new TestResult(false, "ActionSystem does not have a flag to prevent concurrent action execution");
            }

            // Check that Perform method checks this flag
            var performSection = ExtractMethod(content, "Perform");
            if (performSection != null)
            {
                if (!Regex.IsMatch(performSection, @"if\s*\(\s*is[Pp]reforming|if\s*\(\s*is[Pp]erforming|if\s*\(\s*_performing"))
                {
                    return new TestResult(false, "Perform method does not check the performing flag");
                }
            }

            return new TestResult(true, "ActionSystem has concurrent action prevention");
        }

        /// <summary>
        /// Test 9: Verify required GameAction types exist (from prompt requirements)
        /// </summary>
        public static TestResult TestRequiredGameActionsExist()
        {
            var requiredGAs = new[] { "DrawCardGA", "PlayCardGA", "EndTurnGA" };
            var optionalNewGAs = new[] { "DiscardCardGA", "DestroyCardGA", "ModifyCardGA", "ActivateAbilityGA", "EditCardGA", "BeginTurnGA" };
            
            var errors = new List<string>();
            var foundNew = new List<string>();

            // Check required GAs exist
            foreach (var ga in requiredGAs)
            {
                var path = Path.Combine(SourceDir, $"{ga}.cs");
                if (!File.Exists(path))
                {
                    errors.Add($"Required GameAction {ga} not found");
                }
            }

            // Check for new GAs (at least some should be implemented per the prompt)
            foreach (var ga in optionalNewGAs)
            {
                var path = Path.Combine(SourceDir, $"{ga}.cs");
                if (File.Exists(path))
                {
                    foundNew.Add(ga);
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            var message = $"All required GameActions exist. New GAs found: {(foundNew.Any() ? string.Join(", ", foundNew) : "none yet")}";
            return new TestResult(true, message);
        }

        private static string ExtractMethod(string content, string methodName)
        {
            // Simple extraction - find method start and try to match braces
            var pattern = $@"(public|private|protected)\s+\w+\s+{methodName}\s*[<\(]";
            var match = Regex.Match(content, pattern);
            if (!match.Success) return null;

            int start = match.Index;
            int braceCount = 0;
            bool foundFirst = false;
            int end = start;

            for (int i = start; i < content.Length; i++)
            {
                if (content[i] == '{')
                {
                    braceCount++;
                    foundFirst = true;
                }
                else if (content[i] == '}')
                {
                    braceCount--;
                    if (foundFirst && braceCount == 0)
                    {
                        end = i;
                        break;
                    }
                }
            }

            return content.Substring(start, end - start + 1);
        }
    }
}
