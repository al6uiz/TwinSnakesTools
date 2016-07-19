using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace TwinSnakesTools.GUI
{
    public abstract class ThemePalette : Freezable
    {
        // Methods
        internal ThemePalette()
        {
        }

        internal static Color ColorFromLongValue(long colorValue)
        {
            byte a = (byte)(colorValue >> 0x18);
            byte r = (byte)((colorValue & 0xff0000L) >> 0x10);
            byte g = (byte)((colorValue & 0xff00L) >> 8);
            byte b = (byte)(colorValue & 0xffL);
            return Color.FromArgb(a, r, g, b);
        }

        protected sealed override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        internal static void FreezeAndSetResource(ResourceDictionary resourceDictioanry, object key, object resource)
        {
            Freezable freezable = resource as Freezable;
            if (freezable != null)
            {
                freezable.Freeze();
            }
            resourceDictioanry[key] = resource;
        }

        protected sealed override bool FreezeCore(bool isChecking)
        {
            if (!isChecking)
            {
                this.FreezeOverride();
            }
            return base.FreezeCore(isChecking);
        }

        internal abstract void FreezeOverride();
        private static DependencyProperty GetBindingProperty(DependencyObject freezable)
        {
            if (freezable is SolidColorBrush)
            {
                return SolidColorBrush.ColorProperty;
            }
            if (freezable is GradientStop)
            {
                return GradientStop.ColorProperty;
            }
            return null;
        }

        private static void OnFreezableChanged(object sender, EventArgs e)
        {
            Freezable freezable = sender as Freezable;
            DependencyProperty bindingProperty = GetBindingProperty(freezable);
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(freezable, bindingProperty);
            if (bindingExpression != null)
            {
                freezable.Changed -= new EventHandler(ThemePalette.OnFreezableChanged);
                bindingExpression.UpdateTarget();
                freezable.SetValue(bindingProperty, freezable.GetValue(bindingProperty));
                BindingOperations.ClearAllBindings(freezable);
                freezable.Freeze();
            }
        }

        internal static void OnIsFreezableChanged(DependencyObject d, DependencyPropertyChangedEventArgs args, DependencyProperty isFrozenProperty, DependencyObject freezeSource)
        {
            if (!((bool)args.NewValue))
            {
                throw new InvalidOperationException("ThemeResources.IsFreezable attached property can not be unset");
            }
            Freezable freezable = d as Freezable;
            if (freezable != null)
            {
                if ((bool)freezeSource.GetValue(isFrozenProperty))
                {
                    TryFreeze(freezable);
                }
                else if (freezable.ReadLocalValue(isFrozenProperty) == DependencyProperty.UnsetValue)
                {
                    Binding binding = new Binding
                    {
                        Path = new PropertyPath(isFrozenProperty),
                        Source = freezeSource
                    };
                    BindingOperations.SetBinding(freezable, isFrozenProperty, binding);
                }
            }
        }

        internal static void OnIsFrozenChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue)
            {
                Freezable freezable = d as Freezable;
                if (freezable != null)
                {
                    TryFreeze(freezable);
                }
            }
        }

        internal virtual void OnResourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnResourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePalette palette = d as ThemePalette;
            if (palette != null)
            {
                palette.OnResourcePropertyChanged(e);
            }
        }

        internal static DependencyProperty RegisterColor<Palette>(string propertyName, long color) where Palette : DependencyObject
        {
            byte a = (byte)(color >> 0x18);
            byte r = (byte)((color & 0xff0000L) >> 0x10);
            byte g = (byte)((color & 0xff00L) >> 8);
            byte b = (byte)(color & 0xffL);
            return DependencyProperty.Register(propertyName, typeof(Color), typeof(Palette), new PropertyMetadata(Color.FromArgb(a, r, g, b), new PropertyChangedCallback(ThemePalette.OnResourcePropertyChanged)));
        }

        internal static DependencyProperty RegisterCornerRadius<Palette>(string propertyName, double valueLeft, double valueTop, double valueRight, double valueBottom)
        {
            return DependencyProperty.Register(propertyName, typeof(CornerRadius), typeof(Palette), new PropertyMetadata(new CornerRadius(valueLeft, valueTop, valueRight, valueBottom), new PropertyChangedCallback(ThemePalette.OnResourcePropertyChanged)));
        }

        internal static DependencyProperty RegisterDouble<Palette>(string propertyName, double value)
        {
            return DependencyProperty.Register(propertyName, typeof(double), typeof(Palette), new PropertyMetadata(value, new PropertyChangedCallback(ThemePalette.OnResourcePropertyChanged)));
        }

        internal static DependencyProperty RegisterFontFamily<Palette>(string propertyName, string family)
        {
            return DependencyProperty.Register(propertyName, typeof(FontFamily), typeof(Palette), new PropertyMetadata(new FontFamily(family), new PropertyChangedCallback(ThemePalette.OnResourcePropertyChanged)));
        }

        internal static Color TransformAlpha(Color baseColor, byte alpha)
        {
            baseColor.A = alpha;
            return baseColor;
        }

        private static void TryFreeze(Freezable freezable)
        {
            DependencyProperty bindingProperty = GetBindingProperty(freezable);
            if (bindingProperty != null)
            {
                BindingExpression bindingExpression = BindingOperations.GetBindingExpression(freezable, bindingProperty);
                if (bindingExpression == null)
                {
                    freezable.Changed += new EventHandler(ThemePalette.OnFreezableChanged);
                }
                else
                {
                    bindingExpression.UpdateTarget();
                    freezable.SetValue(bindingProperty, freezable.GetValue(bindingProperty));
                    BindingOperations.ClearAllBindings(freezable);
                    freezable.Freeze();
                }
            }
        }
    }


    public sealed class VisualStudio2013ResourceDictionary : ResourceDictionary
    {
        // Methods
        public VisualStudio2013ResourceDictionary()
        {
            base.MergedDictionaries.Add(VisualStudio2013Palette.ResourceDictionary);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    public sealed class VisualStudio2013ResourceExtension : DynamicResourceExtension
    {
   

        // Properties
        public VisualStudio2013ResourceKey KeyName
        {
            get
            {
                return (base.ResourceKey as VisualStudio2013ResourceKey);
            }
            set
            {
                base.ResourceKey = value;
            }
        }
    }






    [TypeConverter(typeof(VisualStudio2013ResourceKeyTypeConverter))]
    public sealed class VisualStudio2013ResourceKey : ResourceKey
    {
        // Fields
        public static readonly ResourceKey AccentBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.AccentBrush);
        public static readonly ResourceKey AccentDarkBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.AccentDarkBrush);
        public static readonly ResourceKey AccentMainBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.AccentMainBrush);
        public static readonly ResourceKey AlternativeBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.AlternativeBrush);
        public static readonly ResourceKey BasicBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.BasicBrush);
        public static readonly ResourceKey ComplementaryBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.ComplementaryBrush);
        public static readonly ResourceKey DefaultForegroundColor = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.DefaultForegroundColor);
        public static readonly ResourceKey FontFamily = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontFamily);
        public static readonly ResourceKey FontSize = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSize);
        public static readonly ResourceKey FontSizeL = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeL);
        public static readonly ResourceKey FontSizeS = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeS);
        public static readonly ResourceKey FontSizeXL = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeXL);
        public static readonly ResourceKey FontSizeXS = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeXS);
        public static readonly ResourceKey FontSizeXXL = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeXXL);
        public static readonly ResourceKey FontSizeXXS = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeXXS);
        public static readonly ResourceKey FontSizeXXXS = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.FontSizeXXXS);
        public static readonly ResourceKey HeaderBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.HeaderBrush);
        public static readonly ResourceKey MainBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.MainBrush);
        public static readonly ResourceKey MarkerBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.MarkerBrush);
        public static readonly ResourceKey MouseOverBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.MouseOverBrush);
        public static readonly ResourceKey PrimaryBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.PrimaryBrush);
        public static readonly ResourceKey QualityGoodBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.QualityGoodBrush);
        public static readonly ResourceKey QualityPoorBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.QualityPoorBrush);
        public static readonly ResourceKey QualitySatisfactoryBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.QualitySatisfactoryBrush);
        public static readonly ResourceKey SelectedBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.SelectedBrush);
        public static readonly ResourceKey SemiBasicBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.SemiBasicBrush);
        public static readonly ResourceKey SemiSelectedBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.SemiSelectedBrush);
        public static readonly ResourceKey StrongBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.StrongBrush);
        public static readonly ResourceKey ValidationBrush = new VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID.ValidationBrush);

        // Methods
        internal VisualStudio2013ResourceKey(VisualStudio2013ResourceKeyID key)
        {
            this.Key = key;
        }

        public override string ToString()
        {
            return this.Key.ToString();
        }

        // Properties
        public override Assembly Assembly
        {
            get
            {
                return typeof(VisualStudio2013ResourceKey).Assembly;
            }
        }

        internal VisualStudio2013ResourceKeyID Key { get; private set; }
    }


    [EditorBrowsable(EditorBrowsableState.Never)]
    internal enum VisualStudio2013ResourceKeyID
    {
        DefaultForegroundColor,
        AccentBrush,
        AccentMainBrush,
        AccentDarkBrush,
        ValidationBrush,
        BasicBrush,
        SemiBasicBrush,
        PrimaryBrush,
        MarkerBrush,
        StrongBrush,
        AlternativeBrush,
        SelectedBrush,
        MouseOverBrush,
        ComplementaryBrush,
        MainBrush,
        HeaderBrush,
        QualityPoorBrush,
        QualitySatisfactoryBrush,
        QualityGoodBrush,
        SemiSelectedBrush,
        FontSizeXXXS,
        FontSizeXXS,
        FontSizeXS,
        FontSizeS,
        FontSize,
        FontSizeL,
        FontSizeXL,
        FontSizeXXL,
        FontFamily
    }


    public abstract class ThemeResourceKeyTypeConverter<T> : TypeConverter where T : ResourceKey
    {
        // Methods
        internal ThemeResourceKeyTypeConverter()
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (((destinationType == typeof(MarkupExtension)) && (context is IValueSerializerContext)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string str = value as string;
            if (str != null)
            {
                ResourceKey key = null;
                if (this.Keys.TryGetValue(str, out key))
                {
                    return key;
                }
                if (str.StartsWith("{x:Static telerik:") && str.EndsWith("}"))
                {
                    string[] strArray = str.Substring(0x12, str.Length - 0x13).Split(new char[] { '.' });
                    if ((strArray.Length == 2) && this.Keys.TryGetValue(strArray[1], out key))
                    {
                        return key;
                    }
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ("{x:Static telerik:" + typeof(T).Name + "." + Convert.ToString(value) + "}");
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        // Properties
        internal abstract IDictionary<string, ResourceKey> Keys { get; }
    }



    public class VisualStudio2013ResourceKeyTypeConverter : ThemeResourceKeyTypeConverter<VisualStudio2013ResourceKey>
    {
        // Fields
        private static Dictionary<string, ResourceKey> keys = new Dictionary<string, ResourceKey>();
        private static Lazy<TypeConverter.StandardValuesCollection> standardValues = new Lazy<TypeConverter.StandardValuesCollection>(() => new TypeConverter.StandardValuesCollection(keys.Keys.ToList<string>()));

        // Methods
        static VisualStudio2013ResourceKeyTypeConverter()
        {
            keys.Add(VisualStudio2013ResourceKeyID.DefaultForegroundColor.ToString(), VisualStudio2013ResourceKey.DefaultForegroundColor);
            keys.Add(VisualStudio2013ResourceKeyID.AccentBrush.ToString(), VisualStudio2013ResourceKey.AccentBrush);
            keys.Add(VisualStudio2013ResourceKeyID.AccentMainBrush.ToString(), VisualStudio2013ResourceKey.AccentMainBrush);
            keys.Add(VisualStudio2013ResourceKeyID.AccentDarkBrush.ToString(), VisualStudio2013ResourceKey.AccentDarkBrush);
            keys.Add(VisualStudio2013ResourceKeyID.ValidationBrush.ToString(), VisualStudio2013ResourceKey.ValidationBrush);
            keys.Add(VisualStudio2013ResourceKeyID.BasicBrush.ToString(), VisualStudio2013ResourceKey.BasicBrush);
            keys.Add(VisualStudio2013ResourceKeyID.SemiBasicBrush.ToString(), VisualStudio2013ResourceKey.SemiBasicBrush);
            keys.Add(VisualStudio2013ResourceKeyID.PrimaryBrush.ToString(), VisualStudio2013ResourceKey.PrimaryBrush);
            keys.Add(VisualStudio2013ResourceKeyID.MarkerBrush.ToString(), VisualStudio2013ResourceKey.MarkerBrush);
            keys.Add(VisualStudio2013ResourceKeyID.StrongBrush.ToString(), VisualStudio2013ResourceKey.StrongBrush);
            keys.Add(VisualStudio2013ResourceKeyID.AlternativeBrush.ToString(), VisualStudio2013ResourceKey.AlternativeBrush);
            keys.Add(VisualStudio2013ResourceKeyID.SelectedBrush.ToString(), VisualStudio2013ResourceKey.SelectedBrush);
            keys.Add(VisualStudio2013ResourceKeyID.MouseOverBrush.ToString(), VisualStudio2013ResourceKey.MouseOverBrush);
            keys.Add(VisualStudio2013ResourceKeyID.ComplementaryBrush.ToString(), VisualStudio2013ResourceKey.ComplementaryBrush);
            keys.Add(VisualStudio2013ResourceKeyID.MainBrush.ToString(), VisualStudio2013ResourceKey.MainBrush);
            keys.Add(VisualStudio2013ResourceKeyID.HeaderBrush.ToString(), VisualStudio2013ResourceKey.HeaderBrush);
            keys.Add(VisualStudio2013ResourceKeyID.QualityPoorBrush.ToString(), VisualStudio2013ResourceKey.QualityPoorBrush);
            keys.Add(VisualStudio2013ResourceKeyID.QualitySatisfactoryBrush.ToString(), VisualStudio2013ResourceKey.QualitySatisfactoryBrush);
            keys.Add(VisualStudio2013ResourceKeyID.QualityGoodBrush.ToString(), VisualStudio2013ResourceKey.QualityGoodBrush);
            keys.Add(VisualStudio2013ResourceKeyID.SemiSelectedBrush.ToString(), VisualStudio2013ResourceKey.SemiSelectedBrush);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeXXXS.ToString(), VisualStudio2013ResourceKey.FontSizeXXXS);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeXXS.ToString(), VisualStudio2013ResourceKey.FontSizeXXS);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeXS.ToString(), VisualStudio2013ResourceKey.FontSizeXS);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeS.ToString(), VisualStudio2013ResourceKey.FontSizeS);
            keys.Add(VisualStudio2013ResourceKeyID.FontSize.ToString(), VisualStudio2013ResourceKey.FontSize);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeL.ToString(), VisualStudio2013ResourceKey.FontSizeL);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeXL.ToString(), VisualStudio2013ResourceKey.FontSizeXL);
            keys.Add(VisualStudio2013ResourceKeyID.FontSizeXXL.ToString(), VisualStudio2013ResourceKey.FontSizeXXL);
            keys.Add(VisualStudio2013ResourceKeyID.FontFamily.ToString(), VisualStudio2013ResourceKey.FontFamily);
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return standardValues.Value;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        // Properties
        internal override IDictionary<string, ResourceKey> Keys
        {
            get
            {
                return keys;
            }
        }
    }
    public enum ThemeLocation
    {
        BuiltIn,
        External
    }




    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ThemeLocationAttribute : Attribute
    {
        // Fields
        private ThemeLocation location;

        // Methods
        public ThemeLocationAttribute(ThemeLocation location)
        {
            this.location = location;
        }

        // Properties
        public ThemeLocation Location
        {
            get
            {
                return this.location;
            }
        }
    }




    [EditorBrowsable(EditorBrowsableState.Never), ThemeLocation(ThemeLocation.BuiltIn)]
    internal class VisualStudio2013Theme : Theme
    {
        // Methods
        public VisualStudio2013Theme()
        {
        }

        public VisualStudio2013Theme(VisualStudio2013Palette.ColorVariation colorVariation)
        {
            VisualStudio2013Palette.LoadPreset(colorVariation);
        }
    }
    [TypeConverter(typeof(ThemeConverter))]
    public class Theme
    {
        // Methods
        public override string ToString()
        {
            return base.GetType().Name.Replace("Theme", string.Empty);
        }

        // Nested Types
        internal class DefaultStyleKeyHelper : Control
        {
            // Methods
            public static void SetDefaultStyleKey(Control control, object value)
            {
                if (StyleManager.IsEnabled)
                {
                    control.SetValue(FrameworkElement.DefaultStyleKeyProperty, value);
                }
            }
        }
    }

    public static class StyleManager
    {
        // Fields
        private static Theme applicationTheme;
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(StyleManager), new PropertyMetadata(new PropertyChangedCallback(StyleManager.OnThemeChanged)));

        // Methods
        static StyleManager()
        {
            IsEnabled = true;
        }
        internal interface IThemable
        {
            // Methods
            void ResetTheme();
        }




        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public sealed class ThemableAttribute : Attribute
        {
        }



        [Category("Appearance"), AttachedPropertyBrowsableWhenAttributePresent(typeof(ThemableAttribute))]
        public static Theme GetTheme(DependencyObject element)
        {
            return (Theme)element.GetValue(ThemeProperty);
        }

        private static void OnThemeChanged(DependencyObject target, DependencyPropertyChangedEventArgs changedEventArgs)
        {
            if (IsEnabled)
            {
                Theme newValue = changedEventArgs.NewValue as Theme;
                IThemable themable = target as IThemable;
                if (themable != null)
                {
                    themable.ResetTheme();
                }
                else
                {
                    Control control = target as Control;
                    if (control != null)
                    {
                        Theme.DefaultStyleKeyHelper.SetDefaultStyleKey(control, ThemeResourceKey.GetDefaultStyleKey(newValue, control.GetType()));
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public static void SetDefaultStyleKey(Control control, Type controlType)
        {
            if (IsEnabled)
            {
                ThemeResourceKey defaultStyleKey = ThemeResourceKey.GetDefaultStyleKey(GetTheme(control), controlType);
                Theme.DefaultStyleKeyHelper.SetDefaultStyleKey(control, defaultStyleKey);
            }
        }

        public static void SetTheme(DependencyObject element, Theme value)
        {
            element.SetValue(ThemeProperty, value);
        }

        public static void SetThemeFromParent(DependencyObject element, DependencyObject parent)
        {
            if (IsEnabled && ((element != null) && (parent != null)))
            {
                SetTheme(element, GetTheme(parent));
            }
        }

        // Properties
        public static Theme ApplicationTheme
        {
            get
            {
                return applicationTheme;
            }
            set
            {
                if (IsEnabled && (applicationTheme != value))
                {
                    applicationTheme = value;
                }
            }
        }

        public static bool IsEnabled
        {
            get; set;
        }
    }

    public static class CaretBrushHelper
    {
        // Fields
        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty.RegisterAttached("CaretBrush", typeof(Brush), typeof(CaretBrushHelper), new PropertyMetadata(new PropertyChangedCallback(CaretBrushHelper.OnCaretBrushChanged)));

        // Methods
        public static Brush GetCaretBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CaretBrushProperty);
        }

        private static void OnCaretBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox box = d as TextBox;
                box.CaretBrush = GetCaretBrush(box);
            }
            else if (d is PasswordBox)
            {
                PasswordBox box2 = d as PasswordBox;
                box2.CaretBrush = GetCaretBrush(box2);
            }
        }

        public static void SetCaretBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(CaretBrushProperty, value);
        }
    }




    [MarkupExtensionReturnType(typeof(ThemeResourceKey))]
    public class ThemeResourceKey : ResourceKey
    {
        // Fields
        private Type themeType;

        // Methods
        public ThemeResourceKey()
        {
        }

        public ThemeResourceKey(Type themeType, Type elementType)
        {
            this.themeType = themeType;
            this.ElementType = elementType;
        }

        public ThemeResourceKey(Type themeType, Type elementType, object resourceId)
        {
            this.themeType = themeType;
            this.ElementType = elementType;
            this.ResourceId = resourceId;
        }

        public override bool Equals(object obj)
        {
            this.VerifyParameters();
            ThemeResourceKey key = obj as ThemeResourceKey;
            if (key == null)
            {
                return false;
            }
            return (((key.ThemeType == this.ThemeType) && (key.ElementType == this.ElementType)) && object.Equals(key.ResourceId, this.ResourceId));
        }

        public static ThemeResourceKey GetDefaultStyleKey(Theme theme, Type elementType)
        {
            return new ThemeResourceKey(PrepareThemeType(theme), elementType);
        }

        public static ThemeResourceKey GetDefaultStyleKey(Theme theme, Type elementType, Type defaultTheme)
        {
            return new ThemeResourceKey((theme == null) ? defaultTheme : theme.GetType(), elementType);
        }

        public override int GetHashCode()
        {
            int num = (this.ResourceId != null) ? this.ResourceId.GetHashCode() : 0;
            if ((this.ThemeType != null) && (this.ElementType != null))
            {
                return ((this.ThemeType.GetHashCode() ^ this.ElementType.GetHashCode()) ^ num);
            }
            return num;
        }

        private static bool IsSystemType(Type type)
        {
            return type.AssemblyQualifiedName.Contains("PresentationFramework");
        }

        private static Type PrepareThemeType(Theme theme)
        {
            if (theme == null)
            {
                return ApplicationTheme.GetType();
            }
            return theme.GetType();
        }

        public override string ToString()
        {
            return string.Format("{0} {{ThemeType:{1}, ElementType:{2}, ResourceId:{3}}}", new object[] { base.GetType(), this.ThemeType, this.ElementType, this.ResourceId });
        }

        private void VerifyParameters()
        {
            if ((this.ThemeType == null) || (this.ElementType == null))
            {
                throw new ArgumentException("Must specify both theme and control type.");
            }
        }

        // Properties
        private static Theme ApplicationTheme
        {
            get
            {
                if (StyleManager.IsEnabled)
                {
                    if (StyleManager.ApplicationTheme != null)
                    {
                        return StyleManager.ApplicationTheme;
                    }
                    string str = ConfigurationManager.AppSettings["Telerik.Theme"];
                    if (!string.IsNullOrEmpty(str))
                    {
                        return (Theme)new ThemeConverter().ConvertFrom(null, CultureInfo.CurrentUICulture, str);
                    }
                }
                return DefaultSuiteTheme;
            }
        }

        public override Assembly Assembly
        {
            get
            {
                this.VerifyParameters();
                object[] customAttributes = this.ThemeType.GetCustomAttributes(typeof(ThemeLocationAttribute), false);
                ThemeLocation external = ThemeLocation.External;
                if (customAttributes.Length > 0)
                {
                    external = ((ThemeLocationAttribute)customAttributes[0]).Location;
                }
                if ((external != ThemeLocation.External) && !IsSystemType(this.ElementType))
                {
                    return this.ElementType.Assembly;
                }
                return this.ThemeType.Assembly;
            }
        }

        internal static Theme DefaultSuiteTheme
        {
            get
            {
                return ThemeManager.StandardThemes[ThemeManager.DefaultThemeName];
            }
        }

        public Type ElementType { get; set; }

        public object ResourceId { get; set; }

        public Type ThemeType
        {
            get
            {
                if (this.themeType == null)
                {
                    this.themeType = PrepareThemeType(null);
                }
                return this.themeType;
            }
            set
            {
                this.themeType = value;
            }
        }
    }


    public class ThemeConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string themeName = value as string;
            Theme theme = ThemeManager.FromName(themeName);
            if (theme == null)
            {
                theme = CreateDynamicTheme(themeName);
            }
            if (theme == null)
            {
                return base.ConvertFrom(value);
            }
            return theme;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();
        }

        private static Theme CreateDynamicTheme(string themeTypeName)
        {
            Type type = Type.GetType(themeTypeName);
            if (type != null)
            {
                return (Theme)Activator.CreateInstance(type);
            }
            return null;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new TypeConverter.StandardValuesCollection(ThemeManager.StandardThemeNames);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }



    public static class ThemeManager
    {
        // Fields
        internal static readonly string DefaultThemeName = "Office_Black";
        private static readonly List<string> standardThemeNames = new List<string>();
        public static readonly ReadOnlyCollection<string> StandardThemeNames = new ReadOnlyCollection<string>(new List<string>());
        public static readonly Dictionary<string, Theme> StandardThemes = new Dictionary<string, Theme>();

        public static Theme FromName(string themeName)
        {
            if (themeName == null)
            {
                return null;
            }
            if (StandardThemes.ContainsKey(themeName))
            {
                return StandardThemes[themeName];
            }
            return StandardThemes[DefaultThemeName];
        }

        private static void RegisterTheme(string name, Theme theme, bool isCommon)
        {
            StandardThemes.Add(name, theme);
            if (isCommon)
            {
                standardThemeNames.Add(name);
            }
        }
    }



    public sealed class VisualStudio2013Palette : ThemePalette
    {
        // Fields
        public static readonly DependencyProperty AccentColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("AccentColor", 0xff007accL);
        public static readonly DependencyProperty AccentDarkColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("AccentDarkColor", 0xff007accL);
        public static readonly DependencyProperty AccentMainColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("AccentMainColor", 0xff3399ffL);
        public static readonly DependencyProperty AlternativeColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("AlternativeColor", 0xfff5f5f5L);
        public static readonly DependencyProperty BasicColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("BasicColor", 0xffcccedbL);
        public static readonly DependencyProperty ComplementaryColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("ComplementaryColor", 0xffdbdde6L);
        public static readonly DependencyProperty DefaultForegroundColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("DefaultForegroundColor", 0xff1e1e1eL);
        public static readonly DependencyProperty FontFamilyProperty = ThemePalette.RegisterFontFamily<VisualStudio2013Palette>("FontFamily", "Segoe UI");
        public static readonly DependencyProperty FontSizeLProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeL", 13.0);
        public static readonly DependencyProperty FontSizeProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSize", 12.0);
        public static readonly DependencyProperty FontSizeSProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeS", 11.0);
        public static readonly DependencyProperty FontSizeXLProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeXL", 20.0);
        public static readonly DependencyProperty FontSizeXSProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeXS", 10.0);
        public static readonly DependencyProperty FontSizeXXLProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeXXL", 22.0);
        public static readonly DependencyProperty FontSizeXXSProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeXXS", 9.0);
        public static readonly DependencyProperty FontSizeXXXSProperty = ThemePalette.RegisterDouble<VisualStudio2013Palette>("FontSizeXXXS", 8.0);
        private static VisualStudio2013Palette frozenPalette;
        public static readonly DependencyProperty HeaderColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("HeaderColor", 0xff007accL);
        public static readonly DependencyProperty IsFreezableProperty = DependencyProperty.RegisterAttached("IsFreezable", typeof(bool), typeof(VisualStudio2013Palette), new PropertyMetadata(false, new PropertyChangedCallback(VisualStudio2013Palette.OnIsFreezablePropertyChanged)));
        private static readonly DependencyProperty IsFrozenProperty = DependencyProperty.RegisterAttached("IsFrozen", typeof(bool), typeof(VisualStudio2013Palette), new PropertyMetadata(false, new PropertyChangedCallback(ThemePalette.OnIsFrozenChanged)));
        public static readonly DependencyProperty MainColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("MainColor", 0xffffffffL);
        public static readonly DependencyProperty MarkerColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("MarkerColor", 0xff1e1e1eL);
        public static readonly DependencyProperty MouseOverColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("MouseOverColor", 0xffc9def5L);
        [ThreadStatic]
        private static VisualStudio2013Palette palette;
        public static readonly DependencyProperty PrimaryColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("PrimaryColor", 0xffeeeef2L);
        [ThreadStatic]
        private static ResourceDictionary resourceDictionary;
        public static readonly DependencyProperty SelectedColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("SelectedColor", 0xffffffffL);
        public static readonly DependencyProperty SemiBasicColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("SemiBasicColor", 0x66cccedbL);
        public static readonly DependencyProperty StrongColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("StrongColor", 0xff717171L);
        public static readonly DependencyProperty ValidationColorProperty = ThemePalette.RegisterColor<VisualStudio2013Palette>("ValidationColor", 0xffff3333L);

        // Methods
        private VisualStudio2013Palette()
        {
        }

        internal override void FreezeOverride()
        {
            frozenPalette = this;
            base.SetValue(IsFrozenProperty, true);
        }

        public static bool GetIsFreezable(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsFreezableProperty);
        }

        private static void InitializeThemeDictionary()
        {
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.DefaultForegroundColor, Palette.DefaultForegroundColor);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AccentBrush, new SolidColorBrush(Palette.AccentColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AccentMainBrush, new SolidColorBrush(Palette.AccentMainColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AccentDarkBrush, new SolidColorBrush(Palette.AccentDarkColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.ValidationBrush, new SolidColorBrush(Palette.ValidationColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.BasicBrush, new SolidColorBrush(Palette.BasicColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.SemiBasicBrush, new SolidColorBrush(Palette.SemiBasicColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.PrimaryBrush, new SolidColorBrush(Palette.PrimaryColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.MarkerBrush, new SolidColorBrush(Palette.MarkerColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.StrongBrush, new SolidColorBrush(Palette.StrongColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AlternativeBrush, new SolidColorBrush(Palette.AlternativeColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.SelectedBrush, new SolidColorBrush(Palette.SelectedColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.MouseOverBrush, new SolidColorBrush(Palette.MouseOverColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.ComplementaryBrush, new SolidColorBrush(Palette.ComplementaryColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.MainBrush, new SolidColorBrush(Palette.MainColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.HeaderBrush, new SolidColorBrush(Palette.HeaderColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.QualityPoorBrush, new SolidColorBrush(Palette.StrongColor));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.QualitySatisfactoryBrush, new SolidColorBrush(ThemePalette.TransformAlpha(Palette.StrongColor, 0x99)));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.QualityGoodBrush, new SolidColorBrush(ThemePalette.TransformAlpha(Palette.StrongColor, 0x4c)));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.SemiSelectedBrush, new SolidColorBrush(ThemePalette.TransformAlpha(Palette.SelectedColor, 0x26)));
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXXXS, Palette.FontSizeXXXS);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXXS, Palette.FontSizeXXS);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXS, Palette.FontSizeXS);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeS, Palette.FontSizeS);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSize, Palette.FontSize);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeL, Palette.FontSizeL);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXL, Palette.FontSizeXL);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXXL, Palette.FontSizeXXL);
            ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontFamily, Palette.FontFamily);
        }

        public static void LoadPreset(ColorVariation preset)
        {
            switch (preset)
            {
                case ColorVariation.Dark:
                    Palette.AccentColor = ThemePalette.ColorFromLongValue(0xff007accL);
                    Palette.AccentMainColor = ThemePalette.ColorFromLongValue(0xff3399ffL);
                    Palette.AccentDarkColor = ThemePalette.ColorFromLongValue(0xff007accL);
                    Palette.ValidationColor = ThemePalette.ColorFromLongValue(0xffff3333L);
                    Palette.BasicColor = ThemePalette.ColorFromLongValue(0xff3f3f46L);
                    Palette.SemiBasicColor = ThemePalette.ColorFromLongValue(0x663f3f46L);
                    Palette.PrimaryColor = ThemePalette.ColorFromLongValue(0xff2d2d30L);
                    Palette.MarkerColor = ThemePalette.ColorFromLongValue(0xfff1f1f1L);
                    Palette.StrongColor = ThemePalette.ColorFromLongValue(0xff999999L);
                    Palette.AlternativeColor = ThemePalette.ColorFromLongValue(0xff252526L);
                    Palette.SelectedColor = ThemePalette.ColorFromLongValue(0xffffffffL);
                    Palette.MouseOverColor = ThemePalette.ColorFromLongValue(0xff3e3e40L);
                    Palette.ComplementaryColor = ThemePalette.ColorFromLongValue(0xff434346L);
                    Palette.MainColor = ThemePalette.ColorFromLongValue(0xff1e1e1eL);
                    Palette.HeaderColor = ThemePalette.ColorFromLongValue(0xff007accL);
                    Palette.DefaultForegroundColor = ThemePalette.ColorFromLongValue(0xfff1f1f1L);
                    return;

                case ColorVariation.Blue:
                    Palette.AccentColor = ThemePalette.ColorFromLongValue(0xffe5c365L);
                    Palette.AccentMainColor = ThemePalette.ColorFromLongValue(0xff3399ffL);
                    Palette.AccentDarkColor = ThemePalette.ColorFromLongValue(0xff007accL);
                    Palette.ValidationColor = ThemePalette.ColorFromLongValue(0xffff3333L);
                    Palette.BasicColor = ThemePalette.ColorFromLongValue(0xffcccedbL);
                    Palette.SemiBasicColor = ThemePalette.ColorFromLongValue(0x66cccedbL);
                    Palette.PrimaryColor = ThemePalette.ColorFromLongValue(0xffe6ebf5L);
                    Palette.MarkerColor = ThemePalette.ColorFromLongValue(0xff1e1e1eL);
                    Palette.StrongColor = ThemePalette.ColorFromLongValue(0xff717171L);
                    Palette.AlternativeColor = ThemePalette.ColorFromLongValue(0xfff6f6f6L);
                    Palette.SelectedColor = ThemePalette.ColorFromLongValue(0xffffffffL);
                    Palette.MouseOverColor = ThemePalette.ColorFromLongValue(0xfffdf4bfL);
                    Palette.ComplementaryColor = ThemePalette.ColorFromLongValue(0xffdbdde6L);
                    Palette.MainColor = ThemePalette.ColorFromLongValue(0xffffffffL);
                    Palette.HeaderColor = ThemePalette.ColorFromLongValue(0xff35496aL);
                    Palette.DefaultForegroundColor = ThemePalette.ColorFromLongValue(0xff1e1e1eL);
                    return;
            }
            Palette.AccentColor = ThemePalette.ColorFromLongValue(0xff007accL);
            Palette.AccentMainColor = ThemePalette.ColorFromLongValue(0xff3399ffL);
            Palette.AccentDarkColor = ThemePalette.ColorFromLongValue(0xff007accL);
            Palette.ValidationColor = ThemePalette.ColorFromLongValue(0xffff3333L);
            Palette.BasicColor = ThemePalette.ColorFromLongValue(0xffcccedbL);
            Palette.SemiBasicColor = ThemePalette.ColorFromLongValue(0x66cccedbL);
            Palette.PrimaryColor = ThemePalette.ColorFromLongValue(0xffeeeef2L);
            Palette.MarkerColor = ThemePalette.ColorFromLongValue(0xff1e1e1eL);
            Palette.StrongColor = ThemePalette.ColorFromLongValue(0xff717171L);
            Palette.AlternativeColor = ThemePalette.ColorFromLongValue(0xfff5f5f5L);
            Palette.SelectedColor = ThemePalette.ColorFromLongValue(0xffffffffL);
            Palette.MouseOverColor = ThemePalette.ColorFromLongValue(0xffc9def5L);
            Palette.ComplementaryColor = ThemePalette.ColorFromLongValue(0xffdbdde6L);
            Palette.MainColor = ThemePalette.ColorFromLongValue(0xffffffffL);
            Palette.HeaderColor = ThemePalette.ColorFromLongValue(0xff007accL);
            Palette.DefaultForegroundColor = ThemePalette.ColorFromLongValue(0xff1e1e1eL);
        }

        private static void OnIsFreezablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePalette.OnIsFreezableChanged(d, e, IsFrozenProperty, Palette);
        }

        private static void OnIsFrozenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThemePalette.OnIsFrozenChanged(d, e);
        }

        internal override void OnResourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DefaultForegroundColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.DefaultForegroundColor, Palette.DefaultForegroundColor);
            }
            else if (e.Property == AccentColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AccentBrush, new SolidColorBrush(Palette.AccentColor));
            }
            else if (e.Property == AccentMainColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AccentMainBrush, new SolidColorBrush(Palette.AccentMainColor));
            }
            else if (e.Property == AccentDarkColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AccentDarkBrush, new SolidColorBrush(Palette.AccentDarkColor));
            }
            else if (e.Property == ValidationColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.ValidationBrush, new SolidColorBrush(Palette.ValidationColor));
            }
            else if (e.Property == BasicColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.BasicBrush, new SolidColorBrush(Palette.BasicColor));
            }
            else if (e.Property == SemiBasicColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.SemiBasicBrush, new SolidColorBrush(Palette.SemiBasicColor));
            }
            else if (e.Property == PrimaryColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.PrimaryBrush, new SolidColorBrush(Palette.PrimaryColor));
            }
            else if (e.Property == StrongColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.StrongBrush, new SolidColorBrush(Palette.StrongColor));
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.QualityPoorBrush, new SolidColorBrush(Palette.StrongColor));
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.QualitySatisfactoryBrush, new SolidColorBrush(ThemePalette.TransformAlpha(Palette.StrongColor, 0x99)));
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.QualityGoodBrush, new SolidColorBrush(ThemePalette.TransformAlpha(Palette.StrongColor, 0x4c)));
            }
            else if (e.Property == AlternativeColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.AlternativeBrush, new SolidColorBrush(Palette.AlternativeColor));
            }
            else if (e.Property == SelectedColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.SelectedBrush, new SolidColorBrush(Palette.SelectedColor));
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.SemiSelectedBrush, new SolidColorBrush(ThemePalette.TransformAlpha(Palette.SelectedColor, 0x26)));
            }
            else if (e.Property == MouseOverColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.MouseOverBrush, new SolidColorBrush(Palette.MouseOverColor));
            }
            else if (e.Property == ComplementaryColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.ComplementaryBrush, new SolidColorBrush(Palette.ComplementaryColor));
            }
            else if (e.Property == MainColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.MainBrush, new SolidColorBrush(Palette.MainColor));
            }
            else if (e.Property == HeaderColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.HeaderBrush, new SolidColorBrush(Palette.HeaderColor));
            }
            else if (e.Property == MarkerColorProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.MarkerBrush, new SolidColorBrush(Palette.MarkerColor));
            }
            else if (e.Property == FontSizeXXXSProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXXXS, Palette.FontSizeXXXS);
            }
            else if (e.Property == FontSizeXXSProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXXS, Palette.FontSizeXXS);
            }
            else if (e.Property == FontSizeXSProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXS, Palette.FontSizeXS);
            }
            else if (e.Property == FontSizeSProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeS, Palette.FontSizeS);
            }
            else if (e.Property == FontSizeProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSize, Palette.FontSize);
            }
            else if (e.Property == FontSizeLProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeL, Palette.FontSizeL);
            }
            else if (e.Property == FontSizeXLProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXL, Palette.FontSizeXL);
            }
            else if (e.Property == FontSizeXXLProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontSizeXXL, Palette.FontSizeXXL);
            }
            else if (e.Property == FontFamilyProperty)
            {
                ThemePalette.FreezeAndSetResource(ResourceDictionary, VisualStudio2013ResourceKey.FontFamily, Palette.FontFamily);
            }
        }

        public static void SetIsFreezable(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFreezableProperty, value);
        }

        // Properties
        public Color AccentColor
        {
            get
            {
                return (Color)base.GetValue(AccentColorProperty);
            }
            set
            {
                base.SetValue(AccentColorProperty, value);
            }
        }

        public Color AccentDarkColor
        {
            get
            {
                return (Color)base.GetValue(AccentDarkColorProperty);
            }
            set
            {
                base.SetValue(AccentDarkColorProperty, value);
            }
        }

        public Color AccentMainColor
        {
            get
            {
                return (Color)base.GetValue(AccentMainColorProperty);
            }
            set
            {
                base.SetValue(AccentMainColorProperty, value);
            }
        }

        public Color AlternativeColor
        {
            get
            {
                return (Color)base.GetValue(AlternativeColorProperty);
            }
            set
            {
                base.SetValue(AlternativeColorProperty, value);
            }
        }

        public Color BasicColor
        {
            get
            {
                return (Color)base.GetValue(BasicColorProperty);
            }
            set
            {
                base.SetValue(BasicColorProperty, value);
            }
        }

        public Color ComplementaryColor
        {
            get
            {
                return (Color)base.GetValue(ComplementaryColorProperty);
            }
            set
            {
                base.SetValue(ComplementaryColorProperty, value);
            }
        }

        public Color DefaultForegroundColor
        {
            get
            {
                return (Color)base.GetValue(DefaultForegroundColorProperty);
            }
            set
            {
                base.SetValue(DefaultForegroundColorProperty, value);
            }
        }

        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)base.GetValue(FontFamilyProperty);
            }
            set
            {
                base.SetValue(FontFamilyProperty, value);
            }
        }

        public double FontSize
        {
            get
            {
                return (double)base.GetValue(FontSizeProperty);
            }
            set
            {
                base.SetValue(FontSizeProperty, value);
            }
        }

        public double FontSizeL
        {
            get
            {
                return (double)base.GetValue(FontSizeLProperty);
            }
            set
            {
                base.SetValue(FontSizeLProperty, value);
            }
        }

        public double FontSizeS
        {
            get
            {
                return (double)base.GetValue(FontSizeSProperty);
            }
            set
            {
                base.SetValue(FontSizeSProperty, value);
            }
        }

        public double FontSizeXL
        {
            get
            {
                return (double)base.GetValue(FontSizeXLProperty);
            }
            set
            {
                base.SetValue(FontSizeXLProperty, value);
            }
        }

        public double FontSizeXS
        {
            get
            {
                return (double)base.GetValue(FontSizeXSProperty);
            }
            set
            {
                base.SetValue(FontSizeXSProperty, value);
            }
        }

        public double FontSizeXXL
        {
            get
            {
                return (double)base.GetValue(FontSizeXXLProperty);
            }
            set
            {
                base.SetValue(FontSizeXXLProperty, value);
            }
        }

        public double FontSizeXXS
        {
            get
            {
                return (double)base.GetValue(FontSizeXXSProperty);
            }
            set
            {
                base.SetValue(FontSizeXXSProperty, value);
            }
        }

        public double FontSizeXXXS
        {
            get
            {
                return (double)base.GetValue(FontSizeXXXSProperty);
            }
            set
            {
                base.SetValue(FontSizeXXXSProperty, value);
            }
        }

        public Color HeaderColor
        {
            get
            {
                return (Color)base.GetValue(HeaderColorProperty);
            }
            set
            {
                base.SetValue(HeaderColorProperty, value);
            }
        }

        public Color MainColor
        {
            get
            {
                return (Color)base.GetValue(MainColorProperty);
            }
            set
            {
                base.SetValue(MainColorProperty, value);
            }
        }

        public Color MarkerColor
        {
            get
            {
                return (Color)base.GetValue(MarkerColorProperty);
            }
            set
            {
                base.SetValue(MarkerColorProperty, value);
            }
        }

        public Color MouseOverColor
        {
            get
            {
                return (Color)base.GetValue(MouseOverColorProperty);
            }
            set
            {
                base.SetValue(MouseOverColorProperty, value);
            }
        }

        public static VisualStudio2013Palette Palette
        {
            get
            {
                if (frozenPalette != null)
                {
                    return frozenPalette;
                }
                if (palette == null)
                {
                    palette = new VisualStudio2013Palette();
                }
                return palette;
            }
        }

        public Color PrimaryColor
        {
            get
            {
                return (Color)base.GetValue(PrimaryColorProperty);
            }
            set
            {
                base.SetValue(PrimaryColorProperty, value);
            }
        }

        internal static ResourceDictionary ResourceDictionary
        {
            get
            {
                if (resourceDictionary == null)
                {
                    resourceDictionary = new ResourceDictionary();
                    InitializeThemeDictionary();
                }
                return resourceDictionary;
            }
        }

        public Color SelectedColor
        {
            get
            {
                return (Color)base.GetValue(SelectedColorProperty);
            }
            set
            {
                base.SetValue(SelectedColorProperty, value);
            }
        }

        public Color SemiBasicColor
        {
            get
            {
                return (Color)base.GetValue(SemiBasicColorProperty);
            }
            set
            {
                base.SetValue(SemiBasicColorProperty, value);
            }
        }

        public Color StrongColor
        {
            get
            {
                return (Color)base.GetValue(StrongColorProperty);
            }
            set
            {
                base.SetValue(StrongColorProperty, value);
            }
        }

        public Color ValidationColor
        {
            get
            {
                return (Color)base.GetValue(ValidationColorProperty);
            }
            set
            {
                base.SetValue(ValidationColorProperty, value);
            }
        }

        // Nested Types
        public enum ColorVariation
        {
            Dark,
            Light,
            Blue
        }
    }



}
