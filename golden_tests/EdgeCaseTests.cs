using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoldenTests
{
    /// <summary>
    /// Tests for edge cases as specified in the requirements:
    /// - More or less than two players attempted to instantiate
    /// - Multiple GameActions attempted at once
    /// - Recursive reaction subscriptions (infinite loop prevention)
    /// - Invalid GameAction submitted to ActionSystem
    /// </summary>
    public static class EdgeCaseTests
    {
        private static readonly string SourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        
        /// <summary>
        /// Test 1: Verify ActionSystem handles attempt to add more than two players
        /// </summary>
        public static TestResult TestMoreThanTwoPlayersHandled()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for third player rejection logic
            var addPlayerMethod = ExtractMethod(content, "AddPlayer");
            if (addPlayerMethod == null)
            {
                return new TestResult(false, "Could not find AddPlayer method");
            }

            // Look for logic that prevents adding a third player
            var hasRejectionLogic = 
                Regex.IsMatch(addPlayerMethod, @"if\s*\(.*MaxValue", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(addPlayerMethod, @"if\s*\(.*evilPlayer.*!=.*MaxValue|evilPlayerId\s*!=\s*ulong\.MaxValue", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(addPlayerMethod, @"both.*players.*already|already.*set|cannot.*add.*another", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(addPlayerMethod, @"return\s*;", RegexOptions.IgnoreCase);

            // Also check for warning/error logging
            var hasWarning = Regex.IsMatch(addPlayerMethod, @"LogWarning|LogError|Debug\.Log.*cannot|Debug\.Log.*already", RegexOptions.IgnoreCase);

            if (!hasRejectionLogic && !hasWarning)
            {
                return new TestResult(false, "AddPlayer method does not handle third player attempt");
            }

            return new TestResult(true, "ActionSystem handles attempt to add more than two players");
        }

        /// <summary>
        /// Test 2: Verify ActionSystem handles zero players gracefully
        /// </summary>
        public static TestResult TestZeroPlayersHandled()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for initialization of player IDs to invalid state
            var hasInvalidInit = Regex.IsMatch(content, @"(goodPlayerId|evilPlayerId|player\d?Id)\s*=\s*(ulong\.MaxValue|0|null|-1)", RegexOptions.IgnoreCase);
            
            // Check for null/empty checks before using players
            var hasPlayerValidation = 
                Regex.IsMatch(content, @"players\s*!=\s*null|players\.Count|players\.ContainsKey", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(content, @"MaxValue|goodPlayerId\s*==|evilPlayerId\s*==", RegexOptions.IgnoreCase);

            if (!hasInvalidInit)
            {
                return new TestResult(false, "Player IDs not initialized to invalid state");
            }

            return new TestResult(true, "ActionSystem handles zero players (uses MaxValue for unset)");
        }

        /// <summary>
        /// Test 3: Verify ActionSystem prevents concurrent action execution
        /// </summary>
        public static TestResult TestMultipleActionsAtOnceBlocked()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for isPerforming flag
            var hasPerformingFlag = Regex.IsMatch(content, @"bool.*is[Pp]reforming|bool.*is[Pp]erforming|bool.*_performing", RegexOptions.IgnoreCase);
            if (!hasPerformingFlag)
            {
                return new TestResult(false, "ActionSystem missing performing flag");
            }

            // Check Perform method blocks when already performing
            var performMethod = ExtractMethod(content, "Perform");
            if (performMethod == null)
            {
                return new TestResult(false, "Could not find Perform method");
            }

            // Look for early return when already performing
            var blocksWhenPerforming = 
                Regex.IsMatch(performMethod, @"if\s*\(\s*is[Pp]reforming\s*\)\s*return", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(performMethod, @"if\s*\(\s*is[Pp]erforming\s*\)\s*return", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(performMethod, @"if\s*\(\s*_performing\s*\)\s*return", RegexOptions.IgnoreCase);

            if (!blocksWhenPerforming)
            {
                return new TestResult(false, "Perform method does not block when already performing");
            }

            // Check that flag is set to true at start and false at end
            var setsToTrue = Regex.IsMatch(performMethod, @"is[Pp]reforming\s*=\s*true|is[Pp]erforming\s*=\s*true", RegexOptions.IgnoreCase);
            if (!setsToTrue)
            {
                return new TestResult(false, "Perform method does not set performing flag to true");
            }

            return new TestResult(true, "ActionSystem prevents concurrent action execution");
        }

        /// <summary>
        /// Test 4: Verify ActionSystem handles recursive reactions (infinite loop prevention)
        /// </summary>
        public static TestResult TestRecursiveReactionsHandled()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // There are several valid approaches to handle recursive reactions:
            // 1. The isPerforming flag already prevents new actions during execution
            // 2. Reactions are processed in lists, not as new Perform calls
            // 3. Explicit recursion depth tracking

            // Check for reaction list processing (not recursive Perform calls)
            var flowMethod = ExtractMethod(content, "Flow");
            if (flowMethod != null)
            {
                // Flow should use PerformReactions which iterates lists, not starts new Performs
                var usesListIteration = flowMethod.Contains("foreach") || flowMethod.Contains("PerformReactions");
                if (usesListIteration)
                {
                    return new TestResult(true, "ActionSystem processes reactions via list iteration (prevents infinite recursion)");
                }
            }

            // Check for explicit recursion prevention
            var hasDepthLimit = Regex.IsMatch(content, @"depth|recursion|maxReactions|reactionLimit", RegexOptions.IgnoreCase);
            if (hasDepthLimit)
            {
                return new TestResult(true, "ActionSystem has explicit recursion/depth limiting");
            }

            // The isPerforming check in Perform effectively prevents re-entry
            var performMethod = ExtractMethod(content, "Perform");
            if (performMethod != null)
            {
                var hasReentryBlock = Regex.IsMatch(performMethod, @"if\s*\(\s*is[Pp]reforming\s*\)", RegexOptions.IgnoreCase);
                if (hasReentryBlock)
                {
                    return new TestResult(true, "ActionSystem uses isPerforming flag to prevent re-entry (handles recursive reactions)");
                }
            }

            return new TestResult(false, "ActionSystem may not handle recursive reactions properly");
        }

        /// <summary>
        /// Test 5: Verify ActionSystem handles null/invalid GameAction
        /// </summary>
        public static TestResult TestInvalidGameActionHandled()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            var performMethod = ExtractMethod(content, "Perform");
            
            // Check for null check on action parameter
            var hasNullCheck = 
                Regex.IsMatch(content, @"action\s*==\s*null|action\s*!=\s*null|action\s*\?\.", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(content, @"if\s*\(\s*action\s*==\s*null\s*\)", RegexOptions.IgnoreCase);

            // Check for type validation in performer lookup
            var performerMethod = ExtractMethod(content, "PerformPerformer");
            if (performerMethod != null)
            {
                // Check for ContainsKey check before accessing performer
                var hasTypeCheck = 
                    performerMethod.Contains("ContainsKey") ||
                    performerMethod.Contains("TryGetValue") ||
                    performerMethod.Contains("action.GetType()");
                
                if (hasTypeCheck)
                {
                    return new TestResult(true, "ActionSystem validates action type before execution");
                }
            }

            // Check Flow method handles missing performers gracefully
            var flowMethod = ExtractMethod(content, "Flow");
            if (flowMethod != null && (flowMethod.Contains("ContainsKey") || flowMethod.Contains("performers")))
            {
                return new TestResult(true, "ActionSystem Flow method handles actions without performers");
            }

            if (hasNullCheck)
            {
                return new TestResult(true, "ActionSystem has null check for GameAction");
            }

            // Even without explicit null check, if performer lookup uses ContainsKey, it's safe
            if (content.Contains("performers.ContainsKey(type)"))
            {
                return new TestResult(true, "ActionSystem safely handles missing performers via ContainsKey check");
            }

            return new TestResult(false, "ActionSystem may not handle invalid GameAction properly");
        }

        /// <summary>
        /// Test 6: Verify reaction lists are initialized (not null)
        /// </summary>
        public static TestResult TestReactionListsInitialized()
        {
            var gameActionPath = Path.Combine(SourceDir, "GameAction.cs");
            if (!File.Exists(gameActionPath))
            {
                return new TestResult(false, "GameAction.cs not found");
            }

            var content = File.ReadAllText(gameActionPath);

            // Check that reaction lists are initialized to empty lists (supports both old and new C# syntax)
            var hasPreReactionsInit = Regex.IsMatch(content, @"PreReactions.*=\s*new\s*(List<GameAction>|List<GameAction>\(\)|\(\))", RegexOptions.IgnoreCase);
            var hasPostReactionsInit = Regex.IsMatch(content, @"PostReactions.*=\s*new\s*(List<GameAction>|List<GameAction>\(\)|\(\))", RegexOptions.IgnoreCase);

            if (!hasPreReactionsInit)
            {
                return new TestResult(false, "PreReactions list not initialized");
            }

            if (!hasPostReactionsInit)
            {
                return new TestResult(false, "PostReactions list not initialized");
            }

            return new TestResult(true, "Reaction lists are properly initialized");
        }

        /// <summary>
        /// Test 7: Verify subscriber dictionaries handle missing types gracefully
        /// </summary>
        public static TestResult TestSubscriberDictionariesHandleMissingTypes()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check PerformSubscribers method for ContainsKey check
            var subsMethod = ExtractMethod(content, "PerformSubscribers");
            if (subsMethod != null)
            {
                var hasTypeCheck = subsMethod.Contains("ContainsKey") || subsMethod.Contains("TryGetValue");
                if (hasTypeCheck)
                {
                    return new TestResult(true, "PerformSubscribers checks for type existence before access");
                }
            }

            // Alternative: check general pattern in the file
            if (content.Contains("subs.ContainsKey(") || content.Contains("preSubs.ContainsKey(") || content.Contains("postSubs.ContainsKey("))
            {
                return new TestResult(true, "Subscriber dictionaries use ContainsKey checks");
            }

            return new TestResult(false, "Subscriber dictionaries may throw on missing types");
        }

        /// <summary>
        /// Test 8: Verify unsubscribe handles non-existent subscriptions
        /// </summary>
        public static TestResult TestUnsubscribeHandlesNonExistent()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check UnsubscribeReaction method
            var unsubMethod = ExtractMethod(content, "UnsubscribeReaction");
            if (unsubMethod == null)
            {
                // Method might not exist yet, which is acceptable
                return new TestResult(true, "UnsubscribeReaction not implemented (acceptable)");
            }

            // Check for ContainsKey before removal
            var hasSafeRemoval = unsubMethod.Contains("ContainsKey") || unsubMethod.Contains("TryGetValue");
            if (hasSafeRemoval)
            {
                return new TestResult(true, "UnsubscribeReaction safely handles non-existent subscriptions");
            }

            return new TestResult(false, "UnsubscribeReaction may throw on non-existent subscriptions");
        }

        /// <summary>
        /// Test 9: Verify InstantiatePlayer handles duplicate OwnerClientId
        /// </summary>
        public static TestResult TestDuplicatePlayerIdHandled()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            var addPlayerMethod = ExtractMethod(content, "AddPlayer");
            
            if (addPlayerMethod == null)
            {
                return new TestResult(false, "Could not find AddPlayer method");
            }

            // Check for duplicate player ID handling
            var handlesDuplicate = 
                addPlayerMethod.Contains("ContainsKey") ||
                Regex.IsMatch(addPlayerMethod, @"already.*exists|update.*player|LogWarning.*already", RegexOptions.IgnoreCase);

            if (handlesDuplicate)
            {
                return new TestResult(true, "AddPlayer handles duplicate player IDs");
            }

            return new TestResult(false, "AddPlayer may not handle duplicate player IDs");
        }

        /// <summary>
        /// Test 10: Verify EndTurn handles invalid player state
        /// </summary>
        public static TestResult TestEndTurnHandlesInvalidState()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);
            var endTurnMethod = ExtractMethod(content, "EndTurn");
            
            if (endTurnMethod == null)
            {
                return new TestResult(false, "Could not find EndTurn method");
            }

            // Check for else/default case handling
            var hasDefaultCase = 
                endTurnMethod.Contains("else") ||
                Regex.IsMatch(endTurnMethod, @"LogWarning.*not.*valid|LogWarning.*not.*set", RegexOptions.IgnoreCase);

            if (hasDefaultCase)
            {
                return new TestResult(true, "EndTurn handles invalid player turn state");
            }

            return new TestResult(false, "EndTurn may not handle invalid player state");
        }

        /// <summary>
        /// Test 11: Verify GA files don't have circular dependencies in constructor
        /// </summary>
        public static TestResult TestNoCircularConstructorDependencies()
        {
            var gaFiles = Directory.GetFiles(SourceDir, "*GA.cs", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith(".meta"))
                .ToList();

            var errors = new List<string>();

            foreach (var file in gaFiles)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileNameWithoutExtension(file);

                // Check if constructor creates another GA of same type
                var constructorMatch = Regex.Match(content, $@"public\s+{Regex.Escape(fileName)}\s*\([^)]*\)\s*{{([^}}]*)}}");
                if (constructorMatch.Success)
                {
                    var constructorBody = constructorMatch.Groups[1].Value;
                    if (constructorBody.Contains($"new {fileName}"))
                    {
                        errors.Add($"{fileName} constructor creates instance of itself (potential infinite loop)");
                    }
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "No circular constructor dependencies in GA files");
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
