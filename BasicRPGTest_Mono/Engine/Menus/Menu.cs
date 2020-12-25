using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
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

        public void Draw(SpriteBatch batch, SpriteFont font, Rectangle area, Color color, Color highlightColor, Alignment textAlign = Alignment.Left, Alignment vertAlign = Alignment.Top, int lineSpacing = 5)
        {
            if (entries.Count == 0) return;

            batch.DrawRectangle(area, Color.White);

            int lineHeight = (int)font.MeasureString(entries[0].label).Y;

            Vector2 pos;
            Vector2 newPos;
            Vector2 textAnchor = new Vector2(-1, -1);

            Color textColor;

            int i;

            switch (textAlign)
            {
                case Alignment.Center:
                    foreach (MenuItem entry in entries)
                    {

                        int vertOffset = 0;
                        if (vertAlign == Alignment.Center)
                            vertOffset = (int)((area.Height / 2) + area.Y - ((lineHeight + lineSpacing) * ((entries.Count - 1) / 2.0)));
                        System.Diagnostics.Debug.WriteLine("Text area height: " + ((lineHeight + lineSpacing) * ((entries.Count - 1) / 2.0)));

                        if (textAnchor.X == -1 && textAnchor.Y == -1)
                            textAnchor = new Vector2(font.MeasureString(entry.label).X / 2, ((font.MeasureString(entry.label).Y / 2) + lineSpacing));
                        else 
                            textAnchor = new Vector2(font.MeasureString(entry.label).X / 2, textAnchor.Y - (lineHeight + lineSpacing));

                        if (entry == entries[index])
                            textColor = highlightColor;
                        else
                            textColor = color;

                        pos = new Vector2(area.X + (area.Width / 2) - textAnchor.X, vertOffset - textAnchor.Y);
                        batch.DrawString(font, entry.label, pos, textColor);

                    }
                    break;
                case Alignment.Left:
                    i = 0;
                    pos = new Vector2(area.X + lineSpacing, area.Y + lineSpacing);
                    foreach (MenuItem entry in entries)
                    {
                        if (entry.label == entries[index].label)
                            textColor = highlightColor;
                        else
                            textColor = color;

                        if (i != 0)
                            pos = new Vector2(pos.X, pos.Y + (lineHeight + lineSpacing));

                        batch.DrawString(font, entry.label, pos, textColor);
                        i++;
                    }
                    break;
                case Alignment.Right:
                    i = 0;
                    pos = new Vector2(area.X + area.Width - lineSpacing, area.Y + lineSpacing);
                    newPos = pos;
                    foreach (MenuItem entry in entries)
                    {
                        if (entry.label == entries[index].label)
                            textColor = highlightColor;
                        else
                            textColor = color;

                        if (i != 0)
                            newPos = new Vector2(pos.X, (newPos.Y + (lineHeight + lineSpacing)));
                        newPos = new Vector2(pos.X - (font.MeasureString(entry.label).X), newPos.Y);

                        batch.DrawString(font, entry.label, newPos, textColor);
                        i++;
                    }
                    break;
            }

        }

    }

    public enum Alignment
    {
        Left,
        Center,
        Right,
        Top,
        Bottom
    }
}
