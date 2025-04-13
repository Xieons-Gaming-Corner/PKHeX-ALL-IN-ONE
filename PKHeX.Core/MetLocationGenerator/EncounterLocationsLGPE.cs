using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PKHeX.Core.MetLocationGenerator
{
    public static class EncounterLocationsLGPE
    {
        public static void GenerateEncounterDataJSON(string outputPath, string errorLogPath)
        {
            try
            {
                using var errorLogger = new StreamWriter(errorLogPath, false, Encoding.UTF8);
                errorLogger.WriteLine($"[{DateTime.Now}] Starting JSON generation process for encounters in Let's Go Pikachu/Eevee.");

                var gameStrings = GameInfo.GetStrings("en");
                errorLogger.WriteLine($"[{DateTime.Now}] Game strings loaded.");

                var pt = PersonalTable.GG;
                errorLogger.WriteLine($"[{DateTime.Now}] PersonalTable for LGPE loaded.");

                var encounterData = new Dictionary<string, List<EncounterInfo>>();

                ProcessEncounterSlots(Encounters7GG.SlotsGP, "Let's Go Pikachu", encounterData, gameStrings, errorLogger);
                ProcessEncounterSlots(Encounters7GG.SlotsGE, "Let's Go Eevee", encounterData, gameStrings, errorLogger);

                ProcessStaticEncounters(Encounters7GG.Encounter_GG, "Both", encounterData, gameStrings, errorLogger);
                ProcessStaticEncounters(Encounters7GG.StaticGP, "Let's Go Pikachu", encounterData, gameStrings, errorLogger);
                ProcessStaticEncounters(Encounters7GG.StaticGE, "Let's Go Eevee", encounterData, gameStrings, errorLogger);

                ProcessTradeEncounters(Encounters7GG.TradeGift_GG, "Both", encounterData, gameStrings, errorLogger);
                ProcessTradeEncounters(Encounters7GG.TradeGift_GP, "Let's Go Pikachu", encounterData, gameStrings, errorLogger);
                ProcessTradeEncounters(Encounters7GG.TradeGift_GE, "Let's Go Eevee", encounterData, gameStrings, errorLogger);

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(encounterData, jsonOptions);

                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false)))
                {
                    streamWriter.Write(jsonString);
                }

                errorLogger.WriteLine($"[{DateTime.Now}] JSON file generated successfully without BOM at: {outputPath}");
            }
            catch (Exception ex)
            {
                using var errorLogger = new StreamWriter(errorLogPath, true, Encoding.UTF8);
                errorLogger.WriteLine($"[{DateTime.Now}] An error occurred: {ex.Message}");
                errorLogger.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private static void ProcessEncounterSlots(EncounterArea7b[] areas, string versionName, Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            foreach (var area in areas)
            {
                var locationId = area.Location;
                var locationName = gameStrings.GetLocationName(false, (ushort)locationId, 7, 7, GameVersion.GG);
                if (string.IsNullOrEmpty(locationName))
                    locationName = $"Unknown Location {locationId}";

                foreach (var slot in area.Slots)
                {
                    AddEncounterInfoWithEvolutions(encounterData, gameStrings, errorLogger, slot.Species, slot.Form,
                        locationName, locationId, slot.LevelMin, slot.LevelMax, "Wild", false, string.Empty, versionName);
                }
            }
        }

        private static void ProcessStaticEncounters(EncounterStatic7b[] encounters, string versionName, Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            foreach (var encounter in encounters)
            {
                var locationId = encounter.Location;
                var locationName = gameStrings.GetLocationName(false, (ushort)locationId, 7, 7, GameVersion.GG);
                if (string.IsNullOrEmpty(locationName))
                    locationName = $"Unknown Location {locationId}";

                AddEncounterInfoWithEvolutions(encounterData, gameStrings, errorLogger, encounter.Species, encounter.Form,
                    locationName, locationId, encounter.Level, encounter.Level, "Static",
                    encounter.Shiny == Shiny.Never,
                    encounter.FixedBall != Ball.None ? encounter.FixedBall.ToString() : string.Empty,
                    versionName);
            }
        }

        private static void ProcessTradeEncounters(EncounterTrade7b[] encounters, string versionName, Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            const int tradeLocationId = 30001;
            const string tradeLocationName = "a Link Trade (NPC)";

            foreach (var encounter in encounters)
            {
                AddEncounterInfoWithEvolutions(encounterData, gameStrings, errorLogger, encounter.Species, encounter.Form,
                    tradeLocationName, tradeLocationId, encounter.Level, encounter.Level, "Trade",
                    encounter.Shiny == Shiny.Never, encounter.FixedBall.ToString(), versionName);
            }
        }

        private static void AddEncounterInfoWithEvolutions(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, int speciesIndex, int form, string locationName, int locationId,
            int minLevel, int maxLevel, string encounterType, bool isShinyLocked, string fixedBall, string encounterVersion)
        {
            AddSingleEncounterInfo(encounterData, gameStrings, errorLogger, speciesIndex, form, locationName, locationId,
                minLevel, maxLevel, encounterType, isShinyLocked, fixedBall, encounterVersion);

            var processedForms = new HashSet<(int Species, int Form)> { ((int)speciesIndex, form) };
            ProcessEvolutions(speciesIndex, form, minLevel, locationId, locationName, isShinyLocked,
                fixedBall, encounterVersion, encounterType, encounterData, gameStrings, errorLogger, processedForms);
        }

        private static void AddSingleEncounterInfo(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, int speciesIndex, int form, string locationName, int locationId,
            int minLevel, int maxLevel, string encounterType, bool isShinyLocked, string fixedBall, string encounterVersion)
        {
            string dexNumber = speciesIndex.ToString();
            if (form > 0)
                dexNumber += $"-{form}";

            if (!encounterData.ContainsKey(dexNumber))
                encounterData[dexNumber] = new List<EncounterInfo>();

            var speciesName = gameStrings.specieslist[speciesIndex];
            if (string.IsNullOrEmpty(speciesName))
            {
                errorLogger.WriteLine($"[{DateTime.Now}] Empty species name for index {speciesIndex}. Skipping.");
                return;
            }

            encounterData[dexNumber].Add(new EncounterInfo
            {
                SpeciesName = speciesName,
                SpeciesIndex = speciesIndex,
                Form = form,
                LocationName = locationName,
                LocationId = locationId,
                MinLevel = minLevel,
                MaxLevel = maxLevel,
                EncounterType = encounterType,
                IsShinyLocked = isShinyLocked,
                FixedBall = fixedBall,
                EncounterVersion = encounterVersion
            });

            errorLogger.WriteLine($"[{DateTime.Now}] Processed encounter: {speciesName} (Dex: {dexNumber}) at {locationName} (ID: {locationId}), Levels {minLevel}-{maxLevel}, Type: {encounterType}");
        }

        private static void ProcessEvolutions(int speciesIndex, int form, int baseLevel, int locationId, string locationName,
            bool isShinyLocked, string fixedBall, string versionName, string encounterType,
            Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, HashSet<(int Species, int Form)> processedForms)
        {
            var tree = EvolutionTree.GetEvolutionTree(EntityContext.Gen7b);
            var evos = tree.Forward.GetForward((ushort)speciesIndex, (byte)form);

            foreach (var evo in evos.Span)
            {
                int evolvedSpecies = evo.Species;
                int evolvedForm = evo.Form;

                if (!processedForms.Add((evolvedSpecies, evolvedForm)))
                    continue;

                var evolvedSpeciesName = gameStrings.specieslist[evolvedSpecies];
                if (string.IsNullOrEmpty(evolvedSpeciesName))
                {
                    errorLogger.WriteLine($"[{DateTime.Now}] Empty species name for evolved index {evolvedSpecies}. Skipping.");
                    continue;
                }

                string evolvedDexNumber = evolvedSpecies.ToString();
                if (evolvedForm > 0)
                    evolvedDexNumber += $"-{evolvedForm}";

                if (!encounterData.ContainsKey(evolvedDexNumber))
                    encounterData[evolvedDexNumber] = new List<EncounterInfo>();

                int minEvoLevel = evo.LevelUp > 0 ? evo.LevelUp : (evo.Method == EvolutionType.LevelUp ? evo.Argument : 1);
                int minLevel = Math.Max(baseLevel, minEvoLevel);

                string evolvedEncounterType = $"{encounterType} (Evolved)";

                encounterData[evolvedDexNumber].Add(new EncounterInfo
                {
                    SpeciesName = evolvedSpeciesName,
                    SpeciesIndex = evolvedSpecies,
                    Form = evolvedForm,
                    LocationName = locationName,
                    LocationId = locationId,
                    MinLevel = minLevel,
                    MaxLevel = 100,
                    EncounterType = evolvedEncounterType,
                    IsShinyLocked = isShinyLocked,
                    FixedBall = fixedBall,
                    EncounterVersion = versionName
                });

                errorLogger.WriteLine($"[{DateTime.Now}] Processed evolved encounter: {evolvedSpeciesName} (Dex: {evolvedDexNumber}) at {locationName} (ID: {locationId}), Min Level {minLevel}, Type: {evolvedEncounterType}");

                ProcessEvolutions(evolvedSpecies, evolvedForm, minLevel, locationId, locationName,
                    isShinyLocked, fixedBall, versionName, encounterType, encounterData, gameStrings, errorLogger, processedForms);
            }
        }

        private class EncounterInfo
        {
            public string? SpeciesName { get; set; }
            public int SpeciesIndex { get; set; }
            public int Form { get; set; }
            public string? LocationName { get; set; }
            public int LocationId { get; set; }
            public int MinLevel { get; set; }
            public int MaxLevel { get; set; }
            public string? EncounterType { get; set; }
            public bool IsShinyLocked { get; set; }
            public string? FixedBall { get; set; }
            public string? EncounterVersion { get; set; }
        }
    }
}
