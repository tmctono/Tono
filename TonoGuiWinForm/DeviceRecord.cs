using System.Collections;
using System.Windows.Forms;

#pragma warning disable 1591, 1572, 1573

namespace Tono.GuiWinForm
{
    /// <summary>
    /// DeviceRecord �̊T�v�̐����ł��B
    /// </summary>
    public class DeviceRecord
    {
        public class TagSkipStart
        {
        }
        public class TagSkipEnd
        {
        }
        public class TagFileSelect
        {
            public System.Windows.Forms.DialogResult DialogResult;
            public string FileName;

            private TagFileSelect()
            {
            }
            public TagFileSelect(System.Windows.Forms.DialogResult ret, string filename)
            {
                DialogResult = ret;
                FileName = filename;
            }
            public override string ToString()
            {
                return DialogResult.ToString() + " Filename = " + FileName;
            }
        }

        /// <summary>
        /// �}�E�X�A�b�v
        /// </summary>
        public class TagMouseUp
        {
            public MouseState MouseState;
            private TagMouseUp()
            {
            }
            public TagMouseUp(MouseState value)
            {
                MouseState = value;
            }
            public override string ToString()
            {
                return GetType().Name + "; " + MouseState.ToString();
            }
        }

        /// <summary>
        /// �}�E�X�_�E��
        /// </summary>
        public class TagMouseDown
        {
            public MouseState MouseState;
            private TagMouseDown()
            {
            }
            public TagMouseDown(MouseState value)
            {
                MouseState = value;
            }
            public override string ToString()
            {
                return GetType().Name + "; " + MouseState.ToString();
            }
        }

        /// <summary>
        /// �}�E�X���[�u
        /// </summary>
        public class TagMouseMove
        {
            public MouseState MouseState;
            private TagMouseMove()
            {
            }
            public TagMouseMove(MouseState value)
            {
                MouseState = value;
            }
            public override string ToString()
            {
                return GetType().Name + "; " + MouseState.ToString();
            }
        }

        /// <summary>
        /// �}�E�X�z�C�[��
        /// </summary>
        public class TagMouseWheel
        {
            public MouseState MouseState;
            private TagMouseWheel()
            {
            }
            public TagMouseWheel(MouseState value)
            {
                MouseState = value;
            }
            public override string ToString()
            {
                return GetType().Name + "; " + MouseState.ToString();
            }
        }

        /// <summary>
        /// �L�[�_�E��
        /// </summary>
        public class TagKeyDown
        {
            public KeyState KeyState;
            private TagKeyDown()
            {
            }
            public TagKeyDown(KeyState value)
            {
                KeyState = value;
            }
            public override string ToString()
            {
                return GetType().Name + "; " + KeyState.ToString();
            }
        }

        /// <summary>
        /// �L�[�A�b�v
        /// </summary>
        public class TagKeyUp
        {
            public KeyState KeyState;
            private TagKeyUp()
            {
            }
            public TagKeyUp(KeyState value)
            {
                KeyState = value;
            }
            public override string ToString()
            {
                return GetType().Name + "; " + KeyState.ToString();
            }
        }

        /// <summary>
        /// �g�[�N��
        /// </summary>
        public class TagToken
        {
            public object State;
            private TagToken()
            {
            }
            public TagToken(object value)
            {
                State = value;
            }
            public override string ToString()
            {
                if (State is NamedId)
                {
                    return GetType().Name + "; NamedId = " + State.ToString();
                }
                else
                {
                    return GetType().Name + "; " + State.GetType().Name;
                }
            }
        }

        public class TagTextBox
        {
            public string Name;
            public string Text;
            public int SelectionStart;
            public int SelectionLength;

            public TagTextBox()
            {
            }
            public TagTextBox(TextBox value)
            {
                Name = value.Name;
                Text = value.Text;
                SelectionStart = value.SelectionStart;
                SelectionLength = value.SelectionLength;
            }
        }

        public class TagListView
        {
            public string Name;
            public ArrayList Selected;

            public TagListView()
            {
            }
            public TagListView(ListView value)
            {
                Name = value.Name;
                Selected = new ArrayList();
                foreach (int id in value.SelectedIndices)
                {
                    Selected.Add(id);
                }
            }
        }

        public class TagControlEvent
        {
            public Id FeatureID;
            public string Name;
            public string EventName;

            public TagControlEvent()
            {
            }
            public TagControlEvent(Id fcid, Control value, string eventName)
            {
                FeatureID = fcid;
                Name = value.Name;
                EventName = eventName;
            }
        }

        public class TagCheckBox
        {
            public string Name;
            public bool Checked;
            public TagCheckBox(CheckBox value)
            {
                Name = value.Name;
                Checked = value.Checked;
            }
        }
    }
}