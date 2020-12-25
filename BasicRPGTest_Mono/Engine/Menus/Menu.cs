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

        public void Draw(SpriteBatch batch, SpriteFont font, Rectangle area, Color color, Color highlightColor, Alignment textAlign = Alignment.Left, Alignment vertAlign = Alignment.Top, int padding = 5)
        {
            if (entries.Count == 0) return;

            batch.DrawRectangle(area, Color.White);

            int lineHeight = (int)font.MeasureString(entries[0].label).Y;

            Vector2 pos;
            Vector2 newPos;
            Vector2 textAnchor = new Vector2(-1, -1);

            Color textColor;

            int i;
            int vertOffset = 0;

            switch (textAlign)
            {
                case Alignment.Center:
                    i = 0;

                    pos = new Vector2(0, vertOffset + padding);

                    vertOffset = 0;
                    if (vertAlign == Alignment.Top)
                    {
                        vertOffset = area.Top;
                        pos = new Vector2(0, vertOffset + padding);
                    }
                    if (vertAlign == Alignment.Center)
                    {
                        vertOffset = (int)(area.Top + (area.Height / 2) - (lineHeight * (entries.Count / 2.0)));
                        pos = new Vector2(0, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(area.Bottom - (lineHeight * entries.Count));
                        pos = new Vector2(0, vertOffset - padding);
                    }

                    foreach (MenuItem entry in entries)
                    {

                        //System.Diagnostics.Debug.WriteLine("Text area height: " + ((lineHeight + lineSpacing) * ((entries.Count - 1) / 2.0)));

                        if (textAnchor.X == -1 && textAnchor.Y == -1)
                            textAnchor = new Vector2(font.MeasureString(entry.label).X / 2, (font.MeasureString(entry.label).Y / 2));
                        else 
                            textAnchor = new Vector2(font.MeasureString(entry.label).X / 2, lineHeight);

                        if (entry == entries[index])
                            textColor = highlightColor;
                        else
                            textColor = color;

                        if (i != 0)
                            pos = new Vector2(area.Left + (area.Width / 2) - textAnchor.X, pos.Y + textAnchor.Y);
                        else
                            pos = new Vector2(area.Left + (area.Width / 2) - textAnchor.X, pos.Y);
                        batch.DrawString(font, entry.label, pos, textColor);

                        i++;
                    }
                    break;
                case Alignment.Left:
                    i = 0;

                    pos = new Vector2(0, vertOffset + padding);

                    vertOffset = 0;
                    if (vertAlign == Alignment.Top)
                    {
                        vertOffset = area.Top;
                        pos = new Vector2(area.Left + padding, vertOffset + padding);
                    }
                    if (vertAlign == Alignment.Center)
                    {
                        vertOffset = (int)(area.Top + (area.Height / 2) - (lineHeight * (entries.Count / 2.0)));
                        pos = new Vector2(area.Left + padding, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(area.Bottom - (lineHeight * entries.Count));
                        pos = new Vector2(area.Left + padding, vertOffset - padding);
                    }

                    foreach (MenuItem entry in entries)
                    {

                        if (entry.label == entries[index].label)
                            textColor = highlightColor;
                        else
                            textColor = color;

                        if (i != 0)
                            pos = new Vector2(pos.X, pos.Y + lineHeight);

                        batch.DrawString(font, entry.label, pos, textColor);
                        i++;
                    }
                    break;
                case Alignment.Right:
                    i = 0;

                    pos = new Vector2(0, vertOffset + padding);

                    vertOffset = 0;
                    if (vertAlign == Alignment.Top)
                    {
                        vertOffset = area.Top;
                        pos = new Vector2(area.Right - padding, vertOffset + padding);
                    }
                    if (vertAlign == Alignment.Center)
                    {
                        vertOffset = (int)(area.Top + (area.Height / 2) - (lineHeight * (entries.Count / 2.0)));
                        pos = new Vector2(area.Right - padding, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(area.Bottom - (lineHeight * entries.Count));
                        pos = new Vector2(area.Right - padding, vertOffset - padding);
                    }

                    newPos = pos;
                    foreach (MenuItem entry in entries)
                    {

                        if (entry.label == entries[index].label)
                            textColor = highlightColor;
                        else
                            textColor = color;

                        if (i != 0)
                            newPos = new Vector2(pos.X, (newPos.Y + lineHeight));
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
