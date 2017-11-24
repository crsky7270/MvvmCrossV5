using System;
namespace MvxForms.Core
{
	public static class CommonUtils
	{
		private static long lastClickTime = 0;
		/// <summary>
		/// 是否点击太快，2秒之内为太快
		/// </summary>
		/// <returns></returns>
		public static bool isFastDoubleClick()
		{
			long time = DateTime.Now.Ticks / 10000;
			long betweentime = time - lastClickTime;
			if (betweentime > 0 && betweentime < 2000)
			{
				return true;
			}
			lastClickTime = time;
			return false;
		}
	}
}
