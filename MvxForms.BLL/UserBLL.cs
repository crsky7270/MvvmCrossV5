using System;
using MvxForms.IBLL;

namespace MvxForms.BLL
{
	public class UserBLL : IUserBLL
	{
		public UserBLL()
		{
		}

		public string GetUserNameById(int uid)
		{
			return uid == 1 ? "Crsky" : "Guest";
		}
	}
}
