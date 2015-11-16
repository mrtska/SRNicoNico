using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using Codeplex.Data;

namespace SRNicoNico.Models.NicoNicoWrapper
{
    public class NicoNicoVitaUserApi : NotificationObject
    {

        //ユーザー情報取得API
        private const string UserDataApiUrl = "http://api.ce.nicovideo.jp/api/v1/user.info?__format=json&user_id=";


        public static NicoNicoVitaApiUserData GetUserData(string userId)
        {

            string result = NicoNicoWrapperMain.GetSession().GetAsync(UserDataApiUrl + userId).Result;

            var json = DynamicJson.Parse(result);
            var response = json.nicovideo_user_response;

            NicoNicoVitaApiUserData ret = new NicoNicoVitaApiUserData();

            ret.Id = response.user.id;
            ret.Name = response.user.nickname;
            ret.IconUrl = response.user.thumbnail_url;
            ret.UserSecret = int.Parse(response.vita_option.user_secret);
            
            return ret;
        }
    }

    public class NicoNicoVitaApiUserData : NotificationObject
    {

        //UserID
        public string Id { get; set; }

        //名前
        public string Name { get; set; }

        //ユーザーアイコン
        public string IconUrl { get; set; }

        //？
        public int UserSecret { get; set; }        
    }

}
