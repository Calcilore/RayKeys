using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RayKeys.Misc;
using RayKeys.Render;
using TextCopy;

namespace RayKeys.UI {
    public class InputField : FocusableMenuItem {
        private int fontSize;
        private int sizeX;
        private int sizeY;

        public delegate void EnterEventD(int id, string text, params object[] args);
        public event EnterEventD EnterEvent;

        public object[] args;
        public string Label;
        
        public string Text = "";
        public int cursorPos;
        private float cursorTimer = 0f;
        private const float CursorTimerMax = 0.4f;

        public InputField(Menu parent, bool followCamera, Align h, Align v, int id, string label, int x, int y, int sizeX = 600, int sizeY = 200, int fontSize = 4) {
            Game1.Game.UpdateEvent += Update;
            Game1.Game.Window.TextInput += TextInput;

            this.parent = parent;
            this.followCamera = followCamera;
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.fontSize = fontSize;
            this.Label = label;
            this.Id = id;
            Alh = h;
            Alv = v;

            tPos.X = x - sizeX / 2; // idk why it needs this
            tPos.Y = y;
            
            tPosFoc = tPos + new Vector2(h == Align.Right ? -64 : 64, 0);
            pos = tPos;
        }

        private void Update(float delta) {
            if (IsFocused) {
                cursorTimer -= delta;
                if (cursorTimer < -CursorTimerMax)
                    cursorTimer = CursorTimerMax;
                
                if (RKeyboard.IsKeyPressed(Keys.Enter)) {
                    EnterEvent?.Invoke(Id, Text, args);
                }

                foreach (Keys k in RKeyboard.PressedKeys) {
                    switch (k) {
                        case Keys.Left:
                            if (cursorPos <= 0) break;
                            
                            AddCursorPos(-1);
                            break;
                        
                        case Keys.Right:
                            if (cursorPos >= Text.Length) break;
                            
                            AddCursorPos(1);
                            break;
                        
                        case Keys.Home:
                            SetCursorPos(0);
                            break;
                        
                        case Keys.End:
                            SetCursorPos(Text.Length);
                            break;
                        
                        case Keys.V:
                            if (RKeyboard.IsKeyHeld(Keys.LeftControl)) {
                                // Paste
                                string clipboard = RKeyboard.GetClipboard();
                                Logger.Debug("Clipboard Is: " + clipboard);

                                if (clipboard != null) {
                                    Text = Text.Insert(cursorPos, clipboard);
                                    AddCursorPos(clipboard.Length);
                                }
                            }

                            break;
                    }
                }
            }
            else {
                cursorTimer = CursorTimerMax;
            }
        }
        
        private void TextInput(object a, TextInputEventArgs arg) {
            if (!IsFocused) return;
            if (arg.Key == Keys.Enter) return;
                
            if (arg.Character != '	' && (char.IsLetterOrDigit(arg.Character) || char.IsPunctuation(arg.Character) || char.IsSymbol(arg.Character) || char.IsWhiteSpace(arg.Character))) {
                Text = Text.Insert(cursorPos, arg.Character.ToString());
                AddCursorPos(1);
            }
            else {
                switch (arg.Key) {
                    case Keys.Back:
                        if (cursorPos <= 0) break;

                        if (RKeyboard.IsKeyHeld(Keys.LeftControl)) {
                            int spaceIndex = Text.LastIndexOf(' ', cursorPos - 1);
                            if (spaceIndex == -1) spaceIndex = 0;

                            Text = Text[..spaceIndex] + Text[cursorPos..];
                            AddCursorPos(spaceIndex - cursorPos);
                        }
                        else {
                            Text = Text.Remove(cursorPos - 1, 1);
                            AddCursorPos(-1);
                        }
                        break;
                        
                    case Keys.Delete:
                        if (cursorPos >= Text.Length) break;
                            
                        if (RKeyboard.IsKeyHeld(Keys.LeftControl)) {
                            int spaceIndex = Text.IndexOf(' ', cursorPos) + 1;
                            if (spaceIndex == 0) {
                                spaceIndex = Text.Length;
                            }

                            Text = Text[..cursorPos] + Text[spaceIndex..];
                        }
                        else {
                            Text = Text.Remove(cursorPos, 1);
                        }
                        break;
                }
            }
        }

        protected override void Draw(float delta) {
            base.Draw(delta);
            
            pos.X = ThingTools.Lerp(pos.X, IsFocused ? tPosFoc.X : tPos.X, 10 * delta);
            Point finalPos = pos.ToPoint();
            finalPos.X += sizeX / 2;
            finalPos.Y += sizeY / 2;
            if (followCamera) finalPos += RRender.CameraPos.ToPoint();
            
            RRender.DrawString(Alh, Alv, Align.Left, Align.Top, Label, finalPos.X, finalPos.Y, fontSize);
            RRender.DrawString(Alh, Alv, Align.Left, Align.Top, Text, finalPos.X, finalPos.Y + 50, fontSize);
            RRender.DrawBlank(Alh, Alv, finalPos.X, finalPos.Y + 100, sizeX, 6, Color.White);
            if (IsFocused && cursorTimer > 0) RRender.DrawBlank(Alh, Alv, finalPos.X + (int)RRender.MeasureString(fontSize, Text[..cursorPos]).X, finalPos.Y + 90, 40, 6, Color.White);
        }

        protected override Vector2 CalculateOffset() {
            // int hah = AlvT switch {
            //     Align.Center => 0,
            //     Align.Bottom => Stuffs.GetTexture(Textures.Arrow).Height,
            //     /* Top */ _  => Stuffs.GetTexture(Textures.Arrow).Height / 2
            // };
            
            int tLen = (int) RRender.MeasureString(fontSize, Label).X;

            return Alh switch { // I dont understand these magical numbers for the x axis
                Align.Left  => new Vector2(tLen + 96, 128),
                Align.Right => new Vector2(-tLen + 416, 128),
                /*Center*/_ => new Vector2(-tLen / 2f + 256, 128),
            };
        }
        
        private void SetCursorPos(int value) {
            cursorPos = value;
            cursorTimer = CursorTimerMax;
        }
        
        private void AddCursorPos(int value) {
            cursorPos += value;
            cursorTimer = CursorTimerMax;
        }

        public override void UnFocus() {
            EnterEvent?.Invoke(Id, Text, args);
            base.UnFocus();
        }
    }
}