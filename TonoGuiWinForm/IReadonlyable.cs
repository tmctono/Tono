#pragma warning disable 1591, 1572, 1573

namespace Tono.GuiWinForm
{
    /// <summary>
    /// IReadonlyable の概要の説明です。
    /// </summary>
    public interface IReadonlyable
    {
        bool IsReadonly { get; }
        void SetReadonly();
    }
}
