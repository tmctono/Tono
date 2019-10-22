// Copyright (c) Manabu Tonosaki All rights reserved.
// Licensed under the MIT license.

#pragma warning disable 1591, 1572, 1573

namespace Tono.GuiWinForm
{
    /// <summary>
    /// ITimeSpan の概要の説明です。
    /// </summary>
    public interface ITimeSpan
    {
        DateTimeEx Start { get; set; }
        DateTimeEx End { get; set; }
        DateTimeEx Span { get; }
    }
}
