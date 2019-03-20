// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using NUnit.Framework.Constraints;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Screens.Menu;
using osuTK;

namespace osu.Game.Graphics.Containers
{
    public class LogoFacadeContainer : Container
    {
        public bool TrackLogo
        {
            get => trackLogo;
            set
            {
                trackLogo = value;
                lastTrackLogo = false;
            }
        }

        private OsuLogo logo;
        private readonly float scaffoldScale;

        private bool lastTrackLogo;
        private bool trackLogo;

        public LogoFacadeContainer(float scaffoldScale = 0.5f)
        {
            this.scaffoldScale = scaffoldScale;
        }

        public void SetLogo(OsuLogo logo)
        {
            this.logo = logo;
            Width = Height = logo.SizeForFlow * scaffoldScale;
        }

        private Vector2 logoTrackingPosition => logo.Parent.ToLocalSpace(ScreenSpaceDrawQuad.Centre);

        protected override void Update()
        {
            base.Update();

            if (logo != null)
            {
                Width = Height = logo.SizeForFlow * scaffoldScale;

                if (TrackLogo && IsLoaded)
                {
                    logo.RelativePositionAxes = Axes.None;
                    if (!lastTrackLogo)
                    {
                        Schedule(() => logo.MoveTo(logoTrackingPosition, 500, Easing.InOutExpo));
                    }
                    else if (logo.Transforms.Count == 0)
                    {
                        logo.MoveTo(logoTrackingPosition, 0);
                    }

                    lastTrackLogo = true;
                }
            }
        }
    }
}
