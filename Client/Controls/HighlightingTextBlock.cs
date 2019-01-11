using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
namespace GazRouter.Controls
{
    /// <summary>
    ///     A specialized highlighting text block control.
    /// </summary>
    public class HighlightingTextBlock : Control
    {
        /// <summary>
        ///     Identifies the HighlightBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register(
                "HighlightBrush",
                typeof (Brush),
                typeof (HighlightingTextBlock),
                new PropertyMetadata(null, OnHighlightBrushPropertyChanged));

        /// <summary>
        ///     Identifies the HighlightFontWeight dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightFontWeightProperty =
            DependencyProperty.Register(
                "HighlightFontWeight",
                typeof (FontWeight),
                typeof (HighlightingTextBlock),
                new PropertyMetadata(FontWeights.Normal, OnHighlightFontWeightPropertyChanged));

        /// <summary>
        ///     Identifies the HighlightText dependency property.
        /// </summary>
        public static readonly DependencyProperty HighlightTextProperty =
            DependencyProperty.Register(
                "HighlightText",
                typeof (string),
                typeof (HighlightingTextBlock),
                new PropertyMetadata(OnHighlightTextPropertyChanged));

        /// <summary>
        ///     Gets or sets the contents of the TextBox.
        /// </summary>
        /// <summary>
        ///     Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof (string),
                typeof (HighlightingTextBlock),
                new PropertyMetadata(OnTextPropertyChanged));

        private const string TemplateXaml =
          @"<ControlTemplate   
                xmlns=""http://schemas.microsoft.com/client/2007""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    <TextBlock x:Name=""Text"" />
                </ControlTemplate>";

        /// <summary>
        ///     The name of the TextBlock part.
        /// </summary>
        private const string TextBlockName = "Text";
        private TextBlock _textBlock;

        /// <summary>
        ///     Initializes a new HighlightingTextBlock class.
        /// </summary>
        public HighlightingTextBlock()
        {
            // DefaultStyleKey = typeof(HighlightingTextBlock);
            //    Loaded += OnLoaded;

            Template = (ControlTemplate) XamlReader.Load(TemplateXaml);
        }

        /// <summary>
        ///     Gets or sets the highlight brush.
        /// </summary>
        public Brush HighlightBrush
        {
            get { return GetValue(HighlightBrushProperty) as Brush; }
            set { SetValue(HighlightBrushProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the font weight used on highlighted text.
        /// </summary>
        public FontWeight HighlightFontWeight
        {
            get { return (FontWeight) GetValue(HighlightFontWeightProperty); }
            set { SetValue(HighlightFontWeightProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the highlighted text.
        /// </summary>
        public string HighlightText
        {
            get { return GetValue(HighlightTextProperty) as string; }
            set { SetValue(HighlightTextProperty, value); }
        }

        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        ///     Override the apply template handler.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Grab the template part
            _textBlock = GetTemplateChild(TextBlockName) as TextBlock;

            FillTextBlock();
        }

        /// <summary>
        ///     HighlightText property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its HighlightText.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnHighlightTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HighlightingTextBlock)d).FillTextBlock();
        }

        /// <summary>
        ///     HighlightBrushProperty property changed handler.
        /// </summary>
        /// <param name="d">HighlightingTextBlock that changed its HighlightBrush.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnHighlightBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HighlightingTextBlock)d).FillTextBlock();
        }

        /// <summary>
        ///     HighlightFontWeightProperty property changed handler.
        /// </summary>
        /// <param name="d">HighlightingTextBlock that changed its HighlightFontWeight.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnHighlightFontWeightPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        ///     TextProperty property changed handler.
        /// </summary>
        /// <param name="d">AutoCompleteBox that changed its Text.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HighlightingTextBlock) d).FillTextBlock();
        }

        private void FillTextBlockSingle()
        {
            if (_textBlock == null)
            {
                return;
            }

            var newValue = Text;
            _textBlock.Inlines.Clear();

            if (newValue == null)
            {
                return;
            }

            var highlight = HighlightText ?? string.Empty;

            var cur = 0;
            while (cur < newValue.Length)
            {
                var indexOf = highlight.Length == 0
                    ? -1
                    : newValue.IndexOf(highlight, cur, StringComparison.OrdinalIgnoreCase);

                if (indexOf != -1)
                {
                    Inline run = new Run {Text = newValue.Substring(cur, indexOf - cur)};
                    _textBlock.Inlines.Add(run);

                    run = new Run
                    {
                        Text = newValue.Substring(indexOf, highlight.Length),
                        Foreground = HighlightBrush,
                        FontWeight = HighlightFontWeight
                    };
                    _textBlock.Inlines.Add(run);
                    cur = indexOf + highlight.Length;
                }
                else
                {
                    Inline run = new Run {Text = newValue.Substring(cur, newValue.Length - cur)};
                    _textBlock.Inlines.Add(run);
                    cur = newValue.Length;
                }
            }
        }
        /// <summary> apply the visual highlighting. </summary>
        private void FillTextBlock()
        {
            if (_textBlock?.Inlines == null) return;
            if (Text == null) return;            
            _textBlock.Inlines.Clear();
            var fill = FillTextBlock(HighlightText, Text);
            foreach (var xy in fill)
            {
                if (xy.IsHigh)
                {
                    _textBlock.Inlines.Add(new Run
                    {
                        Text = xy.Fragment,
                        Foreground = HighlightBrush,
                        FontWeight = HighlightFontWeight
                    });
                    continue;
                }
                _textBlock.Inlines.Add(new Run { Text = xy.Fragment });
            }
        }
        private static IEnumerable<TextFragment> FillTextBlock(string highlight, string text)
        {
            if (string.IsNullOrEmpty(text)) return new[] { new TextFragment { Fragment = "", IsHigh = false } };
            if (string.IsNullOrEmpty(highlight)) return new[] { new TextFragment { Fragment = text, IsHigh = false } };
            //
            var splits = highlight.Trim().Split(' ');
            var list = new List<TextFragment>();
            Func<string, string[], TextFragment> func = (fullText, arrSplits) =>
            {
                var xy = new TextFragment { Index = 0, Fragment = fullText, IsHigh = false };
                foreach (var split in arrSplits)
                {
                    var f = fullText.IndexOf(split, 0, StringComparison.OrdinalIgnoreCase);
                    if (f < 0 || (f > xy.Index & xy.IsHigh)) continue;
                    xy = new TextFragment { Index = f, Fragment = fullText.Substring(f, split.Length), IsHigh = true };
                }
                return xy;
            };
            while (text.Length > 0)
            {
                var xy = func(text, splits);
                if (!xy.IsHigh)
                {
                    list.Add(new TextFragment { Fragment = text, IsHigh = false });
                    break;
                }
                if (xy.Index > 0) list.Add(new TextFragment { Fragment = text.Substring(0, xy.Index), IsHigh = false });
                list.Add(xy);
                text = text.Remove(0, xy.Index + xy.Length);
            }
            return list;
        }
    }
    internal class TextFragment
    {
        public int Index { get; set; }
        public string Fragment { get; set; }
        public bool IsHigh { get; set; }
        public int Length => string.IsNullOrEmpty(Fragment) ? 0 : Fragment.Length;
    }
}
/*
private void FillTextBlock2()
{
    if (_textBlock == null) return;
    //
    var newValue = Text;
    _textBlock.Inlines.Clear();
    if (newValue == null) return;
    //
    var highlight = HighlightText ?? string.Empty;
    var cur = 0;
    while (cur < newValue.Length)
    {
        var indexOf = highlight.Length == 0
            ? -1
            : newValue.IndexOf(highlight, cur, StringComparison.OrdinalIgnoreCase);

        if (indexOf != -1)
        {
            Inline run = new Run { Text = newValue.Substring(cur, indexOf - cur) };
            _textBlock.Inlines.Add(run);

            run = new Run
            {
                Text = newValue.Substring(indexOf, highlight.Length),
                Foreground = HighlightBrush,
                FontWeight = HighlightFontWeight
            };
            _textBlock.Inlines.Add(run);
            cur = indexOf + highlight.Length;
        }
        else
        {
            Inline run = new Run { Text = newValue.Substring(cur, newValue.Length - cur) };
            _textBlock.Inlines.Add(run);
            cur = newValue.Length;
        }
    }
}
*/

/*
StringComparison compare = StringComparison.OrdinalIgnoreCase;

int cur = 0;
while (cur < text.Length)
{
    int i = highlight.Length == 0 ? -1 : text.IndexOf(highlight, cur, compare);
    i = i < 0 ? text.Length : i;

    // Clear
    while (cur < i && cur < text.Length)
    {
        _textBlock.Inlines[cur].Foreground = Foreground;
        _textBlock.Inlines[cur].FontWeight = FontWeight;
        cur++;
    }

    // Highlight
    int start = cur;
    while (cur < start + highlight.Length && cur < text.Length)
    {
        _textBlock.Inlines[cur].Foreground = HighlightBrush;
        _textBlock.Inlines[cur].FontWeight = HighlightFontWeight;
        cur++;
    }
}
*/
