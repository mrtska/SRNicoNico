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
using System.Collections.Specialized;

namespace SRNicoNico.Models.NicoNicoViewer {
    public class Settings : NotificationObject {

        //インスタンス
        public static Settings Instance = new Settings();

        //設定ファイルがあるディレクトリ
        private readonly string Dir;

        //設定ファイルには型情報が文字列で保存されてるので文字からその型をルックアップできるようにするためのセット
        private readonly Dictionary<string, Type> TypePair;

        //汎用性なんてない
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

            //%APPDATA%/SRNicoNico/user.settings が設定ファイルの場所
            Dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SRNicoNico\";
            Directory.CreateDirectory(Dir);

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

                //ジェネリクスはちょっとややこしい
                //型名に<Generic>を追加する
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

            var fi = new StreamWriter(Dir + "user.settings");
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

                    //NGフィルターのエントリを設定ファイルからパースする
                    foreach(var entry in entries) {

                        var ng = new NGCommentEntry();
                        ng.Type = Enum.Parse(typeof(NGType), entry.Type);
                        ng.Content = entry.Content;
                        ng.IsEnabled = entry.IsEnabled;

                        //NGフィルターの内容が更新されたら即セーブする
                        ng.PropertyChanged += ((sender, e) => Save());

                        col.Add(ng);
                    }

                    //新しくフィルターリストにエントリが追加/削除されたらセーブする
                    //追加だったら新しいエントリに即セーブするイベントを設定
                    col.CollectionChanged += ((sender, e) => {

                        if(e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count != 0 && e.NewItems[0] is NGCommentEntry) {

                            var ng = e.NewItems[0] as NGCommentEntry;
                            ng.PropertyChanged += ((sdr, ee) => Save());
                            Save();
                        } else {

                            Save();
                        }
                    });

                    return col;
                default:
                    return value;
            }
        }


        //設定ファイルから各種プロパティにロードする
        public void Load() {

            Loading = true;

            //%APPDATA%に保存してある
            var info = new FileInfo(Dir + "user.settings");

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

                //無効な設定はスキップ
                if(property == null) {

                    continue;
                }

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
            //なんてことは出来ないので
            Save();
            Load();
        }

        //Jsonをいい感じにインデントする
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


        #region NGList変更通知プロパティ
        private DispatcherCollection<NGCommentEntry> _NGList = new DispatcherCollection<NGCommentEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NGCommentEntry> NGList {
            get { return _NGList; }
            set {
                if(_NGList == value)
                    return;
                _NGList = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingPeriod変更通知プロパティ
        private int _RankingPeriodIndex = 1;

        public int RankingPeriodIndex {
            get { return _RankingPeriodIndex; }
            set {
                if(_RankingPeriodIndex == value)
                    return;
                _RankingPeriodIndex = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingTarget変更通知プロパティ
        private int _RankingTargetIndex = 0;

        public int RankingTargetIndex {
            get { return _RankingTargetIndex; }
            set {
                if(_RankingTargetIndex == value)
                    return;
                _RankingTargetIndex = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingCategoryTotal変更通知プロパティ
        private bool _RankingCategoryTotal = true;

        public bool RankingCategoryTotal {
            get { return _RankingCategoryTotal; }
            set {
                if(_RankingCategoryTotal == value)
                    return;
                _RankingCategoryTotal = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingEntameMusic変更通知プロパティ
        private bool _RankingEntameMusic = false;

        public bool RankingEntameMusic {
            get { return _RankingEntameMusic; }
            set {
                if(_RankingEntameMusic == value)
                    return;
                _RankingEntameMusic = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingEntertainment変更通知プロパティ
        private bool _RankingEntertainment = false;

        public bool RankingEntertainment {
            get { return _RankingEntertainment; }
            set {
                if(_RankingEntertainment == value)
                    return;
                _RankingEntertainment = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingMusic変更通知プロパティ
        private bool _RankingMusic = false;

        public bool RankingMusic {
            get { return _RankingMusic; }
            set {
                if(_RankingMusic == value)
                    return;
                _RankingMusic = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingSingaSong変更通知プロパティ
        private bool _RankingSingaSong = false;

        public bool RankingSingaSong {
            get { return _RankingSingaSong; }
            set {
                if(_RankingSingaSong == value)
                    return;
                _RankingSingaSong = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingPlayaMusic変更通知プロパティ
        private bool _RankingPlayaMusic = false;

        public bool RankingPlayaMusic {
            get { return _RankingPlayaMusic; }
            set {
                if(_RankingPlayaMusic == value)
                    return;
                _RankingPlayaMusic = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingDancing変更通知プロパティ
        private bool _RankingDancing = false;

        public bool RankingDancing {
            get { return _RankingDancing; }
            set {
                if(_RankingDancing == value)
                    return;
                _RankingDancing = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingVOCALOID変更通知プロパティ
        private bool _RankingVOCALOID = false;

        public bool RankingVOCALOID {
            get { return _RankingVOCALOID; }
            set {
                if(_RankingVOCALOID == value)
                    return;
                _RankingVOCALOID = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingIndies変更通知プロパティ
        private bool _RankingIndies = false;

        public bool RankingIndies {
            get { return _RankingIndies; }
            set {
                if(_RankingIndies == value)
                    return;
                _RankingIndies = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingLifeSports変更通知プロパティ
        private bool _RankingLifeSports = true;

        public bool RankingLifeSports {
            get { return _RankingLifeSports; }
            set {
                if(_RankingLifeSports == value)
                    return;
                _RankingLifeSports = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingAnimal変更通知プロパティ
        private bool _RankingAnimal = false;

        public bool RankingAnimal {
            get { return _RankingAnimal; }
            set {
                if(_RankingAnimal == value)
                    return;
                _RankingAnimal = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingCooking変更通知プロパティ
        private bool _RankingCooking = false;

        public bool RankingCooking {
            get { return _RankingCooking; }
            set {
                if(_RankingCooking == value)
                    return;
                _RankingCooking = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingNature変更通知プロパティ
        private bool _RankingNature = false;

        public bool RankingNature {
            get { return _RankingNature; }
            set {
                if(_RankingNature == value)
                    return;
                _RankingNature = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingTravel変更通知プロパティ
        private bool _RankingTravel = false;

        public bool RankingTravel {
            get { return _RankingTravel; }
            set {
                if(_RankingTravel == value)
                    return;
                _RankingTravel = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingSports変更通知プロパティ
        private bool _RankingSports = false;

        public bool RankingSports {
            get { return _RankingSports; }
            set {
                if(_RankingSports == value)
                    return;
                _RankingSports = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingNicoNicoDougaLecture変更通知プロパティ
        private bool _RankingNicoNicoDougaLecture = false;

        public bool RankingNicoNicoDougaLecture {
            get { return _RankingNicoNicoDougaLecture; }
            set {
                if(_RankingNicoNicoDougaLecture == value)
                    return;
                _RankingNicoNicoDougaLecture = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingDriveVideo変更通知プロパティ
        private bool _RankingDriveVideo = false;

        public bool RankingDriveVideo {
            get { return _RankingDriveVideo; }
            set {
                if(_RankingDriveVideo == value)
                    return;
                _RankingDriveVideo = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingHistory変更通知プロパティ
        private bool _RankingHistory = false;

        public bool RankingHistory {
            get { return _RankingHistory; }
            set {
                if(_RankingHistory == value)
                    return;
                _RankingHistory = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingPolitics変更通知プロパティ
        private bool _RankingPolitics = true;

        public bool RankingPolitics {
            get { return _RankingPolitics; }
            set {
                if(_RankingPolitics == value)
                    return;
                _RankingPolitics = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingScienceTech変更通知プロパティ
        private bool _RankingScienceTech = true;

        public bool RankingScienceTech {
            get { return _RankingScienceTech; }
            set {
                if(_RankingScienceTech == value)
                    return;
                _RankingScienceTech = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingScience変更通知プロパティ
        private bool _RankingScience = false;

        public bool RankingScience {
            get { return _RankingScience; }
            set {
                if(_RankingScience == value)
                    return;
                _RankingScience = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingNicoNicoTech変更通知プロパティ
        private bool _RankingNicoNicoTech = false;

        public bool RankingNicoNicoTech {
            get { return _RankingNicoNicoTech; }
            set {
                if(_RankingNicoNicoTech == value)
                    return;
                _RankingNicoNicoTech = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingHandicraft変更通知プロパティ
        private bool _RankingHandicraft = false;

        public bool RankingHandicraft {
            get { return _RankingHandicraft; }
            set {
                if(_RankingHandicraft == value)
                    return;
                _RankingHandicraft = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingMaking変更通知プロパティ
        private bool _RankingMaking = false;

        public bool RankingMaking {
            get { return _RankingMaking; }
            set {
                if(_RankingMaking == value)
                    return;
                _RankingMaking = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingAnimeGameIllust変更通知プロパティ
        private bool _RankingAnimeGameIllust = true;

        public bool RankingAnimeGameIllust {
            get { return _RankingAnimeGameIllust; }
            set {
                if(_RankingAnimeGameIllust == value)
                    return;
                _RankingAnimeGameIllust = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingAnime変更通知プロパティ
        private bool _RankingAnime = false;

        public bool RankingAnime {
            get { return _RankingAnime; }
            set {
                if(_RankingAnime == value)
                    return;
                _RankingAnime = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingGame変更通知プロパティ
        private bool _RankingGame = false;

        public bool RankingGame {
            get { return _RankingGame; }
            set {
                if(_RankingGame == value)
                    return;
                _RankingGame = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingTouhou変更通知プロパティ
        private bool _RankingTouhou = false;

        public bool RankingTouhou {
            get { return _RankingTouhou; }
            set {
                if(_RankingTouhou == value)
                    return;
                _RankingTouhou = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingIdolmaster変更通知プロパティ
        private bool _RankingIdolmaster = false;

        public bool RankingIdolmaster {
            get { return _RankingIdolmaster; }
            set {
                if(_RankingIdolmaster == value)
                    return;
                _RankingIdolmaster = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingRadio変更通知プロパティ
        private bool _RankingRadio = false;

        public bool RankingRadio {
            get { return _RankingRadio; }
            set {
                if(_RankingRadio == value)
                    return;
                _RankingRadio = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingDrawing変更通知プロパティ
        private bool _RankingDrawing = false;

        public bool RankingDrawing {
            get { return _RankingDrawing; }
            set {
                if(_RankingDrawing == value)
                    return;
                _RankingDrawing = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingOtherTotal変更通知プロパティ
        private bool _RankingOtherTotal = true;

        public bool RankingOtherTotal {
            get { return _RankingOtherTotal; }
            set {
                if(_RankingOtherTotal == value)
                    return;
                _RankingOtherTotal = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingReinoAre変更通知プロパティ
        private bool _RankingReinoAre = false;

        public bool RankingReinoAre {
            get { return _RankingReinoAre; }
            set {
                if(_RankingReinoAre == value)
                    return;
                _RankingReinoAre = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingDiary変更通知プロパティ
        private bool _RankingDiary = false;

        public bool RankingDiary {
            get { return _RankingDiary; }
            set {
                if(_RankingDiary == value)
                    return;
                _RankingDiary = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region RankingOther変更通知プロパティ
        private bool _RankingOther = false;

        public bool RankingOther {
            get { return _RankingOther; }
            set {
                if(_RankingOther == value)
                    return;
                _RankingOther = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region WebViewDefaultPage変更通知プロパティ
        private string _WebViewDefaultPage = "http://www.nicovideo.jp/";

        public string WebViewDefaultPage {
            get { return _WebViewDefaultPage; }
            set { 
                if(_WebViewDefaultPage == value)
                    return;
                _WebViewDefaultPage = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion


        #region ConfirmExit変更通知プロパティ
        private bool _ConfirmExit = true;

        public bool ConfirmExit {
            get { return _ConfirmExit; }
            set { 
                if(_ConfirmExit == value)
                    return;
                _ConfirmExit = value;
                RaisePropertyChanged();
                Save();
            }
        }
        #endregion







    }
}
