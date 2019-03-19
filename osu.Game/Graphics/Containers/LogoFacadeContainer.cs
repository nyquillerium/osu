// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Screens.Menu;
using osuTK;

namespace osu.Game.Graphics.Containers
{
    public class LogoFacadeContainer : Container
    {
        public bool TrackLogo;

        private OsuLogo logo;
        private readonly float scaffoldScale;

        public LogoFacadeContainer(float scaffoldScale = 0.5f)
        {
            this.scaffoldScale = scaffoldScale;
        }

        public void SetLogo(OsuLogo logo)
        {
            this.logo = logo;
        }

        private Vector2 logoTrackingPosition => logo.Parent.ToLocalSpace(ScreenSpaceDrawQuad.Centre);

        protected override void Update()
        {
            base.Update();

            if (logo != null)
            {
                if (TrackLogo && logo.RelativePositionAxes == Axes.None && IsLoaded)
                    logo.Position = logoTrackingPosition;

                if (logo.Position != logoTrackingPosition && !TrackLogo && logo.Transforms.Count == 0)
                {
                    logo.MoveTo(logoTrackingPosition, 500, Easing.InOutExpo);
                }

                Width = Height = logo.SizeForFlow * scaffoldScale;
            }
        }
    }
}
