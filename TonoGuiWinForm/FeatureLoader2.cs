using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;

#pragma warning disable 1591, 1572, 1573

namespace Tono.GuiWinForm
{
    /// <summary>
    /// uFeatureLoader �̊T�v�̐����ł��B
    /// .NET Framework 2.0�p�AMenuStrip�ɑ΂��郍�[�_
    /// </summary>
    public class FeatureLoader2 : FeatureLoaderBase
    {
        private static Hashtable _assemblyList = null;
        private FeatureGroupRoot _root;
        private readonly Stack _group = new Stack();
        private readonly IDictionary _menuorder = new Hashtable();
        private Form _form = null;
        private static ResourceManager _resMan = null;

        /// <summary>
        /// ���\�[�X���i�[����Ă���N���X��o�^����
        /// </summary>
        /// <param name="resClass"></param>
        public static void SetResources(ResourceManager resMan)
        {
            _resMan = resMan;
        }

        /// <summary>
        /// �g�p���̃A�Z���u�����܂܂��N���X���w�肷��
        /// </summary>
        /// <param name="t">�N���X</param>
        public static void SetUsingClass(Type t)
        {
            if (_assemblyList == null)
            {
                _assemblyList = new Hashtable
                {
                    [Assembly.GetEntryAssembly().FullName] = Assembly.GetEntryAssembly(),
                    [Assembly.GetExecutingAssembly().FullName] = Assembly.GetExecutingAssembly()
                };
            }
            var a = t.Assembly;
            _assemblyList[a.FullName] = a;
        }

        /// <summary>
        /// �g�p���̃A�Z���u�����X�g�𓾂�
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetUsingClasses()
        {
            var ret = new Assembly[_assemblyList.Count];
            var i = 0;
            foreach (Assembly val in _assemblyList.Values)
            {
                ret[i++] = val;
            }
            return ret;
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="root"></param>
        public override void Load(FeatureGroupRoot root, string fname)
        {
            _root = root;
            _group.Push(_root);
            fname = FileUtil.MakeMesFilename(fname);
            load(fname);
        }

        /// <summary>
        /// �t�@�C���������ǂݍ��݃t�B�[�`���[���\�z����
        /// </summary>
        /// <param name="fullpath">�t���p�X</param>
        private void load(string fullpath)
        {
            // �t�H�[�����擾����
            Control c;
            for (c = _root.GetFeatureRich(); c is Form == false; c = c.Parent)
            {
                ;
            }

            _form = (Form)c;

            // �ǂݍ��ݏ���
            var xd = new XmlDocument();
            xd.Load(fullpath);

            var root = xd.DocumentElement;
            foreach (XmlNode node in root.ChildNodes)
            {
                loopProc(node);
            }

            // ���j���[�I�[�_�[���t���Ă��Ȃ����j���[�ɃI�[�_�[�ԍ�������
            if (_form.MainMenuStrip != null)
            {
                menuSetOrderLoopProc(_form.MainMenuStrip.Items);

                // �I�[�_�[�ɂ��������ă\�[�g����
                menuSortLoop(_form.MainMenuStrip.Items);
            }
        }

        #region ���j���[�I�[�_�[�ɏ]���Ĕ�r����
        private class Sorter : IComparer
        {
            private readonly IDictionary _order;
            public Sorter(IDictionary orderbuf)
            {
                _order = orderbuf;
            }
            #region IComparer �����o

            public int Compare(object x, object y)
            {
                var i1 = _order[x];
                var i2 = _order[y];
                if (i1 == null && i2 == null)
                {
                    return 0;
                }
                if (i1 == null && i2 != null)
                {
                    return 1;
                }
                if (i1 != null && i2 == null)
                {
                    return -1;
                }
                return (int)i1 - (int)i2;
            }

            #endregion
        }
        #endregion

        private void menuSortLoop(ToolStripItemCollection mc)
        {
            if (mc == null)
            {
                return;
            }

            for (var i = 0; i < mc.Count; i++)
            {
                if (mc[i] is ToolStripDropDownItem)
                {
                    menuSortLoop(((ToolStripDropDownItem)mc[i]).DropDownItems);
                }
            }
            var mis = new ArrayList();
            foreach (ToolStripItem mi in mc)
            {
                mis.Add(mi);
            }
            mis.Sort(new Sorter(_menuorder));
            mc.Clear();
            for (var i = 0; i < mis.Count; i++)
            {
                mc.Add((ToolStripItem)mis[i]);
            }
        }
        private void menuSetOrderLoopProc(ToolStripItemCollection mc)
        {
            if (mc == null)
            {
                return;
            }
            for (var i = 0; i < mc.Count; i++)
            {
                var mi = mc[i];
                if (_menuorder[mi] == null)
                {
                    _menuorder[mi] = (i + 1) * 100;
                }
                if (mi is ToolStripDropDownItem)
                {
                    menuSetOrderLoopProc(((ToolStripDropDownItem)mi).DropDownItems);
                }
            }
        }

        private void loopProc(XmlNode current)
        {
            var tag = current.Name.ToLower();
            if (tag == "group")
            {
                //_group.Push(((fgBase)_group.Peek()).AddChildGroup());	���̃R�����g�ŁA�O���[�v���g�p���Ȃ��悤�ɂ��Ă���Bby Tono
                foreach (XmlNode child in current.ChildNodes)
                {
                    loopProc(child);
                }
                //				if( _group.Count > 1 )
                //				{
                //					_group.Pop();
                //				}
            }
            if (tag == "implement")
            {
                procImplement(current);
            }
            if (tag == "feature")
            {
                procFeature(current);
            }
            if (tag == "menuitem")
            {
                procMenuItem(current, null);
            }
        }
        private void procMenuItem(XmlNode node, FeatureBase feature)
        {
            try
            {
                if (node.Attributes["menu"] != null)
                {
                    var ms = node.Attributes["menu"].Value;
                    var mss = ms.Split(new char[] { '/' });
                    var mcol = _form.MainMenuStrip.Items;
                    ToolStripItem foundMenu = null;

                    int i;
                    for (i = 0; i < mss.Length; i++)
                    {
                        var isFound = false;
                        if (mss[i] != "-")
                        {
                            foreach (ToolStripItem mi in mcol)
                            {
                                if (mi.Text == mss[i])
                                {
                                    if (mi is ToolStripDropDownItem)
                                    {
                                        mcol = ((ToolStripDropDownItem)mi).DropDownItems;
                                        foundMenu = mi;
                                        isFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (isFound == false)
                        {
                            break;
                        }
                    }
                    for (; i < mss.Length; i++)
                    {
                        if (i == mss.Length - 1)
                        {
                            // �t�B�[�`���[�N���p�̃��j���[���쐬
                            ToolStripItem mi;
                            if (feature != null)
                            {
                                mi = new TMenuListener2
                                {
                                    Text = mss[i]
                                };
                                mcol.Add(mi);
                                ((TMenuListener2)mi).LinkFeature(feature);
                            }
                            else
                            {
                                if (mss[i] == "-")
                                {
                                    mi = new ToolStripSeparator();
                                }
                                else
                                {
                                    mi = new ToolStripMenuItem
                                    {
                                        Text = mss[i]
                                    };
                                }
                                mcol.Add(mi);
                            }

                            // �C���[�W��o�^
                            try
                            {
                                if (_resMan != null)
                                {
                                    if (mi is ToolStripItem)
                                    {
                                        if (node.Attributes["image"] != null)
                                        {
                                            var image = node.Attributes["image"].Value;
                                            var tarImage = _resMan.GetObject(image) as Image;
                                            mi.Image = tarImage;
                                        }
                                        //if( node.Attributes["tooltip"] != null)
                                        //{
                                        //	string meskey = node.Attributes["tooltip"].Value;
                                        //	var ttmes = uMes.Current[meskey];
                                        //	mi.ToolTipText = ttmes == null || ttmes == "" ? meskey : ttmes;
                                        //}
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                            // �V���[�g�J�b�g������
                            try
                            {
                                if (mi is ToolStripMenuItem)
                                {
                                    int id;
                                    var sm = (ToolStripMenuItem)mi;
                                    var ks = Keys.None;
                                    if (node.Attributes["shortcut"] != null)
                                    {
                                        var scs = node.Attributes["shortcut"].Value;
                                        if ((id = scs.ToLower().IndexOf("ctrl")) >= 0)
                                        {
                                            ks |= Keys.Control;
                                            scs = scs.Substring(0, id) + scs.Substring(id + 4);
                                        }
                                        if ((id = scs.ToLower().IndexOf("alt")) >= 0)
                                        {
                                            ks |= Keys.Alt;
                                            scs = scs.Substring(0, id) + scs.Substring(id + 3);
                                        }
                                        if ((id = scs.ToLower().IndexOf("shift")) >= 0)
                                        {
                                            ks |= Keys.Shift;
                                            scs = scs.Substring(0, id) + scs.Substring(id + 5);
                                        }

                                        var fi = typeof(Keys).GetField(scs);

                                        var sc = fi.GetValue(null);
                                        ks |= (Keys)sc;
                                        sm.ShortcutKeys = ks;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                            // ���j���[�I�[�_�[�i���ԁj���L�^
                            try
                            {
                                if (node.Attributes["menuorder"] != null)
                                {
                                    var mo = node.Attributes["menuorder"].Value;
                                    _menuorder[mi] = int.Parse(mo);
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else
                        {
                            // �|�b�v�A�b�v���j���[���쐬
                            var mi = new ToolStripDropDown
                            {
                                Text = mss[i]
                            };
                            //mcol.Add(mi.Text);
                            mcol = mi.Items;
                        }
                    }
                    // �R���g���[���ƃ����N���Ă���t�B�[�`���[�Ȃǂ́ACanStart�ŁA�R���g���[����Enable�𑀍삷��̂ł����ň�x�s���Ă���
                    if (feature != null)
                    {
                        var dummy = feature.CanStart;
                    }
                }
            }
            catch (Exception)
            {
                //System.Diagnostics.Debug.WriteLine( e.Message, e.StackTrace );
            }
        }

        private void procImplement(XmlNode node)
        {
            try
            {
                var typeData = getType(node.Attributes["Data"].Value);
                var typeParts = getType(node.Attributes["Parts"].Value);
                var typeLink = getType(node.Attributes["Link"].Value);

                _root.AssignAppData((DataHotBase)Activator.CreateInstance(typeData));
                _root.AssignPartsSet((PartsCollectionBase)Activator.CreateInstance(typeParts));
                _root.AssignLink((DataLinkBase)Activator.CreateInstance(typeLink));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Feature Loader Error ��O : '" + e.Message + "'");
            }
        }

        /// <summary>
        /// �X�L��������A�Z���u��
        /// </summary>
        private static IDictionary namespaces = null;

        /// <summary>
        /// �A�Z���u���𒴂��Č^���擾����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Type getType(string name)
        {
            if (name.Trim().Length < 1)
            {
                return null;
            }
            if (namespaces == null)
            {
                namespaces = new Hashtable();
                if (_assemblyList == null)
                {
                    FeatureLoader2.SetUsingClass(GetType());
                }

                foreach (Assembly a in _assemblyList.Values)
                {
                    foreach (var t in a.GetTypes())
                    {
                        var ss = a.FullName.Split(new char[] { ',' });
                        var s = t.Namespace + ".@CLASSNAME@, " + ss[0];
                        namespaces[s] = a;
                    }
                }
            }
            Type type = null;
            foreach (string a in namespaces.Keys)
            {
                type = Type.GetType(a.Replace("@CLASSNAME@", name));
                if (type != null)
                {
                    break;
                }
            }
            System.Diagnostics.Debug.Assert(type != null, string.Format("Class '{0}' registerd in a portfolio is not in this application.", name));
            return type;
        }

        private static readonly FieldInfo _fiParamString = typeof(FeatureBase).GetField("_paramString", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private void procFeature(XmlNode node)
        {
            try
            {
                var nameAtt = node.Attributes["class"];
                if (nameAtt != null)
                {
                    var type = getType(nameAtt.Value);
                    if (type != null)
                    {
                        var feature = ((FeatureGroupBase)_group.Peek()).AddChildFeature(type);
                        try
                        {
                            var swAtt = node.Attributes["name"];
                            if (swAtt != null)
                            {
                                if (swAtt.Value != null && swAtt.Value.Length > 0)
                                {
                                    feature.Name = swAtt.Value;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            var swAtt = node.Attributes["com"];
                            if (swAtt != null)
                            {
                                if (swAtt.Value != null && swAtt.Value.Length > 0)
                                {
                                    feature.CommandParameter = swAtt.Value;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            var swAtt = node.Attributes["enabled"];
                            if (swAtt != null)
                            {
                                if (Const.IsFalse(swAtt.Value))
                                {
                                    feature.Enabled = false;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }                   // ���j���[���
                                            // �t�B�[�`���[�Ɉ��������蓖�Ă�
                        try
                        {
                            if (node.InnerText != "")
                            {
                                _fiParamString.SetValue(feature, node.InnerText);
                                feature.ParseParameter(node.InnerText);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        procMenuItem(node, feature);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Feature Loader Error : '" + nameAtt.Value + "'�͎�������Ă��Ȃ��t�B�[�`���[�ł�");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Feature Loader Error ��O : '" + e.Message + "'");
            }
        }

        /// <summary>
        /// �w�肵��uFeatureLoader�p�t�@�C�������݂��邩��������
        /// </summary>
        /// <param name="file">��������t�@�C����</param>
        /// <returns>�������� True:�݂� / False:����</returns>
        public static bool FileExists(string file)
        {
            var fname = FileUtil.MakeMesFilename(file);
            return File.Exists(fname);
        }
    }
}