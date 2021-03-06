﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SRNicoNico.Models.NicoNicoWrapper;

namespace SRNicoNico.Services {
    /// <summary>
    /// ユーザーを操作する処理を提供するサービス
    /// </summary>
    public interface IUserService {

        /// <summary>
        /// 自分がフォローしているユーザーを返す
        /// </summary>
        /// <param name="page">ページ数</param>
        /// <returns>ユーザーリスト 総数が多い場合はpageを使って取得する</returns>
        /// <exception cref="Models.StatusErrorException">取得に失敗した場合</exception>
        Task<UserList> GetFollowedUsersAsync(int page = 1);




    }
}
