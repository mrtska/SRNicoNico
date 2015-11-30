using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Windows.Documents;

namespace SRNicoNico.Views.Extentions {
    // This code was taken from MSDN as an example of converting HTML to XAML.
    //
    // I've combined all the classes together and made some spelling corrections.
    //
    // Copyright (C) Microsoft Corporation.  All rights reserved.
    //
    // Description: Prototype for Xaml - Html conversion

    /// <summary>
    /// types of lexical tokens for html-to-xaml converter
    /// </summary>
    internal enum HtmlTokenType {
        OpeningTagStart,
        ClosingTagStart,
        TagEnd,
        EmptyTagEnd,
        EqualSign,
        Name,
        Atom, // any attribute value not in quotes
        Text, //text content when accepting text
        Comment,
        EOF,
    }

    internal static class HtmlCssParser {
        // .................................................................
        //
        // Processing CSS Attributes
        //
        // .................................................................

        internal static void GetElementPropertiesFromCssAttributes(XmlElement htmlElement, string elementName, CssStylesheet stylesheet, Hashtable localProperties, List<XmlElement> sourceContext) {
            string styleFromStylesheet = stylesheet.GetStyle(elementName, sourceContext);

            string styleInline = HtmlToXamlConverter.GetAttribute(htmlElement, "style");

            // Combine styles from stylesheet and from inline attribute.
            // The order is important - the latter styles will override the former.
            string style = styleFromStylesheet != null ? styleFromStylesheet : null;
            if(styleInline != null) {
                style = style == null ? styleInline : (style + ";" + styleInline);
            }

            // Apply local style to current formatting properties
            if(style != null) {
                string[] styleValues = style.Split(';');
                for(int i = 0; i < styleValues.Length; i++) {
                    string[] styleNameValue;

                    styleNameValue = styleValues[i].Split(':');
                    if(styleNameValue.Length == 2) {
                        string styleName = styleNameValue[0].Trim().ToLower();
                        string styleValue = HtmlToXamlConverter.UnQuote(styleNameValue[1].Trim()).ToLower();
                        int nextIndex = 0;

                        switch(styleName) {
                            case "font":
                                ParseCssFont(styleValue, localProperties);
                                break;
                            case "font-family":
                                ParseCssFontFamily(styleValue, ref nextIndex, localProperties);
                                break;
                            case "font-size":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, "font-size", /*mustBeNonNegative:*/true);
                                break;
                            case "font-style":
                                ParseCssFontStyle(styleValue, ref nextIndex, localProperties);
                                break;
                            case "font-weight":
                                ParseCssFontWeight(styleValue, ref nextIndex, localProperties);
                                break;
                            case "font-variant":
                                ParseCssFontVariant(styleValue, ref nextIndex, localProperties);
                                break;
                            case "line-height":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, "line-height", /*mustBeNonNegative:*/true);
                                break;
                            case "word-spacing":
                                //  Implement word-spacing conversion
                                break;
                            case "letter-spacing":
                                //  Implement letter-spacing conversion
                                break;
                            case "color":
                                ParseCssColor(styleValue, ref nextIndex, localProperties, "color");
                                break;

                            case "text-decoration":
                                ParseCssTextDecoration(styleValue, ref nextIndex, localProperties);
                                break;

                            case "text-transform":
                                ParseCssTextTransform(styleValue, ref nextIndex, localProperties);
                                break;

                            case "background-color":
                                ParseCssColor(styleValue, ref nextIndex, localProperties, "background-color");
                                break;
                            case "background":
                                // TODO: need to parse composite background property
                                ParseCssBackground(styleValue, ref nextIndex, localProperties);
                                break;

                            case "text-align":
                                ParseCssTextAlign(styleValue, ref nextIndex, localProperties);
                                break;
                            case "vertical-align":
                                ParseCssVerticalAlign(styleValue, ref nextIndex, localProperties);
                                break;
                            case "text-indent":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, "text-indent", /*mustBeNonNegative:*/false);
                                break;

                            case "width":
                            case "height":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, styleName, /*mustBeNonNegative:*/true);
                                break;

                            case "margin": // top/right/bottom/left
                                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, styleName);
                                break;
                            case "margin-top":
                            case "margin-right":
                            case "margin-bottom":
                            case "margin-left":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, styleName, /*mustBeNonNegative:*/true);
                                break;

                            case "padding":
                                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, styleName);
                                break;
                            case "padding-top":
                            case "padding-right":
                            case "padding-bottom":
                            case "padding-left":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, styleName, /*mustBeNonNegative:*/true);
                                break;

                            case "border":
                                ParseCssBorder(styleValue, ref nextIndex, localProperties);
                                break;
                            case "border-style":
                            case "border-width":
                            case "border-color":
                                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, styleName);
                                break;
                            case "border-top":
                            case "border-right":
                            case "border-left":
                            case "border-bottom":
                                //  Parse css border style
                                break;

                            // NOTE: css names for elementary border styles have side indications in the middle (top/bottom/left/right)
                            // In our internal notation we intentionally put them at the end - to unify processing in ParseCssRectangleProperty method
                            case "border-top-style":
                            case "border-right-style":
                            case "border-left-style":
                            case "border-bottom-style":
                            case "border-top-color":
                            case "border-right-color":
                            case "border-left-color":
                            case "border-bottom-color":
                            case "border-top-width":
                            case "border-right-width":
                            case "border-left-width":
                            case "border-bottom-width":
                                //  Parse css border style
                                break;

                            case "display":
                                //  Implement display style conversion
                                break;

                            case "float":
                                ParseCssFloat(styleValue, ref nextIndex, localProperties);
                                break;
                            case "clear":
                                ParseCssClear(styleValue, ref nextIndex, localProperties);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        // .................................................................
        //
        // Parsing CSS - Lexical Helpers
        //
        // .................................................................

        // Skips whitespaces in style values
        private static void ParseWhiteSpace(string styleValue, ref int nextIndex) {
            while(nextIndex < styleValue.Length && Char.IsWhiteSpace(styleValue[nextIndex])) {
                nextIndex++;
            }
        }

        // Checks if the following character matches to a given word and advances nextIndex
        // by the word's length in case of success.
        // Otherwise leaves nextIndex in place (except for possible whitespaces).
        // Returns true or false depending on success or failure of matching.
        private static bool ParseWord(string word, string styleValue, ref int nextIndex) {
            ParseWhiteSpace(styleValue, ref nextIndex);

            for(int i = 0; i < word.Length; i++) {
                if(!(nextIndex + i < styleValue.Length && word[i] == styleValue[nextIndex + i])) {
                    return false;
                }
            }

            if(nextIndex + word.Length < styleValue.Length && Char.IsLetterOrDigit(styleValue[nextIndex + word.Length])) {
                return false;
            }

            nextIndex += word.Length;
            return true;
        }

        // CHecks whether the following character sequence matches to one of the given words,
        // and advances the nextIndex to matched word length.
        // Returns null in case if there is no match or the word matched.
        private static string ParseWordEnumeration(string[] words, string styleValue, ref int nextIndex) {
            for(int i = 0; i < words.Length; i++) {
                if(ParseWord(words[i], styleValue, ref nextIndex)) {
                    return words[i];
                }
            }

            return null;
        }

        private static void ParseWordEnumeration(string[] words, string styleValue, ref int nextIndex, Hashtable localProperties, string attributeName) {
            string attributeValue = ParseWordEnumeration(words, styleValue, ref nextIndex);
            if(attributeValue != null) {
                localProperties[attributeName] = attributeValue;
            }
        }

        private static string ParseCssSize(string styleValue, ref int nextIndex, bool mustBeNonNegative) {
            ParseWhiteSpace(styleValue, ref nextIndex);

            int startIndex = nextIndex;

            // Parse optional munis sign
            if(nextIndex < styleValue.Length && styleValue[nextIndex] == '-') {
                nextIndex++;
            }

            if(nextIndex < styleValue.Length && Char.IsDigit(styleValue[nextIndex])) {
                while(nextIndex < styleValue.Length && (Char.IsDigit(styleValue[nextIndex]) || styleValue[nextIndex] == '.')) {
                    nextIndex++;
                }

                string number = styleValue.Substring(startIndex, nextIndex - startIndex);

                string unit = ParseWordEnumeration(_fontSizeUnits, styleValue, ref nextIndex);
                if(unit == null) {
                    unit = "px"; // Assuming pixels by default
                }

                if(mustBeNonNegative && styleValue[startIndex] == '-') {
                    return "0";
                } else {
                    return number + unit;
                }
            }

            return null;
        }

        private static void ParseCssSize(string styleValue, ref int nextIndex, Hashtable localValues, string propertyName, bool mustBeNonNegative) {
            string length = ParseCssSize(styleValue, ref nextIndex, mustBeNonNegative);
            if(length != null) {
                localValues[propertyName] = length;
            }
        }

        private static readonly string[] _colors = new string[]
            {
                "aliceblue", "antiquewhite", "aqua", "aquamarine", "azure", "beige", "bisque", "black", "blanchedalmond",
                "blue", "blueviolet", "brown", "burlywood", "cadetblue", "chartreuse", "chocolate", "coral",
                "cornflowerblue", "cornsilk", "crimson", "cyan", "darkblue", "darkcyan", "darkgoldenrod", "darkgray",
                "darkgreen", "darkkhaki", "darkmagenta", "darkolivegreen", "darkorange", "darkorchid", "darkred",
                "darksalmon", "darkseagreen", "darkslateblue", "darkslategray", "darkturquoise", "darkviolet", "deeppink",
                "deepskyblue", "dimgray", "dodgerblue", "firebrick", "floralwhite", "forestgreen", "fuchsia", "gainsboro",
                "ghostwhite", "gold", "goldenrod", "gray", "green", "greenyellow", "honeydew", "hotpink", "indianred",
                "indigo", "ivory", "khaki", "lavender", "lavenderblush", "lawngreen", "lemonchiffon", "lightblue", "lightcoral",
                "lightcyan", "lightgoldenrodyellow", "lightgreen", "lightgrey", "lightpink", "lightsalmon", "lightseagreen",
                "lightskyblue", "lightslategray", "lightsteelblue", "lightyellow", "lime", "limegreen", "linen", "magenta",
                "maroon", "mediumaquamarine", "mediumblue", "mediumorchid", "mediumpurple", "mediumseagreen", "mediumslateblue",
                "mediumspringgreen", "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream", "mistyrose", "moccasin",
                "navajowhite", "navy", "oldlace", "olive", "olivedrab", "orange", "orangered", "orchid", "palegoldenrod",
                "palegreen", "paleturquoise", "palevioletred", "papayawhip", "peachpuff", "peru", "pink", "plum", "powderblue",
                "purple", "red", "rosybrown", "royalblue", "saddlebrown", "salmon", "sandybrown", "seagreen", "seashell",
                "sienna", "silver", "skyblue", "slateblue", "slategray", "snow", "springgreen", "steelblue", "tan", "teal",
                "thistle", "tomato", "turquoise", "violet", "wheat", "white", "whitesmoke", "yellow", "yellowgreen",
            };

        private static readonly string[] _systemColors = new string[]
            {
                "activeborder", "activecaption", "appworkspace", "background", "buttonface", "buttonhighlight", "buttonshadow",
                "buttontext", "captiontext", "graytext", "highlight", "highlighttext", "inactiveborder", "inactivecaption",
                "inactivecaptiontext", "infobackground", "infotext", "menu", "menutext", "scrollbar", "threeddarkshadow",
                "threedface", "threedhighlight", "threedlightshadow", "threedshadow", "window", "windowframe", "windowtext",
            };

        private static string ParseCssColor(string styleValue, ref int nextIndex) {
            //  Implement color parsing
            // rgb(100%,53.5%,10%)
            // rgb(255,91,26)
            // #FF5B1A
            // black | silver | gray | ... | aqua
            // transparent - for background-color
            ParseWhiteSpace(styleValue, ref nextIndex);

            string color = null;

            if(nextIndex < styleValue.Length) {
                int startIndex = nextIndex;
                char character = styleValue[nextIndex];

                if(character == '#') {
                    nextIndex++;
                    while(nextIndex < styleValue.Length) {
                        character = Char.ToUpper(styleValue[nextIndex]);
                        if(!('0' <= character && character <= '9' || 'A' <= character && character <= 'F')) {
                            break;
                        }
                        nextIndex++;
                    }
                    if(nextIndex > startIndex + 1) {
                        color = styleValue.Substring(startIndex, nextIndex - startIndex);
                    }
                } else if(styleValue.Substring(nextIndex, 3).ToLower() == "rbg") {
                    //  Implement real rgb() color parsing
                    while(nextIndex < styleValue.Length && styleValue[nextIndex] != ')') {
                        nextIndex++;
                    }
                    if(nextIndex < styleValue.Length) {
                        nextIndex++; // to skip ')'
                    }
                    color = "gray"; // return bogus color
                } else if(Char.IsLetter(character)) {
                    color = ParseWordEnumeration(_colors, styleValue, ref nextIndex);
                    if(color == null) {
                        color = ParseWordEnumeration(_systemColors, styleValue, ref nextIndex);
                        if(color != null) {
                            //  Implement smarter system color converions into real colors
                            color = "black";
                        }
                    }
                }
            }

            return color;
        }

        private static void ParseCssColor(string styleValue, ref int nextIndex, Hashtable localValues, string propertyName) {
            string color = ParseCssColor(styleValue, ref nextIndex);
            if(color != null) {
                localValues[propertyName] = color;
            }
        }

        // .................................................................
        //
        // Pasring CSS font Property
        //
        // .................................................................

        // CSS has five font properties: font-family, font-style, font-variant, font-weight, font-size.
        // An aggregated "font" property lets you specify in one action all the five in combination
        // with additional line-height property.
        //
        // font-family: [<family-name>,]* [<family-name> | <generic-family>]
        //    generic-family: serif | sans-serif | monospace | cursive | fantasy
        //       The list of families sets priorities to choose fonts;
        //       Quotes not allowed around generic-family names
        // font-style: normal | italic | oblique
        // font-variant: normal | small-caps
        // font-weight: normal | bold | bolder | lighter | 100 ... 900 |
        //    Default is "normal", normal==400
        // font-size: <absolute-size> | <relative-size> | <length> | <percentage>
        //    absolute-size: xx-small | x-small | small | medium | large | x-large | xx-large
        //    relative-size: larger | smaller
        //    length: <point> | <pica> | <ex> | <em> | <points> | <millimeters> | <centimeters> | <inches>
        //    Default: medium
        // font: [ <font-style> || <font-variant> || <font-weight ]? <font-size> [ / <line-height> ]? <font-family>

        private static readonly string[] _fontGenericFamilies = new string[] { "serif", "sans-serif", "monospace", "cursive", "fantasy" };
        private static readonly string[] _fontStyles = new string[] { "normal", "italic", "oblique" };
        private static readonly string[] _fontVariants = new string[] { "normal", "small-caps" };
        private static readonly string[] _fontWeights = new string[] { "normal", "bold", "bolder", "lighter", "100", "200", "300", "400", "500", "600", "700", "800", "900" };
        private static readonly string[] _fontAbsoluteSizes = new string[] { "xx-small", "x-small", "small", "medium", "large", "x-large", "xx-large" };
        private static readonly string[] _fontRelativeSizes = new string[] { "larger", "smaller" };
        private static readonly string[] _fontSizeUnits = new string[] { "px", "mm", "cm", "in", "pt", "pc", "em", "ex", "%" };

        // Parses CSS string fontStyle representing a value for css font attribute
        private static void ParseCssFont(string styleValue, Hashtable localProperties) {
            int nextIndex = 0;

            ParseCssFontStyle(styleValue, ref nextIndex, localProperties);
            ParseCssFontVariant(styleValue, ref nextIndex, localProperties);
            ParseCssFontWeight(styleValue, ref nextIndex, localProperties);

            ParseCssSize(styleValue, ref nextIndex, localProperties, "font-size", /*mustBeNonNegative:*/true);

            ParseWhiteSpace(styleValue, ref nextIndex);
            if(nextIndex < styleValue.Length && styleValue[nextIndex] == '/') {
                nextIndex++;
                ParseCssSize(styleValue, ref nextIndex, localProperties, "line-height", /*mustBeNonNegative:*/true);
            }

            ParseCssFontFamily(styleValue, ref nextIndex, localProperties);
        }

        private static void ParseCssFontStyle(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_fontStyles, styleValue, ref nextIndex, localProperties, "font-style");
        }

        private static void ParseCssFontVariant(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_fontVariants, styleValue, ref nextIndex, localProperties, "font-variant");
        }

        private static void ParseCssFontWeight(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_fontWeights, styleValue, ref nextIndex, localProperties, "font-weight");
        }

        private static void ParseCssFontFamily(string styleValue, ref int nextIndex, Hashtable localProperties) {
            string fontFamilyList = null;

            while(nextIndex < styleValue.Length) {
                // Try generic-family
                string fontFamily = ParseWordEnumeration(_fontGenericFamilies, styleValue, ref nextIndex);

                if(fontFamily == null) {
                    // Try quoted font family name
                    if(nextIndex < styleValue.Length && (styleValue[nextIndex] == '"' || styleValue[nextIndex] == '\'')) {
                        char quote = styleValue[nextIndex];

                        nextIndex++;

                        int startIndex = nextIndex;

                        while(nextIndex < styleValue.Length && styleValue[nextIndex] != quote) {
                            nextIndex++;
                        }

                        fontFamily = '"' + styleValue.Substring(startIndex, nextIndex - startIndex) + '"';
                    }

                    if(fontFamily == null) {
                        // Try unquoted font family name
                        int startIndex = nextIndex;
                        while(nextIndex < styleValue.Length && styleValue[nextIndex] != ',' && styleValue[nextIndex] != ';') {
                            nextIndex++;
                        }

                        if(nextIndex > startIndex) {
                            fontFamily = styleValue.Substring(startIndex, nextIndex - startIndex).Trim();
                            if(fontFamily.Length == 0) {
                                fontFamily = null;
                            }
                        }
                    }
                }

                ParseWhiteSpace(styleValue, ref nextIndex);
                if(nextIndex < styleValue.Length && styleValue[nextIndex] == ',') {
                    nextIndex++;
                }

                if(fontFamily != null) {
                    //  css font-family can contein a list of names. We only consider the first name from the list. Need a decision what to do with remaining names
                    // fontFamilyList = (fontFamilyList == null) ? fontFamily : fontFamilyList + "," + fontFamily;
                    if(fontFamilyList == null && fontFamily.Length > 0) {
                        if(fontFamily[0] == '"' || fontFamily[0] == '\'') {
                            // Unquote the font family name
                            fontFamily = fontFamily.Substring(1, fontFamily.Length - 2);
                        } else {
                            // Convert generic css family name
                        }
                        fontFamilyList = fontFamily;
                    }
                } else {
                    break;
                }
            }

            if(fontFamilyList != null) {
                localProperties["font-family"] = fontFamilyList;
            }
        }

        // .................................................................
        //
        // Pasring CSS list-style Property
        //
        // .................................................................

        // list-style: [ <list-style-type> || <list-style-position> || <list-style-image> ]

        private static readonly string[] _listStyleTypes = new string[] { "disc", "circle", "square", "decimal", "lower-roman", "upper-roman", "lower-alpha", "upper-alpha", "none" };
        private static readonly string[] _listStylePositions = new string[] { "inside", "outside" };

        private static void ParseCssListStyle(string styleValue, Hashtable localProperties) {
            int nextIndex = 0;

            while(nextIndex < styleValue.Length) {
                string listStyleType = ParseCssListStyleType(styleValue, ref nextIndex);
                if(listStyleType != null) {
                    localProperties["list-style-type"] = listStyleType;
                } else {
                    string listStylePosition = ParseCssListStylePosition(styleValue, ref nextIndex);
                    if(listStylePosition != null) {
                        localProperties["list-style-position"] = listStylePosition;
                    } else {
                        string listStyleImage = ParseCssListStyleImage(styleValue, ref nextIndex);
                        if(listStyleImage != null) {
                            localProperties["list-style-image"] = listStyleImage;
                        } else {
                            // TODO: Process unrecognized list style value
                            break;
                        }
                    }
                }
            }
        }

        private static string ParseCssListStyleType(string styleValue, ref int nextIndex) {
            return ParseWordEnumeration(_listStyleTypes, styleValue, ref nextIndex);
        }

        private static string ParseCssListStylePosition(string styleValue, ref int nextIndex) {
            return ParseWordEnumeration(_listStylePositions, styleValue, ref nextIndex);
        }

        private static string ParseCssListStyleImage(string styleValue, ref int nextIndex) {
            // TODO: Implement URL parsing for images
            return null;
        }

        // .................................................................
        //
        // Pasring CSS text-decorations Property
        //
        // .................................................................

        private static readonly string[] _textDecorations = new string[] { "none", "underline", "overline", "line-through", "blink" };

        private static void ParseCssTextDecoration(string styleValue, ref int nextIndex, Hashtable localProperties) {
            // Set default text-decorations:none;
            for(int i = 1; i < _textDecorations.Length; i++) {
                localProperties["text-decoration-" + _textDecorations[i]] = "false";
            }

            // Parse list of decorations values
            while(nextIndex < styleValue.Length) {
                string decoration = ParseWordEnumeration(_textDecorations, styleValue, ref nextIndex);
                if(decoration == null || decoration == "none") {
                    break;
                }
                localProperties["text-decoration-" + decoration] = "true";
            }
        }

        // .................................................................
        //
        // Pasring CSS text-transform Property
        //
        // .................................................................

        private static readonly string[] _textTransforms = new string[] { "none", "capitalize", "uppercase", "lowercase" };

        private static void ParseCssTextTransform(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_textTransforms, styleValue, ref nextIndex, localProperties, "text-transform");
        }

        // .................................................................
        //
        // Pasring CSS text-align Property
        //
        // .................................................................

        private static readonly string[] _textAligns = new string[] { "left", "right", "center", "justify" };

        private static void ParseCssTextAlign(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_textAligns, styleValue, ref nextIndex, localProperties, "text-align");
        }

        // .................................................................
        //
        // Pasring CSS vertical-align Property
        //
        // .................................................................

        private static readonly string[] _verticalAligns = new string[] { "baseline", "sub", "super", "top", "text-top", "middle", "bottom", "text-bottom" };

        private static void ParseCssVerticalAlign(string styleValue, ref int nextIndex, Hashtable localProperties) {
            //  Parse percentage value for vertical-align style
            ParseWordEnumeration(_verticalAligns, styleValue, ref nextIndex, localProperties, "vertical-align");
        }

        // .................................................................
        //
        // Pasring CSS float Property
        //
        // .................................................................

        private static readonly string[] _floats = new string[] { "left", "right", "none" };

        private static void ParseCssFloat(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_floats, styleValue, ref nextIndex, localProperties, "float");
        }

        // .................................................................
        //
        // Pasring CSS clear Property
        //
        // .................................................................

        private static readonly string[] _clears = new string[] { "none", "left", "right", "both" };

        private static void ParseCssClear(string styleValue, ref int nextIndex, Hashtable localProperties) {
            ParseWordEnumeration(_clears, styleValue, ref nextIndex, localProperties, "clear");
        }

        // .................................................................
        //
        // Pasring CSS margin and padding Properties
        //
        // .................................................................

        // Generic method for parsing any of four-values properties, such as margin, padding, border-width, border-style, border-color
        private static bool ParseCssRectangleProperty(string styleValue, ref int nextIndex, Hashtable localProperties, string propertyName) {
            // CSS Spec:
            // If only one value is set, then the value applies to all four sides;
            // If two or three values are set, then missinng value(s) are taken fromm the opposite side(s).
            // The order they are applied is: top/right/bottom/left

            Debug.Assert(propertyName == "margin" || propertyName == "padding" || propertyName == "border-width" || propertyName == "border-style" || propertyName == "border-color");

            string value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
            if(value != null) {
                localProperties[propertyName + "-top"] = value;
                localProperties[propertyName + "-bottom"] = value;
                localProperties[propertyName + "-right"] = value;
                localProperties[propertyName + "-left"] = value;
                value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
                if(value != null) {
                    localProperties[propertyName + "-right"] = value;
                    localProperties[propertyName + "-left"] = value;
                    value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
                    if(value != null) {
                        localProperties[propertyName + "-bottom"] = value;
                        value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
                        if(value != null) {
                            localProperties[propertyName + "-left"] = value;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        // .................................................................
        //
        // Pasring CSS border Properties
        //
        // .................................................................

        // border: [ <border-width> || <border-style> || <border-color> ]

        private static void ParseCssBorder(string styleValue, ref int nextIndex, Hashtable localProperties) {
            while(
                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-width") ||
                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-style") ||
                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-color")) {
            }
        }

        // .................................................................
        //
        // Pasring CSS border-style Propertie
        //
        // .................................................................

        private static readonly string[] _borderStyles = new string[] { "none", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset" };

        private static string ParseCssBorderStyle(string styleValue, ref int nextIndex) {
            return ParseWordEnumeration(_borderStyles, styleValue, ref nextIndex);
        }


        // .................................................................
        //
        //  What are these definitions doing here:
        //
        // .................................................................

        private static string[] _blocks = new string[] { "block", "inline", "list-item", "none" };

        // .................................................................
        //
        // Pasring CSS Background Properties
        //
        // .................................................................

        private static void ParseCssBackground(string styleValue, ref int nextIndex, Hashtable localValues) {
            //  Implement parsing background attribute
        }
    }
    internal class CssStylesheet {
        // Constructor
        public CssStylesheet(XmlElement htmlElement) {
            if(htmlElement != null) {
                this.DiscoverStyleDefinitions(htmlElement);
            }
        }

        // Recursively traverses an html tree, discovers STYLE elements and creates a style definition table
        // for further cascading style application
        public void DiscoverStyleDefinitions(XmlElement htmlElement) {
            if(htmlElement.LocalName.ToLower() == "link") {
                return;
                //  Add LINK elements processing for included stylesheets
                // <LINK href="http://sc.msn.com/global/css/ptnr/orange.css" type=text/css \r\nrel=stylesheet>
            }

            if(htmlElement.LocalName.ToLower() != "style") {
                // This is not a STYLE element. Recurse into it
                for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                    if(htmlChildNode is XmlElement) {
                        this.DiscoverStyleDefinitions((XmlElement)htmlChildNode);
                    }
                }
                return;
            }

            // Add style definitions from this style.

            // Collect all text from this style definition
            StringBuilder stylesheetBuffer = new StringBuilder();

            for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                if(htmlChildNode is XmlText || htmlChildNode is XmlComment) {
                    stylesheetBuffer.Append(RemoveComments(htmlChildNode.Value));
                }
            }

            // CssStylesheet has the following syntactical structure:
            //     @import declaration;
            //     selector { definition }
            // where "selector" is one of: ".classname", "tagname"
            // It can contain comments in the following form: /*...*/

            int nextCharacterIndex = 0;
            while(nextCharacterIndex < stylesheetBuffer.Length) {
                // Extract selector
                int selectorStart = nextCharacterIndex;
                while(nextCharacterIndex < stylesheetBuffer.Length && stylesheetBuffer[nextCharacterIndex] != '{') {
                    // Skip declaration directive starting from @
                    if(stylesheetBuffer[nextCharacterIndex] == '@') {
                        while(nextCharacterIndex < stylesheetBuffer.Length && stylesheetBuffer[nextCharacterIndex] != ';') {
                            nextCharacterIndex++;
                        }
                        selectorStart = nextCharacterIndex + 1;
                    }
                    nextCharacterIndex++;
                }

                if(nextCharacterIndex < stylesheetBuffer.Length) {
                    // Extract definition
                    int definitionStart = nextCharacterIndex;
                    while(nextCharacterIndex < stylesheetBuffer.Length && stylesheetBuffer[nextCharacterIndex] != '}') {
                        nextCharacterIndex++;
                    }

                    // Define a style
                    if(nextCharacterIndex - definitionStart > 2) {
                        this.AddStyleDefinition(
                            stylesheetBuffer.ToString(selectorStart, definitionStart - selectorStart),
                            stylesheetBuffer.ToString(definitionStart + 1, nextCharacterIndex - definitionStart - 2));
                    }

                    // Skip closing brace
                    if(nextCharacterIndex < stylesheetBuffer.Length) {
                        Debug.Assert(stylesheetBuffer[nextCharacterIndex] == '}');
                        nextCharacterIndex++;
                    }
                }
            }
        }

        // Returns a string with all c-style comments replaced by spaces
        private string RemoveComments(string text) {
            int commentStart = text.IndexOf("/*");
            if(commentStart < 0) {
                return text;
            }

            int commentEnd = text.IndexOf("*/", commentStart + 2);
            if(commentEnd < 0) {
                return text.Substring(0, commentStart);
            }

            return text.Substring(0, commentStart) + " " + RemoveComments(text.Substring(commentEnd + 2));
        }


        public void AddStyleDefinition(string selector, string definition) {
            // Notrmalize parameter values
            selector = selector.Trim().ToLower();
            definition = definition.Trim().ToLower();
            if(selector.Length == 0 || definition.Length == 0) {
                return;
            }

            if(_styleDefinitions == null) {
                _styleDefinitions = new List<StyleDefinition>();
            }

            string[] simpleSelectors = selector.Split(',');

            for(int i = 0; i < simpleSelectors.Length; i++) {
                string simpleSelector = simpleSelectors[i].Trim();
                if(simpleSelector.Length > 0) {
                    _styleDefinitions.Add(new StyleDefinition(simpleSelector, definition));
                }
            }
        }

        public string GetStyle(string elementName, List<XmlElement> sourceContext) {
            Debug.Assert(sourceContext.Count > 0);
            Debug.Assert(elementName == sourceContext[sourceContext.Count - 1].LocalName);

            //  Add id processing for style selectors
            if(_styleDefinitions != null) {
                for(int i = _styleDefinitions.Count - 1; i >= 0; i--) {
                    string selector = _styleDefinitions[i].Selector;

                    string[] selectorLevels = selector.Split(' ');

                    int indexInSelector = selectorLevels.Length - 1;
                    int indexInContext = sourceContext.Count - 1;
                    string selectorLevel = selectorLevels[indexInSelector].Trim();

                    if(MatchSelectorLevel(selectorLevel, sourceContext[sourceContext.Count - 1])) {
                        return _styleDefinitions[i].Definition;
                    }
                }
            }

            return null;
        }

        private bool MatchSelectorLevel(string selectorLevel, XmlElement xmlElement) {
            if(selectorLevel.Length == 0) {
                return false;
            }

            int indexOfDot = selectorLevel.IndexOf('.');
            int indexOfPound = selectorLevel.IndexOf('#');

            string selectorClass = null;
            string selectorId = null;
            string selectorTag = null;
            if(indexOfDot >= 0) {
                if(indexOfDot > 0) {
                    selectorTag = selectorLevel.Substring(0, indexOfDot);
                }
                selectorClass = selectorLevel.Substring(indexOfDot + 1);
            } else if(indexOfPound >= 0) {
                if(indexOfPound > 0) {
                    selectorTag = selectorLevel.Substring(0, indexOfPound);
                }
                selectorId = selectorLevel.Substring(indexOfPound + 1);
            } else {
                selectorTag = selectorLevel;
            }

            if(selectorTag != null && selectorTag != xmlElement.LocalName) {
                return false;
            }

            if(selectorId != null && HtmlToXamlConverter.GetAttribute(xmlElement, "id") != selectorId) {
                return false;
            }

            if(selectorClass != null && HtmlToXamlConverter.GetAttribute(xmlElement, "class") != selectorClass) {
                return false;
            }

            return true;
        }

        private class StyleDefinition {
            public StyleDefinition(string selector, string definition) {
                this.Selector = selector;
                this.Definition = definition;
            }

            public string Selector;

            public string Definition;
        }

        private List<StyleDefinition> _styleDefinitions;
    }

    /// <summary>
    /// lexical analyzer class
    /// recognizes tokens as groups of characters separated by arbitrary amounts of whitespace
    /// also classifies tokens according to type
    /// </summary>
    internal class HtmlLexicalAnalyzer {
        // ---------------------------------------------------------------------
        //
        // Constructors
        //
        // ---------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// initializes the _inputStringReader member with the string to be read
        /// also sets initial values for _nextCharacterCode and _nextTokenType
        /// </summary>
        /// <param name="inputTextString">
        /// text string to be parsed for xml content
        /// </param>
        internal HtmlLexicalAnalyzer(string inputTextString) {
            _inputStringReader = new StringReader(inputTextString);
            _nextCharacterCode = 0;
            _nextCharacter = ' ';
            _lookAheadCharacterCode = _inputStringReader.Read();
            _lookAheadCharacter = (char)_lookAheadCharacterCode;
            _previousCharacter = ' ';
            _ignoreNextWhitespace = true;
            _nextToken = new StringBuilder(100);
            _nextTokenType = HtmlTokenType.Text;
            // read the first character so we have some value for the NextCharacter property
            this.GetNextCharacter();
        }

        #endregion Constructors

        // ---------------------------------------------------------------------
        //
        // Internal methods
        //
        // ---------------------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// retrieves next recognizable token from input string
        /// and identifies its type
        /// if no valid token is found, the output parameters are set to null
        /// if end of stream is reached without matching any token, token type
        /// paramter is set to EOF
        /// </summary>
        internal void GetNextContentToken() {
            Debug.Assert(_nextTokenType != HtmlTokenType.EOF);
            _nextToken.Length = 0;
            if(this.IsAtEndOfStream) {
                _nextTokenType = HtmlTokenType.EOF;
                return;
            }

            if(this.IsAtTagStart) {
                this.GetNextCharacter();

                if(this.NextCharacter == '/') {
                    _nextToken.Append("</");
                    _nextTokenType = HtmlTokenType.ClosingTagStart;

                    // advance
                    this.GetNextCharacter();
                    _ignoreNextWhitespace = false; // Whitespaces after closing tags are significant
                } else {
                    _nextTokenType = HtmlTokenType.OpeningTagStart;
                    _nextToken.Append("<");
                    _ignoreNextWhitespace = true; // Whitespaces after opening tags are insignificant
                }
            } else if(this.IsAtDirectiveStart) {
                // either a comment or CDATA
                this.GetNextCharacter();
                if(_lookAheadCharacter == '[') {
                    // cdata
                    this.ReadDynamicContent();
                } else if(_lookAheadCharacter == '-') {
                    this.ReadComment();
                } else {
                    // neither a comment nor cdata, should be something like DOCTYPE
                    // skip till the next tag ender
                    this.ReadUnknownDirective();
                }
            } else {
                // read text content, unless you encounter a tag
                _nextTokenType = HtmlTokenType.Text;
                while(!this.IsAtTagStart && !this.IsAtEndOfStream && !this.IsAtDirectiveStart) {
                    if(this.NextCharacter == '<' && !this.IsNextCharacterEntity && _lookAheadCharacter == '?') {
                        // ignore processing directive
                        this.SkipProcessingDirective();
                    } else {
                        if(this.NextCharacter <= ' ') {
                            //  Respect xml:preserve or its equivalents for whitespace processing
                            if(_ignoreNextWhitespace) {
                                // Ignore repeated whitespaces
                            } else {
                                // Treat any control character sequence as one whitespace
                                _nextToken.Append(' ');
                            }
                            _ignoreNextWhitespace = true; // and keep ignoring the following whitespaces
                        } else {
                            _nextToken.Append(this.NextCharacter);
                            _ignoreNextWhitespace = false;
                        }
                        this.GetNextCharacter();
                    }
                }
            }
        }

        /// <summary>
        /// Unconditionally returns a token which is one of: TagEnd, EmptyTagEnd, Name, Atom or EndOfStream
        /// Does not guarantee token reader advancing.
        /// </summary>
        internal void GetNextTagToken() {
            _nextToken.Length = 0;
            if(this.IsAtEndOfStream) {
                _nextTokenType = HtmlTokenType.EOF;
                return;
            }

            this.SkipWhiteSpace();

            if(this.NextCharacter == '>' && !this.IsNextCharacterEntity) {
                // &gt; should not end a tag, so make sure it's not an entity
                _nextTokenType = HtmlTokenType.TagEnd;
                _nextToken.Append('>');
                this.GetNextCharacter();
                // Note: _ignoreNextWhitespace must be set appropriately on tag start processing
            } else if(this.NextCharacter == '/' && _lookAheadCharacter == '>') {
                // could be start of closing of empty tag
                _nextTokenType = HtmlTokenType.EmptyTagEnd;
                _nextToken.Append("/>");
                this.GetNextCharacter();
                this.GetNextCharacter();
                _ignoreNextWhitespace = false; // Whitespace after no-scope tags are sifnificant
            } else if(IsGoodForNameStart(this.NextCharacter)) {
                _nextTokenType = HtmlTokenType.Name;

                // starts a name
                // we allow character entities here
                // we do not throw exceptions here if end of stream is encountered
                // just stop and return whatever is in the token
                // if the parser is not expecting end of file after this it will call
                // the get next token function and throw an exception
                while(IsGoodForName(this.NextCharacter) && !this.IsAtEndOfStream) {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            } else {
                // Unexpected type of token for a tag. Reprot one character as Atom, expecting that HtmlParser will ignore it.
                _nextTokenType = HtmlTokenType.Atom;
                _nextToken.Append(this.NextCharacter);
                this.GetNextCharacter();
            }
        }

        /// <summary>
        /// Unconditionally returns equal sign token. Even if there is no
        /// real equal sign in the stream, it behaves as if it were there.
        /// Does not guarantee token reader advancing.
        /// </summary>
        internal void GetNextEqualSignToken() {
            Debug.Assert(_nextTokenType != HtmlTokenType.EOF);
            _nextToken.Length = 0;

            _nextToken.Append('=');
            _nextTokenType = HtmlTokenType.EqualSign;

            this.SkipWhiteSpace();

            if(this.NextCharacter == '=') {
                // '=' is not in the list of entities, so no need to check for entities here
                this.GetNextCharacter();
            }
        }

        /// <summary>
        /// Unconditionally returns an atomic value for an attribute
        /// Even if there is no appropriate token it returns Atom value
        /// Does not guarantee token reader advancing.
        /// </summary>
        internal void GetNextAtomToken() {
            Debug.Assert(_nextTokenType != HtmlTokenType.EOF);
            _nextToken.Length = 0;

            this.SkipWhiteSpace();

            _nextTokenType = HtmlTokenType.Atom;

            if((this.NextCharacter == '\'' || this.NextCharacter == '"') && !this.IsNextCharacterEntity) {
                char startingQuote = this.NextCharacter;
                this.GetNextCharacter();

                // Consume all characters between quotes
                while(!(this.NextCharacter == startingQuote && !this.IsNextCharacterEntity) && !this.IsAtEndOfStream) {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
                if(this.NextCharacter == startingQuote) {
                    this.GetNextCharacter();
                }

                // complete the quoted value
                // NOTE: our recovery here is different from IE's
                // IE keeps reading until it finds a closing quote or end of file
                // if end of file, it treats current value as text
                // if it finds a closing quote at any point within the text, it eats everything between the quotes
                // TODO: Suggestion:
                // however, we could stop when we encounter end of file or an angle bracket of any kind
                // and assume there was a quote there
                // so the attribute value may be meaningless but it is never treated as text
            } else {
                while(!this.IsAtEndOfStream && !Char.IsWhiteSpace(this.NextCharacter) && this.NextCharacter != '>') {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            }
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Internal Properties
        //
        // ---------------------------------------------------------------------

        #region Internal Properties

        internal HtmlTokenType NextTokenType {
            get {
                return _nextTokenType;
            }
        }

        internal string NextToken {
            get {
                return _nextToken.ToString();
            }
        }

        #endregion Internal Properties

        // ---------------------------------------------------------------------
        //
        // Private methods
        //
        // ---------------------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Advances a reading position by one character code
        /// and reads the next availbale character from a stream.
        /// This character becomes available as NextCharacter property.
        /// </summary>
        /// <remarks>
        /// Throws InvalidOperationException if attempted to be called on EndOfStream
        /// condition.
        /// </remarks>
        private void GetNextCharacter() {
            if(_nextCharacterCode == -1) {
                throw new InvalidOperationException("GetNextCharacter method called at the end of a stream");
            }

            _previousCharacter = _nextCharacter;

            _nextCharacter = _lookAheadCharacter;
            _nextCharacterCode = _lookAheadCharacterCode;
            // next character not an entity as of now
            _isNextCharacterEntity = false;

            this.ReadLookAheadCharacter();

            if(_nextCharacter == '&') {
                if(_lookAheadCharacter == '#') {
                    // numeric entity - parse digits - &#DDDDD;
                    int entityCode;
                    entityCode = 0;
                    this.ReadLookAheadCharacter();

                    // largest numeric entity is 7 characters
                    for(int i = 0; i < 7 && Char.IsDigit(_lookAheadCharacter); i++) {
                        entityCode = 10 * entityCode + (_lookAheadCharacterCode - (int)'0');
                        this.ReadLookAheadCharacter();
                    }
                    if(_lookAheadCharacter == ';') {
                        // correct format - advance
                        this.ReadLookAheadCharacter();
                        _nextCharacterCode = entityCode;

                        // if this is out of range it will set the character to '?'
                        _nextCharacter = (char)_nextCharacterCode;

                        // as far as we are concerned, this is an entity
                        _isNextCharacterEntity = true;
                    } else {
                        // not an entity, set next character to the current lookahread character
                        // we would have eaten up some digits
                        _nextCharacter = _lookAheadCharacter;
                        _nextCharacterCode = _lookAheadCharacterCode;
                        this.ReadLookAheadCharacter();
                        _isNextCharacterEntity = false;
                    }
                } else if(Char.IsLetter(_lookAheadCharacter)) {
                    // entity is written as a string
                    string entity = "";

                    // maximum length of string entities is 10 characters
                    for(int i = 0; i < 10 && (Char.IsLetter(_lookAheadCharacter) || Char.IsDigit(_lookAheadCharacter)); i++) {
                        entity += _lookAheadCharacter;
                        this.ReadLookAheadCharacter();
                    }
                    if(_lookAheadCharacter == ';') {
                        // advance
                        this.ReadLookAheadCharacter();

                        if(HtmlSchema.IsEntity(entity)) {
                            _nextCharacter = HtmlSchema.EntityCharacterValue(entity);
                            _nextCharacterCode = (int)_nextCharacter;
                            _isNextCharacterEntity = true;
                        } else {
                            // just skip the whole thing - invalid entity
                            // move on to the next character
                            _nextCharacter = _lookAheadCharacter;
                            _nextCharacterCode = _lookAheadCharacterCode;
                            this.ReadLookAheadCharacter();

                            // not an entity
                            _isNextCharacterEntity = false;
                        }
                    } else {
                        // skip whatever we read after the ampersand
                        // set next character and move on
                        _nextCharacter = _lookAheadCharacter;
                        this.ReadLookAheadCharacter();
                        _isNextCharacterEntity = false;
                    }
                }
            }
        }

        private void ReadLookAheadCharacter() {
            if(_lookAheadCharacterCode != -1) {
                _lookAheadCharacterCode = _inputStringReader.Read();
                _lookAheadCharacter = (char)_lookAheadCharacterCode;
            }
        }

        /// <summary>
        /// skips whitespace in the input string
        /// leaves the first non-whitespace character available in the NextCharacter property
        /// this may be the end-of-file character, it performs no checking
        /// </summary>
        private void SkipWhiteSpace() {
            // TODO: handle character entities while processing comments, cdata, and directives
            // TODO: SUGGESTION: we could check if lookahead and previous characters are entities also
            while(true) {
                if(_nextCharacter == '<' && (_lookAheadCharacter == '?' || _lookAheadCharacter == '!')) {
                    this.GetNextCharacter();

                    if(_lookAheadCharacter == '[') {
                        // Skip CDATA block and DTDs(?)
                        while(!this.IsAtEndOfStream && !(_previousCharacter == ']' && _nextCharacter == ']' && _lookAheadCharacter == '>')) {
                            this.GetNextCharacter();
                        }
                        if(_nextCharacter == '>') {
                            this.GetNextCharacter();
                        }
                    } else {
                        // Skip processing instruction, comments
                        while(!this.IsAtEndOfStream && _nextCharacter != '>') {
                            this.GetNextCharacter();
                        }
                        if(_nextCharacter == '>') {
                            this.GetNextCharacter();
                        }
                    }
                }


                if(!Char.IsWhiteSpace(this.NextCharacter)) {
                    break;
                }

                this.GetNextCharacter();
            }
        }

        /// <summary>
        /// checks if a character can be used to start a name
        /// if this check is true then the rest of the name can be read
        /// </summary>
        /// <param name="character">
        /// character value to be checked
        /// </param>
        /// <returns>
        /// true if the character can be the first character in a name
        /// false otherwise
        /// </returns>
        private bool IsGoodForNameStart(char character) {
            return character == '_' || Char.IsLetter(character);
        }

        /// <summary>
        /// checks if a character can be used as a non-starting character in a name
        /// uses the IsExtender and IsCombiningCharacter predicates to see
        /// if a character is an extender or a combining character
        /// </summary>
        /// <param name="character">
        /// character to be checked for validity in a name
        /// </param>
        /// <returns>
        /// true if the character can be a valid part of a name
        /// </returns>
        private bool IsGoodForName(char character) {
            // we are not concerned with escaped characters in names
            // we assume that character entities are allowed as part of a name
            return
                this.IsGoodForNameStart(character) ||
                character == '.' ||
                character == '-' ||
                character == ':' ||
                Char.IsDigit(character) ||
                IsCombiningCharacter(character) ||
                IsExtender(character);
        }

        /// <summary>
        /// identifies a character as being a combining character, permitted in a name
        /// TODO: only a placeholder for now but later to be replaced with comparisons against
        /// the list of combining characters in the XML documentation
        /// </summary>
        /// <param name="character">
        /// character to be checked
        /// </param>
        /// <returns>
        /// true if the character is a combining character, false otherwise
        /// </returns>
        private bool IsCombiningCharacter(char character) {
            // TODO: put actual code with checks against all combining characters here
            return false;
        }

        /// <summary>
        /// identifies a character as being an extender, permitted in a name
        /// TODO: only a placeholder for now but later to be replaced with comparisons against
        /// the list of extenders in the XML documentation
        /// </summary>
        /// <param name="character">
        /// character to be checked
        /// </param>
        /// <returns>
        /// true if the character is an extender, false otherwise
        /// </returns>
        private bool IsExtender(char character) {
            // TODO: put actual code with checks against all extenders here
            return false;
        }

        /// <summary>
        /// skips dynamic content starting with '<![' and ending with ']>'
        /// </summary>
        private void ReadDynamicContent() {
            // verify that we are at dynamic content, which may include CDATA
            Debug.Assert(_previousCharacter == '<' && _nextCharacter == '!' && _lookAheadCharacter == '[');

            // Let's treat this as empty text
            _nextTokenType = HtmlTokenType.Text;
            _nextToken.Length = 0;

            // advance twice, once to get the lookahead character and then to reach the start of the cdata
            this.GetNextCharacter();
            this.GetNextCharacter();

            // NOTE: 10/12/2004: modified this function to check when called if's reading CDATA or something else
            // some directives may start with a <![ and then have some data and they will just end with a ]>
            // this function is modified to stop at the sequence ]> and not ]]>
            // this means that CDATA and anything else expressed in their own set of [] within the <! [...]>
            // directive cannot contain a ]> sequence. However it is doubtful that cdata could contain such
            // sequence anyway, it probably stops at the first ]
            while(!(_nextCharacter == ']' && _lookAheadCharacter == '>') && !this.IsAtEndOfStream) {
                // advance
                this.GetNextCharacter();
            }

            if(!this.IsAtEndOfStream) {
                // advance, first to the last >
                this.GetNextCharacter();

                // then advance past it to the next character after processing directive
                this.GetNextCharacter();
            }
        }

        /// <summary>
        /// skips comments starting with '<!-' and ending with '-->'
        /// NOTE: 10/06/2004: processing changed, will now skip anything starting with
        /// the "<!-"  sequence and ending in "!>" or "->", because in practice many html pages do not
        /// use the full comment specifying conventions
        /// </summary>
        private void ReadComment() {
            // verify that we are at a comment
            Debug.Assert(_previousCharacter == '<' && _nextCharacter == '!' && _lookAheadCharacter == '-');

            // Initialize a token
            _nextTokenType = HtmlTokenType.Comment;
            _nextToken.Length = 0;

            // advance to the next character, so that to be at the start of comment value
            this.GetNextCharacter(); // get first '-'
            this.GetNextCharacter(); // get second '-'
            this.GetNextCharacter(); // get first character of comment content

            while(true) {
                // Read text until end of comment
                // Note that in many actual html pages comments end with "!>" (while xml standard is "-->")
                while(!this.IsAtEndOfStream && !(_nextCharacter == '-' && _lookAheadCharacter == '-' || _nextCharacter == '!' && _lookAheadCharacter == '>')) {
                    _nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }

                // Finish comment reading
                this.GetNextCharacter();
                if(_previousCharacter == '-' && _nextCharacter == '-' && _lookAheadCharacter == '>') {
                    // Standard comment end. Eat it and exit the loop
                    this.GetNextCharacter(); // get '>'
                    break;
                } else if(_previousCharacter == '!' && _nextCharacter == '>') {
                    // Nonstandard but possible comment end - '!>'. Exit the loop
                    break;
                } else {
                    // Not an end. Save character and continue continue reading
                    _nextToken.Append(_previousCharacter);
                    continue;
                }
            }

            // Read end of comment combination
            if(_nextCharacter == '>') {
                this.GetNextCharacter();
            }
        }

        /// <summary>
        /// skips past unknown directives that start with "<!" but are not comments or Cdata
        /// ignores content of such directives until the next ">" character
        /// applies to directives such as DOCTYPE, etc that we do not presently support
        /// </summary>
        private void ReadUnknownDirective() {
            // verify that we are at an unknown directive
            Debug.Assert(_previousCharacter == '<' && _nextCharacter == '!' && !(_lookAheadCharacter == '-' || _lookAheadCharacter == '['));

            // Let's treat this as empty text
            _nextTokenType = HtmlTokenType.Text;
            _nextToken.Length = 0;

            // advance to the next character
            this.GetNextCharacter();

            // skip to the first tag end we find
            while(!(_nextCharacter == '>' && !IsNextCharacterEntity) && !this.IsAtEndOfStream) {
                this.GetNextCharacter();
            }

            if(!this.IsAtEndOfStream) {
                // advance past the tag end
                this.GetNextCharacter();
            }
        }

        /// <summary>
        /// skips processing directives starting with the characters '<?' and ending with '?>'
        /// NOTE: 10/14/2004: IE also ends processing directives with a />, so this function is
        /// being modified to recognize that condition as well
        /// </summary>
        private void SkipProcessingDirective() {
            // verify that we are at a processing directive
            Debug.Assert(_nextCharacter == '<' && _lookAheadCharacter == '?');

            // advance twice, once to get the lookahead character and then to reach the start of the drective
            this.GetNextCharacter();
            this.GetNextCharacter();

            while(!((_nextCharacter == '?' || _nextCharacter == '/') && _lookAheadCharacter == '>') && !this.IsAtEndOfStream) {
                // advance
                // we don't need to check for entities here because '?' is not an entity
                // and even though > is an entity there is no entity processing when reading lookahead character
                this.GetNextCharacter();
            }

            if(!this.IsAtEndOfStream) {
                // advance, first to the last >
                this.GetNextCharacter();

                // then advance past it to the next character after processing directive
                this.GetNextCharacter();
            }
        }

        #endregion Private Methods

        // ---------------------------------------------------------------------
        //
        // Private Properties
        //
        // ---------------------------------------------------------------------

        #region Private Properties

        private char NextCharacter {
            get {
                return _nextCharacter;
            }
        }

        private bool IsAtEndOfStream {
            get {
                return _nextCharacterCode == -1;
            }
        }

        private bool IsAtTagStart {
            get {
                return _nextCharacter == '<' && (_lookAheadCharacter == '/' || IsGoodForNameStart(_lookAheadCharacter)) && !_isNextCharacterEntity;
            }
        }

        private bool IsAtTagEnd {
            // check if at end of empty tag or regular tag
            get {
                return (_nextCharacter == '>' || (_nextCharacter == '/' && _lookAheadCharacter == '>')) && !_isNextCharacterEntity;
            }
        }

        private bool IsAtDirectiveStart {
            get {
                return (_nextCharacter == '<' && _lookAheadCharacter == '!' && !this.IsNextCharacterEntity);
            }
        }

        private bool IsNextCharacterEntity {
            // check if next character is an entity
            get {
                return _isNextCharacterEntity;
            }
        }

        #endregion Private Properties

        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------

        #region Private Fields

        // string reader which will move over input text
        private StringReader _inputStringReader;
        // next character code read from input that is not yet part of any token
        // and the character it represents
        private int _nextCharacterCode;
        private char _nextCharacter;
        private int _lookAheadCharacterCode;
        private char _lookAheadCharacter;
        private char _previousCharacter;
        private bool _ignoreNextWhitespace;
        private bool _isNextCharacterEntity;

        // store token and type in local variables before copying them to output parameters
        StringBuilder _nextToken;
        HtmlTokenType _nextTokenType;

        #endregion Private Fields
    }

    /// <summary>
    /// HtmlSchema class
    /// maintains static information about HTML structure
    /// can be used by HtmlParser to check conditions under which an element starts or ends, etc.
    /// </summary>
    internal class HtmlSchema {
        // ---------------------------------------------------------------------
        //
        // Constructors
        //
        // ---------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// static constructor, initializes the ArrayLists
        /// that hold the elements in various sub-components of the schema
        /// e.g _htmlEmptyElements, etc.
        /// </summary>
        static HtmlSchema() {
            // initializes the list of all html elements
            InitializeInlineElements();

            InitializeBlockElements();

            InitializeOtherOpenableElements();

            // initialize empty elements list
            InitializeEmptyElements();

            // initialize list of elements closing on the outer element end
            InitializeElementsClosingOnParentElementEnd();

            // initalize list of elements that close when a new element starts
            InitializeElementsClosingOnNewElementStart();

            // Initialize character entities
            InitializeHtmlCharacterEntities();
        }

        #endregion Constructors;

        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// returns true when xmlElementName corresponds to empty element
        /// </summary>
        /// <param name="xmlElementName">
        /// string representing name to test
        /// </param>
        internal static bool IsEmptyElement(string xmlElementName) {
            // convert to lowercase before we check
            // because element names are not case sensitive
            return _htmlEmptyElements.Contains(xmlElementName.ToLower());
        }

        /// <summary>
        /// returns true if xmlElementName represents a block formattinng element.
        /// It used in an algorithm of transferring inline elements over block elements
        /// in HtmlParser
        /// </summary>
        /// <param name="xmlElementName"></param>
        /// <returns></returns>
        internal static bool IsBlockElement(string xmlElementName) {
            return _htmlBlockElements.Contains(xmlElementName);
        }

        /// <summary>
        /// returns true if the xmlElementName represents an inline formatting element
        /// </summary>
        /// <param name="xmlElementName"></param>
        /// <returns></returns>
        internal static bool IsInlineElement(string xmlElementName) {
            return _htmlInlineElements.Contains(xmlElementName);
        }

        /// <summary>
        /// It is a list of known html elements which we
        /// want to allow to produce bt HTML parser,
        /// but don'tt want to act as inline, block or no-scope.
        /// Presence in this list will allow to open
        /// elements during html parsing, and adding the
        /// to a tree produced by html parser.
        /// </summary>
        internal static bool IsKnownOpenableElement(string xmlElementName) {
            return _htmlOtherOpenableElements.Contains(xmlElementName);
        }

        /// <summary>
        /// returns true when xmlElementName closes when the outer element closes
        /// this is true of elements with optional start tags
        /// </summary>
        /// <param name="xmlElementName">
        /// string representing name to test
        /// </param>
        internal static bool ClosesOnParentElementEnd(string xmlElementName) {
            // convert to lowercase when testing
            return _htmlElementsClosingOnParentElementEnd.Contains(xmlElementName.ToLower());
        }

        /// <summary>
        /// returns true if the current element closes when the new element, whose name has just been read, starts
        /// </summary>
        /// <param name="currentElementName">
        /// string representing current element name
        /// </param>
        /// <param name="elementName"></param>
        /// string representing name of the next element that will start
        internal static bool ClosesOnNextElementStart(string currentElementName, string nextElementName) {
            Debug.Assert(currentElementName == currentElementName.ToLower());
            switch(currentElementName) {
                case "colgroup":
                    return _htmlElementsClosingColgroup.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "dd":
                    return _htmlElementsClosingDD.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "dt":
                    return _htmlElementsClosingDT.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "li":
                    return _htmlElementsClosingLI.Contains(nextElementName);
                case "p":
                    return HtmlSchema.IsBlockElement(nextElementName);
                case "tbody":
                    return _htmlElementsClosingTbody.Contains(nextElementName);
                case "tfoot":
                    return _htmlElementsClosingTfoot.Contains(nextElementName);
                case "thead":
                    return _htmlElementsClosingThead.Contains(nextElementName);
                case "tr":
                    return _htmlElementsClosingTR.Contains(nextElementName);
                case "td":
                    return _htmlElementsClosingTD.Contains(nextElementName);
                case "th":
                    return _htmlElementsClosingTH.Contains(nextElementName);
            }
            return false;
        }

        /// <summary>
        /// returns true if the string passed as argument is an Html entity name
        /// </summary>
        /// <param name="entityName">
        /// string to be tested for Html entity name
        /// </param>
        internal static bool IsEntity(string entityName) {
            // we do not convert entity strings to lowercase because these names are case-sensitive
            if(_htmlCharacterEntities.Contains(entityName)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// returns the character represented by the entity name string which is passed as an argument, if the string is an entity name
        /// as specified in _htmlCharacterEntities, returns the character value of 0 otherwise
        /// </summary>
        /// <param name="entityName">
        /// string representing entity name whose character value is desired
        /// </param>
        internal static char EntityCharacterValue(string entityName) {
            if(_htmlCharacterEntities.Contains(entityName)) {
                return (char)_htmlCharacterEntities[entityName];
            } else {
                return (char)0;
            }
        }

        #endregion Internal Methods


        // ---------------------------------------------------------------------
        //
        //  Internal Properties
        //
        // ---------------------------------------------------------------------

        #region Internal Properties

        #endregion Internal Indexers


        // ---------------------------------------------------------------------
        //
        // Private Methods
        //
        // ---------------------------------------------------------------------


        #region Private Methods

        private static void InitializeInlineElements() {
            _htmlInlineElements = new ArrayList();
            _htmlInlineElements.Add("a");
            _htmlInlineElements.Add("abbr");
            _htmlInlineElements.Add("acronym");
            _htmlInlineElements.Add("address");
            _htmlInlineElements.Add("b");
            _htmlInlineElements.Add("bdo"); // ???
            _htmlInlineElements.Add("big");
            _htmlInlineElements.Add("button");
            _htmlInlineElements.Add("code");
            _htmlInlineElements.Add("del"); // deleted text
            _htmlInlineElements.Add("dfn");
            _htmlInlineElements.Add("em");
            _htmlInlineElements.Add("font");
            _htmlInlineElements.Add("i");
            _htmlInlineElements.Add("ins"); // inserted text
            _htmlInlineElements.Add("kbd"); // text to entered by a user
            _htmlInlineElements.Add("label");
            _htmlInlineElements.Add("legend"); // ???
            _htmlInlineElements.Add("q"); // short inline quotation
            _htmlInlineElements.Add("s"); // strike-through text style
            _htmlInlineElements.Add("samp"); // Specifies a code sample
            _htmlInlineElements.Add("small");
            _htmlInlineElements.Add("span");
            _htmlInlineElements.Add("strike");
            _htmlInlineElements.Add("strong");
            _htmlInlineElements.Add("sub");
            _htmlInlineElements.Add("sup");
            _htmlInlineElements.Add("u");
            _htmlInlineElements.Add("var"); // indicates an instance of a program variable
        }

        private static void InitializeBlockElements() {
            _htmlBlockElements = new ArrayList();

            _htmlBlockElements.Add("blockquote");
            _htmlBlockElements.Add("body");
            _htmlBlockElements.Add("caption");
            _htmlBlockElements.Add("center");
            _htmlBlockElements.Add("cite");
            _htmlBlockElements.Add("dd");
            _htmlBlockElements.Add("dir"); //  treat as UL element
            _htmlBlockElements.Add("div");
            _htmlBlockElements.Add("dl");
            _htmlBlockElements.Add("dt");
            _htmlBlockElements.Add("form"); // Not a block according to XHTML spec
            _htmlBlockElements.Add("h1");
            _htmlBlockElements.Add("h2");
            _htmlBlockElements.Add("h3");
            _htmlBlockElements.Add("h4");
            _htmlBlockElements.Add("h5");
            _htmlBlockElements.Add("h6");
            _htmlBlockElements.Add("html");
            _htmlBlockElements.Add("li");
            _htmlBlockElements.Add("menu"); //  treat as UL element
            _htmlBlockElements.Add("ol");
            _htmlBlockElements.Add("p");
            _htmlBlockElements.Add("pre"); // Renders text in a fixed-width font
            _htmlBlockElements.Add("table");
            _htmlBlockElements.Add("tbody");
            _htmlBlockElements.Add("td");
            _htmlBlockElements.Add("textarea");
            _htmlBlockElements.Add("tfoot");
            _htmlBlockElements.Add("th");
            _htmlBlockElements.Add("thead");
            _htmlBlockElements.Add("tr");
            _htmlBlockElements.Add("tt");
            _htmlBlockElements.Add("ul");
        }

        /// <summary>
        /// initializes _htmlEmptyElements with empty elements in HTML 4 spec at
        /// http://www.w3.org/TR/REC-html40/index/elements.html
        /// </summary>
        private static void InitializeEmptyElements() {
            // Build a list of empty (no-scope) elements
            // (element not requiring closing tags, and not accepting any content)
            _htmlEmptyElements = new ArrayList();
            _htmlEmptyElements.Add("area");
            _htmlEmptyElements.Add("base");
            _htmlEmptyElements.Add("basefont");
            _htmlEmptyElements.Add("br");
            _htmlEmptyElements.Add("col");
            _htmlEmptyElements.Add("frame");
            _htmlEmptyElements.Add("hr");
            _htmlEmptyElements.Add("img");
            _htmlEmptyElements.Add("input");
            _htmlEmptyElements.Add("isindex");
            _htmlEmptyElements.Add("link");
            _htmlEmptyElements.Add("meta");
            _htmlEmptyElements.Add("param");
        }

        private static void InitializeOtherOpenableElements() {
            // It is a list of known html elements which we
            // want to allow to produce bt HTML parser,
            // but don'tt want to act as inline, block or no-scope.
            // Presence in this list will allow to open
            // elements during html parsing, and adding the
            // to a tree produced by html parser.
            _htmlOtherOpenableElements = new ArrayList();
            _htmlOtherOpenableElements.Add("applet");
            _htmlOtherOpenableElements.Add("base");
            _htmlOtherOpenableElements.Add("basefont");
            _htmlOtherOpenableElements.Add("colgroup");
            _htmlOtherOpenableElements.Add("fieldset");
            //_htmlOtherOpenableElements.Add("form"); --> treated as block
            _htmlOtherOpenableElements.Add("frameset");
            _htmlOtherOpenableElements.Add("head");
            _htmlOtherOpenableElements.Add("iframe");
            _htmlOtherOpenableElements.Add("map");
            _htmlOtherOpenableElements.Add("noframes");
            _htmlOtherOpenableElements.Add("noscript");
            _htmlOtherOpenableElements.Add("object");
            _htmlOtherOpenableElements.Add("optgroup");
            _htmlOtherOpenableElements.Add("option");
            _htmlOtherOpenableElements.Add("script");
            _htmlOtherOpenableElements.Add("select");
            _htmlOtherOpenableElements.Add("style");
            _htmlOtherOpenableElements.Add("title");
        }

        /// <summary>
        /// initializes _htmlElementsClosingOnParentElementEnd with the list of HTML 4 elements for which closing tags are optional
        /// we assume that for any element for which closing tags are optional, the element closes when it's outer element
        /// (in which it is nested) does
        /// </summary>
        private static void InitializeElementsClosingOnParentElementEnd() {
            _htmlElementsClosingOnParentElementEnd = new ArrayList();
            _htmlElementsClosingOnParentElementEnd.Add("body");
            _htmlElementsClosingOnParentElementEnd.Add("colgroup");
            _htmlElementsClosingOnParentElementEnd.Add("dd");
            _htmlElementsClosingOnParentElementEnd.Add("dt");
            _htmlElementsClosingOnParentElementEnd.Add("head");
            _htmlElementsClosingOnParentElementEnd.Add("html");
            _htmlElementsClosingOnParentElementEnd.Add("li");
            _htmlElementsClosingOnParentElementEnd.Add("p");
            _htmlElementsClosingOnParentElementEnd.Add("tbody");
            _htmlElementsClosingOnParentElementEnd.Add("td");
            _htmlElementsClosingOnParentElementEnd.Add("tfoot");
            _htmlElementsClosingOnParentElementEnd.Add("thead");
            _htmlElementsClosingOnParentElementEnd.Add("th");
            _htmlElementsClosingOnParentElementEnd.Add("tr");
        }

        private static void InitializeElementsClosingOnNewElementStart() {
            _htmlElementsClosingColgroup = new ArrayList();
            _htmlElementsClosingColgroup.Add("colgroup");
            _htmlElementsClosingColgroup.Add("tr");
            _htmlElementsClosingColgroup.Add("thead");
            _htmlElementsClosingColgroup.Add("tfoot");
            _htmlElementsClosingColgroup.Add("tbody");

            _htmlElementsClosingDD = new ArrayList();
            _htmlElementsClosingDD.Add("dd");
            _htmlElementsClosingDD.Add("dt");
            // TODO: dd may end in other cases as well - if a new "p" starts, etc.
            // TODO: these are the basic "legal" cases but there may be more recovery

            _htmlElementsClosingDT = new ArrayList();
            _htmlElementsClosingDD.Add("dd");
            _htmlElementsClosingDD.Add("dt");
            // TODO: dd may end in other cases as well - if a new "p" starts, etc.
            // TODO: these are the basic "legal" cases but there may be more recovery

            _htmlElementsClosingLI = new ArrayList();
            _htmlElementsClosingLI.Add("li");
            // TODO: more complex recovery

            _htmlElementsClosingTbody = new ArrayList();
            _htmlElementsClosingTbody.Add("tbody");
            _htmlElementsClosingTbody.Add("thead");
            _htmlElementsClosingTbody.Add("tfoot");
            // TODO: more complex recovery

            _htmlElementsClosingTR = new ArrayList();
            // NOTE: tr should not really close on a new thead
            // because if there are rows before a thead, it is assumed to be in tbody, whose start tag is optional
            // and thead can't come after tbody
            // however, if we do encounter this, it's probably best to end the row and ignore the thead or treat
            // it as part of the table
            _htmlElementsClosingTR.Add("thead");
            _htmlElementsClosingTR.Add("tfoot");
            _htmlElementsClosingTR.Add("tbody");
            _htmlElementsClosingTR.Add("tr");
            // TODO: more complex recovery

            _htmlElementsClosingTD = new ArrayList();
            _htmlElementsClosingTD.Add("td");
            _htmlElementsClosingTD.Add("th");
            _htmlElementsClosingTD.Add("tr");
            _htmlElementsClosingTD.Add("tbody");
            _htmlElementsClosingTD.Add("tfoot");
            _htmlElementsClosingTD.Add("thead");
            // TODO: more complex recovery

            _htmlElementsClosingTH = new ArrayList();
            _htmlElementsClosingTH.Add("td");
            _htmlElementsClosingTH.Add("th");
            _htmlElementsClosingTH.Add("tr");
            _htmlElementsClosingTH.Add("tbody");
            _htmlElementsClosingTH.Add("tfoot");
            _htmlElementsClosingTH.Add("thead");
            // TODO: more complex recovery

            _htmlElementsClosingThead = new ArrayList();
            _htmlElementsClosingThead.Add("tbody");
            _htmlElementsClosingThead.Add("tfoot");
            // TODO: more complex recovery

            _htmlElementsClosingTfoot = new ArrayList();
            _htmlElementsClosingTfoot.Add("tbody");
            // although thead comes before tfoot, we add it because if it is found the tfoot should close
            // and some recovery processing be done on the thead
            _htmlElementsClosingTfoot.Add("thead");
            // TODO: more complex recovery
        }

        /// <summary>
        /// initializes _htmlCharacterEntities hashtable with the character corresponding to entity names
        /// </summary>
        private static void InitializeHtmlCharacterEntities() {
            _htmlCharacterEntities = new Hashtable();
            _htmlCharacterEntities["Aacute"] = (char)193;
            _htmlCharacterEntities["aacute"] = (char)225;
            _htmlCharacterEntities["Acirc"] = (char)194;
            _htmlCharacterEntities["acirc"] = (char)226;
            _htmlCharacterEntities["acute"] = (char)180;
            _htmlCharacterEntities["AElig"] = (char)198;
            _htmlCharacterEntities["aelig"] = (char)230;
            _htmlCharacterEntities["Agrave"] = (char)192;
            _htmlCharacterEntities["agrave"] = (char)224;
            _htmlCharacterEntities["alefsym"] = (char)8501;
            _htmlCharacterEntities["Alpha"] = (char)913;
            _htmlCharacterEntities["alpha"] = (char)945;
            _htmlCharacterEntities["amp"] = (char)38;
            _htmlCharacterEntities["and"] = (char)8743;
            _htmlCharacterEntities["ang"] = (char)8736;
            _htmlCharacterEntities["Aring"] = (char)197;
            _htmlCharacterEntities["aring"] = (char)229;
            _htmlCharacterEntities["asymp"] = (char)8776;
            _htmlCharacterEntities["Atilde"] = (char)195;
            _htmlCharacterEntities["atilde"] = (char)227;
            _htmlCharacterEntities["Auml"] = (char)196;
            _htmlCharacterEntities["auml"] = (char)228;
            _htmlCharacterEntities["bdquo"] = (char)8222;
            _htmlCharacterEntities["Beta"] = (char)914;
            _htmlCharacterEntities["beta"] = (char)946;
            _htmlCharacterEntities["brvbar"] = (char)166;
            _htmlCharacterEntities["bull"] = (char)8226;
            _htmlCharacterEntities["cap"] = (char)8745;
            _htmlCharacterEntities["Ccedil"] = (char)199;
            _htmlCharacterEntities["ccedil"] = (char)231;
            _htmlCharacterEntities["cent"] = (char)162;
            _htmlCharacterEntities["Chi"] = (char)935;
            _htmlCharacterEntities["chi"] = (char)967;
            _htmlCharacterEntities["circ"] = (char)710;
            _htmlCharacterEntities["clubs"] = (char)9827;
            _htmlCharacterEntities["cong"] = (char)8773;
            _htmlCharacterEntities["copy"] = (char)169;
            _htmlCharacterEntities["crarr"] = (char)8629;
            _htmlCharacterEntities["cup"] = (char)8746;
            _htmlCharacterEntities["curren"] = (char)164;
            _htmlCharacterEntities["dagger"] = (char)8224;
            _htmlCharacterEntities["Dagger"] = (char)8225;
            _htmlCharacterEntities["darr"] = (char)8595;
            _htmlCharacterEntities["dArr"] = (char)8659;
            _htmlCharacterEntities["deg"] = (char)176;
            _htmlCharacterEntities["Delta"] = (char)916;
            _htmlCharacterEntities["delta"] = (char)948;
            _htmlCharacterEntities["diams"] = (char)9830;
            _htmlCharacterEntities["divide"] = (char)247;
            _htmlCharacterEntities["Eacute"] = (char)201;
            _htmlCharacterEntities["eacute"] = (char)233;
            _htmlCharacterEntities["Ecirc"] = (char)202;
            _htmlCharacterEntities["ecirc"] = (char)234;
            _htmlCharacterEntities["Egrave"] = (char)200;
            _htmlCharacterEntities["egrave"] = (char)232;
            _htmlCharacterEntities["empty"] = (char)8709;
            _htmlCharacterEntities["emsp"] = (char)8195;
            _htmlCharacterEntities["ensp"] = (char)8194;
            _htmlCharacterEntities["Epsilon"] = (char)917;
            _htmlCharacterEntities["epsilon"] = (char)949;
            _htmlCharacterEntities["equiv"] = (char)8801;
            _htmlCharacterEntities["Eta"] = (char)919;
            _htmlCharacterEntities["eta"] = (char)951;
            _htmlCharacterEntities["ETH"] = (char)208;
            _htmlCharacterEntities["eth"] = (char)240;
            _htmlCharacterEntities["Euml"] = (char)203;
            _htmlCharacterEntities["euml"] = (char)235;
            _htmlCharacterEntities["euro"] = (char)8364;
            _htmlCharacterEntities["exist"] = (char)8707;
            _htmlCharacterEntities["fnof"] = (char)402;
            _htmlCharacterEntities["forall"] = (char)8704;
            _htmlCharacterEntities["frac12"] = (char)189;
            _htmlCharacterEntities["frac14"] = (char)188;
            _htmlCharacterEntities["frac34"] = (char)190;
            _htmlCharacterEntities["frasl"] = (char)8260;
            _htmlCharacterEntities["Gamma"] = (char)915;
            _htmlCharacterEntities["gamma"] = (char)947;
            _htmlCharacterEntities["ge"] = (char)8805;
            _htmlCharacterEntities["gt"] = (char)62;
            _htmlCharacterEntities["harr"] = (char)8596;
            _htmlCharacterEntities["hArr"] = (char)8660;
            _htmlCharacterEntities["hearts"] = (char)9829;
            _htmlCharacterEntities["hellip"] = (char)8230;
            _htmlCharacterEntities["Iacute"] = (char)205;
            _htmlCharacterEntities["iacute"] = (char)237;
            _htmlCharacterEntities["Icirc"] = (char)206;
            _htmlCharacterEntities["icirc"] = (char)238;
            _htmlCharacterEntities["iexcl"] = (char)161;
            _htmlCharacterEntities["Igrave"] = (char)204;
            _htmlCharacterEntities["igrave"] = (char)236;
            _htmlCharacterEntities["image"] = (char)8465;
            _htmlCharacterEntities["infin"] = (char)8734;
            _htmlCharacterEntities["int"] = (char)8747;
            _htmlCharacterEntities["Iota"] = (char)921;
            _htmlCharacterEntities["iota"] = (char)953;
            _htmlCharacterEntities["iquest"] = (char)191;
            _htmlCharacterEntities["isin"] = (char)8712;
            _htmlCharacterEntities["Iuml"] = (char)207;
            _htmlCharacterEntities["iuml"] = (char)239;
            _htmlCharacterEntities["Kappa"] = (char)922;
            _htmlCharacterEntities["kappa"] = (char)954;
            _htmlCharacterEntities["Lambda"] = (char)923;
            _htmlCharacterEntities["lambda"] = (char)955;
            _htmlCharacterEntities["lang"] = (char)9001;
            _htmlCharacterEntities["laquo"] = (char)171;
            _htmlCharacterEntities["larr"] = (char)8592;
            _htmlCharacterEntities["lArr"] = (char)8656;
            _htmlCharacterEntities["lceil"] = (char)8968;
            _htmlCharacterEntities["ldquo"] = (char)8220;
            _htmlCharacterEntities["le"] = (char)8804;
            _htmlCharacterEntities["lfloor"] = (char)8970;
            _htmlCharacterEntities["lowast"] = (char)8727;
            _htmlCharacterEntities["loz"] = (char)9674;
            _htmlCharacterEntities["lrm"] = (char)8206;
            _htmlCharacterEntities["lsaquo"] = (char)8249;
            _htmlCharacterEntities["lsquo"] = (char)8216;
            _htmlCharacterEntities["lt"] = (char)60;
            _htmlCharacterEntities["macr"] = (char)175;
            _htmlCharacterEntities["mdash"] = (char)8212;
            _htmlCharacterEntities["micro"] = (char)181;
            _htmlCharacterEntities["middot"] = (char)183;
            _htmlCharacterEntities["minus"] = (char)8722;
            _htmlCharacterEntities["Mu"] = (char)924;
            _htmlCharacterEntities["mu"] = (char)956;
            _htmlCharacterEntities["nabla"] = (char)8711;
            _htmlCharacterEntities["nbsp"] = (char)160;
            _htmlCharacterEntities["ndash"] = (char)8211;
            _htmlCharacterEntities["ne"] = (char)8800;
            _htmlCharacterEntities["ni"] = (char)8715;
            _htmlCharacterEntities["not"] = (char)172;
            _htmlCharacterEntities["notin"] = (char)8713;
            _htmlCharacterEntities["nsub"] = (char)8836;
            _htmlCharacterEntities["Ntilde"] = (char)209;
            _htmlCharacterEntities["ntilde"] = (char)241;
            _htmlCharacterEntities["Nu"] = (char)925;
            _htmlCharacterEntities["nu"] = (char)957;
            _htmlCharacterEntities["Oacute"] = (char)211;
            _htmlCharacterEntities["ocirc"] = (char)244;
            _htmlCharacterEntities["OElig"] = (char)338;
            _htmlCharacterEntities["oelig"] = (char)339;
            _htmlCharacterEntities["Ograve"] = (char)210;
            _htmlCharacterEntities["ograve"] = (char)242;
            _htmlCharacterEntities["oline"] = (char)8254;
            _htmlCharacterEntities["Omega"] = (char)937;
            _htmlCharacterEntities["omega"] = (char)969;
            _htmlCharacterEntities["Omicron"] = (char)927;
            _htmlCharacterEntities["omicron"] = (char)959;
            _htmlCharacterEntities["oplus"] = (char)8853;
            _htmlCharacterEntities["or"] = (char)8744;
            _htmlCharacterEntities["ordf"] = (char)170;
            _htmlCharacterEntities["ordm"] = (char)186;
            _htmlCharacterEntities["Oslash"] = (char)216;
            _htmlCharacterEntities["oslash"] = (char)248;
            _htmlCharacterEntities["Otilde"] = (char)213;
            _htmlCharacterEntities["otilde"] = (char)245;
            _htmlCharacterEntities["otimes"] = (char)8855;
            _htmlCharacterEntities["Ouml"] = (char)214;
            _htmlCharacterEntities["ouml"] = (char)246;
            _htmlCharacterEntities["para"] = (char)182;
            _htmlCharacterEntities["part"] = (char)8706;
            _htmlCharacterEntities["permil"] = (char)8240;
            _htmlCharacterEntities["perp"] = (char)8869;
            _htmlCharacterEntities["Phi"] = (char)934;
            _htmlCharacterEntities["phi"] = (char)966;
            _htmlCharacterEntities["pi"] = (char)960;
            _htmlCharacterEntities["piv"] = (char)982;
            _htmlCharacterEntities["plusmn"] = (char)177;
            _htmlCharacterEntities["pound"] = (char)163;
            _htmlCharacterEntities["prime"] = (char)8242;
            _htmlCharacterEntities["Prime"] = (char)8243;
            _htmlCharacterEntities["prod"] = (char)8719;
            _htmlCharacterEntities["prop"] = (char)8733;
            _htmlCharacterEntities["Psi"] = (char)936;
            _htmlCharacterEntities["psi"] = (char)968;
            _htmlCharacterEntities["quot"] = (char)34;
            _htmlCharacterEntities["radic"] = (char)8730;
            _htmlCharacterEntities["rang"] = (char)9002;
            _htmlCharacterEntities["raquo"] = (char)187;
            _htmlCharacterEntities["rarr"] = (char)8594;
            _htmlCharacterEntities["rArr"] = (char)8658;
            _htmlCharacterEntities["rceil"] = (char)8969;
            _htmlCharacterEntities["rdquo"] = (char)8221;
            _htmlCharacterEntities["real"] = (char)8476;
            _htmlCharacterEntities["reg"] = (char)174;
            _htmlCharacterEntities["rfloor"] = (char)8971;
            _htmlCharacterEntities["Rho"] = (char)929;
            _htmlCharacterEntities["rho"] = (char)961;
            _htmlCharacterEntities["rlm"] = (char)8207;
            _htmlCharacterEntities["rsaquo"] = (char)8250;
            _htmlCharacterEntities["rsquo"] = (char)8217;
            _htmlCharacterEntities["sbquo"] = (char)8218;
            _htmlCharacterEntities["Scaron"] = (char)352;
            _htmlCharacterEntities["scaron"] = (char)353;
            _htmlCharacterEntities["sdot"] = (char)8901;
            _htmlCharacterEntities["sect"] = (char)167;
            _htmlCharacterEntities["shy"] = (char)173;
            _htmlCharacterEntities["Sigma"] = (char)931;
            _htmlCharacterEntities["sigma"] = (char)963;
            _htmlCharacterEntities["sigmaf"] = (char)962;
            _htmlCharacterEntities["sim"] = (char)8764;
            _htmlCharacterEntities["spades"] = (char)9824;
            _htmlCharacterEntities["sub"] = (char)8834;
            _htmlCharacterEntities["sube"] = (char)8838;
            _htmlCharacterEntities["sum"] = (char)8721;
            _htmlCharacterEntities["sup"] = (char)8835;
            _htmlCharacterEntities["sup1"] = (char)185;
            _htmlCharacterEntities["sup2"] = (char)178;
            _htmlCharacterEntities["sup3"] = (char)179;
            _htmlCharacterEntities["supe"] = (char)8839;
            _htmlCharacterEntities["szlig"] = (char)223;
            _htmlCharacterEntities["Tau"] = (char)932;
            _htmlCharacterEntities["tau"] = (char)964;
            _htmlCharacterEntities["there4"] = (char)8756;
            _htmlCharacterEntities["Theta"] = (char)920;
            _htmlCharacterEntities["theta"] = (char)952;
            _htmlCharacterEntities["thetasym"] = (char)977;
            _htmlCharacterEntities["thinsp"] = (char)8201;
            _htmlCharacterEntities["THORN"] = (char)222;
            _htmlCharacterEntities["thorn"] = (char)254;
            _htmlCharacterEntities["tilde"] = (char)732;
            _htmlCharacterEntities["times"] = (char)215;
            _htmlCharacterEntities["trade"] = (char)8482;
            _htmlCharacterEntities["Uacute"] = (char)218;
            _htmlCharacterEntities["uacute"] = (char)250;
            _htmlCharacterEntities["uarr"] = (char)8593;
            _htmlCharacterEntities["uArr"] = (char)8657;
            _htmlCharacterEntities["Ucirc"] = (char)219;
            _htmlCharacterEntities["ucirc"] = (char)251;
            _htmlCharacterEntities["Ugrave"] = (char)217;
            _htmlCharacterEntities["ugrave"] = (char)249;
            _htmlCharacterEntities["uml"] = (char)168;
            _htmlCharacterEntities["upsih"] = (char)978;
            _htmlCharacterEntities["Upsilon"] = (char)933;
            _htmlCharacterEntities["upsilon"] = (char)965;
            _htmlCharacterEntities["Uuml"] = (char)220;
            _htmlCharacterEntities["uuml"] = (char)252;
            _htmlCharacterEntities["weierp"] = (char)8472;
            _htmlCharacterEntities["Xi"] = (char)926;
            _htmlCharacterEntities["xi"] = (char)958;
            _htmlCharacterEntities["Yacute"] = (char)221;
            _htmlCharacterEntities["yacute"] = (char)253;
            _htmlCharacterEntities["yen"] = (char)165;
            _htmlCharacterEntities["Yuml"] = (char)376;
            _htmlCharacterEntities["yuml"] = (char)255;
            _htmlCharacterEntities["Zeta"] = (char)918;
            _htmlCharacterEntities["zeta"] = (char)950;
            _htmlCharacterEntities["zwj"] = (char)8205;
            _htmlCharacterEntities["zwnj"] = (char)8204;
        }

        #endregion Private Methods

        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------

        #region Private Fields

        // html element names
        // this is an array list now, but we may want to make it a hashtable later for better performance
        private static ArrayList _htmlInlineElements;

        private static ArrayList _htmlBlockElements;

        private static ArrayList _htmlOtherOpenableElements;

        // list of html empty element names
        private static ArrayList _htmlEmptyElements;

        // names of html elements for which closing tags are optional, and close when the outer nested element closes
        private static ArrayList _htmlElementsClosingOnParentElementEnd;

        // names of elements that close certain optional closing tag elements when they start

        // names of elements closing the colgroup element
        private static ArrayList _htmlElementsClosingColgroup;

        // names of elements closing the dd element
        private static ArrayList _htmlElementsClosingDD;

        // names of elements closing the dt element
        private static ArrayList _htmlElementsClosingDT;

        // names of elements closing the li element
        private static ArrayList _htmlElementsClosingLI;

        // names of elements closing the tbody element
        private static ArrayList _htmlElementsClosingTbody;

        // names of elements closing the td element
        private static ArrayList _htmlElementsClosingTD;

        // names of elements closing the tfoot element
        private static ArrayList _htmlElementsClosingTfoot;

        // names of elements closing the thead element
        private static ArrayList _htmlElementsClosingThead;

        // names of elements closing the th element
        private static ArrayList _htmlElementsClosingTH;

        // names of elements closing the tr element
        private static ArrayList _htmlElementsClosingTR;

        // html character entities hashtable
        private static Hashtable _htmlCharacterEntities;

        #endregion Private Fields
    }

    /// <summary>
    /// HtmlParser class accepts a string of possibly badly formed Html, parses it and returns a string
    /// of well-formed Html that is as close to the original string in content as possible
    /// </summary>
    internal class HtmlParser {
        // ---------------------------------------------------------------------
        //
        // Constructors
        //
        // ---------------------------------------------------------------------

        #region Constructors

        /// <summary>
        /// Constructor. Initializes the _htmlLexicalAnalayzer element with the given input string
        /// </summary>
        /// <param name="inputString">
        /// string to parsed into well-formed Html
        /// </param>
        private HtmlParser(string inputString) {
            // Create an output xml document
            _document = new XmlDocument();

            // initialize open tag stack
            _openedElements = new Stack<XmlElement>();

            _pendingInlineElements = new Stack<XmlElement>();

            // initialize lexical analyzer
            _htmlLexicalAnalyzer = new HtmlLexicalAnalyzer(inputString);

            // get first token from input, expecting text
            _htmlLexicalAnalyzer.GetNextContentToken();
        }

        #endregion Constructors

        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// Instantiates an HtmlParser element and calls the parsing function on the given input string
        /// </summary>
        /// <param name="htmlString">
        /// Input string of pssibly badly-formed Html to be parsed into well-formed Html
        /// </param>
        /// <returns>
        /// XmlElement rep
        /// </returns>
        internal static XmlElement ParseHtml(string htmlString) {
            HtmlParser htmlParser = new HtmlParser(htmlString);

            XmlElement htmlRootElement = htmlParser.ParseHtmlContent();

            return htmlRootElement;
        }

        // .....................................................................
        //
        // Html Header on Clipboard
        //
        // .....................................................................

        // Html header structure.
        //      Version:1.0
        //      StartHTML:000000000
        //      EndHTML:000000000
        //      StartFragment:000000000
        //      EndFragment:000000000
        //      StartSelection:000000000
        //      EndSelection:000000000
        internal const string HtmlHeader = "Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\nStartSelection:{4:D10}\r\nEndSelection:{5:D10}\r\n";
        internal const string HtmlStartFragmentComment = "<!--StartFragment-->";
        internal const string HtmlEndFragmentComment = "<!--EndFragment-->";

        /// <summary>
        /// Extracts Html string from clipboard data by parsing header information in htmlDataString
        /// </summary>
        /// <param name="htmlDataString">
        /// String representing Html clipboard data. This includes Html header
        /// </param>
        /// <returns>
        /// String containing only the Html data part of htmlDataString, without header
        /// </returns>
        internal static string ExtractHtmlFromClipboardData(string htmlDataString) {
            int startHtmlIndex = htmlDataString.IndexOf("StartHTML:");
            if(startHtmlIndex < 0) {
                return "ERROR: Urecognized html header";
            }
            // TODO: We assume that indices represented by strictly 10 zeros ("0123456789".Length),
            // which could be wrong assumption. We need to implement more flrxible parsing here
            startHtmlIndex = Int32.Parse(htmlDataString.Substring(startHtmlIndex + "StartHTML:".Length, "0123456789".Length));
            if(startHtmlIndex < 0 || startHtmlIndex > htmlDataString.Length) {
                return "ERROR: Urecognized html header";
            }

            int endHtmlIndex = htmlDataString.IndexOf("EndHTML:");
            if(endHtmlIndex < 0) {
                return "ERROR: Urecognized html header";
            }
            // TODO: We assume that indices represented by strictly 10 zeros ("0123456789".Length),
            // which could be wrong assumption. We need to implement more flrxible parsing here
            endHtmlIndex = Int32.Parse(htmlDataString.Substring(endHtmlIndex + "EndHTML:".Length, "0123456789".Length));
            if(endHtmlIndex > htmlDataString.Length) {
                endHtmlIndex = htmlDataString.Length;
            }

            return htmlDataString.Substring(startHtmlIndex, endHtmlIndex - startHtmlIndex);
        }

        /// <summary>
        /// Adds Xhtml header information to Html data string so that it can be placed on clipboard
        /// </summary>
        /// <param name="htmlString">
        /// Html string to be placed on clipboard with appropriate header
        /// </param>
        /// <returns>
        /// String wrapping htmlString with appropriate Html header
        /// </returns>
        internal static string AddHtmlClipboardHeader(string htmlString) {
            StringBuilder stringBuilder = new StringBuilder();

            // each of 6 numbers is represented by "{0:D10}" in the format string
            // must actually occupy 10 digit positions ("0123456789")
            int startHTML = HtmlHeader.Length + 6 * ("0123456789".Length - "{0:D10}".Length);
            int endHTML = startHTML + htmlString.Length;
            int startFragment = htmlString.IndexOf(HtmlStartFragmentComment, 0);
            if(startFragment >= 0) {
                startFragment = startHTML + startFragment + HtmlStartFragmentComment.Length;
            } else {
                startFragment = startHTML;
            }
            int endFragment = htmlString.IndexOf(HtmlEndFragmentComment, 0);
            if(endFragment >= 0) {
                endFragment = startHTML + endFragment;
            } else {
                endFragment = endHTML;
            }

            // Create HTML clipboard header string
            stringBuilder.AppendFormat(HtmlHeader, startHTML, endHTML, startFragment, endFragment, startFragment, endFragment);

            // Append HTML body.
            stringBuilder.Append(htmlString);

            return stringBuilder.ToString();
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Private methods
        //
        // ---------------------------------------------------------------------

        #region Private Methods

        private void InvariantAssert(bool condition, string message) {
            if(!condition) {
                throw new Exception("Assertion error: " + message);
            }
        }

        /// <summary>
        /// Parses the stream of html tokens starting
        /// from the name of top-level element.
        /// Returns XmlElement representing the top-level
        /// html element
        /// </summary>
        private XmlElement ParseHtmlContent() {
            // Create artificial root elelemt to be able to group multiple top-level elements
            // We create "html" element which may be a duplicate of real HTML element, which is ok, as HtmlConverter will swallow it painlessly..
            XmlElement htmlRootElement = _document.CreateElement("html", XhtmlNamespace);
            OpenStructuringElement(htmlRootElement);

            while(_htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF) {
                if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.OpeningTagStart) {
                    _htmlLexicalAnalyzer.GetNextTagToken();
                    if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name) {
                        string htmlElementName = _htmlLexicalAnalyzer.NextToken.ToLower();
                        _htmlLexicalAnalyzer.GetNextTagToken();

                        // Create an element
                        XmlElement htmlElement = _document.CreateElement(htmlElementName, XhtmlNamespace);

                        // Parse element attributes
                        ParseAttributes(htmlElement);

                        if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.EmptyTagEnd || HtmlSchema.IsEmptyElement(htmlElementName)) {
                            // It is an element without content (because of explicit slash or based on implicit knowledge aboout html)
                            AddEmptyElement(htmlElement);
                        } else if(HtmlSchema.IsInlineElement(htmlElementName)) {
                            // Elements known as formatting are pushed to some special
                            // pending stack, which allows them to be transferred
                            // over block tags - by doing this we convert
                            // overlapping tags into normal heirarchical element structure.
                            OpenInlineElement(htmlElement);
                        } else if(HtmlSchema.IsBlockElement(htmlElementName) || HtmlSchema.IsKnownOpenableElement(htmlElementName)) {
                            // This includes no-scope elements
                            OpenStructuringElement(htmlElement);
                        } else {
                            // Do nothing. Skip the whole opening tag.
                            // Ignoring all unknown elements on their start tags.
                            // Thus we will ignore them on closinng tag as well.
                            // Anyway we don't know what to do withthem on conversion to Xaml.
                        }
                    } else {
                        // Note that the token following opening angle bracket must be a name - lexical analyzer must guarantee that.
                        // Otherwise - we skip the angle bracket and continue parsing the content as if it is just text.
                        //  Add the following asserion here, right? or output "<" as a text run instead?:
                        // InvariantAssert(false, "Angle bracket without a following name is not expected");
                    }
                } else if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.ClosingTagStart) {
                    _htmlLexicalAnalyzer.GetNextTagToken();
                    if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name) {
                        string htmlElementName = _htmlLexicalAnalyzer.NextToken.ToLower();

                        // Skip the name token. Assume that the following token is end of tag,
                        // but do not check this. If it is not true, we simply ignore one token
                        // - this is our recovery from bad xml in this case.
                        _htmlLexicalAnalyzer.GetNextTagToken();

                        CloseElement(htmlElementName);
                    }
                } else if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Text) {
                    AddTextContent(_htmlLexicalAnalyzer.NextToken);
                } else if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Comment) {
                    AddComment(_htmlLexicalAnalyzer.NextToken);
                }

                _htmlLexicalAnalyzer.GetNextContentToken();
            }

            // Get rid of the artificial root element
            if(htmlRootElement.FirstChild is XmlElement &&
                htmlRootElement.FirstChild == htmlRootElement.LastChild &&
                htmlRootElement.FirstChild.LocalName.ToLower() == "html") {
                htmlRootElement = (XmlElement)htmlRootElement.FirstChild;
            }

            return htmlRootElement;
        }

        private XmlElement CreateElementCopy(XmlElement htmlElement) {
            XmlElement htmlElementCopy = _document.CreateElement(htmlElement.LocalName, XhtmlNamespace);
            for(int i = 0; i < htmlElement.Attributes.Count; i++) {
                XmlAttribute attribute = htmlElement.Attributes[i];
                htmlElementCopy.SetAttribute(attribute.Name, attribute.Value);
            }
            return htmlElementCopy;
        }

        private void AddEmptyElement(XmlElement htmlEmptyElement) {
            InvariantAssert(_openedElements.Count > 0, "AddEmptyElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
            XmlElement htmlParent = _openedElements.Peek();
            htmlParent.AppendChild(htmlEmptyElement);
        }

        private void OpenInlineElement(XmlElement htmlInlineElement) {
            _pendingInlineElements.Push(htmlInlineElement);
        }

        // Opens structurig element such as Div or Table etc.
        private void OpenStructuringElement(XmlElement htmlElement) {
            // Close all pending inline elements
            // All block elements are considered as delimiters for inline elements
            // which forces all inline elements to be closed and re-opened in the following
            // structural element (if any).
            // By doing that we guarantee that all inline elements appear only within most nested blocks
            if(HtmlSchema.IsBlockElement(htmlElement.LocalName)) {
                while(_openedElements.Count > 0 && HtmlSchema.IsInlineElement(_openedElements.Peek().LocalName)) {
                    XmlElement htmlInlineElement = _openedElements.Pop();
                    InvariantAssert(_openedElements.Count > 0, "OpenStructuringElement: stack of opened elements cannot become empty here");

                    _pendingInlineElements.Push(CreateElementCopy(htmlInlineElement));
                }
            }

            // Add this block element to its parent
            if(_openedElements.Count > 0) {
                XmlElement htmlParent = _openedElements.Peek();

                // Check some known block elements for auto-closing (LI and P)
                if(HtmlSchema.ClosesOnNextElementStart(htmlParent.LocalName, htmlElement.LocalName)) {
                    _openedElements.Pop();
                    htmlParent = _openedElements.Count > 0 ? _openedElements.Peek() : null;
                }

                if(htmlParent != null) {
                    // NOTE:
                    // Actually we never expect null - it would mean two top-level P or LI (without a parent).
                    // In such weird case we will loose all paragraphs except the first one...
                    htmlParent.AppendChild(htmlElement);
                }
            }

            // Push it onto a stack
            _openedElements.Push(htmlElement);
        }

        private bool IsElementOpened(string htmlElementName) {
            foreach(XmlElement openedElement in _openedElements) {
                if(openedElement.LocalName == htmlElementName) {
                    return true;
                }
            }
            return false;
        }

        private void CloseElement(string htmlElementName) {
            // Check if the element is opened and already added to the parent
            InvariantAssert(_openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            // Check if the element is opened and still waiting to be added to the parent
            if(_pendingInlineElements.Count > 0 && _pendingInlineElements.Peek().LocalName == htmlElementName) {
                // Closing an empty inline element.
                // Note that HtmlConverter will skip empty inlines, but for completeness we keep them here on parser level.
                XmlElement htmlInlineElement = _pendingInlineElements.Pop();
                InvariantAssert(_openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
                XmlElement htmlParent = _openedElements.Peek();
                htmlParent.AppendChild(htmlInlineElement);
                return;
            } else if(IsElementOpened(htmlElementName)) {
                while(_openedElements.Count > 1) // we never pop the last element - the artificial root
                {
                    // Close all unbalanced elements.
                    XmlElement htmlOpenedElement = _openedElements.Pop();

                    if(htmlOpenedElement.LocalName == htmlElementName) {
                        return;
                    }

                    if(HtmlSchema.IsInlineElement(htmlOpenedElement.LocalName)) {
                        // Unbalances Inlines will be transfered to the next element content
                        _pendingInlineElements.Push(CreateElementCopy(htmlOpenedElement));
                    }
                }
            }

            // If element was not opened, we simply ignore the unbalanced closing tag
            return;
        }

        private void AddTextContent(string textContent) {
            OpenPendingInlineElements();

            InvariantAssert(_openedElements.Count > 0, "AddTextContent: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            XmlElement htmlParent = _openedElements.Peek();
            XmlText textNode = _document.CreateTextNode(textContent);
            htmlParent.AppendChild(textNode);
        }

        private void AddComment(string comment) {
            OpenPendingInlineElements();

            InvariantAssert(_openedElements.Count > 0, "AddComment: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            XmlElement htmlParent = _openedElements.Peek();
            XmlComment xmlComment = _document.CreateComment(comment);
            htmlParent.AppendChild(xmlComment);
        }

        // Moves all inline elements pending for opening to actual document
        // and adds them to current open stack.
        private void OpenPendingInlineElements() {
            if(_pendingInlineElements.Count > 0) {
                XmlElement htmlInlineElement = _pendingInlineElements.Pop();

                OpenPendingInlineElements();

                InvariantAssert(_openedElements.Count > 0, "OpenPendingInlineElements: Stack of opened elements cannot be empty, as we have at least one artificial root element");

                XmlElement htmlParent = _openedElements.Peek();
                htmlParent.AppendChild(htmlInlineElement);
                _openedElements.Push(htmlInlineElement);
            }
        }

        private void ParseAttributes(XmlElement xmlElement) {
            while(_htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF && //
                _htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.TagEnd && //
                _htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EmptyTagEnd) {
                // read next attribute (name=value)
                if(_htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name) {
                    string attributeName = _htmlLexicalAnalyzer.NextToken;
                    _htmlLexicalAnalyzer.GetNextEqualSignToken();

                    _htmlLexicalAnalyzer.GetNextAtomToken();

                    string attributeValue = _htmlLexicalAnalyzer.NextToken;
                    xmlElement.SetAttribute(attributeName, attributeValue);
                }
                _htmlLexicalAnalyzer.GetNextTagToken();
            }
        }

        #endregion Private Methods


        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------

        #region Private Fields

        internal const string XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        private HtmlLexicalAnalyzer _htmlLexicalAnalyzer;

        // document from which all elements are created
        private XmlDocument _document;

        // stack for open elements
        Stack<XmlElement> _openedElements;
        Stack<XmlElement> _pendingInlineElements;

        #endregion Private Fields
    }

    /// <summary>
    /// HtmlToXamlConverter is a static class that takes an HTML string
    /// and converts it into XAML
    /// </summary>
    public static class HtmlToXamlConverter {
        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// Converts an html string into xaml string.
        /// </summary>
        /// <param name="htmlString">
        /// Input html which may be badly formated xml.
        /// </param>
        /// <param name="asFlowDocument">
        /// true indicates that we need a FlowDocument as a root element;
        /// false means that Section or Span elements will be used
        /// dependeing on StartFragment/EndFragment comments locations.
        /// </param>
        /// <returns>
        /// Well-formed xml representing XAML equivalent for the input html string.
        /// </returns>
        public static string ConvertHtmlToXaml(string htmlString, bool asFlowDocument) {
            // Create well-formed Xml from Html string
            XmlElement htmlElement = HtmlParser.ParseHtml(htmlString);


            ReplaceInvalidThings(htmlElement);
            // Decide what name to use as a root
            string rootElementName = asFlowDocument ? Xaml_FlowDocument : Xaml_Section;

            // Create an XmlDocument for generated xaml
            XmlDocument xamlTree = new XmlDocument();
            XmlElement xamlFlowDocumentElement = xamlTree.CreateElement(null, rootElementName, _xamlNamespace);

            // Extract style definitions from all STYLE elements in the document
            CssStylesheet stylesheet = new CssStylesheet(htmlElement);

            // Source context is a stack of all elements - ancestors of a parentElement
            List<XmlElement> sourceContext = new List<XmlElement>(10);

            // Clear fragment parent
            InlineFragmentParentElement = null;

            // convert root html element
            AddBlock(xamlFlowDocumentElement, htmlElement, new Hashtable(), stylesheet, sourceContext);

            // In case if the selected fragment is inline, extract it into a separate Span wrapper
            if(!asFlowDocument) {
                xamlFlowDocumentElement = ExtractInlineFragment(xamlFlowDocumentElement);
            }

            // Return a string representing resulting Xaml
            xamlFlowDocumentElement.SetAttribute("xml:space", "preserve");
            string xaml = xamlFlowDocumentElement.OuterXml;

            return xaml;
        }


        private static void ReplaceInvalidThings(XmlElement element) {

            foreach(XmlNode node in element) {

                if(node is XmlElement) {

                    ReplaceInvalidThings((XmlElement)node);
                }

                if(node.ParentNode != null && node.LocalName == "a" && node.ParentNode.LocalName == "a") {

                    node.ParentNode.ParentNode.ReplaceChild(node, node.ParentNode);
                    //node.ParentNode.RemoveAll();
                }

            }

        }





        /// <summary>
        /// Returns a value for an attribute by its name (ignoring casing)
        /// </summary>
        /// <param name="element">
        /// XmlElement in which we are trying to find the specified attribute
        /// </param>
        /// <param name="attributeName">
        /// String representing the attribute name to be searched for
        /// </param>
        /// <returns></returns>
        public static string GetAttribute(XmlElement element, string attributeName) {
            attributeName = attributeName.ToLower();

            for(int i = 0; i < element.Attributes.Count; i++) {
                if(element.Attributes[i].Name.ToLower() == attributeName) {
                    return element.Attributes[i].Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns string extracted from quotation marks
        /// </summary>
        /// <param name="value">
        /// String representing value enclosed in quotation marks
        /// </param>
        internal static string UnQuote(string value) {
            if(value.StartsWith("\"") && value.EndsWith("\"") || value.StartsWith("'") && value.EndsWith("'")) {
                value = value.Substring(1, value.Length - 2).Trim();
            }
            return value;
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Private Methods
        //
        // ---------------------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// Analyzes the given htmlElement expecting it to be converted
        /// into some of xaml Block elements and adds the converted block
        /// to the children collection of xamlParentElement.
        ///
        /// Analyzes the given XmlElement htmlElement, recognizes it as some HTML element
        /// and adds it as a child to a xamlParentElement.
        /// In some cases several following siblings of the given htmlElement
        /// will be consumed too (e.g. LIs encountered without wrapping UL/OL,
        /// which must be collected together and wrapped into one implicit List element).
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent xaml element, to which new converted element will be added
        /// </param>
        /// <param name="htmlElement">
        /// Source html element subject to convert to xaml.
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from an outer context.
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        /// <returns>
        /// Last processed html node. Normally it should be the same htmlElement
        /// as was passed as a paramater, but in some irregular cases
        /// it could one of its following siblings.
        /// The caller must use this node to get to next sibling from it.
        /// </returns>
        private static XmlNode AddBlock(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            if(htmlNode is XmlComment) {
                DefineInlineFragmentParent((XmlComment)htmlNode, /*xamlParentElement:*/null);
            } else if(htmlNode is XmlText) {
                htmlNode = AddImplicitParagraph(xamlParentElement, htmlNode, inheritedProperties, stylesheet, sourceContext);
            } else if(htmlNode is XmlElement) {
                // Identify element name
                XmlElement htmlElement = (XmlElement)htmlNode;

                string htmlElementName = htmlElement.LocalName; // Keep the name case-sensitive to check xml names
                string htmlElementNamespace = htmlElement.NamespaceURI;

                if(htmlElementNamespace != HtmlParser.XhtmlNamespace) {
                    // Non-html element. skip it
                    // Isn't it too agressive? What if this is just an error in html tag name?
                    // TODO: Consider skipping just a wparrer in recursing into the element tree,
                    // which may produce some garbage though coming from xml fragments.
                    return htmlElement;
                }

                // Put source element to the stack
                sourceContext.Add(htmlElement);

                // Convert the name to lowercase, because html elements are case-insensitive
                htmlElementName = htmlElementName.ToLower();

                // Switch to an appropriate kind of processing depending on html element name
                switch(htmlElementName) {
                    // Sections:
                    case "html":
                    case "body":
                    case "div":
                    case "form": // not a block according to xhtml spec
                    case "pre": // Renders text in a fixed-width font
                    case "blockquote":
                    case "caption":
                    case "center":
                    case "cite":
                        AddSection(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    // Paragraphs:
                    case "p":
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                    case "nsrtitle":
                    case "textarea":
                    case "dd": // ???
                    case "dl": // ???
                    case "dt": // ???
                    case "tt": // ???
                        AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "ol":
                    case "ul":
                    case "dir": //  treat as UL element
                    case "menu": //  treat as UL element
                        // List element conversion
                        AddList(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                    case "li":
                        // LI outside of OL/UL
                        // Collect all sibling LIs, wrap them into a List and then proceed with the element following the last of LIs
                        htmlNode = AddOrphanListItems(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "img":
                        // TODO: Add image processing
                        AddImage(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "table":
                        // hand off to table parsing function which will perform special table syntax checks
                        AddTable(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "tbody":
                    case "tfoot":
                    case "thead":
                    case "tr":
                    case "td":
                    case "th":
                        // Table stuff without table wrapper
                        // TODO: add special-case processing here for elements that should be within tables when the
                        // parent element is NOT a table. If the parent element is a table they can be processed normally.
                        // we need to compare against the parent element here, we can't just break on a switch
                        goto default; // Thus we will skip this element as unknown, but still recurse into it.

                    case "style": // We already pre-processed all style elements. Ignore it now
                    case "meta":
                    case "head":
                    case "title":
                    case "script":
                        // Ignore these elements
                        break;

                    default:
                        // Wrap a sequence of inlines into an implicit paragraph
                        htmlNode = AddImplicitParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                }

                // Remove the element from the stack
                Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlElement);
                sourceContext.RemoveAt(sourceContext.Count - 1);
            }

            // Return last processed node
            return htmlNode;
        }

        // .............................................................
        //
        // Line Breaks
        //
        // .............................................................

        private static void AddBreak(XmlElement xamlParentElement, string htmlElementName) {
            // Create new xaml element corresponding to this html element
            XmlElement xamlLineBreak = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_LineBreak, _xamlNamespace);
            xamlParentElement.AppendChild(xamlLineBreak);
            if(htmlElementName == "hr") {
                XmlText xamlHorizontalLine = xamlParentElement.OwnerDocument.CreateTextNode("----------------------");
                xamlParentElement.AppendChild(xamlHorizontalLine);
                xamlLineBreak = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_LineBreak, _xamlNamespace);
                xamlParentElement.AppendChild(xamlLineBreak);
            }
        }

        // .............................................................
        //
        // Text Flow Elements
        //
        // .............................................................

        /// <summary>
        /// Generates Section or Paragraph element from DIV depending whether it contains any block elements or not
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlElement">
        /// XmlElement representing Html element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        /// true indicates that a content added by this call contains at least one block element
        /// </param>
        private static void AddSection(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Analyze the content of htmlElement to decide what xaml element to choose - Section or Paragraph.
            // If this Div has at least one block child then we need to use Section, otherwise use Paragraph
            bool htmlElementContainsBlocks = false;
            for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                if(htmlChildNode is XmlElement) {
                    string htmlChildName = ((XmlElement)htmlChildNode).LocalName.ToLower();
                    if(HtmlSchema.IsBlockElement(htmlChildName)) {
                        htmlElementContainsBlocks = true;
                        break;
                    }
                }
            }

            if(!htmlElementContainsBlocks) {
                // The Div does not contain any block elements, so we can treat it as a Paragraph
                AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
            } else {
                // The Div has some nested blocks, so we treat it as a Section

                // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
                Hashtable localProperties;
                Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

                // Create a XAML element corresponding to this html element
                XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_Section, _xamlNamespace);
                ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true);

                // Decide whether we can unwrap this element as not having any formatting significance.
                if(!xamlElement.HasAttributes) {
                    // This elements is a group of block elements whitout any additional formatting.
                    // We can add blocks directly to xamlParentElement and avoid
                    // creating unnecessary Sections nesting.
                    xamlElement = xamlParentElement;
                }

                // Recurse into element subtree
                for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null) {
                    htmlChildNode = AddBlock(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
                }

                // Add the new element to the parent.
                if(xamlElement != xamlParentElement) {
                    xamlParentElement.AppendChild(xamlElement);
                }
            }
        }

        /// <summary>
        /// Generates Paragraph element from P, H1-H7, Center etc.
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlElement">
        /// XmlElement representing Html element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        /// true indicates that a content added by this call contains at least one block element
        /// </param>
        private static void AddParagraph(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // Create a XAML element corresponding to this html element
            XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_Paragraph, _xamlNamespace);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true);

            // Recurse into element subtree
            for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {


                AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add the new element to the parent.
            xamlParentElement.AppendChild(xamlElement);
        }

        /// <summary>
        /// Creates a Paragraph element and adds all nodes starting from htmlNode
        /// converted to appropriate Inlines.
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlNode">
        /// XmlNode starting a collection of implicitly wrapped inlines.
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        /// true indicates that a content added by this call contains at least one block element
        /// </param>
        /// <returns>
        /// The last htmlNode added to the implicit paragraph
        /// </returns>
        private static XmlNode AddImplicitParagraph(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Collect all non-block elements and wrap them into implicit Paragraph
            XmlElement xamlParagraph = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_Paragraph, _xamlNamespace);
            XmlNode lastNodeProcessed = null;
            while(htmlNode != null) {
                if(htmlNode is XmlComment) {
                    DefineInlineFragmentParent((XmlComment)htmlNode, /*xamlParentElement:*/null);
                } else if(htmlNode is XmlText) {
                    if(htmlNode.Value.Trim().Length > 0) {
                        AddTextRun(xamlParagraph, htmlNode.Value);
                    }
                } else if(htmlNode is XmlElement) {
                    string htmlChildName = ((XmlElement)htmlNode).LocalName.ToLower();
                    if(HtmlSchema.IsBlockElement(htmlChildName)) {
                        // The sequence of non-blocked inlines ended. Stop implicit loop here.
                        break;
                    } else {
                        AddInline(xamlParagraph, (XmlElement)htmlNode, inheritedProperties, stylesheet, sourceContext);
                    }
                }

                // Store last processed node to return it at the end
                lastNodeProcessed = htmlNode;
                htmlNode = htmlNode.NextSibling;
            }

            // Add the Paragraph to the parent
            // If only whitespaces and commens have been encountered,
            // then we have nothing to add in implicit paragraph; forget it.
            if(xamlParagraph.FirstChild != null) {
                xamlParentElement.AppendChild(xamlParagraph);
            }

            // Need to return last processed node
            return lastNodeProcessed;
        }

        // .............................................................
        //
        // Inline Elements
        //
        // .............................................................

        private static void AddInline(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            


            if(htmlNode is XmlComment) {
                DefineInlineFragmentParent((XmlComment)htmlNode, xamlParentElement);
            } else if(htmlNode is XmlText) {
                AddTextRun(xamlParentElement, htmlNode.Value);
            } else if(htmlNode is XmlElement) {
                XmlElement htmlElement = (XmlElement)htmlNode;

                // Check whether this is an html element
                if(htmlElement.NamespaceURI != HtmlParser.XhtmlNamespace) {
                    return; // Skip non-html elements
                }

                // Identify element name
                string htmlElementName = htmlElement.LocalName.ToLower();

                // Put source element to the stack
                sourceContext.Add(htmlElement);

                switch(htmlElementName) {
                    case "a":
                        AddHyperlink(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                    case "img":
                        AddImage(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                    case "br":
                    case "hr":
                        AddBreak(xamlParentElement, htmlElementName);
                        break;
                    default:
                        if(HtmlSchema.IsInlineElement(htmlElementName) || HtmlSchema.IsBlockElement(htmlElementName)) {
                            // Note: actually we do not expect block elements here,
                            // but if it happens to be here, we will treat it as a Span.

                            AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        }
                        break;
                }
                // Ignore all other elements non-(block/inline/image)

                // Remove the element from the stack
                Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlElement);
                sourceContext.RemoveAt(sourceContext.Count - 1);
            }
        }

        private static void AddSpanOrRun(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Decide what XAML element to use for this inline element.
            // Check whether it contains any nested inlines
            bool elementHasChildren = false;
            for(XmlNode htmlNode = htmlElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling) {
                if(htmlNode is XmlElement) {
                    string htmlChildName = ((XmlElement)htmlNode).LocalName.ToLower();
                    if(HtmlSchema.IsInlineElement(htmlChildName) || HtmlSchema.IsBlockElement(htmlChildName) ||
                        htmlChildName == "img" || htmlChildName == "br" || htmlChildName == "hr") {
                        elementHasChildren = true;
                        break;
                    }
                }
            }

            string xamlElementName = elementHasChildren ? HtmlToXamlConverter.Xaml_Span : HtmlToXamlConverter.Xaml_Run;

            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // Create a XAML element corresponding to this html element
            XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/xamlElementName, _xamlNamespace);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false);

            // Recurse into element subtree
            for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add the new element to the parent.
            xamlParentElement.AppendChild(xamlElement);
        }

        // Adds a text run to a xaml tree
        private static void AddTextRun(XmlElement xamlElement, string textData) {
            // Remove control characters
            for(int i = 0; i < textData.Length; i++) {
                if(Char.IsControl(textData[i])) {
                    textData = textData.Remove(i--, 1);  // decrement i to compensate for character removal
                }
            }

            // Replace No-Breaks by spaces (160 is a code of &nbsp; entity in html)
            //  This is a work around since WPF/XAML does not support &nbsp.
            textData = textData.Replace((char)160, ' ');

            if(textData.Length > 0) {
                xamlElement.AppendChild(xamlElement.OwnerDocument.CreateTextNode(textData));
            }
        }

        private static void AddHyperlink(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Convert href attribute into NavigateUri and TargetName
            string href = GetAttribute(htmlElement, "href");
            if(href == null) {
                // When href attribute is missing - ignore the hyperlink
                AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
            } else {
                // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
                Hashtable localProperties;
                Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

                // Create a XAML element corresponding to this html element
                XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_Hyperlink, _xamlNamespace);
                ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false);

                string[] hrefParts = href.Split(new char[] { '#' });
                if(hrefParts.Length > 0 && hrefParts[0].Trim().Length > 0) {
                    xamlElement.SetAttribute(HtmlToXamlConverter.Xaml_Hyperlink_NavigateUri, hrefParts[0].Trim());
                }
                if(hrefParts.Length == 2 && hrefParts[1].Trim().Length > 0) {
                    xamlElement.SetAttribute(HtmlToXamlConverter.Xaml_Hyperlink_TargetName, hrefParts[1].Trim());
                }

                // Recurse into element subtree
                for(XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                    AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
                }
                

                // Add the new element to the parent.
                xamlParentElement.AppendChild(xamlElement);
            }
        }

        // Stores a parent xaml element for the case when selected fragment is inline.
        private static XmlElement InlineFragmentParentElement;

        // Called when html comment is encountered to store a parent element
        // for the case when the fragment is inline - to extract it to a separate
        // Span wrapper after the conversion.
        private static void DefineInlineFragmentParent(XmlComment htmlComment, XmlElement xamlParentElement) {
            if(htmlComment.Value == "StartFragment") {
                InlineFragmentParentElement = xamlParentElement;
            } else if(htmlComment.Value == "EndFragment") {
                if(InlineFragmentParentElement == null && xamlParentElement != null) {
                    // Normally this cannot happen if comments produced by correct copying code
                    // in Word or IE, but when it is produced manually then fragment boundary
                    // markers can be inconsistent. In this case StartFragment takes precedence,
                    // but if it is not set, then we get the value from EndFragment marker.
                    InlineFragmentParentElement = xamlParentElement;
                }
            }
        }

        // Extracts a content of an element stored as InlineFragmentParentElement
        // into a separate Span wrapper.
        // Note: when selected content does not cross paragraph boundaries,
        // the fragment is marked within
        private static XmlElement ExtractInlineFragment(XmlElement xamlFlowDocumentElement) {
            if(InlineFragmentParentElement != null) {
                if(InlineFragmentParentElement.LocalName == HtmlToXamlConverter.Xaml_Span) {
                    xamlFlowDocumentElement = InlineFragmentParentElement;
                } else {
                    xamlFlowDocumentElement = xamlFlowDocumentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.Xaml_Span, _xamlNamespace);
                    while(InlineFragmentParentElement.FirstChild != null) {
                        XmlNode copyNode = InlineFragmentParentElement.FirstChild;
                        InlineFragmentParentElement.RemoveChild(copyNode);
                        xamlFlowDocumentElement.AppendChild(copyNode);
                    }
                }
            }

            return xamlFlowDocumentElement;
        }

        // .............................................................
        //
        // Images
        //
        // .............................................................

        private static void AddImage(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            //  Implement images
        }

        // .............................................................
        //
        // Lists
        //
        // .............................................................

        /// <summary>
        /// Converts Html ul or ol element into Xaml list element. During conversion if the ul/ol element has any children
        /// that are not li elements, they are ignored and not added to the list element
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing Xaml parent to which the converted element should be added
        /// </param>
        /// <param name="htmlListElement">
        /// XmlElement representing Html ul/ol element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        private static void AddList(XmlElement xamlParentElement, XmlElement htmlListElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            string htmlListElementName = htmlListElement.LocalName.ToLower();

            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlListElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // Create Xaml List element
            XmlElement xamlListElement = xamlParentElement.OwnerDocument.CreateElement(null, Xaml_List, _xamlNamespace);

            // Set default list markers
            if(htmlListElementName == "ol") {
                // Ordered list
                xamlListElement.SetAttribute(HtmlToXamlConverter.Xaml_List_MarkerStyle, Xaml_List_MarkerStyle_Decimal);
            } else {
                // Unordered list - all elements other than OL treated as unordered lists
                xamlListElement.SetAttribute(HtmlToXamlConverter.Xaml_List_MarkerStyle, Xaml_List_MarkerStyle_Disc);
            }

            // Apply local properties to list to set marker attribute if specified
            // TODO: Should we have separate list attribute processing function?
            ApplyLocalProperties(xamlListElement, localProperties, /*isBlock:*/true);

            // Recurse into list subtree
            for(XmlNode htmlChildNode = htmlListElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                if(htmlChildNode is XmlElement && htmlChildNode.LocalName.ToLower() == "li") {
                    sourceContext.Add((XmlElement)htmlChildNode);
                    AddListItem(xamlListElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                    Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode);
                    sourceContext.RemoveAt(sourceContext.Count - 1);
                } else {
                    // Not an li element. Add it to previous ListBoxItem
                    //  We need to append the content to the end
                    // of a previous list item.
                }
            }

            // Add the List element to xaml tree - if it is not empty
            if(xamlListElement.HasChildNodes) {
                xamlParentElement.AppendChild(xamlListElement);
            }
        }

        /// <summary>
        /// If li items are found without a parent ul/ol element in Html string, creates xamlListElement as their parent and adds
        /// them to it. If the previously added node to the same xamlParentElement was a List, adds the elements to that list.
        /// Otherwise, we create a new xamlListElement and add them to it. Elements are added as long as li elements appear sequentially.
        /// The first non-li or text node stops the addition.
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent element for the list
        /// </param>
        /// <param name="htmlLIElement">
        /// Start Html li element without parent list
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <returns>
        /// XmlNode representing the first non-li node in the input after one or more li's have been processed.
        /// </returns>
        private static XmlElement AddOrphanListItems(XmlElement xamlParentElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            Debug.Assert(htmlLIElement.LocalName.ToLower() == "li");

            XmlElement lastProcessedListItemElement = null;

            // Find out the last element attached to the xamlParentElement, which is the previous sibling of this node
            XmlNode xamlListItemElementPreviousSibling = xamlParentElement.LastChild;
            XmlElement xamlListElement;
            if(xamlListItemElementPreviousSibling != null && xamlListItemElementPreviousSibling.LocalName == Xaml_List) {
                // Previously added Xaml element was a list. We will add the new li to it
                xamlListElement = (XmlElement)xamlListItemElementPreviousSibling;
            } else {
                // No list element near. Create our own.
                xamlListElement = xamlParentElement.OwnerDocument.CreateElement(null, Xaml_List, _xamlNamespace);
                xamlParentElement.AppendChild(xamlListElement);
            }

            XmlNode htmlChildNode = htmlLIElement;
            string htmlChildNodeName = htmlChildNode == null ? null : htmlChildNode.LocalName.ToLower();

            //  Current element properties missed here.
            //currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out localProperties, stylesheet);

            // Add li elements to the parent xamlListElement we created as long as they appear sequentially
            // Use properties inherited from xamlParentElement for context
            while(htmlChildNode != null && htmlChildNodeName == "li") {
                AddListItem(xamlListElement, (XmlElement)htmlChildNode, inheritedProperties, stylesheet, sourceContext);
                lastProcessedListItemElement = (XmlElement)htmlChildNode;
                htmlChildNode = htmlChildNode.NextSibling;
                htmlChildNodeName = htmlChildNode == null ? null : htmlChildNode.LocalName.ToLower();
            }

            return lastProcessedListItemElement;
        }

        /// <summary>
        /// Converts htmlLIElement into Xaml ListItem element, and appends it to the parent xamlListElement
        /// </summary>
        /// <param name="xamlListElement">
        /// XmlElement representing Xaml List element to which the converted td/th should be added
        /// </param>
        /// <param name="htmlLIElement">
        /// XmlElement representing Html li element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        private static void AddListItem(XmlElement xamlListElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Parameter validation
            Debug.Assert(xamlListElement != null);
            Debug.Assert(xamlListElement.LocalName == Xaml_List);
            Debug.Assert(htmlLIElement != null);
            Debug.Assert(htmlLIElement.LocalName.ToLower() == "li");
            Debug.Assert(inheritedProperties != null);

            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            XmlElement xamlListItemElement = xamlListElement.OwnerDocument.CreateElement(null, Xaml_ListItem, _xamlNamespace);

            // TODO: process local properties for li element

            // Process children of the ListItem
            for(XmlNode htmlChildNode = htmlLIElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null) {
                htmlChildNode = AddBlock(xamlListItemElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add resulting ListBoxItem to a xaml parent
            xamlListElement.AppendChild(xamlListItemElement);
        }

        // .............................................................
        //
        // Tables
        //
        // .............................................................

        /// <summary>
        /// Converts htmlTableElement to a Xaml Table element. Adds tbody elements if they are missing so
        /// that a resulting Xaml Table element is properly formed.
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent xaml element to which a converted table must be added.
        /// </param>
        /// <param name="htmlTableElement">
        /// XmlElement reprsenting the Html table element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Hashtable representing properties inherited from parent context.
        /// </param>
        private static void AddTable(XmlElement xamlParentElement, XmlElement htmlTableElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Parameter validation
            Debug.Assert(htmlTableElement.LocalName.ToLower() == "table");
            Debug.Assert(xamlParentElement != null);
            Debug.Assert(inheritedProperties != null);

            // Create current properties to be used by children as inherited properties, set local properties
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlTableElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // TODO: process localProperties for tables to override defaults, decide cell spacing defaults

            // Check if the table contains only one cell - we want to take only its content
            XmlElement singleCell = GetCellFromSingleCellTable(htmlTableElement);

            if(singleCell != null) {
                //  Need to push skipped table elements onto sourceContext
                sourceContext.Add(singleCell);

                // Add the cell's content directly to parent
                for(XmlNode htmlChildNode = singleCell.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null) {
                    htmlChildNode = AddBlock(xamlParentElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
                }

                Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == singleCell);
                sourceContext.RemoveAt(sourceContext.Count - 1);
            } else {
                // Create xamlTableElement
                XmlElement xamlTableElement = xamlParentElement.OwnerDocument.CreateElement(null, Xaml_Table, _xamlNamespace);

                // Analyze table structure for column widths and rowspan attributes
                ArrayList columnStarts = AnalyzeTableStructure(htmlTableElement, stylesheet);

                // Process COLGROUP & COL elements
                AddColumnInformation(htmlTableElement, xamlTableElement, columnStarts, currentProperties, stylesheet, sourceContext);

                // Process table body - TBODY and TR elements
                XmlNode htmlChildNode = htmlTableElement.FirstChild;

                while(htmlChildNode != null) {
                    string htmlChildName = htmlChildNode.LocalName.ToLower();

                    // Process the element
                    if(htmlChildName == "tbody" || htmlChildName == "thead" || htmlChildName == "tfoot") {
                        //  Add more special processing for TableHeader and TableFooter
                        XmlElement xamlTableBodyElement = xamlTableElement.OwnerDocument.CreateElement(null, Xaml_TableRowGroup, _xamlNamespace);
                        xamlTableElement.AppendChild(xamlTableBodyElement);

                        sourceContext.Add((XmlElement)htmlChildNode);

                        // Get properties of Html tbody element
                        Hashtable tbodyElementLocalProperties;
                        Hashtable tbodyElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out tbodyElementLocalProperties, stylesheet, sourceContext);
                        // TODO: apply local properties for tbody

                        // Process children of htmlChildNode, which is tbody, for tr elements
                        AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode.FirstChild, tbodyElementCurrentProperties, columnStarts, stylesheet, sourceContext);
                        if(xamlTableBodyElement.HasChildNodes) {
                            xamlTableElement.AppendChild(xamlTableBodyElement);
                            // else: if there is no TRs in this TBody, we simply ignore it
                        }

                        Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode);
                        sourceContext.RemoveAt(sourceContext.Count - 1);

                        htmlChildNode = htmlChildNode.NextSibling;
                    } else if(htmlChildName == "tr") {
                        // Tbody is not present, but tr element is present. Tr is wrapped in tbody
                        XmlElement xamlTableBodyElement = xamlTableElement.OwnerDocument.CreateElement(null, Xaml_TableRowGroup, _xamlNamespace);

                        // We use currentProperties of xamlTableElement when adding rows since the tbody element is artificially created and has
                        // no properties of its own

                        htmlChildNode = AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode, currentProperties, columnStarts, stylesheet, sourceContext);
                        if(xamlTableBodyElement.HasChildNodes) {
                            xamlTableElement.AppendChild(xamlTableBodyElement);
                        }
                    } else {
                        // Element is not tbody or tr. Ignore it.
                        // TODO: add processing for thead, tfoot elements and recovery for td elements
                        htmlChildNode = htmlChildNode.NextSibling;
                    }
                }

                if(xamlTableElement.HasChildNodes) {
                    xamlParentElement.AppendChild(xamlTableElement);
                }
            }
        }

        private static XmlElement GetCellFromSingleCellTable(XmlElement htmlTableElement) {
            XmlElement singleCell = null;

            for(XmlNode tableChild = htmlTableElement.FirstChild; tableChild != null; tableChild = tableChild.NextSibling) {
                string elementName = tableChild.LocalName.ToLower();
                if(elementName == "tbody" || elementName == "thead" || elementName == "tfoot") {
                    if(singleCell != null) {
                        return null;
                    }
                    for(XmlNode tbodyChild = tableChild.FirstChild; tbodyChild != null; tbodyChild = tbodyChild.NextSibling) {
                        if(tbodyChild.LocalName.ToLower() == "tr") {
                            if(singleCell != null) {
                                return null;
                            }
                            for(XmlNode trChild = tbodyChild.FirstChild; trChild != null; trChild = trChild.NextSibling) {
                                string cellName = trChild.LocalName.ToLower();
                                if(cellName == "td" || cellName == "th") {
                                    if(singleCell != null) {
                                        return null;
                                    }
                                    singleCell = (XmlElement)trChild;
                                }
                            }
                        }
                    }
                } else if(tableChild.LocalName.ToLower() == "tr") {
                    if(singleCell != null) {
                        return null;
                    }
                    for(XmlNode trChild = tableChild.FirstChild; trChild != null; trChild = trChild.NextSibling) {
                        string cellName = trChild.LocalName.ToLower();
                        if(cellName == "td" || cellName == "th") {
                            if(singleCell != null) {
                                return null;
                            }
                            singleCell = (XmlElement)trChild;
                        }
                    }
                }
            }

            return singleCell;
        }

        /// <summary>
        /// Processes the information about table columns - COLGROUP and COL html elements.
        /// </summary>
        /// <param name="htmlTableElement">
        /// XmlElement representing a source html table.
        /// </param>
        /// <param name="xamlTableElement">
        /// XmlElement repesenting a resulting xaml table.
        /// </param>
        /// <param name="columnStartsAllRows">
        /// Array of doubles - column start coordinates.
        /// Can be null, which means that column size information is not available
        /// and we must use source colgroup/col information.
        /// In case wneh it's not null, we will ignore source colgroup/col information.
        /// </param>
        /// <param name="currentProperties"></param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        private static void AddColumnInformation(XmlElement htmlTableElement, XmlElement xamlTableElement, ArrayList columnStartsAllRows, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Add column information
            if(columnStartsAllRows != null) {
                // We have consistent information derived from table cells; use it
                // The last element in columnStarts represents the end of the table
                for(int columnIndex = 0; columnIndex < columnStartsAllRows.Count - 1; columnIndex++) {
                    XmlElement xamlColumnElement;

                    xamlColumnElement = xamlTableElement.OwnerDocument.CreateElement(null, Xaml_TableColumn, _xamlNamespace);
                    xamlColumnElement.SetAttribute(Xaml_Width, ((double)columnStartsAllRows[columnIndex + 1] - (double)columnStartsAllRows[columnIndex]).ToString());
                    xamlTableElement.AppendChild(xamlColumnElement);
                }
            } else {
                // We do not have consistent information from table cells;
                // Translate blindly colgroups from html.
                for(XmlNode htmlChildNode = htmlTableElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling) {
                    if(htmlChildNode.LocalName.ToLower() == "colgroup") {
                        // TODO: add column width information to this function as a parameter and process it
                        AddTableColumnGroup(xamlTableElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                    } else if(htmlChildNode.LocalName.ToLower() == "col") {
                        AddTableColumn(xamlTableElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                    } else if(htmlChildNode is XmlElement) {
                        // Some element which belongs to table body. Stop column loop.
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts htmlColgroupElement into Xaml TableColumnGroup element, and appends it to the parent
        /// xamlTableElement
        /// </summary>
        /// <param name="xamlTableElement">
        /// XmlElement representing Xaml Table element to which the converted column group should be added
        /// </param>
        /// <param name="htmlColgroupElement">
        /// XmlElement representing Html colgroup element to be converted
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        private static void AddTableColumnGroup(XmlElement xamlTableElement, XmlElement htmlColgroupElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlColgroupElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // TODO: process local properties for colgroup

            // Process children of colgroup. Colgroup may contain only col elements.
            for(XmlNode htmlNode = htmlColgroupElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling) {
                if(htmlNode is XmlElement && htmlNode.LocalName.ToLower() == "col") {
                    AddTableColumn(xamlTableElement, (XmlElement)htmlNode, currentProperties, stylesheet, sourceContext);
                }
            }
        }

        /// <summary>
        /// Converts htmlColElement into Xaml TableColumn element, and appends it to the parent
        /// xamlTableColumnGroupElement
        /// </summary>
        /// <param name="xamlTableElement"></param>
        /// <param name="htmlColElement">
        /// XmlElement representing Html col element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        private static void AddTableColumn(XmlElement xamlTableElement, XmlElement htmlColElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlColElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            XmlElement xamlTableColumnElement = xamlTableElement.OwnerDocument.CreateElement(null, Xaml_TableColumn, _xamlNamespace);

            // TODO: process local properties for TableColumn element

            // Col is an empty element, with no subtree
            xamlTableElement.AppendChild(xamlTableColumnElement);
        }

        /// <summary>
        /// Adds TableRow elements to xamlTableBodyElement. The rows are converted from Html tr elements that
        /// may be the children of an Html tbody element or an Html table element with tbody missing
        /// </summary>
        /// <param name="xamlTableBodyElement">
        /// XmlElement representing Xaml TableRowGroup element to which the converted rows should be added
        /// </param>
        /// <param name="htmlTRStartNode">
        /// XmlElement representing the first tr child of the tbody element to be read
        /// </param>
        /// <param name="currentProperties">
        /// Hashtable representing current properties of the tbody element that are generated and applied in the
        /// AddTable function; to be used as inheritedProperties when adding tr elements
        /// </param>
        /// <param name="columnStarts"></param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        /// <returns>
        /// XmlNode representing the current position of the iterator among tr elements
        /// </returns>
        private static XmlNode AddTableRowsToTableBody(XmlElement xamlTableBodyElement, XmlNode htmlTRStartNode, Hashtable currentProperties, ArrayList columnStarts, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Parameter validation
            Debug.Assert(xamlTableBodyElement.LocalName == Xaml_TableRowGroup);
            Debug.Assert(currentProperties != null);

            // Initialize child node for iteratimg through children to the first tr element
            XmlNode htmlChildNode = htmlTRStartNode;
            ArrayList activeRowSpans = null;
            if(columnStarts != null) {
                activeRowSpans = new ArrayList();
                InitializeActiveRowSpans(activeRowSpans, columnStarts.Count);
            }

            while(htmlChildNode != null && htmlChildNode.LocalName.ToLower() != "tbody") {
                if(htmlChildNode.LocalName.ToLower() == "tr") {
                    XmlElement xamlTableRowElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, Xaml_TableRow, _xamlNamespace);

                    sourceContext.Add((XmlElement)htmlChildNode);

                    // Get tr element properties
                    Hashtable trElementLocalProperties;
                    Hashtable trElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out trElementLocalProperties, stylesheet, sourceContext);
                    // TODO: apply local properties to tr element

                    AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode.FirstChild, trElementCurrentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
                    if(xamlTableRowElement.HasChildNodes) {
                        xamlTableBodyElement.AppendChild(xamlTableRowElement);
                    }

                    Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode);
                    sourceContext.RemoveAt(sourceContext.Count - 1);

                    // Advance
                    htmlChildNode = htmlChildNode.NextSibling;

                } else if(htmlChildNode.LocalName.ToLower() == "td") {
                    // Tr element is not present. We create one and add td elements to it
                    XmlElement xamlTableRowElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, Xaml_TableRow, _xamlNamespace);

                    // This is incorrect formatting and the column starts should not be set in this case
                    Debug.Assert(columnStarts == null);

                    htmlChildNode = AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode, currentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
                    if(xamlTableRowElement.HasChildNodes) {
                        xamlTableBodyElement.AppendChild(xamlTableRowElement);
                    }
                } else {
                    // Not a tr or td  element. Ignore it.
                    // TODO: consider better recovery here
                    htmlChildNode = htmlChildNode.NextSibling;
                }
            }
            return htmlChildNode;
        }

        /// <summary>
        /// Adds TableCell elements to xamlTableRowElement.
        /// </summary>
        /// <param name="xamlTableRowElement">
        /// XmlElement representing Xaml TableRow element to which the converted cells should be added
        /// </param>
        /// <param name="htmlTDStartNode">
        /// XmlElement representing the child of tr or tbody element from which we should start adding td elements
        /// </param>
        /// <param name="currentProperties">
        /// properties of the current html tr element to which cells are to be added
        /// </param>
        /// <returns>
        /// XmlElement representing the current position of the iterator among the children of the parent Html tbody/tr element
        /// </returns>
        private static XmlNode AddTableCellsToTableRow(XmlElement xamlTableRowElement, XmlNode htmlTDStartNode, Hashtable currentProperties, ArrayList columnStarts, ArrayList activeRowSpans, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // parameter validation
            Debug.Assert(xamlTableRowElement.LocalName == Xaml_TableRow);
            Debug.Assert(currentProperties != null);
            if(columnStarts != null) {
                Debug.Assert(activeRowSpans.Count == columnStarts.Count);
            }

            XmlNode htmlChildNode = htmlTDStartNode;
            double columnStart = 0;
            double columnWidth = 0;
            int columnIndex = 0;
            int columnSpan = 0;

            while(htmlChildNode != null && htmlChildNode.LocalName.ToLower() != "tr" && htmlChildNode.LocalName.ToLower() != "tbody" && htmlChildNode.LocalName.ToLower() != "thead" && htmlChildNode.LocalName.ToLower() != "tfoot") {
                if(htmlChildNode.LocalName.ToLower() == "td" || htmlChildNode.LocalName.ToLower() == "th") {
                    XmlElement xamlTableCellElement = xamlTableRowElement.OwnerDocument.CreateElement(null, Xaml_TableCell, _xamlNamespace);

                    sourceContext.Add((XmlElement)htmlChildNode);

                    Hashtable tdElementLocalProperties;
                    Hashtable tdElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out tdElementLocalProperties, stylesheet, sourceContext);

                    // TODO: determine if localProperties can be used instead of htmlChildNode in this call, and if they can,
                    // make necessary changes and use them instead.
                    ApplyPropertiesToTableCellElement((XmlElement)htmlChildNode, xamlTableCellElement);

                    if(columnStarts != null) {
                        Debug.Assert(columnIndex < columnStarts.Count - 1);
                        while(columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0) {
                            activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                            Debug.Assert((int)activeRowSpans[columnIndex] >= 0);
                            columnIndex++;
                        }
                        Debug.Assert(columnIndex < columnStarts.Count - 1);
                        columnStart = (double)columnStarts[columnIndex];
                        columnWidth = GetColumnWidth((XmlElement)htmlChildNode);
                        columnSpan = CalculateColumnSpan(columnIndex, columnWidth, columnStarts);
                        int rowSpan = GetRowSpan((XmlElement)htmlChildNode);

                        // Column cannot have no span
                        Debug.Assert(columnSpan > 0);
                        Debug.Assert(columnIndex + columnSpan < columnStarts.Count);

                        xamlTableCellElement.SetAttribute(Xaml_TableCell_ColumnSpan, columnSpan.ToString());

                        // Apply row span
                        for(int spannedColumnIndex = columnIndex; spannedColumnIndex < columnIndex + columnSpan; spannedColumnIndex++) {
                            Debug.Assert(spannedColumnIndex < activeRowSpans.Count);
                            activeRowSpans[spannedColumnIndex] = (rowSpan - 1);
                            Debug.Assert((int)activeRowSpans[spannedColumnIndex] >= 0);
                        }

                        columnIndex = columnIndex + columnSpan;
                    }

                    AddDataToTableCell(xamlTableCellElement, htmlChildNode.FirstChild, tdElementCurrentProperties, stylesheet, sourceContext);
                    if(xamlTableCellElement.HasChildNodes) {
                        xamlTableRowElement.AppendChild(xamlTableCellElement);
                    }

                    Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode);
                    sourceContext.RemoveAt(sourceContext.Count - 1);

                    htmlChildNode = htmlChildNode.NextSibling;
                } else {
                    // Not td element. Ignore it.
                    // TODO: Consider better recovery
                    htmlChildNode = htmlChildNode.NextSibling;
                }
            }
            return htmlChildNode;
        }

        /// <summary>
        /// adds table cell data to xamlTableCellElement
        /// </summary>
        /// <param name="xamlTableCellElement">
        /// XmlElement representing Xaml TableCell element to which the converted data should be added
        /// </param>
        /// <param name="htmlDataStartNode">
        /// XmlElement representing the start element of data to be added to xamlTableCellElement
        /// </param>
        /// <param name="currentProperties">
        /// Current properties for the html td/th element corresponding to xamlTableCellElement
        /// </param>
        private static void AddDataToTableCell(XmlElement xamlTableCellElement, XmlNode htmlDataStartNode, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Parameter validation
            Debug.Assert(xamlTableCellElement.LocalName == Xaml_TableCell);
            Debug.Assert(currentProperties != null);

            for(XmlNode htmlChildNode = htmlDataStartNode; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null) {
                // Process a new html element and add it to the td element
                htmlChildNode = AddBlock(xamlTableCellElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }
        }

        /// <summary>
        /// Performs a parsing pass over a table to read information about column width and rowspan attributes. This information
        /// is used to determine the starting point of each column.
        /// </summary>
        /// <param name="htmlTableElement">
        /// XmlElement representing Html table whose structure is to be analyzed
        /// </param>
        /// <returns>
        /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
        /// all the points which are the starting position of any column in the table, ordered from left to right.
        /// In case if analisys was impossible we return null.
        /// </returns>
        private static ArrayList AnalyzeTableStructure(XmlElement htmlTableElement, CssStylesheet stylesheet) {
            // Parameter validation
            Debug.Assert(htmlTableElement.LocalName.ToLower() == "table");
            if(!htmlTableElement.HasChildNodes) {
                return null;
            }

            bool columnWidthsAvailable = true;

            ArrayList columnStarts = new ArrayList();
            ArrayList activeRowSpans = new ArrayList();
            Debug.Assert(columnStarts.Count == activeRowSpans.Count);

            XmlNode htmlChildNode = htmlTableElement.FirstChild;
            double tableWidth = 0;  // Keep track of table width which is the width of its widest row

            // Analyze tbody and tr elements
            while(htmlChildNode != null && columnWidthsAvailable) {
                Debug.Assert(columnStarts.Count == activeRowSpans.Count);

                switch(htmlChildNode.LocalName.ToLower()) {
                    case "tbody":
                        // Tbody element, we should analyze its children for trows
                        double tbodyWidth = AnalyzeTbodyStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans, tableWidth, stylesheet);
                        if(tbodyWidth > tableWidth) {
                            // Table width must be increased to supported newly added wide row
                            tableWidth = tbodyWidth;
                        } else if(tbodyWidth == 0) {
                            // Tbody analysis may return 0, probably due to unprocessable format.
                            // We should also fail.
                            columnWidthsAvailable = false; // interrupt the analisys
                        }
                        break;
                    case "tr":
                        // Table row. Analyze column structure within row directly
                        double trWidth = AnalyzeTRStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans, tableWidth, stylesheet);
                        if(trWidth > tableWidth) {
                            tableWidth = trWidth;
                        } else if(trWidth == 0) {
                            columnWidthsAvailable = false; // interrupt the analisys
                        }
                        break;
                    case "td":
                        // Incorrect formatting, too deep to analyze at this level. Return null.
                        // TODO: implement analysis at this level, possibly by creating a new tr
                        columnWidthsAvailable = false; // interrupt the analisys
                        break;
                    default:
                        // Element should not occur directly in table. Ignore it.
                        break;
                }

                htmlChildNode = htmlChildNode.NextSibling;
            }

            if(columnWidthsAvailable) {
                // Add an item for whole table width
                columnStarts.Add(tableWidth);
                VerifyColumnStartsAscendingOrder(columnStarts);
            } else {
                columnStarts = null;
            }

            return columnStarts;
        }

        /// <summary>
        /// Performs a parsing pass over a tbody to read information about column width and rowspan attributes. Information read about width
        /// attributes is stored in the reference ArrayList parameter columnStarts, which contains a list of all starting
        /// positions of all columns in the table, ordered from left to right. Row spans are taken into consideration when
        /// computing column starts
        /// </summary>
        /// <param name="htmlTbodyElement">
        /// XmlElement representing Html tbody whose structure is to be analyzed
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList of type double which contains the function output. If analysis fails, this parameter is set to null
        /// </param>
        /// <param name="tableWidth">
        /// Current width of the table. This is used to determine if a new column when added to the end of table should
        /// come after the last column in the table or is actually splitting the last column in two. If it is only splitting
        /// the last column it should inherit row span for that column
        /// </param>
        /// <returns>
        /// Calculated width of a tbody.
        /// In case of non-analizable column width structure return 0;
        /// </returns>
        private static double AnalyzeTbodyStructure(XmlElement htmlTbodyElement, ArrayList columnStarts, ArrayList activeRowSpans, double tableWidth, CssStylesheet stylesheet) {
            // Parameter validation
            Debug.Assert(htmlTbodyElement.LocalName.ToLower() == "tbody");
            Debug.Assert(columnStarts != null);

            double tbodyWidth = 0;
            bool columnWidthsAvailable = true;

            if(!htmlTbodyElement.HasChildNodes) {
                return tbodyWidth;
            }

            // Set active row spans to 0 - thus ignoring row spans crossing tbody boundaries
            ClearActiveRowSpans(activeRowSpans);

            XmlNode htmlChildNode = htmlTbodyElement.FirstChild;

            // Analyze tr elements
            while(htmlChildNode != null && columnWidthsAvailable) {
                switch(htmlChildNode.LocalName.ToLower()) {
                    case "tr":
                        double trWidth = AnalyzeTRStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans, tbodyWidth, stylesheet);
                        if(trWidth > tbodyWidth) {
                            tbodyWidth = trWidth;
                        }
                        break;
                    case "td":
                        columnWidthsAvailable = false; // interrupt the analisys
                        break;
                    default:
                        break;
                }
                htmlChildNode = htmlChildNode.NextSibling;
            }

            // Set active row spans to 0 - thus ignoring row spans crossing tbody boundaries
            ClearActiveRowSpans(activeRowSpans);

            return columnWidthsAvailable ? tbodyWidth : 0;
        }

        /// <summary>
        /// Performs a parsing pass over a tr element to read information about column width and rowspan attributes.
        /// </summary>
        /// <param name="htmlTRElement">
        /// XmlElement representing Html tr element whose structure is to be analyzed
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
        /// all the points which are the starting position of any column in the tr, ordered from left to right. If analysis fails,
        /// the ArrayList is set to null
        /// </param>
        /// <param name="activeRowSpans">
        /// ArrayList representing all columns currently spanned by an earlier row span attribute. These columns should
        /// not be used for data in this row. The ArrayList actually contains notation for all columns in the table, if the
        /// active row span is set to 0 that column is not presently spanned but if it is > 0 the column is presently spanned
        /// </param>
        /// <param name="tableWidth">
        /// Double value representing the current width of the table.
        /// Return 0 if analisys was insuccessful.
        /// </param>
        private static double AnalyzeTRStructure(XmlElement htmlTRElement, ArrayList columnStarts, ArrayList activeRowSpans, double tableWidth, CssStylesheet stylesheet) {
            double columnWidth;

            // Parameter validation
            Debug.Assert(htmlTRElement.LocalName.ToLower() == "tr");
            Debug.Assert(columnStarts != null);
            Debug.Assert(activeRowSpans != null);
            Debug.Assert(columnStarts.Count == activeRowSpans.Count);

            if(!htmlTRElement.HasChildNodes) {
                return 0;
            }

            bool columnWidthsAvailable = true;

            double columnStart = 0; // starting position of current column
            XmlNode htmlChildNode = htmlTRElement.FirstChild;
            int columnIndex = 0;
            double trWidth = 0;

            // Skip spanned columns to get to real column start
            if(columnIndex < activeRowSpans.Count) {
                Debug.Assert((double)columnStarts[columnIndex] >= columnStart);
                if((double)columnStarts[columnIndex] == columnStart) {
                    // The new column may be in a spanned area
                    while(columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0) {
                        activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                        Debug.Assert((int)activeRowSpans[columnIndex] >= 0);
                        columnIndex++;
                        columnStart = (double)columnStarts[columnIndex];
                    }
                }
            }

            while(htmlChildNode != null && columnWidthsAvailable) {
                Debug.Assert(columnStarts.Count == activeRowSpans.Count);

                VerifyColumnStartsAscendingOrder(columnStarts);

                switch(htmlChildNode.LocalName.ToLower()) {
                    case "td":
                        Debug.Assert(columnIndex <= columnStarts.Count);
                        if(columnIndex < columnStarts.Count) {
                            Debug.Assert(columnStart <= (double)columnStarts[columnIndex]);
                            if(columnStart < (double)columnStarts[columnIndex]) {
                                columnStarts.Insert(columnIndex, columnStart);
                                // There can be no row spans now - the column data will appear here
                                // Row spans may appear only during the column analysis
                                activeRowSpans.Insert(columnIndex, 0);
                            }
                        } else {
                            // Column start is greater than all previous starts. Row span must still be 0 because
                            // we are either adding after another column of the same row, in which case it should not inherit
                            // the previous column's span. Otherwise we are adding after the last column of some previous
                            // row, and assuming the table widths line up, we should not be spanned by it. If there is
                            // an incorrect tbale structure where a columns starts in the middle of a row span, we do not
                            // guarantee correct output
                            columnStarts.Add(columnStart);
                            activeRowSpans.Add(0);
                        }
                        columnWidth = GetColumnWidth((XmlElement)htmlChildNode);
                        if(columnWidth != -1) {
                            int nextColumnIndex;
                            int rowSpan = GetRowSpan((XmlElement)htmlChildNode);

                            nextColumnIndex = GetNextColumnIndex(columnIndex, columnWidth, columnStarts, activeRowSpans);
                            if(nextColumnIndex != -1) {
                                // Entire column width can be processed without hitting conflicting row span. This means that
                                // column widths line up and we can process them
                                Debug.Assert(nextColumnIndex <= columnStarts.Count);

                                // Apply row span to affected columns
                                for(int spannedColumnIndex = columnIndex; spannedColumnIndex < nextColumnIndex; spannedColumnIndex++) {
                                    activeRowSpans[spannedColumnIndex] = rowSpan - 1;
                                    Debug.Assert((int)activeRowSpans[spannedColumnIndex] >= 0);
                                }

                                columnIndex = nextColumnIndex;

                                // Calculate columnsStart for the next cell
                                columnStart = columnStart + columnWidth;

                                if(columnIndex < activeRowSpans.Count) {
                                    Debug.Assert((double)columnStarts[columnIndex] >= columnStart);
                                    if((double)columnStarts[columnIndex] == columnStart) {
                                        // The new column may be in a spanned area
                                        while(columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0) {
                                            activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                                            Debug.Assert((int)activeRowSpans[columnIndex] >= 0);
                                            columnIndex++;
                                            columnStart = (double)columnStarts[columnIndex];
                                        }
                                    }
                                    // else: the new column does not start at the same time as a pre existing column
                                    // so we don't have to check it for active row spans, it starts in the middle
                                    // of another column which has been checked already by the GetNextColumnIndex function
                                }
                            } else {
                                // Full column width cannot be processed without a pre existing row span.
                                // We cannot analyze widths
                                columnWidthsAvailable = false;
                            }
                        } else {
                            // Incorrect column width, stop processing
                            columnWidthsAvailable = false;
                        }
                        break;
                    default:
                        break;
                }

                htmlChildNode = htmlChildNode.NextSibling;
            }

            // The width of the tr element is the position at which it's last td element ends, which is calculated in
            // the columnStart value after each td element is processed
            if(columnWidthsAvailable) {
                trWidth = columnStart;
            } else {
                trWidth = 0;
            }

            return trWidth;
        }

        /// <summary>
        /// Gets row span attribute from htmlTDElement. Returns an integer representing the value of the rowspan attribute.
        /// Default value if attribute is not specified or if it is invalid is 1
        /// </summary>
        /// <param name="htmlTDElement">
        /// Html td element to be searched for rowspan attribute
        /// </param>
        private static int GetRowSpan(XmlElement htmlTDElement) {
            string rowSpanAsString;
            int rowSpan;

            rowSpanAsString = GetAttribute((XmlElement)htmlTDElement, "rowspan");
            if(rowSpanAsString != null) {
                if(!Int32.TryParse(rowSpanAsString, out rowSpan)) {
                    // Ignore invalid value of rowspan; treat it as 1
                    rowSpan = 1;
                }
            } else {
                // No row span, default is 1
                rowSpan = 1;
            }
            return rowSpan;
        }

        /// <summary>
        /// Gets index at which a column should be inseerted into the columnStarts ArrayList. This is
        /// decided by the value columnStart. The columnStarts ArrayList is ordered in ascending order.
        /// Returns an integer representing the index at which the column should be inserted
        /// </summary>
        /// <param name="columnStarts">
        /// Array list representing starting coordinates of all columns in the table
        /// </param>
        /// <param name="columnStart">
        /// Starting coordinate of column we wish to insert into columnStart
        /// </param>
        /// <param name="columnIndex">
        /// Int representing the current column index. This acts as a clue while finding the insertion index.
        /// If the value of columnStarts at columnIndex is the same as columnStart, then this position alrady exists
        /// in the array and we can jsut return columnIndex.
        /// </param>
        /// <returns></returns>
        private static int GetNextColumnIndex(int columnIndex, double columnWidth, ArrayList columnStarts, ArrayList activeRowSpans) {
            double columnStart;
            int spannedColumnIndex;

            // Parameter validation
            Debug.Assert(columnStarts != null);
            Debug.Assert(0 <= columnIndex && columnIndex <= columnStarts.Count);
            Debug.Assert(columnWidth > 0);

            columnStart = (double)columnStarts[columnIndex];
            spannedColumnIndex = columnIndex + 1;

            while(spannedColumnIndex < columnStarts.Count && (double)columnStarts[spannedColumnIndex] < columnStart + columnWidth && spannedColumnIndex != -1) {
                if((int)activeRowSpans[spannedColumnIndex] > 0) {
                    // The current column should span this area, but something else is already spanning it
                    // Not analyzable
                    spannedColumnIndex = -1;
                } else {
                    spannedColumnIndex++;
                }
            }

            return spannedColumnIndex;
        }


        /// <summary>
        /// Used for clearing activeRowSpans array in the beginning/end of each tbody
        /// </summary>
        /// <param name="activeRowSpans">
        /// ArrayList representing currently active row spans
        /// </param>
        private static void ClearActiveRowSpans(ArrayList activeRowSpans) {
            for(int columnIndex = 0; columnIndex < activeRowSpans.Count; columnIndex++) {
                activeRowSpans[columnIndex] = 0;
            }
        }

        /// <summary>
        /// Used for initializing activeRowSpans array in the before adding rows to tbody element
        /// </summary>
        /// <param name="activeRowSpans">
        /// ArrayList representing currently active row spans
        /// </param>
        /// <param name="count">
        /// Size to be give to array list
        /// </param>
        private static void InitializeActiveRowSpans(ArrayList activeRowSpans, int count) {
            for(int columnIndex = 0; columnIndex < count; columnIndex++) {
                activeRowSpans.Add(0);
            }
        }


        /// <summary>
        /// Calculates width of next TD element based on starting position of current element and it's width, which
        /// is calculated byt he function
        /// </summary>
        /// <param name="htmlTDElement">
        /// XmlElement representing Html td element whose width is to be read
        /// </param>
        /// <param name="columnStart">
        /// Starting position of current column
        /// </param>
        private static double GetNextColumnStart(XmlElement htmlTDElement, double columnStart) {
            double columnWidth;
            double nextColumnStart;

            // Parameter validation
            Debug.Assert(htmlTDElement.LocalName.ToLower() == "td" || htmlTDElement.LocalName.ToLower() == "th");
            Debug.Assert(columnStart >= 0);

            nextColumnStart = -1;  // -1 indicates inability to calculate columnStart width

            columnWidth = GetColumnWidth(htmlTDElement);

            if(columnWidth == -1) {
                nextColumnStart = -1;
            } else {
                nextColumnStart = columnStart + columnWidth;
            }

            return nextColumnStart;
        }


        private static double GetColumnWidth(XmlElement htmlTDElement) {
            string columnWidthAsString;
            double columnWidth;

            columnWidthAsString = null;
            columnWidth = -1;

            // Get string valkue for the width
            columnWidthAsString = GetAttribute(htmlTDElement, "width");
            if(columnWidthAsString == null) {
                columnWidthAsString = GetCssAttribute(GetAttribute(htmlTDElement, "style"), "width");
            }

            // We do not allow column width to be 0, if specified as 0 we will fail to record it
            if(!TryGetLengthValue(columnWidthAsString, out columnWidth) || columnWidth == 0) {
                columnWidth = -1;
            }
            return columnWidth;
        }

        /// <summary>
        /// Calculates column span based the column width and the widths of all other columns. Returns an integer representing
        /// the column span
        /// </summary>
        /// <param name="columnIndex">
        /// Index of the current column
        /// </param>
        /// <param name="columnWidth">
        /// Width of the current column
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList repsenting starting coordinates of all columns
        /// </param>
        private static int CalculateColumnSpan(int columnIndex, double columnWidth, ArrayList columnStarts) {
            // Current status of column width. Indicates the amount of width that has been scanned already
            double columnSpanningValue;
            int columnSpanningIndex;
            int columnSpan;
            double subColumnWidth; // Width of the smallest-grain columns in the table

            Debug.Assert(columnStarts != null);
            Debug.Assert(columnIndex < columnStarts.Count - 1);
            Debug.Assert((double)columnStarts[columnIndex] >= 0);
            Debug.Assert(columnWidth > 0);

            columnSpanningIndex = columnIndex;
            columnSpanningValue = 0;
            columnSpan = 0;
            subColumnWidth = 0;

            while(columnSpanningValue < columnWidth && columnSpanningIndex < columnStarts.Count - 1) {
                subColumnWidth = (double)columnStarts[columnSpanningIndex + 1] - (double)columnStarts[columnSpanningIndex];
                Debug.Assert(subColumnWidth > 0);
                columnSpanningValue += subColumnWidth;
                columnSpanningIndex++;
            }

            // Now, we have either covered the width we needed to cover or reached the end of the table, in which
            // case the column spans all the columns until the end
            columnSpan = columnSpanningIndex - columnIndex;
            Debug.Assert(columnSpan > 0);

            return columnSpan;
        }

        /// <summary>
        /// Verifies that values in columnStart, which represent starting coordinates of all columns, are arranged
        /// in ascending order
        /// </summary>
        /// <param name="columnStarts">
        /// ArrayList representing starting coordinates of all columns
        /// </param>
        private static void VerifyColumnStartsAscendingOrder(ArrayList columnStarts) {
            Debug.Assert(columnStarts != null);

            double columnStart;

            columnStart = -0.01;

            for(int columnIndex = 0; columnIndex < columnStarts.Count; columnIndex++) {
                Debug.Assert(columnStart < (double)columnStarts[columnIndex]);
                columnStart = (double)columnStarts[columnIndex];
            }
        }

        // .............................................................
        //
        // Attributes and Properties
        //
        // .............................................................

        /// <summary>
        /// Analyzes local properties of Html element, converts them into Xaml equivalents, and applies them to xamlElement
        /// </summary>
        /// <param name="xamlElement">
        /// XmlElement representing Xaml element to which properties are to be applied
        /// </param>
        /// <param name="localProperties">
        /// Hashtable representing local properties of Html element that is converted into xamlElement
        /// </param>
        private static void ApplyLocalProperties(XmlElement xamlElement, Hashtable localProperties, bool isBlock) {
            bool marginSet = false;
            string marginTop = "0";
            string marginBottom = "0";
            string marginLeft = "0";
            string marginRight = "0";

            bool paddingSet = false;
            string paddingTop = "0";
            string paddingBottom = "0";
            string paddingLeft = "0";
            string paddingRight = "0";

            string borderColor = null;

            bool borderThicknessSet = false;
            string borderThicknessTop = "0";
            string borderThicknessBottom = "0";
            string borderThicknessLeft = "0";
            string borderThicknessRight = "0";

            IDictionaryEnumerator propertyEnumerator = localProperties.GetEnumerator();
            while(propertyEnumerator.MoveNext()) {
                switch((string)propertyEnumerator.Key) {
                    case "font-family":
                        //  Convert from font-family value list into xaml FontFamily value
                        xamlElement.SetAttribute(Xaml_FontFamily, (string)propertyEnumerator.Value);
                        break;
                    case "font-style":
                        xamlElement.SetAttribute(Xaml_FontStyle, (string)propertyEnumerator.Value);
                        break;
                    case "font-variant":
                        //  Convert from font-variant into xaml property
                        break;
                    case "font-weight":
                        xamlElement.SetAttribute(Xaml_FontWeight, (string)propertyEnumerator.Value);
                        break;
                    case "font-size":
                        //  Convert from css size into FontSize
                        xamlElement.SetAttribute(Xaml_FontSize, (string)propertyEnumerator.Value);
                        break;
                    case "color":
                        SetPropertyValue(xamlElement, TextElement.ForegroundProperty, (string)propertyEnumerator.Value);
                        break;
                    case "background-color":
                        SetPropertyValue(xamlElement, TextElement.BackgroundProperty, (string)propertyEnumerator.Value);
                        break;
                    case "text-decoration-underline":
                        if(!isBlock) {
                            if((string)propertyEnumerator.Value == "true") {
                                xamlElement.SetAttribute(Xaml_TextDecorations, Xaml_TextDecorations_Underline);
                            }
                        }
                        break;
                    case "text-decoration-none":
                    case "text-decoration-overline":
                    case "text-decoration-line-through":
                    case "text-decoration-blink":
                        //  Convert from all other text-decorations values
                        if(!isBlock) {
                        }
                        break;
                    case "text-transform":
                        //  Convert from text-transform into xaml property
                        break;

                    case "text-indent":
                        if(isBlock) {
                            xamlElement.SetAttribute(Xaml_TextIndent, (string)propertyEnumerator.Value);
                        }
                        break;

                    case "text-align":
                        if(isBlock) {
                            xamlElement.SetAttribute(Xaml_TextAlignment, (string)propertyEnumerator.Value);
                        }
                        break;

                    case "width":
                    case "height":
                        //  Decide what to do with width and height propeties
                        break;

                    case "margin-top":
                        marginSet = true;
                        marginTop = (string)propertyEnumerator.Value;
                        break;
                    case "margin-right":
                        marginSet = true;
                        marginRight = (string)propertyEnumerator.Value;
                        break;
                    case "margin-bottom":
                        marginSet = true;
                        marginBottom = (string)propertyEnumerator.Value;
                        break;
                    case "margin-left":
                        marginSet = true;
                        marginLeft = (string)propertyEnumerator.Value;
                        break;

                    case "padding-top":
                        paddingSet = true;
                        paddingTop = (string)propertyEnumerator.Value;
                        break;
                    case "padding-right":
                        paddingSet = true;
                        paddingRight = (string)propertyEnumerator.Value;
                        break;
                    case "padding-bottom":
                        paddingSet = true;
                        paddingBottom = (string)propertyEnumerator.Value;
                        break;
                    case "padding-left":
                        paddingSet = true;
                        paddingLeft = (string)propertyEnumerator.Value;
                        break;

                    // NOTE: css names for elementary border styles have side indications in the middle (top/bottom/left/right)
                    // In our internal notation we intentionally put them at the end - to unify processing in ParseCssRectangleProperty method
                    case "border-color-top":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-color-right":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-color-bottom":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-color-left":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-style-top":
                    case "border-style-right":
                    case "border-style-bottom":
                    case "border-style-left":
                        //  Implement conversion from border style
                        break;
                    case "border-width-top":
                        borderThicknessSet = true;
                        borderThicknessTop = (string)propertyEnumerator.Value;
                        break;
                    case "border-width-right":
                        borderThicknessSet = true;
                        borderThicknessRight = (string)propertyEnumerator.Value;
                        break;
                    case "border-width-bottom":
                        borderThicknessSet = true;
                        borderThicknessBottom = (string)propertyEnumerator.Value;
                        break;
                    case "border-width-left":
                        borderThicknessSet = true;
                        borderThicknessLeft = (string)propertyEnumerator.Value;
                        break;

                    case "list-style-type":
                        if(xamlElement.LocalName == Xaml_List) {
                            string markerStyle;
                            switch(((string)propertyEnumerator.Value).ToLower()) {
                                case "disc":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_Disc;
                                    break;
                                case "circle":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_Circle;
                                    break;
                                case "none":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_None;
                                    break;
                                case "square":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_Square;
                                    break;
                                case "box":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_Box;
                                    break;
                                case "lower-latin":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_LowerLatin;
                                    break;
                                case "upper-latin":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_UpperLatin;
                                    break;
                                case "lower-roman":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_LowerRoman;
                                    break;
                                case "upper-roman":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_UpperRoman;
                                    break;
                                case "decimal":
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_Decimal;
                                    break;
                                default:
                                    markerStyle = HtmlToXamlConverter.Xaml_List_MarkerStyle_Disc;
                                    break;
                            }
                            xamlElement.SetAttribute(HtmlToXamlConverter.Xaml_List_MarkerStyle, markerStyle);
                        }
                        break;

                    case "float":
                    case "clear":
                        if(isBlock) {
                            //  Convert float and clear properties
                        }
                        break;

                    case "display":
                        break;
                }
            }

            if(isBlock) {
                if(marginSet) {
                    ComposeThicknessProperty(xamlElement, Xaml_Margin, marginLeft, marginRight, marginTop, marginBottom);
                }

                if(paddingSet) {
                    ComposeThicknessProperty(xamlElement, Xaml_Padding, paddingLeft, paddingRight, paddingTop, paddingBottom);
                }

                if(borderColor != null) {
                    //  We currently ignore possible difference in brush colors on different border sides. Use the last colored side mentioned
                    xamlElement.SetAttribute(Xaml_BorderBrush, borderColor);
                }

                if(borderThicknessSet) {
                    ComposeThicknessProperty(xamlElement, Xaml_BorderThickness, borderThicknessLeft, borderThicknessRight, borderThicknessTop, borderThicknessBottom);
                }
            }
        }

        // Create syntactically optimized four-value Thickness
        private static void ComposeThicknessProperty(XmlElement xamlElement, string propertyName, string left, string right, string top, string bottom) {
            // Xaml syntax:
            // We have a reasonable interpreation for one value (all four edges), two values (horizontal, vertical),
            // and four values (left, top, right, bottom).
            //  switch (i) {
            //    case 1: return new Thickness(lengths[0]);
            //    case 2: return new Thickness(lengths[0], lengths[1], lengths[0], lengths[1]);
            //    case 4: return new Thickness(lengths[0], lengths[1], lengths[2], lengths[3]);
            //  }
            string thickness;

            // We do not accept negative margins
            if(left[0] == '0' || left[0] == '-') left = "0";
            if(right[0] == '0' || right[0] == '-') right = "0";
            if(top[0] == '0' || top[0] == '-') top = "0";
            if(bottom[0] == '0' || bottom[0] == '-') bottom = "0";

            if(left == right && top == bottom) {
                if(left == top) {
                    thickness = left;
                } else {
                    thickness = left + "," + top;
                }
            } else {
                thickness = left + "," + top + "," + right + "," + bottom;
            }

            //  Need safer processing for a thickness value
            xamlElement.SetAttribute(propertyName, thickness);
        }

        private static void SetPropertyValue(XmlElement xamlElement, DependencyProperty property, string stringValue) {
            System.ComponentModel.TypeConverter typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType);
            try {
                object convertedValue = typeConverter.ConvertFromInvariantString(stringValue);
                if(convertedValue != null) {
                    xamlElement.SetAttribute(property.Name, stringValue);
                }
            } catch(Exception) {
            }
        }

        /// <summary>
        /// Analyzes the tag of the htmlElement and infers its associated formatted properties.
        /// After that parses style attribute and adds all inline css styles.
        /// The resulting style attributes are collected in output parameter localProperties.
        /// </summary>
        /// <param name="htmlElement">
        /// </param>
        /// <param name="inheritedProperties">
        /// set of properties inherited from ancestor elements. Currently not used in the code. Reserved for the future development.
        /// </param>
        /// <param name="localProperties">
        /// returns all formatting properties defined by this element - implied by its tag, its attributes, or its css inline style
        /// </param>
        /// <param name="stylesheet"></param>
        /// <param name="sourceContext"></param>
        /// <returns>
        /// returns a combination of previous context with local set of properties.
        /// This value is not used in the current code - inntended for the future development.
        /// </returns>
        private static Hashtable GetElementProperties(XmlElement htmlElement, Hashtable inheritedProperties, out Hashtable localProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext) {
            // Start with context formatting properties
            Hashtable currentProperties = new Hashtable();
            IDictionaryEnumerator propertyEnumerator = inheritedProperties.GetEnumerator();
            while(propertyEnumerator.MoveNext()) {
                currentProperties[propertyEnumerator.Key] = propertyEnumerator.Value;
            }

            // Identify element name
            string elementName = htmlElement.LocalName.ToLower();
            string elementNamespace = htmlElement.NamespaceURI;

            // update current formatting properties depending on element tag

            localProperties = new Hashtable();
            switch(elementName) {
                // Character formatting
                case "i":
                case "italic":
                case "em":
                    localProperties["font-style"] = "italic";
                    break;
                case "b":
                case "bold":
                case "strong":
                case "dfn":
                    localProperties["font-weight"] = "bold";
                    break;
                case "u":
                case "underline":
                    localProperties["text-decoration-underline"] = "true";
                    break;
                case "font":
                    string attributeValue = GetAttribute(htmlElement, "face");
                    if(attributeValue != null) {
                        localProperties["font-family"] = attributeValue;
                    }
                    attributeValue = GetAttribute(htmlElement, "size");
                    if(attributeValue != null) {
                        /* fontタグ size属性は無効にする
                        double fontSize = double.Parse(attributeValue) * (12.0 / 3.0);
                        if(fontSize < 1.0) {
                            fontSize = 1.0;
                        } else if(fontSize > 1000.0) {
                            fontSize = 1000.0;
                        }
                        localProperties["font-size"] = fontSize.ToString();
                        */
                    }
                    attributeValue = GetAttribute(htmlElement, "color");
                    if(attributeValue != null) {
                        localProperties["color"] = attributeValue;
                    }
                    break;
                case "samp":
                    localProperties["font-family"] = "Courier New"; // code sample
                    localProperties["font-size"] = Xaml_FontSize_XXSmall;
                    localProperties["text-align"] = "Left";
                    break;
                case "sub":
                    break;
                case "sup":
                    break;

                // Hyperlinks
                case "a": // href, hreflang, urn, methods, rel, rev, title
                    //  Set default hyperlink properties
                    break;
                case "acronym":
                    break;

                // Paragraph formatting:
                case "p":
                    //  Set default paragraph properties
                    break;
                case "div":
                    //  Set default div properties
                    break;
                case "pre":
                    localProperties["font-family"] = "Courier New"; // renders text in a fixed-width font
                    localProperties["font-size"] = Xaml_FontSize_XXSmall;
                    localProperties["text-align"] = "Left";
                    break;
                case "blockquote":
                    localProperties["margin-left"] = "16";
                    break;

                case "h1":
                    localProperties["font-size"] = Xaml_FontSize_XXLarge;
                    break;
                case "h2":
                    localProperties["font-size"] = Xaml_FontSize_XLarge;
                    break;
                case "h3":
                    localProperties["font-size"] = Xaml_FontSize_Large;
                    break;
                case "h4":
                    localProperties["font-size"] = Xaml_FontSize_Medium;
                    break;
                case "h5":
                    localProperties["font-size"] = Xaml_FontSize_Small;
                    break;
                case "h6":
                    localProperties["font-size"] = Xaml_FontSize_XSmall;
                    break;
                // List properties
                case "ul":
                    localProperties["list-style-type"] = "disc";
                    break;
                case "ol":
                    localProperties["list-style-type"] = "decimal";
                    break;

                case "table":
                case "body":
                case "html":
                    break;
            }

            // Override html defaults by css attributes - from stylesheets and inline settings
            HtmlCssParser.GetElementPropertiesFromCssAttributes(htmlElement, elementName, stylesheet, localProperties, sourceContext);

            // Combine local properties with context to create new current properties
            propertyEnumerator = localProperties.GetEnumerator();
            while(propertyEnumerator.MoveNext()) {
                currentProperties[propertyEnumerator.Key] = propertyEnumerator.Value;
            }

            return currentProperties;
        }

        /// <summary>
        /// Extracts a value of css attribute from css style definition.
        /// </summary>
        /// <param name="cssStyle">
        /// Source csll style definition
        /// </param>
        /// <param name="attributeName">
        /// A name of css attribute to extract
        /// </param>
        /// <returns>
        /// A string rrepresentation of an attribute value if found;
        /// null if there is no such attribute in a given string.
        /// </returns>
        private static string GetCssAttribute(string cssStyle, string attributeName) {
            //  This is poor man's attribute parsing. Replace it by real css parsing
            if(cssStyle != null) {
                string[] styleValues;

                attributeName = attributeName.ToLower();

                // Check for width specification in style string
                styleValues = cssStyle.Split(';');

                for(int styleValueIndex = 0; styleValueIndex < styleValues.Length; styleValueIndex++) {
                    string[] styleNameValue;

                    styleNameValue = styleValues[styleValueIndex].Split(':');
                    if(styleNameValue.Length == 2) {
                        if(styleNameValue[0].Trim().ToLower() == attributeName) {
                            return styleNameValue[1].Trim();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts a length value from string representation to a double.
        /// </summary>
        /// <param name="lengthAsString">
        /// Source string value of a length.
        /// </param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static bool TryGetLengthValue(string lengthAsString, out double length) {
            length = Double.NaN;

            if(lengthAsString != null) {
                lengthAsString = lengthAsString.Trim().ToLower();

                // We try to convert currentColumnWidthAsString into a double. This will eliminate widths of type "50%", etc.
                if(lengthAsString.EndsWith("pt")) {
                    lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
                    if(Double.TryParse(lengthAsString, out length)) {
                        length = (length * 96.0) / 72.0; // convert from points to pixels
                    } else {
                        length = Double.NaN;
                    }
                } else if(lengthAsString.EndsWith("px")) {
                    lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
                    if(!Double.TryParse(lengthAsString, out length)) {
                        length = Double.NaN;
                    }
                } else {
                    if(!Double.TryParse(lengthAsString, out length)) // Assuming pixels
                    {
                        length = Double.NaN;
                    }
                }
            }

            return !Double.IsNaN(length);
        }

        // .................................................................
        //
        // Pasring Color Attribute
        //
        // .................................................................

        private static string GetColorValue(string colorValue) {
            // TODO: Implement color conversion
            return colorValue;
        }

        /// <summary>
        /// Applies properties to xamlTableCellElement based on the html td element it is converted from.
        /// </summary>
        /// <param name="htmlChildNode">
        /// Html td/th element to be converted to xaml
        /// </param>
        /// <param name="xamlTableCellElement">
        /// XmlElement representing Xaml element for which properties are to be processed
        /// </param>
        /// <remarks>
        /// TODO: Use the processed properties for htmlChildNode instead of using the node itself
        /// </remarks>
        private static void ApplyPropertiesToTableCellElement(XmlElement htmlChildNode, XmlElement xamlTableCellElement) {
            // Parameter validation
            Debug.Assert(htmlChildNode.LocalName.ToLower() == "td" || htmlChildNode.LocalName.ToLower() == "th");
            Debug.Assert(xamlTableCellElement.LocalName == Xaml_TableCell);

            // set default border thickness for xamlTableCellElement to enable gridlines
            xamlTableCellElement.SetAttribute(Xaml_TableCell_BorderThickness, "1,1,1,1");
            xamlTableCellElement.SetAttribute(Xaml_TableCell_BorderBrush, Xaml_Brushes_Black);
            string rowSpanString = GetAttribute((XmlElement)htmlChildNode, "rowspan");
            if(rowSpanString != null) {
                xamlTableCellElement.SetAttribute(Xaml_TableCell_RowSpan, rowSpanString);
            }
        }

        #endregion Private Methods

        // ----------------------------------------------------------------
        //
        // Internal Constants
        //
        // ----------------------------------------------------------------

        // The constants reprtesent all Xaml names used in a conversion
        public const string Xaml_FlowDocument = "FlowDocument";

        public const string Xaml_Run = "Run";
        public const string Xaml_Span = "Span";
        public const string Xaml_Hyperlink = "Hyperlink";
        public const string Xaml_Hyperlink_NavigateUri = "NavigateUri";
        public const string Xaml_Hyperlink_TargetName = "TargetName";

        public const string Xaml_Section = "Section";

        public const string Xaml_List = "List";

        public const string Xaml_List_MarkerStyle = "MarkerStyle";
        public const string Xaml_List_MarkerStyle_None = "None";
        public const string Xaml_List_MarkerStyle_Decimal = "Decimal";
        public const string Xaml_List_MarkerStyle_Disc = "Disc";
        public const string Xaml_List_MarkerStyle_Circle = "Circle";
        public const string Xaml_List_MarkerStyle_Square = "Square";
        public const string Xaml_List_MarkerStyle_Box = "Box";
        public const string Xaml_List_MarkerStyle_LowerLatin = "LowerLatin";
        public const string Xaml_List_MarkerStyle_UpperLatin = "UpperLatin";
        public const string Xaml_List_MarkerStyle_LowerRoman = "LowerRoman";
        public const string Xaml_List_MarkerStyle_UpperRoman = "UpperRoman";

        public const string Xaml_ListItem = "ListItem";

        public const string Xaml_LineBreak = "LineBreak";

        public const string Xaml_Paragraph = "Paragraph";

        public const string Xaml_Margin = "Margin";
        public const string Xaml_Padding = "Padding";
        public const string Xaml_BorderBrush = "BorderBrush";
        public const string Xaml_BorderThickness = "BorderThickness";

        public const string Xaml_Table = "Table";

        public const string Xaml_TableColumn = "TableColumn";
        public const string Xaml_TableRowGroup = "TableRowGroup";
        public const string Xaml_TableRow = "TableRow";

        public const string Xaml_TableCell = "TableCell";
        public const string Xaml_TableCell_BorderThickness = "BorderThickness";
        public const string Xaml_TableCell_BorderBrush = "BorderBrush";

        public const string Xaml_TableCell_ColumnSpan = "ColumnSpan";
        public const string Xaml_TableCell_RowSpan = "RowSpan";

        public const string Xaml_Width = "Width";
        public const string Xaml_Brushes_Black = "Black";
        public const string Xaml_FontFamily = "FontFamily";

        public const string Xaml_FontSize = "FontSize";
        public const string Xaml_FontSize_XXLarge = "22pt"; // "XXLarge";
        public const string Xaml_FontSize_XLarge = "20pt"; // "XLarge";
        public const string Xaml_FontSize_Large = "18pt"; // "Large";
        public const string Xaml_FontSize_Medium = "16pt"; // "Medium";
        public const string Xaml_FontSize_Small = "12pt"; // "Small";
        public const string Xaml_FontSize_XSmall = "10pt"; // "XSmall";
        public const string Xaml_FontSize_XXSmall = "8pt"; // "XXSmall";

        public const string Xaml_FontWeight = "FontWeight";
        public const string Xaml_FontWeight_Bold = "Bold";

        public const string Xaml_FontStyle = "FontStyle";

        public const string Xaml_Foreground = "Foreground";
        public const string Xaml_Background = "Background";
        public const string Xaml_TextDecorations = "TextDecorations";
        public const string Xaml_TextDecorations_Underline = "Underline";

        public const string Xaml_TextIndent = "TextIndent";
        public const string Xaml_TextAlignment = "TextAlignment";

        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------

        #region Private Fields

        static string _xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        #endregion Private Fields
    }

    /// <summary>
    /// HtmlFromXamlConverter is a static class that takes an XAML string
    /// and converts it into HTML
    /// </summary>
    public static class HtmlFromXamlConverter {
        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------

        #region Internal Methods

        /// <summary>
        /// Main entry point for Xaml-to-Html converter.
        /// Converts a xaml string into html string.
        /// </summary>
        /// <param name="xamlString">
        /// Xaml strinng to convert.
        /// </param>
        /// <returns>
        /// Html string produced from a source xaml.
        /// </returns>
        internal static string ConvertXamlToHtml(string xamlString) {
            XmlTextReader xamlReader;
            StringBuilder htmlStringBuilder;
            XmlTextWriter htmlWriter;

            xamlReader = new XmlTextReader(new StringReader(xamlString));

            htmlStringBuilder = new StringBuilder(100);
            htmlWriter = new XmlTextWriter(new StringWriter(htmlStringBuilder));

            if(!WriteFlowDocument(xamlReader, htmlWriter)) {
                return "";
            }

            string htmlString = htmlStringBuilder.ToString();

            return htmlString;
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Private Methods
        //
        // ---------------------------------------------------------------------

        #region Private Methods
        /// <summary>
        /// Processes a root level element of XAML (normally it's FlowDocument element).
        /// </summary>
        /// <param name="xamlReader">
        /// XmlTextReader for a source xaml.
        /// </param>
        /// <param name="htmlWriter">
        /// XmlTextWriter producing resulting html
        /// </param>
        private static bool WriteFlowDocument(XmlTextReader xamlReader, XmlTextWriter htmlWriter) {
            if(!ReadNextToken(xamlReader)) {
                // Xaml content is empty - nothing to convert
                return false;
            }

            if(xamlReader.NodeType != XmlNodeType.Element || xamlReader.Name != "FlowDocument") {
                // Root FlowDocument elemet is missing
                return false;
            }

            // Create a buffer StringBuilder for collecting css properties for inline STYLE attributes
            // on every element level (it will be re-initialized on every level).
            StringBuilder inlineStyle = new StringBuilder();

            htmlWriter.WriteStartElement("HTML");
            htmlWriter.WriteStartElement("BODY");

            WriteFormattingProperties(xamlReader, htmlWriter, inlineStyle);

            WriteElementContent(xamlReader, htmlWriter, inlineStyle);

            htmlWriter.WriteEndElement();
            htmlWriter.WriteEndElement();

            return true;
        }

        /// <summary>
        /// Reads attributes of the current xaml element and converts
        /// them into appropriate html attributes or css styles.
        /// </summary>
        /// <param name="xamlReader">
        /// XmlTextReader which is expected to be at XmlNodeType.Element
        /// (opening element tag) position.
        /// The reader will remain at the same level after function complete.
        /// </param>
        /// <param name="htmlWriter">
        /// XmlTextWriter for output html, which is expected to be in
        /// after WriteStartElement state.
        /// </param>
        /// <param name="inlineStyle">
        /// String builder for collecting css properties for inline STYLE attribute.
        /// </param>
        private static void WriteFormattingProperties(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle) {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            // Clear string builder for the inline style
            inlineStyle.Remove(0, inlineStyle.Length);

            if(!xamlReader.HasAttributes) {
                return;
            }

            bool borderSet = false;

            while(xamlReader.MoveToNextAttribute()) {
                string css = null;

                switch(xamlReader.Name) {
                    // Character fomatting properties
                    // ------------------------------
                    case "Background":
                        css = "background-color:" + ParseXamlColor(xamlReader.Value) + ";";
                        break;
                    case "FontFamily":
                        css = "font-family:" + xamlReader.Value + ";";
                        break;
                    case "FontStyle":
                        css = "font-style:" + xamlReader.Value.ToLower() + ";";
                        break;
                    case "FontWeight":
                        css = "font-weight:" + xamlReader.Value.ToLower() + ";";
                        break;
                    case "FontStretch":
                        break;
                    case "FontSize":
                        css = "font-size:" + xamlReader.Value + ";";
                        break;
                    case "Foreground":
                        css = "color:" + ParseXamlColor(xamlReader.Value) + ";";
                        break;
                    case "TextDecorations":
                        css = "text-decoration:underline;";
                        break;
                    case "TextEffects":
                        break;
                    case "Emphasis":
                        break;
                    case "StandardLigatures":
                        break;
                    case "Variants":
                        break;
                    case "Capitals":
                        break;
                    case "Fraction":
                        break;

                    // Paragraph formatting properties
                    // -------------------------------
                    case "Padding":
                        css = "padding:" + ParseXamlThickness(xamlReader.Value) + ";";
                        break;
                    case "Margin":
                        css = "margin:" + ParseXamlThickness(xamlReader.Value) + ";";
                        break;
                    case "BorderThickness":
                        css = "border-width:" + ParseXamlThickness(xamlReader.Value) + ";";
                        borderSet = true;
                        break;
                    case "BorderBrush":
                        css = "border-color:" + ParseXamlColor(xamlReader.Value) + ";";
                        borderSet = true;
                        break;
                    case "LineHeight":
                        break;
                    case "TextIndent":
                        css = "text-indent:" + xamlReader.Value + ";";
                        break;
                    case "TextAlignment":
                        css = "text-align:" + xamlReader.Value + ";";
                        break;
                    case "IsKeptTogether":
                        break;
                    case "IsKeptWithNext":
                        break;
                    case "ColumnBreakBefore":
                        break;
                    case "PageBreakBefore":
                        break;
                    case "FlowDirection":
                        break;

                    // Table attributes
                    // ----------------
                    case "Width":
                        css = "width:" + xamlReader.Value + ";";
                        break;
                    case "ColumnSpan":
                        htmlWriter.WriteAttributeString("COLSPAN", xamlReader.Value);
                        break;
                    case "RowSpan":
                        htmlWriter.WriteAttributeString("ROWSPAN", xamlReader.Value);
                        break;
                }

                if(css != null) {
                    inlineStyle.Append(css);
                }
            }

            if(borderSet) {
                inlineStyle.Append("border-style:solid;mso-element:para-border-div;");
            }

            // Return the xamlReader back to element level
            xamlReader.MoveToElement();
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);
        }

        private static string ParseXamlColor(string color) {
            if(color.StartsWith("#")) {
                // Remove transparancy value
                color = "#" + color.Substring(3);
            }
            return color;
        }

        private static string ParseXamlThickness(string thickness) {
            string[] values = thickness.Split(',');

            for(int i = 0; i < values.Length; i++) {
                double value;
                if(double.TryParse(values[i], out value)) {
                    values[i] = Math.Ceiling(value).ToString();
                } else {
                    values[i] = "1";
                }
            }

            string cssThickness;
            switch(values.Length) {
                case 1:
                    cssThickness = thickness;
                    break;
                case 2:
                    cssThickness = values[1] + " " + values[0];
                    break;
                case 4:
                    cssThickness = values[1] + " " + values[2] + " " + values[3] + " " + values[0];
                    break;
                default:
                    cssThickness = values[0];
                    break;
            }

            return cssThickness;
        }

        /// <summary>
        /// Reads a content of current xaml element, converts it
        /// </summary>
        /// <param name="xamlReader">
        /// XmlTextReader which is expected to be at XmlNodeType.Element
        /// (opening element tag) position.
        /// </param>
        /// <param name="htmlWriter">
        /// May be null, in which case we are skipping the xaml element;
        /// witout producing any output to html.
        /// </param>
        /// <param name="inlineStyle">
        /// StringBuilder used for collecting css properties for inline STYLE attribute.
        /// </param>
        private static void WriteElementContent(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle) {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            bool elementContentStarted = false;

            if(xamlReader.IsEmptyElement) {
                if(htmlWriter != null && !elementContentStarted && inlineStyle.Length > 0) {
                    // Output STYLE attribute and clear inlineStyle buffer.
                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                    inlineStyle.Remove(0, inlineStyle.Length);
                }
                elementContentStarted = true;
            } else {
                while(ReadNextToken(xamlReader) && xamlReader.NodeType != XmlNodeType.EndElement) {
                    switch(xamlReader.NodeType) {
                        case XmlNodeType.Element:
                            if(xamlReader.Name.Contains(".")) {
                                AddComplexProperty(xamlReader, inlineStyle);
                            } else {
                                if(htmlWriter != null && !elementContentStarted && inlineStyle.Length > 0) {
                                    // Output STYLE attribute and clear inlineStyle buffer.
                                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                                    inlineStyle.Remove(0, inlineStyle.Length);
                                }
                                elementContentStarted = true;
                                WriteElement(xamlReader, htmlWriter, inlineStyle);
                            }
                            Debug.Assert(xamlReader.NodeType == XmlNodeType.EndElement || xamlReader.NodeType == XmlNodeType.Element && xamlReader.IsEmptyElement);
                            break;
                        case XmlNodeType.Comment:
                            if(htmlWriter != null) {
                                if(!elementContentStarted && inlineStyle.Length > 0) {
                                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                                }
                                htmlWriter.WriteComment(xamlReader.Value);
                            }
                            elementContentStarted = true;
                            break;
                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                            if(htmlWriter != null) {
                                if(!elementContentStarted && inlineStyle.Length > 0) {
                                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                                }
                                htmlWriter.WriteString(xamlReader.Value);
                            }
                            elementContentStarted = true;
                            break;
                    }
                }

                Debug.Assert(xamlReader.NodeType == XmlNodeType.EndElement);
            }
        }

        /// <summary>
        /// Conberts an element notation of complex property into
        /// </summary>
        /// <param name="xamlReader">
        /// On entry this XmlTextReader must be on Element start tag;
        /// on exit - on EndElement tag.
        /// </param>
        /// <param name="inlineStyle">
        /// StringBuilder containing a value for STYLE attribute.
        /// </param>
        private static void AddComplexProperty(XmlTextReader xamlReader, StringBuilder inlineStyle) {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            if(inlineStyle != null && xamlReader.Name.EndsWith(".TextDecorations")) {
                inlineStyle.Append("text-decoration:underline;");
            }

            // Skip the element representing the complex property
            WriteElementContent(xamlReader, /*htmlWriter:*/null, /*inlineStyle:*/null);
        }

        /// <summary>
        /// Converts a xaml element into an appropriate html element.
        /// </summary>
        /// <param name="xamlReader">
        /// On entry this XmlTextReader must be on Element start tag;
        /// on exit - on EndElement tag.
        /// </param>
        /// <param name="htmlWriter">
        /// May be null, in which case we are skipping xaml content
        /// without producing any html output
        /// </param>
        /// <param name="inlineStyle">
        /// StringBuilder used for collecting css properties for inline STYLE attributes on every level.
        /// </param>
        private static void WriteElement(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle) {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            if(htmlWriter == null) {
                // Skipping mode; recurse into the xaml element without any output
                WriteElementContent(xamlReader, /*htmlWriter:*/null, null);
            } else {
                string htmlElementName = null;

                switch(xamlReader.Name) {
                    case "Run":
                    case "Span":
                        htmlElementName = "SPAN";
                        break;
                    case "InlineUIContainer":
                        htmlElementName = "SPAN";
                        break;
                    case "Bold":
                        htmlElementName = "B";
                        break;
                    case "Italic":
                        htmlElementName = "I";
                        break;
                    case "Paragraph":
                        htmlElementName = "P";
                        break;
                    case "BlockUIContainer":
                        htmlElementName = "DIV";
                        break;
                    case "Section":
                        htmlElementName = "DIV";
                        break;
                    case "Table":
                        htmlElementName = "TABLE";
                        break;
                    case "TableColumn":
                        htmlElementName = "COL";
                        break;
                    case "TableRowGroup":
                        htmlElementName = "TBODY";
                        break;
                    case "TableRow":
                        htmlElementName = "TR";
                        break;
                    case "TableCell":
                        htmlElementName = "TD";
                        break;
                    case "List":
                        string marker = xamlReader.GetAttribute("MarkerStyle");
                        if(marker == null || marker == "None" || marker == "Disc" || marker == "Circle" || marker == "Square" || marker == "Box") {
                            htmlElementName = "UL";
                        } else {
                            htmlElementName = "OL";
                        }
                        break;
                    case "ListItem":
                        htmlElementName = "LI";
                        break;
                    default:
                        htmlElementName = null; // Ignore the element
                        break;
                }

                if(htmlWriter != null && htmlElementName != null) {
                    htmlWriter.WriteStartElement(htmlElementName);

                    WriteFormattingProperties(xamlReader, htmlWriter, inlineStyle);

                    WriteElementContent(xamlReader, htmlWriter, inlineStyle);

                    htmlWriter.WriteEndElement();
                } else {
                    // Skip this unrecognized xaml element
                    WriteElementContent(xamlReader, /*htmlWriter:*/null, null);
                }
            }
        }

        // Reader advance helpers
        // ----------------------

        /// <summary>
        /// Reads several items from xamlReader skipping all non-significant stuff.
        /// </summary>
        /// <param name="xamlReader">
        /// XmlTextReader from tokens are being read.
        /// </param>
        /// <returns>
        /// True if new token is available; false if end of stream reached.
        /// </returns>
        private static bool ReadNextToken(XmlReader xamlReader) {
            while(xamlReader.Read()) {
                Debug.Assert(xamlReader.ReadState == ReadState.Interactive, "Reader is expected to be in Interactive state (" + xamlReader.ReadState + ")");
                switch(xamlReader.NodeType) {
                    case XmlNodeType.Element:
                    case XmlNodeType.EndElement:
                    case XmlNodeType.None:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace:
                        return true;

                    case XmlNodeType.Whitespace:
                        if(xamlReader.XmlSpace == XmlSpace.Preserve) {
                            return true;
                        }
                        // ignore insignificant whitespace
                        break;

                    case XmlNodeType.EndEntity:
                    case XmlNodeType.EntityReference:
                        //  Implement entity reading
                        //xamlReader.ResolveEntity();
                        //xamlReader.Read();
                        //ReadChildNodes( parent, parentBaseUri, xamlReader, positionInfo);
                        break; // for now we ignore entities as insignificant stuff

                    case XmlNodeType.Comment:
                        return true;
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.DocumentType:
                    case XmlNodeType.XmlDeclaration:
                    default:
                        // Ignorable stuff
                        break;
                }
            }
            return false;
        }

        #endregion Private Methods

        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------

        #region Private Fields

        #endregion Private Fields
    }

}
