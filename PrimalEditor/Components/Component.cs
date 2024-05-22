using System.Runtime.Serialization;

namespace PrimalEditor.Components
{
	[DataContract]
	public class Component : ViewModelBase
	{
		[DataMember]
		public GameEntity Owner { get; private set; }

		public Component(GameEntity owner)
		{
			Debug.Assert(owner != null);
			Owner = owner;
		}
	}
}
