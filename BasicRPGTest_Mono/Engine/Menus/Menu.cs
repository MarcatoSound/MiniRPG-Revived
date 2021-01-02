using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace BasicRPGTest_Mono.Engine.Menus
{
    public class Menu
    {
        public string menuLabel;
        public List<MenuItem> entries;
        public Rectangle box;
        public SpriteFont font;
        public Color textColor;
        public Color highlightColor;
        public float lineHeight;
        public int padding;
        public int start;
        public int maxDisplay;

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

                if (value + 1 > maxDisplay && value > _index)
                    start++;
                if (value < start)
                    start = value;

                _index = value;
            }
        }
        ///<summary>
        ///A menu with a selectable list of strings. USE  run = () => { /* CODE HERE */ }
        ///To add unique code to the menu selection.
        ///</summary>
        public Menu(string label, Rectangle rect, Color textColor, Color highlightColor, SpriteFont font, int padding = 5)
        {
            menuLabel = label;
            entries = new List<MenuItem>();
            box = rect;
            this.textColor = textColor;
            this.highlightColor = highlightColor;
            this.font = font;
            this.padding = padding;
            lineHeight = font.MeasureString("?").Y;
            start = 0;
            maxDisplay = (int)(box.Height / lineHeight);
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

        public void Draw(SpriteBatch batch, Alignment textAlign = Alignment.Left, Alignment vertAlign = Alignment.Top)
        {
            

            if (entries.Count == 0) return;

            batch.DrawRectangle(box, Color.White);

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

                    if (vertAlign == Alignment.Top)
                    {
                        vertOffset = box.Top;
                        pos = new Vector2(0, vertOffset + padding);
                    }
                    if (vertAlign == Alignment.Center)
                    {
                        vertOffset = (int)(box.Top + (box.Height / 2) - (lineHeight * (entries.Count / 2.0)));
                        pos = new Vector2(0, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(box.Bottom - (lineHeight * entries.Count));
                        pos = new Vector2(0, vertOffset - padding);
                    }

                    for (int z = start; z < maxDisplay+start; z++)
                    {
                        if (entries.Count-1 < z) break;
                        MenuItem entry = entries[z];

                        if (textAnchor.X == -1 && textAnchor.Y == -1)
                            textAnchor = new Vector2(font.MeasureString(entry.label).X / 2, (font.MeasureString(entry.label).Y / 2));
                        else 
                            textAnchor = new Vector2(font.MeasureString(entry.label).X / 2, lineHeight);

                        if (z == index)
                            textColor = highlightColor;
                        else
                            textColor = this.textColor;

                        if (i != 0)
                            pos = new Vector2(box.Left + (box.Width / 2) - textAnchor.X, pos.Y + textAnchor.Y);
                        else
                            pos = new Vector2(box.Left + (box.Width / 2) - textAnchor.X, pos.Y);

                        batch.DrawString(font, entry.label, pos, textColor);

                        i++;
                    }
                    break;
                case Alignment.Left:
                    i = 0;

                    pos = new Vector2(0, vertOffset + padding);

                    if (vertAlign == Alignment.Top)
                    {
                        vertOffset = box.Top;
                        pos = new Vector2(box.Left + padding, vertOffset + padding);
                    }
                    if (vertAlign == Alignment.Center)
                    {
                        vertOffset = (int)(box.Top + (box.Height / 2) - (lineHeight * (entries.Count / 2.0)));
                        pos = new Vector2(box.Left + padding, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(box.Bottom - (lineHeight * entries.Count));
                        pos = new Vector2(box.Left + padding, vertOffset - padding);
                    }

                    for (int z = start; z < maxDisplay + start; z++)
                    {
                        if (entries.Count - 1 < z) break;
                        MenuItem entry = entries[z];

                        if (z == index)
                            textColor = highlightColor;
                        else
                            textColor = this.textColor;

                        if (i != 0)
                            pos = new Vector2(pos.X, pos.Y + lineHeight);

                        batch.DrawString(font, entry.label, pos, textColor);
                        i++;
                    }
                    break;
                case Alignment.Right:
                    i = 0;

                    pos = new Vector2(0, vertOffset + padding);

                    if (vertAlign == Alignment.Top)
                    {
                        vertOffset = box.Top;
                        pos = new Vector2(box.Right - padding, vertOffset + padding);
                    }
                    if (vertAlign == Alignment.Center)
                    {
                        vertOffset = (int)(box.Top + (box.Height / 2) - (lineHeight * (entries.Count / 2.0)));
                        pos = new Vector2(box.Right - padding, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(box.Bottom - (lineHeight * entries.Count));
                        pos = new Vector2(box.Right - padding, vertOffset - padding);
                    }

                    newPos = pos;
                    for (int z = start; z < maxDisplay + start; z++)
                    {
                        if (entries.Count - 1 < z) break;
                        MenuItem entry = entries[z];

                        if (z == index)
                            textColor = highlightColor;
                        else
                            textColor = this.textColor;

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
