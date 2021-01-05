using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BasicRPGTest_Mono.Engine.Utility
{
    public static class Util
    {
        /// <summary>
        /// Tries to convert keyboard input to characters and prevents repeatedly returning the 
        /// same character if a key was pressed last frame, but not yet unpressed this frame.
        /// </summary>
        /// <param name="keyboard">The current KeyboardState</param>
        /// <param name="oldKeyboard">The KeyboardState of the previous frame</param>
        /// <param name="key">When this method returns, contains the correct character if conversion succeeded.
        /// Else contains the null, (000), character.</param>
        /// <returns>True if conversion was successful</returns>
        public static bool TryConvertKeyboardInput(KeyboardState keyboard, KeyboardState oldKeyboard, out char key)
        {
            Keys[] keys = keyboard.GetPressedKeys();
            bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);

            if (keys.Length > 0 && !oldKeyboard.IsKeyDown(keys[0]))
            {
                switch (keys[0])
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = '('; } else { key = '9'; } return true;

                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;

                    //Special keys
                    case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } return true;
                    case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } return true;
                    case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } return true;
                    case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } return true;
                    case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } return true;
                    case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } return true;
                    case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } return true;
                    case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } return true;
                    case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } return true;
                    case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return true;
                    case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; } return true;
                    case Keys.Space: key = ' '; return true;
                }
            }

            key = (char)0;
            return false;
        }

        public static char getCharacter(KeyboardState state, Keys pressed)
        {

            bool shift = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
            char key;
            switch (pressed)
            {
                //Alphabet keys
                case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return key;
                case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return key;
                case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return key;
                case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return key;
                case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return key;
                case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return key;
                case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return key;
                case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return key;
                case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return key;
                case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return key;
                case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return key;
                case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return key;
                case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return key;
                case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return key;
                case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return key;
                case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return key;
                case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return key;
                case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return key;
                case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return key;
                case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return key;
                case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return key;
                case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return key;
                case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return key;
                case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return key;
                case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return key;
                case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return key;

                //Decimal keys
                case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } return key;
                case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return key;
                case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } return key;
                case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } return key;
                case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return key;
                case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return key;
                case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } return key;
                case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } return key;
                case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } return key;
                case Keys.D9: if (shift) { key = '('; } else { key = '9'; } return key;

                //Decimal numpad keys
                case Keys.NumPad0: key = '0'; return key;
                case Keys.NumPad1: key = '1'; return key;
                case Keys.NumPad2: key = '2'; return key;
                case Keys.NumPad3: key = '3'; return key;
                case Keys.NumPad4: key = '4'; return key;
                case Keys.NumPad5: key = '5'; return key;
                case Keys.NumPad6: key = '6'; return key;
                case Keys.NumPad7: key = '7'; return key;
                case Keys.NumPad8: key = '8'; return key;
                case Keys.NumPad9: key = '9'; return key;

                //Special keys
                case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } return key;
                case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } return key;
                case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } return key;
                case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } return key;
                case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } return key;
                case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } return key;
                case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } return key;
                case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } return key;
                case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } return key;
                case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return key;
                case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; } return key;
                case Keys.Space: key = ' '; return key;
            }

            key = (char)0;
            return key;
        }

        public static void drawAlignedText(List<string> str, Color color, SpriteBatch batch, SpriteFont font, Rectangle box, Alignment horAlign = Alignment.Left, Alignment vertAlign = Alignment.Top, int padding = 5)
        {

            int lineHeight = (int)font.MeasureString(str[0]).Y;

            Vector2 pos;
            Vector2 newPos;
            Vector2 textAnchor = new Vector2(-1, -1);

            int i;
            int vertOffset = 0;

            switch (horAlign)
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
                        vertOffset = (int)(box.Top + (box.Height / 2) - (lineHeight * (str.Count / 2.0)));
                        pos = new Vector2(0, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(box.Bottom - (lineHeight * str.Count));
                        pos = new Vector2(0, vertOffset - padding);
                    }

                    foreach (string entry in str)
                    {

                        //System.Diagnostics.Debug.WriteLine("Text box height: " + ((lineHeight + lineSpacing) * ((entries.Count - 1) / 2.0)));

                        if (textAnchor.X == -1 && textAnchor.Y == -1)
                            textAnchor = new Vector2(font.MeasureString(entry).X / 2, (font.MeasureString(entry).Y / 2));
                        else
                            textAnchor = new Vector2(font.MeasureString(entry).X / 2, lineHeight);


                        if (i != 0)
                            pos = new Vector2(box.Left + (box.Width / 2) - textAnchor.X, pos.Y + textAnchor.Y);
                        else
                            pos = new Vector2(box.Left + (box.Width / 2) - textAnchor.X, pos.Y);
                        batch.DrawString(font, entry, pos, color);

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
                        vertOffset = (int)(box.Top + (box.Height / 2) - (lineHeight * (str.Count / 2.0)));
                        pos = new Vector2(box.Left + padding, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(box.Bottom - (lineHeight * str.Count));
                        pos = new Vector2(box.Left + padding, vertOffset - padding);
                    }

                    foreach (string entry in str)
                    {

                        if (i != 0)
                            pos = new Vector2(pos.X, pos.Y + lineHeight);

                        batch.DrawString(font, entry, pos, color);
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
                        vertOffset = (int)(box.Top + (box.Height / 2) - (lineHeight * (str.Count / 2.0)));
                        pos = new Vector2(box.Right - padding, vertOffset);
                    }
                    // TODO Bottom align is broken.
                    if (vertAlign == Alignment.Bottom)
                    {
                        vertOffset = (int)(box.Bottom - (lineHeight * str.Count));
                        pos = new Vector2(box.Right - padding, vertOffset - padding);
                    }

                    newPos = pos;
                    foreach (string entry in str)
                    {

                        if (i != 0)
                            newPos = new Vector2(pos.X, (newPos.Y + lineHeight));
                        newPos = new Vector2(pos.X - (font.MeasureString(entry).X), newPos.Y);

                        batch.DrawString(font, entry, newPos, color);
                        i++;
                    }
                    break;
            }
        }
        public static void drawAlignedText(string str, Color color, SpriteBatch batch, SpriteFont font, Rectangle box, Alignment horAlign = Alignment.Left, Alignment vertAlign = Alignment.Top, int padding = 5)
        {
            List<string> strings = new List<string>();
            strings.Add(str);
            drawAlignedText(strings, color, batch, font, box, horAlign, vertAlign, padding);
        }


        public static int weightedRandom(List<int> chances)
        {
            int totalRatio = 0;

            foreach (int c in chances)
                totalRatio += c;

            Random random = new Random();
            int x = random.Next(0, totalRatio);

            int iteration = 0; // so you know what to do next
            foreach (int c in chances)
            {
                iteration++;
                if ((x -= c) < 0)
                    break;
            }

            return iteration;

        }
        public static Spawn randomizeSpawn(ConcurrentDictionary<int, Spawn> spawns)
        {
            int totalRatio = 0;

            foreach (Spawn s in spawns.Values)
                totalRatio += s.weight;

            Random random = new Random();
            int x = random.Next(0, totalRatio);

            int iteration = 0; // so you know what to do next
            foreach (Spawn s in spawns.Values)
            {
                if ((x -= s.weight) < 0)
                    break;
                iteration++;
            }

            return spawns[iteration];

        }

        public static Vector2 getTilePosition(Vector2 truePos)
        {
            Vector2 pos = new Vector2(truePos.X, truePos.Y);

            pos.X = (int)pos.X / TileManager.dimensions;
            pos.Y = (int)pos.Y / TileManager.dimensions;

            return pos;
        }

        public static Texture2D getSpriteFromSet(Texture2D spriteset, int row, int column, int dimensions = 32)
        {

            int x = column * dimensions;
            int y = row * dimensions;

            Rectangle sourceRectangle = new Rectangle(x, y, dimensions, dimensions);

            Texture2D cropTexture = new Texture2D(Core.graphics, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            spriteset.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }
        public static Texture2D getSpriteFromSet(Texture2D spriteset, int row, int column, int width, int height)
        {

            int x = column * width;
            int y = row * height;

            Rectangle sourceRectangle = new Rectangle(x, y, width, height);

            Texture2D cropTexture = new Texture2D(Core.graphics, sourceRectangle.Width, sourceRectangle.Height);
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            spriteset.GetData(0, sourceRectangle, data, 0, data.Length);
            cropTexture.SetData(data);

            return cropTexture;
        }

        public static Texture2D buildWindowTexture(Rectangle box, Texture2D spriteset, int dimensions = 16)
        {

            if (box.Width / dimensions % 1 == 0)
                throw new InvalidOperationException("GUI Window dimensions must be a multiple of " + dimensions + ": " + box.Width / dimensions);

            int tileWidth = box.Width / dimensions;
            int tileHeight = box.Height / dimensions;

            SpriteBatch batch = new SpriteBatch(Core.graphics);

            Vector2 pos = new Vector2();
            Matrix drawingMatrix = Matrix.CreateTranslation(0, 0, 0);
            batch.Begin(transformMatrix: drawingMatrix);

            Texture2D topLeftCornerTexture = getSpriteFromSet(spriteset, 0, 0, dimensions);
            Texture2D topCenterTexture = getSpriteFromSet(spriteset, 0, 1, dimensions);
            Texture2D topRightCornerTexture = getSpriteFromSet(spriteset, 0, 2, dimensions);

            Texture2D midLeftTexture = getSpriteFromSet(spriteset, 1, 0, dimensions);
            Texture2D midCenterTexture = getSpriteFromSet(spriteset, 1, 1, dimensions);
            Texture2D midRightTexture = getSpriteFromSet(spriteset, 1, 2, dimensions);

            Texture2D bottomLeftTexture = getSpriteFromSet(spriteset, 2, 0, dimensions);
            Texture2D bottomCenterTexture = getSpriteFromSet(spriteset, 2, 1, dimensions);
            Texture2D bottomRightTexture = getSpriteFromSet(spriteset, 2, 2, dimensions);

            for (int row = 0; row <= tileHeight; row++)
            {
                for (int column = 0; column <= tileWidth; column++)
                {
                    if (row == 0)
                    {
                        // Draw the top-left-corner tile
                        if (column == 0)
                        {
                            batch.Draw(topLeftCornerTexture, pos, null, Color.White);
                            pos.X += dimensions;
                            continue;
                        }

                        // Draw the top-right-corner tile
                        if (column == tileWidth)
                        {
                            batch.Draw(topRightCornerTexture, pos, null, Color.White);
                            pos.X = 0;
                            continue;
                        }

                        // Draw the top-center tiles
                        batch.Draw(topCenterTexture, pos, null, Color.White);
                        pos.X += dimensions;

                        continue;
                    }

                    if (row == tileHeight)
                    {
                        // Draw the bottom-left-corner tile
                        if (column == 0)
                        {
                            batch.Draw(bottomLeftTexture, pos, null, Color.White);
                            pos.X += dimensions;
                            continue;
                        }

                        // Draw the bottom-right-corner tile
                        if (column == tileWidth)
                        {
                            batch.Draw(bottomRightTexture, pos, null, Color.White);
                            pos.X = 0;
                            continue;
                        }

                        // Draw the bottom-center tiles
                        batch.Draw(bottomCenterTexture, pos, null, Color.White);
                        pos.X += dimensions;

                        continue;
                    }

                    // Draw the middle-left tile
                    if (column == 0)
                    {
                        batch.Draw(midLeftTexture, pos, null, Color.White);
                        pos.X += dimensions;
                        continue;
                    }

                    // Draw the middle-right tile
                    if (column == tileWidth)
                    {
                        batch.Draw(midRightTexture, pos, null, Color.White);
                        pos.X = 0;
                        continue;
                    }

                    // Draw the middle-center tiles
                    batch.Draw(midCenterTexture, pos, null, Color.White);
                    pos.X += dimensions;

                }

                pos.Y += dimensions;

            }

            batch.End();



            Texture2D window = new Texture2D(Core.graphics, box.Width, box.Height);
            Color[] data = new Color[box.Width * box.Height];

            Texture2D tile;
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    tile = getSpriteFromSet(spriteset, row, column, dimensions);


                }
            }


            return window;
        }

        public static string cleanString(string str)
        {
            str = Regex.Replace(str, "[!@#$%^&*();:'\",.<>/?[\\]{}\\-+_=|\\s]", "");

            return str;
        }


        //----------------------------------------------------------------------------------
        // Handy Debug Message Wrappers
        //----------------------------------------------------------------------------------
        public static void myDebug(string mMessage)
        {
            // Gives basic Debug Message
            System.Diagnostics.Debug.WriteLine(mMessage);
        }

        public static void myDebug(string mSource, string mMessage)
        {
            // Gives a Debug mMessage with given mSource String at beginning.
            System.Diagnostics.Debug.WriteLine("<" + mSource + ">:  " + mMessage);
        }

        public static void myDebug(bool mError, string mSource, string mMessage)
        {
            // Adds an ERROR warning to beginning of Debug mMessage if mError = True
            if (mError == true)
            {
                // Gives Error message at start of mSource and Debug mMessage
                System.Diagnostics.Debug.WriteLine("! ERROR:  <" + mSource + ">:  " + mMessage);

            }
            else
            {
                // Gives a Debug mMessage with given mSource String at beginning.
                System.Diagnostics.Debug.WriteLine("<" + mSource + ">:  " + mMessage);
            }
        }


    }
}
