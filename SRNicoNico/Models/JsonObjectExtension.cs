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

        /// <summary>
        /// Jsonの配列をstringのリストに変換する
        /// </summary>
        /// <param name="jsonObject">JsonObject</param>
        /// <param name="defaultValues">デフォルト値として追加する値</param>
        /// <returns>string配列</returns>
        public static IEnumerable<string> ToStringArray(this JsonObject jsonObject, params string[] defaultValues) {

            var list = new List<string>();
            list.AddRange(defaultValues);

            foreach (var element in (dynamic)jsonObject) {

                list.Add(element);
            }
            return list;
        }

    }
}
