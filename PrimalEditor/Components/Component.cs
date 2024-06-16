﻿using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace PrimalEditor.Components
{
	interface IMSComponent { }

	[DataContract]
	abstract class Component : ViewModelBase
	{
		[DataMember]
		public GameEntity Owner { get; private set; }

		public abstract IMSComponent GetMultiselectionComponent(MSEntity msEntity);
		public abstract void WriteToBinary(BinaryWriter bw);

		public Component(GameEntity owner)
		{
			Debug.Assert(owner != null);
			Owner = owner;
		}
	}

	abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component
	{
		private bool _enableUpdates = true;
		public List<T> SelectedComponents { get; }

		protected abstract bool UpdateComponents(string propertyName);
		protected abstract bool UpdateMSComponent();

		public void Refresh()
		{
			_enableUpdates = false;
			UpdateMSComponent();
			_enableUpdates = true;
		}

		public MSComponent(MSEntity msEntity)
		{
			Debug.Assert(msEntity?.SelectedEntities?.Any() == true);
			SelectedComponents = msEntity.SelectedEntities.Select(entity => entity.GetComponent<T>()).ToList();
			PropertyChanged += (s, e) => { if (_enableUpdates) UpdateComponents(e.PropertyName); };
		}
	}
}
