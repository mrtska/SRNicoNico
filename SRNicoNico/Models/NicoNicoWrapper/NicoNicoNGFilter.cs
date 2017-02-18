using Codeplex.Data;
using Livet;
using Newtonsoft.Json;
using SRNicoNico.Models.NicoNicoViewer;
using SRNicoNico.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SRNicoNico.Models.NicoNicoWrapper {
    public class NicoNicoNGFilter : NotificationObject {


        #region NGList変更通知プロパティ
        private DispatcherCollection<NGCommentEntry> _NGList = new DispatcherCollection<NGCommentEntry>(DispatcherHelper.UIDispatcher);

        public DispatcherCollection<NGCommentEntry> NGList {
            get { return _NGList; }
            set { 
                if(_NGList == value)
                    return;
                _NGList = value;
                value.CollectionChanged += ((sender, e) => {

                    if(e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count != 0 && e.NewItems[0] is NGCommentEntry) {

                        var ng = e.NewItems[0] as NGCommentEntry;
                        ng.PropertyChanged += ((sdr, ee) => Save());
                        Save();
                    } else {

                        Save();
                    }
                });
                RaisePropertyChanged();
            }
        }
        #endregion


        //NG設定ファイルがあるディレクトリ
        private readonly string Dir;

        public NicoNicoNGFilter() {

            Dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\SRNicoNico\";
            Directory.CreateDirectory(Dir);

            Load();
        }

        public void AddNGEntry(NGCommentEntry entry) {

            NGList.Insert(0, entry);
            Save();
        }

        public void Save() {

            dynamic json = DynamicJson.Serialize(NGList);

            var s = Format(json.ToString());

            var fi = new StreamWriter(Dir + "nglist");
            fi.AutoFlush = true;
            fi.Write(s);
            fi.Close();
        }

        public void Load() {

            //設定ファイルが無かったらロードしない(出来ないわ)
            if(!File.Exists(Dir + "nglist")) {

                return;
            }

            var info = new FileInfo(Dir + "nglist");

            var reader = info.OpenText();
            var raw = DynamicJson.Parse(reader.ReadToEnd());
            reader.Close();

            NGList.Clear();

            foreach(var jsonentry in raw) {

                var entry = new NGCommentEntry();

                entry.IsEnabled = jsonentry.IsEnabled;
                entry.Type = Enum.Parse(typeof(NGType), jsonentry.Type);
                entry.Content = jsonentry.Content;

                entry.PropertyChanged += ((sender, e) => Save());

                NGList.Add(entry);
            }

        }


        //Jsonをいい感じにインデントする
        private string Format(string json) {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

    }

    public class NGCommentEntry : NotificationObject {

        //有効かどうか
        #region IsEnabled変更通知プロパティ
        private bool _IsEnabled;

        public bool IsEnabled {
            get { return _IsEnabled; }
            set {
                if(_IsEnabled == value)
                    return;
                _IsEnabled = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //ユーザーIDか文字列か
        #region Type変更通知プロパティ
        private NGType _Type;

        public NGType Type {
            get { return _Type; }
            set {
                if(_Type == value)
                    return;
                _Type = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        //NG文字列またはID
        #region Content変更通知プロパティ
        private string _Content;

        public string Content {
            get { return _Content; }
            set {
                if(_Content == value)
                    return;
                _Content = value;
                RaisePropertyChanged();
            }
        }
        #endregion
    }

    public enum NGType {

        UserId,
        Word,
        WordContains,
        Command,
        RegEx

    }
}
