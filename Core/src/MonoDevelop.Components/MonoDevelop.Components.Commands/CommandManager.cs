//
// CommandManager.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Reflection;
using System.Collections;

namespace MonoDevelop.Components.Commands
{
	public class CommandManager: IDisposable
	{
		public static CommandManager Main = new CommandManager ();
		Gtk.Window rootWidget;
		
		Hashtable cmds = new Hashtable ();
		Hashtable handlerInfo = new Hashtable ();
		ArrayList toolbars = new ArrayList ();
		ArrayList globalHandlers = new ArrayList ();
		ArrayList commandUpdateErrors = new ArrayList ();
		Hashtable overloadedAccelCommands = new Hashtable ();
		Stack delegatorStack = new Stack ();
		bool disposed;
		bool toolbarUpdaterRunning;
		bool enableToolbarUpdate;
		
		Gtk.AccelGroup accelGroup;
		
		public CommandManager (): this (null)
		{
		}
		
		public CommandManager (Gtk.Window root)
		{
			rootWidget = root;
			ActionCommand c = new ActionCommand (CommandSystemCommands.ToolbarList, "Toolbar List", null, null, ActionType.Check);
			c.CommandArray = true;
			RegisterCommand (c, "");
		}
		
		public void SetRootWindow (Gtk.Window root)
		{
			rootWidget = root;
			if (rootWidget.IsRealized)
				rootWidget.AddAccelGroup (AccelGroup);
			else
				rootWidget.Realized += new EventHandler (RootRealized);
		}
		
		public void Dispose ()
		{
			disposed = true;
		}
		
		public bool EnableIdleUpdate {
			get { return enableToolbarUpdate; }
			set {
				if (enableToolbarUpdate != value) {
					if (value) {
						if (toolbars.Count > 0) {
							if (!toolbarUpdaterRunning) {
								GLib.Timeout.Add (500, new GLib.TimeoutHandler (UpdateStatus));
								toolbarUpdaterRunning = true;
							}
						}
					} else {
						toolbarUpdaterRunning = false;
					}
					enableToolbarUpdate = value;
				}
			}
		}

		
		public void RegisterCommand (Command cmd)
		{
			RegisterCommand (cmd, "");
		}
		
		public void RegisterCommand (Command cmd, string category)
		{
			cmds [cmd.Id] = cmd;
			
			if (cmd.AccelKey != null && cmd.AccelKey != "") {
				// Register overloaded key bindings
				foreach (Command ac in cmds.Values) {
					if (ac.AccelKey == cmd.AccelKey) {
						ArrayList list = (ArrayList) overloadedAccelCommands [ac.AccelKey];
						if (list == null) {
							list = new ArrayList ();
							overloadedAccelCommands [ac.AccelKey] = list;
							list.Add (ac.Id);
						}
						list.Add (cmd.Id);
						break;
					}
				}
			}
		}
		
		public void RegisterGlobalHandler (object handler)
		{
			if (!globalHandlers.Contains (handler))
				globalHandlers.Add (handler);
		}
		
		public void UnregisterGlobalHandler (object handler)
		{
			globalHandlers.Remove (handler);
		}
		
		public Command GetCommand (object cmdId)
		{
			Command cmd = cmds [cmdId] as Command;
			if (cmd == null)
				throw new InvalidOperationException ("Invalid command id: " + cmdId);
			return cmd;
		}
		
		internal Command FindCommand (object cmdId)
		{
			return cmds [cmdId] as Command;
		}
		
		public ActionCommand GetActionCommand (object cmdId)
		{
			ActionCommand cmd = cmds [cmdId] as ActionCommand;
			if (cmd == null)
				throw new InvalidOperationException ("Invalid action command id: " + cmdId);
			return cmd;
		}
		
		public Gtk.MenuBar CreateMenuBar (string name, CommandEntrySet entrySet)
		{
			Gtk.MenuBar topMenu = new CommandMenuBar (this);
			foreach (CommandEntry entry in entrySet)
				topMenu.Append (entry.CreateMenuItem (this));
			return topMenu;
		}
		
/*		public Gtk.Toolbar CreateToolbar (CommandEntrySet entrySet)
		{
			return CreateToolbar ("", entrySet);
		}
		
*/		public Gtk.Menu CreateMenu (CommandEntrySet entrySet)
		{
			CommandMenu menu = new CommandMenu (this);
			foreach (CommandEntry entry in entrySet)
				menu.Append (entry.CreateMenuItem (this));
			return menu;
		}
		
		public void InsertOptions (Gtk.Menu menu, CommandEntrySet entrySet, int index)
		{
			foreach (CommandEntry entry in entrySet) {
				Gtk.MenuItem item = entry.CreateMenuItem (this);
				int n = menu.Children.Length;
				menu.Insert (item, index);
				if (item is ICommandUserItem)
					((ICommandUserItem)item).Update (null);
				else
					item.Show ();
				index += menu.Children.Length - n;
			}
		}
		
		public void ShowContextMenu (CommandEntrySet entrySet)
		{
			ShowContextMenu (entrySet, null);
		}
		
		public void ShowContextMenu (CommandEntrySet entrySet, object initialTarget)
		{
			CommandMenu menu = (CommandMenu) CreateMenu (entrySet);
			menu.InitialCommandTarget = initialTarget;
			ShowContextMenu (menu);
		}
		
		public void ShowContextMenu (Gtk.Menu menu)
		{
			menu.Popup (null, null, null, 0, Gtk.Global.CurrentEventTime);
		}
		
		public Gtk.Toolbar CreateToolbar (string id, CommandEntrySet entrySet)
		{
			CommandToolbar toolbar = new CommandToolbar (this, id, entrySet.Name);
			foreach (CommandEntry entry in entrySet)
				toolbar.Add (entry.CreateToolItem (this));
			return toolbar;
		}

		public bool DispatchCommand (object commandId)
		{
			return DispatchCommand (commandId, null, null);
		}
		
		public bool DispatchCommand (object commandId, object dataItem)
		{
			return DispatchCommand (commandId, dataItem, null);
		}

		public bool DispatchCommand (object commandId, object dataItem, object initialTarget)
		{
			ActionCommand cmd = null;
			try {
				cmd = GetActionCommand (commandId);
				
				int globalPos = -1;
				object cmdTarget = initialTarget != null ? initialTarget : GetFirstCommandTarget (out globalPos);
				CommandInfo info = new CommandInfo (cmd);
				
				while (cmdTarget != null)
				{
					HandlerTypeInfo typeInfo = GetTypeHandlerInfo (cmdTarget);
					
					CommandUpdaterInfo cui = typeInfo.GetCommandUpdater (commandId);
					if (cui != null) {
						if (cmd.CommandArray) {
							// Make sure that the option is still active
							CommandArrayInfo ainfo = new CommandArrayInfo (info);
							cui.Run (cmdTarget, ainfo);
							bool found = false;
							foreach (CommandInfo ci in ainfo) {
								if (Object.Equals (dataItem, ci.DataItem)) {
									found = true;
									break;
								}
							}
							if (!found) return false;
						} else {
							cui.Run (cmdTarget, info);
							if (!info.Enabled || !info.Visible) return false;
						}
					}
					
					CommandHandlerInfo chi = typeInfo.GetCommandHandler (commandId);
					if (chi != null) {
						if (cmd.CommandArray)
							chi.Run (cmdTarget, dataItem);
						else
							chi.Run (cmdTarget);
						UpdateToolbars ();
						return true;
					}
					
					cmdTarget = GetNextCommandTarget (cmdTarget, ref globalPos);
				}
	
				return cmd.DispatchCommand (dataItem);
			}
			catch (Exception ex) {
				string name = (cmd != null && cmd.Text != null && cmd.Text.Length > 0) ? cmd.Text : commandId.ToString ();
				name = name.Replace ("_","");
				ReportError (commandId, "Error while executing command: " + name, ex);
				return false;
			}
		}
		
		internal CommandInfo GetCommandInfo (object commandId, object initialTarget)
		{
			ActionCommand cmd = GetActionCommand (commandId);
			CommandInfo info = new CommandInfo (cmd);
			
			try {
				int globalPos = -1;
				object cmdTarget = initialTarget != null ? initialTarget : GetFirstCommandTarget (out globalPos);
				
				while (cmdTarget != null)
				{
					HandlerTypeInfo typeInfo = GetTypeHandlerInfo (cmdTarget);
					CommandUpdaterInfo cui = typeInfo.GetCommandUpdater (commandId);
					
					if (cui != null) {
						if (cmd.CommandArray) {
							info.ArrayInfo = new CommandArrayInfo (info);
							cui.Run (cmdTarget, info.ArrayInfo);
							return info;
						}
						else {
							cui.Run (cmdTarget, info);
							return info;
						}
					}
					
					if (typeInfo.GetCommandHandler (commandId) != null) {
						info.Enabled = true;
						info.Visible = true;
						return info;
					}
					
					cmdTarget = GetNextCommandTarget (cmdTarget, ref globalPos);
				}
				
				cmd.UpdateCommandInfo (info);
			}
			catch (Exception ex) {
				if (!commandUpdateErrors.Contains (commandId)) {
					commandUpdateErrors.Add (commandId);
					ReportError (commandId, "Error while updating status of command: " + commandId, ex);
				}
				info.Enabled = false;
				info.Visible = true;
			}
			return info;
		}
		
		internal ArrayList FindAlternateAccelCommands (object commandId)
		{
			return (ArrayList) overloadedAccelCommands [commandId];
		}
		
		internal bool DispatchCommandFromAccel (object commandId, object dataItem, object initialTarget)
		{
			// Dispatches a command that has been fired by an accelerator.
			// The difference from a normal dispatch is that there may
			// be several commands bound to the same accelerator, and in
			// this case it will execute the one that is enabled.
			
			// If the original key has been modified
			// by a CommandUpdate handler, it won't work. That's a limitation,
			// but checking all possible commands would be too slow.
			
			Command cmd = GetCommand (commandId);
			string accel = cmd.AccelKey;
			if (accel == null || accel == "")
				return DispatchCommand (commandId, dataItem, initialTarget);
			
			ArrayList list = (ArrayList) overloadedAccelCommands [accel];
			if (list == null)
				// The command is not overloaded, so it can be handled normally.
				return DispatchCommand (commandId, dataItem, initialTarget);
			
			// Get the accelerator used to fire the command and make sure it has not changed.
			CommandInfo accelInfo = GetCommandInfo (commandId, initialTarget);
			bool res = DispatchCommand (commandId, accelInfo.DataItem, initialTarget);

			// If the accelerator has changed, we can't handle overloading.
			if (res || accel != accelInfo.AccelKey)
				return res;
			
			// Execution failed. Now try to execute alternate commands
			// bound to the same key.
			
			foreach (object altId in list) {
				if (altId == commandId)	// already handled above.
					continue;
				CommandInfo cinfo = GetCommandInfo (altId, initialTarget);
				if (cinfo.AccelKey != accel)	// Key changed by a handler, just ignore the command.
					continue;
				if (DispatchCommand (altId, cinfo.DataItem, initialTarget))
					return true;
			}
			return false;
		}
		
		internal string GetAccelPath (string key)
		{
			string path = "<MonoDevelop>/MainWindow/" + key;
			if (!Gtk.AccelMap.LookupEntry (path, new Gtk.AccelKey()) ) {
				string[] keys = key.Split ('|');
				Gdk.ModifierType mod = 0;
				uint ckey = 0;
				foreach (string keyp in keys) {
					if (keyp == "Control") {
						mod |= Gdk.ModifierType.ControlMask;
					} else if (keyp == "Shift") {
						mod |= Gdk.ModifierType.ShiftMask;
					} else if (keyp == "Alt") {
						mod |= Gdk.ModifierType.Mod1Mask;
					} else {
						ckey = Gdk.Keyval.FromName (keyp);
					}
				}
				Gtk.AccelMap.AddEntry (path, ckey, mod);
			}
			return path;
		}
		
		void RootRealized (object sender, EventArgs e)
		{
			rootWidget.AddAccelGroup (AccelGroup);
		}
		
		internal Gtk.AccelGroup AccelGroup {
			get {
				if (accelGroup == null) {
					accelGroup = new Gtk.AccelGroup ();
				} 
				return accelGroup;
			}
		}
		
		HandlerTypeInfo GetTypeHandlerInfo (object cmdTarget)
		{
			HandlerTypeInfo typeInfo = (HandlerTypeInfo) handlerInfo [cmdTarget.GetType ()];
			if (typeInfo != null) return typeInfo;
			
			Type type = cmdTarget.GetType ();
			typeInfo = new HandlerTypeInfo ();
			
			ArrayList handlers = new ArrayList ();
			ArrayList updaters = new ArrayList ();
			
			MethodInfo[] methods = type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo method in methods) {
				object attr = Attribute.GetCustomAttribute (method, typeof(CommandHandlerAttribute));
				if (attr != null)
					handlers.Add (new CommandHandlerInfo (method, (CommandHandlerAttribute) attr));

				attr = Attribute.GetCustomAttribute (method, typeof(CommandUpdateHandlerAttribute));
				if (attr != null)
					updaters.Add (new CommandUpdaterInfo (method, (CommandUpdateHandlerAttribute) attr));
			}
			
			if (handlers.Count > 0)
				typeInfo.CommandHandlers = (CommandHandlerInfo[]) handlers.ToArray (typeof(CommandHandlerInfo)); 
			if (updaters.Count > 0)
				typeInfo.CommandUpdaters = (CommandUpdaterInfo[]) updaters.ToArray (typeof(CommandUpdaterInfo));
				 
			handlerInfo [type] = typeInfo;
			return typeInfo;
		}
		
		object GetFirstCommandTarget (out int globalPos)
		{
			object cmdTarget = GetActiveWidget (rootWidget);
			if (cmdTarget == null) {
				globalPos = 0;
				if (globalHandlers.Count == 0) return null;
				return globalHandlers [0];
			} else {
				globalPos = -1;
				return cmdTarget;
			}
		}
		
		object GetNextCommandTarget (object cmdTarget, ref int globalPos)
		{
			if (globalPos != -1) {
				if (++globalPos < globalHandlers.Count)
					return globalHandlers [globalPos];
				else
					return null;
			}
			
			if (cmdTarget is ICommandDelegatorRouter) {
				delegatorStack.Push (cmdTarget);
				cmdTarget = ((ICommandDelegatorRouter)cmdTarget).GetDelegatedCommandTarget ();
			}
			else if (cmdTarget is ICommandRouter)
				cmdTarget = ((ICommandRouter)cmdTarget).GetNextCommandTarget ();
			else if (cmdTarget is Gtk.Widget)
				cmdTarget = ((Gtk.Widget)cmdTarget).Parent;
			else
				cmdTarget = null;
			
			if (cmdTarget == null) {
				if (delegatorStack.Count > 0) {
					ICommandDelegatorRouter del = (ICommandDelegatorRouter) delegatorStack.Pop ();
					cmdTarget = del.GetNextCommandTarget ();
					if (cmdTarget != null)
						return cmdTarget;
				}
				if (globalHandlers.Count == 0) return null;
				globalPos = 0;
				return globalHandlers [0];
			} else
				return cmdTarget;
		}
		
		Gtk.Widget GetActiveWidget (Gtk.Window win)
		{
			Gtk.Window[] wins = Gtk.Window.ListToplevels ();
			
			foreach (Gtk.Window w in wins) {
				if (w.IsActive && w.Visible && w.Type == Gtk.WindowType.Toplevel) {
					win = w;
					break;
				}
			}
			
			if (win != null) {
				Gtk.Widget widget = win;
				while (widget is Gtk.Container) {
					Gtk.Widget child = ((Gtk.Container)widget).FocusChild;
					if (child != null)
						widget = child;
					else
						break;
				}
				return widget;
			}
			return win;
			
/*			Gtk.Widget widget = win;
			while (widget is Gtk.Container) {
				Gtk.Widget child = ((Gtk.Container)widget).FocusChild;
				if (child != null)
					widget = child;
				else
					break;
			}
			return widget;*/
		}
		
		bool UpdateStatus ()
		{
			if (!disposed)
				UpdateToolbars ();
			else
				toolbarUpdaterRunning = false;
				
			return toolbarUpdaterRunning;
		}
		
		internal void RegisterToolbar (CommandToolbar toolbar)
		{
			toolbars.Add (toolbar);
			if (enableToolbarUpdate && !toolbarUpdaterRunning) {
				GLib.Timeout.Add (500, new GLib.TimeoutHandler (UpdateStatus));
				toolbarUpdaterRunning = true;
			}
		}
		
		void UpdateToolbars ()
		{
			foreach (CommandToolbar toolbar in toolbars) {
				if (toolbar.Visible)
					toolbar.Update ();
			}
		}
		
		public void ReportError (object commandId, string message, Exception ex)
		{
			if (CommandError != null) {
				CommandErrorArgs args = new CommandErrorArgs (commandId, message, ex);
				CommandError (this, args);
			}
		}
		
		public event CommandErrorHandler CommandError;
	}
	
	internal class HandlerTypeInfo
	{
		public CommandHandlerInfo[] CommandHandlers;
		public CommandUpdaterInfo[] CommandUpdaters;
		
		public CommandHandlerInfo GetCommandHandler (object commandId)
		{
			if (CommandHandlers == null) return null;
			foreach (CommandHandlerInfo cui in CommandHandlers)
				if (cui.CommandId.Equals (commandId))
					return cui;
			return null;
		}
		
		public CommandUpdaterInfo GetCommandUpdater (object commandId)
		{
			if (CommandUpdaters == null) return null;
			foreach (CommandUpdaterInfo cui in CommandUpdaters)
				if (cui.CommandId.Equals (commandId))
					return cui;
			return null;
		}
	}

	
	internal class CommandMethodInfo
	{
		public object CommandId;
		protected MethodInfo Method;
		
		public CommandMethodInfo (MethodInfo method, CommandMethodAttribute attr)
		{
			this.Method = method;
			CommandId = attr.CommandId;
		}
	}
		
	internal class CommandUpdaterInfo: CommandMethodInfo
	{
		bool isArray;
		
		public CommandUpdaterInfo (MethodInfo method, CommandUpdateHandlerAttribute attr): base (method, attr)
		{
			ParameterInfo[] pars = method.GetParameters ();
			if (pars.Length == 1) {
				Type t = pars[0].ParameterType;
				
				if (t == typeof(CommandArrayInfo)) {
					isArray = true;
					return;
				} else if (t == typeof(CommandInfo))
					return;
			}
			throw new InvalidOperationException ("Invalid signature for command update handler: " + method.DeclaringType + "." + method.Name + "()");
		}
		
		public void Run (object cmdTarget, CommandInfo info)
		{
			if (isArray)
				throw new InvalidOperationException ("Invalid signature for command update handler: " + Method.DeclaringType + "." + Method.Name + "()");
			Method.Invoke (cmdTarget, new object[] {info} );
		}
		
		public void Run (object cmdTarget, CommandArrayInfo info)
		{
			if (!isArray)
				throw new InvalidOperationException ("Invalid signature for command update handler: " + Method.DeclaringType + "." + Method.Name + "()");
			Method.Invoke (cmdTarget, new object[] {info} );
		}
	}
	
	internal class CommandHandlerInfo: CommandMethodInfo
	{
		public CommandHandlerInfo (MethodInfo method, CommandHandlerAttribute attr): base (method, attr)
		{
			ParameterInfo[] pars = method.GetParameters ();
			if (pars.Length > 1)
				throw new InvalidOperationException ("Invalid signature for command handler: " + method.DeclaringType + "." + method.Name + "()");
		}
		
		public void Run (object cmdTarget)
		{
			Method.Invoke (cmdTarget, null);
		}
		
		public void Run (object cmdTarget, object dataItem)
		{
			Method.Invoke (cmdTarget, new object[] {dataItem});
		}
	}
}

