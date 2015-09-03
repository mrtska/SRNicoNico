using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using System.Timers;

using SRNicoNico.ViewModels;

using SRNicoNico.Models.NicoNicoWrapper;
using SRNicoNico.Views.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;

namespace SRNicoNico.Models.NicoNicoViewer {

    //コメント描画位置など
    public class CommentRasterizer : NotificationObject {


        private static Dictionary<string, Brush> NicoNicoColorMap = new Dictionary<string, Brush>();

        
        static CommentRasterizer() {

            //---全会員共通---
            NicoNicoColorMap["white"] = new SolidColorBrush(Colors.White);
            NicoNicoColorMap["red"] = new SolidColorBrush(Colors.Red);
            NicoNicoColorMap["pink"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x80, 0x80));
            NicoNicoColorMap["orange"] = new SolidColorBrush(Color.FromRgb(0xFF, 0xC0, 0x00));
            NicoNicoColorMap["yellow"] = new SolidColorBrush(Colors.Yellow);
            NicoNicoColorMap["green"] = new SolidColorBrush(Color.FromRgb(0x00, 0xFF, 0x00));
            NicoNicoColorMap["cyan"] = new SolidColorBrush(Colors.Cyan);
            NicoNicoColorMap["blue"] = new SolidColorBrush(Colors.Blue);
            NicoNicoColorMap["purple"] = new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0xFF));
            NicoNicoColorMap["black"] = new SolidColorBrush(Colors.Black);
            //------

            //---プレミアム会員のみ---
            NicoNicoColorMap["white2"] = new SolidColorBrush(Color.FromRgb(0xCC, 0xCC,0x99));
            NicoNicoColorMap["niconicowhite"] = NicoNicoColorMap["white2"];

            NicoNicoColorMap["red2"] = new SolidColorBrush(Color.FromRgb(0xCC, 0x00, 0x33));
            NicoNicoColorMap["truered"] = NicoNicoColorMap["red2"];

            NicoNicoColorMap["pink2"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x33, 0xCC));

            NicoNicoColorMap["orange2"] = new SolidColorBrush(Color.FromRgb(0xFF, 0x66, 0x00));
            NicoNicoColorMap["passionorange"] = NicoNicoColorMap["orange2"];

            NicoNicoColorMap["yellow2"] = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x00));
            NicoNicoColorMap["madyellow"] = NicoNicoColorMap["yellow2"];

            NicoNicoColorMap["green2"] = new SolidColorBrush(Color.FromRgb(0x00, 0xCC, 0x66));
            NicoNicoColorMap["elementalgreen"] = NicoNicoColorMap["green2"];

            NicoNicoColorMap["cyan2"] = new SolidColorBrush(Color.FromRgb(0x00, 0xCC, 0xCC));

            NicoNicoColorMap["blue2"] = new SolidColorBrush(Color.FromRgb(0x33, 0x99, 0xFF));
            NicoNicoColorMap["marineblue"] = NicoNicoColorMap["blue2"];

            NicoNicoColorMap["purple2"] = new SolidColorBrush(Color.FromRgb(0x66, 0x33, 0xCC));
            NicoNicoColorMap["nobleviolet"] = NicoNicoColorMap["purple2"];

            NicoNicoColorMap["black2"] = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            //------

        }




        //4秒かけてアニメーションする
        private Duration Duration = new Duration(TimeSpan.FromSeconds(4));
        private Duration FixedDuration = new Duration(TimeSpan.FromSeconds(3));




        private ObservableCollection<CommentEntryViewModel> CommentList;

        private readonly CommentView View;

        private CommentPositioner Positioner;

        private Dictionary<int, CommentEntry> Drawing;

        private int NextIndex = 0;

        public CommentRasterizer(CommentView view, ObservableCollection<CommentEntryViewModel> list, Dictionary<int, CommentEntry> drawing) {

            View = view;
            CommentList = list;

            Drawing = drawing;
            Positioner = new CommentPositioner(view.ActualWidth, view.ActualHeight, drawing);
            

        }

        //ウィンドウサイズ変更時
        public void UpdateSize(double width, double height) {

            Positioner.UpdateSize(width, height);
        }




        public void RenderComment(double vpos, bool forceReset) {

            

            for(int i = NextIndex; i < CommentList.Count; i++) {

                CommentEntryViewModel vm = CommentList[i];

                CommentEntry entry = new CommentEntry(vm.Entry);

                

                //コメントの位置、表示時間を決定する
                if(entry.Raw.Mail.Contains("ue")) {

                    entry.Pos.EnumPos = EnumCommentPosition.Ue;
                    entry.Raw.Duration = FixedDuration;

                } else if(entry.Raw.Mail.Contains("shita")) {

                    entry.Pos.EnumPos = EnumCommentPosition.Shita;
                    entry.Raw.Duration = FixedDuration;

                } else {

                    entry.Pos.EnumPos = EnumCommentPosition.Naka;
                    entry.Raw.Duration = Duration;
                }

                entry.Raw.Vend = entry.Raw.Vpos + entry.Raw.Duration.TimeSpan.Seconds * 100;
                
                if(vpos < entry.Raw.Vpos) {

                    NextIndex = i;
                    return;
                }

                if(vpos >= entry.Raw.Vpos && vpos < entry.Raw.Vend) {

                    if(!Drawing.Keys.Contains(entry.Raw.No)) {

                        TriggerComment(entry, vpos);
                    }
                   
                }







            }

        }

        

        //コメントを描画する
        public void TriggerComment(CommentEntry entry, double vpos) {





            //コメントカラー
            entry.Decoration.Color = new SolidColorBrush(Colors.White);
            foreach(KeyValuePair<string, Brush> pair in NicoNicoColorMap) {

                if(entry.Raw.Mail.Contains(pair.Key)) {

                    entry.Decoration.Color = pair.Value;
                    break;
                }
            }

            if(entry.Raw.Mail.Contains('#')) {

                string col = entry.Raw.Mail.Substring(entry.Raw.Mail.IndexOf('#'));

                //Convert.ToInt32(); :TODO
            }



            //コメントサイズ
            entry.Decoration.FontSize = 24;
            if(entry.Raw.Mail.Contains("big")) {

                //複数行コメントはbigが指定してあっても大きくしない
                if(!entry.Raw.Content.Contains("\n")) {

                    entry.Decoration.FontSize = 39;
                }

            } else if(entry.Raw.Mail.Contains("small")) {

                entry.Decoration.FontSize = 15;
            }

            double length = 0;
            //複数行コメントだったら
            if(entry.Raw.Content.Contains("\n")) {

                foreach(string co in entry.Raw.Content.Split('\n')) {

                    length = Math.Max(length, co.Length * entry.Decoration.FontSize);
                }
            } else {

                length = entry.Raw.Content.Length * entry.Decoration.FontSize;
            }



            TextBlock text = new TextBlock();
            text.Name = "No" + entry.Raw.No.ToString();
            text.Text = entry.Raw.Content;
            text.FontFamily = new FontFamily("Arial");
            text.FontWeight = FontWeights.Bold;
            text.FontSize = entry.Decoration.FontSize;
            text.Foreground = entry.Decoration.Color;
            text.Height = text.FontSize;
            text.Width = length;
            text.VerticalAlignment = VerticalAlignment.Top;
            text.HorizontalAlignment = HorizontalAlignment.Left;

            //テキストエフェクト いろいろ
            DropShadowEffect effect = new DropShadowEffect();
            effect.ShadowDepth = 3;
            effect.Direction = 315;
            effect.Color = Colors.BlueViolet;
            effect.Opacity = 0.5;
            effect.Freeze();
            text.Effect = effect;
            


            entry.Text = text;


            if(entry.Pos.EnumPos == EnumCommentPosition.Naka) {

                entry.Pos.XPos = View.ActualWidth;
            } else {

                entry.Pos.XPos = Positioner.GetX(entry, vpos);
            }

            entry.Pos.YPos = Positioner.GetY(entry);


           // Console.WriteLine();

            //DispatcherHelper.UIDispatcher.BeginInvoke(new Action(()  =>  View.DrawComment(entry)));
            View.DrawComment(entry);
            
        }
    }
}
