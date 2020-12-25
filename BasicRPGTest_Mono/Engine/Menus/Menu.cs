using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BasicRPGTest_Mono.Engine.Menus
{
    public class Menu
    {
        public string menuLabel;
        public List<MenuItem> entries;

        private int _index;
        public int index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value >= entries.Count) return;
                if (value < 0) return;
                _index = value;
            }
        }

        public Menu(string label)
        {
            menuLabel = label;
            entries = new List<MenuItem>();
        }

        public void add(MenuItem entry)
        {
            entries.Add(entry);
        }
        public void remove(int index)
        {
            entries.Remove(entries[index]);
        }
        public void select()
        {
            entries[index].run();
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {

        }

    }
}
