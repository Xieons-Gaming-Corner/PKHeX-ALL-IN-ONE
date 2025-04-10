using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace PKHeX.Core.MetLocationGenerator
{
    public static class EncounterLocationsSWSH
    {
        private const ushort MaxLair = 244; // Max Lair location ID

        public static void GenerateEncounterDataJSON(string outputPath, string errorLogPath)
        {
            try
            {
                using var errorLogger = new StreamWriter(errorLogPath, false, Encoding.UTF8);
                errorLogger.WriteLine($"[{DateTime.Now}] Starting JSON generation process for encounters in Sword/Shield.");

                var gameStrings = GameInfo.GetStrings("en");
                var encounterData = new Dictionary<string, List<EncounterInfo>>();

                // Process regular encounter slots
                ProcessEncounterSlots(Encounters8.SlotsSW_Symbol, encounterData, gameStrings, errorLogger, "Sword Symbol");
                ProcessEncounterSlots(Encounters8.SlotsSW_Hidden, encounterData, gameStrings, errorLogger, "Sword Hidden");
                ProcessEncounterSlots(Encounters8.SlotsSH_Symbol, encounterData, gameStrings, errorLogger, "Shield Symbol");
                ProcessEncounterSlots(Encounters8.SlotsSH_Hidden, encounterData, gameStrings, errorLogger, "Shield Hidden");

                // Process static encounters
                ProcessStaticEncounters(Encounters8.StaticSWSH, "Both", encounterData, gameStrings, errorLogger);
                ProcessStaticEncounters(Encounters8.StaticSW, "Sword", encounterData, gameStrings, errorLogger);
                ProcessStaticEncounters(Encounters8.StaticSH, "Shield", encounterData, gameStrings, errorLogger);

                // Process egg met locations
                ProcessEggMetLocations(encounterData, gameStrings, errorLogger);

                // Process Den encounters
                ProcessDenEncounters(encounterData, gameStrings, errorLogger);

                // Process Max Lair encounters
                ProcessMaxLairEncounters(encounterData, gameStrings, errorLogger);

                var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(encounterData, jsonOptions);

                File.WriteAllText(outputPath, jsonString, new UTF8Encoding(false));

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

        private static void ProcessEncounterSlots(EncounterArea8[] areas, Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger, string slotType)
        {
            foreach (var area in areas)
            {
                var locationName = gameStrings.GetLocationName(false, (ushort)area.Location, 8, 8, GameVersion.SWSH);

                foreach (var slot in area.Slots)
                {
                    bool canGigantamax = Gigantamax.CanToggle(slot.Species, slot.Form);

                    AddEncounterInfo(encounterData, gameStrings, errorLogger, slot.Species, slot.Form, locationName, area.Location, slot.LevelMin, slot.LevelMax, $"Wild {slotType}", false, false, string.Empty, "Both", canGigantamax);
                }
            }
        }

        private static void ProcessEggMetLocations(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            const int eggMetLocationId = 60002;
            const string locationName = "a Nursery Worker";

            errorLogger.WriteLine($"[{DateTime.Now}] Processing egg met locations with location ID: {eggMetLocationId} ({locationName})");

            var pt = PersonalTable.SWSH;

            // Process all breedable Pokémon
            for (ushort species = 1; species < pt.MaxSpeciesID; species++)
            {
                var personalInfo = pt.GetFormEntry(species, 0);
                if (personalInfo is null || !personalInfo.IsPresentInGame)
                    continue;

                // Skip species that can't breed (Undiscovered egg group)
                if (personalInfo.EggGroup1 == 15 || personalInfo.EggGroup2 == 15)
                    continue;

                // For each valid form
                byte formCount = personalInfo.FormCount;
                for (byte form = 0; form < formCount; form++)
                {
                    var formInfo = pt.GetFormEntry(species, form);
                    if (formInfo is null || !formInfo.IsPresentInGame)
                        continue;

                    // Skip forms that can't breed
                    if (formInfo.EggGroup1 == 15 || formInfo.EggGroup2 == 15)
                        continue;

                    bool canGigantamax = Gigantamax.CanToggle(species, form);

                    // Eggs hatch at level 1
                    AddSingleEncounterInfo(
                        encounterData,
                        gameStrings,
                        errorLogger,
                        species,
                        form,
                        locationName,
                        eggMetLocationId,
                        1, // Min level
                        1, // Max level
                        "Egg", // Encounter type
                        false, // Eggs are never shiny locked
                        true, // Eggs are considered "gift" Pokémon
                        string.Empty, // No fixed ball for eggs
                        "Both", // Available in both versions
                        canGigantamax
                    );
                }
            }
        }

        private static void ProcessStaticEncounters(EncounterStatic8[] encounters, string versionName, Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            foreach (var encounter in encounters)
            {
                var locationName = gameStrings.GetLocationName(false, (ushort)encounter.Location, 8, 8, GameVersion.SWSH);
                bool canGigantamax = Gigantamax.CanToggle(encounter.Species, encounter.Form) || encounter.CanGigantamax;

                // Use string.Empty instead of null for the fixedBall parameter
                AddEncounterInfo(encounterData, gameStrings, errorLogger, encounter.Species, encounter.Form, locationName, encounter.Location, encounter.Level, encounter.Level, "Static", encounter.Shiny == Shiny.Never, encounter.Gift, encounter.FixedBall != Ball.None ? encounter.FixedBall.ToString() : string.Empty, versionName, canGigantamax);
            }
        }

        private static void ProcessDenEncounters(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            const int denLocationId = Encounters8Nest.SharedNest; // 162 (Pokemon Den)
            var locationName = gameStrings.GetLocationName(false, (ushort)denLocationId, 8, 8, GameVersion.SWSH);

            errorLogger.WriteLine($"[{DateTime.Now}] Processing Pokémon Den encounters with location ID: {denLocationId} ({locationName})");

            // Process all den encounters with a consistent "Max Raid" encounter type
            ProcessNestEncounters(Encounters8Nest.Nest_SW, "Sword", encounterData, gameStrings, errorLogger, denLocationId, locationName);
            ProcessNestEncounters(Encounters8Nest.Nest_SH, "Shield", encounterData, gameStrings, errorLogger, denLocationId, locationName);
            ProcessDistributionEncounters(Encounters8Nest.Dist_SW, "Sword", encounterData, gameStrings, errorLogger, denLocationId, locationName);
            ProcessDistributionEncounters(Encounters8Nest.Dist_SH, "Shield", encounterData, gameStrings, errorLogger, denLocationId, locationName);
            ProcessCrystalEncounters(Encounters8Nest.Crystal_SWSH, encounterData, gameStrings, errorLogger, denLocationId, locationName);
        }

        private static void ProcessNestEncounters(EncounterStatic8N[] encounters, string versionName,
            Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, int locationId, string locationName)
        {
            foreach (var encounter in encounters)
            {
                bool canGigantamax = Gigantamax.CanToggle(encounter.Species, encounter.Form) || encounter.CanGigantamax;
                bool isShinyLocked = encounter.Shiny == Shiny.Never;

                // For standard nest encounters, min and max level are the same
                int minLevel = encounter.Level;
                int maxLevel = encounter.Level;

                // Use AddSingleEncounterInfo instead of AddEncounterInfo to bypass evolution processing
                AddSingleEncounterInfo(
                    encounterData,
                    gameStrings,
                    errorLogger,
                    encounter.Species,
                    encounter.Form,
                    locationName,
                    locationId,
                    minLevel,
                    maxLevel,
                    "Max Raid", // Consistent type
                    isShinyLocked,
                    false,
                    string.Empty,
                    versionName,
                    canGigantamax
                );
            }
        }

        private static void ProcessDistributionEncounters(EncounterStatic8ND[] encounters, string versionName,
            Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, int locationId, string locationName)
        {
            foreach (var encounter in encounters)
            {
                bool canGigantamax = Gigantamax.CanToggle(encounter.Species, encounter.Form) || encounter.CanGigantamax;
                bool isShinyLocked = encounter.Shiny == Shiny.Never;

                // For distribution encounters, min and max level are the same
                int minLevel = encounter.Level;
                int maxLevel = encounter.Level;

                // Use AddSingleEncounterInfo instead of AddEncounterInfo to bypass evolution processing
                AddSingleEncounterInfo(
                    encounterData,
                    gameStrings,
                    errorLogger,
                    encounter.Species,
                    encounter.Form,
                    locationName,
                    locationId,
                    minLevel,
                    maxLevel,
                    "Max Raid", // Consistent type
                    isShinyLocked,
                    false,
                    string.Empty,
                    versionName,
                    canGigantamax
                );
            }
        }

        private static void ProcessCrystalEncounters(EncounterStatic8NC[] encounters,
            Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, int locationId, string locationName)
        {
            foreach (var encounter in encounters)
            {
                string versionName = encounter.Version == GameVersion.SW ? "Sword" :
                                     encounter.Version == GameVersion.SH ? "Shield" : "Both";

                bool canGigantamax = Gigantamax.CanToggle(encounter.Species, encounter.Form) || encounter.CanGigantamax;
                bool isShinyLocked = encounter.Shiny == Shiny.Never;

                // For crystal encounters, min and max level are the same
                int minLevel = encounter.Level;
                int maxLevel = encounter.Level;

                // Use AddSingleEncounterInfo instead of AddEncounterInfo to bypass evolution processing
                AddSingleEncounterInfo(
                    encounterData,
                    gameStrings,
                    errorLogger,
                    encounter.Species,
                    encounter.Form,
                    locationName,
                    locationId,
                    minLevel,
                    maxLevel,
                    "Max Raid", // Consistent type
                    isShinyLocked,
                    false,
                    string.Empty,
                    versionName,
                    canGigantamax
                );
            }
        }

        private static void ProcessMaxLairEncounters(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings, StreamWriter errorLogger)
        {
            foreach (var encounter in Encounters8Nest.DynAdv_SWSH)
            {
                var locationName = gameStrings.GetLocationName(false, MaxLair, 8, 8, GameVersion.SWSH);
                bool canGigantamax = Gigantamax.CanToggle(encounter.Species, encounter.Form) || encounter.CanGigantamax;

                AddEncounterInfo(encounterData, gameStrings, errorLogger, encounter.Species, encounter.Form, locationName, MaxLair, encounter.Level, encounter.Level, "Max Lair", encounter.Shiny == Shiny.Never, false, string.Empty, "Both", canGigantamax);
            }
        }

        private static void AddEncounterInfo(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            StreamWriter errorLogger, ushort speciesIndex, byte form, string locationName, int locationId, int minLevel, int maxLevel,
            string encounterType, bool isShinyLocked = false, bool isGift = false, string fixedBall = "",
            string encounterVersion = "Both", bool canGigantamax = false)
        {
            var pt = PersonalTable.SWSH;
            var personalInfo = pt[speciesIndex];
            if (personalInfo is null || !personalInfo.IsPresentInGame)
            {
                errorLogger.WriteLine($"[{DateTime.Now}] Species {speciesIndex} not present in SWSH. Skipping.");
                return;
            }
            // Process base species and its evolutions
            AddEncounterInfoWithEvolutions(encounterData, gameStrings, pt, errorLogger, speciesIndex, form, locationName, locationId,
                minLevel, maxLevel, encounterType, isShinyLocked, isGift, fixedBall, encounterVersion, canGigantamax);
        }

        private static void AddEncounterInfoWithEvolutions(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            PersonalTable8SWSH pt, StreamWriter errorLogger, ushort speciesIndex, byte form, string locationName, int locationId,
            int minLevel, int maxLevel, string encounterType, bool isShinyLocked, bool isGift, string fixedBall,
            string encounterVersion, bool canGigantamax)
        {
            var personalInfo = pt.GetFormEntry(speciesIndex, form);
            if (personalInfo is null || !personalInfo.IsPresentInGame)
            {
                errorLogger.WriteLine($"[{DateTime.Now}] Species {speciesIndex} form {form} not present in SWSH. Skipping.");
                return;
            }

            // Process base species
            AddSingleEncounterInfo(encounterData, gameStrings, errorLogger, speciesIndex, form, locationName, locationId,
                minLevel, maxLevel, encounterType, isShinyLocked, isGift, fixedBall, encounterVersion, canGigantamax);

            // Track processed species/forms to avoid duplicates
            var processedForms = new HashSet<(ushort Species, byte Form)>();
            processedForms.Add((speciesIndex, form));

            // Process all evolutions recursively
            ProcessEvolutionLine(encounterData, gameStrings, pt, errorLogger, speciesIndex, form, locationName, locationId,
                minLevel, encounterType, isShinyLocked, isGift, fixedBall, encounterVersion, canGigantamax, processedForms);
        }

        private static int GetMinEvolutionLevel(ushort baseSpecies, ushort evolvedSpecies)
        {
            var tree = EvolutionTree.Evolves8;
            var pk = new PK8 { Species = baseSpecies, Form = 0, CurrentLevel = 100, Version = GameVersion.SW };
            int maxLevel = 1;

            // Get possible evolutions forward from base species
            var evos = tree.Forward.GetForward(baseSpecies, 0);
            foreach (var evo in evos.Span)
            {
                if (evo.Species == evolvedSpecies)
                {
                    // Direct evolution - use level requirement
                    maxLevel = Math.Max(maxLevel, Math.Max(evo.Level, evo.LevelUp));
                }
                else
                {
                    // Check if this is an intermediate evolution
                    var nextEvos = tree.Forward.GetForward(evo.Species, 0);
                    foreach (var nextEvo in nextEvos.Span)
                    {
                        if (nextEvo.Species == evolvedSpecies)
                        {
                            // Found target species - use highest level requirement from chain
                            int evoLevel = Math.Max(evo.Level, evo.LevelUp);
                            int nextEvoLevel = Math.Max(nextEvo.Level, nextEvo.LevelUp);
                            maxLevel = Math.Max(maxLevel, Math.Max(evoLevel, nextEvoLevel));
                        }
                    }
                }
            }

            return maxLevel;
        }

        private static void ProcessEvolutionLine(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
            PersonalTable8SWSH pt, StreamWriter errorLogger, ushort species, byte form, string locationName, int locationId,
            int baseLevel, string encounterType, bool isShinyLocked, bool isGift, string fixedBall, string encounterVersion,
            bool baseCanGigantamax, HashSet<(ushort Species, byte Form)> processedForms)
        {
            var personalInfo = pt.GetFormEntry(species, form);
            if (personalInfo == null || !personalInfo.IsPresentInGame)
                return;

            var nextEvolutions = TraverseEvolutions(species, form, pt, processedForms);
            foreach (var (evoSpecies, evoForm) in nextEvolutions)
            {
                // Skip if we've already processed this form
                if (!processedForms.Add((evoSpecies, evoForm)))
                    continue;

                var evoPersonalInfo = pt.GetFormEntry(evoSpecies, evoForm);
                if (evoPersonalInfo == null || !evoPersonalInfo.IsPresentInGame)
                    continue;

                // Get minimum evolution level
                var evolutionMinLevel = GetMinEvolutionLevel(species, evoSpecies);
                // Use the higher of the evolution requirement and base encounter level
                var minLevel = Math.Max(baseLevel, evolutionMinLevel);

                bool evoCanGigantamax = baseCanGigantamax || Gigantamax.CanToggle(evoSpecies, evoForm);
                AddSingleEncounterInfo(encounterData, gameStrings, errorLogger, evoSpecies, evoForm, locationName, locationId,
                    minLevel, minLevel, encounterType, isShinyLocked, isGift, fixedBall, encounterVersion, evoCanGigantamax);

                // Recursively process next evolutions
                ProcessEvolutionLine(encounterData, gameStrings, pt, errorLogger, evoSpecies, evoForm, locationName, locationId,
                    minLevel, encounterType, isShinyLocked, isGift, fixedBall, encounterVersion, evoCanGigantamax, processedForms);
            }
        }

        private static List<(ushort Species, byte Form)> TraverseEvolutions(ushort species, byte form, PersonalTable8SWSH pt, HashSet<(ushort Species, byte Form)> processedForms)
        {
            var results = new List<(ushort Species, byte Form)>();
            var personalInfo = pt.GetFormEntry(species, form);

            if (personalInfo == null)
                return results;

            // Get evolutions (each species can have different forms)
            for (ushort evoSpecies = 1; evoSpecies < pt.MaxSpeciesID; evoSpecies++)
            {
                if (processedForms.Contains((evoSpecies, 0)))
                    continue;

                var evoPersonalInfo = pt.GetFormEntry(evoSpecies, 0);
                if (evoPersonalInfo == null || !evoPersonalInfo.IsPresentInGame)
                    continue;

                // Check if this species evolves from our current species
                if (evoPersonalInfo.HatchSpecies == species)
                {
                    // Get all valid forms for this evolved species
                    byte formCount = evoPersonalInfo.FormCount;
                    for (byte evoForm = 0; evoForm < formCount; evoForm++)
                    {
                        var formInfo = pt.GetFormEntry(evoSpecies, evoForm);
                        if (formInfo != null && formInfo.IsPresentInGame)
                        {
                            results.Add((evoSpecies, evoForm));
                        }
                    }
                }
            }

            return results;
        }

        private static string CombineVersions(string version1, string version2)
        {
            if (version1 == "Both" || version2 == "Both")
                return "Both";
            if ((version1 == "Sword" && version2 == "Shield") ||
                (version1 == "Shield" && version2 == "Sword"))
            {
                return "Both";
            }

            return version1; // Return existing version if they're the same
        }

        private static void AddSingleEncounterInfo(Dictionary<string, List<EncounterInfo>> encounterData, GameStrings gameStrings,
           StreamWriter errorLogger, ushort speciesIndex, byte form, string locationName, int locationId, int minLevel, int maxLevel,
           string encounterType, bool isShinyLocked, bool isGift, string fixedBall, string encounterVersion, bool canGigantamax)
        {
            string dexNumber = speciesIndex.ToString();
            if (form > 0)
                dexNumber += $"-{form}";

            if (!encounterData.ContainsKey(dexNumber))
                encounterData[dexNumber] = new List<EncounterInfo>();

            var personalInfo = PersonalTable.SWSH.GetFormEntry(speciesIndex, form);
            string genderRatio = DetermineGenderRatio(personalInfo);

            var existingEncounter = encounterData[dexNumber].FirstOrDefault(e =>
                e.LocationId == locationId &&
                e.SpeciesIndex == speciesIndex &&
                e.Form == form &&
                e.EncounterType == encounterType &&
                e.CanGigantamax == canGigantamax &&
                e.Gender == genderRatio);

            if (existingEncounter != null)
            {
                existingEncounter.MinLevel = Math.Min(existingEncounter.MinLevel, minLevel);
                existingEncounter.MaxLevel = Math.Max(existingEncounter.MaxLevel, maxLevel);

                // Ensure non-null values for both version parameters
                string existingVersion = existingEncounter.EncounterVersion ?? string.Empty;
                string newVersion = encounterVersion ?? string.Empty;
                existingEncounter.EncounterVersion = CombineVersions(existingVersion, newVersion);

                errorLogger.WriteLine($"[{DateTime.Now}] Updated existing encounter: {gameStrings.specieslist[speciesIndex]} " +
                    $"(Dex: {dexNumber}) at {locationName} (ID: {locationId}), Levels {existingEncounter.MinLevel}-{existingEncounter.MaxLevel}, " +
                    $"Type: {encounterType}, Version: {existingEncounter.EncounterVersion}, Can Gigantamax: {canGigantamax}, Gender: {genderRatio}");
            }
            else
            {
                encounterData[dexNumber].Add(new EncounterInfo
                {
                    SpeciesName = gameStrings.specieslist[speciesIndex],
                    SpeciesIndex = speciesIndex,
                    Form = form,
                    LocationName = locationName,
                    LocationId = locationId,
                    MinLevel = minLevel,
                    MaxLevel = maxLevel,
                    EncounterType = encounterType,
                    IsShinyLocked = isShinyLocked,
                    IsGift = isGift,
                    FixedBall = fixedBall,
                    EncounterVersion = encounterVersion,
                    CanGigantamax = canGigantamax,
                    Gender = genderRatio
                });

                errorLogger.WriteLine($"[{DateTime.Now}] Processed new encounter: {gameStrings.specieslist[speciesIndex]} " +
                    $"(Dex: {dexNumber}) at {locationName} (ID: {locationId}), Levels {minLevel}-{maxLevel}, " +
                    $"Type: {encounterType}, Version: {encounterVersion}, Can Gigantamax: {canGigantamax}, Gender: {genderRatio}");
            }
        }

        private static string DetermineGenderRatio(IPersonalInfo personalInfo)
        {
            if (personalInfo == null)
                return "Unknown";

            if (personalInfo.Genderless)
                return "Genderless";
            if (personalInfo.OnlyFemale)
                return "Female";
            if (personalInfo.OnlyMale)
                return "Male";

            // Handle regular gender ratios
            return personalInfo.Gender switch
            {
                0 => "Male",         // 100% Male
                254 => "Female",     // 100% Female
                255 => "Genderless", // Genderless
                _ => "Male, Female"  // Mixed gender ratio
            };
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
            public bool IsGift { get; set; }
            public string? FixedBall { get; set; }
            public string? EncounterVersion { get; set; }
            public bool CanGigantamax { get; set; }
            public string? Gender { get; set; } 
        }
    }
}
