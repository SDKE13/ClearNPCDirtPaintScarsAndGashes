using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using nifly;
using Noggog;
using System.Text;
using Reloaded.Memory.Utilities;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using System.Linq;
using CommandLine;
using Serilog;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole;
using Serilog.Core;
using System.ComponentModel.DataAnnotations;
using ClearNPCDirtPaintScarsAndGashes.Common;
using System.Collections.Generic;

namespace ClearNPCDirtPaintScarsAndGashes.Models
{
    public class RaceTintMask
    {
       
        public IRaceGetter? _race { get; set; }
        public Dictionary<TintType, List<ushort>> femaleRaceTintmasks { get; set; }
        public Dictionary<TintType, List<ushort>> maleRaceTintmasks { get; set; }

        //Dictionary<FormKey, List<ushort>> raceTintmasks = new Dictionary<FormKey, List<ushort>>();
        //Dictionary<FormKey, List<ushort>> dirtPaintTintmasks = new Dictionary<FormKey, List<ushort>>();
        //Dictionary<FormKey, List<ushort>> Tintmasks = new Dictionary<FormKey, List<ushort>>();
        //Dictionary<FormKey, List<ushort>> raceTintmasks = new Dictionary<FormKey, List<ushort>>();
        //Dictionary<FormKey, List<ushort>> femaleRaceTintmasks = new Dictionary<FormKey, List<ushort>>();
        //Dictionary<FormKey, List<ushort>> maleRaceTintmasks = new Dictionary<FormKey, List<ushort>>();

        private RaceTintMask()
        {
            femaleRaceTintmasks = new Dictionary<TintType, List<ushort>>();
            maleRaceTintmasks = new Dictionary<TintType, List<ushort>>();
        }

        public RaceTintMask(IRaceGetter race) : this()
        {
            _race = race;
            IReadOnlyList<ITintAssetsGetter>? t;

            t = _race.HeadData?.Female?.TintMasks ?? null;
            
            if (t != null)
            {
                femaleRaceTintmasks = HydrateTintMasks(t);
            }

            t = _race.HeadData?.Male?.TintMasks ?? null;

            if (t != null)
            {
                maleRaceTintmasks = HydrateTintMasks(t);
            }
        }

        protected Dictionary<TintType, List<ushort>> HydrateTintMasks(IReadOnlyList<ITintAssetsGetter> tintMaskGetter)
        {
            Dictionary<TintType, List<ushort>> genderTintMasks = new Dictionary<TintType, List<ushort>>();


            var dirtIndexs = tintMaskGetter.Where(a => IsDirt(a)).Select(s => s.Index ?? 0);
            genderTintMasks.Add(TintType.Dirt, dirtIndexs.ToList<ushort>());

            var paintIndexs = tintMaskGetter.Where(a => IsPaint(a)).Select(s => s.Index ?? 0);
            genderTintMasks.Add(TintType.Paint, paintIndexs.ToList<ushort>());

            var tattooIndexs = tintMaskGetter.Where(a => IsTattoo(a)).Select(s => s.Index ?? 0);
            genderTintMasks.Add(TintType.Tattoo, tattooIndexs.ToList<ushort>());

            return genderTintMasks;
        }           

        protected bool IsDirt(ITintAssetsGetter tintAsset)
        {
            return (tintAsset.MaskType ?? TintAssets.TintMaskType.None) == TintAssets.TintMaskType.Dirt;
        }

        protected bool IsPaint(ITintAssetsGetter tintAsset)
        {
            return (tintAsset.MaskType ?? TintAssets.TintMaskType.None) == TintAssets.TintMaskType.Paint;
        }

        protected bool IsTattoo(ITintAssetsGetter tintAsset)
        {
            return (tintAsset.FileName?.ToString() ?? string.Empty).Contains("Tattoo", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
