using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace FrameWork
{
    public class DebugUtil
    {

        public static bool LogEnable = true;

        [Conditional("UNITY_EDITOR")]
        static public void Assert(bool test, string assertString)
        {
#if UNITY_EDITOR
            if (!test)
            {
                StackTrace trace = new StackTrace(true);
                StackFrame frame = trace.GetFrame(1);

                string assertInformation;
                assertInformation = "Filename: " + frame.GetFileName() + "\n";
                assertInformation += "Method: " + frame.GetMethod() + "\n";
                assertInformation += "Line: " + frame.GetFileLineNumber();

                UnityEngine.Debug.Break();

                string assertMessage = assertString + "\n\n" + assertInformation;
                if (UnityEditor.EditorUtility.DisplayDialog("Assert!", assertMessage, "OK"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(frame.GetFileName(), frame.GetFileLineNumber());
                    UnityEngine.Debug.Log(assertInformation);
                }
            }
#endif
        }

        [Conditional("UNITY_EDITOR")]
        static public void FixMaterialInEditor(GameObject go)
        {
#if UNITY_EDITOR
            var renders = go.GetComponentsInChildren<Renderer>();
            foreach (var render in renders)
            {
                render.material.shader = Shader.Find(render.material.shader.name);
            }
#endif
        }


        public static void Log(object obj, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                string msg = obj == null ? "NULL" : obj.ToString();

                Log(msg, args);
            }
#endif
        }
        
        public static void Log(string str, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                if (args != null && args.Length > 0)
                {
                    str = GetLogString(str, args);
                }
#if DEBUG_LOG_EXTEND
                UnityEngine.Debug.Log($"{GetLogModuleName()} [I]> {str}");
#else
                UnityEngine.Debug.Log($"[I]> {str}");
#endif
            }
#endif
        }

        public static void LogWarning(object obj, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                string msg = obj == null ? "NULL" : obj.ToString();

                LogWarning(msg, args);
            }
#endif
        }

        public static void LogWarning(string str, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                if (args != null && args.Length > 0)
                {
                    str = GetLogString(str, args);
                }
#if DEBUG_LOG_EXTEND
                UnityEngine.Debug.LogWarning($"{GetLogModuleName()} [W]> {str}");
#else
                UnityEngine.Debug.LogWarning($"[W]> {str}");
#endif
            }
#endif
        }


        public static void LogError(object obj, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                string msg = obj == null ? "NULL" : obj.ToString();

                LogError(msg, args);
            }
#endif
        }

        public static void LogError(string str, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                if (args != null && args.Length > 0)
                {
                    str = GetLogString(str, args);
                }
#if DEBUG_LOG_EXTEND
                UnityEngine.Debug.LogError($"{GetLogModuleName()} [E]> {str}");
#else
                UnityEngine.Debug.LogError($"[E]> {str}");
#endif
            }
#endif
        }

        public static void ThrowException(string str, params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                if (args != null && args.Length > 0)
                {
                    str = GetLogString(str, args);
                }
                throw new Exception("[EX]> " + str);
            }
#endif
        }

#if DEBUG || DEVELOPMENT_BUILD
        private static string GetLogModuleName()
        {
            StackTrace st = new StackTrace();
            var sfs = st.GetFrames();
            string ret;
            if (sfs?.Length > 3)
            {
                Stack<string> infoStack = new Stack<string>();
                infoStack.Push(GetDebugLogModuleFlag(sfs[2], true));
                for (var i = 3; i < sfs.Length; ++i)
                {
                    var info = GetDebugLogModuleFlag(sfs[i]);
                    if (info != null && infoStack.Peek() != info)
                    {
                        infoStack.Push(info);
                    }
                }

                StringBuilder sb = new StringBuilder();
                for (var i = 0; i < infoStack.Count - 1; ++i)
                {
                    sb.Append($"[{infoStack.Pop()}]");
                }
                sb.Append($"<{infoStack.Pop()}>");
                ret = sb.ToString();
            }
            else
            {
                if (sfs != null)
                {
                    ret = $"<{GetDebugLogModuleFlag(sfs[sfs.Length - 1], true)}>";
                }
                else
                {
                    ret = string.Empty;
                }
            }
            return ret;
        }

        private static string GetDebugLogModuleFlag(System.Diagnostics.StackFrame stackFrame, bool acceptName = false)
        {
            var method = stackFrame?.GetMethod();
            if (method == null)
            {
                return null;
            }

            Type declaringType = null;
            if (method.Name == "MoveNext") //这是一个迭代器（可能是协程），应该去找定义类的定义类
            {
                declaringType = stackFrame?.GetMethod()?.DeclaringType?.DeclaringType;
            }
            else
            {
                declaringType = stackFrame?.GetMethod()?.DeclaringType;
            }

            
            if (declaringType == null)
            {
                return "EmptyDeclaringType";
            }
            var ret = (declaringType.GetCustomAttribute(typeof(DebugLogModuleFlag)) as DebugLogModuleFlag)?.ModuleFlag;
            if (ret == null && acceptName)
            {
                ret = declaringType.Name;
            }
            return ret;
        }
#endif
        
        private static string GetLogString(string str, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            DateTime now = DateTime.Now;

            sb.Append(now.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")));
            sb.Append(" ");

            sb.AppendFormat(str, args);

            return sb.ToString();
        }
        
        //------Colorful Log-----
        public static void LogG(string str,[CallerMemberName]string caller="")
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
                UnityEngine.Debug.Log($"<color=#00BB00>({caller}) {str}</color>" );
#endif
        }
        public static void LogP(string str, [CallerMemberName]string caller="")
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
                UnityEngine.Debug.Log($"<color=#ff99ff>({caller}) {str}</color>" );
#endif
        }
        public static void LogY(string str, [CallerMemberName]string caller="")
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
                UnityEngine.Debug.Log($"<color=#FFFF66>({caller}) {str}</color>" );
#endif
        }
        
        //--------Project Transfer Debug---------
        public static void LogPjTransTip(string msg,params object[] args)
        {
#if DEBUG || DEVELOPMENT_BUILD
            if (LogEnable)
            {
                if (args != null && args.Length > 0)
                {
                    msg = GetLogString(msg, args);
                }
                UnityEngine.Debug.Log("[project迁移] -->" + msg);
            }
#endif
        }
    }

    public class DebugLogModuleFlag : Attribute
    {
        public readonly string ModuleFlag;

        public DebugLogModuleFlag(string moduleFlag)
        {
            ModuleFlag = moduleFlag;
        }
    }
}

