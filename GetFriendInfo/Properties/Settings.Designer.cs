﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GetFriendInfo.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://www2.kh.xxx.co.jp")]
        public string BoardServerURI {
            get {
                return ((string)(this["BoardServerURI"]));
            }
            set {
                this["BoardServerURI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"<!DOCTYPE html>
<html>
<head>
<meta charset=""UTF-8"">
<title>偽ボード</title>
<link rel=""stylesheet"" type=""text/css"" href=""http://www2.kh.xxx.co.jp/styles/wb2.css"" />
</head>
<body>
<table>")]
        public string HtmlHeader {
            get {
                return ((string)(this["HtmlHeader"]));
            }
            set {
                this["HtmlHeader"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"</table>
<br>
<table width=""100%"" align=""right"" border=""0"" cellspacing=""0"" cellpadding=""0"">
<tr><td class=""shubetsu"" nowrap>
<table border=""0"" cellspacing=""0"" cellpadding=""1"">
<tr valign=""middle"" align=""left"">
<td class=""shubetsu"" nowrap>&nbsp;情報種別</td>
<td class=""shubetsu"" rowspan=""3"" nowrap>&nbsp;</td>
<td class=""shubetsu"" nowrap>社外秘</td></tr>
<tr valign=""middle"" align=""left"">
<td class=""shubetsu"" nowrap>&nbsp;会社名</td>
<td class=""shubetsu"" nowrap>○○○○○○株式会社</td></tr>
<tr valign=""middle"" align=""left"">
<td class=""shubetsu"" nowrap>&nbsp;情報所有者</td>
<td class=""shubetsu"" nowrap>情報システム部</td></tr>
</table></td></tr></table>
<br>
</body>
</html>")]
        public string HtmlFooter {
            get {
                return ((string)(this["HtmlFooter"]));
            }
            set {
                this["HtmlFooter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("board.sqlite3")]
        public string DatabaseFile {
            get {
                return ((string)(this["DatabaseFile"]));
            }
            set {
                this["DatabaseFile"] = value;
            }
        }
    }
}