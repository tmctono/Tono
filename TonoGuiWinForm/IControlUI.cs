// (c) 2019 Manabu Tonosaki
// Licensed under the MIT license.

#pragma warning disable 1591, 1572, 1573

namespace Tono.GuiWinForm
{
    /// <summary>
    /// Control�^��UI����舵��
    /// </summary>
    public interface IControlUI
    {
        System.Windows.Forms.Cursor Cursor
        {
            get;
            set;
        }
    }
}
