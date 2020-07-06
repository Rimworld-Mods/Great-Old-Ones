using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using UnityEngine;         // Always needed
using Verse;               // RimWorld universal objects are here (like 'Building')
using Verse.AI;            // Needed when you do something with the AI
using Verse.AI.Group;
using Verse.Sound;         // Needed when you do something with Sound
using Verse.Noise;         // Needed when you do something with Noises
using RimWorld;            // RimWorld specific functions are found here (like 'Building_Battery')
using RimWorld.Planet;     // RimWorld specific functions for world creation

namespace Cults
{
    public static class FloatingOptionsUtility
    { 
        //-------------------------------------------------
        // Deity

        public static void SelectDeity(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<CosmicEntityDef> list = new List<CosmicEntityDef>();

            foreach (CosmicEntity candidate in CultKnowledge.deities)
            {
                list.Add(candidate.def);
            }

            foreach(CosmicEntityDef thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.label, 
                    delegate 
                    { 
                        if(altar.ritualDeity != thing)
                        {
                            altar.ritualParmsHuman.reward = null;
                            altar.ritualParmsAnimal.reward = null;
                            altar.ritualParmsItem.reward = null;
                            altar.ritualParmsFood.reward = null;
                        }
                        altar.ritualDeity = thing; 
                    }, 
                    thing.symbolTex, 
                    new Color(1f, 0f, 0f, 1f), 
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Preacher

        public static void SelectPreacher(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<Pawn> list = new List<Pawn>();

            foreach (Pawn candidate in altar.Map.mapPawns.AllPawnsSpawned)
            {
                if (!candidate.IsColonist) continue;
                list.Add(candidate);
            }

            foreach(Pawn thing in list)
            {
                options.Add(new FloatMenuOption(
                    "        " + thing.Name.ToString(), 
                    delegate { altar.ritualPreacher = thing; }, 
                    MenuOptionPriority.Default,
                    null,
                    null,
                    32,
                    (rect => {
                        rect.x = 0;
                        Widgets.ThingIcon(rect, thing);
                        return false;
                    })
                ));

                /*
                options.Add(new FloatMenuOption(
                    thing.Name.ToString(), 
                    delegate { parms.sacrifice = thing; }, 
                    MenuOptionPriority.Default
                ));
                */
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Animal

        public static void SelectAnimalSacrifice(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<Pawn> list = new List<Pawn>();

            foreach (Pawn candidate in altar.Map.mapPawns.AllPawnsSpawned)
            {
                if (candidate.Faction != Faction.OfPlayer) continue;
                if (candidate.RaceProps == null) continue;
                if (candidate.RaceProps.Humanlike) continue;
                list.Add(candidate);
            }

            foreach(Pawn thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.Name.ToString(), 
                    delegate { parms.sacrifice = thing; }, 
                    thing.def,
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Human

        public static void SelectHumanSacrifice(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<Pawn> list = new List<Pawn>();

            foreach (Pawn candidate in altar.Map.mapPawns.AllPawnsSpawned)
            {
                if (!candidate.IsPrisonerOfColony) continue;
                list.Add(candidate);
            }

            foreach(Pawn thing in list)
            {
                
                options.Add(new FloatMenuOption(
                    "        " + thing.Name.ToString(), 
                    delegate { parms.sacrifice = thing; }, 
                    MenuOptionPriority.Default,
                    null,
                    null,
                    32,
                    (rect => {
                        rect.x = 0;
                        Widgets.ThingIcon(rect, thing);
                        return false;
                    })
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }
        
        //-------------------------------------------------
        // Food

        public static void SelectFoodSacrifice(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();


            // Gets all item on each cell separated
            List<Thing> things = altar.Map.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            List<Thing> list = new List<Thing>();

            foreach(Thing thing in things)
            {
                if(thing.def.IsWithinCategory(ThingCategoryDefOf.Foods))
                {
                    list.Add(thing);
                };
            }

            foreach(Thing thing in things)
            {
                options.Add(new FloatMenuOption(
                    thing.def.label, 
                    delegate { /*parms.sacrifice = thing;*/ }, 
                    thing.def,
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

        //-------------------------------------------------
        // Artefact

        public static void SelectItemSacrifice(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }


        //-------------------------------------------------
        // Reward

        public static void SelectRewardSpell(Building_BaseAltar altar, Building_BaseAltar.RitualParms parms)
        {
            
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            List<SpellDef> list = new List<SpellDef>();

            if(altar.ritualDeity == null) return;

            foreach (SpellDef def in altar.ritualDeity.spells)
            {
                // condition
                list.Add(def);
            }

            

            foreach(SpellDef thing in list)
            {
                options.Add(new FloatMenuOption(
                    thing.label,
                    delegate { parms.reward = thing; }, 
                    MenuOptionPriority.Default
                ));
            }

            if(options.NullOrEmpty()) return;
            Find.WindowStack.Add(new FloatMenu(options));
        }

    }
}