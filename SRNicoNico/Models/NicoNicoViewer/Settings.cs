using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Media;

using Codeplex.Data;
using Livet;
using Newtonsoft.Json;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class Settings : NotificationObject {

        public static Settings Instance = new Settings();

        private readonly string Dir;


        private readonly Dictionary<string, Type> TypePair;


        private Settings() {

            TypePair = new Dictionary<string, Type>();
            TypePair["Int32"] = typeof(int);
            TypePair["Boolean"] = typeof(bool);
            TypePair["String"] = typeof(string);
            TypePair["Single"] = typeof(float);
            TypePair["Uri"] = typeof(Uri);
            TypePair["FontFamily"] = typeof(FontFamily);
            TypePair["GridLength"] = typeof(GridLength);
            TypePair["DispatcherCollection<NGCommentEntry>"] = typeof(DispatcherCollection<NGCommentEntry>);

            Dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SRNicoNico\user.settings";

            Load();
        }

        private bool Loading = false;

        public void Save() {

            //ロード中に呼び出されたら困る
            if(Loading) {

                return;
            }

            //このクラスのタイプを取得
            var type = GetType();

            //%APPDATA%/SRNicoNico/user.settingsに保存する
            var properties = type.GetProperties();

            dynamic json = new DynamicJson();

            var list = new List<object>();

            foreach(var property in properties) {


                if(property.PropertyType.GenericTypeArguments.Length != 0) {

                    string generic = "<";
                    foreach(var types in property.PropertyType.GenericTypeArguments) {

                        generic += types.Name + ", ";
                    }
                    generic = generic.Substring(0, generic.Length - 2) + ">";
                    string st = property.PropertyType.Name;
                    st = st.Split('`')[0] + generic;

                    list.Add(new { Name = property.Name, Type = st, Value = GetNeedValue(st, property) });
                } else {

                    list.Add(new { Name = property.Name, Type = property.PropertyType.Name, Value = GetNeedValue(property.PropertyType.Name, property) });
                }
            }
            json.settings = list;
            var s = Format(json.ToString());

            var fi = new StreamWriter(Dir);
            fi.AutoFlush = true;
            fi.Write(s);
            fi.Close();

        }

        //設定ファイルに保存する値を各クラスから指定する
        private object GetNeedValue(string name, PropertyInfo property) {

            var value = property.GetValue(this);

            switch(name) {
                case "FontFamily":
                    return ((FontFamily)value).Source;
                case "GridLength":
                    return ((GridLength)value).ToString();
                case "Uri":
                    return ((Uri)value).OriginalString;
                case "DispatcherCollection<NGCommentEntry>":

                    return value;
                default:
                    return value;
            }
        }

        //設定ファイルの値からインスタンスを生成する
        private object GetNeedValue(string name, PropertyInfo property, object value) {

            switch(name) {
                case "FontFamily":
                    return new FontFamily((string)value);
                case "GridLength":
                    return new GridLengthConverter().ConvertFrom(value);
                case "Uri":
                    return new Uri((string)value);
                case "DispatcherCollection<NGCommentEntry>":
                    var col = new DispatcherCollection<NGCommentEntry>(DispatcherHelper.UIDispatcher);

                    dynamic entries = (DynamicJson)value;

                    foreach(var entry in entries) {

                        ;
                    }


                    return col;
                default:
                    return value;
            }
        }


        //設定ファイルから各種プロパティにロードする
        public void Load() {

            Loading = true;

            //%APPDATA%に保存してある
            var info = new FileInfo(Dir);

            //ファイルが存在しなかったらすべてデフォルト値
            if(!info.Exists) {

                Loading = false;
                return;
            }

            //このクラスのタイプを取得
            var type = GetType();

            var reader = info.OpenText();
            var raw = DynamicJson.Parse(reader.ReadToEnd());
            reader.Close();

            foreach(var entry in raw.settings) {

                PropertyInfo property = type.GetProperty(entry.Name);
                property.SetValue(this, Convert.ChangeType(GetNeedValue(entry.Type, property, entry.Value), TypePair[entry.Type]));
            }

            Loading = false;


        }

        public void Reset() {

            //ファイルを完全に消す
            var fi = new FileInfo(Dir);
            if(fi.Exists) {

                fi.Delete();
            }

            //設定ファイルをデフォルト値で生成して反映させる
            Save();
            Load();
        }

        private string Format(string json) {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        #region Volume変更通知プロパティ
        private int _Volume = 100;

        public int Volume {
            get { return _Volume; }
            set {
                if(_Volume == value)
                    return;
                _Volume = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region CommentVisibility変更通知プロパティ
        private bool _CommentVisibility = true;

        public bool CommentVisibility {
            get { return _CommentVisibility; }
            set {
                if(_CommentVisibility == value)
                    return;
                _CommentVisibility = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region IsRepeat変更通知プロパティ
        private bool _IsRepeat = false;

        public bool IsRepeat {
            get { return _IsRepeat; }
            set {
                if(_IsRepeat == value)
                    return;
                _IsRepeat = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region VideoInfoPlacement変更通知プロパティ
        private string _VideoInfoPlacement = "Right";

        public string VideoInfoPlacement {
            get { return _VideoInfoPlacement; }
            set {
                if(_VideoInfoPlacement == value)
                    return;
                _VideoInfoPlacement = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region EnableTwitterLink変更通知プロパティ
        private bool _EnableTwitterLink = false;

        public bool EnableTwitterLink {
            get { return _EnableTwitterLink; }
            set {
                if(_EnableTwitterLink == value)
                    return;
                _EnableTwitterLink = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region EnableUrlLink変更通知プロパティ
        private bool _EnableUrlLink = true;

        public bool EnableUrlLink {
            get { return _EnableUrlLink; }
            set {
                if(_EnableUrlLink == value)
                    return;
                _EnableUrlLink = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region SearchIndex変更通知プロパティ
        private int _SearchIndex = 2;

        public int SearchIndex {
            get { return _SearchIndex; }
            set {
                if(_SearchIndex == value)
                    return;
                _SearchIndex = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region UserSelectedFont変更通知プロパティ
        private FontFamily _UserSelectedFont = new FontFamily("Yu Gothic UI");

        public FontFamily UserSelectedFont {
            get { return _UserSelectedFont; }
            set {
                if(_UserSelectedFont == value)
                    return;
                _UserSelectedFont = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region SplitterHeight変更通知プロパティ
        private GridLength _SplitterHeight = new GridLength(115, GridUnitType.Star);

        public GridLength SplitterHeight {
            get { return _SplitterHeight; }
            set {
                if(_SplitterHeight == value)
                    return;
                _SplitterHeight = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region AlwaysShowSeekBar変更通知プロパティ
        private bool _AlwaysShowSeekBar = false;

        public bool AlwaysShowSeekBar {
            get { return _AlwaysShowSeekBar; }
            set {
                if(_AlwaysShowSeekBar == value)
                    return;
                _AlwaysShowSeekBar = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region Use184変更通知プロパティ
        private bool _Use184 = false;

        public bool Use184 {
            get { return _Use184; }
            set {
                if(_Use184 == value)
                    return;
                _Use184 = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region Hide3DSComment変更通知プロパティ
        private bool _Hide3DSComment = false;

        public bool Hide3DSComment {
            get { return _Hide3DSComment; }
            set {
                if(_Hide3DSComment == value)
                    return;
                _Hide3DSComment = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region HideWiiUComment変更通知プロパティ
        private bool _HideWiiUComment = false;

        public bool HideWiiUComment {
            get { return _HideWiiUComment; }
            set {
                if(_HideWiiUComment == value)
                    return;
                _HideWiiUComment = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region NGSharedLevel変更通知プロパティ
        private string _NGSharedLevel = "無";

        public string NGSharedLevel {
            get { return _NGSharedLevel; }
            set {
                if(_NGSharedLevel == value)
                    return;
                _NGSharedLevel = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region CommentAlpha変更通知プロパティ
        private float _CommentAlpha = 80;

        public float CommentAlpha {
            get { return _CommentAlpha; }
            set {
                if(_CommentAlpha == value)
                    return;
                _CommentAlpha = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region CommentSize変更通知プロパティ
        private string _CommentSize = "標準";

        public string CommentSize {
            get { return _CommentSize; }
            set {
                if(_CommentSize == value)
                    return;
                _CommentSize = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region ClickOnPause変更通知プロパティ
        private bool _ClickOnPause = false;

        public bool ClickOnPause {
            get { return _ClickOnPause; }
            set {
                if(_ClickOnPause == value)
                    return;
                _ClickOnPause = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region UseWindowMode変更通知プロパティ
        private bool _UseWindowMode = false;

        public bool UseWindowMode {
            get { return _UseWindowMode; }
            set {
                if(_UseWindowMode == value)
                    return;
                _UseWindowMode = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region RefreshInterval変更通知プロパティ
        private int _RefreshInterval = 300000;

        public int RefreshInterval {
            get { return _RefreshInterval; }
            set {
                if(_RefreshInterval == value)
                    return;
                _RefreshInterval = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region RankingPageUrl変更通知プロパティ
        private Uri _RankingPageUrl = new Uri("http://www.nicovideo.jp/ranking");

        public Uri RankingPageUrl {
            get { return _RankingPageUrl; }
            set {
                if(_RankingPageUrl == value)
                    return;
                _RankingPageUrl = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion

        #region NGList変更通知プロパティ
        private DispatcherCollection<NGCommentEntry> _NGList = new DispatcherCollection<NGCommentEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NGCommentEntry> NGList {
            get { return _NGList; }
            set { 
                if(_NGList == value)
                    return;
                _NGList = value;
                RaisePropertyChanged();
            }
        }
        #endregion

    }
}