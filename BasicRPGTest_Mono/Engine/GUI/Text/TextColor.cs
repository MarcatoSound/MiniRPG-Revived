using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.Text
{
    public class TextColor : IDisposable
    {
        public Color color;
        public byte alpha;
        private List<Color> colors;

        public Color startColor;
        public Color activeColor;
        public Color nextColor;
        public float rate;
        public float status;

        private int _currentStep;
        private int totalSteps;
        private int currentStep
        {
            get { return _currentStep; }
            set
            {
                if (value < totalSteps)
                {
                    _currentStep = value;

                    if (value == totalSteps - 1)
                    {
                        nextColor = colors[0];
                    }
                    else
                    {
                        nextColor = colors[currentStep + 1];
                    }
                }
                else
                {
                    _currentStep = 0;

                    if (colors.Count != 1)
                        nextColor = colors[currentStep + 1];
                    else
                        nextColor = colors[0];
                }

                activeColor = colors[currentStep];
                //System.Diagnostics.Debug.WriteLine($"Active color is: {activeColor}");
                //System.Diagnostics.Debug.WriteLine($"Next color is: {nextColor}");

            }
        }

        public TextColor(Color color)
        {
            this.colors = new List<Color>();
            colors.Add(color);

            totalSteps = 1;
            currentStep = 0;

            this.color = activeColor;
        }
        public TextColor(Color startColor, Color secondColor, float rate)
        {
            this.colors = new List<Color>();
            colors.Add(startColor);
            colors.Add(secondColor);

            totalSteps = colors.Count; 
            currentStep = 0;

            this.color = activeColor;
            this.rate = rate;
        }
        public TextColor(Color[] colors, float rate)
        {
            // TODO: This will break if there's only one color provided.
            this.colors = new List<Color>(colors);
            totalSteps = this.colors.Count;
            currentStep = 0;

            this.color = activeColor;
            this.rate = rate;
        }


        public void update()
        {
            if (rate == 0) return;
            if (status > 1)
            {
                currentStep++;
                status -= 1;
            }

            color = Color.Lerp(activeColor, nextColor, status);

            status += rate;

        }


        public void Dispose() { }

        public static implicit operator Color(TextColor tc) => tc.color;
        public static implicit operator TextColor(Color color) => new TextColor(color);

    }
}
