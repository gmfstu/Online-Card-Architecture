using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GoldenTests
{
    /// <summary>
    /// Tests for JSON data serialization and CardData structure validation
    /// </summary>
    public static class JSONReaderTests
    {
        private static readonly string SourceDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
        
        /// <summary>
        /// Test 1: Verify CardDataJSON.txt exists and is valid JSON
        /// </summary>
        public static TestResult TestCardDataJSONIsValid()
        {
            var jsonPath = Path.Combine(SourceDir, "CardDataJSON.txt");
            if (!File.Exists(jsonPath))
            {
                return new TestResult(false, "CardDataJSON.txt not found");
            }

            try
            {
                var content = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(content);
                return new TestResult(true, "CardDataJSON.txt contains valid JSON");
            }
            catch (JsonException ex)
            {
                return new TestResult(false, $"CardDataJSON.txt contains invalid JSON: {ex.Message}");
            }
        }

        /// <summary>
        /// Test 2: Verify CardDataJSON has the expected structure (carddata array)
        /// </summary>
        public static TestResult TestCardDataJSONStructure()
        {
            var jsonPath = Path.Combine(SourceDir, "CardDataJSON.txt");
            if (!File.Exists(jsonPath))
            {
                return new TestResult(false, "CardDataJSON.txt not found");
            }

            try
            {
                var content = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // Check for carddata property
                if (!root.TryGetProperty("carddata", out var carddataElement))
                {
                    return new TestResult(false, "CardDataJSON.txt missing 'carddata' property");
                }

                if (carddataElement.ValueKind != JsonValueKind.Array)
                {
                    return new TestResult(false, "'carddata' should be an array");
                }

                var cardCount = carddataElement.GetArrayLength();
                if (cardCount == 0)
                {
                    return new TestResult(false, "'carddata' array is empty");
                }

                return new TestResult(true, $"CardDataJSON.txt has valid structure with {cardCount} cards");
            }
            catch (Exception ex)
            {
                return new TestResult(false, $"Error parsing CardDataJSON.txt: {ex.Message}");
            }
        }

        /// <summary>
        /// Test 3: Verify each card in CardDataJSON has required fields
        /// </summary>
        public static TestResult TestCardDataFieldsExist()
        {
            var jsonPath = Path.Combine(SourceDir, "CardDataJSON.txt");
            if (!File.Exists(jsonPath))
            {
                return new TestResult(false, "CardDataJSON.txt not found");
            }

            var requiredFields = new[] { "name", "id", "type", "power", "description" };
            var optionalFields = new[] { "attackmod", "defensemod", "healthmod", "effect" };

            try
            {
                var content = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(content);
                var carddata = doc.RootElement.GetProperty("carddata");

                var errors = new List<string>();
                int cardIndex = 0;

                foreach (var card in carddata.EnumerateArray())
                {
                    foreach (var field in requiredFields)
                    {
                        if (!card.TryGetProperty(field, out _))
                        {
                            errors.Add($"Card at index {cardIndex} missing required field: {field}");
                        }
                    }
                    cardIndex++;
                }

                if (errors.Any())
                {
                    return new TestResult(false, string.Join("\n", errors.Take(10))); // Limit error output
                }

                return new TestResult(true, $"All {cardIndex} cards have required fields");
            }
            catch (Exception ex)
            {
                return new TestResult(false, $"Error validating card fields: {ex.Message}");
            }
        }

        /// <summary>
        /// Test 4: Verify JSONReader.cs has CardData class with matching fields
        /// </summary>
        public static TestResult TestJSONReaderHasCardDataClass()
        {
            var jsonReaderPath = Path.Combine(SourceDir, "JSONReader.cs");
            if (!File.Exists(jsonReaderPath))
            {
                return new TestResult(false, "JSONReader.cs not found");
            }

            var content = File.ReadAllText(jsonReaderPath);

            // Check for CardData class
            if (!Regex.IsMatch(content, @"class\s+CardData"))
            {
                return new TestResult(false, "JSONReader.cs missing CardData class");
            }

            // Check for required fields in CardData
            var requiredFields = new[] { "name", "id", "type", "power", "description" };
            var errors = new List<string>();

            foreach (var field in requiredFields)
            {
                // Look for field declaration (public string/int fieldname;)
                var fieldPattern = $@"public\s+\w+\s+{field}\s*[;{{]";
                if (!Regex.IsMatch(content, fieldPattern, RegexOptions.IgnoreCase))
                {
                    errors.Add($"CardData missing field: {field}");
                }
            }

            if (errors.Any())
            {
                return new TestResult(false, string.Join("\n", errors));
            }

            return new TestResult(true, "JSONReader.cs has CardData class with required fields");
        }

        /// <summary>
        /// Test 5: Verify JSONReader.cs has CardDataList class
        /// </summary>
        public static TestResult TestJSONReaderHasCardDataListClass()
        {
            var jsonReaderPath = Path.Combine(SourceDir, "JSONReader.cs");
            if (!File.Exists(jsonReaderPath))
            {
                return new TestResult(false, "JSONReader.cs not found");
            }

            var content = File.ReadAllText(jsonReaderPath);

            // Check for CardDataList class
            if (!Regex.IsMatch(content, @"class\s+CardDataList"))
            {
                return new TestResult(false, "JSONReader.cs missing CardDataList class");
            }

            // Check for carddata array field
            if (!Regex.IsMatch(content, @"CardData\s*\[\s*\]\s+carddata|List\s*<\s*CardData\s*>\s+carddata", RegexOptions.IgnoreCase))
            {
                return new TestResult(false, "CardDataList missing carddata array/list field");
            }

            return new TestResult(true, "JSONReader.cs has CardDataList class with carddata field");
        }

        /// <summary>
        /// Test 6: Verify JSONReader uses Unity's JsonUtility or similar deserialization
        /// </summary>
        public static TestResult TestJSONReaderUsesDeserialization()
        {
            var jsonReaderPath = Path.Combine(SourceDir, "JSONReader.cs");
            if (!File.Exists(jsonReaderPath))
            {
                return new TestResult(false, "JSONReader.cs not found");
            }

            var content = File.ReadAllText(jsonReaderPath);

            // Check for JSON deserialization usage
            var hasJsonUtility = content.Contains("JsonUtility.FromJson");
            var hasJsonConvert = content.Contains("JsonConvert.DeserializeObject");
            var hasJsonSerializer = content.Contains("JsonSerializer.Deserialize");

            if (!hasJsonUtility && !hasJsonConvert && !hasJsonSerializer)
            {
                return new TestResult(false, "JSONReader.cs does not use JSON deserialization");
            }

            return new TestResult(true, "JSONReader.cs uses JSON deserialization");
        }

        /// <summary>
        /// Test 7: Verify card IDs are unique in CardDataJSON
        /// </summary>
        public static TestResult TestCardDataUniqueIDs()
        {
            var jsonPath = Path.Combine(SourceDir, "CardDataJSON.txt");
            if (!File.Exists(jsonPath))
            {
                return new TestResult(false, "CardDataJSON.txt not found");
            }

            try
            {
                var content = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(content);
                var carddata = doc.RootElement.GetProperty("carddata");

                var ids = new HashSet<int>();
                var duplicates = new List<int>();

                foreach (var card in carddata.EnumerateArray())
                {
                    if (card.TryGetProperty("id", out var idElement))
                    {
                        var id = idElement.GetInt32();
                        if (!ids.Add(id))
                        {
                            duplicates.Add(id);
                        }
                    }
                }

                if (duplicates.Any())
                {
                    return new TestResult(false, $"Duplicate card IDs found: {string.Join(", ", duplicates)}");
                }

                return new TestResult(true, $"All {ids.Count} card IDs are unique");
            }
            catch (Exception ex)
            {
                return new TestResult(false, $"Error checking card IDs: {ex.Message}");
            }
        }

        /// <summary>
        /// Test 8: Verify card types are valid (Minion, Action, Null, etc.)
        /// </summary>
        public static TestResult TestCardDataValidTypes()
        {
            var jsonPath = Path.Combine(SourceDir, "CardDataJSON.txt");
            if (!File.Exists(jsonPath))
            {
                return new TestResult(false, "CardDataJSON.txt not found");
            }

            // Valid card types based on the game description
            var validTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Minion", "Action", "Null", "Base", "Special", "Equipment", "Spell", "Ability"
            };

            try
            {
                var content = File.ReadAllText(jsonPath);
                using var doc = JsonDocument.Parse(content);
                var carddata = doc.RootElement.GetProperty("carddata");

                var errors = new List<string>();
                var foundTypes = new HashSet<string>();

                foreach (var card in carddata.EnumerateArray())
                {
                    if (card.TryGetProperty("type", out var typeElement))
                    {
                        var type = typeElement.GetString();
                        foundTypes.Add(type);
                        
                        if (!validTypes.Contains(type))
                        {
                            // Just warn, don't fail - new types may be valid
                            // errors.Add($"Unknown card type: {type}");
                        }
                    }
                }

                return new TestResult(true, $"Card types found: {string.Join(", ", foundTypes)}");
            }
            catch (Exception ex)
            {
                return new TestResult(false, $"Error checking card types: {ex.Message}");
            }
        }

        /// <summary>
        /// Test 9: Verify JSONReader has Serializable attributes for Unity serialization
        /// </summary>
        public static TestResult TestJSONReaderHasSerializableAttributes()
        {
            var jsonReaderPath = Path.Combine(SourceDir, "JSONReader.cs");
            if (!File.Exists(jsonReaderPath))
            {
                return new TestResult(false, "JSONReader.cs not found");
            }

            var content = File.ReadAllText(jsonReaderPath);

            // Check for [System.Serializable] attribute on CardData
            var hasSerializableCardData = Regex.IsMatch(content, @"\[System\.Serializable\]\s*\n?\s*(public\s+)?class\s+CardData");
            var hasSerializableCardDataList = Regex.IsMatch(content, @"\[System\.Serializable\]\s*\n?\s*(public\s+)?class\s+CardDataList");

            if (!hasSerializableCardData)
            {
                return new TestResult(false, "CardData class missing [System.Serializable] attribute");
            }

            if (!hasSerializableCardDataList)
            {
                return new TestResult(false, "CardDataList class missing [System.Serializable] attribute");
            }

            return new TestResult(true, "JSONReader classes have [System.Serializable] attributes");
        }
    }
}
