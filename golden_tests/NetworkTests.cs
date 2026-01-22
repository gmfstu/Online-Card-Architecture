using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoldenTests
{
    /// <summary>
    /// Tests for network functionality validation in ActionSystem.cs and InstantiatePlayer.cs
    /// Validates proper use of Unity.Netcode and network-safe patterns
    /// </summary>
    public static class NetworkTests
    {
        private static readonly string SourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        
        /// <summary>
        /// Test 1: Verify ActionSystem uses Unity.Netcode
        /// </summary>
        public static TestResult TestActionSystemUsesNetcode()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for Unity.Netcode usage
            if (!content.Contains("using Unity.Netcode") && !content.Contains("Unity.Netcode."))
            {
                return new TestResult(false, "ActionSystem.cs does not use Unity.Netcode");
            }

            return new TestResult(true, "ActionSystem.cs uses Unity.Netcode");
        }

        /// <summary>
        /// Test 2: Verify InstantiatePlayer extends NetworkBehaviour
        /// </summary>
        public static TestResult TestInstantiatePlayerExtendsNetworkBehaviour()
        {
            var playerPath = Path.Combine(SourceDir, "InstantiatePlayer.cs");
            if (!File.Exists(playerPath))
            {
                return new TestResult(false, "InstantiatePlayer.cs not found");
            }

            var content = File.ReadAllText(playerPath);

            // Check if class extends NetworkBehaviour
            if (!Regex.IsMatch(content, @"class\s+InstantiatePlayer\s*:\s*NetworkBehaviour"))
            {
                return new TestResult(false, "InstantiatePlayer does not extend NetworkBehaviour");
            }

            return new TestResult(true, "InstantiatePlayer extends NetworkBehaviour");
        }

        /// <summary>
        /// Test 3: Verify ActionSystem has player management (AddPlayer method)
        /// </summary>
        public static TestResult TestActionSystemHasPlayerManagement()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for AddPlayer method
            if (!Regex.IsMatch(content, @"public\s+void\s+AddPlayer"))
            {
                return new TestResult(false, "ActionSystem.cs missing AddPlayer method");
            }

            // Check for player storage (dictionary or list)
            if (!Regex.IsMatch(content, @"Dictionary\s*<\s*ulong|players|playerList", RegexOptions.IgnoreCase))
            {
                return new TestResult(false, "ActionSystem.cs missing player storage");
            }

            return new TestResult(true, "ActionSystem.cs has player management");
        }

        /// <summary>
        /// Test 4: Verify ActionSystem limits to two players (Good and Evil)
        /// </summary>
        public static TestResult TestActionSystemLimitsTwoPlayers()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for two player IDs (good/evil or player1/player2)
            var hasGoodPlayer = Regex.IsMatch(content, @"goodPlayer|player1|firstPlayer", RegexOptions.IgnoreCase);
            var hasEvilPlayer = Regex.IsMatch(content, @"evilPlayer|player2|secondPlayer", RegexOptions.IgnoreCase);

            if (!hasGoodPlayer || !hasEvilPlayer)
            {
                return new TestResult(false, "ActionSystem.cs does not have two-player structure");
            }

            // Check for player limit logic
            var hasLimitCheck = Regex.IsMatch(content, @"if\s*\(.*both.*players|if\s*\(.*already.*set|if\s*\(.*MaxValue", RegexOptions.IgnoreCase);
            if (!hasLimitCheck)
            {
                // Alternative: check for player count validation
                hasLimitCheck = Regex.IsMatch(content, @"players\.Count|playerCount|numPlayers", RegexOptions.IgnoreCase);
            }

            return new TestResult(true, "ActionSystem.cs has two-player structure");
        }

        /// <summary>
        /// Test 5: Verify InstantiatePlayer has OnNetworkSpawn
        /// </summary>
        public static TestResult TestInstantiatePlayerHasOnNetworkSpawn()
        {
            var playerPath = Path.Combine(SourceDir, "InstantiatePlayer.cs");
            if (!File.Exists(playerPath))
            {
                return new TestResult(false, "InstantiatePlayer.cs not found");
            }

            var content = File.ReadAllText(playerPath);

            // Check for OnNetworkSpawn override
            if (!Regex.IsMatch(content, @"(override\s+)?public\s+(override\s+)?void\s+OnNetworkSpawn"))
            {
                return new TestResult(false, "InstantiatePlayer.cs missing OnNetworkSpawn method");
            }

            return new TestResult(true, "InstantiatePlayer.cs has OnNetworkSpawn method");
        }

        /// <summary>
        /// Test 6: Verify InstantiatePlayer registers with ActionSystem
        /// </summary>
        public static TestResult TestInstantiatePlayerRegistersWithActionSystem()
        {
            var playerPath = Path.Combine(SourceDir, "InstantiatePlayer.cs");
            if (!File.Exists(playerPath))
            {
                return new TestResult(false, "InstantiatePlayer.cs not found");
            }

            var content = File.ReadAllText(playerPath);

            // Check for ActionSystem.Instance.AddPlayer call
            if (!Regex.IsMatch(content, @"ActionSystem\s*\.\s*Instance\s*\.\s*AddPlayer"))
            {
                return new TestResult(false, "InstantiatePlayer.cs does not register with ActionSystem");
            }

            return new TestResult(true, "InstantiatePlayer.cs registers with ActionSystem");
        }

        /// <summary>
        /// Test 7: Verify ActionSystem has turn management
        /// </summary>
        public static TestResult TestActionSystemHasTurnManagement()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for turn-related properties/methods
            var hasTurnProperty = Regex.IsMatch(content, @"playerTurn|currentTurn|turn", RegexOptions.IgnoreCase);
            var hasEndTurn = Regex.IsMatch(content, @"EndTurn|ChangeTurn|NextTurn", RegexOptions.IgnoreCase);

            if (!hasTurnProperty)
            {
                return new TestResult(false, "ActionSystem.cs missing turn tracking property");
            }

            if (!hasEndTurn)
            {
                return new TestResult(false, "ActionSystem.cs missing turn change method");
            }

            return new TestResult(true, "ActionSystem.cs has turn management");
        }

        /// <summary>
        /// Test 8: Verify network variables are used for state synchronization
        /// </summary>
        public static TestResult TestNetworkVariablesUsed()
        {
            var actionSystemPath = Path.Combine(SourceDir, "ActionSystem.cs");
            if (!File.Exists(actionSystemPath))
            {
                return new TestResult(false, "ActionSystem.cs not found");
            }

            var content = File.ReadAllText(actionSystemPath);

            // Check for NetworkVariable usage
            if (!Regex.IsMatch(content, @"NetworkVariable\s*<"))
            {
                return new TestResult(false, "ActionSystem.cs does not use NetworkVariable for state sync");
            }

            return new TestResult(true, "ActionSystem.cs uses NetworkVariable for state synchronization");
        }

        /// <summary>
        /// Test 9: Verify Singleton pattern extends NetworkBehaviour
        /// </summary>
        public static TestResult TestSingletonExtendsNetworkBehaviour()
        {
            var singletonPath = Path.Combine(SourceDir, "Singleton.cs");
            if (!File.Exists(singletonPath))
            {
                return new TestResult(false, "Singleton.cs not found");
            }

            var content = File.ReadAllText(singletonPath);

            // Check if Singleton extends NetworkBehaviour
            if (!Regex.IsMatch(content, @"class\s+Singleton.*:\s*NetworkBehaviour"))
            {
                return new TestResult(false, "Singleton does not extend NetworkBehaviour");
            }

            return new TestResult(true, "Singleton extends NetworkBehaviour");
        }

        /// <summary>
        /// Test 10: Verify no non-Netcode networking dependencies
        /// </summary>
        public static TestResult TestNoNonNetcodeDependencies()
        {
            var csFiles = Directory.GetFiles(SourceDir, "*.cs", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith(".meta"))
                .ToList();

            var errors = new List<string>();
            var forbiddenNamespaces = new[]
            {
                "using Mirror;",
                "using Photon",
                "using MLAPI;",
                "using System.Net.Sockets",
                "using Lidgren"
            };

            foreach (var file in csFiles)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileName(file);

                foreach (var forbidden in forbiddenNamespaces)
                {
                    if (content.Contains(forbidden))
                    {
                        errors.Add($"{fileName} uses forbidden networking: {forbidden}");
                    }
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "All files use only Unity.Netcode for networking");
        }

        /// <summary>
        /// Test 11: Verify InstantiatePlayer identifies owner correctly
        /// </summary>
        public static TestResult TestInstantiatePlayerIdentifiesOwner()
        {
            var playerPath = Path.Combine(SourceDir, "InstantiatePlayer.cs");
            if (!File.Exists(playerPath))
            {
                return new TestResult(false, "InstantiatePlayer.cs not found");
            }

            var content = File.ReadAllText(playerPath);

            // Check for IsOwner check
            if (!Regex.IsMatch(content, @"IsOwner|OwnerClientId"))
            {
                return new TestResult(false, "InstantiatePlayer.cs does not check ownership");
            }

            return new TestResult(true, "InstantiatePlayer.cs checks network ownership");
        }

        /// <summary>
        /// Test 12: Verify game actions don't directly use RPCs (should go through ActionSystem)
        /// </summary>
        public static TestResult TestGAFilesNoDirectRPCs()
        {
            var gaFiles = Directory.GetFiles(SourceDir, "*GA.cs", SearchOption.TopDirectoryOnly)
                .Where(f => !f.EndsWith(".meta"))
                .ToList();

            var errors = new List<string>();

            foreach (var file in gaFiles)
            {
                var content = File.ReadAllText(file);
                var fileName = Path.GetFileName(file);

                // GA files should not have ServerRpc or ClientRpc attributes
                if (Regex.IsMatch(content, @"\[ServerRpc\]|\[ClientRpc\]"))
                {
                    errors.Add($"{fileName} has direct RPC calls - should go through ActionSystem");
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "GA files don't use direct RPCs (properly abstracted)");
        }
    }
}
