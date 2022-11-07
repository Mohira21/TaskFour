using System.Drawing;
using TaskFour.Models;

namespace TaskFour.Services;

public class UserStore
{
    public Dictionary<string, User> Users { get; set; }

	public UserStore()
	{
		Users = new Dictionary<string, User>();
	}
}
