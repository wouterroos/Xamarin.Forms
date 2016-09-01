using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Threading;
using System;
using Xamarin.Forms.Maps;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 39489, "Memory leak when using NavigationPage with Maps")]
	public class Bugzilla39489 : TestNavigationPage
	{
		protected override void Init()
		{
			PushAsync(new Bz39489Content());
		}

		protected override bool OnBackButtonPressed()
		{
			System.Diagnostics.Debug.WriteLine(">>>>>>>> Running Garbage Collection");
			GC.Collect();
			GC.WaitForPendingFinalizers();
			return base.OnBackButtonPressed();
		}
	}

	public class MyXFMap : Map
	{
		static int s_count;

		public MyXFMap()
		{
			Interlocked.Increment(ref s_count);
			System.Diagnostics.Debug.WriteLine($"++++++++ {nameof(MyXFMap)} : {s_count}");
		}

		~MyXFMap()
		{
			Interlocked.Decrement(ref s_count);
			System.Diagnostics.Debug.WriteLine($"++++++++ {nameof(MyXFMap)} : {s_count}");
		}
	}

	[Preserve(AllMembers = true)]
	public class Bz39489Content : ContentPage
	{
		static int s_count;

		private Map map;

		private Button button;

		public Bz39489Content()
		{
			Interlocked.Increment(ref s_count);
			System.Diagnostics.Debug.WriteLine($"++++++++ {nameof(Bz39489Content)} : {s_count}");

			button = new Button { Text = "New Page" };

			map = new Map();

			button.Clicked += Button_Clicked;

			Content = new StackLayout { Children = { button, map, } };
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new Bz39489Content());
			System.Diagnostics.Debug.WriteLine(">>>>>>>> Running Garbage Collection");
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		~Bz39489Content()
		{
			Interlocked.Decrement(ref s_count);
			System.Diagnostics.Debug.WriteLine($"-------- {nameof(Bz39489Content)} : {s_count}");
		}
	}
}