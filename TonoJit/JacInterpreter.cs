﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// Just-in-time model as a Code interpreter
/// </summary>
/// <see cref="UnitTestProject1.TonoJit_JaC.cs"/>
/// <remarks>
/// [1] to create a new instance.
/// new [Type]
/// 
/// [1-1] get return
/// a = new [Type]
/// 
/// ** example **
/// a = new Process
/// 
/// [2] to create a new initialized instance
/// new [Type1] 
/// ____[Property1-1] = [Object1-1]
/// ____[Property1-2] = [Object1-2]
/// 
/// ** example **
/// new Process
///     Name = "TestProcess"
///     
/// [3] nest property
/// new [Type1]
/// ____[Property1-1]
/// ____ ___[Property1-1-1] = [Object1-1-1]
/// ____ ___[Property1-1-2] = new [Type1-1-2]
/// ____ ___ ___[Propery1-1-2-1] = [Object1-1-2-1]
/// ____ ___[Property1-1-3] = [Variable1-1-3] new [Type1-1-3]
/// ____ ___ ___[Property1-1-3-1] = [Object1-1-3-1]
/// 
/// 
/// [4] Collection
/// new [Type1]
/// ____[Property1-1]  // collection property that set [JacListAdd] or [JacListRemove]
/// ____ ___add [Object1-1-1]
/// ____ ___add [Variable1-1-2] = [Object1-1-2]
/// ____ ___remove [Object1-1-3]
/// ____ ___add new [Type1-1-4]
/// ____ ___ ___[Property1-1-4-1] = [Object1-1-4-1]
/// </remarks>
/// <example>
/// </example>
namespace Tono.Jit
{
    /// <summary>
    /// Jac : Jit model as a Code
    /// Interpreter object
    /// </summary>
    public class JacInterpreter
    {
        private static readonly Dictionary<string/*JacTarget.Name*/, Type> JacTargets = new Dictionary<string, Type>();
        private readonly Dictionary<string, object> Vars = new Dictionary<string, object>();  // variables buffer
        private readonly Stack<(int Level, string Com)> rpnStack = new Stack<(int Level, string Com)>();
        private readonly Stack<(int Level, string Com)> opeStack = new Stack<(int Level, string Com)>();
        private readonly Dictionary<string/*instance Key*/, object> instanceBuf = new Dictionary<string, object>();
        private readonly Dictionary<string/*variable name*/, object> varBuf = new Dictionary<string, object>();
        private int instanceIdCounter = 0;

        /// <summary>
        /// Static constructor
        /// </summary>
        static JacInterpreter()
        {
            RegisterJacTarget(typeof(JacInterpreter).Assembly);
        }


        /// <summary>
        /// Parse command
        /// </summary>
        /// <param name="code"></param>
        public void Exec(string code)
        {
            // PREPARING
            NormalizeCode(code);

            // PARSING
            for (var iChunk = 0; iChunk < Levels.Count; iChunk++)
            {
                for (var iLine = 0; iLine < Lines[iChunk].Count; iLine++)
                {
                    var line = Lines[iChunk][iLine];
                    var level = Levels[iChunk][iLine];
                    var isNextLine = iLine < Lines[iChunk].Count - 1;
                    var blocks = StrUtil.SplitConsideringQuatationContainsSeparator(line, new[] { '=', ' ' }, true, true, false);

                    // PUSH PHASE =========================================
                    for (var i = 0; i < blocks.Length; i++)
                    {
                        var com = blocks[i];
                        switch (com)
                        {
                            case "=":
                            case "new":
                            case "add":
                            case "remove":
                                opeStack.Push((level, com));
                                break;
                            default:
                                rpnStack.Push((level, com));
                                break;
                        }
                    }
                    // POP PHASE =========================================
                    var isNextIndent = false;
                    var isSameIndent = false;
                    var isBackIndent = false;
                    var isNextAddRemove = false;
                    var nextLineLevel = int.MaxValue;
                    if (isNextLine)
                    {
                        isNextIndent = Levels[iChunk][iLine + 1] > Levels[iChunk][iLine];
                        isBackIndent = Levels[iChunk][iLine + 1] < Levels[iChunk][iLine];
                        isSameIndent = Levels[iChunk][iLine + 1] == Levels[iChunk][iLine];
                        nextLineLevel = Levels[iChunk][iLine + 1];
                        var nextTrimmed = Lines[iChunk][iLine + 1].Trim();
                        isNextAddRemove = nextTrimmed.StartsWith("add") | nextTrimmed.StartsWith("remove");
                    }
                    if (opeStack.Count > 0 && isNextIndent)
                    {
                        var ope = opeStack.Peek();
                        switch (ope.Com)
                        {
                            case "new":
                                opeStack.Pop();
                                ProcNew(rpnStack);
                                break;
                        }
                    }
                    if (opeStack.Count > 0 && (isBackIndent || isSameIndent || isNextLine == false))
                    {
                        TargetLevel = (nextLineLevel - 1) / tabspace * tabspace;
                        bool isStackComsuming;
                        do
                        {
                            isStackComsuming = !isNextLine;
                            var ope = opeStack.Peek();
                            switch (ope.Com)
                            {
                                case "=":
                                    opeStack.Pop();
                                    ProcSet(rpnStack);
                                    break;
                                case "add":
                                    opeStack.Pop();
                                    ProcList(rpnStack, isNextAddRemove, true);
                                    break;
                                case "remove":
                                    opeStack.Pop();
                                    ProcList(rpnStack, isNextAddRemove, false);
                                    break;
                                case "new":
                                    opeStack.Pop();
                                    ProcNew(rpnStack);
                                    break;
                            }
                            if (isStackComsuming == false && opeStack.Count > 0)
                            {
                                var nextope = opeStack.Peek();
                                if (nextope.Level > TargetLevel) // || nextope.Com == "=" && nextope.Level >= targetLevel)
                                {
                                    isStackComsuming = true;
                                }
                            }
                        } while (isStackComsuming && opeStack.Count > 0);
                    }
                }
            }
        }

        /// <summary>
        /// Parse command from JitTemplate
        /// </summary>
        /// <param name="template"></param>
        public void Exec(JitTemplate template)
        {
            foreach( var block in template.GetBlocks())
            {
                Exec(block);
            }
        }

        /// <summary>
        /// Instanciate from code
        /// </summary>
        /// <param name="jac"></param>
        /// <returns></returns>
        public static JacInterpreter From(string jac)
        {
            var ret = new JacInterpreter();
            ret.Exec(jac);
            return ret;
        }
        
        /// <summary>
        /// Instanciate from code
        /// </summary>
        /// <param name="jac"></param>
        /// <returns></returns>
        public static JacInterpreter From(JitTemplate jac)
        {
            var ret = new JacInterpreter();
            ret.Exec(jac);
            return ret;
        }


        private const int tabspace = 4;
        private readonly List<List<string>> Lines = new List<List<string>>();
        private readonly List<List<int>> Levels = new List<List<int>>();
        private int TargetLevel;

        /// <summary>
        /// Normalize code string
        /// </summary>
        /// <param name="code"></param>
        private void NormalizeCode(string code)
        {
            Lines.Clear();
            Levels.Clear();
            rpnStack.Clear();
            opeStack.Clear();
            code = code.Replace('\r', '\n');
            code = code.Replace("\n\n", "\n");
            var lines = code.Split('\n').Select(a => CleanLine(a)).Where(a => a.Trim().Length > 0).ToArray();
            var levels = Collection<int>.Rep(0, lines.Length).ToArray();
            TargetLevel = int.MaxValue;

            for (var iLine = 0; iLine < lines.Length; iLine++)
            {
                var line = lines[iLine];
                int nSp = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    if (c == ' ')
                    {
                        nSp++;
                    }
                    else
                    if (c == '\t')
                    {
                        nSp = nSp / tabspace + tabspace;
                    }
                    else
                    {
                        levels[iLine] = nSp / tabspace * tabspace;
                        lines[iLine] = line.Trim();
                        break;
                    }
                }
            }
            var min = levels.Min();
            levels = levels.Select(a => a - min).ToArray();

            if (levels.Length < 0)
            {
                throw new JacException(JacException.Codes.SyntaxError, $"No data");
            }

            if (levels[0] != 0)
            {
                throw new JacException(JacException.Codes.SyntaxError, $"Indent error. It should be no indent for first line.");
            }

            List<int> lvs = null;
            List<string> lis = null;
            for (var i = 0; i < levels.Length; i++)
            {
                if (levels[i] == 0)
                {
                    Levels.Add(lvs = new List<int>());
                    Lines.Add(lis = new List<string>());
                }
                lvs.Add(levels[i]);
                lis.Add(lines[i]);
            }
        }

        private void ProcSet(Stack<(int Level, string Com)> rpnStack)
        {
            if (rpnStack.Count < 1)
            {
                throw new JacException(JacException.Codes.SyntaxError, $"Cannot set '=' when no value");
            }

            if (rpnStack.Count < 2)
            {
                throw new JacException(JacException.Codes.SyntaxError, $"Cannot set '=' when no left value");
            }

            var val = rpnStack.Pop();           // value name
            var item = ParseValue(val.Com);     // value object
            var variable = rpnStack.Pop();      // variable name

            if (rpnStack.Count > 0)
            {
                var objName = rpnStack.Peek();  // parent object name
                if (IsInstance(objName.Com) && objName.Level < variable.Level)
                {
                    var obj = ParseValue(objName.Com);
                    var pi = obj.GetType().GetProperty(variable.Com);
                    if (pi != null)
                    {
                        if (pi.GetMethod.ReturnParameter.ParameterType.Name == "Func`1")
                        {
                            switch (item?.GetType().Name)
                            {
                                case "JitStage": pi.SetValue(obj, (Func<JitStage>)(() => (JitStage)item)); break;
                                case "JitProcess": pi.SetValue(obj, (Func<JitProcess>)(() => (JitProcess)item)); break;
                                case "JitKanban": pi.SetValue(obj, (Func<JitKanban>)(() => (JitKanban)item)); break;
                                case "JitVariable": pi.SetValue(obj, (Func<JitVariable>)(() => (JitVariable)item)); break;
                                case "JitWork": pi.SetValue(obj, (Func<JitWork>)(() => (JitWork)item)); break;
                                default: throw new JacException(JacException.Codes.NotSupportLazyMethodType, $"Not supported Func<{item?.GetType().Name}> property.");
                            }
                        }
                        else
                        if (pi.GetMethod.ReturnParameter.ParameterType.Name == "JitVariable" && item != null)
                        {
                            if (item is JitVariable itemvar)
                            {
                                pi.SetValue(obj, itemvar);
                            }
                            else
                            if (item is string itemstr)
                            {
                                pi.SetValue(obj, JitVariable.From(itemstr));
                            }
                            else
                            if (item is string itemint)
                            {
                                pi.SetValue(obj, JitVariable.From(itemint));
                            }
                            else
                            if (item is string itemdouble)
                            {
                                pi.SetValue(obj, JitVariable.From(itemdouble));
                            }
                            else
                            {
                                throw new JacException(JacException.Codes.NotSupportedType, $"Type {item.GetType().Name} is not support to JitVariable.Value");
                            }
                        }
                        else
                        {
                            pi.SetValue(obj, item);
                            if (variable.Com.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                            {
                                varBuf[ParseValue(val.Com)?.ToString()] = obj;
                            }
                        }
                        return;
                    }
                    else
                    {
                        throw new JacException(JacException.Codes.NotImplementedProperty, $"Property name '{variable.Com}' is not implemented yet in {(obj?.GetType().Name ?? "null")}.");
                    }
                }
            }
            varBuf[variable.Com] = ParseValue(val.Com);
            rpnStack.Push(variable);
        }

        private void ProcList(Stack<(int Level, string Com)> rpnStack, bool isNextAddRemove, bool isAdd)
        {
            if (rpnStack.Count < 3)
            {
                throw new JacException(JacException.Codes.SyntaxError, $"should chain next command : obj collection {(isAdd ? "add" : "remove")} value");
            }

            var attrType = isAdd ? typeof(JacListAddAttribute) : typeof(JacListRemoveAttribute);
            var itemName = rpnStack.Pop();
            var itemValue = ParseValue(itemName.Com);
            var collectionName = rpnStack.Pop();
            var parentObjectName = rpnStack.Pop();
            var parentObject = ParseValue(parentObjectName.Com);
            var methods =
                from method in parentObject.GetType().GetMethods()
                from atobj in method.GetCustomAttributes(attrType, true)
                let at = atobj as JacListAccessAttrubute
                where at.PropertyName == collectionName.Com
                select method;
            var tarMethod = methods.FirstOrDefault();
            if (tarMethod == null)
            {
                throw new JacException(JacException.Codes.NotImplementedMethod, $"There is no method implemented that have [{attrType.Name} PropertyName=\"{collectionName.Com}\"] in {parentObjectName.Com}");
            }
            tarMethod?.Invoke(parentObject, new[] { itemValue });

            rpnStack.Push(parentObjectName);
            if (isNextAddRemove)
            {
                rpnStack.Push(collectionName);
            }
        }


        private void ProcNew(Stack<(int Level, string Com)> rpnStack)
        {
            if (rpnStack.Count < 1)
            {
                throw new JacException(JacException.Codes.SyntaxError, $"Cannot new empty instance");
            }

            var typeName = rpnStack.Pop();
            var instanceKey = $"::Jac:Instance:{typeName.Com}:{++instanceIdCounter}::";
            if (JacTargets.TryGetValue(typeName.Com, out var type))
            {
                var instance = type.Assembly.CreateInstance(type.FullName);
                var pi = type.GetProperty("Name");
                if (pi != null)
                {
                    pi.SetValue(instance, instanceKey); // Set Name Property
                }
                instanceBuf[instanceKey] = instance;
                rpnStack.Push((typeName.Level, instanceKey));
            }
            else
            {
                throw new JacException(JacException.Codes.TypeNotFound, $"Type {typeName.Com} not found.");
            }
        }

        private static readonly Regex chkTimeSpan = new Regex(@"^[0-9]+(\.[0-9]*)?(ms|s|m|h|d|w)$");
        private static readonly Regex chkInteger = new Regex(@"^[0-9]+$");
        private static readonly Regex chkDouble = new Regex(@"^[0-9]+(\.[0-9]*)?$");

        /// <summary>
        /// Get value managed instance name, variable, string and some object parsing.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private object ParseValue(object value)
        {
            if (value is string valuestr)
            {
                if (chkTimeSpan.IsMatch(valuestr))
                {
                    return ParseTimeSpan(valuestr);
                }
                if (chkInteger.IsMatch(valuestr))
                {
                    return int.Parse(valuestr);
                }
                if (chkDouble.IsMatch(valuestr))
                {
                    return double.Parse(valuestr);
                }
                if (valuestr.StartsWith("'") && valuestr.EndsWith("'") || valuestr.StartsWith("\"") && valuestr.EndsWith("\""))
                {
                    return ParseValue(valuestr.Substring(1, valuestr.Length - 2));
                }
                if (instanceBuf.TryGetValue(valuestr, out var ret1))
                {
                    return ret1;
                }
                if (varBuf.TryGetValue(valuestr, out var ret2))
                {
                    return ret2;
                }
            }
            return value;
        }

        /// <summary>
        /// Parse TimeSpan string
        /// </summary>
        /// <param name="valuestr"></param>
        /// <returns></returns>
        private TimeSpan ParseTimeSpan(string valuestr)
        {
            double val;
            string unit;
            if (valuestr.EndsWith("ms"))
            {
                unit = "ms";
                val = double.Parse(valuestr.Substring(0, valuestr.Length - 2));
            }
            else
            {
                unit = StrUtil.Right(valuestr, 1);
                val = double.Parse(valuestr.Substring(0, valuestr.Length - 1));
            }
            switch (unit)
            {
                case "ms":
                    return TimeSpan.FromMilliseconds(val);
                case "s":
                    return TimeSpan.FromSeconds(val);
                case "m":
                    return TimeSpan.FromMinutes(val);
                case "h":
                    return TimeSpan.FromHours(val);
                case "d":
                    return TimeSpan.FromDays(val);
                case "w":
                    return TimeSpan.FromDays(val * 7);
                default:
                    throw new JacException(JacException.Codes.NotSupportedUnit, $"Unit '{unit}' is not supported.");
            }
        }

        /// <summary>
        /// Register your assembly that contains Jac Targets
        /// </summary>
        /// <param name="assempbly"></param>
        public static void RegisterJacTarget(Assembly assempbly)
        {
            var tars =
                from type in assempbly.GetTypes()
                from attr0 in type.GetCustomAttributes(typeof(JacTargetAttribute), false)
                let attr = attr0 as JacTargetAttribute
                select (attr.Name, type);
            foreach (var tar in tars)
            {
                JacTargets[tar.Name] = tar.type;
            }
        }

        /// <summary>
        /// Access to Variable object
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public object this[string varname]
        {
            get => ParseValue(varname);
            set => varBuf[varname] = value;
        }

        /// <summary>
        /// Get variable as a Template
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public JitTemplate Template(string varname)
        {
            return ParseValue(varname) as JitTemplate;
        }

        /// <summary>
        /// Get variable as a Stage
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public JitStage Stage(string varname)
        {
            return ParseValue(varname) as JitStage;
        }

        /// <summary>
        /// Get variable as a Process
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public JitProcess Process(string varname)
        {
            return ParseValue(varname) as JitProcess;
        }

        /// <summary>
        /// Get variable as a Work object
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public JitWork Work(string varname)
        {
            return ParseValue(varname) as JitWork;
        }

        /// <summary>
        /// Get variable as a Variable object
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public JitVariable Variable(string varname)
        {
            return ParseValue(varname) as JitVariable;
        }

        /// <summary>
        /// Get variable as a Kanban object
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public JitKanban Kanban(string varname)
        {
            return ParseValue(varname) as JitKanban;
        }

        /// <summary>
        /// line string cleaner like comment remove
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string CleanLine(string s)
        {
            int i = s.IndexOf("//");
            if (i >= 0)
            {
                return s.Substring(0, i).TrimEnd(); // remove line comment
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// Check instance name or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool IsInstance(string str)
        {
            return str.StartsWith("::Jac:Instance:");
        }
    }

    /// <summary>
    /// Jit model as a Code common exception object
    /// </summary>
    public class JacException : Exception
    {
        public enum Codes
        {
            TypeMismatch,
            SyntaxError,
            TypeNotFound,
            NotImplementedProperty,
            NotImplementedMethod,
            NotSupportedUnit,
            NotSupportLazyMethodType,
            NotSupportedType,
        }
        public Codes Code { get; set; }

        public JacException()
        {
        }
        public JacException(Codes code)
        {
            Code = code;
        }
        public JacException(Codes code, string message) : base(message)
        {
            Code = code;
        }
    }

    /// <summary>
    /// Class attribute : Mark to Jit model as a Code
    /// </summary>
    public class JacTargetAttribute : Attribute
    {
        public string Name { get; set; }
    }


    /// <summary>
    /// Method attribute base class of list accessor
    /// </summary>
    public abstract class JacListAccessAttrubute : Attribute
    {
        public string PropertyName { get; set; }
    }

    /// <summary>
    /// Method attribute : Mark to Jit model as a Code, support list item Add 
    /// </summary>
    public class JacListAddAttribute : JacListAccessAttrubute
    {
    }

    /// <summary>
    /// Method attribute : Mark to Jit model as a Code, support list item Remove 
    /// </summary>
    public class JacListRemoveAttribute : JacListAccessAttrubute
    {
    }
}
