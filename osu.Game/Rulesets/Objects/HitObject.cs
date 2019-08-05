﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Objects
{
    /// <summary>
    /// A HitObject describes an object in a Beatmap.
    /// <para>
    /// HitObjects may contain more properties for which you should be checking through the IHas* types.
    /// </para>
    /// </summary>
    public class HitObject
    {
        /// <summary>
        /// A small adjustment to the start time of control points to account for rounding/precision errors.
        /// </summary>
        private const double control_point_leniency = 1;

        /// <summary>
        /// The time at which the HitObject starts.
        /// </summary>
        public virtual double StartTime { get; set; }

        private List<HitSampleInfo> samples;

        /// <summary>
        /// The samples to be played when this hit object is hit.
        /// <para>
        /// In the case of <see cref="IHasRepeats"/> types, this is the sample of the curve body
        /// and can be treated as the default samples for the hit object.
        /// </para>
        /// </summary>
        public List<HitSampleInfo> Samples
        {
            get => samples ?? (samples = new List<HitSampleInfo>());
            set => samples = value;
        }

        [JsonIgnore]
        public SampleControlPoint SampleControlPoint;

        /// <summary>
        /// Whether this <see cref="HitObject"/> is in Kiai time.
        /// </summary>
        [JsonIgnore]
        public bool Kiai { get; private set; }

        /// <summary>
        /// The hit windows for this <see cref="HitObject"/>.
        /// </summary>
        public HitWindows HitWindows { get; set; }

        private readonly List<HitObject> nestedHitObjects = new List<HitObject>();

        [JsonIgnore]
        public IReadOnlyList<HitObject> NestedHitObjects => nestedHitObjects;

        /// <summary>
        /// Applies default values to this HitObject.
        /// </summary>
        /// <param name="controlPointInfo">The control points.</param>
        /// <param name="difficulty">The difficulty settings to use.</param>
        public void ApplyDefaults(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            ApplyDefaultsToSelf(controlPointInfo, difficulty);

            // This is done here since ApplyDefaultsToSelf may be used to determine the end time
            SampleControlPoint = controlPointInfo.SamplePointAt(((this as IHasEndTime)?.EndTime ?? StartTime) + control_point_leniency);

            nestedHitObjects.Clear();

            CreateNestedHitObjects();

            nestedHitObjects.Sort((h1, h2) => h1.StartTime.CompareTo(h2.StartTime));

            foreach (var h in nestedHitObjects)
            {
                h.HitWindows = HitWindows;
                h.ApplyDefaults(controlPointInfo, difficulty);
            }
        }

        protected virtual void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            Kiai = controlPointInfo.EffectPointAt(StartTime + control_point_leniency).KiaiMode;

            if (HitWindows == null)
                HitWindows = CreateHitWindows();
            HitWindows?.SetDifficulty(difficulty.OverallDifficulty);
        }

        protected virtual void CreateNestedHitObjects()
        {
        }

        /// <summary>
        /// Adds a <see cref="HitObject"/>s to this object's <see cref="NestedHitObjects"/>.
        /// </summary>
        /// <param name="hitObject">The <see cref="HitObject"/> to add.</param>
        protected void AddNested(HitObject hitObject) => nestedHitObjects.Add(hitObject);

        /// <summary>
        /// Removes a <see cref="HitObject"/>s from this object's <see cref="NestedHitObjects"/>.
        /// </summary>
        /// <param name="predicate">The predicate the <see cref="HitObject"/> must match to be removed.</param>
        protected void RemoveAllFromNested(Predicate<HitObject> predicate) => nestedHitObjects.RemoveAll(predicate);

        /// <summary>
        /// Creates the <see cref="Judgement"/> that represents the scoring information for this <see cref="HitObject"/>.
        /// May be null.
        /// </summary>
        public virtual Judgement CreateJudgement() => null;

        /// <summary>
        /// Creates the <see cref="HitWindows"/> for this <see cref="HitObject"/>.
        /// This can be null to indicate that the <see cref="HitObject"/> has no <see cref="HitWindows"/>.
        /// <para>
        /// This will only be invoked if <see cref="HitWindows"/> hasn't been set externally (e.g. from a <see cref="BeatmapConverter{T}"/>.
        /// </para>
        /// </summary>
        protected virtual HitWindows CreateHitWindows() => new HitWindows();
    }
}
