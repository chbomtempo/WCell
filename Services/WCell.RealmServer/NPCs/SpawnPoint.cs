using System;
using System.Collections.Generic;
using System.Linq;
using WCell.Constants.Spells;
using WCell.Constants.World;
using WCell.Core.Timers;
using WCell.RealmServer.Entities;
using WCell.RealmServer.Global;
using WCell.RealmServer.NPCs.Figurines;
using WCell.RealmServer.Gossips;
using WCell.Util.Data;
using WCell.Constants;
using WCell.RealmServer.Handlers;
using WCell.Constants.NPCs;
using WCell.Util.Graphics;

namespace WCell.RealmServer.NPCs
{
	/// <summary>
	/// Represents a SpawnPoint that continuesly spawns NPCs.
	/// 
	/// TODO: Change Timer to TimerEntry
	/// </summary>
	public class SpawnPoint : IWorldLocation
	{
		public static SpellId ConnectingSpell = SpellId.ClassSkillDrainLifeRank1;
			//SpellId.ClassSkillDrainMana;

		protected SpawnEntry m_spawnEntry;
		protected bool m_active;
		protected TimerEntry m_timer;
		protected int m_nextRespawn;
		protected ICollection<Unit> m_spawns;
		private SpawnPointMenu m_GossipMenu;


		/// <summary>
		/// Visual representation of this spawnpoint
		/// </summary>
		protected SpawnFigurine m_figurine;

		private LinkedList<WaypointFigurine> m_wpFigurines;

		public Region Region
		{
			get;
			private set;
		}

		public MapId RegionId
		{
			get { return Region.Id; }
		}

		public Vector3 Position
		{
			get { return m_spawnEntry.Position; }
		}

		public SpawnPoint(SpawnEntry entry, Region region)
		{
			m_spawns = new List<Unit>();
			m_timer = new TimerEntry { Action = Spawn };
			m_timer.Stop();
			Region = region;
			SpawnEntry = entry;
		}

		#region Properties
		public uint Id
		{
			get
			{
				return m_spawnEntry.SpawnId;
			}
			set
			{
				if (Id != m_spawnEntry.SpawnId)
				{
					SpawnEntry = NPCMgr.SpawnEntries[value];
				}
			}
		}

		public SpawnEntry SpawnEntry
		{
			get
			{
				return m_spawnEntry;
			}
			set
			{
				if (m_spawnEntry != value)
				{
					m_spawnEntry = value;
					//Figurine = new SpawnFigurine(this);
				}
			}
		}

		/// <summary>
		/// Whether this Spawn point is actively spawning.
		/// Requires region context.
		/// </summary>
		public bool Active
		{
			get
			{
				return m_active;
			}
			set
			{
				if (m_active != value)
				{
					m_active = value;
					if (m_active)
					{
						// activate

						if (m_spawns.Count == 0)
						{
							// first time start
							SpawnFull();
							//Reset(m_spawnEntry.RandomNormalDelay());
						}
						else if (m_spawns.Count < m_spawnEntry.MaxAmount)
						{
							var now = Environment.TickCount;
							if (now >= m_nextRespawn)
							{
								Spawn(0.0f);
							}
							else
							{
								Reset(m_nextRespawn - now);
							}
						}
					}
					else
					{
						// deactivate
						m_timer.Stop();
					}
				}
			}
		}
		#endregion

		#region Spawning
		/// <summary>
		/// Is called when the given spawn died or was removed from Region.
		/// </summary>
		protected internal void SignalSpawnlingDied(NPC npc)
		{
			m_spawns.Remove(npc);

			Reset(m_spawnEntry.GetRandomRespawnMillis());
		}


		/// <summary>
		/// Spawns NPCs until MaxAmount NPCs are spawned.
		/// Requires region context.
		/// </summary>
		public void SpawnFull()
		{
			for (int i = m_spawns.Count; i < m_spawnEntry.MaxAmount; i++)
			{
				SpawnOne();
			}
		}

		public void RespawnFull()
		{
			Clear();
			SpawnFull();
		}

		/// <summary>
		/// Restarts the spawn timer with the given delay
		/// </summary>
		public void Reset(int delay)
		{
			if (Active && !m_timer.IsRunning)
			{
				m_nextRespawn = Environment.TickCount + delay;
				m_timer.Start(delay, 0);
				Region.RegisterUpdatableLater(m_timer);
			}
		}

		/// <summary>
		/// Spawns a single NPC.
		/// Requires region context.
		/// </summary>
		public void SpawnOne()
		{
			var spawn = SpawnEntry.Entry.Create(this);
			var pos = m_spawnEntry.Position;

			m_spawns.Add(spawn);

			spawn.Position = pos;
			Region.AddObjectLater(spawn);
		}

		protected void Spawn(float dt)
		{
			SpawnOne();
			if (m_spawns.Count < m_spawnEntry.MaxAmount)
			{
				var delay = m_spawnEntry.GetRandomRespawnMillis();
				Reset(delay);
			}
			else
			{
				m_timer.Stop();
				Region.UnregisterUpdatableLater(m_timer);
			}
		}
		#endregion

		#region Figurines
		/// <summary>
		/// The visual representation of this SpawnPoint (or null)
		/// </summary>
		public SpawnFigurine Figurine
		{
			get
			{
				return m_figurine;
			}
			set
			{
				m_figurine = value;
				m_figurine.Position = m_spawnEntry.Position;
				Region.AddObjectLater(m_figurine);
			}
		}

		/// <summary>
		/// The list of currently active WaypointFigurines
		/// </summary>
		public LinkedList<WaypointFigurine> WPFigurines
		{
			get { return m_wpFigurines; }
		}

		/// <summary>
		/// Gets or sets whether figurines of this Spawnpoint are currently added
		/// </summary>
		public bool IsVisible
		{
			get
			{
				return m_figurine != null;
			}
			set
			{
				// Make sure old remainders (if any now get removed)
				if (IsVisible != value)
				{
					RemoveWPFigurines();
					if (value)
					{
						m_figurine = new SpawnFigurine(this);
						Region.AddObjectLater(m_figurine);
						AddWPFigurines();
					}
					else
					{
						if (m_figurine.IsInWorld)
						{
							m_figurine.Delete();
						}
						m_figurine = null;
					}
				}
			}
		}

		public void ToggleVisiblity()
		{
			IsVisible = !IsVisible;
		}

		public WaypointFigurine GetWPFigurine(WaypointEntry entry)
		{
			if (m_wpFigurines == null)
			{
				return null;
			}
			foreach (var fig in m_wpFigurines)
			{
				if (fig.Waypoint == entry)
				{
					return fig;
				}
			}
			return null;
		}

		public LinkedListNode<WaypointFigurine> GetWPFigurineNode(WaypointEntry entry)
		{
			if (m_wpFigurines == null)
			{
				return null;
			}
			var node = m_wpFigurines.First;
			while (node != null)
			{
				if (node.Value.Waypoint == entry)
				{
					return node;
				}
				node = node.Next;
			}
			return node;
		}

		private void AddWPFigurines()
		{
			Figurine last = m_figurine;
			m_wpFigurines = new LinkedList<WaypointFigurine>();
			foreach (var wp in SpawnEntry.Waypoints)
			{
				var figurine = new WaypointFigurine(this, wp);
				last.Orientation = last.GetAngleTowards(wp);
				last.ChannelObject = figurine;
				last.ChannelSpell = ConnectingSpell;

				last = figurine;
				m_wpFigurines.AddLast(figurine);
				Region.AddObjectLater(figurine);
			}
		}

		private void RemoveWPFigurines()
		{
			if (m_wpFigurines == null)
			{
				return;
			}

			foreach (var child in m_wpFigurines)
			{
				if (child.IsInWorld)
				{
					child.Delete();
				}
			}

			m_wpFigurines = null;
		}

		WaypointFigurine InsertFigurine(LinkedListNode<WaypointFigurine> last, WaypointEntry wp)
		{
			Figurine lastFig;
			Figurine nextFig;
			var newFig = new WaypointFigurine(this, wp);
			if (last == null)
			{
				// first WP
				lastFig = m_figurine;
				nextFig = m_wpFigurines.First.Value;
			}
			else
			{
				lastFig = last.Value;
				nextFig = last.Next.Value;
			}

			if (lastFig != null)
			{
				lastFig.SetOrientationTowards(newFig);
				lastFig.ChannelObject = newFig;
				lastFig.ChannelSpell = ConnectingSpell;
			}
			if (nextFig != null)
			{
				newFig.SetOrientationTowards(nextFig);
				newFig.ChannelObject = nextFig;
				newFig.ChannelSpell = ConnectingSpell;
			}

			m_wpFigurines.AddLast(newFig);
			Region.AddObjectLater(newFig);
			return newFig;
		}

		#endregion

		/// <summary>
		/// Removes all active Spawns from this SpawnPoint and makes it
		/// collectable by the GC (if not in Region anymore).
		/// </summary>
		/// <remarks>Requires region context</remarks>
		public void Clear()
		{
			var arr = m_spawns.ToArray();
			for (var i = 0; i < arr.Length; i++)
			{
				var spawn = arr[i];
				spawn.Delete();
			}
        }

        public void RemoveSpawnLater()
        {
            Region.AddMessage(() => RemoveSpawnNow());
        }

		public void RemoveSpawnNow()
		{
			if (Region != null)
			{
				Region.UnregisterUpdatable(m_timer);
				IsVisible = false;
				Active = false;
				Clear();
				Region.RemoveSpawn(this);
				Region = null;
			}
		}

		#region Spawn Menu Stuff
		/// <summary>
		/// Removes the given WP from this SpawnPoint's SpawnEntry
		/// </summary>
		/// <param name="wp"></param>
		internal void RemoveWP(WaypointEntry wp)
		{
			// remove from List
			m_spawnEntry.Waypoints.Remove(wp);

			if (m_GossipMenu != null)
			{
				// remove item from Gossip menu
				m_GossipMenu.RemoveWPItem(wp);
			}

			// figure out the figurines
			var figNode = GetWPFigurineNode(wp);
			if (figNode != null)
			{
				// delete
				figNode.Value.Delete();

				// update orientation
				var prev = figNode.Previous;
				figNode.List.Remove(figNode);
				if (prev != null)
				{
					var next = prev.Next;
					if (next != null)
					{
						prev.Value.Face(next.Value);
						prev.Value.ChannelObject = next.Value;
					}
					else
					{
						prev.Value.ChannelObject = null;
					}
				}
			}
		}

		/// <summary>
		/// Moves the WP to the given new Position
		/// </summary>
		/// <param name="wp"></param>
		internal void MoveWP(WaypointEntry wp, Vector3 pos)
		{
			wp.Position = pos;

			var node = GetWPFigurineNode(wp);
			if (node != null)
			{
				var fig = node.Value;

				// update orientations
				var next = node.Next;
				if (next != null)
				{
					fig.SetOrientationTowards(next.Value);
				}
				if (node.Previous != null)
				{
					node.Previous.Value.SetOrientationTowards(fig);
				}

				// move over
				MovementHandler.SendMoveToPacket(fig, ref pos, fig.Orientation, 1000, MonsterMoveFlags.DefaultMask);
				fig.TeleportTo(ref pos);
			}
		}

		/// <summary>
		/// Adds a new WP after the given oldWp or at the end if its new with 
		/// the given position and orientation.
		/// </summary>
		/// <param name="oldWp">May be null</param>
		internal WaypointEntry InsertAfter(WaypointEntry oldWp, Vector3 pos, float orientation)
		{
			var newWp = m_spawnEntry.CreateWP(pos, orientation);

			LinkedListNode<WaypointEntry> newNode;
			LinkedListNode<WaypointFigurine> oldFigNode;
			if (oldWp != null)
			{
				oldFigNode = GetWPFigurineNode(oldWp);
				var oldNode = oldWp.Node;
				var fig = oldNode.Value;
				newNode = oldNode.List.AddAfter(oldNode, newWp);
			}
			else
			{
				newNode = m_spawnEntry.Waypoints.AddLast(newWp);
				oldFigNode = null;
			}
			newWp.Node = newNode;

			var newFig = InsertFigurine(oldFigNode, newWp);
			newFig.Highlight();
			return newWp;
		}

		[NotPersistent]
		public SpawnPointMenu GossipMenu
		{
			get
			{
				if (m_GossipMenu == null)
				{
					m_GossipMenu = new SpawnPointMenu(this);
				}
				return m_GossipMenu;
			}
			set { m_GossipMenu = value; }
		}
		#endregion
	}
}