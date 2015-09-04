using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Models.NicoNicoViewer {

    //コメントの位置を決定するクラス
    [Obsolete]
    public class CommentPositioner : NotificationObject {

        //コメント描画領域の横幅
        private double Width;

        //コメント描画領域の縦幅
        private double Height;

        private Random random = new Random();

        private readonly Dictionary<int, CommentEntry> Drawing;

        public CommentPositioner(double width, double height, Dictionary<int, CommentEntry> drawing) {

            Width = width;
            Height = height;
            Drawing = drawing;
        }

        public void UpdateSize(double width, double height) {

            Width = width;
            Height = height;
        }


        //現在のvposからそのコメントのX座標を取得する
        public double GetX(CommentEntry entry, double vpos) {

            //流れない系のコメントのX座標は一定なのでこれ
            if(entry.Pos.EnumPos != EnumCommentPosition.Naka) {

                return (Width - entry.Text.Width) / 2;
            }

            //差分
            double sub = vpos - entry.Raw.Vpos;

            double ret = (Width + entry.Text.Width) / (entry.Raw.Vend - entry.Raw.Vpos);

            return Width - sub * ret;
            
        }

        //現在描画されているコメントの一覧(Dictionary)から指定のコメントのY座標を取得する
        public double GetY(CommentEntry entry) {


            bool flag = false;

            //Y座標
			double offsetY = 0;

            //下コメだったら現在の高さからコメントの高さを引けばいいよね
			if(entry.Pos.EnumPos == EnumCommentPosition.Shita) {
				
				offsetY = Height - entry.Text.Height;
			}

            do {

                flag = false;
                int count = 0;

                if(count > Drawing.Count) {

                    break;
                }


                //描画中のリストを走査して描画できるY座標を特定する
                foreach(KeyValuePair<int, CommentEntry> pair in Drawing) {

                    CommentEntry target = pair.Value;

                    //描画したいコメントと同じだったらやり直し
                    if(entry.Raw.No == target.Raw.No) {

                        continue;
                    }
                    

                    //同じだったら
                    if(entry.Pos.EnumPos == target.Pos.EnumPos) {


                        //候補のY座標がすでに使われていたら
                        if(target.Pos.YPos + target.Text.Height > offsetY) {

                            //ターゲットよりも下に描画する必要があるからoffsetYを変更する
                            if(offsetY + entry.Text.Height > target.Pos.YPos) {

                                //下コメだったら
                                if(entry.Pos.EnumPos == EnumCommentPosition.Shita) {

                                    //すでに描画されているコメントの上に描画する
                                    offsetY = target.Pos.YPos - entry.Text.Height - 1;

                                    //描画出来る位置が無かったら仕方ないのでテキトーに位置に描画する
                                    if(offsetY < 0) {

                                        offsetY = random.Next() * (Height - entry.Text.Height);
                                        break;
                                    }
                                    flag = true;
                                    break;

                                }

                                //上コメだったら
                                if(entry.Pos.EnumPos == EnumCommentPosition.Ue) {


                                    //ターゲットより下に描画する
                                    offsetY = target.Pos.YPos + target.Text.Height + 1;

                                    //描画したい位置が下に突き抜けたら仕方ない
                                    if(offsetY + entry.Text.Height > Height) {

                                        offsetY = random.Next() * (Height - entry.Text.Height);
                                        break;
                                    }
                                    flag = true;
                                    break;
                                }

                                //中コメ
                                var max = Math.Max(entry.Raw.Vpos, target.Raw.Vpos);
                                var min = Math.Min(entry.Raw.Vend, target.Raw.Vend);
                                var x1 = GetX(entry, max);
                                var x2 = GetX(entry, min);
                                var x3 = GetX(target, max);
                                var x4 = GetX(target, min);

                                if(x1 <= x3 + target.Text.Width && x3 <= x1 + entry.Text.Width || x2 <= x4 + target.Text.Width && x4 <= x2 + entry.Text.Width) {

                                    offsetY = target.Pos.YPos + target.Text.Height + 1;

                                    if(offsetY + entry.Text.Height > Height) {

                                        offsetY = random.Next() * Height - entry.Text.Height;
                                        break;
                                    }
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                    count++;
                }
            } while(flag);

            return offsetY;
		}
    }
}
