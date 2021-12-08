using System;
using System.Threading;
using MelonLoader;

namespace PendulumClient.Main
{
	public class VRCMod
	{
		public virtual string Name
		{
			get
			{
				return "Example Module";
			}
		}
		public virtual string Description
		{
			get
			{
				return "Example Description";
			}
		}
		public VRCMod()
		{
		}
		public virtual void OnStart()
		{
			new Thread(delegate ()
			{
				for (; ; )
				{
					Thread.Sleep(15000);
					this.OnUpdate();
				}
			})
			{
				IsBackground = true
			}.Start();
		}
		public virtual void OnUpdate()
		{
		}

		public virtual void OnModSettingsApplied()
        {
        }

		public virtual void OnLevelWasInitialized(int level)
        {
        }
	}
}
