﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Game.Beatmaps;
using osu.Game.Screens;
using osu.Game.Screens.Menu;
using osu.Game.Screens.Play;
using osuTK;

namespace osu.Game.Tests.Visual
{
    public class TestCasePlayerLoader : ManualInputManagerTestCase
    {
        private PlayerLoader loader;
        private readonly ScreenStack stack;

        [Cached]
        private OsuLogo logo;

        [Cached]
        private BackgroundScreenStack backgroundStack;

        public TestCasePlayerLoader()
        {
            InputManager.Add(backgroundStack = new BackgroundScreenStack { RelativeSizeAxes = Axes.Both });
            InputManager.Add(stack = new ScreenStack { RelativeSizeAxes = Axes.Both });
            InputManager.Add(logo = new OsuLogo());
        }

        [BackgroundDependencyLoader]
        private void load(OsuGameBase game)
        {
            Beatmap.Value = new DummyWorkingBeatmap(game);

            AddStep("Reset logo position", () => logo = new OsuLogo { Position = new Vector2(0, 0) });

            AddStep("load dummy beatmap", () => stack.Push(loader = new PlayerLoader(() => new Player
            {
                AllowPause = false,
                AllowLeadIn = false,
                AllowResults = false,
            })));

            AddUntilStep("wait for current", () => loader.IsCurrentScreen());

            AddStep("mouse in centre", () => InputManager.MoveMouseTo(loader.ScreenSpaceDrawQuad.Centre));

            AddUntilStep("wait for no longer current", () => !loader.IsCurrentScreen());

            AddStep("exit loader", () => loader.Exit());

            AddUntilStep("wait for no longer alive", () => !loader.IsAlive);

            AddStep("load slow dummy beatmap", () =>
            {
                SlowLoadPlayer slow = null;

                stack.Push(loader = new PlayerLoader(() => slow = new SlowLoadPlayer
                {
                    AllowPause = false,
                    AllowLeadIn = false,
                    AllowResults = false,
                }));
            });

            AddUntilStep("wait for no longer current", () => !loader.IsCurrentScreen());
        }

        protected class SlowLoadPlayer : Player
        {
            public bool Ready;

            [BackgroundDependencyLoader]
            private void load()
            {
                while (!Ready)
                    Thread.Sleep(1);
            }
        }
    }
}
