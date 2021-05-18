using System.Collections.Generic;
using DynaJson;

namespace SRNicoNico.Models {
    /// <summary>
    /// JsonObjectの拡張メソッド
    /// </summary>
    public static class JsonObjectExtension {

        /// <summary>
        /// Jsonの配列をstringのリストに変換する
        /// </summary>
        /// <param name="jsonObject">JsonObject</param>
        /// <returns>string配列</returns>
        public static IEnumerable<string> ToStringArray(this JsonObject jsonObject) {

            var list = new List<string>();
            foreach (var element in (dynamic)jsonObject) {

                list.Add(element);
            }
            return list;
        }


    }
}
