// Copyright (c) Manabu Tonosaki All rights reserved.
// Licensed under the MIT license.

namespace Tono.GuiWinForm
{
    /// <summary>
    /// ズームがあった事を示すイベントを実装できる
    /// </summary>
    public interface IZoomListener
    {
        /// <summary>
        /// ズームの対象となるペーン
        /// </summary>
        IRichPane[] ZoomEventTargets
        {
            get;
        }
        /// <summary>
        /// ズームがあったことを示すイベント
        /// </summary>
        /// <param name="rp">イベントを受信するペーン</param>
        void ZoomChanged(IRichPane rp);
    }
}
