using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRNicoNico.Views.Extentions {
	public static class DictionaryExtentions {

		//JavaのHashMapみたいな使い方がしたかった
		public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key) {

			TValue rtn;
			dic.TryGetValue(key, out rtn);
			return rtn;
		}


	}
}
