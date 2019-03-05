﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Screens;
using osu.Game.Screens.Backgrounds;

namespace osu.Game.Tests.Visual
{
    public class TestCaseParallaxContainer : OsuTestCase
    {
        public TestCaseParallaxContainer()
        {
            ParallaxContainer parallax;

            Add(parallax = new ParallaxContainer
            {
                Child = new OsuScreenStack(new BackgroundScreenDefault { Alpha = 0.8f })
                {
                    RelativeSizeAxes = Axes.Both,
                }
            });

            AddStep("default parallax", () => parallax.ParallaxAmount = ParallaxContainer.DEFAULT_PARALLAX_AMOUNT);
            AddStep("high parallax", () => parallax.ParallaxAmount = ParallaxContainer.DEFAULT_PARALLAX_AMOUNT * 10);
            AddStep("no parallax", () => parallax.ParallaxAmount = 0);
            AddStep("negative parallax", () => parallax.ParallaxAmount = -ParallaxContainer.DEFAULT_PARALLAX_AMOUNT);
        }
    }
}
