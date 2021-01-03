// Copyright 2020 Felix Kahle. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FelixKahle.UnityProjectsCore
{
    public class ConfigVariableAttribute : Attribute
    {
        public string Name = null;
        public string DefaultValue = "";
        public ConfigVariable.Flags Flags = ConfigVariable.Flags.None;
        public string Description = "";
    }

    public class ConfigVariable
    {
        public static Dictionary<string, ConfigVariable> ConfigVariables;
        public static Flags DirtyFlags = Flags.None;

        private static bool initialized = false;

        public readonly string name;
        public readonly string description;
        public readonly string defaultValue;
        public readonly Flags flags;
        public bool changed;

        private string stringValue;
        private float floatValue;
        private int intValue;
        private bool boolValue;

        /// <summary>
        /// Initialize.
        /// </summary>
        public static void Init()
        {
            if (initialized)
            {
                return;
            }

            ConfigVariables = new Dictionary<string, ConfigVariable>();
            InjectAttributeConfigVariables();
            initialized = true;
        }

        /// <summary>
        /// Resets all ConfigVariables to default.
        /// </summary>
        public static void ResetAllToDefault()
        {
            foreach (var v in ConfigVariables)
            {
                v.Value.ResetToDefault();
            }
        }

        /// <summary>
        /// Saves the changed variables.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public static void SaveChangedVariables(string filename)
        {
            if ((DirtyFlags & Flags.Save) == Flags.None)
            {
                return;
            }

            Save(filename);
        }

        /// <summary>
        /// Saves.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public static void Save(string filename)
        {
            using (var st = System.IO.File.CreateText(filename))
            {
                foreach (var cvar in ConfigVariables.Values)
                {
                    if ((cvar.flags & Flags.Save) == Flags.Save)
                        st.WriteLine("{0} \"{1}\"", cvar.name, cvar.Value);
                }
                DirtyFlags &= ~Flags.Save;
            }
            Debug.Log("saved: " + filename);
        }

        /// <summary>
        /// Regex.
        /// </summary>
        private static Regex validateNameRe = new Regex(@"^[a-z_+-][a-z0-9_+.-]*$");

        /// <summary>
        /// Registers a ConfigVariable.
        /// </summary>
        /// <param name="cvar">The ConfigVariable to register.</param>
        public static void RegisterConfigVariable(ConfigVariable cvar)
        {
            if (ConfigVariables.ContainsKey(cvar.name))
            {
                Debug.LogError("Trying to register cvar " + cvar.name + " twice");
                return;
            }
            if (!validateNameRe.IsMatch(cvar.name))
            {
                Debug.LogError("Trying to register cvar with invalid name: " + cvar.name);
                return;
            }
            ConfigVariables.Add(cvar.name, cvar);
        }

        /// <summary>
        /// Flags.
        /// </summary>
        [Flags]
        public enum Flags
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// Causes the cvar to be save to settings.cfg.
            /// </summary>
            Save = 0x1,

            /// <summary>
            /// Consider this a cheat var. Can only be set if cheats enabled.
            /// </summary>
            Cheat = 0x2,

            /// <summary>
            /// These vars are sent to clients when connecting and when changed
            /// </summary>
            ServerInfo = 0x4,

            /// <summary>
            /// These vars are sent to server when connecting and when changed
            /// </summary>
            ClientInfo = 0x8,

            /// <summary>
            /// User created variable
            /// </summary>
            User = 0x10
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="flags">The flags.</param>
        public ConfigVariable(string name, string description, string defaultValue, Flags flags = Flags.None)
        {
            this.name = name;
            this.flags = flags;
            this.description = description;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Getter and setter for the value.
        /// </summary>
        public virtual string Value
        {
            get { return stringValue; }
            set
            {
                if (stringValue == value)
                {
                    return;
                }
                DirtyFlags |= flags;
                stringValue = value;
                if (!int.TryParse(value, out intValue))
                {
                    intValue = 0;
                }
                if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out floatValue))
                {
                    floatValue = 0;
                }
                if(!bool.TryParse(value, out boolValue))
                {
                    boolValue = false;
                }
                changed = true;
            }
        }

        /// <summary>
        /// Getter for the int value.
        /// </summary>
        public int IntValue
        {
            get { return intValue; }
        }

        /// <summary>
        /// Getter for the float value.
        /// </summary>
        public float FloatValue
        {
            get { return floatValue; }
        }

        /// <summary>
        /// Getter for the bool value.
        /// </summary>
        public bool BoolValue
        {
            get { return boolValue; }
        }

        /// <summary>
        /// Injects the attribute config variables.
        /// </summary>
        private static void InjectAttributeConfigVariables()
        {
            Debug.Log("count:" + AppDomain.CurrentDomain.GetAssemblies().Length);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var currentClass in assembly.GetTypes())
                    {
                        if (!currentClass.IsClass)
                        {
                            continue;
                        }
                        foreach (var field in currentClass.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
                        {
                            if (!field.IsDefined(typeof(ConfigVariableAttribute), false))
                            {
                                continue;
                            }
                            if (!field.IsStatic)
                            {
                                Debug.LogError("Cannot use ConfigVar attribute on non-static fields");
                                continue;
                            }
                            if (field.FieldType != typeof(ConfigVariable))
                            {
                                Debug.LogError("Cannot use ConfigVar attribute on fields not of type ConfigVar");
                                continue;
                            }
                            var attr = field.GetCustomAttributes(typeof(ConfigVariableAttribute), false)[0] as ConfigVariableAttribute;
                            var name = attr.Name != null ? attr.Name : currentClass.Name.ToLower() + "." + field.Name.ToLower();
                            var cvar = field.GetValue(null) as ConfigVariable;
                            if (cvar != null)
                            {
                                Debug.LogError("ConfigVars (" + name + ") should not be initialized from code; just marked with attribute");
                                continue;
                            }
                            cvar = new ConfigVariable(name, attr.Description, attr.DefaultValue, attr.Flags);
                            cvar.ResetToDefault();
                            RegisterConfigVariable(cvar);
                            field.SetValue(null, cvar);
                        }
                    }
                }
                catch (System.Reflection.ReflectionTypeLoadException)
                {
                    Debug.LogWarning("Unable to load types for assembly " + assembly.FullName);
                }
            }

            // Clear dirty flags as default values shouldn't count as dirtying
            DirtyFlags = Flags.None;
        }

        /// <summary>
        /// Resets to default.
        /// </summary>
        void ResetToDefault()
        {
            this.Value = defaultValue;
        }

        /// <summary>
        /// Checks if a change occured.
        /// </summary>
        /// <returns>True, if a change occured, false otherwise.</returns>
        public bool ChangeCheck()
        {
            if (!changed)
            {
                return false;
            }
            changed = false;
            return true;
        }
    }
}
