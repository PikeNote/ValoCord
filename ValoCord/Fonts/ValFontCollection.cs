using System;
using Avalonia.Media.Fonts;

namespace ValoCord.Fonts;

public sealed class ValFontCollection : EmbeddedFontCollection
{
    public ValFontCollection() : base(
        new Uri("fonts:ValFont", UriKind.Absolute),
        new Uri("avares://ValoCord/Assets/Fonts/ValFont.ttf#VALORANT", UriKind.Absolute))
    { }
}