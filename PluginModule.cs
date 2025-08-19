using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapIcons
{
    public abstract class PluginModule {
        protected Plugin Plugin { get; }
        protected Settings Settings => Plugin.Settings;
        protected ExileCore.GameController GameController => Plugin.GameController;
        protected ExileCore.Graphics Graphics => Plugin.Graphics;

        protected PluginModule(Plugin plugin) {
            Plugin = plugin;
        }

    }
}
