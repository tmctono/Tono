using System;
using System.Collections.Generic;
using System.Drawing;

#pragma warning disable 1591, 1572, 1573

namespace Tono.GuiWinForm
{
    /// <summary>
    /// �w���`�ɓ��荞�񂾃p�[�c��I����Ԃɂ���
    /// </summary>
    public class FeaturePartsSelectOnRect : FeatureControlBridgeBase, IMouseListener, IPartsIllusionable
#if DEBUG == false
, IAutoRemovable
#endif
    {
        private class dpMask : PartsRectangle, IPartsVisible
        {
            private static readonly Brush _maskBG = new SolidBrush(Color.FromArgb(48, 0, 255, 0));
            private static readonly Pen _maskPen = new Pen(Color.FromArgb(128, 0, 255, 0));

            /// <summary>
            /// �I��̈�̋�`��`�悷��
            /// </summary>
            /// <param name="rp"></param>
            /// <returns></returns>
            public override bool Draw(IRichPane rp)
            {
                if (_isVisible)
                {
                    var spos = GetScRect(rp);
                    if (isInClip(rp, spos) == false)    // �`��s�v�ł���΁A�Ȃɂ����Ȃ�
                    {
                        return false;
                    }
                    rp.Graphics.FillRectangle(_maskBG, spos);
                    rp.Graphics.DrawRectangle(_maskPen, spos);
                }
                return true;
            }

            #region IPartsVisible �����o
            private bool _isVisible = true;
            public bool Visible
            {
                get => _isVisible;
                set => _isVisible = value;
            }

            #endregion
        }

        [NonSerialized]
        private dpMask _mask = null;
        [NonSerialized]
        private IRichPane _tarPane = null;

        /// <summary>�I�𒆂̃p�[�c�i���L�ϐ��j</summary>
        private PartsCollectionBase _selectedParts;

        /// <summary>
        /// �R�[�_�[
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private CodeRect _coder(LayoutRect rect, PartsBase target)
        {
            _tarPane.Convert(ScreenRect.FromLTRB(rect.LT.X, rect.LT.Y, rect.RB.X, rect.RB.Y));
            return CodeRect.FromLTRB(rect.LT.X, rect.LT.Y, rect.RB.X, rect.RB.Y);
        }

        /// <summary>
        /// �|�W�V���i�[
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private LayoutRect _positioner(CodeRect rect, PartsBase target)
        {
            var ret = _tarPane.Convert(ScreenRect.FromLTRB(rect.LT.X, rect.LT.Y, rect.RB.X, rect.RB.Y));
            return ret;
        }

        /// <summary>
        /// ������
        /// </summary>
        public override void OnInitInstance()
        {
            base.OnInitInstance();

            // �X�e�[�^�X����
            _selectedParts = (PartsCollectionBase)Share.Get("SelectedParts", typeof(PartsCollection));
            _tarPane = Pane.GetPane("Resource");

            // ��`�`��p�p�[�c�𐶐�����
            _mask = new dpMask
            {
                Rect = CodeRect.FromLTWH(int.MinValue / 2, int.MinValue / 2, 0, 0),
                PartsPositionCorder = new PartsBase.PosCoderMethod(_coder),
                PartsPositioner = new PartsBase.PositionerMethod(_positioner),
                Visible = false
            };
            Parts.Add(_tarPane, _mask, Const.Layer.StaticLayers.MaskRect);
        }

        /// <summary>
        /// �p�[�c�ꗗ���t�B���^�[����i�I�[�o�[���C�h���Ȃ��ƑS�p�[�c�ƂȂ�j
        /// </summary>
        /// <returns></returns>
        protected virtual ICollection<PartsBase> GetFilteredParts()
        {
            var parts = new List<PartsBase>();
            foreach (PartsCollection.PartsEntry pe in Parts)
            {
                if (pe.Pane.IdText == _tarPane.IdText)
                {
                    parts.Add(pe.Parts);
                }
            }
            return parts;
        }

        private ScreenPos _startPos = null;
        private readonly List<IPartsSelectable> _shiftAdd = new List<IPartsSelectable>();

        /// <summary>
        /// �S BarTrip�̑I������
        /// </summary>
        private void resetSelect(bool isInvaliadte)
        {
            _selectedParts.Clear();
            foreach (var pts in GetFilteredParts())
            {
                var part = pts as IPartsSelectable;
                if (part != null && part is PartsBase)
                {
                    if (part.IsSelected)
                    {
                        part.IsSelected = false;
                        if (isInvaliadte)
                        {
                            Parts.Invalidate((PartsBase)part, _tarPane);
                        }
                    }
                }
            }
        }


        #region IMouseListener �����o

        public void OnMouseMove(MouseState e)
        {
            if (_mask.Visible)
            {
                if (e.Attr.IsButton == false)
                {
                    OnMouseUp(e);
                    return;
                }
                _mask.Rect = CodeRect.FromLTRB(_startPos.X, _startPos.Y, e.Pos.X, e.Pos.Y);
                _mask.Rect.Normalize();

                // �I���J�n
                resetSelect(false);
                foreach (var shiftAdd in _shiftAdd)    // SHIFT�L�[�Œǉ��w�肵���p�[�c
                {
                    shiftAdd.IsSelected = true;
                    _selectedParts.Add(_tarPane, (PartsBase)shiftAdd);
                }
                // �I��̈�̃p�[�c
                foreach (var pts in GetFilteredParts())
                {
                    if (Parts.IsOverlapped(_tarPane, pts, _tarPane, _mask, true))
                    {
                        var selp = pts as IPartsSelectable;
                        if (selp != null)
                        {
                            var selected = _shiftAdd.Contains(selp);
                            var prevSel = selp.IsSelected;
                            selp.IsSelected = !selected;
                            if (selp.IsSelected)
                            {
                                _selectedParts.Add(_tarPane, (PartsBase)selp);
                            }
                            else
                            {
                                _selectedParts.Remove((PartsBase)selp);
                            }
                        }
                    }
                }
                Pane.Invalidate(null);
            }
        }

        public void OnMouseDown(MouseState e)
        {
            if (e.Attr.IsCtrl || e.Attr.IsButtonMiddle)
            {
                return;
            }
            if (ClickParts != null)
            {
                return; // �p�[�c�O�̃h���b�O�̂݁A�I���J�n�ł���B
            }
            if (e.Pos.Y >= _tarPane.GetPaneRect().RB.Y - 16)    // �X�N���[���o�[��ł́A�I���J�n�ł��Ȃ�
            {
                return;
            }
            if (e.Pos.X >= _tarPane.GetPaneRect().RB.X - 16)    // �X�N���[���o�[��ł́A�I���J�n�ł��Ȃ�
            {
                return;
            }

            _shiftAdd.Clear();
            if (e.Attr.IsShift)
            {
                foreach (PartsCollection.PartsEntry pe in _selectedParts)
                {
                    _shiftAdd.Add((IPartsSelectable)pe.Parts);
                }
            }
            resetSelect(true);
            _startPos = (ScreenPos)e.Pos.Clone();
            _mask.Rect = CodeRect.FromLTWH(_startPos.X, _startPos.Y, 0, 0);
            _mask.Visible = true;
        }

        public void OnMouseUp(MouseState e)
        {
            _mask.Visible = false;
            Pane.Invalidate(null);
        }

        public void OnMouseWheel(MouseState e)
        {
        }
        #endregion
    }
}