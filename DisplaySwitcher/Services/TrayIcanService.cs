using System;
using System.Collections.Generic;
using DisplaySwitcher.Models;
using Forms = System.Windows.Forms;

namespace DisplaySwitcher.Services
{
    public class TrayIconService
    {
        private readonly Forms.NotifyIcon _notifyIcon;
        private readonly IEnumerable<DisplayProfile> _profiles;

        public event Action? OpenRequested;
        public event Action? ManageProfilesRequested;
        public event Action<DisplayProfile>? ProfileRequested;

        public TrayIconService(IEnumerable<DisplayProfile> profiles)
        {
            _profiles = profiles;

            _notifyIcon = new Forms.NotifyIcon
            {
                Icon = System.Drawing.SystemIcons.Application,
                Text = "DisplaySwitcher",
                Visible = true
            };

            RefreshContextMenu();
        }

        public void RefreshContextMenu()
        {
            _notifyIcon.ContextMenuStrip = CreateContextMenu();
        }

        private Forms.ContextMenuStrip CreateContextMenu()
        {
            Forms.ContextMenuStrip menu = new Forms.ContextMenuStrip();

            foreach (DisplayProfile profile in _profiles)
            {
                Forms.ToolStripMenuItem profileItem =
                    new Forms.ToolStripMenuItem(profile.Name);

                profileItem.Checked = profile.IsSelected;

                profileItem.Click += (s, e) =>
                {
                    ProfileRequested?.Invoke(profile);
                };

                menu.Items.Add(profileItem);
            }

            menu.Items.Add(new Forms.ToolStripSeparator());

            Forms.ToolStripMenuItem manageProfilesItem =
                new Forms.ToolStripMenuItem("Gérer les profils");

            manageProfilesItem.Click += (s, e) =>
            {
                ManageProfilesRequested?.Invoke();
            };

            menu.Items.Add(manageProfilesItem);

            menu.Items.Add(new Forms.ToolStripSeparator());

            Forms.ToolStripMenuItem openItem =
                new Forms.ToolStripMenuItem("Ouvrir DisplaySwitcher");

            openItem.Click += (s, e) =>
            {
                OpenRequested?.Invoke();
            };

            menu.Items.Add(openItem);

            Forms.ToolStripMenuItem quitItem =
                new Forms.ToolStripMenuItem("Quitter");

            quitItem.Click += (s, e) =>
            {
                System.Windows.Application.Current.Shutdown();
            };

            menu.Items.Add(quitItem);

            return menu;
        }
    }
}