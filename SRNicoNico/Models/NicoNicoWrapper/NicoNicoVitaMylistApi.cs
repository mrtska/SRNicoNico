using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper
{

    public class NicoNicoVitaMylistApi : NotificationObject
    {

        //マイリスト情報取得API
        private const string MylistDataApiUrl = "http://api.ce.nicovideo.jp/nicoapi/v1/mylistgroup.get?__format=json&detail=";


        public static NicoNicoVitaApiMylistData GetMylistData(string mylistId)
        {
            string detail = "0"; //より細かい1も存在
            string result = NicoNicoWrapperMain.GetSession().GetAsync(MylistDataApiUrl + detail + "&group_id=" + mylistId).Result;

            var json = DynamicJson.Parse(result);
            var response = json.nicovideo_mylistgroup_response;

            NicoNicoVitaApiMylistData ret = new NicoNicoVitaApiMylistData();

            ret.Id = response.mylistgroup.id;
            ret.UserId = response.mylistgroup.user_id;
            ret.ViewCount = response.mylistgroup.view_counter;
            ret.Name = response.mylistgroup.name;
            ret.Description = response.mylistgroup.description;
            ret.isPublic = response.mylistgroup.@public == "1" ? true : false;
            ret.DefaultSort = int.Parse(response.mylistgroup.default_sort);
            ret.DefaultSortMethod = response.mylistgroup.default_sort_method;
            ret.SortOrder = int.Parse(response.mylistgroup.sort_order);
            ret.DefaultSortOrder = response.mylistgroup.default_sort_order;
            ret.IconId = response.mylistgroup.icon_id;
            ret.UpdateTime = NicoNicoUtil.DateFromVitaFormatDate(response.mylistgroup.update_time);
            ret.CreateTime = NicoNicoUtil.DateFromVitaFormatDate(response.mylistgroup.create_time);
            ret.Count = int.Parse(response.mylistgroup.count);

            NicoNicoVitaApiUserData re = NicoNicoVitaUserApi.GetUserData(ret.UserId);
            ret.UserName = re.Name;

            return ret;
        }
    }

    public class NicoNicoVitaApiMylistData : NotificationObject
    {

        //MylistID
        public string Id { get; set; }

        //UserID
        public string UserId { get; set; }

        //
        public string UserName { get; set; }

        //ViewCount
        public string ViewCount { get; set; }
        
        //名前
        public string Name { get; set; }

        //説明
        public string Description { get; set; }

        //公開かどうか
        public bool isPublic { get; set; }

        //既定並び替え順(数字/文字)
        public int DefaultSort { get; set; }
        public string DefaultSortMethod { get; set; }
        
        //既定並び替え順の昇順/降順(数字/文字)
        public int SortOrder { get; set; }
        public string DefaultSortOrder { get; set; }

        //？
        public string IconId { get; set; }

        //更新時間
        public string UpdateTime { get; set; }

        //作成時間
        public string CreateTime { get; set; }

        //？
        public int Count { get; set; }
    }

}
